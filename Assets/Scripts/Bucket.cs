using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bucket : Holdable
{
    [Header("References")] 
    [SerializeField] private GameObject waterVisual;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject waterBallPrefab;

    private bool filled;

    private void Update()
    {
        if (ManagersManager.instance.inputManager.Inputs.PlayerGround.PrimaryAction.triggered)
            Use();
    }

    private void Use()
    {
        animator.SetTrigger("Fire");
    }

    public void ThrowWater()
    {
        filled = false;
        waterVisual.SetActive(false);

        GameObject go = Instantiate(waterBallPrefab);
        go.transform.position = transform.position;

        //go.GetComponent<WaterBall>().Throw(owner.LookDir * 10f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            filled = true;
            waterVisual.SetActive(true);
        }
    }
}
