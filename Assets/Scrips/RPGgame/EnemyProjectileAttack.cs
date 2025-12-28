using UnityEngine;
using System.Collections;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

public class EnemyProjectileAttack : MonoBehaviour
{
    private Color rayColor = Color.blue;
    public float attackRange;
    public bool isCoolDown;
    public Animator animator;
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] Special special;
    // Projectile Logic
    public bool readyToThrow;
    public GameObject objectToThrow;
    public Transform rangeAttackPoint;
    public float throwFroce;
    public float throwUpwardForce;
    public float throwCoolDown;

    void Awake()
    {
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        special = FindAnyObjectByType<Special>();
        GameObject animGameObject = gameObject.transform.Find("Bat").gameObject;
        animator = animGameObject.GetComponent<Animator>();
    }
    void Start()
    {
        readyToThrow = true;
        isCoolDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (readyToThrow)
        {
            enemyDetect();
        }
        
    }
    private void enemyDetect()
    {
        readyToThrow = false;
        GameObject projectile = Instantiate(objectToThrow, rangeAttackPoint.position, objectToThrow.transform.rotation);
        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();

        Vector3 forceToAdd = this.transform.forward * throwFroce + transform.up * throwUpwardForce;
        projectileRB.AddForce(forceToAdd, ForceMode.Impulse);

        Invoke(nameof(ReserThrow), throwCoolDown);
    }

    private void ReserThrow()
    {
        readyToThrow = true;
    }
    IEnumerator CoolDownRoutine()
    {
        animator.Play("BatAttack");
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
