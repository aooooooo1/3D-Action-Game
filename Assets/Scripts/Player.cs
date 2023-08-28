using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    float h;
    float v;
    bool walk;
    bool playerJump;
    bool areYouJump;
    bool areYouDodge;
    Vector3 moveVec;
    Vector3 dodgeVec;
    Animator anim;
    Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        getKey();
        playerMove();
        playerTurn();
        playerJumpAction();
        playerDodgeAction();
    }

    void getKey()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
        walk = Input.GetButton("Walk");
        playerJump = Input.GetButtonDown("Jump");
    }
    void playerMove()
    {
        moveVec = new Vector3(h, 0, v).normalized;
        if (areYouDodge)
        {
            moveVec = dodgeVec; //이동할때 닷지하면 이동방향으로 닫지방향이 결정된다
        }
        transform.position += speed * (walk ? 0.3f : 1f) * Time.deltaTime * moveVec;
        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", walk);
    }
    void playerTurn()
    {
        transform.LookAt(transform.position + moveVec);
    }
    void playerJumpAction()
    {
        if (playerJump && areYouJump==false && moveVec ==Vector3.zero )
        {
            rigid.AddForce(Vector3.up*15, ForceMode.Impulse);
            anim.SetBool("areYouJump", true);
            anim.SetTrigger("doJump");
            areYouJump = true;
        }
        
    }
    void playerDodgeAction()
    {
        if (moveVec != Vector3.zero && areYouJump == false && playerJump && !areYouDodge)
        {
            dodgeVec = moveVec; //닷지 일때 방향을 기억한다 
            speed *= 2;
            anim.SetTrigger("doDodge");
            areYouDodge = true;

            Invoke("dodgeOut",0.5f);
        }

    }
    void dodgeOut()
    {
        speed *= 0.5f;
        areYouDodge = false;
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "floor")
        {
            anim.SetBool("areYouJump", false);
            areYouJump = false;

        }
    }
}
