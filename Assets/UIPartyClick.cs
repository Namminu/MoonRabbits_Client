using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPartyClick : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        CanvasManager.Instance.partyUI.partyWindow.transform.SetAsLastSibling();
    }
}
