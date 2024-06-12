using System;
using UnityEngine;

public class SkillNeutral : SkillBase
{
    public override void UseSkill(Action onActionComplete)
    {
        ExecuteSkill(onActionComplete);
    }

    public override string GetSkillName() => "SkillNeutral";
}
