using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public int sectorCode;

    public enum GateType
    {
        prev = 1,
        next = 2,
    }

    public GateType type;
}
