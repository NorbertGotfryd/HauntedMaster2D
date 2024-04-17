using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    private const int MAX_ACTIVE_SKILLS = 4;

    [SerializeField] public abstract string GetSkillName();

    protected bool isActive;

    [SerializeField] protected StatusBase skillStatus;
    protected Action onSkillComplete;
    protected CharacterBase character;

    protected SkillElement skillElement;

    protected enum SkillElement
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

    public abstract void UseSkill(CharacterBase targetCharacter, Action onMakeAction, Action onActionComplete);
}
