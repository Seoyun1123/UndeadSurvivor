using UnityEngine;

public class ExpItem : MonoBehaviour
{
    public int expValue = 10; // 경험치 점수
    public Transform playerTransform;
    
    public float startSpeed = 3f;
    public float accel = 10f;
    public float spawnDelay = 0.3f; // 💡 0.3초 무적/대기 시간

    private Rigidbody2D rigid;
    private Collider2D coll;
    private float speed;
    private float spawnTimer;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        spawnTimer = 0f;
        speed = startSpeed;

        // 💡 1. 태어날 때 일단 콜라이더를 끕니다! (Player.cs의 OnTriggerEnter2D에 바로 걸리는 것 방지)
        if (coll != null)
        {
            coll.enabled = false; 
        }

        if (rigid != null)
        {
            rigid.bodyType = RigidbodyType2D.Dynamic;
            rigid.linearVelocity = Vector2.zero;
            rigid.angularVelocity = 0f;

            // 💡 2. 바닥으로 튕겨 나가는 힘 주입
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            rigid.AddForce(randomDir * 2.5f, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.instance != null && !GameManager.instance.isLive) return;

        spawnTimer += Time.fixedDeltaTime;

        // 💡 3. 0.3초 동안은 바닥에 튕겨 나가는 연출을 보장하며 아무것도 안 함!
        if (spawnTimer < spawnDelay) return;

        // 💡 4. 0.3초가 지난 뒤에야 비로소 콜라이더를 켜서 플레이어가 먹을 수 있게 만듭니다!
        if (coll != null && !coll.enabled)
        {
            coll.enabled = true;
        }

        if (playerTransform == null) return;

        // 5. 플레이어를 향해 가속 이동
        speed += accel * Time.fixedDeltaTime;
        Vector2 myPos = rigid.position;
        Vector2 targetPos = playerTransform.position;
        Vector2 dirVec = targetPos - myPos;

        rigid.linearVelocity = dirVec.normalized * speed;

        // 6. 아주 가까워지면 먹기
        if (Vector2.Distance(myPos, targetPos) <= 0.3f)
        {
            EatExp();
        }
    }

    public void EatExp()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.GetExp(expValue);
        }

        gameObject.SetActive(false);
    }
}