using System.Runtime.InteropServices.WindowsRuntime;
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
	private int currentRotation;

	private void Start()
	{
		previewMaterialInstance = new Material(previewMaterialsPrefab);
		cellIndicator.SetActive(false);
		cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
	}

	public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
	{
		previewObject = Instantiate(prefab);
		currentRotation = 0;
		PreparePreaview(previewObject);
		PrepareCursor(size);
		cellIndicator.SetActive(true);
	}

	public void RotatePreview(int angle)
	{
		if (previewObject == null) return;

		currentRotation += angle; 
		currentRotation %= 360;

		/* Get Select Object's Bound Size */
		Transform targetTransform = GetMeshTransform(previewObject.transform);
		if (targetTransform == null) return;
		Renderer renderer = targetTransform.GetComponent<Renderer>();
		Vector3 objectSize = renderer.bounds.size;

		/* Calculate Roation Center To Objects X/Z Center */
		Vector3 newPivot = new(objectSize.x * 0.5f, 0, objectSize.z * 0.5f);

		/* Rotate */
		targetTransform.transform.position -= newPivot;
		targetTransform.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
		targetTransform.transform.position += newPivot;
	}

	internal void StartShowingRemovePreview()
	{
		cellIndicator.SetActive(true);
		PrepareCursor(Vector2Int.one);
		ApplyFeedBackToCursor(false);
	}

	private void PrepareCursor(Vector2Int size)
	{
		if (size.x > 0 || size.y > 0)
		{
			cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
			cellIndicatorRenderer.material.mainTextureScale = size;
		}
	}

	private void PreparePreaview(GameObject previewObject)
	{
		Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in renderers)
		{
			Material[] materials = renderer.materials;
			for (int i = 0; i < materials.Length; i++)
			{
				materials[i] = previewMaterialInstance;
			}
			renderer.materials = materials;
		}
	}

	public void StopShowingPreview()
	{
		cellIndicator.SetActive(false);
		if (previewObject != null)
			Destroy(previewObject);
	}

	public void UpdatePosition(Vector3 position, bool validity)
	{
		if (previewObject != null)
		{
			MovePreview(position);
			ApplyFeedBackToPreview(validity);
		}

		MoveCursor(position);
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

	private void MoveCursor(Vector3 position)
	{
		cellIndicator.transform.position = position;
	}

	private void MovePreview(Vector3 position)
	{
		previewObject.transform.position = new Vector3(position.x, position.y + previewYOffset, position.z);
	}

	private Transform GetMeshTransform(Transform parent)
	{
		foreach(Transform child in parent)
		{
			if (child.GetComponent<Renderer>() != null)
			{
				return child;
			}
		}
		return null;
	}

	public Quaternion GetCurrentRotation()
	{
		return Quaternion.Euler(0, currentRotation, 0);
	}
}
