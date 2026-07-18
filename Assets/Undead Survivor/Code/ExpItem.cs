using UnityEngine;

public class ExpItem : MonoBehaviour
{
    [Header("경험치 설정")]
    public int expValue = 10;          
    public float detectRadius = 1.6f;   

    [Header("실시간 추적 대상 (몬스터가 꽂아줌)")]
    public Transform playerTransform; 

    private Rigidbody2D rigid;
    private Collider2D coll;
    
    private bool isTargeting = false;
    private float spawnDelay = 0.2f;    
    private float spawnTimer = 0f;

    [Header("자석 속도 설정")]
    public float speed = 5f;            
    public float accel = 8f;            

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        isTargeting = false;
        spawnTimer = 0f;
        speed = 5f; 

        transform.localScale = Vector3.one; 
        transform.rotation = Quaternion.identity;

        // 콜라이더 트리거 설정 (자석 이동 중에 튕기지 않게)
        if (coll != null) 
        {
            coll.enabled = false;
            coll.isTrigger = true; 
        }

        if (rigid != null)
        {
            rigid.bodyType = RigidbodyType2D.Dynamic;
            rigid.linearVelocity = Vector2.zero; 
            rigid.angularVelocity = 0f;
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;

            // 몬스터 몸에서 사방으로 튕겨 나가는 초기 물리 연출
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            rigid.AddForce(randomDir * 2.5f, ForceMode2D.Impulse); 
        }
    }

    private void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.isLive) return;
        if (playerTransform == null) return; 

        // 소환 직후 튕겨나가는 시간 딜레이
        if (spawnTimer < spawnDelay)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnDelay)
            {
                if (coll != null) coll.enabled = true;
            }
            return; 
        }

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (!isTargeting)
        {
            if (distance <= detectRadius)
            {
                isTargeting = true;
            }
        }
    }

    // ⭐ 다이나믹 리지드바디에 플레이어로 향하는 힘(속도)만 단순하게 불어넣어 줍니다.
    private void FixedUpdate()
    {
        if (GameManager.instance != null && !GameManager.instance.isLive) return;
        if (playerTransform == null || !isTargeting) return;

        speed += accel * Time.fixedDeltaTime;

        // 몬스터와 똑같이 플레이어 방향 벡터 구하기
        Vector2 dirVec = (Vector2)playerTransform.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed;

        // 다이나믹 속도 벡터에 플레이어 방향 속도를 그냥 꽂아넣기!
        rigid.linearVelocity = nextVec;

        // 거리 확인 후 획득 처리
        float distance = Vector2.Distance(rigid.position, playerTransform.position);
        if (distance <= 0.15f)
        {
            EatExp();
        }
    }

    private void EatExp()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.GetExp(expValue); 
        }
        gameObject.SetActive(false); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EatExp();
        }
    }
}