using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
	[SerializeField] private List<GameObject> placedGameObject = new();

	public int PlaceObject(GameObject prefab, Vector3 position)
	{
		/* GameObject newObject = Instantiate(ItemDataLoader.HousingItemsList[selectedObjectItemId].ItemPrefab); */
		GameObject newObject = Instantiate(prefab);
		newObject.transform.position = position;
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
