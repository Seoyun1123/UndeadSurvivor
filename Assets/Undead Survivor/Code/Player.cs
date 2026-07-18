using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    public Scanner scanner;
    public Hand[] hands;
    public RuntimeAnimatorController[] animCon;


    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        hands = GetComponentsInChildren<Hand>(true);

        
    }
    void OnEnable()
    {
        speed *= Character.Speed;
        anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];
    }


    
    void FixedUpdate()
    {
        if(!GameManager.instance.isLive)
	return;

        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        // 3. 위치 이동
        rigid.MovePosition(rigid.position + nextVec);   
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
        

    }

    void LateUpdate() 
    {
        if(!GameManager.instance.isLive)
	return;

        anim.SetFloat("Speed",inputVec.magnitude);

        if (inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0;
        }

    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.instance.isLive)
            return;
        GameManager.instance.health -= Time.deltaTime * 10;

        if (GameManager.instance.health<0)
        {
            for(int index = 2; index < transform.childCount; index++)
            {
                transform.GetChild(index).gameObject.SetActive(false);
            }

            anim.SetTrigger("Dead");
            GameManager.instance.GameOver();
        
        }

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Exp"))
        {
            ExpItem exp = collision.GetComponent<ExpItem>();
            if (exp != null)
            {
                // 1. GameManager에 경험치 더해주기 (보유하신 함수명에 맞게 수정)
                GameManager.instance.GetExp(exp.expValue);
                
                // 2. (선택) 사운드 재생
                // AudioManager.instance.PlaySfx(AudioManager.Sfx.Select); 

                // 3. 획득했으므로 보석을 비활성화해서 풀로 반환
                collision.gameObject.SetActive(false);
            }
    }
    }
}
