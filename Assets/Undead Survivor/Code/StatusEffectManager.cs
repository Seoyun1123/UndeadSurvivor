using UnityEngine;
using System.Collections;

public class StatusEffectManager : MonoBehaviour
{
    // 독데미지 관련 변수
    private Coroutine poisonCoroutine; // 현재 작동 중인 독 코루틴을 추적하는 변수
    private float poisonTimer = 0f;    // 독이 남은 시간 기록

    // 속박 디버프 관련 변수
    private Coroutine bindCoroutine;
    private float bindTimer = 0f;
    private float originalSpeed = -1f;
    private Player player;

    public void ApplyPoison(float duration, float damagePerTick)
    {
        // 이미 독에 걸려 있다면 시간만 늘려줌
        poisonTimer = duration;

        // 디버프가 없는 상태라면 부여
        if (poisonCoroutine == null)
        {
            poisonCoroutine = StartCoroutine(PoisonRoutine(damagePerTick));
        }
    }

    private IEnumerator PoisonRoutine(float damagePerTick)
    {
        float tickInterval = 0.5f; // 0.5초 주기로 데미지 적용

        // 독 타이머가 남아있는 동안 계속 반복
        while (poisonTimer > 0f)
        {
            // 게임이 정지 상태일 때는 독 데미지 적용하지 않음
            if (GameManager.instance != null && GameManager.instance.isLive)
            {
                // 체력 감소
                GameManager.instance.health -= damagePerTick;

                // 체력이 0 이하가 되면 사망 처리 및 디버프 중단
                if (GameManager.instance.health <= 0)
                {
                    GameManager.instance.health = 0;
                    GameManager.instance.GameOver(); // 게임오버 루틴 호출
                    break;
                }

                // 시간 감소
                poisonTimer -= tickInterval;
            }

            yield return new WaitForSeconds(tickInterval);
        }

        // 루프가 끝나면 변수를 초기화하여 다음 독을 대비
        poisonCoroutine = null;
    }


    // 덩굴 속박 상태 적용
    public void ApplyVineBind(float duration)
    {
        // 속박 중이라면 타이머 시간만 최대로 갱신
        if (bindCoroutine != null)
        {
            bindTimer = duration;
            return;
        }

        // 속박 중이 아니면 디버프 적용
        bindTimer = duration;
        bindCoroutine = StartCoroutine(BindRoutine());
    }

    private IEnumerator BindRoutine()
    {
        if (player == null && GameManager.instance != null)
        {
            player = GameManager.instance.player;
        }

        if (player == null)
        {
            bindCoroutine = null;
            yield break;
        }

        // 원래 속도 백업
        if (originalSpeed < 0f)
        {
            originalSpeed = player.speed;
        }

        // 플레이어의 속도를 0으로 설정
        player.speed = 0f;

        // 속도 벡터 초기화
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // 타이머가 끝날 때까지 대기
        while (bindTimer > 0f)
        {
            if (GameManager.instance != null && GameManager.instance.isLive)
            {
                bindTimer -= Time.deltaTime;
            }
            yield return null; // 매 프레임 대기하며 타이머 체크
        }

        // 속박 해제: 백업해둔 원래 속도로 복구하고 변수들을 리셋
        player.speed = originalSpeed;
        originalSpeed = -1f; // 원래 속도 보관 변수 초기화
        bindCoroutine = null;
    }
}