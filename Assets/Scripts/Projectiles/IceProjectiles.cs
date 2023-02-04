using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class IceProjectiles : MonoBehaviour
{
    public GameObject target;
    [SerializeField] float speed;
    [SerializeField] float dammage;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        transform.LookAt(target.transform);
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity += transform.forward * speed;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            target.GetComponent<PlayerHealth>().TakeDamage(dammage);
        }
        else if (collision.transform.tag == "Plant")
        {
            target.GetComponent<Plant>().TakeDamage(dammage);
        }
        Destroy(gameObject);
    }
}
