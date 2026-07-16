using UnityEngine;

public class PoisonTile : MonoBehaviour
{
    public float poisonDamage = 5f; // 1틱당 들어갈 데미지 양
    public float duration = 3f;     // 독이 지속될 총 시간

    private void OnTriggerEnter2D(Collider2D other)
    {

        // 부딪힌 대상의 태그가 "Player"인지 확인
        if (other.CompareTag("Player"))
        {

            StatusEffectManager effects = other.GetComponent<StatusEffectManager>();

            if (effects != null)
            {
                // 독 상태 이상 부여
                effects.ApplyPoison(duration, poisonDamage);
            }
        }
    }

}