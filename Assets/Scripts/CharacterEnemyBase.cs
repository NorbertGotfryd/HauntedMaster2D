using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//klasa wspolnych funkcji dla przeciwników
public abstract class CharacterEnemyBase : CharacterBase
{
    /*
    private CharacterBase activeCharacter;
    private CharacterBase targetCharacter;

    private void Awake()
    {
        BattleHandler.instance.activeCharacter = activeCharacter;
    }

    //test logki dzialania przeciwnika
    public void EnemyTeamAction() //private
    {
        BattleHandler.instance.characterList[0] = targetCharacter;
        activeCharacter.CharacterAttack(targetCharacter.GetCharacterPosition(),
            () =>
            {

            },
            () =>
            {
                activeCharacter.BackToStartPosition(() => {
                    BattleHandler.instance.ChooseActiveCharacterBattle();
                });
            });
    }

    //ai z wyborem celu ataku
    private CharacterPlayerBase AttackTargetPlayerCharacter()
    { }
        return null;
    }
    */
}
