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

    private Coroutine damageScreenPulseCoroutine; // Variable para rastrear la corrutina de la pantalla de da�o
    public CameraFollow cameraFollow; // Referencia al script de la c�mara
    private Animator animator;

    // Sonidos
    public AudioClip damageSound; // Sonido cuando el jugador recibe da�o
    public AudioClip healSound; // Sonido cuando el jugador se cura
    public AudioClip twoHeartsPulseSound; // Sonido cuando comienzan las palpitaciones a dos corazones
    public AudioClip lowHealthPulseSound; // Sonido cuando queda menos de dos corazones
    private AudioSource audioSource;

    private bool isPlayingLowHealthSound = false; // Controlar si el sonido de baja salud est� en reproducci�n

    private void Start()
    {
        currentHealth = maxHealth; // Inicializar la vida al m�ximo
        UpdateHealthUI(); // Actualizar la UI al inicio
        Debug.Log("Vida inicial del jugador: " + currentHealth);
        // Inicializar la pantalla de da�o como transparente
        SetDamageScreenAlpha(0f);
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); // Obtener el componente AudioSource
    }

    public void TakeDamage(int damageAmount)
    {
        if (isImmune) return; // Si el jugador es inmune, no recibe da�o

        currentHealth -= damageAmount; // Reducir la vida
        Debug.Log("El jugador recibi� da�o. Vida actual: " + currentHealth);
        UpdateHealthUI(); // Actualizar la UI

        if (damageSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        if (cameraFollow != null)
        {
            cameraFollow.ShakeCamera();
        }

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

        if (animator != null)
        {
            animator.SetTrigger("Dead"); // "Dead" es el nombre del trigger de la animaci�n de muerte
        }

        StartCoroutine(CambiarEscenaDespuesDeAnimacion());
    }

    private IEnumerator CambiarEscenaDespuesDeAnimacion()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        GameManager.Instance.PlayerDied();
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

        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].gameObject.SetActive(i < currentHealth);
        }

        if (currentHealth == 2)
        {
            StartCoroutine(PulseHearts(normalPulseIntensity)); // Palpitaci�n suave
            PlayLoopingSound(twoHeartsPulseSound); // Reproducir sonido de dos corazones en loop
        }
        else if (currentHealth == 1)
        {
            StartCoroutine(PulseHearts(strongPulseIntensity)); // Palpitaci�n fuerte
            PlayLoopingSound(lowHealthPulseSound); // Reproducir sonido de baja salud en loop
        }
        else if (currentHealth > 2 && isPlayingLowHealthSound)
        {
            StopLoopingSound(); // Detener los sonidos si el jugador se cura
        }
    }

    private IEnumerator PulseHearts(float intensity)
    {
        while (currentHealth <= 2) // Mientras el jugador tenga 2 o menos corazones
        {

            foreach (var heart in heartImages)
            {
                if (heart.gameObject.activeSelf)
                {
                    heart.transform.localScale = Vector3.one * intensity;
                }
            }

            yield return new WaitForSeconds(1f / pulseSpeed); // Esperar un momento

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

    private void ShowDamageScreen()
    {
        SetDamageScreenAlpha(damageScreenBaseAlpha);

        // Si el jugador tiene m�s de 2 corazones, la pantalla de da�o desaparece despu�s de un tiempo
        if (currentHealth > 2)
        {
            StartCoroutine(HideDamageScreenAfterDelay(damageScreenDuration));
        }
        // Si el jugador tiene 2 o menos corazones, la pantalla de da�o se mantiene visible y palpita
        else
        {
            if (damageScreenPulseCoroutine != null)
            {
                StopCoroutine(damageScreenPulseCoroutine);
            }
            damageScreenPulseCoroutine = StartCoroutine(PulseDamageScreen(damageScreenBaseAlpha, damageScreenPulseIntensity));
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
            damageScreen.transform.localScale = Vector3.one * pulseIntensity;
            SetDamageScreenAlpha(targetAlpha);
            yield return new WaitForSeconds(1f / pulseSpeed); 
            damageScreen.transform.localScale = Vector3.one;
            SetDamageScreenAlpha(targetAlpha * 0.8f); 
            yield return new WaitForSeconds(1f / pulseSpeed); 
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

    public bool CanHeal()
    {
        return currentHealth < maxHealth; // Verificar si el jugador puede curarse
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth); // Curar al jugador sin exceder la vida m�xima
        UpdateHealthUI(); 
        Debug.Log("El jugador se ha curado. Vida actual: " + currentHealth);
        if (healSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(healSound);
        }
        if (currentHealth > 2)
        {
            if (damageScreenPulseCoroutine != null)
            {
                StopCoroutine(damageScreenPulseCoroutine);
                damageScreenPulseCoroutine = null;
            }
            SetDamageScreenAlpha(0f); 
        }
    }

    private void PlayLoopingSound(AudioClip sound)
    {
        if (sound != null && audioSource != null && !isPlayingLowHealthSound)
        {
            audioSource.clip = sound;
            audioSource.loop = true; // Reproducir en loop
            audioSource.Play();
            isPlayingLowHealthSound = true;
        }
    }

    private void StopLoopingSound()
    {
        if (audioSource != null && isPlayingLowHealthSound)
        {
            audioSource.Stop();
            audioSource.loop = false; // Desactivar el loop
            isPlayingLowHealthSound = false;
        }
    }
}