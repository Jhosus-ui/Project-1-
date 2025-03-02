using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 4; 
    private int currentHealth; 
    public float immunityTime = 1f;
    private bool isImmune = false; 

    public Image[] heartImages; 
    public float normalPulseIntensity = 1.1f; 
    public float strongPulseIntensity = 1.2f; 
    public float pulseSpeed = 2f; 

    public Image damageScreen; 
    public float damageScreenBaseAlpha = 0.3f; 
    public float damageScreenStrongAlpha = 0.6f; 
    public float damageScreenPulseIntensity = 1.1f; 
    public float damageScreenDuration = 0.5f; 

    private Coroutine damageScreenPulseCoroutine; 
    public CameraFollow cameraFollow; 
    private Animator animator;

    public AudioClip damageSound; 
    public AudioClip healSound; 
    public AudioClip twoHeartsPulseSound; 
    public AudioClip lowHealthPulseSound; 
    private AudioSource audioSource;

    private bool isPlayingLowHealthSound = false; 

    private void Start()
    {
        currentHealth = maxHealth; 
        UpdateHealthUI(); 
        Debug.Log("Vida inicial del jugador: " + currentHealth);

        SetDamageScreenAlpha(0f);

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); 
    }

    public void TakeDamage(int damageAmount)
    {
        if (isImmune) return; 

        currentHealth -= damageAmount; 
        Debug.Log("El jugador recibió daño. Vida actual: " + currentHealth);
        UpdateHealthUI(); 

        
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
            Die(); 
        }
        else
        {
            StartCoroutine(ActivateImmunity()); 
            ShowDamageScreen(); 
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

      
        if (animator != null)
        {
            animator.SetTrigger("Dead"); 
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
        if (collision.CompareTag("Enemy")) 
        {
            TakeDamage(1); 
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
            StartCoroutine(PulseHearts(normalPulseIntensity)); 
            PlayLoopingSound(twoHeartsPulseSound); 
        }
        else if (currentHealth == 1)
        {
            StartCoroutine(PulseHearts(strongPulseIntensity)); 
            PlayLoopingSound(lowHealthPulseSound); 
        }
        else if (currentHealth > 2 && isPlayingLowHealthSound)
        {
            StopLoopingSound(); 
        }
    }

    private IEnumerator PulseHearts(float intensity)
    {
        while (currentHealth <= 2) 
        {
            
            foreach (var heart in heartImages)
            {
                if (heart.gameObject.activeSelf)
                {
                    heart.transform.localScale = Vector3.one * intensity;
                }
            }

            yield return new WaitForSeconds(1f / pulseSpeed); 

            // Volver a la escala normal
            foreach (var heart in heartImages)
            {
                if (heart.gameObject.activeSelf)
                {
                    heart.transform.localScale = Vector3.one;
                }
            }

            yield return new WaitForSeconds(1f / pulseSpeed); 
        }
    }

    private void ShowDamageScreen() //La implementacion de este 
    {
        SetDamageScreenAlpha(damageScreenBaseAlpha);

        if (currentHealth > 2)
        {
            StartCoroutine(HideDamageScreenAfterDelay(damageScreenDuration));
        }

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
        yield return new WaitForSeconds(delay); 
        SetDamageScreenAlpha(0f); 
    }

    private IEnumerator PulseDamageScreen(float targetAlpha, float pulseIntensity)
    {
        while (currentHealth <= 2) 
        {
           
            damageScreen.transform.localScale = Vector3.one * pulseIntensity;
            SetDamageScreenAlpha(targetAlpha);

            yield return new WaitForSeconds(1f / pulseSpeed); 

            damageScreen.transform.localScale = Vector3.one;
            SetDamageScreenAlpha(targetAlpha * 0.8f); 

            yield return new WaitForSeconds(1f / pulseSpeed); 
        }
    }

    private void SetDamageScreenAlpha(float alpha) //Chequear esto es muy delicado 
    {
      
        if (damageScreen != null)
        {
            Color color = damageScreen.color;
            color.a = alpha;
            damageScreen.color = color;
        }
    }

    public bool CanHeal()
    {
        return currentHealth < maxHealth; 
    }

    public void Heal(int amount) //Mecanica para curacion
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth); 
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
            audioSource.loop = true; 
            audioSource.Play();
            isPlayingLowHealthSound = true;
        }
    }

    private void StopLoopingSound() //Sonidos 
    {
        if (audioSource != null && isPlayingLowHealthSound)
        {
            audioSource.Stop();
            audioSource.loop = false; 
            isPlayingLowHealthSound = false;
        }
    }
}