using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    private int sectorCode;
    public int id;

    [SerializeField]
    private Transform connectedPortal;
    public Transform ConnectedPortal => connectedPortal;
}
