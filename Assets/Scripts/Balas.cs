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
        rigidbody.MovePosition(transform.position + transform.right * speed * Time.fixedDeltaTime);
        float distanceTraveled = Vector2.Distance(initialPosition, transform.position);
        if (distanceTraveled >= maxDistance)
        {
            Destroy(gameObject);
        }
    }
}