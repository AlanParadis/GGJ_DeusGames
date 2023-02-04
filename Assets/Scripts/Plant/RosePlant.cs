using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RosePlant : Plant
{
    Animator anim;
    PlantHostiliti plantHost;
    bool hitPlayer;

    // Start is called before the first frame update
    void Start()
    {
        hitPlayer = false;
        anim = GetComponent<Animator>();
        anim.SetFloat("Dist", int.MaxValue);
        plantHost = GetComponent<PlantHostiliti>();
    }
    public void RoseHit()
    {
        if (hitPlayer)
            m_playerHealth.TakeDamage(damage);
        else
            plantHost.nearest.gameObject.GetComponent<Plant>().TakeDamage(damage);

    }

    void RoseAction(GameObject _go)
    {
        float dist = Vector3.Distance(transform.position, _go.transform.position);
        anim.SetFloat("Dist", dist);
        if (dist < distMin)
            transform.LookAt(_go.transform.position);
    }

    protected override void DoPlantAction()
    {
        base.DoPlantAction();
        if (plantHost.nearest != null)
        {
            float distMobs = Vector3.Distance(transform.position, m_playerController.transform.position);
            float distPlayer = Vector3.Distance(transform.position, m_playerController.transform.position);
            if (distMobs > distPlayer)
            {
                RoseAction(m_playerController.gameObject);
                hitPlayer = true;
            }
            else
            {
                RoseAction(plantHost.nearest.gameObject);
                hitPlayer = false;
            }
        }
        else
        {
            RoseAction(m_playerController.gameObject);
            hitPlayer = true;
        }
        
    }
}
