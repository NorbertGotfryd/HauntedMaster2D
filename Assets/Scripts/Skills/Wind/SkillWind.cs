using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillWind : SkillBase
{
    public override string GetSkillName() => "Skill Wind";

    public override void UseSkill(CharacterBase targetCharacter, Action onMakeAction, Action onActionComplete)
    {
        Debug.Log(GetSkillName());
    }
}
