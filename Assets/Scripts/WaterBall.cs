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

        // if layer is Plot
        if (other.gameObject.layer == UnityEngine.LayerMask.NameToLayer("Plot"))
        {
            Plot plot = other.gameObject.GetComponent<Plot>();
            if (plot != null)
            {
                plot.OnWaterBallEnter(this);
            }
        }

        SoundManager.Instance.PlaySound(SoundManager.Instance.waterSplash, transform.position, 1.0f, true, 1.2f);
        
        Destroy(gameObject);
    }
}
