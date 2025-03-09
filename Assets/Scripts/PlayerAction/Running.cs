using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Running : MonoBehaviour
{
    public Slider staminaSlider; // 스테미나 UI 슬라이더
    public float maxStamina = 100f; // 최대 스테미나
    public float staminaDrainRate = 10f; // 초당 스테미나 감소량
    public float staminaRegenRate = 5f; // 초당 스테미나 회복량
    public float regenDelay = 2f; // 회복 시작 전 대기 시간
    public float normalSpeed = 5f; // 일반 속도
    public float sprintSpeed = 10f; // 달리기 속도
    public float exhaustedSpeed = 2f; // 스테미나 소진 시 속도

    private float currentStamina; // 현재 스테미나 값
    private bool isSprinting = false; // 달리기 여부
    private bool isRegenerating = false; // 회복 중 여부
    private float speed; // 현재 속도

    private void Start()
    {
        currentStamina = maxStamina; // 초기 스테미나는 최대치로 설정
        speed = normalSpeed;
        UpdateStaminaUI();
    }

    private void Update()
    {
        HandleSprint();
        MovePlayer();
        RegenerateStamina();
    }

    private void HandleSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0)
        {
            isSprinting = true;
            isRegenerating = false;
            currentStamina -= staminaDrainRate * Time.deltaTime;
            speed = sprintSpeed;

            if (currentStamina <= 0)
            {
                currentStamina = 0;
                speed = exhaustedSpeed; // 스테미나 소진 시 속도 감소
            }
        }
        else
        {
            isSprinting = false;
            speed = normalSpeed;

            if (!isRegenerating)
            {
                StartCoroutine(StartRegenAfterDelay());
            }
        }

        UpdateStaminaUI();
    }

    private void RegenerateStamina()
    {
        if (isRegenerating && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;

            if (currentStamina >= maxStamina)
            {
                currentStamina = maxStamina;
                isRegenerating = false;
            }

            UpdateStaminaUI();
        }
    }

    private IEnumerator StartRegenAfterDelay()
    {
        yield return new WaitForSeconds(regenDelay);
        isRegenerating = true;
    }

    private void MovePlayer()
    {

        /*Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical) * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);*/
    }

    private void UpdateStaminaUI()
    {
        staminaSlider.value = currentStamina / maxStamina;
    }
}
