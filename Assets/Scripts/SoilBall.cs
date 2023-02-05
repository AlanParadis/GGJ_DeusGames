using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilBall : MonoBehaviour
{
    public GameObject soilPrefab;
    public Item item;

    public void Throw(Vector3 _dir, Item _it)
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.AddForce(_dir, ForceMode.Impulse);

        item = _it;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            return;

        GetComponent<Collider>().enabled = false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 4))
        {
            GameObject go = Instantiate(soilPrefab);

            Gatherable.GatherableItem it;
            it.chance = 100;
            it.dropAmount = 1;
            it.resource = Instantiate(item);

            go.transform.position = hit.point;
            go.GetComponent<Gatherable>().itemPool.Clear();
            go.GetComponent<Gatherable>().itemPool.Add(it);
        }

        SoundManager.Instance.PlaySound(SoundManager.Instance.soilSplash, transform.position);
        
        Destroy(gameObject);
    }
}
