using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class variant : MonoBehaviour
{
    public Material[] variants;
    int currentIndex;

    public void Change()
    {
        if (variants.Length == 0)
            return;
        if(currentIndex + 1 > variants.Length - 1)
        {
            GetComponent<MeshRenderer>().material = variants[0];
            currentIndex = 0;
        }
        else
        {
            GetComponent<MeshRenderer>().material = variants[currentIndex + 1];
            currentIndex++;
        }
    }
}
