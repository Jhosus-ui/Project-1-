using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoManager : MonoBehaviour
{
    public static AmmoManager Instance;

    public int currentAmmo = 10; // Balas en el cartucho actual
    public int maxAmmoPerMagazine = 10; // Máximo de balas por cartucho
    public int totalAmmo = 30; // Balas totales disponibles
    public int maxTotalAmmo = 30; // Máximo de balas totales
    public int regenerationLimit = 30; // Límite de regeneración de balas
    public float timeBetweenShots = 0.5f; // Tiempo entre disparos

    public TextMeshProUGUI totalAmmoText; // Texto para balas totales
    public TextMeshProUGUI magazinesText; // Texto para cartuchos
    public Image[] bulletImages; // Imágenes de las balas en el cartucho (10 imágenes)

    public AudioClip reloadSound; // Sonido de recarga
    public AudioClip noAmmoSound; // Sonido cuando no hay balas para recargar
    public AudioClip ammoAddedSound; // Sonido cuando se aumenta totalAmmo
    private AudioSource audioSource;

    private bool isReloading = false;
    private float lastShotTime = 0f;

    private WeaponAnimations weaponAnimations; // Referencia al script de animaciones del arma

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        weaponAnimations = FindFirstObjectByType<WeaponAnimations>();
        if (weaponAnimations == null) Debug.LogError("WeaponAnimations no encontrado en la escena.");

        audioSource = GetComponent<AudioSource>(); // Obtener el componente AudioSource
        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) TryReload();
    }

    public void Shoot()
    {
        if (Time.time - lastShotTime < timeBetweenShots || isReloading || currentAmmo <= 0) return;

        lastShotTime = Time.time;
        currentAmmo--;
        UpdateBulletImages();
        UpdateUI();
        weaponAnimations.PlayShootAnimation();
    }

    public void TryReload()
    {
        if (isReloading || currentAmmo == maxAmmoPerMagazine) return;

        if (totalAmmo <= 0)
        {
            audioSource.PlayOneShot(noAmmoSound); // Reproducir sonido de "no hay balas"
            return;
        }

        isReloading = true;
        weaponAnimations.PlayReloadAnimation();
        audioSource.PlayOneShot(reloadSound); // Reproducir sonido de recarga

        int bulletsNeeded = maxAmmoPerMagazine - currentAmmo;
        int bulletsToAdd = Mathf.Min(bulletsNeeded, totalAmmo);

        totalAmmo -= bulletsToAdd;
        currentAmmo += bulletsToAdd;

        Invoke("FinishReload", 2.3f); // Ajusta el tiempo según la duración de la animación
    }

    private void FinishReload()
    {
        isReloading = false;
        UpdateBulletImages();
        UpdateUI();
        weaponAnimations.FinishReloadAnimation();
    }

    private void UpdateBulletImages()
    {
        for (int i = 0; i < bulletImages.Length; i++)
            bulletImages[i].gameObject.SetActive(i < currentAmmo);
    }

    private void UpdateUI()
    {
        if (totalAmmoText != null) totalAmmoText.text = totalAmmo.ToString();
        if (magazinesText != null) magazinesText.text = (totalAmmo / maxAmmoPerMagazine).ToString();
    }

    public void AddAmmo(int amount)
    {
        if (CanRegenerateAmmo())
        {
            int previousAmmo = totalAmmo; // Guardar el valor anterior de totalAmmo
            totalAmmo = Mathf.Min(totalAmmo + amount, maxTotalAmmo);

            // Reproducir sonido si hubo un aumento en totalAmmo
            if (totalAmmo > previousAmmo && ammoAddedSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(ammoAddedSound);
            }

            UpdateUI();
        }
    }

    public bool CanRegenerateAmmo() => totalAmmo <= regenerationLimit;

    public void UseAmmo()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            UpdateBulletImages();
            UpdateUI();
        }
    }

    public bool IsReloading() => isReloading;
}