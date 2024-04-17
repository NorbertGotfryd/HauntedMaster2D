using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillWater : SkillBase
{
    public override string GetSkillName() => "Skill Water";

    public override void UseSkill(CharacterBase targetCharacter, Action onMakeAction, Action onActionComplete)
    {
        Debug.Log(GetSkillName());
    }
}
