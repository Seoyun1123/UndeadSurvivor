using UnityEngine;

public class Slime : MonoBehaviour
{
    public float attackRange = 5f; // 💡 공격 사거리
    public float shootInterval = 3f; // 💡 발사 간격
    float timer; // 👈 쓋! 이 변수가 없어서 빨간 줄이 뜬 것이었습니다!
    [Header("슬라임 전용 스펙")]
    public float speed = 0.4f;       // 흙슬라임 전용 속도 (기존 Enemy보다 천천히)
    public float health = 200f;      // 흙슬라임 전용 체력
    public float maxHealth = 200f;
    public float damage = 10f;


    [Header("몬스터 타입 설정")]
    public bool canMove = true;
    public bool useAttackAnimation = false;
    
    

    [Header("원거리 공격")]
    public int bulletPoolIndex = 5;  // PoolManager의 적 총알 인덱스
    private float attackTimer;
    private float nextAttackDelay;

    // --- 아래는 기존 Enemy.cs에서 쓰던 변수와 컴포넌트 그대로 가져옴 ---
    private bool isLive = true;
    private Rigidbody2D rigid;
    private Collider2D coll;
    private SpriteRenderer spriter;
    private Transform target;
    Animator anim;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        isLive = true;
        health = maxHealth;
        if (coll != null) coll.enabled = true;
        if (rigid != null) rigid.simulated = true;

        if (GameManager.instance != null && GameManager.instance.player != null)
            target = GameManager.instance.player.transform;

        // 원거리 공격 쿨타임 초기화 (2~3초 랜덤)
        attackTimer = 0f;
        nextAttackDelay = Random.Range(2.0f, 3.0f);
    }

 

void Update()
{
    if (!isLive || target == null) return;

    // 1. 타이머 계산
    timer += Time.deltaTime;

    // 2. 플레이어와의 거리 계산!
    float distance = Vector2.Distance(transform.position, target.position);

    // 3. 타이머가 다 차고 + "플레이어와의 거리가 사거리(attackRange) 이내일 때만" 발사!
    if (timer > shootInterval && distance <= attackRange)
    {
        timer = 0f;
        Shoot();
    }
}

    // --- 아래 이동/방향전환은 기존 Enemy.cs 로직 100% 동일 ---
    void FixedUpdate()
{
    if(!GameManager.instance.isLive)
        return;

    // Hit 애니메이션 재생 중일 때는 멈칫하도록 기존 Enemy 로직 그대로 적용!
    if(!isLive || target == null)
        return;
    
     // 지옥식물처럼 안 움직이는 몬스터
    if (!canMove)
    {
        rigid.linearVelocity = Vector2.zero;
        return;
    }

    Vector2 dirVec = (Vector2) target.position - rigid.position;
    Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
    rigid.MovePosition(rigid.position + nextVec);
    rigid.linearVelocity = Vector2.zero;
}
    void LateUpdate()
    {
        if (GameManager.instance != null && !GameManager.instance.isLive) return;
        if (!isLive || target == null || spriter == null) return;

        spriter.flipX = target.position.x < rigid.position.x;
    }

    // ⭐ 슬라임 전용 발사 함수
    void Shoot()
{
    if (PoolManager.instance == null) return;

    GameObject bulletObj = PoolManager.instance.Get(bulletPoolIndex);
    if (bulletObj == null) return;

    bulletObj.transform.position = transform.position;
    Vector3 dirVec = (target.position - transform.position).normalized;

    EnemyBullet bullet = bulletObj.GetComponent<EnemyBullet>();
    if (bullet != null)
    {
        // 💡 per를 0으로 전달! (1번 맞으면 per-- 되어서 -1이 되고 바로 사라짐)
        bullet.Init(damage, 0, dirVec,transform.position); 
    }
}

    // --- 아래 피격/사망도 기존 Enemy.cs 로직 기반 ---
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive) return;

        Bullet playerBullet = collision.GetComponent<Bullet>();
        if (playerBullet != null)
        {
            health -= playerBullet.damage;
            //슬라임 피격음
            AudioManager.instance.PlaySfx(AudioManager.Sfx.SlimeHit);
        }

        if (health <= 0)
        {
            Dead();
        }
    }

    void Dead()
{
    Vector3 deadpos = transform.position;

    // 1. PoolManager에서 경험치 상자를 가져옵니다. (Get 안에서 SetActive(true)가 됩니다)
    GameObject expObj = PoolManager.instance.Get(4); 

    if (expObj != null)
    {
        // 2. 일단 몬스터가 죽은 위치로 옮깁니다.
        expObj.transform.position = deadpos;

        // 3. ExpItem 스크립트에 플레이어 Transform 연결!
        ExpItem expItem = expObj.GetComponent<ExpItem>();
        if (expItem != null && GameManager.instance != null && GameManager.instance.player != null)
        {
            expItem.playerTransform = GameManager.instance.player.transform;
        }
    }

    // 4. 몬스터 본인은 꺼주기
    gameObject.SetActive(false);
}
    
}