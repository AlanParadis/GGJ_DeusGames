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
       Item it = InventoryController.Instance.inventory.AddItem(item);
       
       if (it == null)
           Destroy(gameObject);
       else
           item = it;
    }

    public void SetInteractionText()
    {
        ActionUI.Instance.SetVisible();

        ActionUI.Instance.SetText($"Press E to pick {item.displayName}");
    }
    private void Awake()
    {

        item.currentStackAmount = 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        isSauvage = true;
        anim = GetComponent<Animator>();
        anim.SetFloat("Dist", int.MaxValue);
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("Dist", Vector3.Distance(transform.position, player.position));
    }
}
