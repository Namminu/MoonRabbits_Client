using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCamera : MonoBehaviour
{
    public Transform target;
    private Vector3 offset = new(3.5f, 9.5f, 3.5f);

    private void Update()
    {
        if (target != null)
            transform.position = target.position + offset;
    }
}
