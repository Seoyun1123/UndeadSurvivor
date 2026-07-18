using UnityEngine;

public class Spawner : MonoBehaviour
{

    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    

    int level;
    float timer;
    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
        
    }
    void Update()
{
    if (!GameManager.instance.isLive) return;

    // 스페이스바를 누를 때마다 딱 1마리만 내가 원할 때 스폰! ⭐
    if (Input.GetKeyDown(KeyCode.G))
    {
        Spawn(); // 기존 몬스터 소환 함수 호출
    }
}
    // void Update()
    // {
    //     if(!GameManager.instance.isLive)
	// return;

    //     timer += Time.deltaTime;
    //     level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / 10f),spawnData.Length - 1);

    //     if(timer > spawnData[level].spawnTime)
    //     {
    //         timer = 0;
    //         Spawn();
    //     }

    // }

    void Spawn()
    {
        GameObject enemy = GameManager.instance.pool.Get(0);
        enemy.transform.position = spawnPoint[Random.Range(1,spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
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
