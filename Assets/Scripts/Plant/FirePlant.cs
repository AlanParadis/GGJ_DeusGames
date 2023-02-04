using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePlant : Plant
{
    protected override void DoPlantAction()
    {
        base.DoPlantAction();
        
        if (Vector3.Distance(transform.position, m_playerController.transform.position) <= distMin)
        {
            m_playerHealth.TakeDamage(damage);
        }
    }
}
