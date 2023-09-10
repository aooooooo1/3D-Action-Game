using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{//Enemy.cs 에서는 무기등에 의해 맞은 모션 (피격 )을 구현한다
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public Rigidbody rigid;
    public BoxCollider boxColl;
    //적이 총알에 맞을 경우에는 코루틴을 통해 잠깐 빨강색으로 보이게 할것이. 그렇기 때문에 material 을 가지고 온다
    //Material mat; 우리는 자식이 많은 물체의 색을 바꾸기 위해 배열버전 매쉬랜더러를 가지고 온다
    public MeshRenderer[] meshes;
    public NavMeshAgent nav;
    public Animator anim;
    public bool isChase;
    //공격 범위 콜라이더
    public BoxCollider meleeArea;
    //공격 하는지?
    public bool isAttack;
    public enum type { A,B,C,D};
    public type EnemyType;
    //missile variable.
    public GameObject Cmissile;
    public bool isDead;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxColl = GetComponent<BoxCollider>();
        meshes = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        if(EnemyType != type.D)
        {
            Invoke("ChaseStart", 1);
        }
    }
    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    private void Update()
    {
        if (nav.enabled && EnemyType != type.D)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }

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
        foreach(MeshRenderer mesh in meshes)
        {
            mesh.material.color = Color.red;
        }
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0)
        {
            foreach (MeshRenderer mesh in meshes)
            {
                mesh.material.color = Color.white;
            }
        }
        else //죽는 경우
        {
            foreach (MeshRenderer mesh in meshes)
            {
                mesh.material.color = Color.gray;
            }
            gameObject.layer = 12;
            isDead = true;
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
                reactVec += Vector3.up;
                rigid.AddForce(reactVec * 10, ForceMode.Impulse);
            }
            //enemy destroy
            if(EnemyType != type.D)
            {
                Destroy(gameObject, 4);
            }
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
        //몬스터의 타겟팅을 구하는 로직
        targeting();
    }

    //플레이어 도는 현상 막음
    private void stopTrunPlayer()
    {
        if(nav.enabled)
        rigid.velocity = Vector3.zero;
    }


    //타겟팅 구함
    void targeting()
    {
        if(EnemyType != type.D)
        {
            float targetRadius = 0;
            float targetRange = 0;

            switch (EnemyType)
            {
                case type.A:
                    targetRadius = 1.5f;
                    targetRange = 5f;
                    break;
                case type.B:
                    targetRadius = 1f;
                    targetRange = 16f;
                    break;
                case type.C:
                    targetRadius = 1f;
                    targetRange = 25f;
                    break;
            }
            RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position,
                                                    targetRadius,
                                                    transform.forward,
                                                    targetRange,
                                                    LayerMask.GetMask("Player"));
            if (raycastHits.Length > 0 && !isAttack)
            {
                StartCoroutine(EnemyAttack());
            }
        }
    }
    IEnumerator EnemyAttack()
    {
        isChase = false; //공격할때는 따라가는것을 멈춘다.
        isAttack = true; //이걸 true와 false를 지정해야 타겟에서 조건을 걸수있다.
        anim.SetBool("isAttack", true); //공격모션을 실행한다

        switch (EnemyType)
        {
            case type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true; //0.2초 후에 공격범위 활성화 시킨다
                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false; //1초간 공격후 범위 비활성화
                yield return new WaitForSeconds(1f);
                break; //1초 쉬고 다시 복귀
            case type.B: 
                yield return new WaitForSeconds(0.1f);
                meleeArea.enabled = true; //0.1초 후에 공격범위 활성화
                rigid.AddForce(transform.forward * 530, ForceMode.Impulse);
                // 앞 방향으로 힘을 가한다
                yield return new WaitForSeconds(1.5f);
                rigid.velocity = Vector3.zero; //1초간 힘을가하고 다시 물리제로
                meleeArea.enabled = false; //공격범위 비활성화
                yield return new WaitForSeconds(0.5f);
                break;
            case type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject insMi = Instantiate(Cmissile, transform.position, transform.rotation);
                Rigidbody rigidMi = insMi.GetComponent<Rigidbody>();
                rigidMi.velocity = transform.forward * 30;
                yield return new WaitForSeconds(1f);
                break;
        }
        isChase = true; //다시 쫓아간다.
        isAttack = false; //공격중이 아니다
        anim.SetBool("isAttack", false); //모션 끝
    }
}
