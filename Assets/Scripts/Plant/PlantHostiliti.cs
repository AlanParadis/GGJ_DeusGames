using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantHostiliti : MonoBehaviour
{
    enum Plant
    {
        Love,
        Rose,
        Fire,
        Ice,
        Radioactive
    }
    [SerializeField] GameObject parent;
    [SerializeField] Plant plantType;
    [SerializeField] List<Plant> ennemyType;
    public List<GameObject> targets = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "plant")
        {
            for (int i = 0; i < ennemyType.Count; i++)
            {
                if (other.GetComponent<PlantHostiliti>().plantType == ennemyType[i])
                {
                    targets.Add(other.gameObject);
                    return;
                }
            }
        }
    }
}
