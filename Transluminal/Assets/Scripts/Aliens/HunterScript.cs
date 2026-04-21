using Mono.Cecil;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HunterScript : MonoBehaviour
{
    #region Variables
    [SerializeField] private Vector2 speed;
    [SerializeField] private Vector2 maxSpeed;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float attackRadius;
    [SerializeField] private float wakeUpTime;
    [SerializeField] private float wanderTime;
    [SerializeField] private Vector2 t;
    [SerializeField] private float VelocityT;

    private AlienSaveData saveDataInstance;
    private EventManager eventManager;
    private Timer lifeTimeTimer;
    private Timer wakeUpTimer;
    private Timer wanderTimer;
    private Rigidbody2D rb;
    private float lifeTime;
    private Vector2 direction;
    private Vector2 target;
    private bool isAwake;
    private bool isFleeing;
    private bool isWandering;
    private bool isAttacking;
    private bool arrivedAtTarget;

    private Vector2 gizmosTarget;

    private State currentState;
    private enum State
    {
        hide,
        chase,
        attack,
        wander,
        flee
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        lifeTimeTimer = gameObject.AddComponent<Timer>();
        wakeUpTimer = gameObject.AddComponent<Timer>();
        wanderTimer = gameObject.AddComponent<Timer>();
    }

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        SceneManager.sceneUnloaded += OnSceneUnloaded;

        rb = GetComponent<Rigidbody2D>();

        currentState = State.hide;

        wakeUpTimer.Initalize(wakeUpTime, OnWakeUp);
        wanderTimer.Initalize(wanderTime + Random.Range(0f, 4f), OnEndWander);
    }

    private void Update()
    {
        StateController();
    }
    #endregion

    #region Event Methods
    private void OnSceneUnloaded(Scene scene)
    {
        if (GameController.instance != null)
        {
            GameController.instance.GetComponent<AlienManager>().SetRemainingTime(saveDataInstance, lifeTimeTimer.remainingTime);
        }
    }

    private void Dead()
    {
        Destroy(gameObject);
        eventManager.Publish(EventType.KillAlien, saveDataInstance);
    }

    private void OnWakeUp()
    {
        isAwake = true;
    }

    private void OnEndWander()
    {
        isFleeing = true;
    }
    #endregion

    #region Methods
    public void Initialize(AlienSaveData saveDataInstance, float lifeTime)
    {
        this.saveDataInstance = saveDataInstance;
        this.lifeTime = lifeTime;

        lifeTimeTimer.Initalize(this.lifeTime, Dead);
        lifeTimeTimer.Run();
    }

    private void StateController()
    {
        if (isFleeing)
        {
            currentState = State.flee;
        }
        else if (isAwake)
        {
            if (PlayerCollisionCheck(attackRadius))
            {
                currentState = State.attack;
            }
            else if (!isWandering && !arrivedAtTarget)
            {
                // Move to chasing
                currentState = State.chase;
            }
            else if (arrivedAtTarget && PlayerCollisionCheck(attackRadius))
            {
                // Successfully attacked the player
                currentState = State.attack;
            }
            else if (arrivedAtTarget && !PlayerCollisionCheck(attackRadius))
            {
                // Player is not in sight
                currentState = State.wander;
                isWandering = true;
            }
        }
        else
        {
            // Default state
            currentState = State.hide;

            if (!wakeUpTimer.isRunning && PlayerCollisionCheck(detectionRadius))
            {
                wakeUpTimer.Run();
            }
        }

        StateManager();
    }

    private void StateManager()
    {
        switch (currentState)
        {
            case State.hide:

                direction = Vector2.zero;
                rb.linearVelocity = Vector2.zero;

                break;

            case State.chase:

                // Move to player
                MoveToTarget(GameController.instance.GetComponent<GameController>().GetPlayerPos());

                break;

            case State.attack:

                DealDamage();

                direction = Vector2.zero;
                rb.linearVelocity = Vector2.zero;

                if (!isAttacking)
                {
                    eventManager.Publish(EventType.AttackPlayer, gameObject);
                    isAttacking = true;
                    isFleeing = true;
                }

                // Move to flee position after attacking
                target = GameController.instance.GetComponent<AlienManager>().HunterFleePos();

                MoveToTarget(target);

                break;

            case State.wander:

                if (arrivedAtTarget)
                {
                    arrivedAtTarget = false;
                    target = GetRandomTarget();
                }

                MoveToTarget(target);
                
                if (!wanderTimer.isRunning) wanderTimer.Run();

                break;

            case State.flee:

                // Move to flee position
                target = GameController.instance.GetComponent<AlienManager>().HunterFleePos();

                MoveToTarget(target);

                break;
        }
    }

    private void MoveToTarget(Vector2 target)
    {
        gizmosTarget = target;
        // X Direction
        if (Mathf.Abs(target.x - transform.position.x) > 0.3)
        {
            // Lerp direction from wherever it is to the dircetion vector between target and current pos
            direction.x = Mathf.Lerp(direction.x, target.x - transform.position.x, t.x);
        }
        else
        {
            // When close enough to target position

            // Turn off x direction
            direction.x = 0f;

            // Lerp linear velocity to 0
            rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocityX, 0f, VelocityT), rb.linearVelocityY);
        }

        // Y Direction
        if (Mathf.Abs(target.y - transform.position.y) > 0.3)
        {
            // Lerp direction from wherever it is to the dircetion vector between target and current pos
            direction.y = Mathf.Lerp(direction.y, target.y - transform.position.y, t.y);
        }
        else
        {
            // When close enough to target position

            // Turn off y direction
            direction.y = 0f;

            // Lerp linear velocity to 0
            rb.linearVelocity = new Vector2(rb.linearVelocityX, Mathf.Lerp(rb.linearVelocityY, 0f, VelocityT));
        }

        if(currentState == State.wander && Mathf.Abs((target - (Vector2)transform.position).magnitude) < 0.5)
        {
            // Arrived at target
            arrivedAtTarget = true;
        }

        if (currentState == State.chase && Mathf.Abs((target - (Vector2)transform.position).magnitude) < 0.3 && Mathf.Abs(rb.linearVelocity.magnitude) <= 0.5)
        {
            // Arrived at target
            direction = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
            arrivedAtTarget = true;
        }

        if(currentState == State.flee && Mathf.Abs((target - (Vector2)transform.position).magnitude) < 0.5)
        {
            direction = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
            arrivedAtTarget = true;
            lifeTimeTimer.Pause();
        }
                
        Move(direction, speed, maxSpeed);
    }

    private void Move(Vector2 direction, Vector2 speed, Vector2 maxSpeed)
    {
        // Applies an impulse force to the rigidbody
        rb.AddForce(direction * speed * TimeManager.deltaTime, ForceMode2D.Impulse);

        // Truncates velocity to match the maximum velocity variable
        if (Mathf.Abs(rb.linearVelocityX) > maxSpeed.x) rb.linearVelocityX = Mathf.Sign(rb.linearVelocityX) * maxSpeed.x;

        if (Mathf.Abs(rb.linearVelocityY) > maxSpeed.y) rb.linearVelocityY = Mathf.Sign(rb.linearVelocityY) * maxSpeed.y;
    }

    private bool PlayerCollisionCheck(float radius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach(Collider2D obj in colliders)
        {
            if(obj.gameObject.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }

    private Vector2 GetRandomTarget()
    {
        return GameController.instance.GetComponent<AlienManager>().RandomHunterSpawnPoint();
    }

    private void DealDamage()
    {
        Vector2Int damage = GameController.instance.GetComponent<AlienManager>().HunterDamageRange();
        GameController.instance.GetComponent<HealthManager>().SubtractHealth(Random.Range(damage.x, damage.y + 1));
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(gizmosTarget, 0.3f);
    }
    #endregion
}