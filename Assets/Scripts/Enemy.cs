using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{//Enemy.cs 에서는 무기등에 의해 맞은 모션 (피격 )을 구현한다
    public int maxHealth;
    public int curHealth;
    public Transform target;
    Rigidbody rigid;
    BoxCollider boxColl;
    //적이 총알에 맞을 경우에는 코루틴을 통해 잠깐 빨강색으로 보이게 할것이. 그렇기 때문에 material 을 가지고 온다
    Material mat;
    NavMeshAgent nav;
    Animator anim;
    public bool isChase;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxColl = GetComponent<BoxCollider>();
        //material은 매쉬 안에 있는 요소이므로 매쉬를 불러와서 매트를 불러야한.
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        Invoke("ChaseStart", 1);
    }
    private void Update()
    {
        if(isChase)
            nav.SetDestination(target.position);
    }
    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            //enemy가 무기,총알에 맞았으면 그 무기 총알의 데미지만큼 체력이 깍여야한다 그래서 무기 총알의 정보를 가져오도록 한다
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Debug.Log("근접무기에 맞음 체력 : " + curHealth);
            //몬스터 현재위치에서 무기의 위치를 뺀값에 반대 방향으로 넉백을 추가할거에요 .먼저 뺀값을 코루틴에 보네고 죽었을때에 뺀값을 기능 처리함
            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec,false));
        }
        else if (other.tag == "bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Debug.Log("원거리무기에 맞음 체력 : " + curHealth);
            Vector3 reactVec = transform.position - other.transform.position;
            //총알이 enemy를 뚫고 지나가니 여기서 삭제를 시켜주면 된다
            Destroy(other.gameObject);
            StartCoroutine(OnDamage(reactVec,false));
        }
    }
    IEnumerator OnDamage(Vector3 reactVec,bool isExplosion)
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0)
        {
            mat.color = Color.white;
        }
        else
        {
            mat.color = Color.gray;
            gameObject.layer = 12;
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");

            if (isExplosion)
            {
                //s넉백 거리 상관없이 전부 1로 통일한다
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 4;
                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 10, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                //s넉백 거리 상관없이 전부 1로 통일한다
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;
                rigid.AddForce(reactVec * 10, ForceMode.Impulse);
            }
            //enemy destroy
            Destroy(gameObject, 4);
        }
    }
    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }
    private void FixedUpdate()
    {
        //플레이어 충돌시 빙빙도는 현상을 막는 로직 
        stopTrunPlayer();
    }

    //플레이어 도는 현상 막음
    private void stopTrunPlayer()
    {
        if(isChase)
        rigid.velocity = Vector3.zero;
    }
}
