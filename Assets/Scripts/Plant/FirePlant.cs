using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePlant : Plant, IInteractable
{
    Animator anim;
    [SerializeField] Item item;
    [SerializeField] GameObject player;
    [SerializeField] float distMin;
    [SerializeField] float dammage;
    [SerializeField] float cd;
    float actualcd;

    public void DoInteraction()
    {
        if (!GetComponent<PlantScript>().isSauvage)
            return;
        Item it = InventoryController.Instance.inventory.AddItem(item);

        if (it == null)
            Destroy(gameObject);
        else
            item = it;
    }

    public void SetInteractionText()
    {
        if (!GetComponent<PlantScript>().isSauvage)
            return;
        ActionUI.Instance.SetVisible();

        ActionUI.Instance.SetText($"Press E to pick {item.displayName}");
    }
    private void Awake()
    {
        actualcd = 0f;
        item.currentStackAmount = 1;
        GetComponent<PlantScript>().isSauvage = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        //anim = GetComponent<Animator>();
        //anim.SetFloat("Dist", int.MaxValue);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GetComponent<PlantScript>().isSauvage)
            return;
        if (actualcd >= 0)
            actualcd -= Time.deltaTime;

        if (Vector3.Distance(transform.position, player.transform.position) <= distMin)
        {
            if (actualcd <= 0)
            {
                player.GetComponent<PlayerHealth>().health -= dammage;
                actualcd = cd;
            }
        }
    }
}
