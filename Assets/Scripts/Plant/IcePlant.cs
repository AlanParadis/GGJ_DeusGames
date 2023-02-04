using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePlant : MonoBehaviour, IInteractable
{
    Animator anim;
    bool isSauvage;
    [SerializeField] Item item;
    [SerializeField] GameObject player;
    [SerializeField] GameObject iceBall;
    [SerializeField] float nbIceBallGenerate;
    [SerializeField] float distMin;
    [SerializeField] float cd;
    float actualcd;

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
        actualcd = 0f;
        item.currentStackAmount = 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        isSauvage = true;
        //anim = GetComponent<Animator>();
        //anim.SetFloat("Dist", int.MaxValue);
    }

    // Update is called once per frame
    void Update()
    {
        if (actualcd >= 0)
            actualcd -= Time.deltaTime;

        if (Vector3.Distance(transform.position, player.transform.position) <= distMin)
        {
            actualcd -= Time.deltaTime;
            if (actualcd <= 0)
            {
                for (int i = 0; i < nbIceBallGenerate; i++)
                {
                    GameObject go = Instantiate(iceBall, transform.position + Random.onUnitSphere + Vector3.up, transform.rotation);
                    go.GetComponent<IceProjectiles>().player = player;
                }
                actualcd = cd;
            }
        }
    }
}