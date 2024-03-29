using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillNeutral : SkillBase
{
    public override string GetSkillName() => "SkillNeutral";

    protected override int GetSkillPower() => 5;
}
