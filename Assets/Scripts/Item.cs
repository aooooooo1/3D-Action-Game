using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum type1 {Ammo, Coin , Grenade , Heart, Weapon};
    public type1 enumType;
    public int value;

    private void Update()
    {
        transform.Rotate(Vector3.up * 50 * Time.deltaTime);
    }

}
