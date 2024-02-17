using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//klasa do zarzadzania przebiegiem walki
public class BattleHandler : MonoBehaviour
{
    public static BattleHandler Instance { get; private set; }

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private Vector3 playerPos;
    [SerializeField] private Vector3 enemyPos;

    private BattleState battleState;

    private List<CharacterBase> characterList;
    private CharacterBase selectedTargetCharacter;
    private CharacterBase activeCharacter;

    private CharacterBase playerCharacter; //w przyszlosci do usuniecia
    private CharacterBase enemyCharacter; //w przyszlosci do usuniecia

    //status walki
    private enum BattleState
    {
        WaitingForPlayer,
        Busy,
    }

    //pozycja postaci na polu walki
    public enum CharacterLanePosition
    {
        Active,
        TopInactive,
        MiddleInactive,
        BottomInactive,
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
        //jak wszystko zacznie dzialac przeniesc do osobnej funkcji
        switch (battleState)
        {
            case BattleState.WaitingForPlayer:
                //tura gracza
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // Attack
                    battleState = BattleState.Busy;
                    playerCharacter.CharacterAttack(enemyCharacter.GetCharacterPosition(),
                       () =>
                    {
                        enemyCharacter.HealthDamageCalculation();
                        Debug.Log("player.onAttackHit");
                    },
                       () =>
                    {
                        Debug.Log("player.onAttackComplete");
                        playerCharacter.BackToStartPosition(() => Debug.Log("BackToStartPosition"));
                        ChooseActiveCharacterBattle();
                        SetActiveCharacterBattle(enemyCharacter);

                    });
                }
                break;
            case BattleState.Busy:
                break;
        }
    }

    //spawn postaci
    private CharacterBase SpawnCharacter(bool isPlayer)
    {
        Vector3 position;
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

    //ustawienie aktywnej postaci
    //przerobic to na liste
    private void ChooseActiveCharacterBattle()
    {
        //sprawdzenie czy walka nadal trwa
        if (TestBattleOver())
            return;

        //ustawienie aktywnej postaci
        if (activeCharacter == playerCharacter)
        {
            SetActiveCharacterBattle(enemyCharacter);
            battleState = BattleState.Busy;

            //test
            enemyCharacter.CharacterAttack(playerCharacter.GetCharacterPosition(),
            () =>
            {
                Debug.Log("enemy.onAttackHit");
                playerCharacter.HealthDamageCalculation();
            },
            () =>
            {
                Debug.Log("enemy.onAttackComplete");
                enemyCharacter.BackToStartPosition(() => Debug.Log("BackToStartPosition"));
                ChooseActiveCharacterBattle();
            });
        }
        else
        {
            SetActiveCharacterBattle(playerCharacter);
            battleState = BattleState.WaitingForPlayer;
        }
    }

    //funkcja do wlaczania znacznika na aktywnej postaci i wylaczania na nieaktywnych
    //polaczycz z ChooseActiveCharacterBattle()
    private void SetActiveCharacterBattle(CharacterBase characterBattle)
    {
        if (activeCharacter != null)
            activeCharacter.HideSelection();

        activeCharacter = characterBattle;
        activeCharacter.ShowSelection();
    }

    //spradzenie kto wygral walke
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
