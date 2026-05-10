// Reordering levels in the campaign editor.
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class ReorderItems : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform parentToReturnTo = null;
    private GameObject placeholder = null;
    public TMP_Text levelNameText;

    public void Awake()
    {
        levelNameText = GetComponentInChildren<TMP_Text>();
    }

    public void Start()
    {
        parentToReturnTo = this.transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 1. Create a placeholder to keep the space in the Layout Group
        placeholder = new GameObject("Placeholder");
        placeholder.transform.SetParent(parentToReturnTo);
        LayoutElement le = placeholder.AddComponent<LayoutElement>();
        LayoutElement thisLe = GetComponent<LayoutElement>();
        
        le.preferredWidth = thisLe.preferredWidth;
        le.preferredHeight = thisLe.preferredHeight;
        le.flexibleWidth = 0;
        le.flexibleHeight = 0;

        placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());
        
        // Detach so it doesn't get squished by the Layout Group while dragging
        // We use a placeholder or simply change the sibling index
        this.transform.SetAsLastSibling(); 
        
        // Make the item ignore raycasts so we can detect what's UNDER it
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 1. Calculate the local position within the parent
        RectTransform parentRect = parentToReturnTo.GetComponent<RectTransform>();
        Vector2 localPointerPos;
        
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, eventData.pressEventCamera, out localPointerPos))
        {
            Rect rect = parentRect.rect;

            // 2. Clamp the local position so it stays inside the parent's rectangle
            // We use half the size of the dragged item to prevent it from even partially poking out
            RectTransform myRect = GetComponent<RectTransform>();
            float halfWidth = myRect.rect.width / 2;
            float halfHeight = myRect.rect.height / 2;

            localPointerPos.x = Mathf.Clamp(localPointerPos.x, rect.xMin + halfWidth, rect.xMax - halfWidth);
            localPointerPos.y = Mathf.Clamp(localPointerPos.y, rect.yMin + halfHeight, rect.yMax - halfHeight);

            // 3. Convert that clamped local position back to world space
            this.transform.position = parentRect.TransformPoint(localPointerPos);
        }

        // Follow the mouse
        //this.transform.position = new Vector3(this.transform.position.x, eventData.position.y, this.transform.position.z);        

        int newSiblingIndex = parentToReturnTo.childCount;

        for (int i = 0; i < parentToReturnTo.childCount; i++)
        {
            Transform sibling = parentToReturnTo.GetChild(i);
            
            // Check if mouse is above the vertical midpoint of the sibling
            if (eventData.position.y > sibling.position.y)
            {
                newSiblingIndex = i;
                if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                    newSiblingIndex--; // Adjust for the placeholder's own existence

                break;
            }
        }

        placeholder.transform.SetSiblingIndex(newSiblingIndex);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(parentToReturnTo);
        this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        
        Destroy(placeholder);
    }

    public void SetLevelName(string name)
    {
        Debug.LogError("Setting level name: " + name);
        levelNameText.text = name;
        this.transform.gameObject.name = name; // Set the GameObject's name so it can be easily accessed later when creating the campaign data
    }
    public void deleteItem()
    {
        Destroy(this.gameObject);
    }
}
