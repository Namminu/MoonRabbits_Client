using Cinemachine;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private CinemachineFreeLook freelookCam;
    private bool canRotate = false;

    private const string MouseXAxis = "Mouse X";
    private const string MouseYAxis = "Mouse Y";

    void Start()
    {
        freelookCam = GetComponent<CinemachineFreeLook>();
    }

    void Update()
    {
        canRotate = Input.GetMouseButton(1);
        UpdateCameraRotation();
    }

    private void UpdateCameraRotation()
    {
        if (canRotate)
        {
            freelookCam.m_XAxis.m_InputAxisName = MouseXAxis;
            freelookCam.m_YAxis.m_InputAxisName = MouseYAxis;
        }
        else
        {
            ResetCameraInput();
        }
    }

    private void ResetCameraInput()
    {
        freelookCam.m_XAxis.m_InputAxisName = string.Empty;
        freelookCam.m_XAxis.m_InputAxisValue = 0;

        freelookCam.m_YAxis.m_InputAxisName = string.Empty;
        freelookCam.m_YAxis.m_InputAxisValue = 0;
    }
}