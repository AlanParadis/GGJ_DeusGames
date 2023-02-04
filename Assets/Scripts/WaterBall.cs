using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBall : MonoBehaviour
{
    public void Throw(Vector3 _dir)
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.AddForce(_dir, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            return;

        Destroy(gameObject);
    }
}
