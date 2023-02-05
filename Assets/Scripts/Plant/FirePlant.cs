using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePlant : Plant
{
    PlantHostiliti plantHost;

    // Start is called before the first frame update
    void Start()
    {
        plantHost = GetComponent<PlantHostiliti>();
    }
    protected override void DoPlantAction()
    {
        base.DoPlantAction();
        if (plantHost.nearest != null)
        {
            float distMobs = Vector3.Distance(transform.position, plantHost.nearest.transform.position);
            float distPlayer = Vector3.Distance(transform.position, m_playerController.transform.position);
            if (distMobs > distPlayer)
            {
                if (Vector3.Distance(transform.position, m_playerController.transform.position) <= distMin)
                {
                    m_playerHealth.TakeDamage(damage);
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, plantHost.nearest.transform.position) <= distMin)
                {
                    plantHost.nearest.gameObject.GetComponent<Plant>().TakeDamage(damage);
                }
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, m_playerController.transform.position) <= distMin)
            {
                m_playerHealth.TakeDamage(damage);
            }
        }
    }
}
