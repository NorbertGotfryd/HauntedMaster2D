using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    [SerializeField] protected int attackAmount;
    [SerializeField] protected int healingAmount;

    protected CharacterBase character;
    protected Action onSkillComplete;
    protected CharacterBase defenderCharacter;

    [SerializeField] protected SkillElement skillElement;
    [SerializeField] protected SkillType skillType;

    public enum SkillElement
    {
        Neutral,
        Fire,
        Earth,
        Water,
        Wind
    }

    public enum SkillType
    {
        AttackingSkill,
        HealingSkill,
        AttackingAoe,
        BuffSkill,
        HealingAoe, // Added HealingAoe
        SelfBuff    // Added SelfBuff
    }

    public void ExecuteSkill(Action onActionComplete)
    {
        switch (skillType)
        {
            case SkillType.AttackingSkill:
                ExecuteAttackingSkill(onActionComplete);
                break;
            case SkillType.HealingSkill:
                ExecuteHealingSkill(onActionComplete);
                break;
            case SkillType.BuffSkill:
                ExecuteBuffSkill(onActionComplete);
                break;
            case SkillType.AttackingAoe:
                ExecuteAttackingAoeSkill(onActionComplete);
                break;
            case SkillType.HealingAoe:
                ExecuteHealingAoeSkill(onActionComplete);
                break;
            case SkillType.SelfBuff:
                ExecuteSelfBuffSkill(onActionComplete);
                break;
            default:
                Debug.LogError("Unsupported skill type!");
                break;
        }
    }

    private void ExecuteAttackingSkill(Action onActionComplete)
    {
        Debug.Log($"Using attacking skill: {GetSkillName()}");

        BattleHandler.instance.activeCharacter.CharacterAttack(BattleHandler.instance.targetCharacter.GetCharacterPosition(), () =>
        {
            BattleHandler.instance.activeCharacter.DamageCalculation(BattleHandler.instance.targetCharacter);
        },
        () =>
        {
            if (onActionComplete != null)
                onActionComplete();
        });
    }

    private void ExecuteHealingSkill(Action onActionComplete)
    {
        Debug.Log($"Using healing skill: {GetSkillName()}");

        BattleHandler.instance.activeCharacter.CharacterHealing(BattleHandler.instance.targetCharacter.GetCharacterPosition(), () =>
        {
            BattleHandler.instance.activeCharacter.HealingCalculation(BattleHandler.instance.targetCharacter);
        },
        () =>
        {
            if (onActionComplete != null)
                onActionComplete();
        });
    }

    private void ExecuteAttackingAoeSkill(Action onActionComplete)
    {
        Debug.Log($"Using AOE attacking skill: {GetSkillName()}");

        Vector3 centerPosition = new Vector3(0, 0, 0);

        BattleHandler.instance.activeCharacter.MoveToTargetPosition(centerPosition, () =>
        {
            foreach (CharacterBase enemy in BattleHandler.instance.characterEnemyList)
            {
                BattleHandler.instance.activeCharacter.DamageCalculation(enemy);
            }

            BattleHandler.instance.activeCharacter.BackToStartPosition(() =>
            {
                if (onActionComplete != null)
                    onActionComplete();
            });
        });
    }

    private void ExecuteBuffSkill(Action onActionComplete)
    {
        Debug.Log($"Using buff skill: {GetSkillName()}");

        // Buff status
        if (onActionComplete != null)
            onActionComplete();
    }

    private void ExecuteHealingAoeSkill(Action onActionComplete)
    {
        Debug.Log($"Using AOE healing skill: {GetSkillName()}");

        foreach (CharacterBase player in BattleHandler.instance.characterPlayerList)
        {
            BattleHandler.instance.activeCharacter.HealingCalculation(player);
        }

        if (onActionComplete != null)
            onActionComplete();
    }

    private void ExecuteSelfBuffSkill(Action onActionComplete)
    {
        Debug.Log($"Using self buff skill: {GetSkillName()}");

        // Self buff status
        if (onActionComplete != null)
            onActionComplete();
    }

    private void Awake()
    {
        character = GetComponent<CharacterBase>();
    }

    public abstract void UseSkill(Action action);
    public abstract string GetSkillName();
    public SkillElement GetSkillElement() => skillElement;
    public int GetAttackAmount() => attackAmount;
    public int GetHealingAmount() => healingAmount;
}
