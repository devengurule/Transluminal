using UnityEngine;
using UnityEngine.SceneManagement;

public class HunterScript : MonoBehaviour
{
    [SerializeField] private Vector2 speed;
    [SerializeField] private Vector2 maxSpeed;
    
    private AlienSaveData saveDataInstance;
    private EventManager eventManager;
    private Timer timer;
    private Rigidbody2D rb;

    private float lifeTime;
    private Vector2 target;
    private Vector2 direction;

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

        target = GameController.instance.GetComponent<GameController>().HunterFleePos();

        rb = GetComponent<Rigidbody2D>();

        currentState = State.flee;
    }

    private void Update()
    {
        AILogic();
    }

    private void OnDestroy()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;

        
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


                  
                break;

            case State.attack:



                break;

            case State.wander:



                break;

            case State.flee:

                Flee();

                break;
        }
    }

    private void Flee()
    {
        direction.x = Mathf.Lerp(direction.x, target.x - transform.position.x, 0.5f);
        direction.y = Mathf.Lerp(direction.y, target.y - transform.position.y, 0.5f);
        
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
