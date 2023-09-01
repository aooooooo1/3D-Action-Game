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

    //근접무기공격할때 쓰이는 정보
    bool fireKey;
    float fireDelay;
    bool isFireReady;


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
        interact();
        showWeapon();
        Attack();
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
    }
    void playerMove()
    {
        //입력한 키의 방향 moveVec로 설/
        moveVec = new Vector3(h, 0, v).normalized;
        if (areYouDodge)//Dodge만들고 봐야함
        {
            moveVec = dodgeVec; //이동할때 닷지하면 이동방향으로 닫지방향이 결정된다
        }
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

    //item 먹기
    void OnTriggerEnter(Collider other)
    {
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


}

