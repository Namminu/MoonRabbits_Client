using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteDataBase ", menuName = "DataBase/Sprite DataBase")]
public class SpriteDataBase : ScriptableObject
{
	[System.Serializable]
	public struct SpriteEntry
	{
		public int itemId;
		public Sprite sprite;
	}

	public List<SpriteEntry> spriteList;
	 
	private Dictionary<int, Sprite> spriteDictionary;

	public void Initialize()
	{
		spriteDictionary = new Dictionary<int, Sprite>();
		foreach(var entry in spriteList)
		{
			if(!spriteDictionary.ContainsKey(entry.itemId))
			{
				spriteDictionary.Add(entry.itemId, entry.sprite);
			}
		}
	}

	public Sprite GetSprite(int itemId)
	{
		if (spriteDictionary == null) Initialize();
		return spriteDictionary.TryGetValue(itemId, out var sprite) ? sprite : null;
	}
}
