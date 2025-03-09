using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingState : IBuildingState
{
	private int gameObjectIndex = -1;
	Grid grid;
	PreviewSystem previewSystem;
	GridData floorData;
	GridData furnitureData;
	ObjectPlacer objectPlacer;

	public RemovingState(Grid grid,
					  PreviewSystem previewSystem,
					  GridData floorData,
					  GridData furnitureData,
					  ObjectPlacer objectPlacer)
	{
		this.grid = grid;
		this.previewSystem = previewSystem;
		this.floorData = floorData;
		this.furnitureData = furnitureData;
		this.objectPlacer = objectPlacer;

		previewSystem.StartShowingRemovePreview();
	}

	public void EndState()
	{
		previewSystem.StopShowingPreview();
	}

	public void OnAction(ObjectTransInfo gridInfo)
	{
		GridData selectedData = null;
		if(!furnitureData.CanPlaceObjectAt(gridInfo, Vector2Int.one))
		{
			selectedData = furnitureData;
		}
		else if(!floorData.CanPlaceObjectAt(gridInfo, Vector2Int.one))
		{
			selectedData = floorData;
		}

        if (selectedData == null)
        {
            // Sound?
        }
		else
		{
			gameObjectIndex = selectedData.GetRepresentationIndex(gridInfo.ObjectPosition);
			if (gameObjectIndex == -1) return;

			selectedData.RemoveObjectAt(gridInfo.ObjectPosition);
			objectPlacer.RemoveObjectAt(gameObjectIndex);
		}
		Vector3 cellPosition = grid.CellToWorld(gridInfo.ObjectPosition);
		ObjectTransInfo objectInfo = Helper.ChangeDataToTransInfo(
			Helper.VectorDataToInt(cellPosition), gridInfo.ObjectYRotation);

		previewSystem.UpdatePosition(objectInfo, CheckIfSelectionIsValid(gridInfo));
    }

	private bool CheckIfSelectionIsValid(ObjectTransInfo gridInfo)
	{
		return !(furnitureData.CanPlaceObjectAt(gridInfo, Vector2Int.one) &&
			floorData.CanPlaceObjectAt(gridInfo, Vector2Int.one));
	}

	public void UpdateState(ObjectTransInfo gridInfo)
	{
		bool validity = CheckIfSelectionIsValid(gridInfo);
		Vector3 cellPosition = grid.CellToWorld(gridInfo.ObjectPosition);
		ObjectTransInfo objectInfo = Helper.ChangeDataToTransInfo(
			Helper.VectorDataToInt(cellPosition), gridInfo.ObjectYRotation);

		previewSystem.UpdatePosition(objectInfo, validity);
	}
}
