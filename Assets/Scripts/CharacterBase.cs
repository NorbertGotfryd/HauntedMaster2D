using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//klasa bazowa wspolnych funkcji dla wszystkich postaci
public abstract class CharacterBase : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] protected int healhAmountMax;
    [SerializeField] protected int attackAmount;
    [SerializeField] protected int defAmount;
    [SerializeField] protected float moveToTargetSpeed;

    private bool isPlayerTeam;
    protected int healhAmountCurrent;
    protected float stopDistanceToTargetPosition = 1f;

    public Vector2 startingPosition; //na protected zmienic
    protected Vector2 moveToPosition;
    protected Vector2 currentPosition;

    protected CharacterState state;

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
        startingPosition = GetCharacterPosition(); //dziala ale bez sensu
    }

    private void Update()
    {
        currentPosition = GetCharacterPosition();// new Vector2(transform.position.x, transform.position.y); //test

        CharacterBattleState(); //to powinno byc na eventach
    }

    //zainicjalizowanie postaci na polu walki
    //beda tu takie rzeczy jak pozycja, typ, druzyna i statystyki
    public void SetupCharacter(Vector2 startingPosition)
    {

    }

    //ustawienie statusu postaci, trzeba to troche przerobic
    private void CharacterBattleState()
    {
        switch (state)
        {
            case CharacterState.Idle:
                break;
            case CharacterState.MoveToTargetAndAttack:
                if (Vector2.Distance(moveToPosition, GetCharacterPosition()) >= stopDistanceToTargetPosition)
                {
                    currentPosition += (moveToPosition - GetCharacterPosition()) * moveToTargetSpeed * Time.deltaTime;
                }
                else //dotarcie do docelowej pozycji
                {
                    state = CharacterState.AttackTarget;
                    //animationSystem.AnimationAttack(); //jakims cudem blokuje akcje dalej, trzeba w animatorze pogrzebac
                    onAttackComplete();

                    /* chyba cos takiego powinno byc, do sprawdzenia jak reszta bedzie dzialac
                        animationSystem.AnimationAttack(()=>
                        {
                        AnimationIdleEnable();
                        onAttackComplete();
                        });
                    */

                }
                break;
            case CharacterState.MoveToPosition:
                if (Vector2.Distance(moveToPosition, GetCharacterPosition()) > stopDistanceToTargetPosition)
                {
                    currentPosition += (moveToPosition - GetCharacterPosition()) * moveToTargetSpeed * Time.deltaTime;
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
            case CharacterState.MoveToStartPosition: //?
                if (Vector2.Distance(moveToPosition, GetCharacterPosition()) > stopDistanceToTargetPosition)
                {
                    currentPosition += (moveToPosition - GetCharacterPosition()) * moveToTargetSpeed * Time.deltaTime;
                }
                else
                {
                    //funkcja animacji spoczynku (idle)
                    state = CharacterState.Idle;
                    //onMoveComplete();
                }
                break;
            case CharacterState.Busy:
                break;
        }
    }

    //doskok z atakiem
    public void CharacterAttack(Vector2 targetPosition, Action onAttackHit, Action onAttackComplete)
    {
        this.onAttackHit = onAttackHit;
        this.onAttackComplete = onAttackComplete;
        moveToPosition = targetPosition + (GetCharacterPosition() - targetPosition).normalized * moveToTargetSpeed;
        //animacja doskoku do celu lub przygotowania do ataku, w sumie cokolwiek
        state = CharacterState.MoveToTargetAndAttack;
    }

    //doskok
    public void MoveToTargetPosition(Vector2 targetPosition, Action onMoveComplete)
    {
        moveToPosition = targetPosition;
        this.onMoveComplete = onMoveComplete;
        //animacja doskoku do przeciwnika
        state = CharacterState.MoveToPosition;
    }

    //powrot do oryginalnej pozycji
    public void BackToStartPosition(Vector2 startingPosition)//Action onMoveComplete)
    {
        //this.onMoveComplete = onMoveComplete;
        this.startingPosition = startingPosition; //?
        moveToPosition = startingPosition; //?
        //animacja powrotu
        state = CharacterState.MoveToStartPosition;
    }

    //funkcja obliczajaca przyjete obrazenia przez postac
    public int DamageDealth(int attAmount, int protAmount)
    {
        int damageDeal = UnityEngine.Random.Range(1, attAmount) - (0);

        return damageDeal;
    }

    //obliczanie zycia postaci i co sie z nia dzieje
    //to wywolywac do zadawania obrazen
    public void HealthDamageCalculation()
    {
        healhAmountCurrent -= DamageDealth(attackAmount, defAmount);
        if (healhAmountCurrent < 0)
            healhAmountCurrent = 0;

        if (OnHealthChange != null)
            OnHealthChange(this, EventArgs.Empty);

        if (healhAmountCurrent <= 0)
            CharacterDie();

        Debug.Log(gameObject.name + " HP left: " + healhAmountCurrent);
    }

    //smierc postaci
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

    //funkcja do zwrocenia pozycji postaci
    public Vector2 GetCharacterPosition() => new Vector2(transform.position.x, transform.position.y);
}
