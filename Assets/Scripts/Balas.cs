using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bala : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    public float speed = 3;
    public float maxDistance = 10f; 

    private Vector2 initialPosition; 

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        initialPosition = transform.position; 
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        // Mover la bala
        rigidbody.MovePosition(transform.position + transform.right * speed * Time.fixedDeltaTime);

        // Calcular la distancia recorrida
        float distanceTraveled = Vector2.Distance(initialPosition, transform.position);

        // Destruir la bala si supera la distancia máxima
        if (distanceTraveled >= maxDistance)
        {
            Destroy(gameObject);
        }
    }
}