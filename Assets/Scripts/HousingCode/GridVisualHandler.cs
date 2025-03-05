using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualHandler : MonoBehaviour
{
    public static event Action<bool> OnGridVisualizationState;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    void OnEnable() => OnGridVisualizationState?.Invoke(true);

	private void OnDisable() => OnGridVisualizationState?.Invoke(false);
}
