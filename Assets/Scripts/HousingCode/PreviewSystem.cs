using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
	[SerializeField] 
	private float previewYOffset = 0.06f;

	[SerializeField] 
	private GameObject cellIndicator;
	private GameObject previewObject;

	[SerializeField] 
	private Material previewMaterialsPrefab;
	private Material previewMaterialInstance;

	private Renderer cellIndicatorRenderer;

	private void Start()
	{
		previewMaterialInstance = new Material(previewMaterialsPrefab);
		cellIndicator.SetActive(false);
		cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
	}

	public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
	{
		previewObject = Instantiate(prefab);
		PreparePreaview(previewObject);
		PrepareCursor(size);
		cellIndicator.SetActive(true);
	}

	internal void StartShowingRemovePreview()
	{
		cellIndicator.SetActive(true);
		PrepareCursor(Vector2Int.one);
		ApplyFeedBackToCursor(false);
	}

	private void PrepareCursor(Vector2Int size)
	{
		if(size.x > 0 || size.y > 0)
		{
			cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
			cellIndicatorRenderer.material.mainTextureScale = size;
		}
	}

	private void PreparePreaview(GameObject previewObject)
	{
		Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
		foreach(Renderer renderer in renderers)
		{
			Material[] materials = renderer.materials;
			for(int i = 0; i< materials.Length; i++)
			{
				materials[i] = previewMaterialInstance;
			}
			renderer.materials = materials;
		}
	}

	public void StopShowingPreview()
	{
		cellIndicator.SetActive(false);
		if(previewObject != null)
			Destroy(previewObject);
	}

	public void UpdatePosition(ObjectTransInfo objectInfo, bool validity)
	{
        if (previewObject != null)
        {
			MovePreview(objectInfo);
			ApplyFeedBackToPreview(validity);
		}

		MoveCursor(objectInfo);
		ApplyFeedBackToCursor(validity);
	}

	private void ApplyFeedBackToPreview(bool validity)
	{
		Color color = validity ? Color.white : Color.red;
		color.a = 0.5f;
		previewMaterialInstance.color = color;
	}

	private void ApplyFeedBackToCursor(bool validity)
	{
		Color color = validity ? Color.white : Color.red;
		color.a = 0.5f;
		cellIndicatorRenderer.material.color = color;
	}

	private void MoveCursor(ObjectTransInfo objectInfo)
	{
		cellIndicator.transform.position = objectInfo.ObjectPosition;
		cellIndicator.transform.rotation = Quaternion.Euler(0, objectInfo.ObjectYRotation, 0);
	}

	private void MovePreview(ObjectTransInfo objectInfo)
	{
		previewObject.transform.position = new Vector3(
			objectInfo.ObjectPosition.x, objectInfo.ObjectPosition.y + previewYOffset, objectInfo.ObjectPosition.z);
		previewObject.transform.rotation = Quaternion.Euler(0, objectInfo.ObjectYRotation, 0);
	}
}