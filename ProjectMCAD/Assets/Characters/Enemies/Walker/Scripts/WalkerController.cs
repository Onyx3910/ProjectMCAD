using UnityEngine;

public class WalkerController : MonoBehaviour, IVolitile
{
    [Header("Movement")]

    public bool canMove = true;
    public float maxSpeed = 50f;
    public float horizontalSpeed = 5f;

    private Vector2 acceleration;

    public Vector2 VelocityTarget { get; protected set; }
    public Vector2 Velocity { get; protected set; }
    public CapsuleCollider2D Collider { get; private set; }
    public bool WalkingRight => Velocity.x > 0f;
    public bool WalkingLeft => Velocity.x < 0f;
    public SpriteRenderer SpriteRenderer { get; private set; }
    public Health Health { get; private set; }

    void Start()
    {
        VelocityTarget = horizontalSpeed * Vector2.right;
        Collider = GetComponent<CapsuleCollider2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Health = GetComponent<Health>();
    }

    void Update()
    {
        CheckForObstacle();
        if (canMove) Velocity = Vector2.SmoothDamp(Velocity, VelocityTarget, ref acceleration, 0.01f);
        transform.Translate(Velocity * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !Health.Invulnerable)
        {
            collision.GetComponent<Health>().Hit();
            return;
        }
    }

    public void Die()
    {
        canMove = false;
        Velocity = Vector2.zero;
        Destroy(gameObject, 1f);
    }

    public void Flip()
    {
        SpriteRenderer.flipX = !SpriteRenderer.flipX;
        VelocityTarget = -VelocityTarget;
    }

    /// <summary>
    /// Checks for walls and ledges.
    /// </summary>
    private void CheckForObstacle()
    {
        var threeForuthsHeight = Collider.size.y * 0.75f;
        var width = Collider.size.x / 2f;
        if(WalkingRight)
        {
            var origin = transform.position + (width * Vector3.right);
            var hit = Physics2D.Raycast(origin, Vector2.down, threeForuthsHeight, LayerMask.GetMask("Ground"));
            if(!hit)
            {
                Flip();
            }
        }
        else if(WalkingLeft)
        {
            var origin = transform.position + (width * Vector3.left);
            var hit = Physics2D.Raycast(origin, Vector2.down, threeForuthsHeight, LayerMask.GetMask("Ground"));
            if (!hit)
            {
                Flip();
            }
        }
    }
}
