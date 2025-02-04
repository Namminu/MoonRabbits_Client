using UnityEngine;

public class Constants
{
    public const int PlayerCodeFactor = 1001;
    public const int MonsterCodeFactor = 2000;
    public const int EffectCodeFactor = 3001;
    public const int DungeonCodeFactor = 5000;

    public static readonly int TownPlayerMove = Animator.StringToHash("Move");
    public static readonly int TownPlayerAnim1 = Animator.StringToHash("Anim1");
    public static readonly int TownPlayerAnim2 = Animator.StringToHash("Anim2");
    public static readonly int TownPlayerAnim3 = Animator.StringToHash("Anim3");
    
    public static readonly int PlayerBattleAttack1 = Animator.StringToHash("Attack1");
    public static readonly int PlayerBattleDie = Animator.StringToHash("Die");
    public static readonly int PlayerBattleHit = Animator.StringToHash("Hit");

    public static readonly int MonsterAttack1 = Animator.StringToHash("Attack1");
    public static readonly int MonsterAttack2 = Animator.StringToHash("Attack2");
    public static readonly int MonsterTaunting = Animator.StringToHash("Taunting");
    public static readonly int MonsterVictory = Animator.StringToHash("Victory");
    public static readonly int MonsterDie = Animator.StringToHash("Die");
    public static readonly int MonsterHit = Animator.StringToHash("Hit");
}