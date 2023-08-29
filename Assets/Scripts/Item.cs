using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum type {Ammo, Coin , Grenade , Heart, Weapon};

    public type enumType;
    public int value;

    private void Update()
    {
        transform.Rotate(Vector3.up * 50 * Time.deltaTime);
    }

}
