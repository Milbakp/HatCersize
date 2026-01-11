using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private Color rayColor = Color.blue;
    public float attackRange;
    public bool isCoolDown;
    public Animator animator;
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] Special special;
    public AudioSource attackSound;
    void Awake()
    {
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        special = FindAnyObjectByType<Special>();
        GameObject animGameObject = gameObject.transform.Find("Bat").gameObject;
        animator = animGameObject.GetComponent<Animator>();
        attackSound = GetComponent<AudioSource>();
    }
    void Start()
    {
        isCoolDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        enemyDetect();
    }
    private void enemyDetect()
    {
        //Quaternion targetRotation = Quaternion.LookRotation(direction);
        //rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime));
        RaycastHit hit;
        Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward * attackRange, rayColor, 0f);
        if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, attackRange))
        {
            //Debug.Log("Hit " + hit.transform.name);
            if (hit.transform.gameObject.tag == "PlayerCollision" && !isCoolDown)
            {
                StartCoroutine(CoolDownRoutine());
            }
        }
    }
    IEnumerator CoolDownRoutine()
    {
        animator.Play("BatAttack");
        attackSound.Play();
        if(!special.getShieldStatus())
        {
            playerHealth.health -= 1;
        }
        else
        {
            special.shieldHealth -= 1;
            if(special.shieldHealth <= 0)
            {
                special.shieldDeactivate();
            }
        }
        Debug.Log("Player Health: " + playerHealth.health);
        isCoolDown = true;
        yield return new WaitForSeconds(2f);
        isCoolDown = false;
    }
}

