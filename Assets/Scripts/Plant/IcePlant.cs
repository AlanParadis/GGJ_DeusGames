using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePlant : Plant
{
    [SerializeField] GameObject iceBall;
    [SerializeField] float nbIceBallGenerate;
    PlantHostiliti plantHost;
    void Start()
    {
        plantHost = GetComponent<PlantHostiliti>();
    }

    public void Invocation(GameObject _target)
    {
        for (int i = 0; i < nbIceBallGenerate; i++)
        {
            GameObject go = Instantiate(iceBall, transform.position + Random.onUnitSphere + Vector3.up, transform.rotation);
            go.GetComponent<IceProjectiles>().target = _target;
        }
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
                if (Vector3.Distance(transform.position, m_playerController.transform.position) <= distMin)
                {
                    Invocation(m_playerController.gameObject);
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, plantHost.nearest.transform.position) <= distMin)
                {
                    Invocation(plantHost.nearest.gameObject);
                }
            }
        }
    }
}