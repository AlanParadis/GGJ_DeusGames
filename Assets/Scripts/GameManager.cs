using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    public int photocoin;
    private void Awake()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
        photocoin = 0;
    }
}
