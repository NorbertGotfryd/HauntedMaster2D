using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    private const int MAX_ACTIVE_SKILLS = 4;

    protected bool isActive;

    [SerializeField] public abstract string GetSkillName();
    [SerializeField] protected abstract int GetSkillPower();
    
    protected SkillElement skillElement;

    protected StatusBase skillStatus;

    protected enum SkillElement
    {
        Neutral,
        Fire,
        Earth,
        Water,
        Wind
    }
}
