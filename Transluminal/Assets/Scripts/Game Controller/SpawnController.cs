using UnityEngine;
using System.Collections.Generic;

public class SpawnController : MonoBehaviour
{
    [Header("Scrap")]

    [SerializeField] private float maxScrapScale;
    [SerializeField] private Vector2Int numOfScrapRange;
    [SerializeField] GameObject scrapPrefab;
    [SerializeField] List<Sprite> scrapSprites;

    [SerializeField] TierData scrapLowTier;
    [SerializeField] TierData scrapMedTier;
    [SerializeField] TierData scrapHighTier;

    [Header("Salvage")]

    [SerializeField] private float maxSalvageScale;
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
            if (Random.Range(0, chanceToSpawnSalvage) >= chanceToSpawnSalvage)
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

            if(randomPoint != new Vector2(float.NegativeInfinity, float.PositiveInfinity))
            {
                // Spawn Scrap
                float angle = Random.Range(0, 360);
                GameObject scrapObject = Instantiate(scrapPrefab, randomPoint, Quaternion.Euler(0, 0, angle));

                // Set scraps designated tier values
                switch (tier)
                {
                    case ValueTier.none:
                        print("What have you done...");
                        break;

                    case ValueTier.low:
                        scrapObject.GetComponent<ScrapScript>().Initialize(scrapLowTier, maxScrapScale, scrapSprites[Random.Range(0, scrapSprites.Count - 1)]);
                        break;

                    case ValueTier.medium:
                        scrapObject.GetComponent<ScrapScript>().Initialize(scrapMedTier, maxScrapScale, scrapSprites[Random.Range(0, scrapSprites.Count - 1)]);
                        break;

                    case ValueTier.high:
                        scrapObject.GetComponent<ScrapScript>().Initialize(scrapHighTier, maxScrapScale, scrapSprites[Random.Range(0, scrapSprites.Count - 1)]);
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

            if (randomPoint != new Vector2(float.NegativeInfinity, float.PositiveInfinity))
            {

                // Spawn Salvage
                float angle = Random.Range(0, 360);
                GameObject salvageObject = Instantiate(salvagePrefab, randomPoint, Quaternion.Euler(0, 0, angle));
                AlienData alien = Random.Range(0, chanceForAlien) >= chanceForAlien ? alienArchetypes[Random.Range(0, alienArchetypes.Count - 1)] : null;
                SalvageData salvageData = salvageArchetypes[Random.Range(0, salvageArchetypes.Count - 1)];

                // Set salvages designated tier values
                switch (tier)
                {
                    case ValueTier.none:
                        print("What have you done...");
                        break;

                    case ValueTier.low:
                        salvageObject.GetComponent<SalvageScript>().Initialize(salvageLowTier, salvageData, alien, maxSalvageScale);
                        break;

                    case ValueTier.medium:
                        salvageObject.GetComponent<SalvageScript>().Initialize(salvageMedTier, salvageData, alien, maxSalvageScale);
                        break;

                    case ValueTier.high:
                        salvageObject.GetComponent<SalvageScript>().Initialize(salvageHighTier, salvageData, alien, maxSalvageScale);
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
        }
    }

    public void SpawnExistingSalvage(List<SalvageSaveData> salvageObjects)
    {
        foreach (SalvageSaveData obj in salvageObjects)
        {
            GameObject salvageObject = Instantiate(salvagePrefab, obj.position, Quaternion.Euler(obj.eulerRotation));

            salvageObject.GetComponent<SalvageScript>().value = obj.value;
            salvageObject.GetComponent<SalvageScript>().SetScale(obj.salvageData.scale);
        }
    }

    private Vector2 RandomPointInScene()
    {
        float worldHeight = Camera.main.orthographicSize * 2;
        float worldWidth = worldHeight * Camera.main.aspect;

        float x = Random.Range(-worldWidth / 2, worldWidth / 2);
        float y = Random.Range(-worldHeight / 2, worldHeight / 2);

        if(CanSpawnAtPoint(new Vector2(x, y), 0.5f))
        {
            // Only return a point to spawn at if it is possible
            return new Vector2(x, y);
        }

        // Return Junk
        return new Vector2(float.NegativeInfinity, float.PositiveInfinity);
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
        
    }
}
