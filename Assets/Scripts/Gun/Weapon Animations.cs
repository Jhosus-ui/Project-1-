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

    // M�todo para activar la animaci�n de disparo
    public void PlayShootAnimation()
    {
        animator.SetTrigger("Shoot");
    }

    // M�todo para activar la animaci�n de recarga
    public void PlayReloadAnimation()
    {
        animator.SetBool("IsReloading", true);
        animator.SetTrigger("Reload");
    }

    // M�todo para finalizar la animaci�n de recarga
    public void FinishReloadAnimation()
    {
        animator.SetBool("IsReloading", false);
    }

    // M�todo para verificar si el arma est� recargando
    public bool IsReloading()
    {
        return animator.GetBool("IsReloading");
    }
}