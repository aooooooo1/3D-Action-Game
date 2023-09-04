using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeExplosion : MonoBehaviour
{
    public GameObject mesh;
    public GameObject effect;
    public Rigidbody rigid;
    void Start()
    {
        StartCoroutine(Explosion());
    }
    private void Update()
    {
        SphereCollider sColl = GetComponent<SphereCollider>();
        if(sColl != null)
        {
            Vector3 center = transform.TransformPoint(sColl.center);
            float radius = sColl.radius;
            Debug.DrawLine(center + Vector3.up * radius, center - Vector3.up * radius, Color.yellow);
            Debug.DrawLine(center + Vector3.forward * radius, center - Vector3.forward * radius, Color.yellow);
            Debug.DrawLine(center + Vector3.right * radius, center - Vector3.right * radius, Color.yellow);
        }
    }
    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(2.3f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        mesh.SetActive(false);
        effect.SetActive(true);
        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, 6, Vector3.up, 0f, LayerMask.GetMask("Enemy"));
        
        foreach(RaycastHit hitObj in raycastHits)
        {
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }
        Destroy(gameObject, 1);
    }
}
