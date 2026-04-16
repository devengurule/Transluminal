using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HunterScript : MonoBehaviour
{
    private AlienSaveData saveDataInstance;
    private float lifeTime;
    private EventManager eventManager;
    private Timer timer;

    private void Awake()
    {
        timer = gameObject.AddComponent<Timer>();
    }

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        SceneManager.sceneUnloaded += OnSceneUnloaded;

        if (eventManager != null)
        {

        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {

        }
    }

    private void Update()
    {
        print(timer.remainingTime);
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
}
