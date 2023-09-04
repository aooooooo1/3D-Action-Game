 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    //총알이 ~에 충돌시
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "floor")
        {
            Destroy(gameObject, 4);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall") Destroy(gameObject);
    }
}
