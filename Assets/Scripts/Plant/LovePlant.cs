using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class LovePlant : MonoBehaviour, IInteractable
{
    Animator anim;
    bool isSauvage;
    [SerializeField] Item item;
    [SerializeField] Transform player;

    public void DoInteraction()
    {
        isSauvage = GetComponent<PlantScript>().isSauvage;
        if (!isSauvage)
            return;
        Item it = InventoryController.Instance.inventory.AddItem(item);
       
       if (it == null)
           Destroy(gameObject);
       else
           item = it;
    }

    public void SetInteractionText()
    {
        isSauvage = GetComponent<PlantScript>().isSauvage;
        if (!isSauvage)
            return;
        ActionUI.Instance.SetVisible();

        ActionUI.Instance.SetText($"Press E to pick {item.displayName}");
    }
    private void Awake()
    {
        item.currentStackAmount = 1;
        GetComponent<PlantScript>().isSauvage = true;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetFloat("Dist", int.MaxValue);
    }

    // Update is called once per frame
    void Update()
    {
        isSauvage = GetComponent<PlantScript>().isSauvage;
        if (!isSauvage)
            return;
        anim.SetFloat("Dist", Vector3.Distance(transform.position, player.position));
    }
}
