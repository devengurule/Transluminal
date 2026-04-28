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

    [Header("Creature")]
    [SerializeField] private GameObject creaturePrefab;
    [SerializeField] private Vector2 spawnOffsetTimeRange;
    [SerializeField] private Vector2 creatureSpawnPos;
    [SerializeField] private int maxNumberOfCreature;

    private Timer creatureSpawnTimer;
    private int currentNumberOfCreature;
    private GameObject spawnZone;
    private GameObject ratSpawnZone;
    private List<AlienSaveData> alienSaveList = new List<AlienSaveData>();
    private EventManager eventManager;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        creatureSpawnTimer = gameObject.AddComponent<Timer>();
        creatureSpawnTimer.Initalize(Random.Range(spawnOffsetTimeRange.x, spawnOffsetTimeRange.y), QueueCreatureSpawn, true);
     
        spawnZone = GameObject.FindGameObjectWithTag("SpawnZone");

        // Subscribe to active scene change event
        SceneManager.activeSceneChanged += SceneChange;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.SpawnHunter, QueueHunterSpawn);
            eventManager.Subscribe(EventType.SpawnRat, QueueRatSpawn);
            eventManager.Subscribe(EventType.KillAlien, RemoveAlienFromList);
            eventManager.Subscribe(EventType.AddCreature, AddCreature);

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
            eventManager.Unsubscribe(EventType.KillAlien, RemoveAlienFromList);
            eventManager.Unsubscribe(EventType.AddCreature, AddCreature);
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
                    if(data.alienType == AlienType.creature)
                    {
                        SpawnAlien(data);
                        alienSaveList[i] = data;
                    }
                    else if (data.currentFloor == "Floor1Scene")
                    {
                        SpawnAlien(data);
                        alienSaveList[i] = data;
                    }
                    break;

                case "Floor2Scene":
                    if (data.alienType == AlienType.creature)
                    {
                        SpawnAlien(data);
                        alienSaveList[i] = data;
                    }
                    else if (data.currentFloor == "Floor2Scene")
                    {
                        SpawnAlien(data);
                        alienSaveList[i] = data;
                    }
                    break;

                case "Floor3Scene":
                    if (data.alienType == AlienType.creature)
                    {
                        SpawnAlien(data);
                        alienSaveList[i] = data;
                    }
                    else if (data.currentFloor == "Floor3Scene")
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

        print($"Hunter: {data.currentFloor}");

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

        print($"Rat: {data.currentFloor}");

        alienSaveList.Add(data);
    }

    private void QueueCreatureSpawn()
    {
        AlienSaveData data = new();

        data.position = creatureSpawnPos;
        data.prefabObject = creaturePrefab;
        data.alienType = AlienType.creature;
        data.currentFloor = "";
        data.remainingLifeTime = 0;
        alienSaveList.Add(data);

        if(GameObject.FindGameObjectsWithTag("Player").Length > 0)
        {
            eventManager.Publish(EventType.SpawnCreature);
            SpawnAlien(data);
        }
    }

    private void RemoveAlienFromList(object target)
    {
        if(target is AlienSaveData alienData)
        {
            foreach(AlienSaveData data in alienSaveList)
            {
                if(data == alienData)
                {
                    alienSaveList.Remove(data);
                    return;
                }
            }
        }
    }

    private void AddCreature(object target)
    {
        currentNumberOfCreature++;

        if (currentNumberOfCreature >= maxNumberOfCreature)
        {
            creatureSpawnTimer.Run();
        }
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
        else if (data.alienType == AlienType.rat)
        {
            for (int i = 0; i < Random.Range(numOfRat.x, numOfRat.y); i++)
            {
                GameObject alien = Instantiate(data.prefabObject, RandomSpawnPoint(ratSpawnZone), Quaternion.identity);
                alien.GetComponent<RatScript>().Initialize(data);
            }
        }
        else if (data.alienType == AlienType.creature)
        {
            GameObject alien = Instantiate(data.prefabObject, data.position, Quaternion.identity);

            data.position = hunterFleePos;

            alien.GetComponent<CreatureScript>().Initialize(data);
        }
    }

    public void SetRemainingTime(AlienSaveData data, float remainingTime)
    {
        alienSaveList[FindSaveDataIndex(data)].remainingLifeTime = remainingTime;
    }

    public void SetCreaturePosition(AlienSaveData data, Vector2 position)
    {
        alienSaveList[FindSaveDataIndex(data)].position = position;
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

        Gizmos.color = Color.tomato;
        Gizmos.DrawWireSphere(creatureSpawnPos, 0.5f);
    }
    #endregion
}