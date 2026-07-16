using UnityEngine;

public class VileTile : MonoBehaviour
{
    [Header("속박 시간 설정")]
    public float duration = 3f; // 독 상태 이상 지속 시간

    private void OnTriggerEnter2D(Collider2D other)
    {

        // 부딪힌 대상의 태그가 "Player"인지 확인
        if (other.CompareTag("Player"))
        {

            StatusEffectManager effects = other.GetComponent<StatusEffectManager>();

            if (effects != null)
            {
                // 속박 이상 상태 부여
                effects.ApplyVineBind(duration);
            }
        }
    }
}
