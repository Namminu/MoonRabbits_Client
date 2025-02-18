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
		txt_ItemDesc.text = item.Description;
	}

	public RectTransform SetTooltipUIPos(RectTransform slotRect)
	{
		/* 캔버스 스케일러에 따른 해상도 대응 */
		float wRatio = Screen.width / canvasScaler.referenceResolution.x;
		float hRatio = Screen.height / canvasScaler.referenceResolution.y;
		float ratio =
			wRatio * (1f - canvasScaler.matchWidthOrHeight) +
			hRatio * (canvasScaler.matchWidthOrHeight);

		float slotWidth = slotRect.rect.width / ratio;
		float slotHeight = slotRect.rect.height / ratio;

		/* 툴팁 초기 위치 설정 : 슬롯 우하단 */
		rt.position = slotRect.position + new Vector3(slotWidth, -slotHeight);
		Vector2 pos = rt.position;

		/* 툴팁 크기 */
		float width = rt.rect.width * ratio;
		float height = rt.rect.height * ratio;

		/* 툴팁(우측, 하단)이 화면 밖으로 나가는지 여부 */
		bool rightSideOutRange = pos.x + width > Screen.width;
		bool bottomSideOutRange = pos.y - height < 0;
		ref bool R = ref rightSideOutRange;
		ref bool B = ref bottomSideOutRange;

		if (R && !B) // 오른쪽 나가는 경우 -> 슬롯의 Left Bottom
		{
			rt.position = new Vector2(pos.x - width - slotWidth, pos.y);
		}
		else if (!R && B) // 아래쪽 나가는 경우 -> 슬롯의 Right Top
		{
			rt.position = new Vector2(pos.x, pos.y + height + slotHeight);
		}
		else if (R && B) //오른쪽, 아래쪽 나가는 경우 -> 슬롯의 Left Top
		{
			rt.position = new Vector2(pos.x - width - slotWidth, pos.y + height + slotHeight);
		}

		return rt;
	}

	public void Show() {  gameObject.SetActive(true); Debug.Log("툴팁 띄웁니다"); }
	public void Hide() { gameObject.SetActive(false); Debug.Log("툴팁 지워요"); }
}
