using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEngine;


public class Attack : MonoBehaviour
{
    public Transform cam;
    private Color rayColor = Color.red;
    public GameObject coinPrefab;
    public float attackRange;
    int weaponType;
    int damage;
    // Long Range Attack Variables
    public Transform rangeAttackPoint;
    public GameObject objectToThrow;
    public int totalThrows;
    public float throwCoolDown;
    public KeyCode throwKey = KeyCode.Mouse0;
    public float throwFroce;
    public float throwUpwardForce;
    bool readyToThrow;
    // Aoe variables
    public aoeAttack aoehitBox;
    public Animator anim;
    public List<GameObject> weapons = new List<GameObject>();
    private AudioSource aud;
    public List<AudioClip> audioClips = new List<AudioClip>();
    void Start()
    {
        readyToThrow = true;
        weaponType = PlayerPrefs.GetInt("weaponType");
        aoehitBox = FindAnyObjectByType<aoeAttack>();
        aud = gameObject.GetComponent<AudioSource>();
        // GameObject animGameObject = gameObject.transform.Find("Sword").gameObject;
        // anim = animGameObject.GetComponent<Animator>();
        damage = PlayerPrefs.GetInt("playerAttack");
        for(int i = 0; i < weapons.Count; i++)
        {
            if(i+1 == weaponType)
            {
                weapons[i].SetActive(true);
                anim = weapons[i].GetComponent<Animator>();
            }
            else
            weapons[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0)
        // {
        //     longRangeAttack();
        // }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weaponType = 1;
        }else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weaponType = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            weaponType = 3;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            detect();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            aoeAttack();
        }
    }
    public void detect()
    {
        //Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Debug.DrawRay(cam.position, cam.forward * attackRange, rayColor, 0f);
        anim.Play("SwordSwing");
        aud.PlayOneShot(audioClips[1]);
        if (Physics.Raycast(cam.position,cam.forward, out hit, attackRange) && hit.transform.gameObject.tag == "Enemy")
        {
            hit.transform.gameObject.GetComponent<Enemy>().lowerHealth(damage);
            if (hit.transform.gameObject.GetComponent<Enemy>().health <= 0)
            {
                hit.transform.gameObject.GetComponent<Enemy>().Die();
            }
        }
        Debug.LogError("Hit");
    }

    public void longRangeAttack()
    {
        readyToThrow = false;
        GameObject projectile = Instantiate(objectToThrow, rangeAttackPoint.position, objectToThrow.transform.rotation);
        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();

        Vector3 forceToAdd = cam.transform.forward * throwFroce + transform.up * throwUpwardForce;
        projectileRB.AddForce(forceToAdd, ForceMode.Impulse);

        totalThrows--;
        anim.Play("WandSwing");
        aud.PlayOneShot(audioClips[2]);
        Invoke(nameof(ReserThrow), throwCoolDown);
    }

    private void ReserThrow()
    {
        readyToThrow = true;
    }

    public void aoeAttack()
    {
        if(aoehitBox.isReady() == false)
        {
            Debug.LogError("AOE Attack on Cooldown");
            return;
        }
        anim.Play("HammerSwing");
        aud.PlayOneShot(audioClips[3]);
        aoehitBox.aoeDamage(damage);
    }

    public void callAttack()
    {
        if(weaponType == 1)
        {
            detect();
        }else if(weaponType == 2 && readyToThrow && totalThrows > 0){
            longRangeAttack();
        }else if(weaponType == 3)
        {
            aoeAttack();
        }
        Debug.LogError(damage);
    }
}
