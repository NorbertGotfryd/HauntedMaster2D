using System.Collections.Generic;
using System;
using UnityEngine;

public class BattleHandler : MonoBehaviour
{
    public static BattleHandler instance { get; private set; }

    [SerializeField] private GameObject[] playerPrefabs;
    [SerializeField] private GameObject[] enemyPrefabs;

    [SerializeField] private Transform[] playerSpawnPositions;
    [SerializeField] private Transform[] enemySpawnPositions;

    private BattleState battleState;

    public event EventHandler OnActiveUnitChanged;

    public SkillBase skillSelected;

    public List<CharacterBase> characterList = new List<CharacterBase>();
    public List<CharacterBase> characterPlayerList = new List<CharacterBase>();
    public List<CharacterBase> characterEnemyList = new List<CharacterBase>();
    public CharacterBase targetCharacter;
    public CharacterBase activeCharacter;

    private int currentActiveCharacterIndex;
    private int turnCount;

    private enum BattleState
    {
        WaitingForPlayer,
        Busy,
    }

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
        for (int i = 0; i < playerSpawnPositions.Length; i++)
        {
            SpawnCharacter(true, i);
        }

        for (int i = 0; i < enemySpawnPositions.Length; i++)
        {
            SpawnCharacter(false, i);
        }

        characterList.AddRange(characterPlayerList);
        characterList.AddRange(characterEnemyList);

        for (int i = 0; i < characterList.Count; i++)
        {
            characterList[i].SetAttackOrder(i);
        }

        characterList.Sort((x, y) => x.attackOrder.CompareTo(y.attackOrder));

        currentActiveCharacterIndex = 0;
        turnCount = 0;
        SetActiveCharacterBattle(characterList[currentActiveCharacterIndex]);

        battleState = BattleState.WaitingForPlayer;
    }

    private void Update()
    {
        PlayerTeamAction();
    }

    public void PlayerTeamAction()
    {
        switch (battleState)
        {
            case BattleState.WaitingForPlayer:
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                    if (hit.collider != null)
                    {
                        CharacterBase clickedCharacter = hit.collider.GetComponent<CharacterBase>();
                        if (clickedCharacter != null && characterEnemyList.Contains(clickedCharacter))
                        {
                            targetCharacter = clickedCharacter;

                            if (skillSelected != null && targetCharacter != null)
                            {
                                battleState = BattleState.Busy;

                                skillSelected.UseSkill(() =>
                                {
                                    Debug.Log("Skill action complete");
                                    activeCharacter.BackToStartPosition(() =>
                                    {
                                        battleState = BattleState.WaitingForPlayer;
                                        ChooseNextCharacterBattle();
                                    });
                                });
                            }
                        }
                    }
                }
                break;
            case BattleState.Busy:
                // Handle busy state if needed
                break;
        }
    }

    private void ChooseNextCharacterBattle()
    {
        do
        {
            currentActiveCharacterIndex++;
            if (currentActiveCharacterIndex >= characterList.Count)
            {
                currentActiveCharacterIndex = 0;
                turnCount++;
            }
        } while (characterList[currentActiveCharacterIndex].CharacterIsDead());

        SetActiveCharacterBattle(characterList[currentActiveCharacterIndex]);
    }

    private void SetActiveCharacterBattle(CharacterBase character)
    {
        if (activeCharacter != null)
            activeCharacter.HideSelection();

        activeCharacter = character;
        activeCharacter.ShowSelection();
        UpdateSkillButtons();

        activeCharacter.StartTurn();

        if (activeCharacter.CompareTag("Player"))
        {
            battleState = BattleState.WaitingForPlayer;
        }
        else
        {
            battleState = BattleState.Busy;
            EnemyTeamAction();
        }
    }

    public void EnemyTeamAction()
    {
        targetCharacter = characterPlayerList[UnityEngine.Random.Range(0, characterPlayerList.Count)];
        activeCharacter.CharacterAttack(targetCharacter.GetCharacterPosition(),
            () =>
            {
                activeCharacter.DamageCalculation(targetCharacter);
            },
            () =>
            {
                activeCharacter.BackToStartPosition(() => {
                    ChooseNextCharacterBattle();
                });
            });
    }

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

    public void UpdateSkillButtons()
    {
        OnActiveUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    private bool TestBattleOver()
    {
        if (characterPlayerList.TrueForAll(c => c.CharacterIsDead()))
        {
            Debug.Log("Enemy win");
            return true;
        }
        if (characterEnemyList.TrueForAll(c => c.CharacterIsDead()))
        {
            Debug.Log("Player win");
            return true;
        }
        return false;
    }

    public CharacterBase GetTargetCharacter() => targetCharacter;
    public CharacterBase GetActiveCharacter() => activeCharacter;
}
