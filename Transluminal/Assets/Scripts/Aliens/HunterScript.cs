using UnityEngine;
using UnityEngine.SceneManagement;

public class HunterScript : MonoBehaviour
{
    private AlienSaveData saveDataInstance;
    private EventManager eventManager;
    private Timer timer;
    private float lifeTime;
    private Rigidbody2D rb;

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

        currentState = State.hide;
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



                break;
        }
    }
}
