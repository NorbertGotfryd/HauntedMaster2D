using System;
using UnityEngine;

public class SkillFire : SkillBase
{
    public override void UseSkill(Action onActionComplete)
    {
        ExecuteSkill(() =>
        {
            BattleHandler.instance.activeCharacter.DamageCalculation(BattleHandler.instance.targetCharacter);
            ApplyStatusEffect(BattleHandler.instance.targetCharacter);
            onActionComplete?.Invoke();
        });
    }

    public override string GetSkillName() => "SkillFire";
}
