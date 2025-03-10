using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEnter : MonoBehaviour
{
    [SerializeField]
    private Button confirmBtn;

    [SerializeField]
    private GameObject background;

    [SerializeField]
    private TextMeshProUGUI alert;
    private int targetSector;
    private Gate currentGate;

    void Start()
    {
        confirmBtn.onClick.AddListener(() => OnButtonClicked());
    }

    public void IdentifyGate(Gate gate)
    {
        currentGate = gate;
        if (currentGate.type == Gate.GateType.prev)
        {
            targetSector = currentGate.sectorCode - 1;
            alert.text = "이전 섹터로\n이동하시겠습니까?";
        }
        else
        {
            targetSector = currentGate.sectorCode + 1;
            alert.text = "다음 섹터로\n이동하시겠습니까?";
        }
    }

    private void OnButtonClicked()
    {
        if (currentGate == null)
            return;

        var pkt = new C2SMoveSector { TargetSector = targetSector };
        GameManager.Network.Send(pkt);

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}
