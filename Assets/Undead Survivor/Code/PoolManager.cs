using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;
    public GameObject[] prefabs;
    List<GameObject>[] pools;

    void Awake()
    {
        instance = this;
        pools = new List<GameObject>[prefabs.Length];

        for (int index = 0; index < pools.Length; index++)
        {
            pools[index] = new List<GameObject>();
        }

    } 

    public GameObject Get(int index)
    {
        GameObject select = null;

        // 1. 이미 풀에 안전하게 만들어져 있는 게 있다면 가져온다.
        foreach (GameObject item in pools[index])
        {
            if (item != null && !item.activeSelf)
            {
                select = item;
                select.SetActive(true); 
                break;
            }
        }

        // 2. 풀에 없어서 "최초 생성" 해야 할 때 ⭐
        if (!select)
        {
            // 몬스터나 플레이어 위치에서 1000m 떨어진 완전 엉뚱한 우주 공간 좌표 지정
            Vector3 safeSpawnPos = new Vector3(-1000f, -1000f, 0f);

            // ⭐ [핵심] 부모(transform)를 주지 않고, 플레이어 발밑과 전혀 상관없는 저 우주 멀리서 생성합니다!
            // 이렇게 해야 태어날 때 플레이어 발밑 좌표에 한 프레임이라도 비치는 버그가 완전히 원천 차단됩니다.
            select = Instantiate(prefabs[index], safeSpawnPos, Quaternion.identity);
            
            // 생성한 직후 확실하게 비활성화 시킵니다.
            select.SetActive(false);

            // 생성이 완벽히 끝난 뒤에 비로소 PoolManager의 자식으로 깔끔하게 소속시켜 줍니다.
            select.transform.SetParent(transform);

            pools[index].Add(select);
            
            // 몬스터가 가져다 쓸 수 있도록 일단 켜서 내보냅니다.
            select.SetActive(true); 
        }

        return select;
    }
}
