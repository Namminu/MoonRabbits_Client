using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
	[SerializeField] private List<GameObject> placedGameObject = new();

	public int PlaceObject(GameObject prefab, Vector3Int position, float yRotation)
	{
		GameObject newObject = Instantiate(prefab);
		newObject.transform.position = position;
		newObject.transform.rotation = Quaternion.Euler(0, yRotation, 0);
		placedGameObject.Add(newObject);
		Debug.Log("Placer : " + newObject.transform.position + yRotation);

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
