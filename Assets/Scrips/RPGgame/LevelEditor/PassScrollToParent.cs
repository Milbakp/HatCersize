using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PassScrollToParent : MonoBehaviour, IScrollHandler
{
    private ScrollRect parentScrollRect;

    void Start()
    {
        // Search upwards in the hierarchy to find the ScrollRect
        parentScrollRect = GetComponentInParent<ScrollRect>();
        
        if (parentScrollRect == null)
        {
            Debug.LogWarning("PassScrollToParent: No ScrollRect found in parents!", gameObject);
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        // Pass the scroll data directly to the ScrollRect component
        if (parentScrollRect != null)
        {
            parentScrollRect.OnScroll(eventData);
        }
    }
}
