using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rake : Holdable
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform muzzle;

    private void Update()
    {
        animator.SetBool("Fire", ManagersManager.instance.inputManager.Inputs.PlayerGround.PrimaryAction.ReadValue<float>() > .3f);
    }

    public void Attack()
    {
        Collider[] cols = Physics.OverlapSphere(muzzle.position, .6f);

        foreach (Collider col in cols)
        {
            Debug.Log(col.gameObject.name);

            if (!col.gameObject.CompareTag("Plant"))
                continue;

            Plant plant = col.gameObject.GetComponent<Plant>();
            plant.TakeDamage(1);
        }
    }
}
