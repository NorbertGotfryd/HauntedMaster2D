using System;
using UnityEngine;

public class SkillWater : SkillBase
{
    public override void UseSkill(Action onActionComplete)
    {
        ExecuteSkill(() =>
        {
            BattleHandler.instance.activeCharacter.HealingCalculation(BattleHandler.instance.targetCharacter);
            ApplyStatusEffect(BattleHandler.instance.targetCharacter);
            onActionComplete?.Invoke();
        });
    }

    public override string GetSkillName() => "SkillWater";
}
