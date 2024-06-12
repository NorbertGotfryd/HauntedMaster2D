using System;
using UnityEngine;

public class SkillWind : SkillBase
{
    public override void UseSkill(Action onActionComplete)
    {
        ExecuteSkill(onActionComplete);
    }

    public override string GetSkillName() => "SkillWind";
}
