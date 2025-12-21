using System.Collections;
using UnityEngine;

public class ProjectileAddon : MonoBehaviour
{
    public Rigidbody rb;
    private bool targetHit;
    int damage;
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
    // private void OnCollisionEnter(Collision other) {
    //     if (targetHit)
    //     {
    //         return;
    //     }
    //     else
    //     {
    //         targetHit = true;
    //     }
    //     rb.isKinematic = true;
    //     rb.linearVelocity = Vector3.zero;
    //     rb.angularVelocity = Vector3.zero;
    //     transform.SetParent(other.transform, true);
    //     if(other.gameObject.tag == "Enemy")
    //     {
    //         other.gameObject.GetComponent<Enemy>().lowerHealth(1);
    //         if (other.gameObject.GetComponent<Enemy>().health <= 0)
    //         {
    //             Destroy(other.gameObject);
    //            //Instantiate(coinPrefab, hit.transform.gameObject.transform.position, Quaternion.Euler(90f, 0f, 0f));
    //         }
    //     }
    // }
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

        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy e = other.gameObject.GetComponent<Enemy>();
            e.lowerHealth(damage);

            if (e.health <= 0)
            {
                e.Die();
            }
        }
    }

    private IEnumerator lingerTimer()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}
