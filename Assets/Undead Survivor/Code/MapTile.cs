using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    [Header("함정 프리팹 설정")]
    public GameObject[] trapPrefabs;

    [Header("스폰 설정")]
    public int trapCount = 10;        // 타일 하나당 유지할 함정 개수 (고정)
    public float tileSize = 20f;     // 20x20 크기 반영

    public float safeZoneRadius = 3f;

    // 이 타일이 계속 들고 다닐 고정 함정 리스트
    private List<GameObject> activeTraps = new List<GameObject>();
    private Vector3 playerStartPos;

    void Start()
    {
        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            playerStartPos = GameManager.instance.player.transform.position;
        }
        else
        {
            playerStartPos = Vector3.zero;
        }

        CreateInitialTraps();
        ShuffleTrapPositions();
    }

    private void CreateInitialTraps()
    {
        if (trapPrefabs == null || trapPrefabs.Length == 0) return;

        for (int i = 0; i < trapCount; i++)
        {
            // 독/덩굴 중 랜덤 선택
            GameObject selectedPrefab = trapPrefabs[Random.Range(0, trapPrefabs.Length)];

            // 이 타일(transform)의 자식으로 생성
            GameObject trap = Instantiate(selectedPrefab, transform);
            activeTraps.Add(trap);
        }
    }

    public void ShuffleTrapPositions()
    {
        float halfSize = tileSize / 2f;
        float margin = halfSize * 0.8f; // 가장자리에 딱 붙지 않게 80% 영역 제한

        foreach (GameObject trap in activeTraps)
        {
            if (trap != null)
            {
                Vector3 finalLocalPos = Vector3.zero;
                Vector3 finalWorldPos = Vector3.zero;

                int maxAttempts = 10; // 무한 루프 방지용 (최대 10번 재시도)
                int attempt = 0;

                while (attempt < maxAttempts)
                {
                    // 1. 타일 내에 랜덤 로컬 좌표를 잡음
                    float randomX = Random.Range(-margin, margin);
                    float randomY = Random.Range(-margin, margin);
                    finalLocalPos = new Vector3(randomX, randomY, 0f);

                    // 2. 게임 월드 좌표로 변환
                    finalWorldPos = transform.TransformPoint(finalLocalPos);

                    // 3. 플레이어의 시작 위치(playerStartPos)와의 거리를 계산
                    float distanceToPlayerStart = Vector3.Distance(finalWorldPos, playerStartPos);

                    // 4. 거리가 안전 반경보다 멀면 배치 완료
                    if (distanceToPlayerStart > safeZoneRadius)
                    {
                        break;
                    }

                    attempt++;
                }

                // 5. 최종 결정된 좌표 대입
                trap.transform.localPosition = finalLocalPos;
            }
        }
    }
}