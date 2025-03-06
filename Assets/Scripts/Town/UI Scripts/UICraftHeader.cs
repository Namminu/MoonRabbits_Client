using UnityEngine;
using UnityEngine.EventSystems;

public class DragUI : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private RectTransform uiCraftRect; // UICraft의 RectTransform
    private Canvas canvas; // 부모 Canvas
    private Vector2 offset; // 클릭 위치와 RectTransform의 차이

    private void Awake()
    {
        // UICraft의 RectTransform과 Canvas를 가져옵니다.
        uiCraftRect = transform.parent.GetComponent<RectTransform>();
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
        offset = uiCraftRect.anchoredPosition - offset;
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

        uiCraftRect.anchoredPosition = localPoint + offset;
    }
}
