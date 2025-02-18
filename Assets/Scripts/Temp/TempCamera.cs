using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 posOffset;

    private void Update()
    {
        transform.position = target.position + posOffset;
        transform.rotation = Quaternion.Euler(50, 0, 0);
    }
}
