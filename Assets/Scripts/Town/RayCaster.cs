using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCaster : MonoBehaviour
{
    // private IMouseHoverable lastHoverObject;
    // // Start is called before the first frame update
    // void Start()
    // {
    //     lastHoverObject = null;
    // }

    // // Update is called once per frame
    // void Update()
    // {
    //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //     RaycastHit hit;

    //     if(Physics.Raycast(ray, out hit))
    //     {
    //         IMouseHoverable hoveredObject = hit.collider.GetComponent<IMouseHoverable>();

    //         if(hoveredObject != lastHoverObject) 
    //         {
    //             /* ���� ������Ʈ���� ����ٸ� Exit ȣ�� */
    //             lastHoverObject?.OnMouseHoverExit();
    // 			/* ���� ������ ������Ʈ�� Enter ȣ�� */
    // 			hoveredObject?.OnMouseHoverEnter();
    // 			/* ���Ӱ� ������ ������Ʈ ���� */
    // 			lastHoverObject = hoveredObject;
    //         }

    //         if(Input.GetMouseButtonDown(0))
    //         {
    // 			hoveredObject?.OnMouseClicked();
    //         }
    //     }
    //     else
    //     {
    //         /* ���콺�� �ƹ� ������Ʈ���� ���� ������ Exit ȣ�� */
    //         if(lastHoverObject != null) 
    //         {
    //             lastHoverObject.OnMouseHoverExit();
    //             lastHoverObject = null;

    // 		}
    //     }
    // }
}
