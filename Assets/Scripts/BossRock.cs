using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : EattackArea
{
    Rigidbody rigid;
    float torquePowerValue = 2f;
    float localScaleValue = 0.1f;
    public bool isGain;
    public float torquePower = 10f; // X축 주위의 회전력

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.useGravity = false; // 초기에 중력 비활성화

        StartCoroutine(GainScale());//gain Scale
        // 2초후 굴러간다
        StartCoroutine(StartRolling());
    }

    IEnumerator GainScale()
    {
        while (!rigid.useGravity)
        {
            localScaleValue += 0.5f *Time.deltaTime;//deltaTime은 상위에 걸어둔다
            transform.localScale=Vector3.one * localScaleValue;
            yield return null;
        }
    }

    IEnumerator StartRolling()
    {
        yield return new WaitForSeconds(2f); // 2초 후에 굴러가도록 시작

        rigid.useGravity = true; // 중력 활성화
        rigid.AddTorque(Vector3.right * torquePower, ForceMode.VelocityChange); // X축 주위로 회전력 추가
        rigid.velocity = new Vector3(0f, 0f, 20f); 
    }


}
