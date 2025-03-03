#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]
public class ObjectDataBaseSo : ScriptableObject
{
	public List<ObjectData> objectDatas;
}

[Serializable]
public class ObjectData
{
	[field: SerializeField] public string Name { get; private set; }
	[field : SerializeField] public int ID { get; private set; }
	[field: SerializeField] public Vector2Int Size { get; private set; } = Vector2Int.one;
	[field : SerializeField] public GameObject Prefab { get; private set; }
}
#endif