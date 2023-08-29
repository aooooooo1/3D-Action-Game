using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    //수류탄 만들어 놓고 오기 : 필요한것 : 돌기 위한 중심, 도는 속도 , 도는 거리
    public Transform target;
    public float rotSpeed;
    Vector3 offset;

    void Start()
    {
        //수류탄위치에서 player위치만큼 빼준다.
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        //수류탄위치는 player위치에서 조금 더해준다
        transform.position = target.position + offset;
        transform.RotateAround(target.position, Vector3.up, rotSpeed * Time.deltaTime);
        //변한 수류탄위치를 다시 업뎃해준다
        offset = transform.position - target.position;
    }
}
