using UnityEditor.PackageManager;
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
		PreparePreavie(previewObject);
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

	private void PreparePreavie(GameObject previewObject)
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

	public void UpdatePosition(Vector3 position, float yRotation, bool validity)
	{
		ObjectTransInfo newInfo = 
        if (previewObject != null)
        {
			MovePreview(gridInfo);
			ApplyFeedBackToPreview(validity);
		}

		MoveCursor(gridInfo);
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

	private void MoveCursor(ObjectTransInfo gridInfo)
	{
		cellIndicator.transform.position = gridInfo.ItemPosition;
		cellIndicator.transform.rotation = Quaternion.Euler(0, gridInfo.ItemYRotation, 0);
	}

	private void MovePreview(ObjectTransInfo gridInfo)
	{
		//previewObject.transform.position = new Vector3(position.x, position.y + previewYOffset, position.z);
		previewObject.transform.position = new Vector3(gridInfo.ItemPosition.x, gridInfo.ItemPosition.y + previewYOffset, gridInfo.ItemPosition.x);
		previewObject.transform.rotation = Quaternion.Euler(0, gridInfo.ItemYRotation, 0);
	}
}
