using System;
using UnityEngine;

public class SkillFire : SkillBase
{
    public override void UseSkill(Action onActionComplete)
    {
        ExecuteSkill(onActionComplete);
    }

    public override string GetSkillName() => "SkillFire";
}
