using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperController : MonoBehaviour, IVolitile
{
    [Header("Movement")]

    public bool canMove = true;
    public float jumpEveryXSeconds = 2f;
    public float maxSpeed = 50f;
    public float horizontalSpeed = 5f;
    public float verticalSpeed = 5f;

    [Header("Collision Check")]

    public LayerMask ground;
    [Range(0f, 1f)]
    public float collisionCheckRadius = 1f;

    private Vector2 acceleration;

    public bool IsGrounded { get; protected set; }
    public bool WasGrounded { get; protected set; }
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

        StartCoroutine(JumpRoutine());
    }

    void Update()
    {
        Gravity();

        Velocity = Vector2.SmoothDamp(Velocity, VelocityTarget, ref acceleration, 0.01f);
        transform.Translate(Velocity * Time.deltaTime);
    }

    private void LateUpdate()
    {
        HandleCollisions();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !Health.Invulnerable)
        {
            collision.GetComponent<Health>().Hit();
            return;
        }
    }

    public void Die()
    {
        canMove = false;
        VelocityTarget = new Vector2(0f, VelocityTarget.y);
        Velocity = new Vector2(0f, Velocity.y);
        Destroy(gameObject, 1f);
    }

    public void Flip()
    {
        SpriteRenderer.flipX = !SpriteRenderer.flipX;
        horizontalSpeed = -horizontalSpeed;
    }

    protected IEnumerator JumpRoutine()
    {
        while(Health.CurrentHitPoints > 0)
        {
            yield return new WaitForSeconds(jumpEveryXSeconds);
            if (!canMove) continue;
            VelocityTarget += verticalSpeed * Vector2.up;
            VelocityTarget += horizontalSpeed * Vector2.right;
        }
    }

    private void Gravity()
    {
        if (!IsGrounded)
        {
            VelocityTarget += 32f * Time.deltaTime * Vector2.down;
        }
    }

    private bool IsCollidingBelow(out float penetration)
    {
        penetration = 0f;
        var circleCastLength = 2f - collisionCheckRadius;
        var hit = Physics2D.CircleCast(transform.position, collisionCheckRadius, Vector2.down, circleCastLength, ground);
        if (hit.collider != null)
        {
            penetration = circleCastLength - hit.distance;
            return true;
        }

        return false;
    }

    private bool IsCollidingAbove(out float penetration)
    {
        penetration = 0f;
        var circleCastLength = 2f - collisionCheckRadius;
        var hit = Physics2D.CircleCast(transform.position, collisionCheckRadius, Vector2.up, circleCastLength, ground);
        if (hit.collider != null)
        {
            penetration = circleCastLength - hit.distance;
            return true;
        }

        return false;
    }

    private bool IsCollidingLeft(out float penetration)
    {
        penetration = 0f;
        var size = new Vector2(0.1f, 0.8f);
        var boxCastLength = 0.4f;
        var hit = Physics2D.BoxCast(transform.position, size, 0f, Vector2.left, boxCastLength, ground);
        if (hit.collider != null)
        {
            penetration = boxCastLength - hit.distance;
            return true;
        }

        return false;
    }

    private bool IsCollidingRight(out float penetration)
    {
        penetration = 0f;
        var size = new Vector2(0.1f, 0.8f);
        var boxCastLength = 0.4f;
        var hit = Physics2D.BoxCast(transform.position, size, 0f, Vector2.right, boxCastLength, ground);
        if (hit.collider != null)
        {
            penetration = boxCastLength - hit.distance;
            return true;
        }

        return false;
    }

    private void HandleCollisions()
    {
        // Below
        WasGrounded = IsGrounded;
        IsGrounded = IsCollidingBelow(out var penetration);
        if (!WasGrounded && IsGrounded && Velocity.y < 0f)
        {
            VelocityTarget = Vector2.zero;
            Velocity = Vector2.zero;
            transform.Translate(penetration * Vector2.up);
        }
        else if (IsGrounded && Velocity.y < 0f)
        {
            Velocity = Vector2.zero;
            transform.Translate(penetration * Vector2.up);
        }

        // Above
        if (IsCollidingAbove(out penetration) && Velocity.y > 0f)
        {
            VelocityTarget = new Vector2(VelocityTarget.x, -Mathf.Sqrt(32f));
            Velocity = new Vector2(Velocity.x, 0f);
            transform.Translate(penetration * Vector2.down);
        }

        // Left
        if (IsCollidingLeft(out penetration))
        {
            VelocityTarget = new Vector2(0f, VelocityTarget.y);
            Velocity = new Vector2(0f, Velocity.y);
            transform.Translate(penetration * Vector2.right);
        }

        // Right
        if (IsCollidingRight(out penetration))
        {
            VelocityTarget = new Vector2(0f, VelocityTarget.y);
            Velocity = new Vector2(0f, Velocity.y);
            transform.Translate(penetration * Vector2.left);
        }
    }
}
