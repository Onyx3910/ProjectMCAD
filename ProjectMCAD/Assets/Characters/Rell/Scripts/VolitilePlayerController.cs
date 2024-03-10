using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class VolitilePlayerController : MonoBehaviour, IVolitile
{
    [Header("Movement")]

    public bool canMove = true;
    public float maxSpeed = 50f;
    public float horizontalSpeed = 5f;
    public float jumpStrength = 8f;

    [Header("Attack")]
    public bool canAttack = true;
    public GameObject projectilePrefab;
    public float maxAttackChargeTime = 3f;

    [Header("Collision Check")]

    public LayerMask ground;
    [Range(0f, 1f)]
    public float collisionCheckRadius = 1f;

    [Header("UI")]

    public GameObject dialogueOptions;

    private Vector2 acceleration;

    public bool IsGrounded { get; protected set; }
    public bool WasGrounded { get; protected set; }
    public Vector2 VelocityTarget { get; protected set; }
    public Vector2 Velocity { get; protected set; }
    public float ChargeStrength { get; protected set; }
    public SpriteRenderer SpriteRenderer { get; private set; }
    public Animator Animator { get; private set; }
    public Health Health { get; private set; }

    private void Start()
    {
        IsGrounded = true;
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
        Health = GetComponent<Health>();
    }

    private void Update()
    {
        ProcessInput();
        Gravity();

        Velocity = new Vector2(Mathf.Clamp(Velocity.x, -maxSpeed, maxSpeed), Velocity.y);
        transform.Translate(Velocity * Time.deltaTime);
    }

    private void LateUpdate()
    {
        HandleCollisions();

        Animator.SetBool("Jumping", !IsGrounded);
        Animator.SetFloat("VelocityX", Velocity.x);
        Animator.SetFloat("VelocityY", Velocity.y);
    }

    protected void ProcessInput()
    {
        MovementInput();
        AttackInput();
        
        Velocity = Vector2.SmoothDamp(Velocity, VelocityTarget, ref acceleration, 0.01f);
    }

    public void Die()
    {
        canMove = false;
        canAttack = false;
        Destroy(gameObject, 1f);
    }

    #region Movement

    private void MovementInput()
    {
        if (!canMove)
        {
            VelocityTarget = new Vector2(0f, VelocityTarget.y);
            return;
        }

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            VelocityTarget = new Vector2(0f, VelocityTarget.y);
        }

        if (Input.GetKey(KeyCode.A))
        {
            FaceLeft();
            VelocityTarget = new Vector2(-horizontalSpeed, VelocityTarget.y);
        }

        if (Input.GetKey(KeyCode.D))
        {
            FaceRight();
            VelocityTarget = new Vector2(horizontalSpeed, VelocityTarget.y);
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            VelocityTarget += jumpStrength * Vector2.up;
        }
    }

    private void FaceLeft()
    {
        SpriteRenderer.flipX = true;
    }

    private void FaceRight()
    {
        SpriteRenderer.flipX = false;
    }

    #endregion

    #region Attack

    private void AttackInput()
    {
        if (!canAttack) return;

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("Starting Attack!");
            canMove = false;
            ChargeStrength = 0f;
            Animator.SetBool("ChargingAttack", true);
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Debug.Log("Attack Released!");
            //canAttack = false;
            Animator.SetBool("ChargingAttack", false);
        }
    }

    // This method is called from the animation event
    private void SetAttackStrength(float strength)
    {
        ChargeStrength = strength / 100f;
    }

    // This method is called from the animation event
    private void ThrowProjectile()
    {
        Debug.Log("Throwing Projectile!");
        var attackDirection = ((Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position)).normalized;
        var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();
        if (ChargeStrength == 0f) ChargeStrength = 0.33f;
        projectile.Velocity = ChargeStrength * projectile.maxSpeed * attackDirection;
    }

    // This method is called from the animation event
    private void ResetAttack()
    {
        Debug.Log("Attack Reset!");
        canMove = true;
        canAttack = true;
    }

    #endregion

    #region Physics

    private void Gravity()
    {
        if (!IsGrounded)
        {
            VelocityTarget += 32f * Time.deltaTime * Vector2.down;
        }
    }

    #endregion

    #region Collision Handling

    private bool IsCollidingBelow(out float penetration)
    {
        penetration = 0f;
        var circleCastLength = 1f - collisionCheckRadius;
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
        var circleCastLength = 1 - collisionCheckRadius;
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
            VelocityTarget = new Vector2(VelocityTarget.x, 0f);
            Velocity = new Vector2(Velocity.x, 0);
            transform.Translate(penetration * Vector2.up);
        }
        else if (IsGrounded && Velocity.y < 0f)
        {
            Velocity = new Vector2(Velocity.x, 0f);
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

    #endregion

    #region User Interface

    public void SetConversationState(bool isTalking)
    {
        dialogueOptions.transform.parent.gameObject.SetActive(isTalking);
        canAttack = !isTalking;
        canMove = !isTalking;
    }

    public void SetDialogOptions(string option1, string option2, string option3)
    {
        dialogueOptions.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = option1;
        dialogueOptions.transform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = option2;
        dialogueOptions.transform.GetChild(2).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = option3;

        dialogueOptions.transform.GetChild(0).gameObject.SetActive(!string.IsNullOrEmpty(option1));
        dialogueOptions.transform.GetChild(1).gameObject.SetActive(!string.IsNullOrEmpty(option2));
        dialogueOptions.transform.GetChild(2).gameObject.SetActive(!string.IsNullOrEmpty(option3));
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, collisionCheckRadius);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down);
        Gizmos.DrawWireSphere(transform.position + Vector3.down, collisionCheckRadius);
    }
}
