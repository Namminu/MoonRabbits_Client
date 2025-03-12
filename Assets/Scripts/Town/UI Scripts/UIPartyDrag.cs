using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPartyDrag : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private RectTransform uiPartyRect; // UICraft의 RectTransform
    private Canvas canvas; // 부모 Canvas
    private Vector2 offset; // 클릭 위치와 RectTransform의 차이

    // Start is called before the first frame update
    void Awake()
    {
        uiPartyRect = transform.parent.GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 클릭한 위치와 RectTransform의 위치 차이를 계산
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out offset
        );
        offset = uiPartyRect.anchoredPosition - offset;

        CanvasManager.Instance.partyUI.gameObject.transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null)
            return;

        // 드래그 중 위치 업데이트
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint
        );

        uiPartyRect.anchoredPosition = localPoint + offset;
    }
}
