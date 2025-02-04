using System;
using UnityEngine;

public class Avatar : MonoBehaviour
{
    public Transform[] partsRoot;

    public void Start()
    {
        
    }

    public void UpdateTexture(CustomizeItem item)
    {
        
    }

    public void UpdateItem(CustomizeItem item)
    {
        
    }
    
    private void DisableFromRoot(Transform root)
    {
        for (int i = 0; i < root.childCount; i++)
            root.GetChild(i).gameObject.SetActive(false);
    }

}