using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    private enum FacingDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float arrivalDistance = 0.02f;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Transform _spriteTransform;

    private readonly Queue<Vector3> pathQueue = new Queue<Vector3>();
    private Rigidbody2D rb;
    private FacingDirection facingDirection = FacingDirection.Down;

    public bool IsMoving => pathQueue.Count > 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    public void SetPath(List<Vector3> path)
    {
        pathQueue.Clear();

        if (path == null || path.Count == 0)
        {
            UpdateAnimation();
            return;
        }

        foreach (Vector3 waypoint in path)
        {
            Vector2 target = new Vector2(waypoint.x, waypoint.y);

            // Skip only the waypoint if the body is already there.
            if (Vector2.Distance(rb.position, target) > arrivalDistance)
            {
                pathQueue.Enqueue(waypoint);
            }
        }
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (pathQueue.Count == 0)
        {
            UpdateAnimation();
            return;
        }

        Vector3 waypoint = pathQueue.Peek();
        Vector2 target = new Vector2(waypoint.x, waypoint.y);

        if (Vector2.Distance(rb.position, target) <= arrivalDistance)
        {
            rb.position = target;
            pathQueue.Dequeue();
            UpdateAnimation();
            return;
        }

        Vector2 direction = target - rb.position;
        UpdateFacingDirection(direction);

        Vector2 nextPosition = Vector2.MoveTowards(
            rb.position,
            target,
            moveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPosition);
        UpdateAnimation();
    }

    private void UpdateFacingDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            facingDirection = direction.x > 0 ? FacingDirection.Right : FacingDirection.Left;
        }
        else
        {
            facingDirection = direction.y > 0 ? FacingDirection.Up : FacingDirection.Down;
        }
        ApplyFacingDirection();
    }

    private void ApplyFacingDirection()
    {
        switch (facingDirection)
        {
            case FacingDirection.Down:
                _spriteTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                _spriteRenderer.flipX = false;
                _spriteRenderer.flipY = false;
                break;
            case FacingDirection.Up:
                _spriteTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                _spriteRenderer.flipX = false;
                _spriteRenderer.flipY = true;
                break;
            case FacingDirection.Left:
                // Rotate the sprite so it appears as facing left
                _spriteTransform.localRotation = Quaternion.Euler(0f, 0f, -90f);
                _spriteRenderer.flipX = false;
                _spriteRenderer.flipY = false;
                break;
            case FacingDirection.Right:
                // Rotate the sprite so it appears as facing right
                _spriteTransform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                _spriteRenderer.flipX = false;
                _spriteRenderer.flipY = false;
                break;
        }
    }
    private void UpdateAnimation()
    {
        bool walking = IsMoving;

        _animator.SetBool("IsWalking", walking);
        _animator.SetBool("IsIdle", !walking);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        CapsuleCollider2D capsule = GetComponent<CapsuleCollider2D>();
        Vector3 gizmoPosition = capsule != null ? transform.TransformPoint(capsule.offset) : transform.position;
        Gizmos.DrawWireSphere(gizmoPosition, 0.1f);
    }
}