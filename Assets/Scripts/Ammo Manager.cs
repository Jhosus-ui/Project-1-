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

    private bool isReloading = false;
    private float lastShotTime = 0f;

    private WeaponAnimations weaponAnimations; // Referencia al script de animaciones del arma

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Obtener la referencia al script de animaciones del arma
        weaponAnimations = FindObjectOfType<WeaponAnimations>();
        if (weaponAnimations == null)
        {
            Debug.LogError("WeaponAnimations no encontrado en la escena.");
        }

        UpdateUI();
    }

    private void Update()
    {
        HandleInput();
        HandleReload();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.R)) // Recargar con la tecla "R"
        {
            TryReload();
        }
    }

    private void HandleReload()
    {
        if (isReloading)
        {
            // Aquí puedes manejar la lógica de la animación de recarga
            // Por ejemplo, esperar a que termine la animación antes de actualizar las balas
        }
    }

    // Método para disparar
    public void Shoot()
    {
        // Verificar si el arma está recargando o si no hay balas
        if (Time.time - lastShotTime < timeBetweenShots || isReloading || currentAmmo <= 0)
            return;

        lastShotTime = Time.time;
        currentAmmo--;
        UpdateBulletImages();
        UpdateUI();

        // Activar la animación de disparo
        weaponAnimations.PlayShootAnimation();
    }

    // Método para intentar recargar
    public void TryReload()
    {
        if (isReloading || currentAmmo == maxAmmoPerMagazine || totalAmmo <= 0)
            return;

        isReloading = true;

        // Activar la animación de recarga
        weaponAnimations.PlayReloadAnimation();

        int bulletsNeeded = maxAmmoPerMagazine - currentAmmo;
        int bulletsToAdd = Mathf.Min(bulletsNeeded, totalAmmo);

        totalAmmo -= bulletsToAdd;
        currentAmmo += bulletsToAdd;

        // Esperar a que termine la animación de recarga antes de actualizar las imágenes
        Invoke("FinishReload", 2.3f); // Ajusta el tiempo según la duración de la animación
    }

    private void FinishReload()
    {
        isReloading = false;
        UpdateBulletImages();
        UpdateUI();

        // Finalizar la animación de recarga
        weaponAnimations.FinishReloadAnimation();
    }

    // Actualizar las imágenes de las balas en el cartucho
    private void UpdateBulletImages()
    {
        for (int i = 0; i < bulletImages.Length; i++)
        {
            bulletImages[i].gameObject.SetActive(i < currentAmmo);
        }
    }

    // Actualizar la UI (balas totales y cartuchos)
    private void UpdateUI()
    {
        if (totalAmmoText != null)
        {
            totalAmmoText.text = "" + totalAmmo;
        }

        if (magazinesText != null)
        {
            int magazines = totalAmmo / maxAmmoPerMagazine;
            magazinesText.text = "" + magazines;
        }
    }

    // Método para agregar balas (usado por AmmoBox)
    public void AddAmmo(int amount)
    {
        if (CanRegenerateAmmo())
        {
            totalAmmo = Mathf.Min(totalAmmo + amount, maxTotalAmmo);
            UpdateUI();
        }
    }

    // Método para verificar si se puede regenerar balas
    public bool CanRegenerateAmmo()
    {
        return totalAmmo <= regenerationLimit;
    }

    // Método para gastar una bala (usado por ArmaMovimiento)
    public void UseAmmo()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            UpdateBulletImages();
            UpdateUI();
        }
    }

    // Método para verificar si el arma está recargando
    public bool IsReloading()
    {
        return isReloading;
    }
}