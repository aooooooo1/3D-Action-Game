using UnityEngine;
using System.Collections;

public class EattackArea : MonoBehaviour
{
    public int damage;
    public bool isMi;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall" && isMi) Destroy(gameObject);
    }
}
