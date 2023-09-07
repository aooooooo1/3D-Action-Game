using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapon;

    //수류탄 배열
    public GameObject[] grenades;
    //수류탄 던질때 생기는 변수
    public GameObject grenadObj;

    float h;
    float v;
    bool walk;
    bool playerJump;
    bool areYouJump;
    bool areYouDodge;
    bool eKey;
    bool oneKey;
    bool twoKey;
    bool threeKey;
    bool rKey;
    bool isReload;
    bool isBorder;
    int equipObjIndex = -1;
    GameObject nearObj;
    Weapon equipObj;
    Vector3 moveVec;
    Vector3 dodgeVec;
    Animator anim;
    Rigidbody rigid;

    //player가 가지고 있는 기본 정보
    public int ammo;
    public int health;
    public int coin;
    public int hasGrenade;
    public int maxAmmo;
    public int maxHealth;
    public int maxCoin;
    public int maxHasGrenade;

    //무기공격할때 쓰이는 정보
    bool fireKey;
    bool grenadeKey;
    float fireDelay;
    bool isFireReady;

    //총쏠때 마우스방향으로 쏘기
    public Camera followC;

    //플레이어 피격 무적bool
    bool isDamaged;
    //플레이어 피격시 색 변하도록 매쉬가져옴
    MeshRenderer[] meshes;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshes = GetComponentsInChildren<MeshRenderer>();
    }

    void Update()
    {
        getKey();
        playerMove();
        playerTurn();
        playerJumpAction();
        playerDodgeAction();
        interact();
        showWeapon();
        Attack();
        Reload();
        ThrowGrenade();
    }

    void getKey()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
        walk = Input.GetButton("Walk");
        playerJump = Input.GetButtonDown("Jump");
        eKey = Input.GetButtonDown("E");
        oneKey = Input.GetButtonDown("One");
        twoKey = Input.GetButtonDown("Two");
        threeKey = Input.GetButtonDown("Three");
        fireKey = Input.GetButton("Fire1");
        grenadeKey = Input.GetButtonDown("Fire2");
        rKey = Input.GetButtonDown("Reload");
    }
    void playerMove()
    {
        //입력한 키의 방향 moveVec로 설/
        moveVec = new Vector3(h, 0, v).normalized;
        if (areYouDodge)//Dodge만들고 봐야함
        {
            moveVec = dodgeVec; //이동할때 닷지하면 이동방향으로 닫지방향이 결정된다
        }
        if(!isBorder)//벽에 안 데일때만 이동 가능함 
            //player포지션에 moveVec 더해준다.
            transform.position += speed * (walk ? 0.3f : 1f) * Time.deltaTime * moveVec;
        //이동하면 isRun은 true
        anim.SetBool("isRun", moveVec != Vector3.zero);
        //walk발동하면 isWalk= true;
        anim.SetBool("isWalk", walk);
    }
    //player가 가고있는 방향으로 정면을 만들어준다.
    void playerTurn()
    {
        transform.LookAt(transform.position + moveVec);

        //마우스 클릭 방향 총알 쏘기
        if (fireKey)
        {
            //메인카메라에서 Ray를 마우스클릭한곳으로 쏜다.
            Ray ray = followC.ScreenPointToRay(Input.mousePosition);
            //쏜 ray를 담을 객체
            RaycastHit rayHit;
            //raycast로 ray를 100만큼 쏘고 rayHot에 정보를 담는다
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                //마우스위치는 항상 바뀌기 때문에 여기서 플레이어위치를 뺀다
                Vector3 nextVec = rayHit.point - transform.position;
                //마우스 위치는 x,y좌표만 찍는다 
                nextVec.y = 0;
                //player direction + nextVec 
                transform.LookAt(transform.position + nextVec);
            }
        }
    }
    //점프시 ayJump=true;
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
    //Dodge실행함수 
    void playerDodgeAction()
    {
        //조건: 움직이고 점프키를 누르고 점프,닫지상태가 아닐때
        if (moveVec != Vector3.zero && areYouJump == false && playerJump && !areYouDodge)
        {
            dodgeVec = moveVec; //닷지 일때 방향을 기억한다 
            speed *= 2;
            anim.SetTrigger("doDodge");
            areYouDodge = true;

            Invoke("dodgeOut",0.5f);
        }

    }
    //닫지 실행후 0.5초 이따가 areYouDodge=false;
    void dodgeOut()
    {
        speed *= 0.5f;
        areYouDodge = false;
    }
    //바닥에 데이면 anim false, ayjump=false;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "floor")
        {
            anim.SetBool("areYouJump", false);
            areYouJump = false;

        }
    }

    
    void OnTriggerEnter(Collider other)
    {   //item 먹기
        if (other.tag == "Item") {
            Item item = other.GetComponent<Item>();
            switch (item.enumType)
            {
                case Item.type.Ammo:
                    ammo += item.value;
                    if(ammo > maxAmmo)
                    {
                        ammo = maxAmmo;
                    }
                    break;
                case Item.type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                    {
                        coin = maxCoin;
                    }
                    break;
                case Item.type.Grenade:
                    //1.수류탄갯수확인 4이면 return 
                    if (hasGrenade == maxHasGrenade) return;
                    //2.먹은수류탄 활성화
                    grenades[hasGrenade].SetActive(true);
                    //3.갯수 +1 : 배열의 0번째부터 활성화가 되므로 활성화를 시킨후 갯수를 더해준다 
                    hasGrenade += item.value;
                    break;
                case Item.type.Heart:
                    health += item.value;
                    if(health >  maxHealth)
                    {
                        health = maxHealth;
                    }
                    break;

            }
            Destroy(other.gameObject);
        }
        //몬스터한에 공격 당할때
        else if(other.tag == "EnemyAttackArea")
        {
            if (!isDamaged)
            {
                EattackArea eattackArea = other.GetComponent<EattackArea>();
                health -= eattackArea.damage;
                StartCoroutine(OnDamaged());
                if(other.GetComponent<Rigidbody>() != null)
                {
                    Destroy(other.gameObject);
                }
            }
            
        }
    }
    //몬스터 공격 당할때 코루틴 
    IEnumerator OnDamaged()
    {
        isDamaged = true;
        foreach(MeshRenderer mesh in meshes)
        {
            mesh.material.color = Color.green;
        }
        yield return new WaitForSeconds(0.5f);
        foreach (MeshRenderer mesh in meshes)
        {
            mesh.material.color = Color.white;
        }
        isDamaged = false;
    }

    //2.무기획득 및 교체 
    void interact() {
        //조건:nearObj의 정보가 있고 e키를 눌렀을때
        if (nearObj != null && eKey) {
            //Item Class의 value를 쓰기위해 nearObj에 초기화(지금 얘네들은 weapon이라 value값이 있음)
            Item item = nearObj.GetComponent<Item>();
            //아이템벨류값을 int로 저장(배열에 쓰기 위해)
            int weaponValue = item.value;
            //획득한 무기를 배열 bool값으로 저장
            hasWeapon[weaponValue] = true;
            //획득후 무기 삭제
            Destroy(nearObj);
        }
    }
    //3.무기 착용
    void showWeapon() {
        //조건 1:hasWeapon이 false면 무기가 나와선 안된다
        //조건 2: 원레가지고 있는 무기인덱스와 키르누른인덱스가 같으면 리턴한다
        // 생성된 equipObjIndex의 기본값은 0 이므로 -1로 초기화한다 
        if (oneKey && (!hasWeapon[0] || equipObjIndex == 0)) return;
        if (twoKey && (!hasWeapon[1] || equipObjIndex == 1)) return;
        if (threeKey && (!hasWeapon[2] || equipObjIndex == 2)) return;

        //3-1기본 웨폰인덱스를 -1로 설정한후 각 키를 누를때마다 고유인텍스 부여
        int weaponIndex = -1;
        if (oneKey) weaponIndex = 0;
        if (twoKey) weaponIndex = 1;
        if (threeKey) weaponIndex = 2;

        if (oneKey || twoKey || threeKey) {
            //키를 눌렀을때 현제 무기가 있으면 없애준다.
            if (equipObj !=null) {
                equipObj.gameObject.SetActive(false);}
            //equipObjIndex는 현제 키를 눌렀을때 인덱스번호이다.
            equipObjIndex = weaponIndex;
            //equipObj는 현제 키를 눌렀을때 보여지는 무기 인덱스번호이다.
            equipObj = weapons[weaponIndex].GetComponent<Weapon>();
            //2.가지고있는 무기 인덱스를 활성화
            equipObj.gameObject.SetActive(true);
            anim.SetTrigger("doSwap");
        }
    }
    //1.무기와 닿아 있을때(중요) 정보 nearObj에 담음
     void OnTriggerStay(Collider other)
    {
        if(other.tag == "Weapon")
        {
            nearObj = other.gameObject;
        }    
    }
     void OnTriggerExit(Collider other)
    {
        if(other.tag == "Weapon")
        {
            nearObj = null;
        }    
    }

    void Attack()
    {
        if(equipObj == null)
        {
            return;
        }
        fireDelay += Time.deltaTime;
        isFireReady = fireDelay > equipObj.rate;
        if(fireKey && isFireReady && !areYouDodge)
        {
            equipObj.use();
            anim.SetTrigger(equipObj.weaponType == Weapon.type.melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }
    void Reload()
    {
        if (equipObj == null) return;//가진 무기가 없으면 리턴 
        if (equipObj.weaponType == Weapon.type.melee) return;//가진무기가 근접이면 리턴 
        if (ammo == 0) return;//가진총알이 0 이면 리턴 

        if(rKey && !areYouDodge && !areYouJump)//r 키를 누르고 점프, 닫지가 아닐때 
        {
            anim.SetTrigger("doReload");//재장전모션추가 
            isReload = true;//isReload bool 값 추가 
            Invoke("reloadOut", 2f);//reladOut이 2초후에 시작 
        }
    }
    void reloadOut()//재장전으로 총알이 추가되는 함수
    {
        int reAmmo = ammo + equipObj.curAmmo < equipObj.maxAmmo ? ammo : equipObj.maxAmmo-equipObj.curAmmo;
        //총알 5개 그 무기의 현재 총알 10개 < 그 무기의 맥스총알 30 면 5개
        //총알 100개 그무기현재총알 10개 < 그무기맥스총알 30개 면 20개 
        equipObj.curAmmo += reAmmo;
        ammo -= reAmmo;
        isReload = false;
    }

    private void FixedUpdate()
    {
    //플레이어 충돌시 빙빙도는 현상을 막는 로직 
        stopTrunPlayer();
        DontGoWall();
    }

    //플레이어 도는 현상 막음
    private void stopTrunPlayer()
    {
        rigid.angularVelocity = Vector3.zero;
    }
    //벽통과 현상 막음
    private void DontGoWall()
    {
        Debug.DrawRay(transform.position, moveVec*3, Color.green);
        isBorder = Physics.Raycast(transform.position, moveVec, 3, LayerMask.GetMask("Wall"));//Wall에 데이면 true가 됨
        //true일때 playerMove에 제한사항 걸면 됨
    }
    void ThrowGrenade()
    {
        if (hasGrenade == 0) return;

        if (grenadeKey)
        {
            Ray ray = followC.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if(Physics.Raycast(ray, out raycastHit, 100))
            {
                Vector3 grenadeVec = (raycastHit.point - transform.position).normalized;
                grenadeVec.y = 1.5f;
                GameObject grenadInst = Instantiate(grenadObj, transform.position, transform.rotation);
                Rigidbody rigidGrenad = grenadInst.GetComponent<Rigidbody>();
                rigidGrenad.AddForce(grenadeVec*10, ForceMode.Impulse);
                rigidGrenad.AddTorque(Vector3.up * 10, ForceMode.Impulse);
                hasGrenade--;
                grenades[hasGrenade].SetActive(false);
            }
        }
    }
}

