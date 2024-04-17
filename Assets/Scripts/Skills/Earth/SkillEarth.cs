using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEarth : SkillBase
{
    public override string GetSkillName() => "Skill Earth";

    public override void UseSkill(CharacterBase targetCharacter, Action onMakeAction, Action onActionComplete)
    {
        Debug.Log(GetSkillName());
    }
}
