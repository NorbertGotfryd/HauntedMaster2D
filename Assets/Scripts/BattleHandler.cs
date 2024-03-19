using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//klasa do zarzadzania przebiegiem walki
public class BattleHandler : MonoBehaviour
{
    public static BattleHandler instance { get; private set; }

    [SerializeField] private GameObject[] playerPrefabs;
    [SerializeField] private GameObject[] enemyPrefabs;

    [SerializeField] private Transform[] playerSpawnPositions;
    [SerializeField] private Transform[] enemySpawnPositions;

    private BattleState battleState;

    private List<CharacterBase> characterList = new List<CharacterBase>();
    private CharacterBase targetCharacter;
    private CharacterBase activeCharacter;
    private CharacterBase nextActiveCharacter;

    //status walki
    private enum BattleState
    {
        WaitingForPlayer,
        Busy,
    }

    //pozycja postaci na polu walki
    public enum CharacterLanePosition
    {
        First,
        Second,
        Third,
        Fourth,
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
        //characterList[0] -> pierwsza postac gracza
        //test spawnu gracz
        for (int i = 0; i < playerSpawnPositions.Length; i++)
        {
            SpawnCharacter(true, i);
        }

        //characterList[4] -> pierwsza postac przeciwnikow
        //test spawnu przeciwnicy
        for (int i = 0; i < enemySpawnPositions.Length; i++)
        {
            SpawnCharacter(false, i);
        }

        SetActiveCharacterBattle(characterList[0]);
        battleState = BattleState.WaitingForPlayer;
    }

    private void Update()
    {
        PlayerTeamAction();
    }

    //ustawienie listy kolejki postaci wedlug inicjatywy
    private CharacterBase TurnOrder()
    {


        return nextActiveCharacter;
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
                    characterList[0].CharacterAttack(characterList[4].GetCharacterPosition(), () => {

                        characterList[0].DamageCalculation(characterList[4]);
                    },
                       () =>
                       {
                           characterList[0].BackToStartPosition(() => {
                               ChooseActiveCharacterBattle();
                               SetActiveCharacterBattle(characterList[4]);
                           });
                       });
                }
                break;
            case BattleState.Busy:
                break;
        }
    }

    //polaczyc enum z lane
    private void SpawnCharacter(bool isPlayerTeam, int lane)
    {
        if (isPlayerTeam)
        {
            GameObject characterGameObject = Instantiate(playerPrefabs[0], playerSpawnPositions[lane].position, Quaternion.identity);
            CharacterBase spawnedCharacter = characterGameObject.GetComponent<CharacterBase>();
            characterList.Add(spawnedCharacter);
        }
        else if (!isPlayerTeam)
        {
            GameObject characterGameObject = Instantiate(enemyPrefabs[0], enemySpawnPositions[lane].position, Quaternion.identity);
            CharacterBase spawnedCharacter = characterGameObject.GetComponent<CharacterBase>();
            characterList.Add(spawnedCharacter);
        }
    }

    /* stary test spawn, 1 postac na druzyne
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
    private void ChooseActiveCharacterBattle()
    {
        //sprawdzenie czy walka nadal trwa
        if (TestBattleOver())
            return;

        //ustawienie aktywnej postaci
        if (activeCharacter == characterList[0])
        {
            SetActiveCharacterBattle(characterList[0]);
            battleState = BattleState.Busy;

            //test akcji przeciwnika, zaprogramowac AI
            characterList[4].CharacterAttack(characterList[0].GetCharacterPosition(),
            () =>
            {

            },
            () =>
            {
                characterList[4].BackToStartPosition(() => {
                    ChooseActiveCharacterBattle();
                });
            });
        }
        else
        {
            SetActiveCharacterBattle(characterList[0]);
            battleState = BattleState.WaitingForPlayer;
        }
    }

    //funkcja do wlaczania znacznika na aktywnej postaci i wylaczania na nieaktywnych
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
        if (characterList[4].CharacterIsDead())
        {
            //player win
            Debug.Log("Player win");
            return true;
        }
        if (characterList[0].CharacterIsDead())
        {
            //enemy win
            Debug.Log("Enemy win");
            return true;
        }
        return false;
    }
}