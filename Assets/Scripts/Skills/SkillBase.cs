using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    [SerializeField] protected int attackAmount;

    protected CharacterBase character;
    protected Action onSkillComplete;
    protected CharacterBase defenderCharacter;

    [SerializeField] protected SkillElement skillElement;

    [SerializeField] public abstract string GetSkillName();


    public enum SkillElement
    {
        Neutral,
        Fire,
        Earth,
        Water,
        Wind
    }

    private void Awake()
    {
        character = GetComponent<CharacterBase>();
    }

    public abstract void UseSkill(Action action);//,Action onActionComplete);
    public SkillElement GetSkillElement() => skillElement;
    public int GetAttackAmount() => attackAmount;
}
