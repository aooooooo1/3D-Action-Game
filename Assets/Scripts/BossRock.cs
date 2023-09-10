using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : EattackArea
{
    Rigidbody rigid;
    float localScaleValue = 0.1f; //바위가 커지는 벨류
    public float torquePower = 10f; //X축 회전력
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.useGravity = false; // 초기에 중력 비활성화

        StartCoroutine(GainScale());//크기를 키운다
        StartCoroutine(StartRolling()); // 2초후 굴러간다
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall") Destroy(gameObject);
    }

    IEnumerator GainScale() //크기 커지는 코루틴
    {
        while (!rigid.useGravity) //공중에 떠있을때 크기 커짐!
        {
            localScaleValue += 0.5f *Time.deltaTime;//deltaTime은 상위에 걸어둔다
            transform.localScale=Vector3.one * localScaleValue; //크기 커짐!
            yield return null;
        }
    }
    private void Update()
    {
        SphereCollider sColl = GetComponent<SphereCollider>();
        if (sColl != null)
        {
            Vector3 center = transform.TransformPoint(sColl.center);
            float radius = 40f;
            Debug.DrawLine(center + Vector3.up * radius, center - Vector3.up * radius, Color.yellow);
            Debug.DrawLine(center + Vector3.forward * radius, center - Vector3.forward * radius, Color.yellow);
            Debug.DrawLine(center + Vector3.right * radius, center - Vector3.right * radius, Color.yellow);
        }
    }
    IEnumerator StartRolling() //굴러가는 코루틴
    {
        yield return new WaitForSeconds(2f); // 2초 후에 굴러가도록 시작

        rigid.useGravity = true; // 중력 활성화, 땅으로 바위 내려옴
        rigid.AddTorque(Vector3.up * torquePower, ForceMode.VelocityChange); // X축 주위로 회전력 추가

        Vector3 playerDirection = Vector3.zero;

        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position,
                                                    40f,
                                                    transform.forward,
                                                    40f,
                                                    LayerMask.GetMask("Player"));
        foreach (RaycastHit hitObj in raycastHits)
            playerDirection = (hitObj.transform.position - transform.position).normalized;

        // 플레이어 방향으로 바위가 굴러가도록 설정
        rigid.velocity = playerDirection * 10f;
    }


}
