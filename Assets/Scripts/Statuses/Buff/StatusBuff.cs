using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusBuff : StatusBase
{
    public void BuffTest()
    {
        Debug.Log("Buff on target " + BattleHandler.instance.GetTargetCharacter());
    }
}
