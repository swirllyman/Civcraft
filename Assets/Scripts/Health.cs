using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour {

    [Header("Health Info")]
    public Image healthBar;
    public float maxHealth = 5;
    public float currentHealth;

    // Use this for initialization
    void Start () {
        currentHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool TakeDamage(int attackDamage)
    {
        bool KB = false;
        currentHealth -= attackDamage;
        if (currentHealth <= 0)
        {
            KB = true;
            currentHealth = 0;
        }
        healthBar.fillAmount = currentHealth / maxHealth;
        if(healthBar.fillAmount > .66f)
        {
            if (healthBar.color != Color.green)
                healthBar.color = Color.green;
        }
        else if(healthBar.fillAmount > .33f)
        {
            if (healthBar.color != Color.yellow)
                healthBar.color = Color.yellow;
        }
        else
        {
            if (healthBar.color != Color.red)
                healthBar.color = Color.red;
        }

        return KB;
    }
}
