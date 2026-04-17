using UnityEngine;
using UnityEngine.SceneManagement;

public class HunterScript : MonoBehaviour
{
    [SerializeField] private Vector2 speed;
    [SerializeField] private Vector2 maxSpeed;
    [SerializeField] private float XT;
    [SerializeField] private float YT;
    [SerializeField] private float VelocityT;

    private AlienSaveData saveDataInstance;
    private EventManager eventManager;
    private Timer timer;
    private Rigidbody2D rb;
    private float lifeTime;
    private Vector2 direction;
    private Vector2 target;
    private State currentState;
    private enum State
    {
        hide,
        chase,
        attack,
        wander,
        flee
    }

    private void Awake()
    {
        timer = gameObject.AddComponent<Timer>();
    }

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        SceneManager.sceneUnloaded += OnSceneUnloaded;

        rb = GetComponent<Rigidbody2D>();

        currentState = State.chase;

        if(eventManager != null)
        {
        }
    }

    private void Update()
    {
        AILogic();
    }

    private void OnDestroy()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;

        if (eventManager != null)
        {
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (GameController.instance != null)
        {
            GameController.instance.GetComponent<AlienManager>().SetRemainingTime(saveDataInstance, timer.remainingTime);
        }
    }

    private void Dead()
    {
        Destroy(gameObject);
    }

    public void Initialize(AlienSaveData saveDataInstance, float lifeTime)
    {
        this.saveDataInstance = saveDataInstance;
        this.lifeTime = lifeTime;

        timer.Initalize(this.lifeTime, Dead);
        timer.Run();
    }

    private void AILogic()
    {
        switch (currentState)
        {
            case State.hide:



                break;

            case State.chase:

                // Move to player
                MoveToTarget(GameController.instance.GetComponent<GameController>().GetPlayerPos());

                break;

            case State.attack:



                break;

            case State.wander:

                print("Wandering");

                break;

            case State.flee:

                // Move to flee position
                target = GameController.instance.GetComponent<GameController>().HunterFleePos();

                MoveToTarget(target);

                break;
        }
    }

    private void MoveToTarget(Vector2 target)
    {
        // X Direction
        if (Mathf.Abs(target.x - transform.position.x) > 0.5)
        {
            // Lerp direction from wherever it is to the dircetion vector between target and current pos
            direction.x = Mathf.Lerp(direction.x, target.x - transform.position.x, XT);
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
        if (Mathf.Abs(target.y - transform.position.y) > 0.5)
        {
            // Lerp direction from wherever it is to the dircetion vector between target and current pos
            direction.y = Mathf.Lerp(direction.y, target.y - transform.position.y, YT);
        }
        else
        {
            // When close enough to target position

            // Turn off y direction
            direction.y = 0f;

            // Lerp linear velocity to 0
            rb.linearVelocity = new Vector2(rb.linearVelocityX, Mathf.Lerp(rb.linearVelocityY, 0f, VelocityT));
        }

        if (Mathf.Abs((target - (Vector2)transform.position).magnitude) < 0.5 && Mathf.Abs(rb.linearVelocity.magnitude) <= 0.1)
        {
            if(currentState == State.chase)
            {
                currentState = State.wander;
            }
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
}
