using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoManager : MonoBehaviour
{
    public static AmmoManager Instance;

    public int currentAmmo = 10; 
    public int maxAmmoPerMagazine = 10; 
    public int totalAmmo = 30; 
    public int maxTotalAmmo = 30; 
    public int regenerationLimit = 30; 
    public float timeBetweenShots = 0.5f; 

    public TextMeshProUGUI totalAmmoText; 
    public TextMeshProUGUI magazinesText; 
    public Image[] bulletImages; 

    public AudioClip reloadSound; 
    public AudioClip noAmmoSound;
    public AudioClip ammoAddedSound; 
    private AudioSource audioSource;

    private bool isReloading = false;
    private float lastShotTime = 0f;

    private WeaponAnimations weaponAnimations; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        weaponAnimations = FindFirstObjectByType<WeaponAnimations>();
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
            audioSource.PlayOneShot(noAmmoSound); 
            return;
        }

        isReloading = true;
        weaponAnimations.PlayReloadAnimation();
        audioSource.PlayOneShot(reloadSound); 

        int bulletsNeeded = maxAmmoPerMagazine - currentAmmo;
        int bulletsToAdd = Mathf.Min(bulletsNeeded, totalAmmo);

        totalAmmo -= bulletsToAdd;
        currentAmmo += bulletsToAdd;

        Invoke("FinishReload", 2.3f);  //tiempo animacion
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
            int previousAmmo = totalAmmo; 
            totalAmmo = Mathf.Min(totalAmmo + amount, maxTotalAmmo);

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