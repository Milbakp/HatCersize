using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public float moveSpeed;
    public float chaseRange;
    public float rotationSpeed;
    private Transform player;
    private Rigidbody rb;
    public bool isKnockbacked = false;
    public bool rangedEnemey;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        if(GameManager.Instance.CurrentState == GameManager.GameState.Menu)
        {
            Debug.Log("Player is in Menu state, FollowPlayer script will not execute.");
            return;
        }
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

                if (direction != Vector3.zero)
                {
                    // Handle Movement
                    if (!rangedEnemey)
                    {
                        rb.linearVelocity = direction.normalized * moveSpeed;
                    }

                    // Handle Rotation (Snappier than Slerp)
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    Quaternion newRotation = Quaternion.RotateTowards(
                        rb.rotation, 
                        targetRotation, 
                        rotationSpeed * Time.fixedDeltaTime * 100f // Multiplied for better inspector feel
                    );
                    
                    rb.MoveRotation(newRotation);
                }
            }
            else
            {
                rb.linearVelocity = Vector3.zero; // Stop if player is out of range
            }
        }
    }
}
