using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorTester : MonoBehaviour
{
    [SerializeField] private Transform[] animators;
    
    void Start()
    {
        int xCount = 6;
        
        float width = 15;
        float half = width / 2;
        float space = width / xCount;
        
        for (int i = 0; i < animators.Length; i++)
        {
            float posX = (i % xCount) * space - (half / 2);
            float posZ = i / xCount * space - (half / 2);
            
            animators[i].position = new Vector3(posX, 0, posZ);
            animators[i].GetComponentInChildren<Canvas>()?.gameObject.SetActive(false);
        }
    }

    public void SetAnim(int code)
    {
        foreach (var animator in animators)
            animator.GetComponent<Monster>()?.SetAnim(code);
    }
}
