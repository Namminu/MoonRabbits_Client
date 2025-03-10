using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CaptureScreen : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask excludeLayer;

    [Space, SerializeField] private Button btn_Capture;

    private string screenPath;
    private int originCullingMask;

    // Start is called before the first frame update
    void Awake()
    {
        btn_Capture.onClick.AddListener(OnCaptureButton);

        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        screenPath = Path.Combine(desktopPath, "Screenshots");

        if(!Directory.Exists(screenPath))
        {
            Directory.CreateDirectory(screenPath);
        }
	}

    private void OnCaptureButton()
    {
        Debug.Log("Capture!");
        StartCoroutine(CaptureScreenShot());
	}
    
    private IEnumerator CaptureScreenShot()
    {
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;

        /* expection exclude Layer on Camera */
        originCullingMask = mainCamera.cullingMask;
        mainCamera.cullingMask = ~excludeLayer;

        /* Capturing */
        RenderTexture rt = new RenderTexture(width, height, 24);
        mainCamera.targetTexture = rt;
        mainCamera.Render();

        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        /* Restore All ScreenShot Settings */
        mainCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        mainCamera.cullingMask = originCullingMask;

        /* Save Data to Local Path */
        byte[] bytes = screenshot.EncodeToPNG();
        string filename = $"ProjectMR_Screenshot_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png"; 
        string fullPath = Path.Combine(screenPath, filename);
        File.WriteAllBytes(fullPath, bytes);

        Debug.Log("S_Shot Save Path : " + fullPath);
 	}
}
