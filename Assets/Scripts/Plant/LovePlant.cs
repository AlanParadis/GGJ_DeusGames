using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class LovePlant : Plant
{
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetFloat("Dist", int.MaxValue);
    }

    protected override void DoPlantAction()
    {
        base.DoPlantAction();
        
        anim.SetFloat("Dist", Vector3.Distance(transform.position, m_playerController.transform.position));
    }
}
