using UnityEngine;

public class WeaponAnimations : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator no encontrado en el objeto del arma.");
        }
    }

    // Método para activar la animación de disparo
    public void PlayShootAnimation()
    {
        animator.SetTrigger("Shoot");
    }

    // Método para activar la animación de recarga
    public void PlayReloadAnimation()
    {
        animator.SetBool("IsReloading", true);
        animator.SetTrigger("Reload");
    }

    // Método para finalizar la animación de recarga
    public void FinishReloadAnimation()
    {
        animator.SetBool("IsReloading", false);
    }

    // Método para verificar si el arma está recargando
    public bool IsReloading()
    {
        return animator.GetBool("IsReloading");
    }
}