using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private static BattleManager _instance = null;
    public static BattleManager Instance => _instance;

    [SerializeField] private UIScreen uiScreen;
    [SerializeField] private UIBattleLog uiBattleLog;
    [SerializeField] private UIPlayerInformation uiPlayerInformation;

    public UIScreen UiScreen => uiScreen;
    public UIBattleLog UiBattleLog => uiBattleLog;
    public UIPlayerInformation UiPlayerInformation => uiPlayerInformation;

    [SerializeField] private Maps map;

    [SerializeField] private Transform[] players;
    private Animator playerAnimator;

    private readonly Dictionary<int, string> monsterDb = new Dictionary<int, string>();

    [SerializeField] private Transform[] monsterSpawnPos;
    private readonly List<Monster> monsterObjs = new List<Monster>();

    private readonly List<UIMonsterInformation> monsterUis = new List<UIMonsterInformation>();

    private const string BaseMonsterPath = "Monster/Monster1";

    private static readonly int[] AnimCodeList =
    {
        Constants.PlayerBattleAttack1,
        Constants.PlayerBattleDie,
        Constants.PlayerBattleHit
    };

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeMonsterDatabase();
        ConfigureGame(GameManager.Instance.Pkt);
        GameManager.Instance.Pkt = null;
    }

    private void InitializeMonsterDatabase()
    {
        for (int i = 1; i < 30; i++)
        {
            var monsterCode = Constants.MonsterCodeFactor + i;
            var monsterPath = $"Monster/Monster{i}";
            monsterDb.Add(monsterCode, monsterPath);
        }
    }

    public void ConfigureGame(S_EnterDungeon pkt)
    {
        Debug.Log("Entering Dungeon");

        if (pkt.DungeonInfo != null)
        {
            ConfigureDungeon(pkt.DungeonInfo);
        }

        if (pkt.Player != null)
        {
            uiPlayerInformation.Configure(pkt.Player);
            ConfigurePlayer(pkt.Player.PlayerClass);
        }

        if (pkt.ScreenText != null)
        {
            uiScreen.Display(pkt.ScreenText);
        }

        if (pkt.BattleLog != null)
        {
            uiBattleLog.Initialize(pkt.BattleLog);
        }
    }

    private void ConfigurePlayer(int classCode)
    {
        int idx = classCode - Constants.PlayerCodeFactor;
        for (int i = 0; i < players.Length; i++)
        {
            bool isActive = i == idx;
            players[i].gameObject.SetActive(isActive);

            if (isActive)
            {
                playerAnimator = players[i].GetComponent<Animator>();
            }
        }
    }

    public void ConfigureDungeon(DungeonInfo dungeonInfo)
    {
        ConfigureMap(dungeonInfo.DungeonCode);
        ConfigureMonsters(dungeonInfo.Monsters);
    }

    private void ResetMonsters()
    {
        foreach (var monster in monsterObjs)
        {
            if (monster != null)
            {
                Destroy(monster.gameObject);
            }
        }

        monsterObjs.Clear();
        monsterUis.Clear();
    }

    public void ConfigureMonsters(RepeatedField<MonsterStatus> monsters)
    {
        ResetMonsters();
        for (int i = 0; i < monsters.Count; i++)
        {
            var monsterInfo = monsters[i];
            var monsterCode = monsterInfo.MonsterModel;
            var monsterPath = monsterDb.GetValueOrDefault(monsterCode, BaseMonsterPath);
            var monsterRes = Resources.Load<Monster>(monsterPath);
            var monster = Instantiate(monsterRes, monsterSpawnPos[i]);

            monsterObjs.Add(monster);
            monsterUis.Add(monster.UiMonsterInfo);

            monster.UiMonsterInfo.SetName(monsterInfo.MonsterName);
            monster.UiMonsterInfo.SetFullHP(monsterInfo.MonsterHp);
        }
    }

    public void UpdateMonsterHp(int idx, float hp)
    {
        if (idx < 0 || idx >= monsterUis.Count)
        {
            return;
        }

        monsterUis[idx].SetCurHP(hp);
    }

    public Monster GetMonster(int idx)
    {
        return idx >= 0 && idx < monsterObjs.Count ? monsterObjs[idx] : null;
    }

    public void TriggerPlayerHitAnimation()
    {
        TriggerAnimation(Constants.PlayerBattleHit);
    }

    public void TriggerPlayerAnimation(int idx)
    {
        if (idx < 0 || idx >= AnimCodeList.Length)
        {
            return;
        }

        TriggerAnimation(AnimCodeList[idx]);
    }

    private void TriggerAnimation(int animCode)
    {
        playerAnimator.transform.localEulerAngles = Vector3.zero;
        playerAnimator.transform.localPosition = Vector3.zero;
        playerAnimator.applyRootMotion = animCode == Constants.PlayerBattleDie;
        playerAnimator.SetTrigger(animCode);
    }

    public void ConfigureMap(int mapId)
    {
        map.SetMap(mapId);
    }
}
