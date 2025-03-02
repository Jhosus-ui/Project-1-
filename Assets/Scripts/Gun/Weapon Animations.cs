using UnityEngine;

public class WeaponAnimations : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void PlayShootAnimation()
    {
        animator.SetTrigger("Shoot");
    }

    public void PlayReloadAnimation()
    {
        animator.SetBool("IsReloading", true);
        animator.SetTrigger("Reload");
    }

    public void FinishReloadAnimation()
    {
        animator.SetBool("IsReloading", false);
    }

    public bool IsReloading()
    {
        return animator.GetBool("IsReloading");
    }
}