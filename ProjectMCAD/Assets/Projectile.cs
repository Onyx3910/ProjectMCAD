using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Physics")]

    public float maxSpeed = 50f;
    public bool gravityEnabled = true;

    [Header("Collision Check")]

    public LayerMask avoidLayer;
    [Range(0f, 45f)]
    public float floorAngle = 15f;
    public float collisionCheckRadius = 1f;

    //public Vector2 VelocityTarget { get; protected set; }
    public Vector2 Velocity { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Gravity();

        transform.Translate(Velocity * Time.deltaTime);
    }

    private void LateUpdate()
    {
        HandleCollisions();
    }

    private void Gravity()
    {
        if (gravityEnabled)
        {
            Debug.Log("Falling!");
            Velocity += 32f * Time.deltaTime * Vector2.down;
        }
    }

    private void HandleCollisions()
    {
        var collision = Physics2D.CircleCast(transform.position, collisionCheckRadius, Vector2.zero, 0f, ~avoidLayer);
        if (collision)
        {
            if(Vector2.Angle(Vector2.up, collision.normal) <= floorAngle)
            {
                Destroy(gameObject);
                return;
            }

            Velocity = Vector2.Reflect(Velocity, collision.normal);
        }
    }
}
