using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCameraInHouse : MonoBehaviour
{
	[SerializeField, Range(1f, 10f)] private float moveSpeed = 5f;
	[SerializeField, Range(1f, 10f)] private float zoomSpeed = 5f;
	[SerializeField, Range(1f, 10f)] private float rotateSpeed = 5f;

	private Vector3 targetPosition;
	private bool canMoveState;
	private bool isRotate;
	private Quaternion targetRotation;

	private void Awake()
	{
		targetPosition = transform.position;
		targetRotation = transform.rotation;
		isRotate = false;
	}

	void Update()
	{
		if (!canMoveState) return;

		MoveCamera();
		UpdownCamera();
		RotateCamera();

		transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5f);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
	}

	#region Private Method

	/// <summary>
	/// KeyBoard Input to Camera Position Move Action
	/// </summary>
	private void MoveCamera()
	{
		Vector3 moveDirection = Vector3.zero;

		if (Input.GetKey(KeyCode.W)) moveDirection += transform.forward;
		if (Input.GetKey(KeyCode.S)) moveDirection -= transform.forward;
		if (Input.GetKey(KeyCode.D)) moveDirection += transform.right;
		if (Input.GetKey(KeyCode.A)) moveDirection -= transform.right;

		moveDirection.y = 0f;
		targetPosition += moveSpeed * Time.deltaTime * moveDirection.normalized;
	}

	/// <summary>
	/// Mouse Scroll to Camera Height Move Action
	/// </summary>
	private void UpdownCamera()
	{
		float scrollInput = Input.GetAxis("Mouse ScrollWheel");
		if (scrollInput != 0)
		{
			targetPosition.y += -scrollInput * zoomSpeed;
		}
	}

	/// <summary>
	/// Mouse Right Click Hold to Camera Rotation Move Action
	/// </summary>
	private void RotateCamera()
	{
		if(Input.GetMouseButtonDown(1))
		{
			isRotate = true;
			Cursor.lockState = CursorLockMode.Locked;
		}
		if (Input.GetMouseButtonUp(1))
		{
			isRotate = false;
			Cursor.lockState = CursorLockMode.None;
		}

		if (!isRotate) return;

		float mouseX = Input.GetAxis("Mouse X") * rotateSpeed * 15f * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed * 15f * Time.deltaTime;

		Quaternion horizontalRotation = Quaternion.Euler(0, mouseX, 0);
		Quaternion verticalRotation = Quaternion.Euler(-mouseY, 0, 0);

		targetRotation = horizontalRotation * targetRotation;
		targetRotation = targetRotation * verticalRotation;
	}

	private void OnEnable()
	{
		GridVisualHandler.OnGridVisualizationState += UpdateMoveState;
	}

	private void OnDisable()
	{
		GridVisualHandler.OnGridVisualizationState -= UpdateMoveState;
	}

	private void UpdateMoveState(bool isActive) => canMoveState = !isActive;

	#endregion
}
