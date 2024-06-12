﻿using System;
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

    public event EventHandler OnActiveUnitChanged; //private

    public SkillBase skillSelected; //private

    public List<CharacterBase> characterList = new List<CharacterBase>(); //private
    public List<CharacterBase> characterPlayerList = new List<CharacterBase>(); //private
    public List<CharacterBase> characterEnemyList = new List<CharacterBase>(); //private
    public CharacterBase targetCharacter; //private
    public CharacterBase activeCharacter; //private


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
        //test spawnu gracz, i = lane
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

        //laczenie list postaci gracza i przeciwnikow w jedna
        characterList.AddRange(characterPlayerList);
        characterList.AddRange(characterEnemyList);

        // ustawienie kolejnosci ataku dla postaci
        for (int i = 0; i < characterList.Count; i++)
        {
            characterList[i].SetAttackOrder(i);
        }

        // sortowanie postaci wedlug kolejnosci ataku (inicjatywa)
        characterList.Sort((x, y) => x.attackOrder.CompareTo(y.attackOrder));

        //SetActiveCharacterBattle(characterList[0]);
        ChooseActiveCharacterBattle();
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
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    targetCharacter = characterEnemyList[0];

                    if (skillSelected != null && targetCharacter != null)
                    {
                        battleState = BattleState.Busy;

                        // Use the selected skill on the target character
                        skillSelected.UseSkill(() =>
                        {
                            // Skill action complete callback
                            Debug.Log("Skill action complete");

                            // After using the skill, move the active character back to start position
                            activeCharacter.BackToStartPosition(() =>
                            {
                                battleState = BattleState.WaitingForPlayer;
                                ChooseActiveCharacterBattle();
                            });
                        });
                    }
                }
                break;
            case BattleState.Busy:
                // Handle busy state if needed
                break;
        }
    }

    //polaczyc enum z lane
    //osobna lista gracza i przeciwnikow, kolejka na trzeciej liscie polaczonej z obu poprzednich
    private void SpawnCharacter(bool isPlayerTeam, int lane)
    {
        if (isPlayerTeam)
        {
            GameObject characterGameObject = Instantiate(playerPrefabs[0], playerSpawnPositions[lane].position, Quaternion.identity);
            CharacterBase spawnedCharacter = characterGameObject.GetComponent<CharacterBase>();
            characterPlayerList.Add(spawnedCharacter);
        }
        else if (!isPlayerTeam)
        {
            GameObject characterGameObject = Instantiate(enemyPrefabs[0], enemySpawnPositions[lane].position, Quaternion.identity);
            CharacterBase spawnedCharacter = characterGameObject.GetComponent<CharacterBase>();
            characterEnemyList.Add(spawnedCharacter);
        }
    }


    //ustawienie aktywnej postaci
    public void ChooseActiveCharacterBattle() //private
    {
        //sprawdzenie czy walka nadal trwa
        //if (TestBattleOver())
            //return;

        if (activeCharacter != null)
            activeCharacter.HideSelection();

        if (activeCharacter == null || characterList.IndexOf(activeCharacter) == characterList.Count - 1)
            activeCharacter = characterList[0];
        else
            activeCharacter = characterList[characterList.IndexOf(activeCharacter) + 1];

        activeCharacter.ShowSelection();

        UpdateSkillButtons();

        // sprawdzenie czy postac jest zywa
        //przeskok na inna "zywa" postac
        if (activeCharacter.CharacterIsDead())
            return;

        if (activeCharacter.CompareTag("Player"))
            battleState = BattleState.WaitingForPlayer;
        else
        {
            battleState = BattleState.Busy;
            EnemyTeamAction();
        }
    }

    //test logki dzialania przeciwnika
    public void EnemyTeamAction() //private
    {
        targetCharacter = characterList[0];
        activeCharacter.CharacterAttack(targetCharacter.GetCharacterPosition(),
                () =>
            {
                activeCharacter.DamageCalculation(targetCharacter);
            },
                () =>
            {
                activeCharacter.BackToStartPosition(() => {
                    ChooseActiveCharacterBattle();
                });
            });
    }

    //test, nie dziala
    public void UpdateSkillButtons()
    {
        OnActiveUnitChanged?.Invoke(this, EventArgs.Empty);
    }


    //test spradzenia kto wygral walke
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

    public CharacterBase GetTargetCharacter() => targetCharacter;
    public CharacterBase GetActiveCharacter() => activeCharacter;
}