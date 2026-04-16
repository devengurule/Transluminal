using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Android;
using UnityEngine.SceneManagement;
using static UnityEditor.PlayerSettings;

public class AlienManager : MonoBehaviour
{
    [Header("Hunter")]
    [SerializeField] private GameObject hunterPrefab;
    [SerializeField] private float hunterLifeTime;

    [Header("Rat")]
    [SerializeField] private GameObject ratPrefab;
    [SerializeField] private float ratLifeTime;

    private GameObject spawnZone;
    private List<AlienSaveData> alienSaveList = new List<AlienSaveData>();
    private EventManager eventManager;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        // Subscribe to active scene change event
        SceneManager.activeSceneChanged += SceneChange;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.SpawnHunter, QueueHunterSpawn);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to active scene change event
        SceneManager.activeSceneChanged -= SceneChange;

        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.SpawnHunter, QueueHunterSpawn);
        }
    }

    private void SceneChange(Scene current, Scene next)
    {
        spawnZone = GameObject.FindGameObjectWithTag("SpawnZone");

        for (int i = 0; i < alienSaveList.Count; i++)
        {
            AlienSaveData data = alienSaveList[i];

            switch (SceneController.GetCurrentSceneName())
            {
                case "Floor1Scene":
                    if (data.currentFloor == "Floor1Scene")
                    {
                        SpawnAlien(data);
                        alienSaveList[i] = data;
                    }
                    break;

                case "Floor2Scene":
                    if (data.currentFloor == "Floor2Scene")
                    {
                        SpawnAlien(data);
                        alienSaveList[i] = data;
                    }
                    break;

                case "Floor3Scene":
                    if (data.currentFloor == "Floor3Scene")
                    {
                        SpawnAlien(data);
                        alienSaveList[i] = data;
                    }
                    break;
            }
        }
    }

    private void QueueHunterSpawn(object target)
    {
        AlienSaveData data = new();

        data.position = Vector2.zero;
        data.prefabObject = hunterPrefab;
        data.alienType = AlienType.hunter;
        data.currentFloor = GetRandomFloor();
        data.remainingLifeTime = hunterLifeTime;

        print(data.currentFloor);

        alienSaveList.Add(data);
    }

    private Vector2 RandomSpawnPoint()
    {
        Vector3 pos = spawnZone.transform.position;
        Renderer renderer = spawnZone.GetComponent<Renderer>();

        float x = Random.Range(pos.x - renderer.bounds.size.x / 2, pos.x + renderer.bounds.size.x / 2);
        float y = Random.Range(pos.y - renderer.bounds.size.y / 2, pos.y + renderer.bounds.size.y / 2);

        return new Vector2(x, y);
    }

    private string GetRandomFloor()
    {
        int x = Random.Range(1, 4);

        switch (x)
        {
            case 1:
                return "Floor1Scene";
            case 2:
                return "Floor2Scene";
            case 3:
                return "Floor3Scene";
        }
        return "";
    }

    public void SetSpawnZone(GameObject spawnZone)
    {
        this.spawnZone = spawnZone;
    }

    private void SpawnAlien(AlienSaveData data)
    {
        Vector2 randomPos = RandomSpawnPoint();
        GameObject alien = Instantiate(data.prefabObject, randomPos, Quaternion.identity);

        data.position = randomPos;

        if (data.alienType == AlienType.hunter)
        {
            alien.GetComponent<HunterScript>().Initialize(data, data.remainingLifeTime);
        }
    }

    public void SetRemainingTime(AlienSaveData data, float remainingTime)
    {
        alienSaveList[FindSaveDataIndex(data)].remainingLifeTime = remainingTime;
    }

    private int FindSaveDataIndex(AlienSaveData data)
    {
        for (int i = 0; i < alienSaveList.Count; i++)
        {
            if (alienSaveList[i].position == data.position)
            {
                return i;
            }
        }

        return -1;
    }
}