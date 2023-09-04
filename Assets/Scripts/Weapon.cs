using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    //무기가 가져야할 속성 : 근거리 원거리 선택 , 데미지 , 공격범위 , 공격속도 , 이펙트 - 총 5개
    public enum type { melee, range};
    public type weaponType;
    public int damage;
    public float rate;
    public BoxCollider weaponArea;
    public TrailRenderer trailRenderer;

    //총,탄피의 포지션과 프리펩에서 갖다 쓸 옵젝변수추가
    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;

    public int maxAmmo; //최대탄창갯수
    public int curAmmo; //현재탄창개수
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void use()
    {   
        if(weaponType == type.melee)
        {
            StopCoroutine("swing");
            StartCoroutine("swing");
        }
        else if (weaponType == type.range && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine("shot");
        }
    }
    //무기자체가 범위, 이펙트를 가지므로 weapon에서 기능 만들어줌
    IEnumerator swing()
    {
        yield return new WaitForSeconds(0.4f); //0.1초 후에 무기범위,이펙트효과 활성화
        weaponArea.enabled = true;
        trailRenderer.enabled = true;
        yield return new WaitForSeconds(0.7f); //0.3초 후에 효과 비활성화
        trailRenderer.enabled = false;
        yield return new WaitForSeconds(0.1f);//0.3초 후에 무기범위 비활성화
        weaponArea.enabled = false;
        yield break; //종료
    }

    IEnumerator shot() //shot코루틴 원거리공격 
    {
        //instantiate로 객체 생성 1.생성할객체 2.객체의위치 3.객체의각도 
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        //생성한 객체에 물리효과를 추가한다
        Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
        //물리효과를 받으면 velocity를 추가할수 있으니 forward 방향으로 추가한다 
        rigidBullet.velocity = bulletPos.forward * 50;

        yield return null;//한턴쉬고 
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody rigidCase = instantBullet.GetComponent<Rigidbody>();
        // 위치를 지정한다 
        Vector3 caseVac = bulletCasePos.forward * Random.Range(1, 3) + Vector3.up * Random.Range(1,5);
        rigidCase.AddForce(caseVac, ForceMode.Impulse);

    }

}
