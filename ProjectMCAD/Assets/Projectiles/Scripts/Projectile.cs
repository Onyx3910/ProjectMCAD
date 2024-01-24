using UnityEngine;

public class Projectile : MonoBehaviour, IVolitile
{
    [Header("Physics")]

    [Range(0f, 1f)]
    public float elasticity = 0.5f;
    public float maxSpeed = 50f;
    public bool gravityEnabled = true;

    [Header("Collision Check")]

    public LayerMask avoidLayer;
    [Range(0f, 45f)]
    public float floorAngle = 15f;
    public float collisionCheckRadius = 1f;

    public Vector2 Velocity { get; set; }

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
            if (collision.transform.CompareTag("Enemy"))
            {
                collision.transform.GetComponent<Health>().Hit();
            }

            if(Vector2.Angle(Vector2.up, collision.normal) <= floorAngle)
            {
                Die();
                return;
            }

            Velocity = elasticity * Vector2.Reflect(Velocity, collision.normal);
            transform.Translate((collisionCheckRadius - collision.distance) * collision.normal);
        }
    }

    public void Die()
    {
        Velocity = Vector2.zero;
        Destroy(gameObject);
    }
}
