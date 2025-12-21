using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverClip;
    public AudioClip clickClip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        SFXManager.Instance.PlaySFX(hoverClip);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SFXManager.Instance.PlaySFX(clickClip);
    }
}

