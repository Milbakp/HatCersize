using UnityEngine;
using UnityEngine.EventSystems; // Required
using TMPro;

public class FloatingHint : MonoBehaviour
{
    private float floatingHintTimer;
    public GameObject floatingHintObject;
    public TMP_Text hintText;
    private bool isFloatingHintActive = false;
    public float XOffset, YOffset;
    public float XMax, YMax;
    private Vector3 mousePos;
    void Start()
    {
        floatingHintTimer = 0f;
        floatingHintObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(isFloatingHintActive){
            floatingHintTimer += Time.deltaTime;
            if(floatingHintTimer >= 1.5f)
            {
                floatingHintObject.SetActive(true);
                isFloatingHintActive = false;
            }
        }
        mousePos = Input.mousePosition;
        if(mousePos.y > YMax && mousePos.x > XMax)
        {
            floatingHintObject.transform.position =  new Vector3(Input.mousePosition.x - XOffset, Input.mousePosition.y - YOffset, 0f);
        }
        else if(mousePos.y > YMax)
        {
            floatingHintObject.transform.position =  new Vector3(Input.mousePosition.x + XOffset, Input.mousePosition.y - YOffset, 0f);
        }
        else if(mousePos.x > XMax)
        {
            floatingHintObject.transform.position =  new Vector3(Input.mousePosition.x - XOffset, Input.mousePosition.y + YOffset, 0f);
        }
        else{
            floatingHintObject.transform.position =  new Vector3(Input.mousePosition.x + XOffset, Input.mousePosition.y + YOffset, 0f);
        }
        // Debug.Log("Mouse Position: " + mousePos);
    }
    public void StartFloatingHint(BaseEventData eventData)
    {
        // Cast to PointerEventData to access pointer-specific info
        PointerEventData pointerData = eventData as PointerEventData;
        
        if (pointerData == null)
        {  
            return;
        }
        GameObject hoveredObject = pointerData.pointerCurrentRaycast.gameObject;

        string hintMessage = hoveredObject.GetComponent<HintText>().hintMessage;
        hintText.text = hintMessage;
        floatingHintTimer = 0f;
        isFloatingHintActive = true;
        //floatingHintObject.SetActive(true);
    }

    public void StopFloatingHint()
    {
        floatingHintTimer = 0f;
        isFloatingHintActive = false;
        floatingHintObject.SetActive(false);
    }
}
