using UnityEngine;
using System.Collections.Generic;

public class SpawnController : MonoBehaviour
{
    [Header("SpawningField")]
    [SerializeField] private Vector2 spawnOffset;
    [SerializeField] private Vector2 spawnBoundary;

    [Header("Scrap")]

    [SerializeField] private Vector2 scrapScale;
    [SerializeField] private Vector2Int numOfScrapRange;
    [SerializeField] GameObject scrapPrefab;
    [SerializeField] List<Sprite> scrapSprites;

    [SerializeField] TierData scrapLowTier;
    [SerializeField] TierData scrapMedTier;
    [SerializeField] TierData scrapHighTier;

    [Header("Salvage")]

    [SerializeField] private Vector2 salvageScale;
    [SerializeField] private float chanceToSpawnSalvage;
    [SerializeField] private Vector2Int numOfSalvageRange;
    [SerializeField] GameObject salvagePrefab;

    [SerializeField] TierData salvageLowTier;
    [SerializeField] TierData salvageMedTier;
    [SerializeField] TierData salvageHighTier;

    [SerializeField] List<AlienData> alienArchetypes;
    [SerializeField] List<SalvageData> salvageArchetypes;

    private int scrapLeftToSpawn;
    private int salvageLeftToSpawn;

    private void Start()
    {
        scrapLeftToSpawn = Random.Range(numOfScrapRange.x, numOfScrapRange.y);


        // Get initial NumOfSalvageToSpawn
        int targetNumOfSalvageToSpawn = Random.Range(numOfSalvageRange.x, numOfSalvageRange.y);
        for (int i = 0; i < targetNumOfSalvageToSpawn; i++)
        {
            // If win chance to spawn, add to number of salvage left To spawn
            if (Random.Range(0, 1) >= 1 - chanceToSpawnSalvage)
            {
                // Add Salvage to Spawn
                salvageLeftToSpawn++;
            }
        }
    }

    public void SpawnScrap(ValueTier tier)
    {
        while(scrapLeftToSpawn > 0)
        {
            Vector2 randomPoint = RandomPointInScene();

            if(randomPoint != Vector2.zero)
            {
                // Spawn Scrap
                float angle = Random.Range(0, 360);
                GameObject scrapObject = Instantiate(scrapPrefab, randomPoint, Quaternion.Euler(0, 0, angle));

                Sprite sprite = scrapSprites[Random.Range(0, scrapSprites.Count - 1)];

                // Set scraps designated tier values
                switch (tier)
                {
                    case ValueTier.none:
                        print("What have you done...");
                        break;

                    case ValueTier.low:
                        scrapObject.GetComponent<ScrapScript>().Initialize(scrapLowTier, scrapScale, sprite);
                        break;

                    case ValueTier.medium:
                        scrapObject.GetComponent<ScrapScript>().Initialize(scrapMedTier, scrapScale, sprite);
                        break;

                    case ValueTier.high:
                        scrapObject.GetComponent<ScrapScript>().Initialize(scrapHighTier, scrapScale, sprite);
                        break;
                }

                scrapLeftToSpawn--;
            }
        }
    }

    public void SpawnSalvage(ValueTier tier, float chanceForAlien)
    {
        while (salvageLeftToSpawn > 0)
        {
            Vector2 randomPoint = RandomPointInScene();

            if (randomPoint != Vector2.zero)
            {
                // Spawn Salvage
                float angle = Random.Range(0, 360);
                GameObject salvageObject = Instantiate(salvagePrefab, randomPoint, Quaternion.Euler(0, 0, angle));

                AlienData alien = Random.Range(0, 1) >= 1 - chanceForAlien ? alienArchetypes[Random.Range(0, alienArchetypes.Count)] : null;

                if (alien == null) print(1);

                int index = Random.Range(0, salvageArchetypes.Count);

                SalvageData salvageData = salvageArchetypes[index];
                // Set salvages designated tier values
                switch (tier)
                {
                    case ValueTier.none:
                        print("What have you done...");
                        break;

                    case ValueTier.low:
                        salvageObject.GetComponent<SalvageScript>().Initialize(salvageLowTier, salvageData, alien, salvageScale);
                        break;

                    case ValueTier.medium:
                        salvageObject.GetComponent<SalvageScript>().Initialize(salvageMedTier, salvageData, alien, salvageScale);
                        break;

                    case ValueTier.high:
                        salvageObject.GetComponent<SalvageScript>().Initialize(salvageHighTier, salvageData, alien, salvageScale);
                        break;
                }

                salvageLeftToSpawn--;
            }
        }
    }

    public void SpawnExistingScrap(List<ScrapSaveData> scrapObjects)
    {
        foreach(ScrapSaveData obj in scrapObjects)
        {
            GameObject scrapObject = Instantiate(scrapPrefab, obj.position, Quaternion.Euler(obj.eulerRotation));

            scrapObject.GetComponent<ScrapScript>().value = obj.value;
            scrapObject.GetComponent<ScrapScript>().SetSprite(obj.sprite);
            scrapObject.GetComponent<ScrapScript>().SetScale(obj.scale);
        }
    }

    public void SpawnExistingSalvage(List<SalvageSaveData> salvageObjects)
    {
        foreach (SalvageSaveData obj in salvageObjects)
        {
            GameObject salvageObject = Instantiate(salvagePrefab, obj.position, Quaternion.Euler(obj.eulerRotation));

            salvageObject.GetComponent<SalvageScript>().value = obj.value;
            salvageObject.GetComponent<SalvageScript>().SetScale(obj.scale);
            salvageObject.GetComponent<SalvageScript>().SetAlienData(obj.alienData);
            salvageObject.GetComponent<SalvageScript>().SetSalvageData(obj.salvageData);
        }
    }

    private Vector2 RandomPointInScene()
    {
        float worldHeight = spawnBoundary.x + spawnOffset.x;
        float worldWidth = spawnBoundary.y + spawnOffset.y;

        float x = Random.Range(spawnOffset.x - (worldWidth / 2), spawnOffset.x + (worldWidth / 2));
        float y = Random.Range(spawnOffset.y - (worldHeight / 2), spawnOffset.y + (worldHeight / 2));

        if(CanSpawnAtPoint(new Vector2(x, y), 0.5f))
        {
            // Only return a point to spawn at if it is possible
            return new Vector2(x, y);
        }

        // Return Junk
        return Vector2.zero;
    }

    private bool CanSpawnAtPoint(Vector2 coordinate, float radius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(coordinate, radius);

        if(colliders.Length > 0)
        {
            return false;
        }
        else return true;
    }

    public void ResetScrapLeftToSpawn()
    {
        scrapLeftToSpawn = Random.Range(numOfScrapRange.x, numOfScrapRange.y);
    }

    public void ResetSalvageLeftToSpawn()
    {
        salvageLeftToSpawn = 0;

        int targetNumOfSalvageToSpawn = Random.Range(numOfSalvageRange.x, numOfSalvageRange.y);
        for (int i = 0; i < targetNumOfSalvageToSpawn; i++)
        {
            // If win chance to spawn, add to number of salvage left To spawn
            if (Random.Range(0, 1) >= 1 - chanceToSpawnSalvage)
            {
                // Add Salvage to Spawn
                salvageLeftToSpawn++;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(spawnOffset, spawnBoundary);
    }
}
