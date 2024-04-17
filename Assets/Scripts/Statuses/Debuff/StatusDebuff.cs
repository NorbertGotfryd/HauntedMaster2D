using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusDebuff : StatusBase
{
    public void DebuffTest()
    {
        Debug.Log("Debuff on target " + BattleHandler.instance.GetTargetCharacter());
    }
}
