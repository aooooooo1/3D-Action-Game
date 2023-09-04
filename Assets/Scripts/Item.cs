using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum type {Ammo, Coin , Grenade , Heart, Weapon};

    public type enumType;
    public int value;

    Rigidbody rigid;
    SphereCollider sphereCollider;

     void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * 50 * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "floor")
        {
            rigid.isKinematic = true;
            sphereCollider.enabled = false;
        }
    }
}
