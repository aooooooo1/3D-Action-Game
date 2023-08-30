using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //무기가 가져야할 속성 : 근거리 원거리 선택 , 데미지 , 공격범위 , 공격속도 , 이펙트 - 총 5개
    public enum type { melee, range};
    public type weaponType;
    public int damage;
    public float rate;
    public BoxCollider weaponArea;
    public TrailRenderer trailRenderer;
    
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
    }
    //무기자체가 범위, 이펙트를 가지므로 weapon에서 기능 만들어줌
    IEnumerator swing()
    {
        yield return new WaitForSeconds(0.1f);
        weaponArea.enabled = true;
        trailRenderer.enabled = true;
        yield return new WaitForSeconds(0.3f);
        trailRenderer.enabled = false;
        yield return new WaitForSeconds(0.3f);
        weaponArea.enabled = false;
        yield break;
    }
}
