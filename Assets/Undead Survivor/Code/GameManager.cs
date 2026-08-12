using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public Spawner spawner;
    public static GameManager instance;
    [Header("# Game Control")]
    public bool isLive;
    public float gameTime;
    [Header("# Player Info")]
    public int playerId;
    public float health;
    public float maxHealth = 100;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = new int[50];
    [Header("# GameObject")]
    public PoolManager pool;
    public Player player;
    public LevelUp uiLevelUp;
    public Result uiResult;
    public Transform uiJoy;
    public GameObject enemyCleaner;
    public GameObject uiGameStart;
public GameObject characterGroup;
public GameObject mapGroup;

public GameObject[] maps;
public int mapId;

    void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
        for (int i = 0; i < nextExp.Length ; i++)
        {
            nextExp [i] = 10;
        }
    }

    public void GameStart(int id)
    {
        // 캐릭터만 기억
    playerId = id;

    // 캐릭터 선택창 닫기
    characterGroup.SetActive(false);

    // 맵 선택창 열기
    mapGroup.SetActive(true);

    AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    public void MapStart(int id)
{
    mapId = id;

    // 선택된 맵만 활성화
    for (int i = 0; i < maps.Length; i++)
    {
        if (maps[i] != null)
            maps[i].SetActive(i == mapId);
    }

    // 실제 게임 시작
    health = maxHealth;
    player.gameObject.SetActive(true);

    spawner.SetMap(mapId);

    // 시작 UI 전체 끄기
    uiGameStart.SetActive(false);

    uiLevelUp.Select(playerId % 2);

    Resume();

    AudioManager.instance.PlayBgm(true);
    AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
}

    IEnumerator GameOverRoutine()
    {
        isLive = false;

        yield return new WaitForSeconds(0.5f);
        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop();

        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
    }

    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());

    }

    IEnumerator GameVictoryRoutine()
    {
        isLive = false;
        enemyCleaner.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        Stop();
        
        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }
    public void GameQuit()
    {
        Application.Quit();
    }


    void Update()
    {
        if(!isLive)
	return;

        gameTime += Time.deltaTime;

    }

    public void GetExp(int amount) //amount = expvalue 매개변수
    {
        if (!isLive)
            return ;
        exp += amount;
        int requiredExp = nextExp[Mathf.Min(level,nextExp.Length-1)];
        if(exp >= nextExp[Mathf.Min(level,nextExp.Length-1)])
        {
            exp -= requiredExp;
            level++;

            if (level >= 50)
            {
                GameVictory();
                return;
            }
            uiLevelUp.show();
        }
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
        uiJoy.localScale = Vector3.zero;
    }
    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
        uiJoy.localScale = Vector3.one;

    }
}