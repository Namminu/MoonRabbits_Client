using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiTest : MonoBehaviour
{
    private int player_level;
    private int player_exp;
    private int player_targetExp;
    private int player_stat1;
    private int player_stat2;
    private int player_speed;
    private int player_hp;
    private int player_maxHp;

    public Button btnAddExp;
    public Slider hpSlider;
    public Slider expSlider;
    public TextMeshProUGUI hpText;

    // Start is called before the first frame update
    void Start()
    {
        player_level = 1;
        player_maxHp = 100;
        player_hp = player_maxHp;
        player_exp = 0;
        player_targetExp = 10;
        player_stat1 = 100;
        player_stat2 = 5;
        player_speed = 10;

        btnAddExp.onClick.AddListener(OnClickAddExp);
        hpSlider.value = player_hp / player_maxHp;
        expSlider.value = player_exp / player_targetExp;
        hpText.text = $"{player_hp} / {player_maxHp}";
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnClickAddExp()
    {
        var addExpPacket = new C2SChat
        {
            PlayerId = 1,
            SenderName = "nn",
            ChatMsg = "경험치 올리고싶어",
        };

        GameManager.Network.Send(addExpPacket);
    }

    public void SetExp(int updatedExp)
    {
        player_exp = updatedExp;
        if (player_exp > player_targetExp)
        {
            Debug.Log("WTF! exp exceeded requirement for lv up");
            return;
        }
        Debug.Log($"updatedExp : {updatedExp}");

        expSlider.value = player_exp / player_targetExp;
    }

    public void LevelUp()
    {
        player_level++;
    }

}
