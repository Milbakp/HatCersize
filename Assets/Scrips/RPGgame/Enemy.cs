using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public int health;
    public GameObject coinPrefab;
    // Knockback variables
    public Rigidbody rb; // Assign this in the Inspector or get it in Awake/Start
    public float knockbackForce;
    public FollowPlayer fp;
    public float KnockBackdelay;
    // Hit color variables
    public float hitFlashDuration = 0.2f; // Duration of white flash
    private Renderer[] renderers;
    private Color[] originalColors;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        fp = gameObject.GetComponent<FollowPlayer>();

        // Get all Renderer components in this object and its children
        renderers = GetComponentsInChildren<Renderer>();

        // Store original colors
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            lowerHealth(1);
        }
    }
    public void Die()
    {
        Instantiate(coinPrefab, transform.position, Quaternion.Euler(90f, 0f, 0f));
        RPGLevelManager levelManager = FindAnyObjectByType<RPGLevelManager>();
        if (levelManager != null)
        {
            levelManager.numOfEnemies -= 1;
        }
        Destroy(gameObject);
    }
    public void lowerHealth(int damage)
    {
        health -= damage;
        StartCoroutine(FlashWhite());
        knockBack();
    }
    // Continue to work on the knockback effect
    public void knockBack()
    {
        StartCoroutine(ResetKnockback(KnockBackdelay));
        // Reset current velocity to make the knockback consistent
        rb.linearVelocity = Vector3.zero;

        // Apply knockback in the opposite direction the object is facing
        rb.AddForce(-transform.forward * knockbackForce, ForceMode.Impulse);
    }

    public IEnumerator ResetKnockback(float delay)
    {
        fp.isKnockbacked = true;
        yield return new WaitForSeconds(delay);
        fp.isKnockbacked = false;
    }

    private IEnumerator FlashWhite()
    {
        // Change all renderers to white
        foreach (var rend in renderers)
        {
            rend.material.color = Color.white;
        }

        // Wait for duration
        yield return new WaitForSeconds(hitFlashDuration);

        // Revert all renderers to original colors
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = originalColors[i];
        }
    }
}
