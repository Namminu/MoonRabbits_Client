using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
	[SerializeField] private List<GameObject> placedGameObject = new();

	public int PlaceObject(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		GameObject newObject = Instantiate(prefab);

		//  회전 중심 위치 보정
		Renderer renderer = newObject.GetComponentInChildren<Renderer>();
		if (renderer != null)
		{
			Vector3 objectSize = renderer.bounds.size;
			Vector3 pivotOffset = new Vector3(objectSize.x * 0.5f, 0, objectSize.z * 0.5f);
			position -= rotation * pivotOffset; //  회전 적용된 보정값 반영
		}

		newObject.transform.position = position;
		newObject.transform.rotation = rotation;
		placedGameObject.Add(newObject);

		return placedGameObject.Count - 1;
	}

	internal void RemoveObjectAt(int gameObjectIndex)
	{
		if (placedGameObject.Count <= gameObjectIndex || placedGameObject[gameObjectIndex] == null) 
			return;

		Destroy(placedGameObject[gameObjectIndex]);
		placedGameObject[gameObjectIndex] = null;
	}
}
