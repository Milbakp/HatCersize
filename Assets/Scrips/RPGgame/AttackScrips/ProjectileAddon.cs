using System.Collections;
using UnityEngine;

public class ProjectileAddon : MonoBehaviour
{
    public Rigidbody rb;
    private bool targetHit;
    int damage;
    public int projectileType;
    void Start()
    {
        targetHit = false;
        rb = gameObject.GetComponent<Rigidbody>();
        StartCoroutine(lingerTimer());
        damage = PlayerPrefs.GetInt("playerAttack");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (targetHit) return;
        targetHit = true;
        if (other.gameObject.CompareTag("Environment") || other.gameObject.CompareTag("Enemy"))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            GetComponent<Collider>().enabled = false; // Disable collider to stop future collisions
            transform.SetParent(other.gameObject.transform, true);
            rb.Sleep();
             
        }

        if (other.gameObject.CompareTag("Enemy") && projectileType == 0)
        {
            Enemy e = other.gameObject.GetComponent<Enemy>();
            e.lowerHealth(damage);

            if (e.health <= 0)
            {
                e.Die();
            }
        }

        if (other.gameObject.CompareTag("PlayerCollision") && projectileType == 1)
        {
            PlayerHealth ph = other.gameObject.GetComponent<PlayerHealth>();
            ph.health -= 1;
        }
    }

    private IEnumerator lingerTimer()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}
