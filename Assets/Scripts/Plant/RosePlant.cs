using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class RosePlant : Plant
{
    Animator anim;
    public bool hit;
    Vector3 posInit;
    Quaternion rotInit;
    

    // Start is called before the first frame update
    void Start()
    {
        posInit = new Vector3();
        rotInit = new Quaternion();
        posInit = transform.position;
        rotInit = transform.rotation;
        anim = GetComponent<Animator>();
        anim.SetFloat("Dist", int.MaxValue);
    }
    public void RoseHit()
    {
        m_playerHealth.TakeDamage(damage);
    }

    protected override void DoPlantAction()
    {
        base.DoPlantAction();
        
        float dist = Vector3.Distance(transform.position, m_playerController.transform.position);
        anim.SetFloat("Dist", dist);
        if (dist < distMin)
            transform.LookAt(m_playerController.transform.position);
        else
        {
            transform.position = posInit;
            transform.rotation = rotInit;
        }
    }
}
