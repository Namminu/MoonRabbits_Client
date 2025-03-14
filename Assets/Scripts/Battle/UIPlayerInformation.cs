using System;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInformation : MonoBehaviour
{
    [SerializeField]
    private TMP_Text txtLevel;

    [SerializeField]
    private TMP_Text txtName;

    [SerializeField]
    private TMP_Text txtHp;

    [SerializeField]
    private Image imgHpFill;

    [SerializeField]
    private TMP_Text txtMp;

    [SerializeField]
    private Image imgMpFill;

    private float maxHP;
    private float currentHP;

    private float maxMP;
    private float currentMP;

    private const float FillWidth = 634f;
    private const float FillHeight = 40f;

    // public void Configure(PlayerStatus playerStatus)
    // {
    //     Debug.Log(playerStatus);

    //     SetName(playerStatus.PlayerName);
    //     SetLevel(playerStatus.PlayerLevel);
    //     SetMaxHP(playerStatus.PlayerFullHp);
    //     SetMaxMP(playerStatus.PlayerFullMp);
    //     UpdateHP(playerStatus.PlayerCurHp);
    //     UpdateMP(playerStatus.PlayerCurMp);
    // }

    public void SetName(string name)
    {
        txtName.text = name;
    }

    public void SetLevel(int level)
    {
        txtLevel.text = $"Lv.{level}";
    }

    public void SetMaxHP(float hp, bool recover = true)
    {
        maxHP = hp;
        txtHp.text = hp.ToString("0");

        if (recover)
        {
            UpdateHP(hp);
        }

        ResizeText(txtHp);
    }

    public void UpdateHP(float hp)
    {
        currentHP = Mathf.Clamp(hp, 0, maxHP);
        txtHp.text = currentHP.ToString("0");

        float fillPercentage = currentHP / maxHP;
        ResizeFill(imgHpFill, fillPercentage);

        ResizeText(txtHp);
    }

    public void SetMaxMP(float mp, bool recover = true)
    {
        maxMP = mp;
        txtMp.text = mp.ToString("0");

        if (recover)
        {
            UpdateMP(mp);
        }

        ResizeText(txtMp);
    }

    public void UpdateMP(float mp)
    {
        currentMP = Mathf.Clamp(mp, 0, maxMP);
        txtMp.text = currentMP.ToString("0");

        float fillPercentage = currentMP / maxMP;
        ResizeFill(imgMpFill, fillPercentage);

        ResizeText(txtMp);
    }

    private void ResizeFill(Image fillImage, float percentage)
    {
        fillImage.rectTransform.sizeDelta = new Vector2(FillWidth * percentage, FillHeight);
    }

    private void ResizeText(TMP_Text textComponent)
    {
        textComponent.rectTransform.sizeDelta = new Vector2(
            textComponent.preferredWidth + 50,
            FillHeight
        );
    }
}
