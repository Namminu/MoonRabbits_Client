using UnityEngine;

public interface IBuildingState
{
	void EndState();
	void OnAction(ObjectTransInfo gridInfo);
	void UpdateState(ObjectTransInfo gridInfo);
}