using System.Data;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float damage;
    public int per;

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;

        // 💡 per 조건(if per >= 0)을 제거하여, 슬라임이 어떤 per 값을 주더라도 
        // 넘겨받은 dir 방향으로 15의 속도로 시원하게 날아갑니다!
        if (rigid != null)
        {
            rigid.linearVelocity = dir * 15f;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
{
    // 1. 플레이어 태그 확인 및 관통 체크
    if (!collision.CompareTag("Player") || per == -100)
        return;

    // 💡 2. 부딪힌 플레이어 오브젝트의 Player 스크립트를 가져옵니다!
    Player player = collision.GetComponent<Player>();

    if (player != null && GameManager.instance != null)
    {
        // 3. 데미지 적용
        GameManager.instance.health -= damage;

        // 💡 4. 데미지를 받은 후 체력이 0이 되었을 때!
        if (GameManager.instance.health <= 0)
        {
            // 💡 플레이어 스크립트에 "죽어!" 라고 명령 (죽는 애니메이션 트리거 작동!)
            player.StartDeathSequence(); 
            GameManager.instance.GameOver(); // 게임오버 처리
        }
    }

    // 5. 총알 관통 횟수 차감 및 소멸
    per--;

    if (per < 0)
    {
        rigid.linearVelocity = Vector2.zero;
        gameObject.SetActive(false);
    }
}

    void OnTriggerExit2D(Collider2D collision)
    {
        // 맵 영역(Area) 밖으로 나갔을 때 소멸 처리
        if (!collision.CompareTag("Area") || per == -100)
            return;

        rigid.linearVelocity = Vector2.zero;
        gameObject.SetActive(false);
    }
}
