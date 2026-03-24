using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ScrapSpawnController : MonoBehaviour
{
    [SerializeField] private Vector2Int numOfScrapRange;
    [SerializeField] GameObject scrapPrefab;
    [SerializeField] List<ScrapData> lowTierScrapData;
    [SerializeField] List<ScrapData> medTierScrapData;
    [SerializeField] List<ScrapData> highTierScrapData;

    private int scrapLeftToSpawn;

    private void Start()
    {
        scrapLeftToSpawn = Random.Range(numOfScrapRange.x, numOfScrapRange.y);
    }

    public void SpawnScrap(ValueTier tier)
    {
        while(scrapLeftToSpawn > 0)
        {
            Vector2 randomPoint = RandomPointInScene();

            if(CanSpawnAtPoint(randomPoint))
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
                        scrapObject.GetComponent<ScrapScript>().Initialize(lowTierScrapData[Random.Range(0, lowTierScrapData.Count)]);
                        break;

                    case ValueTier.medium:
                        scrapObject.GetComponent<ScrapScript>().Initialize(medTierScrapData[Random.Range(0, medTierScrapData.Count)]);
                        break;

                    case ValueTier.high:
                        scrapObject.GetComponent<ScrapScript>().Initialize(highTierScrapData[Random.Range(0, highTierScrapData.Count)]);
                        break;
                }

                scrapLeftToSpawn--;
            }
        }
    }

    public void SpawnExistingScrap(List<ScrapSaveData> scrapObjects)
    {
        foreach(ScrapSaveData obj in scrapObjects)
        {
            GameObject scrapObject = Instantiate(scrapPrefab, obj.position, Quaternion.Euler(obj.eulerRotation));

            scrapObject.GetComponent<ScrapScript>().value = obj.value;
            scrapObject.GetComponent<ScrapScript>().SetScale(obj.scrapData.scale);
            scrapObject.GetComponent<ScrapScript>().SetSprite(obj.scrapData.sprite);
        }
    }

    private Vector2 RandomPointInScene()
    {
        float worldHeight = Camera.main.orthographicSize * 2;
        float worldWidth = worldHeight * Camera.main.aspect;

        float x = Random.Range(-worldWidth / 2, worldWidth / 2);
        float y = Random.Range(-worldHeight / 2, worldHeight / 2);

        return new Vector2(x, y);
    }

    private bool CanSpawnAtPoint(Vector2 coordinate)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(coordinate, 0.01f);

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
}
