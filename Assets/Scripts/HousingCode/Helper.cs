using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper
{
	public static ObjectTransInfo ChangeDataToTransInfo(Vector3Int position, float rotate)
	{
		return new ObjectTransInfo(position, rotate);
	}
	public static Vector3Int VectorDataToInt(Vector3 vector)
	{
		return new Vector3Int((int)vector.x, (int)vector.y, (int)vector.z);
	}


}