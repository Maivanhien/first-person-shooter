using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float delay = 4f;
    public float radius = 5f;
    public float force = 150f;
    public float throwForce = 40f;
    public LayerMask mask;
    public GameObject explosionEffect;
    float countDown;

    // Start is called before the first frame update
    void Start()
    {
        countDown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countDown -= Time.deltaTime;
        if(countDown <= 0f)
        {
            Explode();
        }
    }

    void Explode()
    {
        // show effect
        Instantiate(explosionEffect, transform.position, transform.rotation);
        //get nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, mask);
        foreach(Collider nearbyObject in colliders)
        {
            //Add force
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(force, transform.position, radius);
            }
            //Damage
        }

        Destroy(gameObject);
    }

    public void ThrowGrenade(Vector3 direction)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(direction * throwForce, ForceMode.VelocityChange);
    }
}
