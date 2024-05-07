using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillWater : SkillBase
{
    public override void UseSkill(Action onActionComplete)
    {
        Debug.Log($"Using skill: {GetSkillName()}");

        onActionComplete();
    }

    public override string GetSkillName() => "Skill Water";
}
