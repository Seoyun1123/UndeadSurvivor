using UnityEngine;

public class Spawner : MonoBehaviour
{

    public Transform[] spawnPoint;
    [Header("맵별 스폰 데이터")]
    public SpawnData[] defaultSpawnData;
    public SpawnData[] swampSpawnData;
    public SpawnData[] desertSpawnData;

    SpawnData[] currentSpawnData;
    int level;
    float timer;
    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
        currentSpawnData = defaultSpawnData;
        
    }
    void Update()
    {
        if(!GameManager.instance.isLive)
	return;

        timer += Time.deltaTime;
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / 10f),currentSpawnData.Length - 1);

        if(timer > currentSpawnData[level].spawnTime)
        {
            timer = 0;
            Spawn();
        }

    }

    public void SetMap(int mapId)
{
    
    switch (mapId)
    {
        case 0:
            currentSpawnData = defaultSpawnData;
            break;

        case 1:
            currentSpawnData = swampSpawnData;
            break;

        case 2:
            currentSpawnData = desertSpawnData;
            break;
    }

    timer = 0;
    level = 0;
}

    void Spawn()
{
    int type = currentSpawnData[level].spriteType;

    GameObject enemyObj;

    // 💡 슬라임(5번)만 전용 프리팹을 꺼내고, 나머지는 전부 0번(일반 몬스터) 프리팹을 꺼냄!
    if (type == 3) // 슬라임 spriteType 번호
    {
        enemyObj = GameManager.instance.pool.Get(3);
    }
    else if (type == 4) // 지옥식물 spriteType 번호
    {
        enemyObj = GameManager.instance.pool.Get(6);
    }
    else
    {
        enemyObj = GameManager.instance.pool.Get(0);
    }

    enemyObj.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;

    // 1. 일반 몬스터일 경우: Init()으로 AcEnemy 애니메이터 바꿔치기
    Enemy enemy = enemyObj.GetComponent<Enemy>();
    if (enemy != null)
    {
        enemy.Init(currentSpawnData[level]);
    }

    // 2. 슬라임일 경우: 슬라임 스펙 초기화
    Slime slime = enemyObj.GetComponent<Slime>();
    if (slime != null)
    {
        slime.health = currentSpawnData[level].health;
        slime.maxHealth = currentSpawnData[level].health;
        slime.speed = currentSpawnData[level].speed;
    }
}
}
[System.Serializable]
public class SpawnData
{
    public float spawnTime;
    public int spriteType;
    public int health;
    public float speed;

}
