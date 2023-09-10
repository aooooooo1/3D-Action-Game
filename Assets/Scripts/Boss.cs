using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Boss : Enemy
{
    public GameObject bossRock;
    public GameObject bossMi;
    public Transform bossMiA;
    public Transform bossMiB;
    protected Vector3 lookVec;
    Vector3 tauntVec;
    public bool isLook;
    
    public Vector3 playerDirection;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxColl = GetComponent<BoxCollider>();
        meshes = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        nav.isStopped = true;//보스는 이동을 멈춤 
    }

    void Start()
    {
        StartCoroutine(RanAtkPattern());
    }
    //바라보다
    void Update()
    {
        Vector3 playerPosition = target.transform.position;
        playerDirection = (playerPosition - transform.position).normalized;
        if (isLook & !isDead)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v)*5f;
            transform.LookAt(target.position + lookVec);
        }
        else
        {
            nav.SetDestination(tauntVec);
        }
    }
    //랜덤어택패턴
    IEnumerator RanAtkPattern()
    {
        yield return new WaitForSeconds(0.1f);
        int ranAct = Random.Range(0, 3);
        switch (ranAct)
        {
            case 0:
                StartCoroutine(miPat());
                break;
            case 1:
                StartCoroutine(rockPat());
                break;
            case 2:
                StartCoroutine(taPat());
                break;
        }
    }
    //미사일패턴
    IEnumerator miPat()
    {
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.1f);
        GameObject intMiA = Instantiate(bossMi,bossMiA.position , bossMiA.rotation);
        BossMi bossMissileA = intMiA.GetComponent<BossMi>();
        bossMissileA.target = target;
        yield return new WaitForSeconds(0.5f);
        GameObject intMiB = Instantiate(bossMi, bossMiB.position, bossMiB.rotation);
        BossMi bossMissileB = intMiB.GetComponent<BossMi>();
        bossMissileB.target = target;
         
        yield return new WaitForSeconds(2f);
        StartCoroutine(RanAtkPattern());
    }
    //바위패턴
    IEnumerator rockPat()
    {
        isLook = false;
        anim.SetTrigger("doBigShot");
        Instantiate(bossRock, transform.position+lookVec, transform.rotation);
        yield return new WaitForSeconds(1.5f);
        isLook = true;
        isLook = false;
        anim.SetTrigger("doBigShot");
        Instantiate(bossRock, transform.position + lookVec, transform.rotation);
        yield return new WaitForSeconds(1.5f);
        isLook = true;
        isLook = false;
        anim.SetTrigger("doBigShot");
        Instantiate(bossRock, transform.position + lookVec, transform.rotation);
        yield return new WaitForSeconds(1.5f);
        isLook = true;
        StartCoroutine(RanAtkPattern());
    }
    //타운트패턴
    IEnumerator taPat()
    {
        tauntVec = target.position + lookVec;
        isLook = false;
        nav.isStopped = false; //이때는 이동을 허락함
        boxColl.enabled = false;
        anim.SetTrigger("doTaunt");
        yield return new WaitForSeconds(1.3f);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(1f);
        meleeArea.enabled = false;
        isLook = true;
        nav.isStopped = true;
        boxColl.enabled = true;
        StartCoroutine(RanAtkPattern());
    }

}
