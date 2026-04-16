using System;
using System.Collections;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public HealthUI healthUI;

    private SpriteRenderer spriteRenderer;

    public static event Action OnPlayedDied;

    void Start()
    {
        ResetHealth();

        spriteRenderer = GetComponent<SpriteRenderer>();
        Gam.OnReset += ResetHealth;
        HealthItem.onHealthCollect += Heal;
        

    }

    void OnTriggerEnter2D(Collider2D collision)//coz we use tritgger 2d
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy)
        {
            //if we colided with an enemy
            TakeDamage(enemy.damage);
        }
        Trap trap = collision.GetComponent<Trap>();
        if (trap && trap.damage > 0)
        {
            TakeDamage(trap.damage);
        }
    }

    void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        healthUI.UpdateHearts(currentHealth); 
    }

    void ResetHealth()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthUI.UpdateHearts(currentHealth);

        //flash red after taking damage
        StartCoroutine(FlashRed());


        if (currentHealth <= 0)
        {
            //player dead -- call the game over scene or animation etc
            OnPlayedDied.Invoke();

        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }
}
