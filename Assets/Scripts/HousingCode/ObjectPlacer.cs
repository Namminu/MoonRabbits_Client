using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private List<GameObject> placedGameObject = new();

    // 재료 소모 로직: 인벤토리 체크 및 재료 차감(InventoryManager와 연동 필요)
    public bool ConsumeMaterials(int craftItemId)
    {
        FurnitureRecipe recipe = RecipeManager.instance.GetRecipeByCraftItemId(craftItemId);
        if (recipe == null)
        {
            Debug.LogError("해당 가구 레시피를 찾을 수 없습니다. craft_item_id: " + craftItemId);
            return false;
        }

        // 예시: InventoryManager를 통해 플레이어 인벤토리에서 재료 수량을 확인
        foreach (RecipeMaterialItem material in recipe.material_items)
        {
            if (!InventoryManager.instance.HasItem(material.item_id, material.count))
            {
                Debug.Log("재료 부족: item_id " + material.item_id);
                return false;
            }
        }

        // 재료가 모두 충분할 경우 소비 처리
        foreach (RecipeMaterialItem material in recipe.material_items)
        {
            InventoryManager.instance.RemoveItem(material.item_id, material.count);
            InventoryManager.instance.UpdateInventoryUI();
        }
        return true;
    }

    // 가구 오브젝트를 배치하는 메서드
    public int PlaceObject(GameObject prefab, Vector3 position, float yRotation, int craftItemId, bool isLoading = false)
    {
        // 재료 소모 확인 후 실패하면 배치 중단
        // 로드 모드가 아닌 경우에만 재료 소모 로직 실행
        if (!isLoading)
        {
            if (!ConsumeMaterials(craftItemId))
            {
                Debug.Log("가구 설치 실패: 재료 소모 실패");
                return -1;
            }
        }
        // 재료 소모 성공 시 가구 인스턴스 생성
        GameObject newObject = Instantiate(prefab, position, Quaternion.Euler(0, yRotation, 0));
        placedGameObject.Add(newObject);
        return placedGameObject.Count - 1;
    }

    // 배치된 오브젝트를 제거하는 메서드
    public void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObject.Count <= gameObjectIndex || placedGameObject[gameObjectIndex] == null)
            return;
        Destroy(placedGameObject[gameObjectIndex]);
        placedGameObject[gameObjectIndex] = null;
        //placedGameObject.RemoveAt(gameObjectIndex);
	}

    #region 이전 코드
    /*public int PlaceObject(GameObject prefab, Vector3 position, float yRotation)
	{
		GameObject newObject = Instantiate(prefab);
		newObject.transform.position = position;
		newObject.transform.rotation = Quaternion.Euler(0, yRotation, 0); ;
		placedGameObject.Add(newObject);

		return placedGameObject.Count - 1;
	}

	internal void RemoveObjectAt(int gameObjectIndex)
	{
		if (placedGameObject.Count <= gameObjectIndex || placedGameObject[gameObjectIndex] == null) 
			return;

		Destroy(placedGameObject[gameObjectIndex]);
		placedGameObject[gameObjectIndex] = null;
	}*/
     #endregion
}
