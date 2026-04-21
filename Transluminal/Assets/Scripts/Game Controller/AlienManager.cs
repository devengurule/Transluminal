using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AlienManager : MonoBehaviour
{
    [Header("Hunter")]
    [SerializeField] private GameObject hunterPrefab;
    [SerializeField] private float hunterLifeTime;
    [SerializeField] private Vector2Int hunterDamageRange;
    [SerializeField] private Vector2 hunterFleePos;

    [Header("Rat")]
    [SerializeField] private GameObject ratPrefab;
    [SerializeField] private float ratLifeTime;
    [SerializeField] private Vector2 numOfRat;

    private GameObject spawnZone;
    private GameObject ratSpawnZone;
    private List<AlienSaveData> alienSaveList = new List<AlienSaveData>();
    private EventManager eventManager;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        spawnZone = GameObject.FindGameObjectWithTag("SpawnZone");

        // Subscribe to active scene change event
        SceneManager.activeSceneChanged += SceneChange;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.SpawnHunter, QueueHunterSpawn);
            eventManager.Subscribe(EventType.SpawnRat, QueueRatSpawn);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to active scene change event
        SceneManager.activeSceneChanged -= SceneChange;

        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.SpawnHunter, QueueHunterSpawn);
            eventManager.Unsubscribe(EventType.SpawnRat, QueueRatSpawn);
        }
    }

    private void SceneChange(Scene current, Scene next)
    {
        spawnZone = GameObject.FindGameObjectWithTag("SpawnZone");
        ratSpawnZone = GameObject.FindGameObjectWithTag("RatSpawnZone");

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
    private void QueueRatSpawn(object target)
    {
        AlienSaveData data = new();

        data.position = Vector2.zero;
        data.prefabObject = ratPrefab;
        data.alienType = AlienType.rat;

        string floor = GetRandomFloor();
        while(floor == "Floor1Scene") floor = GetRandomFloor();

        data.currentFloor = floor;
        data.remainingLifeTime = 0;

        print(data.currentFloor);

        alienSaveList.Add(data);
    }

    public Vector2 RandomSpawnPoint(GameObject spawnZone)
    {
        Vector3 pos = spawnZone.transform.position;
        Renderer renderer = spawnZone.GetComponent<Renderer>();

        float x = Random.Range(pos.x - renderer.bounds.size.x / 2, pos.x + renderer.bounds.size.x / 2);
        float y = Random.Range(pos.y - renderer.bounds.size.y / 2, pos.y + renderer.bounds.size.y / 2);

        return new Vector2(x, y);
    }

    public Vector2 RandomHunterSpawnPoint()
    {
        return RandomSpawnPoint(spawnZone);
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
        if (data.alienType == AlienType.hunter)
        {
            Vector2 randomPos = RandomSpawnPoint(spawnZone);
            GameObject alien = Instantiate(data.prefabObject, randomPos, Quaternion.identity);

            data.position = randomPos;

            alien.GetComponent<HunterScript>().Initialize(data, data.remainingLifeTime);
        }
        else if(data.alienType == AlienType.rat)
        {
            for (int i = 0; i < Random.Range(numOfRat.x, numOfRat.y); i++)
            {
                Instantiate(data.prefabObject, RandomSpawnPoint(ratSpawnZone), Quaternion.identity);
            }
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

    public Vector2 HunterFleePos()
    {
        return hunterFleePos;
    }

    public Vector2Int HunterDamageRange()
    {
        return hunterDamageRange;
    }

    #region Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hunterFleePos, 0.5f);
    }
    #endregion
}