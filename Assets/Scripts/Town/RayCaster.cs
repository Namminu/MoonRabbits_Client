// // using UnityEngine;

// public class RayCaster : MonoBehaviour
// {
//     private IMouseHoverable lastHoverObject;
//     // Start is called before the first frame update
//     void Start()
//     {
//         lastHoverObject = null;
//     }

// //     // Update is called once per frame
// //     void Update()
// //     {
// //         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
// //         RaycastHit hit;

//         if (Physics.Raycast(ray, out hit))
//         {
//             IMouseHoverable hoveredObject = hit.collider.GetComponent<IMouseHoverable>();

//             if (hoveredObject != lastHoverObject)
//             {
// 				/* 이전 오브젝트에서 벗어났다면 Exit 호출 */
// 				lastHoverObject?.OnMouseHoverExit();
// 				/* 새로 감지한 오브젝트의 Enter 호출 */
// 				hoveredObject?.OnMouseHoverEnter();
// 				/* 새롭게 감지된 오브젝트 저장 */
// 				lastHoverObject = hoveredObject;
//             }

//             if (Input.GetMouseButtonDown(0))
//             {
//                 hoveredObject?.OnMouseClicked();
//             }
//         }
//         else
//         {
// 			/* 마우스가 아무 오브젝트에도 닿지 않으면 Exit 호출 */
// 			if (lastHoverObject != null)
//             {
//                 lastHoverObject.OnMouseHoverExit();
//                 lastHoverObject = null;

//             }
//         }
//     }
// }
