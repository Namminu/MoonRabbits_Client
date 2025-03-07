using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Running : MonoBehaviour
{
    public Slider staminaSlider; // ���׹̳� UI �����̴�
    public float maxStamina = 100f; // �ִ� ���׹̳�
    public float staminaDrainRate = 10f; // �ʴ� ���׹̳� ���ҷ�
    public float staminaRegenRate = 5f; // �ʴ� ���׹̳� ȸ����
    public float regenDelay = 2f; // ȸ�� ���� �� ��� �ð�
    public float normalSpeed = 5f; // �Ϲ� �ӵ�
    public float sprintSpeed = 10f; // �޸��� �ӵ�
    public float exhaustedSpeed = 2f; // ���׹̳� ���� �� �ӵ�

    private float currentStamina; // ���� ���׹̳� ��
    private bool isSprinting = false; // �޸��� ����
    private bool isRegenerating = false; // ȸ�� �� ����
    private float speed; // ���� �ӵ�

    private void Start()
    {
        currentStamina = maxStamina; // �ʱ� ���׹̳��� �ִ�ġ�� ����
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
                speed = exhaustedSpeed; // ���׹̳� ���� �� �ӵ� ����
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
