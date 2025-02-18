using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIEntrance : MonoBehaviour
{
    [SerializeField] private Text LoadSceneText;
    [SerializeField] private Button btn_Yes;
	[SerializeField] private Button btn_No;
	[SerializeField] private Button btn_Close;

    private string nextSceneName;
    private string sceneText;
	// Start is called before the first frame update
	void Start()
    {
        btn_Yes.onClick.AddListener(YesButton);
		btn_No.onClick.AddListener(CloseButton);
		btn_Close.onClick.AddListener(CloseButton);
	}

    private void SetSceneInfo(string sceneName)
    {
        switch(sceneName)
        {
            case "Battle":
                sceneText = "전투지역";
                break;
            case "MyHouse":
				sceneText = "집";
				break;
            case "Town":
				sceneText = "광장";
				break;
            default: break;
        }
		nextSceneName = sceneName;
        LoadSceneText.text = sceneText + "으로 이동하시겠습니까?";
	}

    private void YesButton()
    {
        SceneManager.LoadScene(nextSceneName);
    }
    private void CloseButton()
    {
        Destroy(gameObject);
    }
}
