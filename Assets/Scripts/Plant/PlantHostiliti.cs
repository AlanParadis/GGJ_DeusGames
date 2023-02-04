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
    [SerializeField] PlantType plantType;
    [SerializeField] List<PlantType> ennemyType;
    [SerializeField] public GameObject nearest;
    public List<GameObject> targets = new List<GameObject>();
    float dist;
    // Start is called before the first frame update
    void Start()
    {
        dist = GetComponent<Plant>().GetDist();
        InvokeRepeating("CheckEnnemyOverlap", 0, .5f);
    }
    // Update is called once per frame
    void Update()
    {

    }
    void checkNearest()
    {
        if (targets.Count > 0)
        {
            nearest = targets[0];
            float betterDist = Vector3.Distance(nearest.transform.position, transform.position);
            foreach (var _target in targets)
            {
                float tempDist = Vector3.Distance(_target.transform.position, transform.position);
                if (tempDist < betterDist)
                {
                    betterDist = tempDist;
                    nearest = _target;
                }
            }
        }
    }
    bool checkIsEnnemy(PlantType _type)
    {
        for (int i = 0; i < ennemyType.Count; i++)
        {
            if (_type == ennemyType[i])
                return true;
        }
        return false;
    }

    void CheckEnnemyOverlap()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, dist, layerMask);
        List<GameObject> temp = new List<GameObject>();

        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (checkIsEnnemy(colliders[i].GetComponent<PlantHostiliti>().plantType))
                {
                    temp.Add(colliders[i].gameObject);
                }
            }
        }
        targets.Clear();
        targets = temp;
        checkNearest();
    }
}
