using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    private const float STOP_DISTANCE_TO_TARGET = 1f;
    private const float STOP_DISTANCE_TO_RETURN = 0.05f;

    [Header("Base Stats")]
    [SerializeField] protected CharacterElement characterElement;
    [SerializeField] protected int healhAmountMax;
    [SerializeField] protected int defAmount;
    [SerializeField] protected int initiativeAmountMax;
    [SerializeField] protected int moveToTargetSpeed;

    protected int healhAmountCurrent;
    protected int initiativeAmountCurrent;

    protected bool isPlayerTeam;

    protected Vector3 startingPosition;
    protected Vector3 moveToPosition;

    protected CharacterState state;
    protected CharacterElement attackerElement;
    protected CharacterElement defenderElement;

    protected GameObject selectionVisual;
    protected AnimationSystem animationSystem;

    protected SkillBase[] skillsBaseArray;

    private List<StatusBase> statusEffects = new List<StatusBase>();

    private int currentTurn;
    private bool isTurnActive;

    protected Action onSkillUse;
    protected Action onSkillComplete;
    protected Action onMoveComplete;
    protected Action OnHealthChange;
    protected Action OnDeath;

    protected enum CharacterState
    {
        Idle,
        MoveToTargetAndDoAction,
        DoActionOnTarget,
        MoveToStartPosition,
        MoveToPosition,
        Busy,
    }

    public enum CharacterElement
    {
        Neutral,
        Fire,
        Water,
        Wind,
    }

    private void Awake()
    {
        animationSystem = FindObjectOfType<AnimationSystem>();
        skillsBaseArray = GetComponents<SkillBase>();

        selectionVisual = transform.Find("SelectionVisual").gameObject;

        state = CharacterState.Idle;
        HideSelection();
    }

    public int attackOrder;

    public void SetAttackOrder(int order)
    {
        attackOrder = order;
    }

    private void Start()
    {
        healhAmountCurrent = healhAmountMax;
        startingPosition = transform.position;

        currentTurn = 0;
        isTurnActive = false;
    }

    private void Update()
    {
        CharacterBattleState();

        if (isTurnActive)
        {
            ActivateStatusEffects();
        }
    }

    public void StartTurn()
    {
        isTurnActive = true;
        currentTurn++;
        ActivateStatusEffects();
    }

    private void ActivateStatusEffects()
    {
        foreach (var status in new List<StatusBase>(statusEffects))
            status.Activate();
    }

    public void AddStatus(StatusBase status)
    {
        var existingStatus = statusEffects.Find(s => s.GetType() == status.GetType());
        if (existingStatus != null)
        {
            existingStatus.AddPower(status.GetPower());
            existingStatus.Initialize(existingStatus.GetPower(), Math.Max(existingStatus.GetDuration(), status.GetDuration()), this);
        }
        else
        {
            statusEffects.Add(status);
            status.Initialize(status.GetPower(), status.GetDuration(), this);
        }
    }

    public void RemoveStatus(StatusBase status)
    {
        statusEffects.Remove(status);
    }

    public void CharacterAttack(Vector3 targetPosition, Action onSkillUse, Action onSkillComplete)
    {
        this.onSkillUse = onSkillUse;
        this.onSkillComplete = onSkillComplete;
        moveToPosition = targetPosition + (GetCharacterPosition() - targetPosition).normalized;
        state = CharacterState.MoveToTargetAndDoAction;
    }

    public void CharacterHealing(Vector3 targetPosition, Action onSkillUse, Action onSkillComplete)
    {
        this.onSkillUse = onSkillUse;
        this.onSkillComplete = onSkillComplete;
        moveToPosition = targetPosition + (GetCharacterPosition() - targetPosition).normalized;
        state = CharacterState.MoveToTargetAndDoAction;
    }

    public void MoveToTargetPosition(Vector3 targetPosition, Action onMoveComplete)
    {
        this.onMoveComplete = onMoveComplete;
        moveToPosition = targetPosition;
        state = CharacterState.MoveToPosition;
    }

    public void BackToStartPosition(Action onMoveComplete)
    {
        this.onMoveComplete = onMoveComplete;
        moveToPosition = startingPosition;
        state = CharacterState.MoveToStartPosition;
    }

    private float DamageMultiplier(string attackerElement, string defenderElement)
    {
        Dictionary<string, float> attackValues = new Dictionary<string, float>()
        {
            {"NeutralNeutral", 1f},
            {"NeutralFire", 1f},
            {"NeutralEarth", 1f},
            {"NeutralWater", 1f},
            {"NeutralWind", 1f},
            {"FireNeutral", 1f},
            {"FireFire", 1f},
            {"FireEarth", 1.5f},
            {"FireWater", 0.5f},
            {"FireWind", 0.5f},
            {"EarthNeutral", 1f},
            {"EarthFire", 0.5f},
            {"EarthEarth", 1f},
            {"EarthWater", 1.5f},
            {"EarthWind", 0.5f},
            {"WaterNeutral", 1f},
            {"WaterFire", 1.5f},
            {"WaterEarth", 0.5f},
            {"WaterWater", 1f},
            {"WaterWind", 1.5f},
            {"WindNeutral", 1f},
            {"WindFire", 1.5f},
            {"WindEarth", 1.5f},
            {"WindWater", 0.5f},
            {"WindWind", 1f},
        };
        string toDict = attackerElement + defenderElement;
        float atkMultiplier = attackValues[toDict];

        return atkMultiplier;
    }

    public void DamageCalculation(CharacterBase targetCharacter, int damageAmount = 0)
    {
        if (targetCharacter == null)
            return;

        if (damageAmount == 0)
        {
            SkillBase.SkillElement attackerElement = BattleHandler.instance.skillSelected.GetSkillElement();
            int attack = BattleHandler.instance.skillSelected.GetAttackAmount();
            float minDmg = 1f;
            float maxDmg = 1f;
            damageAmount = (int)(UnityEngine.Random.Range(attack * minDmg, attack * maxDmg)
                * DamageMultiplier(attackerElement.ToString(), targetCharacter.GetCharacterElement().ToString()));
        }

        if (damageAmount < 0)
            damageAmount = 0;

        if (targetCharacter.defAmount < 0)
            targetCharacter.defAmount = 0;

        int armorDamage = Math.Min(targetCharacter.defAmount, damageAmount);
        int healthDamage = Math.Min(targetCharacter.healhAmountCurrent, damageAmount - armorDamage);
        targetCharacter.defAmount -= armorDamage;
        targetCharacter.healhAmountCurrent -= healthDamage;

        Debug.Log(targetCharacter.name + " HP: " + targetCharacter.healhAmountCurrent + " & DEF: " + targetCharacter.defAmount);

        if (targetCharacter.healhAmountCurrent < 0)
            targetCharacter.healhAmountCurrent = 0;

        targetCharacter.OnHealthChange?.Invoke();

        if (targetCharacter.healhAmountCurrent <= 0)
            targetCharacter.CharacterDie();
    }

    public void HealingCalculation(CharacterBase targetCharacter, int healingAmount = 0)
    {
        if (targetCharacter == null)
            return;

        if (healingAmount == 0)
        {
            int heal = BattleHandler.instance.skillSelected.GetHealingAmount();
            float minHeal = 1f;
            float maxHeal = 1f;
            healingAmount = (int)(UnityEngine.Random.Range(heal * minHeal, heal * maxHeal));
        }

        if (healingAmount < 0)
            healingAmount = 0;

        int newHealthAmount = targetCharacter.healhAmountCurrent + healingAmount;

        if (newHealthAmount > targetCharacter.healhAmountMax)
            targetCharacter.healhAmountCurrent = targetCharacter.healhAmountMax;
        else
            targetCharacter.healhAmountCurrent = newHealthAmount;

        Debug.Log(targetCharacter.name + " HP: " + targetCharacter.healhAmountCurrent + " & DEF: " + targetCharacter.defAmount);

        targetCharacter.OnHealthChange?.Invoke();
    }

    private void CharacterBattleState()
    {
        switch (state)
        {
            case CharacterState.Idle:
                break;
            case CharacterState.MoveToTargetAndDoAction:
                if (Vector3.Distance(moveToPosition, GetCharacterPosition()) > STOP_DISTANCE_TO_TARGET)
                {
                    transform.position += (moveToPosition - GetCharacterPosition()) * moveToTargetSpeed * Time.deltaTime;
                }
                else
                {
                    onSkillUse();
                    state = CharacterState.DoActionOnTarget;
                    onSkillComplete();
                }
                break;
            case CharacterState.MoveToPosition:
                if (Vector3.Distance(moveToPosition, GetCharacterPosition()) > STOP_DISTANCE_TO_TARGET)
                {
                    transform.position += (moveToPosition - GetCharacterPosition()) * moveToTargetSpeed * Time.deltaTime;
                }
                else
                {
                    state = CharacterState.Busy;
                    state = CharacterState.Idle;
                    onMoveComplete();
                }
                break;
            case CharacterState.DoActionOnTarget:
                break;
            case CharacterState.MoveToStartPosition:
                if (Vector3.Distance(startingPosition, GetCharacterPosition()) >= STOP_DISTANCE_TO_RETURN)
                {
                    transform.position += (moveToPosition - GetCharacterPosition()) * moveToTargetSpeed * Time.deltaTime;
                }
                else
                {
                    state = CharacterState.Idle;
                    onMoveComplete();
                }
                break;
            case CharacterState.Busy:
                break;
        }
    }

    public void CharacterDie()
    {
        OnDeath?.Invoke();
    }

    public void ShowSelection()
    {
        selectionVisual.SetActive(true);
    }

    public void HideSelection()
    {
        selectionVisual.SetActive(false);
    }

    public bool CharacterIsDead() => healhAmountCurrent <= 0;

    public Vector3 GetCharacterPosition() => transform.position;

    public CharacterElement GetCharacterElement() => characterElement;

    public int GetInitiativeAmountMax() => initiativeAmountMax;

    public int GetInitiativeAmountCurrent() => initiativeAmountCurrent;

    public SkillBase[] GetSkillsBaseArray() => skillsBaseArray;
}

