using Oculus.Interaction.HandGrab;
using Oculus.Interaction;
using UnityEngine;

public class Point : MonoBehaviour
{
    public int pointsToGive;
    private Rigidbody rb;
    public bool isBeingPulled = false;
    public bool gotCatched = false;
    private Vector3 pullTarget;
    public float pullSpeed;

    public Transform holder;
    public AudioSource audioS;
    [SerializeField] private GameObject center;

    // Limits for diagonal movement (relative to spawn position)
    [Header("Diagonal Movement Range")]
    public float moveRangeX = 2f;  // Total movement range on X
    public float moveRangeZ = 2f;  // Total movement range on Z
    public float moveSpeed = 0.5f; // Speed of movement

    private Vector3 startPoint;
    private Vector3 endPoint;
    private Vector3 direction;
    private bool movingForward = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 5f;
        rb.drag = 0.5f;
        rb.angularDrag = 4f;

        // Set movement path dynamically
        SetMovementPath();
    }

    private void Update()
    {
        if (!isBeingPulled && !gotCatched)
        {
            MoveAlongLine();
        }
    }

    private void FixedUpdate()
    {
        if (isBeingPulled)
        {
            PullPoint();
        }
    }

    /// <summary>
    /// Defines the movement path dynamically based on the fish's spawn position.
    /// </summary>
    private void SetMovementPath()
    {
        Vector3 spawnPos = transform.position;

        // Calculate start and end points dynamically based on spawn position
        startPoint = new Vector3(spawnPos.x - moveRangeX / 2, spawnPos.y, spawnPos.z - moveRangeZ / 2);
        endPoint = new Vector3(spawnPos.x + moveRangeX / 2, spawnPos.y, spawnPos.z + moveRangeZ / 2);

        // Set movement direction
        direction = (endPoint - startPoint).normalized;

        // Randomly decide initial movement direction
        movingForward = Random.value > 0.5f;

        // Snap to the start position initially
        transform.position = movingForward ? startPoint : endPoint;
    }

    /// <summary>
    /// Moves the object diagonally between start and end points.
    /// </summary>
    private void MoveAlongLine()
    {
        Vector3 moveDir = movingForward ? direction : -direction;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        // Reverse direction when reaching limits
        if (movingForward && Vector3.Distance(transform.position, endPoint) < 0.1f)
        {
            movingForward = false;
        }
        else if (!movingForward && Vector3.Distance(transform.position, startPoint) < 0.1f)
        {
            movingForward = true;
        }
    }

    private void PullPoint()
    {
        Vector3 direction = (pullTarget - transform.position).normalized;
        rb.AddForce(direction * pullSpeed * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    public void StartPullingTowards(Vector3 target, float speed)
    {
        pullTarget = target;
        pullSpeed = speed;
        isBeingPulled = true;
        rb.useGravity = false;
    }

    public void StopPulling()
    {
        isBeingPulled = false;
        gotCatched = true;
    }

    public void GetReleased()
    {
        rb.useGravity = true;
        holder = null;
        transform.SetParent(null, true);
        StopPulling();
    }

    public void ActivateSelf()
    {
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        Invoke("AdjustFallingSpeed", 0.5f);
    }

    public void AdjustFallingSpeed()
    {
        rb.drag = 0.1f;
    }

    public void GetPickedUP(Transform magnet, float speed)
    {
        holder = magnet;
        transform.SetParent(holder, true);
        StartPullingTowards(magnet.position, speed);
    }

    public void GetPoints()
    {
        GameManager.Instance.AddPoints(100);
        center.GetComponent<ParticleSystem>().Play();
        Destroy(gameObject, center.GetComponent<ParticleSystem>().main.duration);
    }

    /// <summary>
    /// Draws the movement path in the Scene View.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(startPoint, endPoint);
        }
    }
}
