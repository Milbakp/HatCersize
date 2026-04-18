using UnityEngine;

public class HatButton : InteractableObject
{
    public GameObject door;
    private Animator left;
    private Animator right;
    public string leftdoorAnimation;
    public string rightdoorAnimation;
    private AudioSource aud;
    public void Start()
    {
        left = door.transform.Find("Left").gameObject.GetComponent<Animator>();
        right = door.transform.Find("Right").gameObject.GetComponent<Animator>();
        aud = gameObject.GetComponent<AudioSource>();
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            OpenDoor();
        }
    }
    public override void interact()
    {
        OpenDoor();
    }

    public void  OpenDoor()
    {
        left.Play(leftdoorAnimation);
        right.Play(rightdoorAnimation);
        aud.Play();
    }
}
