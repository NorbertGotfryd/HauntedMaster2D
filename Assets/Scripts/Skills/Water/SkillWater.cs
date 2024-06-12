using System;
using UnityEngine;

public class SkillWater : SkillBase
{
    public override void UseSkill(Action onActionComplete)
    {
        ExecuteSkill(onActionComplete);
    }

    public override string GetSkillName() => "SkillWater";
}
