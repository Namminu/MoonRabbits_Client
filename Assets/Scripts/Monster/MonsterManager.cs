using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    private static MonsterManager _instance;
    public static MonsterManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("MonsterManager");
                _instance = go.AddComponent<MonsterManager>();
            }
            return _instance;
        }
    }


    private List<MonsterController> monsterList = new();

    public void AddMonster(MonsterController mc)
    {
        monsterList.Add(mc);
    }

    public void SendPositionPacket(int id, Vector3 position)
    {
        var monster = monsterList.Find(x => x.ID == id);
        if (monster == null) return;
        monster.SetPosition(position);

    }

    public void RemoveMonster(int id)
    {
        var monster = monsterList.Find(x => x.ID == id);
        if (monster == null) return;
        monsterList.Remove(monster);
        Destroy(monster);
    }

    public void RemoveMonsterAll()
    {
        for (int i = 0; i < monsterList.Count; i++)
        {
            Destroy(monsterList[i].gameObject);
        }
        monsterList.Clear();
    }

    public MonsterController GetMonster(int id)
    {
        return monsterList.Find(x => x.ID == id);
    }

    public List<MonsterController> GetMonsterAll()
    {
        return monsterList;
    }
}
