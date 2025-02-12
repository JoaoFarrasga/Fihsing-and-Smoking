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
    public DistanceHandGrabInteractable grab;

    // Limits for diagonal movement (relative to spawn position)
    [Header("Diagonal Movement Range")]
    public float moveRangeX = 2f;  // Total movement range on X
    public float moveRangeZ = 2f;  // Total movement range on Z
    public float moveSpeed = 0.5f; // Speed of movement

    private Vector3 startPoint;
    private Vector3 endPoint;
    private Vector3 direction;
    private bool movingForward = true;


    [SerializeField] private float rotationOffsetY = 90f;
    // Ajuste o valor 90f ou -90f dependendo de como seu modelo est� no mundo
    
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


    private void MoveAlongLine()
    {
        Vector3 moveDir = movingForward ? direction : -direction;

        // Normalizamos para evitar problemas de magnitude.
        moveDir = moveDir.normalized;

        // 1) "Fatiar" a componente Y (caso queira ignorar subidas/descidas)
        Vector3 planarDirection = new Vector3(moveDir.x, 0f, moveDir.z);

        // 2) Gerar o "LookRotation" base
        Quaternion targetRot = Quaternion.LookRotation(planarDirection, Vector3.up);

        // 3) Adicionar offset (ex: 90 graus no Y)
        targetRot *= Quaternion.Euler(0f, rotationOffsetY, 0f);

        // 4) Ajustar a rota��o
        transform.rotation = targetRot;

        // 5) Mover
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        // Inverter dire��o ao chegar nos limites
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
        grab.MaxInteractors = 0;
        if(holder != null) holder.GetComponent<Magnet>().points.Remove(this);
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
        center.gameObject.SetActive(true);
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
