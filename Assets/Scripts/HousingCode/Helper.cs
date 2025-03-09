using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper
{
	/// <summary>
	/// Change Vector3 Data to Vector3Int Data
	/// </summary>
	public static Vector3Int Vector3ToInt(Vector3 _vector)
	{
		Vector3Int newVector = new Vector3Int((int)_vector.x, (int)_vector.y, (int)_vector.z);
		return newVector;
	}
}
