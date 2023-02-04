using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePlant : Plant
{
    [SerializeField] GameObject iceBall;
    [SerializeField] float nbIceBallGenerate;
    
    public void Invocation(GameObject _target)
    {
        for (int i = 0; i < nbIceBallGenerate; i++)
        {
            GameObject go = Instantiate(iceBall, transform.position + Random.onUnitSphere + Vector3.up, transform.rotation);
            go.GetComponent<IceProjectiles>().player = _target;
        }
    }

    protected override void DoPlantAction()
    {
        base.DoPlantAction();
        
        if (Vector3.Distance(transform.position, m_playerController.transform.position) <= distMin)
        {
            Invocation(m_playerController.gameObject);
        }
    }
}