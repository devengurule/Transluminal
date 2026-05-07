using UnityEngine;

public class Movement : MonoBehaviour
{
    #region Variables
    [SerializeField] private Vector2 walkVelocity;
    [SerializeField] private Vector2 sprintVelocity;
    [SerializeField] private Vector2 acceleration;
    [SerializeField] private Vector2 friction;
    [SerializeField] private float knockBackForce;
    [SerializeField] private float stunDuration;

    private EventManager eventManager;
    private Animator animator;
    private SpriteRenderer sr;
    private Vector2 move = Vector2.zero;
    private Vector2 maxVelocity;
    private Rigidbody2D rb;
    private Vector2 lastHidingPos;
    private bool canMove = true;
    private bool isKnockedBack;
    private Timer stunTimer;

    private enum AnimeState
    {
        idle,
        walk,
        run
    }
    private AnimeState animeState = AnimeState.idle;

    #endregion

    #region Unity Methods
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        stunTimer = gameObject.AddComponent<Timer>();
        stunTimer.Initalize(stunDuration, () => isKnockedBack = false, true, false);
        
        maxVelocity = walkVelocity;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.Move, OnMovePlayer);
            eventManager.Subscribe(EventType.SprintOn, OnSprintPlayer);
            eventManager.Subscribe(EventType.SprintOff, OffSprint);
            eventManager.Subscribe(EventType.PauseOn, OnPauseGame);
            eventManager.Subscribe(EventType.PauseOff, OffPauseGame);
            eventManager.Subscribe(EventType.OnEnterCloset, OnEnterCloset);
            eventManager.Subscribe(EventType.OnExitCloset, OnExitCloset);
            eventManager.Subscribe(EventType.AttackPlayer, OnKnockBack);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.Move, OnMovePlayer);
            eventManager.Unsubscribe(EventType.SprintOn, OnSprintPlayer);
            eventManager.Unsubscribe(EventType.SprintOff, OffSprint);
            eventManager.Unsubscribe(EventType.PauseOn, OnPauseGame);
            eventManager.Unsubscribe(EventType.PauseOff, OffPauseGame);
            eventManager.Unsubscribe(EventType.OnEnterCloset, OnEnterCloset);
            eventManager.Unsubscribe(EventType.OnExitCloset, OnExitCloset);
            eventManager.Unsubscribe(EventType.AttackPlayer, OnKnockBack);
        }
    }

    private void Update()
    {
        MovementLogic();
        UpdateAnimation();
    }
    #endregion

    #region Event Methods
    private void OnMovePlayer(object target)
    {
        // Set move to the input vector
        if (target is Vector2 move && canMove)
        {
            this.move = move;
            if (move == Vector2.zero) animeState = AnimeState.idle;
            else if(move != Vector2.zero && TimeManager.deltaTime != 0)
            {
                sr.flipX = move.x > 0;
                if (maxVelocity != sprintVelocity) animeState = AnimeState.walk;
                else animeState = AnimeState.run;
            }
        }
    }

    private void OnSprintPlayer(object target)
    {
        // Sets max velocity to sprint if the sprint button is held down
        maxVelocity = sprintVelocity;
        if (move != Vector2.zero && TimeManager.deltaTime != 0) animeState = AnimeState.run;
    }

    private void OffSprint(object target)
    {
        // Sets max velocity to walk if the sprint button is released
        maxVelocity = walkVelocity;
        if(move != Vector2.zero && TimeManager.deltaTime != 0) animeState = AnimeState.walk;
    }

    private void OnPauseGame(object target)
    {
        // Set all physics numbers to zero when paused
        rb.linearVelocity = Vector2.zero;
        move = Vector2.zero;
        animator.speed = 0;
    }

    private void OffPauseGame(object target)
    {
        animator.speed = 1;
    }

    private void OnEnterCloset(object target)
    {
        canMove = false;

        rb.linearVelocity = Vector2.zero;
        move = Vector2.zero;

        lastHidingPos = transform.position;
        transform.position = GameController.instance.PlayerHidingPos();

        eventManager.Publish(EventType.PlayerHiding, lastHidingPos);
    }
    private void OnExitCloset(object target)
    {
        canMove = true;

        if (target is GameObject obj)
        {
            transform.position = lastHidingPos;
        }
    }

    private void OnKnockBack(object target)
    {
        if(target is GameObject enemy)
        {
            isKnockedBack = true;
            stunTimer.Run();

            rb.linearVelocity = Vector2.zero;

            Vector2 direction = (transform.position - enemy.transform.position).normalized;
            rb.AddForce(direction * knockBackForce, ForceMode2D.Impulse);
        }
    }

    #endregion

    #region Methods
    private void MovementLogic()
    {
        if (isKnockedBack) return;

        // Applies an impulse force to the rigidbody
        rb.AddForce(move * acceleration * TimeManager.deltaTime, ForceMode2D.Impulse);


        // Truncates velocity to match the maximum velocity variable
        if (Mathf.Abs(rb.linearVelocityX) > maxVelocity.x) rb.linearVelocityX = Mathf.Sign(rb.linearVelocityX) * maxVelocity.x;

        if (Mathf.Abs(rb.linearVelocityY) > maxVelocity.y) rb.linearVelocityY = Mathf.Sign(rb.linearVelocityY) * maxVelocity.y;


        // Applys an opposite friction force to x & y axis seperately
        if (move.x == 0 && rb.linearVelocityX != 0)
        {
            rb.AddForce(Vector2.right * -rb.linearVelocityX * acceleration * friction.x * TimeManager.deltaTime, ForceMode2D.Impulse);

            if (Mathf.Abs(rb.linearVelocityX) < 0.005) rb.linearVelocityX = 0;
        }
        if (move.y == 0 && rb.linearVelocityY != 0)
        {
            rb.AddForce(Vector2.up * -rb.linearVelocityY * acceleration * friction.y * TimeManager.deltaTime, ForceMode2D.Impulse);

            if (Mathf.Abs(rb.linearVelocityY) < 0.005) rb.linearVelocityY = 0;
        }
    }

    private void UpdateAnimation()
    {
        

        switch(animeState)
        {
            case AnimeState.idle:
                animator.SetFloat("State", 0);
                break;

            case AnimeState.walk:
                animator.SetFloat("State", 1);
                break;

            case AnimeState.run:
                animator.SetFloat("State", 2);
                break;
        }
    }

    #endregion
}
