using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//klasa bazowa wspolnych funkcji dla wszystkich postaci
public abstract class CharacterBase : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] protected CharacterElement characterElement;
    [SerializeField] protected int healhAmountMax;
    [SerializeField] protected int attackAmount;
    [SerializeField] protected int defAmount;
    [SerializeField] protected int initiativeAmountMax;
    [SerializeField] protected int moveToTargetSpeed;

    private bool isPlayerTeam;
    protected int healhAmountCurrent;
    protected int initiativehAmountCurrent;
    protected float stopDistanceToTargetPosition = 1f;

    public Vector3 startingPosition; //protected
    protected Vector3 moveToPosition;

    protected CharacterState state;
    protected CharacterElement attackerElement;
    protected CharacterElement defenderElement;

    protected GameObject selectionVisual;
    protected AnimationSystem animationSystem;

    protected Action onAttackHit;
    protected Action onAttackComplete;
    protected Action onMoveComplete;

    public event EventHandler OnHealthChange;
    public event EventHandler OnDeath;

    //status postaci w czasie walki
    protected enum CharacterState
    {
        Idle,
        MoveToTargetAndAttack,
        AttackTarget,
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

        healhAmountCurrent = healhAmountMax;

        selectionVisual = transform.Find("SelectionVisual").gameObject;
        state = CharacterState.Idle;
        HideSelection();
    }

    private void Start()
    {
        startingPosition = transform.position;
    }

    private void Update()
    {
        CharacterBattleState(); //to musi byc na eventach w przyszlosi
    }

    //zainicjalizowanie postaci na polu walki
    //beda tu takie rzeczy jak pozycja, typ, druzyna etc
    public void SetupCharacter(Vector3 startingPosition)
    {

    }

    //ustawienie statusu postaci
    private void CharacterBattleState()
    {
        switch (state)
        {
            case CharacterState.Idle:
                break;
            case CharacterState.MoveToTargetAndAttack:
                if (Vector3.Distance(moveToPosition, GetCharacterPosition()) > stopDistanceToTargetPosition)
                {
                    transform.position += (moveToPosition - GetCharacterPosition()) * moveToTargetSpeed * Time.deltaTime;
                }
                else //dotarcie do docelowej pozycji
                {
                    onAttackHit();
                    state = CharacterState.AttackTarget;
                    onAttackComplete();
                }
                break;
            case CharacterState.MoveToPosition:
                if (Vector3.Distance(moveToPosition, GetCharacterPosition()) > stopDistanceToTargetPosition)
                {
                    transform.position += (moveToPosition - GetCharacterPosition()) * moveToTargetSpeed * Time.deltaTime;
                }
                else //dotarcie do docelowej pozycji
                {
                    state = CharacterState.Busy;
                    //animacja
                    state = CharacterState.Idle;
                    onMoveComplete();
                }
                break;
            case CharacterState.AttackTarget:
                break;
            case CharacterState.MoveToStartPosition:
                if (Vector3.Distance(moveToPosition, GetCharacterPosition()) > stopDistanceToTargetPosition)
                {
                    transform.position += (moveToPosition - GetCharacterPosition()) * moveToTargetSpeed * Time.deltaTime;
                }
                else
                {
                    //animacji spoczynku (idle)
                    state = CharacterState.Idle;
                    onMoveComplete();
                }
                break;
            case CharacterState.Busy:
                break;
        }
    }

    //doskok z atakiem
    public void CharacterAttack(Vector3 targetPosition, Action onAttackHit, Action onAttackComplete)
    {
        this.onAttackHit = onAttackHit;
        this.onAttackComplete = onAttackComplete;
        moveToPosition = targetPosition + (GetCharacterPosition() - targetPosition).normalized * moveToTargetSpeed;
        //animacja doskoku do celu lub przygotowania do ataku
        state = CharacterState.MoveToTargetAndAttack;
    }

    //dotarcie do celu, bedzie uzywane do wszystkiego innego co wymaga przemieszczenia postaci poza atakiem
    public void MoveToTargetPosition(Vector3 targetPosition, Action onMoveComplete)
    {
        this.onMoveComplete = onMoveComplete;
        moveToPosition = targetPosition;
        //animacja doskoku do przeciwnika
        state = CharacterState.MoveToPosition;
    }

    //powrot do oryginalnej pozycji
    public void BackToStartPosition(Action onMoveComplete)
    {
        this.onMoveComplete = onMoveComplete;
        moveToPosition = startingPosition;
        //animacja powrotu
        state = CharacterState.MoveToStartPosition;
    }

    //funkcja okreslajaca przeliczniki obrazen miedzy zywiolami
    public float DamageMultiplier(string attackerElement, string defenderElement)
    {
        Dictionary<string, float> attackValues = new Dictionary<string, float>()
        {
            //Neutralny
            {"NeutralFire", 1f},
            {"NeutralEarth", 1f},
            {"NeutralWind", 1f},
            {"NeutralWater", 1f},
            {"NeutralNeutral", 1f},
            //Ogien koniunkcja
            {"FireNeutral", 1f},
            {"FireFire", 1f},
            {"FireWater", 1.5f},
            {"FireEarth", 2f},
            {"FireWind", 0.5f},
            //Ziemia koniunkcja
            {"EarthNeutral", 1f},
            {"EarthEarth", 1f},
            {"EarthFire", 0.5f},
            {"EarthWater", 2f},
            {"EarthWind", 1.5f},
            //Woda koniunkcja
            {"WaterNeutral", 1f},
            {"WaterWater", 1f},
            {"WaterFire", 1.5f},
            {"WaterEarth", 0.5f},
            {"WaterWind", 2f},
            //Wiatr koniunkcja
            {"WindNeutral", 1f},
            {"WindWind", 1f},
            {"WindFire", 2f},
            {"WindEarth", 1.5f},
            {"WindWater", 0.5f},
        };
        string toDict = attackerElement + defenderElement;
        float atkMultiplier = attackValues[toDict];

        return atkMultiplier;
    }

    //funkcja obliczajaca przyjete obrazenia przez postac
    public int Damage(int attackAmount, int defAmount)//, float multiplier)
    {
        //do obliczen trzeba dodac defAmount
        int damageAmount = (int)(UnityEngine.Random.Range(attackAmount * 0.8f, attackAmount * 1.2f) * (DamageMultiplier("Wind", "Water")));

        return damageAmount;
    }

    //obliczanie pozostalego HP postaci i sprawdza czy posatc zginela
    //Wywolywac do zadawania obrazen postaciom
    public void HealthDamageCalculation()
    {
        //trzeba postawic tutaj staty przeciwnika bo bierze atakowanego
        healhAmountCurrent -= Damage(attackAmount, defAmount);
        if (healhAmountCurrent < 0)
            healhAmountCurrent = 0;

        if (OnHealthChange != null)
            OnHealthChange(this, EventArgs.Empty);

        if (healhAmountCurrent <= 0)
            CharacterDie();

        Debug.Log(gameObject.name + " HP left: " + healhAmountCurrent);
    }

    //zabicie postaci
    public void CharacterDie()
    {
        if (OnDeath != null)
            OnDeath(this, EventArgs.Empty);
    }

    //ukrycie znacznika aktywnej postaci
    public void ShowSelection()
    {
        selectionVisual.SetActive(true);
    }

    //pokazanie znacznika aktywnej postaci
    public void HideSelection()
    {
        selectionVisual.SetActive(false);
    }

    //sprawdzenie czy postac jest martwa
    public bool CharacterIsDead() => healhAmountCurrent <= 0;

    //zwrocenie pozycji postaci
    public Vector3 GetCharacterPosition() => transform.position;
}