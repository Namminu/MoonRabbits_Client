using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rune : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(40 * Time.deltaTime * Vector3.up);
    }
}
