using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZone : MonoBehaviour
{
    public GameManager manager; //게임메니저에 있는 스테이지함수를 쓸것이다

    private void OnTriggerEnter(Collider other)
    {
        //만약 존에 플레이어가 들어오면 게임메니져의 스테이지함수를 사용한다
        if(other.tag == "Player")
        {
            manager.StageStart(); 
        }
    }
}
