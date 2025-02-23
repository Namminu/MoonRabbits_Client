using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemSlotUI))]
public class InvenSlotEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		ItemSlotUI slot = (ItemSlotUI)target;
		if(GUILayout.Button("�׽�Ʈ ������ �߰�"))
		{
			slot.AddTestItem();
			EditorUtility.SetDirty(slot);
		}
	}
}
