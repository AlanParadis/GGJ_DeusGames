using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TreeEditor;
using UnityEngine;

public class RosePlant : MonoBehaviour, IInteractable
{
    Animator anim;
    bool isSauvage;
    [SerializeField] Item item;
    [SerializeField] Transform player;
    [SerializeField] float distMin;
    [SerializeField] float dammage;
    [SerializeField] float cd;
    float actualcd;
    public bool hit;
    Vector3 posInit;
    Quaternion rotInit;

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
        posInit = new Vector3();
        rotInit = new Quaternion();
        posInit = transform.position;
        rotInit = transform.rotation;
        isSauvage = true;
        anim = GetComponent<Animator>();
        anim.SetFloat("Dist", int.MaxValue);
    }
    public void RoseHit()
    {
        player.GetComponent<PlayerHealth>().health -= dammage;
    }

    // Update is called once per frame
    void Update()
    {
        if (actualcd >= 0)
            actualcd -= Time.deltaTime;

        float dist = Vector3.Distance(transform.position, player.position);
        anim.SetFloat("Dist", dist);
        if (dist < distMin)
            transform.LookAt(player.position);
        else
        {
            transform.position = posInit;
            transform.rotation = rotInit;
        }
    }
}
