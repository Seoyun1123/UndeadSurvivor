using UnityEngine;

public class ExpItem : MonoBehaviour
{
    [Header("경험치 설정")]
    public int expValue = 10;          
    public float detectRadius = 1.6f;   

    [Header("실시간 추적 대상")]
    public Transform playerTransform; 

    private Rigidbody2D rigid;
    private Collider2D coll;
    
    private bool isTargeting = false;
    private float spawnDelay = 0.2f; // 튕겨나가는 안전 시간    
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
        // ⭐ 태어나자마자 자석은 켜두되, 타이머와 속도를 초기화합니다.
        isTargeting = true;
        spawnTimer = 0f;
        speed = 5f; 

        transform.localScale = Vector3.one; 
        transform.rotation = Quaternion.identity;

        if (coll != null) 
        {
            coll.enabled = true;
            coll.isTrigger = true; 
        }

        if (rigid != null)
        {
            rigid.bodyType = RigidbodyType2D.Dynamic;
            rigid.linearVelocity = Vector2.zero; 
            rigid.angularVelocity = 0f;
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;

            // 몬스터 몸에서 "통" 튕겨나가는 힘 주입
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            rigid.AddForce(randomDir * 2.5f, ForceMode2D.Impulse); 
        }
    }

    private void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.isLive) return;
        
        // ⭐ 안전 타이머가 도는 동안에는 거리 계산이나 흡수 로직을 완전히 패스합니다!
        if (spawnTimer < spawnDelay)
        {
            spawnTimer += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.instance != null && !GameManager.instance.isLive) return;
        if (playerTransform == null || !isTargeting) return;

        // ⭐ 튕겨나가는 안전 시간(0.2초) 동안에는 밑에 추적 및 흡수 연산을 아예 실행하지 않습니다!
        // 이방어막 덕분에 풀매니저 우주 공간에서 생성되자마자 자폭하는 버그가 완전히 박살납니다.
        if (spawnTimer < spawnDelay) return;

        speed += accel * Time.fixedDeltaTime;

        Transform realPlayerRoot = playerTransform.root;
        Vector2 myPos = rigid.position;
        Vector2 targetPos = new Vector2(realPlayerRoot.position.x, realPlayerRoot.position.y); 
        Vector2 dirVec = targetPos - myPos;

        // 몬스터 공식으로 속도 벡터 주입
        rigid.linearVelocity = dirVec.normalized * speed;

        // 안전 시간이 지난 후에만 플레이어 품에 닿았는지 체크!
        float distance = Vector2.Distance(myPos, targetPos);
        if (distance <= 0.2f)
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
        // 안전 시간이 지났을 때만 트리거 충돌을 허용합니다.
        if (spawnTimer >= spawnDelay && collision.CompareTag("Player"))
        {
            EatExp();
        }
    }
}