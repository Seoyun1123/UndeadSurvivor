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
    
    GameObject expObj = PoolManager.instance.Get(3); 
    
    expObj.SetActive(false);
    expObj.transform.position = new Vector3(deadpos.x, deadpos.y, 0f);
    
    ExpItem expItem = expObj.GetComponent<ExpItem>();
    if (expItem != null)
    {
        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            // ⭐ [물리 좌표 직통 연결] ⭐
            // 일반 transform이 아니라, 플레이어가 움직일 때 쓰는 진짜 물리 리지드바디의 컴포넌트를 찾아서 보석에게 직접 꽂아줍니다!
            Rigidbody2D playerRigid = GameManager.instance.player.GetComponent<Rigidbody2D>();
            if (playerRigid != null)
            {
                expItem.playerTransform = playerRigid.transform;
            }
            else
            {
                expItem.playerTransform = GameManager.instance.player.transform;
            }
        }
    }
    
    expObj.SetActive(true);
    gameObject.SetActive(false);
}
    

    


}

