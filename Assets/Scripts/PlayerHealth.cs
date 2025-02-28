using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 4; // Vida m�xima del jugador
    private int currentHealth; // Vida actual del jugador
    public float immunityTime = 1f; // Tiempo de inmunidad despu�s de recibir da�o
    private bool isImmune = false; // Indica si el jugador es inmune

    public Image[] heartImages; // Im�genes de los corazones (UI)
    public float normalPulseIntensity = 1.1f; // Intensidad de la palpitaci�n normal (2 corazones)
    public float strongPulseIntensity = 1.2f; // Intensidad de la palpitaci�n fuerte (1 coraz�n)
    public float pulseSpeed = 2f; // Velocidad de la palpitaci�n

    // Variables para la pantalla de da�o
    public Image damageScreen; // Pantalla de da�o (UI)
    public float damageScreenBaseAlpha = 0.3f; // Opacidad base de la pantalla de da�o
    public float damageScreenStrongAlpha = 0.6f; // Opacidad fuerte de la pantalla de da�o
    public float damageScreenPulseIntensity = 1.1f; // Intensidad de la palpitaci�n de la pantalla de da�o
    public float damageScreenDuration = 0.5f; // Duraci�n de la pantalla de da�o antes de desaparecer

    private void Start()
    {
        currentHealth = maxHealth; // Inicializar la vida al m�ximo
        UpdateHealthUI(); // Actualizar la UI al inicio
        Debug.Log("Vida inicial del jugador: " + currentHealth);

        // Inicializar la pantalla de da�o como transparente
        SetDamageScreenAlpha(0f);
    }

    public void TakeDamage(int damageAmount)
    {
        if (isImmune) return; // Si el jugador es inmune, no recibe da�o

        currentHealth -= damageAmount; // Reducir la vida
        Debug.Log("El jugador recibi� da�o. Vida actual: " + currentHealth);
        UpdateHealthUI(); // Actualizar la UI

        if (currentHealth <= 0)
        {
            Die(); // Si la vida llega a 0, el jugador muere
        }
        else
        {
            StartCoroutine(ActivateImmunity()); // Activar inmunidad temporal
            ShowDamageScreen(); // Mostrar la pantalla de da�o
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

        // Mantener la pantalla de da�o con opacidad m�xima
        SetDamageScreenAlpha(damageScreenStrongAlpha);
        StopAllCoroutines(); // Detener todas las corrutinas de palpitaci�n
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) // Si colisiona con un enemigo
        {
            TakeDamage(1); // Recibir da�o
        }
    }

    private void UpdateHealthUI()
    {
        // Actualizar la visibilidad de los corazones (sin cambios)
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentHealth)
            {
                heartImages[i].gameObject.SetActive(true); // Mostrar el coraz�n si tiene vida
            }
            else
            {
                heartImages[i].gameObject.SetActive(false); // Ocultar el coraz�n si no tiene vida
            }
        }

        // Activar o desactivar la palpitaci�n de los corazones seg�n la vida restante
        if (currentHealth == 2)
        {
            StartCoroutine(PulseHearts(normalPulseIntensity)); // Palpitaci�n suave
        }
        else if (currentHealth == 1)
        {
            StartCoroutine(PulseHearts(strongPulseIntensity)); // Palpitaci�n fuerte
        }
    }

    private IEnumerator PulseHearts(float intensity)
    {
        while (currentHealth <= 2) // Mientras el jugador tenga 2 o menos corazones
        {
            // Escalar los corazones hacia arriba y abajo para simular la palpitaci�n
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

    // M�todos para la pantalla de da�o
    private void ShowDamageScreen()
    {
        // Mostrar la pantalla de da�o con opacidad base
        SetDamageScreenAlpha(damageScreenBaseAlpha);

        // Si el jugador tiene m�s de 2 corazones, la pantalla de da�o desaparece despu�s de un tiempo
        if (currentHealth > 2)
        {
            StartCoroutine(HideDamageScreenAfterDelay(damageScreenDuration));
        }
        // Si el jugador tiene 2 o menos corazones, la pantalla de da�o se mantiene visible y palpita
        else
        {
            StartCoroutine(PulseDamageScreen(damageScreenBaseAlpha, damageScreenPulseIntensity));
        }
    }

    private IEnumerator HideDamageScreenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Esperar el tiempo de duraci�n
        SetDamageScreenAlpha(0f); // Ocultar la pantalla de da�o
    }

    private IEnumerator PulseDamageScreen(float targetAlpha, float pulseIntensity)
    {
        while (currentHealth <= 2) // Mientras el jugador tenga 2 o menos corazones
        {
            // Escalar la pantalla de da�o para simular la palpitaci�n
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
        // Ajustar la opacidad de la pantalla de da�o
        if (damageScreen != null)
        {
            Color color = damageScreen.color;
            color.a = alpha;
            damageScreen.color = color;
        }
    }
}