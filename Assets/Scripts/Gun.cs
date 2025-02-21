using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmaMovimiento : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public new Camera camera;
    public Transform spawner;
    public GameObject balaPrefab; 

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Disparar();
        RotateTowardsMouse();
    }

    void Disparar()
    {
        // Verificar si hay balas disponibles
        if (Input.GetButtonDown("Fire1") && AmmoManager.Instance.currentAmmo > 0)
        {
            // Instanciar la bala en el punto de disparo
            GameObject Bala = Instantiate(balaPrefab);
            Bala.transform.position = spawner.position;
            Bala.transform.rotation = transform.rotation;

            // Gastar una bala
            AmmoManager.Instance.UseAmmo();
        }
        else if (Input.GetButtonDown("Fire1") && AmmoManager.Instance.currentAmmo <= 0)
        {
            Debug.Log("No hay balas disponibles."); 
        }
    }

    void RotateTowardsMouse()
    {
        float angle = GetAngleTowardsMouse();
        transform.rotation = Quaternion.Euler(0, 0, angle);
        spriteRenderer.flipY = angle >= 90 && angle <= 270;
    }

    float GetAngleTowardsMouse()
    {
        Vector3 mouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mouseDirection = mouseWorldPosition - transform.position;
        mouseDirection.z = 0;

        float angle = (Vector3.SignedAngle(Vector3.right, mouseDirection, Vector3.forward) + 360) % 360;

        return angle;
    }
}