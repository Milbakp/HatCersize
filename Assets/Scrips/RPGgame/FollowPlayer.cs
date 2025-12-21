using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public float moveSpeed;
    public float chaseRange;
    public float rotationSpeed;
    private Transform player;
    private Rigidbody rb;
    public bool isKnockbacked = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        if (player != null && !isKnockbacked)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer < chaseRange)
            {
                Vector3 direction = player.position - transform.position;
                direction.y = 0f;
                rb.linearVelocity = direction * moveSpeed;

                Quaternion targetRotation = Quaternion.LookRotation(direction);

                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime));
            }
            else
            {
                rb.linearVelocity = Vector2.zero; // Stop if player is out of range
            }
        }
    }
}
