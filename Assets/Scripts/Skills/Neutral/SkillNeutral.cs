using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillNeutral : SkillBase
{
    public override string GetSkillName() => "Skill Neutral";

    public override void UseSkill(CharacterBase targetCharacter, Action onMakeAction, Action onActionComplete)
    {
        Debug.Log(GetSkillName());
    }
}
