using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float arrivalDistance = 0.02f;

    private readonly Queue<Vector3> pathQueue = new Queue<Vector3>();
    private Rigidbody2D rb;

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
            return;

        foreach (Vector3 waypoint in path)
        {
            Vector2 target = new Vector2(waypoint.x, waypoint.y);

            // Skip only the waypoint if the body is already there.
            if (Vector2.Distance(rb.position, target) > arrivalDistance)
            {
                pathQueue.Enqueue(waypoint);
            }
        }
    }

    private void FixedUpdate()
    {
        if (pathQueue.Count == 0)
            return;

        Vector3 waypoint = pathQueue.Peek();
        Vector2 target = new Vector2(waypoint.x, waypoint.y);

        if (Vector2.Distance(rb.position, target) <= arrivalDistance)
        {
            rb.position = target;
            pathQueue.Dequeue();
            return;
        }

        Vector2 nextPosition = Vector2.MoveTowards(
            rb.position,
            target,
            moveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPosition);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        CapsuleCollider2D capsule = GetComponent<CapsuleCollider2D>();
        Vector3 gizmoPosition = capsule != null ? transform.TransformPoint(capsule.offset) : transform.position;
        Gizmos.DrawWireSphere(gizmoPosition, 0.1f);
    }
}