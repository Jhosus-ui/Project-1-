using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 4; // Vida máxima del jugador
    private int currentHealth; // Vida actual del jugador
    public float immunityTime = 1f; // Tiempo de inmunidad después de recibir daño
    private bool isImmune = false; // Indica si el jugador es inmune

    public Image[] heartImages; // Imágenes de los corazones (UI)
    public float normalPulseIntensity = 1.1f; // Intensidad de la palpitación normal (2 corazones)
    public float strongPulseIntensity = 1.2f; // Intensidad de la palpitación fuerte (1 corazón)
    public float pulseSpeed = 2f; // Velocidad de la palpitación

    // Variables para la pantalla de daño
    public Image damageScreen; // Pantalla de daño (UI)
    public float damageScreenBaseAlpha = 0.3f; // Opacidad base de la pantalla de daño
    public float damageScreenStrongAlpha = 0.6f; // Opacidad fuerte de la pantalla de daño
    public float damageScreenPulseIntensity = 1.1f; // Intensidad de la palpitación de la pantalla de daño
    public float damageScreenDuration = 0.5f; // Duración de la pantalla de daño antes de desaparecer

    private void Start()
    {
        currentHealth = maxHealth; // Inicializar la vida al máximo
        UpdateHealthUI(); // Actualizar la UI al inicio
        Debug.Log("Vida inicial del jugador: " + currentHealth);

        // Inicializar la pantalla de daño como transparente
        SetDamageScreenAlpha(0f);
    }

    public void TakeDamage(int damageAmount)
    {
        if (isImmune) return; // Si el jugador es inmune, no recibe daño

        currentHealth -= damageAmount; // Reducir la vida
        Debug.Log("El jugador recibió daño. Vida actual: " + currentHealth);
        UpdateHealthUI(); // Actualizar la UI

        if (currentHealth <= 0)
        {
            Die(); // Si la vida llega a 0, el jugador muere
        }
        else
        {
            StartCoroutine(ActivateImmunity()); // Activar inmunidad temporal
            ShowDamageScreen(); // Mostrar la pantalla de daño
        }
    }

    private IEnumerator ActivateImmunity()
    {
        isImmune = true; // Activar inmunidad
        Debug.Log("Inmunidad activada.");

        yield return new WaitForSeconds(immunityTime); // Esperar el tiempo de inmunidad

        isImmune = false; // Desactivar inmunidad
        Debug.Log("Inmunidad desactivada.");
    }

    private void Die()
    {
        Debug.Log("El jugador ha muerto.");
        GameManager.Instance.PlayerDied(); // Notificar al GameManager que el jugador ha muerto

        // Mantener la pantalla de daño con opacidad máxima
        SetDamageScreenAlpha(damageScreenStrongAlpha);
        StopAllCoroutines(); // Detener todas las corrutinas de palpitación
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) // Si colisiona con un enemigo
        {
            TakeDamage(1); // Recibir daño
        }
    }

    private void UpdateHealthUI()
    {
        // Actualizar la visibilidad de los corazones (sin cambios)
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentHealth)
            {
                heartImages[i].gameObject.SetActive(true); // Mostrar el corazón si tiene vida
            }
            else
            {
                heartImages[i].gameObject.SetActive(false); // Ocultar el corazón si no tiene vida
            }
        }

        // Activar o desactivar la palpitación de los corazones según la vida restante
        if (currentHealth == 2)
        {
            StartCoroutine(PulseHearts(normalPulseIntensity)); // Palpitación suave
        }
        else if (currentHealth == 1)
        {
            StartCoroutine(PulseHearts(strongPulseIntensity)); // Palpitación fuerte
        }
    }

    private IEnumerator PulseHearts(float intensity)
    {
        while (currentHealth <= 2) // Mientras el jugador tenga 2 o menos corazones
        {
            // Escalar los corazones hacia arriba y abajo para simular la palpitación
            foreach (var heart in heartImages)
            {
                if (heart.gameObject.activeSelf)
                {
                    heart.transform.localScale = Vector3.one * intensity;
                }
            }

            yield return new WaitForSeconds(1f / pulseSpeed); // Esperar un momento

            // Volver a la escala normal
            foreach (var heart in heartImages)
            {
                if (heart.gameObject.activeSelf)
                {
                    heart.transform.localScale = Vector3.one;
                }
            }

            yield return new WaitForSeconds(1f / pulseSpeed); // Esperar un momento
        }
    }

    // Métodos para la pantalla de daño
    private void ShowDamageScreen()
    {
        // Mostrar la pantalla de daño con opacidad base
        SetDamageScreenAlpha(damageScreenBaseAlpha);

        // Si el jugador tiene más de 2 corazones, la pantalla de daño desaparece después de un tiempo
        if (currentHealth > 2)
        {
            StartCoroutine(HideDamageScreenAfterDelay(damageScreenDuration));
        }
        // Si el jugador tiene 2 o menos corazones, la pantalla de daño se mantiene visible y palpita
        else
        {
            StartCoroutine(PulseDamageScreen(damageScreenBaseAlpha, damageScreenPulseIntensity));
        }
    }

    private IEnumerator HideDamageScreenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Esperar el tiempo de duración
        SetDamageScreenAlpha(0f); // Ocultar la pantalla de daño
    }

    private IEnumerator PulseDamageScreen(float targetAlpha, float pulseIntensity)
    {
        while (currentHealth <= 2) // Mientras el jugador tenga 2 o menos corazones
        {
            // Escalar la pantalla de daño para simular la palpitación
            damageScreen.transform.localScale = Vector3.one * pulseIntensity;
            SetDamageScreenAlpha(targetAlpha);

            yield return new WaitForSeconds(1f / pulseSpeed); // Esperar un momento

            // Volver a la escala normal
            damageScreen.transform.localScale = Vector3.one;
            SetDamageScreenAlpha(targetAlpha * 0.8f); // Reducir ligeramente la opacidad

            yield return new WaitForSeconds(1f / pulseSpeed); // Esperar un momento
        }
    }

    private void SetDamageScreenAlpha(float alpha)
    {
        // Ajustar la opacidad de la pantalla de daño
        if (damageScreen != null)
        {
            Color color = damageScreen.color;
            color.a = alpha;
            damageScreen.color = color;
        }
    }
}