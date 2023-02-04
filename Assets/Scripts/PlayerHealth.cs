using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] Image lifeBar;
    [SerializeField] private Canvas deathCanvas;
    [SerializeField] private TMP_Text respawnTimerText;
    [SerializeField] private float respawnTime = 3.0f;
    [SerializeField] private float maxTimeBeforeRegainHealth = 5.0f;
    private float currentTimeBeforeRegainHealth = 0.0f;
    private bool canRegainHealth = true;
    //per seconds
    [SerializeField] private float regainHealthAmount = 5.0f;
    private Vector3 spawnPointPos;
    
    private float health;
    private bool isDead = false;

    public float maxHealth = 100.0f;
    // Start is called before the first frame update
    void Start()
    {
        spawnPointPos = gameObject.transform.position;
        
        health = maxHealth; 
        
        if(deathCanvas == null)
            Debug.LogError("deathCanvas reference is null !");
        
        if(respawnTimerText == null)
            Debug.LogError("respawnTimerText reference is null !");
        
        deathCanvas.gameObject.SetActive(false);
    }

    void UpdateTimeBeforeRegainHealth()
    {
        if (!isDead && !canRegainHealth && health < maxHealth)
        {
            currentTimeBeforeRegainHealth -= Time.deltaTime;
            if (currentTimeBeforeRegainHealth <= 0.0f)
            {
                currentTimeBeforeRegainHealth = 0.0f;
                canRegainHealth = true;
            }
        }
    }
    
    void Update()
    {
        UpdateTimeBeforeRegainHealth();
        PassivelyRegainHealth();
    }
    
    void UpdateUI()
    {
        lifeBar.fillAmount = health/maxHealth;
    }
    
    public void TakeDamage(float amount)
    {
        if (isDead)
            return;
        
        health -= amount;
        health = Mathf.Clamp(health,0, 100);
        UpdateUI();

        if (health <= 0.0f)
        {
            Death();
        }

        canRegainHealth = false;
        currentTimeBeforeRegainHealth = maxTimeBeforeRegainHealth;
    }

    void PassivelyRegainHealth()
    {
        if (!isDead && canRegainHealth)
        {
            health += regainHealthAmount * Time.deltaTime;
            if (health > maxHealth)
            {
                health = maxHealth;
                canRegainHealth = false;
            }

            UpdateUI();
        }
    }
    
    void Death()
    {
        if (isDead)
            return;

        StartCoroutine(DeathCoroutine());
    }

    void Respawn()
    {
        gameObject.transform.position = spawnPointPos;
    }
    
    IEnumerator DeathCoroutine()
    {
        isDead = true;

        deathCanvas.gameObject.SetActive(true);

        float currentTimer = respawnTime;
        while (currentTimer > 0.0f)
        {
            currentTimer -= Time.deltaTime;
            if (currentTimer < 0.0f)
                currentTimer = 0.0f;

            respawnTimerText.text = ((int)currentTimer).ToString();
            
            yield return null;
        }
        
        deathCanvas.gameObject.SetActive(false);

        Respawn();

        health = maxHealth;
        UpdateUI();

        isDead = false;
    }
}
