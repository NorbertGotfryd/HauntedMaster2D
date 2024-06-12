using System;
using UnityEngine;

public class SkillEarth : SkillBase
{
    public override void UseSkill(Action onActionComplete)
    {
        ExecuteSkill(onActionComplete);
    }

    public override string GetSkillName() => "SkillEarth";
}
