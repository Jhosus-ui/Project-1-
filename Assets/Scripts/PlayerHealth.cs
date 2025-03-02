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

    private Coroutine damageScreenPulseCoroutine; // Variable para rastrear la corrutina de la pantalla de daño
    public CameraFollow cameraFollow; // Referencia al script de la cámara
    private Animator animator;

    // Sonidos
    public AudioClip damageSound; // Sonido cuando el jugador recibe daño
    public AudioClip healSound; // Sonido cuando el jugador se cura
    public AudioClip twoHeartsPulseSound; // Sonido cuando comienzan las palpitaciones a dos corazones
    public AudioClip lowHealthPulseSound; // Sonido cuando queda menos de dos corazones
    private AudioSource audioSource;

    private bool isPlayingLowHealthSound = false; // Controlar si el sonido de baja salud está en reproducción

    private void Start()
    {
        currentHealth = maxHealth; // Inicializar la vida al máximo
        UpdateHealthUI(); // Actualizar la UI al inicio
        Debug.Log("Vida inicial del jugador: " + currentHealth);

        // Inicializar la pantalla de daño como transparente
        SetDamageScreenAlpha(0f);

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); // Obtener el componente AudioSource
    }

    public void TakeDamage(int damageAmount)
    {
        if (isImmune) return; // Si el jugador es inmune, no recibe daño

        currentHealth -= damageAmount; // Reducir la vida
        Debug.Log("El jugador recibió daño. Vida actual: " + currentHealth);
        UpdateHealthUI(); // Actualizar la UI

        // Reproducir sonido de daño
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

        // Reproducir la animación de muerte
        if (animator != null)
        {
            animator.SetTrigger("Dead"); // "Dead" es el nombre del trigger de la animación de muerte
        }

        // Llamar a la corrutina para cambiar de escena después de la animación
        StartCoroutine(CambiarEscenaDespuesDeAnimacion());
    }

    private IEnumerator CambiarEscenaDespuesDeAnimacion()
    {
        // Esperar a que termine la animación de muerte
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Notificar al GameManager que el jugador ha muerto
        GameManager.Instance.PlayerDied();
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
        // Actualizar la visibilidad de los corazones
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].gameObject.SetActive(i < currentHealth);
        }

        // Activar o desactivar la palpitación de los corazones según la vida restante
        if (currentHealth == 2)
        {
            StartCoroutine(PulseHearts(normalPulseIntensity)); // Palpitación suave
            PlayLoopingSound(twoHeartsPulseSound); // Reproducir sonido de dos corazones en loop
        }
        else if (currentHealth == 1)
        {
            StartCoroutine(PulseHearts(strongPulseIntensity)); // Palpitación fuerte
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
            // Detener la corrutina anterior si existe
            if (damageScreenPulseCoroutine != null)
            {
                StopCoroutine(damageScreenPulseCoroutine);
            }
            // Iniciar la nueva corrutina y almacenar la referencia
            damageScreenPulseCoroutine = StartCoroutine(PulseDamageScreen(damageScreenBaseAlpha, damageScreenPulseIntensity));
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

    public bool CanHeal()
    {
        return currentHealth < maxHealth; // Verificar si el jugador puede curarse
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth); // Curar al jugador sin exceder la vida máxima
        UpdateHealthUI(); // Actualizar la UI de salud
        Debug.Log("El jugador se ha curado. Vida actual: " + currentHealth);

        // Reproducir sonido de curación
        if (healSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(healSound);
        }

        // Si la salud es mayor que 2, detener la palpitación de la pantalla de daño y ocultarla
        if (currentHealth > 2)
        {
            if (damageScreenPulseCoroutine != null)
            {
                StopCoroutine(damageScreenPulseCoroutine);
                damageScreenPulseCoroutine = null;
            }
            SetDamageScreenAlpha(0f); // Ocultar la pantalla de daño
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