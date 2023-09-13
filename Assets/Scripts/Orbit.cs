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
        //수류탄위치에서 player위치만큼 빼준다. -> 수류탄과 플레이어 사이의 거리가 나온다.
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        //공전수류탄 위치는 player위치에서 조금 더해준다 -> 이렇게하면 공전수류탄은 위치가 고정된다
        transform.position = target.position + offset;
        //플레이어 주위를 공전하는 함수 RotateAroud (기준 , 기준축 , 속도), 플레이어가 기준에서 벗어나버리면 오류가 난다 
        transform.RotateAround(target.position, Vector3.up, rotSpeed * Time.deltaTime);
        //변한 수류탄위치를 다시 업뎃해준다
        offset = transform.position - target.position;  
    }
}
