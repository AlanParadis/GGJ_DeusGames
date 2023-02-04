using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] Image lifeBar;
    public float health;
    // Start is called before the first frame update
    void Start()
    {
        health = 100;    
    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health,0, 100);
        lifeBar.fillAmount = health/100;
    }
}
