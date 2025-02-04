using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private static EffectManager _instance = null;
    public static EffectManager Instance => _instance;
    
    [SerializeField] private GameObject[] effects;
    [SerializeField] private Transform playerPos; 
    
    int fullSkillIdx = 23;
    
    void Awake()
    {
        _instance = this;
    }

    public void SetEffectToPlayer(int code)
    {
        SetEffect(playerPos, code);
    }

    public void SetEffectToMonster(int monsterIdx, int code)
    {
        var monster = BattleManager.Instance.GetMonster(monsterIdx);
        SetEffect(monster.transform, code);
    }
    
    void SetEffect(Transform tr, int code)
    {
        var calcId = code - Constants.EffectCodeFactor;
        
        if(calcId < 0 || calcId >= effects.Length)
            return;
       

        if (calcId < fullSkillIdx)
        {
            var pos = new Vector3(tr.position.x, effects[calcId].transform.position.y, tr.position.z);
            effects[calcId].transform.position = pos;
        }
        
        effects[calcId].gameObject.SetActive(false);
        effects[calcId].gameObject.SetActive(true);
    }
}
