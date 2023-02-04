using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePlant : Plant, IInteractable
{
    Animator anim;
    [SerializeField] Item item;
    [SerializeField] GameObject player;
    [SerializeField] GameObject iceBall;
    [SerializeField] float nbIceBallGenerate;
    [SerializeField] float distMin;
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

    public void Invocation(GameObject _target)
    {
        actualcd -= Time.deltaTime;
        if (actualcd <= 0)
        {
            for (int i = 0; i < nbIceBallGenerate; i++)
            {
                GameObject go = Instantiate(iceBall, transform.position + Random.onUnitSphere + Vector3.up, transform.rotation);
                go.GetComponent<IceProjectiles>().player = _target;
            }
            actualcd = cd;
        }
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
            Invocation(player);
        }
    }
}