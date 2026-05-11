using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public float chaseDistance = 10f;
    public float speedLimitMin = 100f;
    public float speedLimitMax = 120f;
    private NavMeshAgent agent;
    private Rigidbody rb;
    void Start()
    {
        if(GameManager.Instance.CurrentState == GameManager.GameState.Menu)
        {
            Debug.Log("Player is in Menu state, Script will not execute.");
            this.enabled = false; // Disable this script to prevent it from running in the Menu state
            return;
        }
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        // IMPORTANT: Tell the agent NOT to update the transform position automatically
        agent.updatePosition = false; 
        agent.updateRotation = true; 

        StartCoroutine(adjustSpeed());
    }
    void FixedUpdate()
    {
        float distanceToTarget = Vector3.Distance(transform.position, agent.destination);
        // Stop moving if we are within a small 'arrival' radius (e.g., 2 units)
        if (distanceToTarget > 2f)
        {
            Vector3 desiredVelocity = agent.desiredVelocity;
            rb.linearVelocity = new Vector3(desiredVelocity.x, rb.linearVelocity.y, desiredVelocity.z);
        }
        else
        {
            // Kill horizontal velocity when close to destination
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    void Update()
    {   
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < chaseDistance -1)
        {
            ChasePlayer();
        }
        // Sync the Agent's internal simulation position to where the enemy actually is
        agent.nextPosition = transform.position;
    }

    void ChasePlayer()
    {
        Vector3 directionToPlayer =  player.position - transform.position;
        Vector3 newPos = transform.position + directionToPlayer.normalized * chaseDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(newPos, out hit, chaseDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
    IEnumerator adjustSpeed()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            agent.speed = Mathf.Floor(Random.Range(speedLimitMin, speedLimitMax));
        }
    }
    public void onSceneUnloaded()
    {
        StopAllCoroutines(); // Stop the speed adjustment coroutine when the scene is unloaded
    }
}
