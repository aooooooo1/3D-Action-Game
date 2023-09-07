using UnityEngine;
using System.Collections;

public class EattackArea : MonoBehaviour
{
    public int damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall") Destroy(gameObject);
    }
}
