using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeCanvas : MonoBehaviour
{
    public static LifeCanvas instance;

    public Canvas canvas;
    public PlayerController playerController;
    public GameObject lifeBarPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            return;
        }

        Destroy(gameObject);
    }
}
