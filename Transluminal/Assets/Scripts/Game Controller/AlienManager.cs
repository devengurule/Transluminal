using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AlienManager : MonoBehaviour
{
    [Header("Hunter")]
    [SerializeField] private GameObject hunterPrefab;
    [SerializeField] private float hunterLifeTime;

    [Header("Rat")]
    [SerializeField] private GameObject ratPrefab;
    [SerializeField] private float ratLifeTime;

    private GameObject spawnZone;
    private List<AlienSaveData> alienSaveList = new();
    private EventManager eventManager;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        // Subscribe to active scene change event
        SceneManager.activeSceneChanged += SceneChange;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.SpawnHunter, QueueHunterSpawn);
            eventManager.Subscribe(EventType.SpawnRat, QueueRatSpawn);
            eventManager.Subscribe(EventType.KillAlien, RemoveAlienFromList);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.SpawnHunter, QueueHunterSpawn);
            eventManager.Unsubscribe(EventType.SpawnRat, QueueRatSpawn);
            eventManager.Unsubscribe(EventType.KillAlien, RemoveAlienFromList);
        }
    }

    private void SceneChange(Scene current, Scene next)
    {
        spawnZone = GameObject.FindGameObjectWithTag("SpawnZone");

        if (SceneController.GetCurrentSceneName() == "Floor1Scene")
        {
            foreach (AlienSaveData data in alienSaveList)
            {
                if (data.currentFloor == "Floor1Scene")
                {
                    GameObject alien = Instantiate(data.alienObject, RandomSpawnPoint(), Quaternion.identity);
                    alien.GetComponent<AlienScript>().lifeTime = data.remainingLifeTime;
                }
            }
        }
        else if (SceneController.GetCurrentSceneName() == "Floor2Scene")
        {
            foreach (AlienSaveData data in alienSaveList)
            {
                if (data.currentFloor == "Floor2Scene")
                {
                    GameObject alien = Instantiate(data.alienObject, RandomSpawnPoint(), Quaternion.identity);
                    alien.GetComponent<AlienScript>().lifeTime = data.remainingLifeTime;
                }
            }
        }
        else if (SceneController.GetCurrentSceneName() == "Floor3Scene")
        {
            foreach (AlienSaveData data in alienSaveList)
            {
                if (data.currentFloor == "Floor3Scene")
                {
                    GameObject alien = Instantiate(data.alienObject, RandomSpawnPoint(), Quaternion.identity);
                    alien.GetComponent<AlienScript>().lifeTime = data.remainingLifeTime;
                }
            }
        }
    }

    private void QueueHunterSpawn(object target)
    {
        print("Adding Hunter");
        alienSaveList.Add(new AlienSaveData(hunterPrefab, GetFloorToSpawn(), hunterLifeTime));
    }
    private void QueueRatSpawn(object target)
    {
        print("Adding Rat");
        alienSaveList.Add(new AlienSaveData(ratPrefab, GetFloorToSpawn(), ratLifeTime));
    }

    private void RemoveAlienFromList(object target)
    {
        foreach(AlienSaveData data in alienSaveList)
        {
            if(data.remainingLifeTime <= 0)
            {
                alienSaveList.Remove(data);
            }
        }
    }

    private Vector2 RandomSpawnPoint()
    {
        Vector3 pos = spawnZone.transform.position;
        Renderer renderer = spawnZone.GetComponent<Renderer>();

        float x = Random.Range(pos.x - renderer.bounds.size.x / 2, pos.x + renderer.bounds.size.x / 2);
        float y = Random.Range(pos.y - renderer.bounds.size.y / 2, pos.y + renderer.bounds.size.y / 2);
        
        return new Vector2(x, y);
    }

    private string GetFloorToSpawn()
    {
        int x = Random.Range(1, 4);

        switch (x)
        {
            case 1:

                if(SceneController.GetCurrentSceneName() != "Floor1Scene")
                {
                    return "Floor1Scene";
                }
                else
                {
                    return "Floor2Scene";
                }

            case 2:

                if (SceneController.GetCurrentSceneName() != "Floor2Scene")
                {
                    return "Floor2Scene";
                }
                else
                {
                    return "Floor3Scene";
                }

            case 3:

                if (SceneController.GetCurrentSceneName() != "Floor3Scene")
                {
                    return "Floor3Scene";
                }
                else
                {
                    return "Floor1Scene";
                }
        }
        return "";
    }

    public void SetSpawnZone(GameObject spawnZone)
    {
        this.spawnZone = spawnZone;
    }
}
