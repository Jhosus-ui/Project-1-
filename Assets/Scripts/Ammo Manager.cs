using UnityEngine;
using TMPro;

public class AmmoManager : MonoBehaviour
{
    public static AmmoManager Instance; 

    public int currentAmmo = 10; 
    public int maxAmmo = 30;
    public int regenerationLimit = 30; 
    public TextMeshProUGUI ammoText;

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
        UpdateAmmoUI();
    }

    // Método para gastar una bala
    public void UseAmmo()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            UpdateAmmoUI();
        }
    }

    // Método para agregar balas
    public void AddAmmo(int amount)
    {
        if (currentAmmo < regenerationLimit)
        {
            currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
            UpdateAmmoUI();
        }
    }

    // Actualizar el texto de la UI
    private void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = "Ammo: " + currentAmmo;
        }
    }

    // Método para verificar si se puede regenerar balas
    public bool CanRegenerateAmmo()
    {
        return currentAmmo <= regenerationLimit / 2; 
    }
}