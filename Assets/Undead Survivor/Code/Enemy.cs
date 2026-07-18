using System.Collections;
using Unity.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;

    bool isLive;
    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    SpriteRenderer spriter;
    WaitForFixedUpdate wait;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(!GameManager.instance.isLive)
	return;


        if(!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.linearVelocity = Vector2.zero;
        
    }

    void LateUpdate() 
    {
        if(!GameManager.instance.isLive)
	return;

        if(!isLive)
            return;
        spriter.flipX = target.position.x < rigid.position.x;
        
    }

    void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder =2;
        anim.SetBool("Dead",false);

        health = maxHealth;

    }
    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive)
            return;

        health -= collision.GetComponent<Bullet>().damage;
        StartCoroutine(KnockBack());

        if (health > 0 )
        {
            // .. Live,Hit Action
            anim.SetTrigger("Hit");
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
        }
        else
        {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            anim.SetBool("Dead",true);
            GameManager.instance.kill++;
            

            if(GameManager.instance.isLive)
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
            
        }
    }

    IEnumerator KnockBack()
    {
        yield return wait; // 하나의 물리 프레임을 딜레이
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
    }
    void Dead()
{
    Vector3 deadpos = this.transform.position;
    
    // 1. 풀매니저에서 보석을 가져옵니다.
    GameObject expObj = PoolManager.instance.Get(3); 
    
    // 2. 일단 비활성화하고 몬스터가 죽은 위치로 세팅합니다.
    expObj.SetActive(false);
    expObj.transform.position = new Vector3(deadpos.x, deadpos.y, 0f);
    
    // ⭐ [핵심 치트키] ⭐
    // 보석에 붙어있는 ExpItem 스크립트를 가져와서, 
    // 몬스터가 추적하고 있던 진짜 플레이어의 트랜스폼 정보를 강제로 직접 꽂아줍니다!
    ExpItem expItem = expObj.GetComponent<ExpItem>();
    if (expItem != null)
    {
        // 몬스터가 원래 잘 쫓아가던 플레이어 대상(예: target 또는 playerTransform 등)을 보석에게 그대로 전수합니다.
        // 만약 몬스터 스크립트에 플레이어 변수 이름이 다르면 그에 맞게 수정해 주시면 됩니다! (예: GameManager.instance.player.transform)
        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            expItem.playerTransform = GameManager.instance.player.transform;
        }
    }
    
    // 3. 이제 플레이어 정보가 강제로 뇌에 박힌 보석을 활성화합니다!
    expObj.SetActive(true);

    gameObject.SetActive(false);
}
    

    


}

