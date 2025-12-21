using System.Collections;
using UnityEngine;

public class Special : MonoBehaviour
{
    public int specialType;
    [SerializeField] PlayerHealth playerHealth;
    private bool shieldIsActive;
    private bool shieldOnCooldown;
    public int shieldHealth;
    public int shieldDurationSeconds;
    public int shieldCooldownSeconds;
    private Coroutine shieldCoroutine;
    public GameObject shieldVisual;
    // Dashing
    [SerializeField] Dash dash;
    void Start()
    {
        specialType = PlayerPrefs.GetInt("specialType");
        shieldIsActive = false;
        dash = FindAnyObjectByType<Dash>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            specialType = 1;
        }else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            specialType = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            specialType = 3;
        }
        //Debug.Log("Special Type: " + specialType);
    }
    public void specialCall()
    {
        if (specialType == 1)
        {
            shieldActivate();
        }else if (specialType == 2)
        {
            dash.DoDash();
        }
    }
    public void shieldActivate()
    {
        if (shieldOnCooldown)
        {
            Debug.LogError("Shield is on cooldown");
            return;
        }
        shieldVisual.SetActive(true);
        shieldIsActive = true;
        shieldCoroutine = StartCoroutine(shieldDuration(shieldDurationSeconds));
        StartCoroutine(shieldCooldown());
        Debug.LogError("Shield Activated");
    }
    IEnumerator shieldCooldown()
    {
        shieldOnCooldown = true;
        yield return new WaitForSeconds(shieldCooldownSeconds);
        shieldOnCooldown = false;
    }
    IEnumerator shieldDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        shieldIsActive = false;
        shieldVisual.SetActive(false);
        Debug.LogError("Shield Deactivated");
    }
    public void shieldDeactivate()
    {
        shieldIsActive = false;
        shieldVisual.SetActive(false);
        StopCoroutine(shieldCoroutine);
        Debug.LogError("Shield Broken");
    }
    public bool getShieldStatus()
    {
        return shieldIsActive;
    }
}
