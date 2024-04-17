using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillFire : SkillBase
{
    public override string GetSkillName() => "Skill Fire";

    public override void UseSkill(CharacterBase targetCharacter, Action onMakeAction, Action onActionComplete)
    {
        Debug.Log(GetSkillName());
    }
}
