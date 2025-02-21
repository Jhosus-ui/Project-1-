using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 4; 
    private int currentHealth; 
    public float immunityTime = 1f; 
    private bool isImmune = false; 

    public TMP_Text healthText; 

    private void Start()
    {
        currentHealth = maxHealth; 
        UpdateHealthUI(); 
        Debug.Log("Vida inicial del jugador: " + currentHealth);
    }
    public void TakeDamage(int damageAmount)
    {
        if (isImmune) return;

        currentHealth -= damageAmount; 
        Debug.Log("El jugador recibió daño. Vida actual: " + currentHealth);
        UpdateHealthUI(); 

        if (currentHealth <= 0)
        {
            Die(); 
        }
        else
        {
            StartCoroutine(ActivateImmunity()); 
        }
    }
    private IEnumerator ActivateImmunity()
    {
        isImmune = true; 
        Debug.Log("Inmunidad activada.");

        yield return new WaitForSeconds(immunityTime); 

        isImmune = false; 
        Debug.Log("Inmunidad desactivada.");
    }
    private void Die()
    {
        Debug.Log("El jugador ha muerto.");
        GameManager.Instance.PlayerDied(); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) 
        {
            TakeDamage(1); 
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "Lives: " + currentHealth;
        }
    }
}