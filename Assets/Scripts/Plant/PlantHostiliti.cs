using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantHostiliti : MonoBehaviour
{
    enum PlantType
    {
        Love,
        Rose,
        Fire,
        Ice,
        Radioactive
    }
    [SerializeField] LayerMask layerMask;
    [SerializeField] GameObject parent;
    [SerializeField] PlantType plantType;
    [SerializeField] List<PlantType> ennemyType;
    public List<GameObject> targets = new List<GameObject>();
    float dist;
    // Start is called before the first frame update
    void Start()
    {
        dist = parent.GetComponent<Plant>().GetDist();
        InvokeRepeating("CheckEnnemyOverlap", 0, .5f);
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    void CheckEnnemyOverlap()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, dist, layerMask);
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
