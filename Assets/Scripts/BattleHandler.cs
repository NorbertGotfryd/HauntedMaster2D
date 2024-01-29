using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//klasa do zarzadzania przebiegiem walki
public class BattleHandler : MonoBehaviour
{
    public static BattleHandler Instance { get; private set; }

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private Vector2 playerPos;
    [SerializeField] private Vector2 enemyPos;

    private List<CharacterBase> characterList;
    private CharacterBase selectedTargetCharacter;
    private CharacterBase activeCharacter;

    private CharacterBase playerCharacter; //w przyszlosci do usuniecia
    private CharacterBase enemyCharacter; //w przyszlosci do usuniecia
    private BattleState battleState;

    //status walki
    private enum BattleState
    {
        WaitingForPlayer,
        Busy,
    }

    public enum CharacterLanePosition
    {
        Top,
        Up,
        Middle,
        Down,
        Bottom,
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one BattleHandler!" + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        playerCharacter = SpawnCharacter(true);
        enemyCharacter = SpawnCharacter(false);

        SetActiveCharacterBattle(playerCharacter);
        battleState = BattleState.WaitingForPlayer;
    }


    private void Update()
    {
        switch (battleState)
        {
            case BattleState.WaitingForPlayer:
                //tura gracza

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // Attack
                    battleState = BattleState.Busy;
                    playerCharacter.CharacterAttack(enemyCharacter.GetCharacterPosition(), () =>
                    {
                        //dlaczego to sie nie odpala?
                        Debug.Log("player.onAttackHit");
                        //animacja ataku
                    }, () =>
                    {
                        Debug.Log("player.onAttackComplete");
                        //playerCharacter.HealthDamageCalculation();
                        playerCharacter.BackToStartPosition(playerCharacter.startingPosition);
                        ChooseActiveCharacterBattle();
                        SetActiveCharacterBattle(enemyCharacter);
                    });
                }

                /*
                //testowa funkcja, przy nacisnieciu spacji postac gracza atakuje
                if(state == BattleState.WaitingForPlayer)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        state = BattleState.Busy;
                        playerCharacterBattle.CharacterAttack(enemyCharacterBattle, () =>
                        {
                            ChooseActiveCharacterBattle();
                        });
                    }
                }
                */
                break;
            case BattleState.Busy:
                break;
        }
    }


    //spawn postaci, do przerobienia zeby lepiej rozpoznawalo kogo ma spawnowac i gdzie
    private CharacterBase SpawnCharacter(bool isPlayer)
    {
        Vector2 position;
        GameObject character;
        if (isPlayer)
        {
            position = playerPos;
            character = playerPrefab;
        }
        else
        {
            position = enemyPos;
            character = enemyPrefab;
        }

        GameObject characterGameObject = Instantiate(character, position, Quaternion.identity);
        CharacterBase characterBattle = characterGameObject.GetComponent<CharacterBase>();

        return characterBattle;
    }

    //logika znacznika aktywnej postaci
    private void SetActiveCharacterBattle(CharacterBase characterBattle)
    {
        if (activeCharacter != null)
            activeCharacter.HideSelection();

        activeCharacter = characterBattle;
        activeCharacter.ShowSelection();
    }

    //ustawienie aktywnej postaci, na razie jest to mega prymitywne bo albo rusza sie gracz albo przeciwnik
    //w przyszlosci bedzie to lista
    private void ChooseActiveCharacterBattle()
    {
        if (TestBattleOver())
            return;

        if (activeCharacter == playerCharacter)
        {
            SetActiveCharacterBattle(enemyCharacter);
            battleState = BattleState.Busy;

            //test
            enemyCharacter.CharacterAttack(playerCharacter.GetCharacterPosition(),
            () =>
            {
                Debug.Log("enemy.onAttackHit");
            },
            () =>
            {
                Debug.Log("enemy.onAttackComplete");
                //enemyCharacter.HealthDamageCalculation();
                enemyCharacter.BackToStartPosition(enemyCharacter.startingPosition);
                ChooseActiveCharacterBattle();
            });


            /*
            enemyCharacterBattle.CharacterAttack(playerCharacterBattle, () =>
            {
                ChooseActiveCharacterBattle();
            });
            */
        }
        else
        {
            SetActiveCharacterBattle(playerCharacter);
            battleState = BattleState.WaitingForPlayer;
        }
    }

    //testowanie kto wygral
    private bool TestBattleOver()
    {
        if (playerCharacter.CharacterIsDead())
        {
            //enemy win
            Debug.Log("Enemy win");
            return true;
        }
        if (enemyCharacter.CharacterIsDead())
        {
            //player win
            Debug.Log("Player win");
            return true;
        }

        return false;
    }
}
