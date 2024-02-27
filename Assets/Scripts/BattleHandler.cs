using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.ParticleSystem;

//klasa do zarzadzania przebiegiem walki
public class BattleHandler : MonoBehaviour
{
    public static BattleHandler instance { get; private set; }

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private Transform[] playerSpawnPositions;
    [SerializeField] private Transform[] enemySpawnPositions;

    private BattleState battleState;

    private List<CharacterBase> characterList = new List<CharacterBase>();
    private CharacterBase selectedTargetCharacter;
    private CharacterBase activeCharacter;

    private CharacterBase playerCharacter; //w przyszlosci do usuniecia (activeCharacter)
    private CharacterBase enemyCharacter; //w przyszlosci do usuniecia (activeCharacter)

    //status walki
    private enum BattleState
    {
        WaitingForPlayer,
        Busy,
    }

    //pozycja postaci na polu walki
    public enum CharacterLanePosition
    {
        First = 0,
        Second = 1,
        Third = 2,
        Fourth = 3,
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There's more than one BattleHandler!" + transform + " - " + instance);
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        //test spawnu
        for (int i = 0; i < 3; i++)
        {
            SpawnCharacter(true, i);
        }

        for (int i = 0; i < 4; i++)
        {
            SpawnCharacter(false, i);
        }

        SetActiveCharacterBattle(playerCharacter);
        battleState = BattleState.WaitingForPlayer;
    }

    private void Update()
    {
        PlayerTeamAction();
    }

    private void PlayerTeamAction()
    {
        switch (battleState)
        {
            case BattleState.WaitingForPlayer:
                //tura gracza
                if (Input.GetKeyDown(KeyCode.Space)) //test
                {
                    //basic attack
                    battleState = BattleState.Busy;
                    playerCharacter.CharacterAttack(enemyCharacter.GetCharacterPosition(), () => {

                        playerCharacter.DamageCalculation(enemyCharacter);
                    },
                       () =>
                       {
                           playerCharacter.BackToStartPosition(() => {
                               ChooseActiveCharacterBattle();
                               SetActiveCharacterBattle(enemyCharacter);
                           });
                       });
                }
                break;
            case BattleState.Busy:
                break;
        }
    }

    private void SpawnCharacter(bool isPlayerTeam, int lane)
    {
        if (isPlayerTeam)
        {
            GameObject characterGameObject = Instantiate(playerPrefab, playerSpawnPositions[lane].position, Quaternion.identity);
            CharacterBase spawnedCharacter = characterGameObject.GetComponent<CharacterBase>();
            characterList.Add(spawnedCharacter);
        }
        else if (!isPlayerTeam)
        {
            GameObject characterGameObject = Instantiate(enemyPrefab, enemySpawnPositions[lane].position, Quaternion.identity);
            CharacterBase spawnedCharacter = characterGameObject.GetComponent<CharacterBase>();
            characterList.Add(spawnedCharacter);
        }
    }

    /*
    //spawn postaci
    private CharacterBase SpawnCharacter(bool isPlayer, int lane)
    {
        Vector3 position;
        GameObject character;
        if (isPlayer)
        {
            position = playerSpawnPositions[lane].position;
            character = playerPrefab;
        }
        else
        {
            position = enemySpawnPositions[0].position;
            character = enemyPrefab;
        }

        GameObject characterGameObject = Instantiate(character, position, Quaternion.identity);
        CharacterBase characterBattle = characterGameObject.GetComponent<CharacterBase>();

        return characterBattle;
    }
    */

    //ustawienie aktywnej postaci
    //przerobic na liste
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

            //test akcji przeciwnika
            enemyCharacter.CharacterAttack(playerCharacter.GetCharacterPosition(),
            () =>
            {

            },
            () =>
            {
                enemyCharacter.BackToStartPosition(() => {
                    ChooseActiveCharacterBattle();
                });
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
        if (enemyCharacter.CharacterIsDead())
        {
            //player win
            Debug.Log("Player win");
            return true;
        }
        if (playerCharacter.CharacterIsDead())
        {
            //enemy win
            Debug.Log("Enemy win");
            return true;
        }
        return false;
    }
}
