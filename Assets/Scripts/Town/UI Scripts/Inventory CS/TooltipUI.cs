using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
	[SerializeField] private TMP_Text txt_ItemName;
	[SerializeField] private TMP_Text txt_ItemDesc;

	private RectTransform rt;
	private CanvasScaler canvasScaler;

	private static readonly Vector2 LeftTop = new Vector2(0f, 1f);
	private static readonly Vector2 LeftButtom = new Vector2(0f, 0f);
	private static readonly Vector2 RightTop = new Vector2(1f, 1f);
	private static readonly Vector2 RightBottom = new Vector2(1f, 0f);


	private void Awake()
	{
		Init();
		Hide();
	}

	private void Init()
	{
		TryGetComponent(out rt);
		rt.pivot = LeftTop;
		canvasScaler = GetComponentInParent<CanvasScaler>();

		CheckoutAllChildRayCastTarget(transform);
	}

	private void CheckoutAllChildRayCastTarget(Transform tr)
	{
		tr.TryGetComponent(out Graphic gr);
		if (gr != null) gr.raycastTarget = false;

		int childCount = tr.childCount;
		if (childCount == 0) return;

		for (int i = 0; i < childCount; i++)
			CheckoutAllChildRayCastTarget(tr.GetChild(i));
	}


	public void SetItemDesc(ItemData item)
	{
		txt_ItemName.text = item.ItemName;
		txt_ItemDesc.text = item.ItemDescription;
	}

	public RectTransform SetTooltipUIPos(RectTransform slotRect)
	{
		Vector3 newPos = GetSelectedConer(slotRect, 3);
		rt.position = newPos;
		Vector2 pos = rt.position;

		float wRatio = Screen.width / canvasScaler.referenceResolution.x;
		float hRatio = Screen.height / canvasScaler.referenceResolution.y;
		float ratio =
			wRatio * (1f - canvasScaler.matchWidthOrHeight) +
			hRatio * (canvasScaler.matchWidthOrHeight);

		/* 툴팁 창의 width, height */
		float width = rt.rect.width;
		float height = rt.rect.height;
		/* 툴팁(우측, 하단)이 화면 밖으로 나가는지 여부 */
		bool rightSideOutRange = pos.x + width > Screen.width;
		bool bottomSideOutRange = pos.y - height < 0;
		ref bool R = ref rightSideOutRange;
		ref bool B = ref bottomSideOutRange;

		if (R && !B) // 오른쪽 나가는 경우 -> 슬롯의 Left Bottom
		{
			Vector3 slotsLeftBottom = GetSelectedConer(slotRect, 0);
			rt.position = new Vector2(slotsLeftBottom.x - width + slotRect.rect.width, slotsLeftBottom.y);
		}
		else if (!R && B) // 아래쪽 나가는 경우 -> 슬롯의 Right Top
		{
			Vector3 slotsRightTop = GetSelectedConer(slotRect, 2);
			rt.position = new Vector2(slotsRightTop.x, slotsRightTop.y + height - slotRect.rect.height);
		}
		else if (R && B) //오른쪽, 아래쪽 나가는 경우 -> 슬롯의 Left Top
		{
			Vector3 slotsLeftTop = GetSelectedConer(slotRect, 1);
			rt.position = new Vector2(slotsLeftTop.x - width + slotRect.rect.width, slotsLeftTop.y + height - slotRect.rect.height);
		}

		return rt;
	}

	/// <summary>
	/// RectTransform For Object's Corner Position
	/// param corner : [0] = left bottom, [1] = left top, [2] = right top, [3] = right bottom
	/// </summary>
	private Vector3 GetSelectedConer(RectTransform rectTr, int corner)
	{
		Vector3[] worldCorners = new Vector3[4];
		rectTr.GetWorldCorners(worldCorners);
		return worldCorners[corner];
	}

	public void Show() { gameObject.SetActive(true); }
	public void Hide() { gameObject.SetActive(false); }
}
