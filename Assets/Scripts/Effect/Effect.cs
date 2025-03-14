using System.Collections;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public bool isTemporary = true; // 이펙트가 일시적인지 여부
    public float duration = 2f; // 일시적인 이펙트의 지속 시간

    private ParticleSystem[] _particleSystems;

    // 초기화 메서드
    public void Initialize(bool temporary, float durationValue)
    {
        isTemporary = temporary;
        duration = durationValue;

        // 하위 모든 파티클 시스템을 가져옵니다.
        _particleSystems = GetComponentsInChildren<ParticleSystem>();

        if (isTemporary)
        {
            // 일시적인 이펙트인 경우 지정된 시간 후에 제거
            Destroy(gameObject, duration);
        }
        else
        {
            // 지속적인 이펙트인 경우, 파티클 시스템이 종료될 때까지 대기
            StartCoroutine(CheckIfFinished());
        }
    }

    private IEnumerator CheckIfFinished()
    {
        // 모든 파티클 시스템이 종료될 때까지 대기
        while (true)
        {
            bool allStopped = true;
            foreach (var particleSystem in _particleSystems)
            {
                if (particleSystem.isPlaying)
                {
                    allStopped = false;
                    break;
                }
            }

            if (allStopped)
            {
                break; // 모든 파티클 시스템이 종료되면 루프 종료
            }

            yield return null; // 다음 프레임까지 대기
        }

        // 모든 파티클이 종료된 후 제거
        Destroy(gameObject);
    }
}
