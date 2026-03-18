using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ScrapSpawnController : MonoBehaviour
{
    [SerializeField] private int numOfScrap;
    [SerializeField] GameObject scrapPrefab;
    private int scrapLeftToSpawn;

    private void Start()
    {
        scrapLeftToSpawn = numOfScrap;
    }

    public void SpawnScrap()
    {
        while(scrapLeftToSpawn > 0)
        {
            Vector2 randomPoint = RandomPointInScene();

            if(CanSpawnAtPoint(randomPoint))
            {
                float angle = Random.Range(0, 360);
                Instantiate(scrapPrefab, randomPoint, Quaternion.Euler(0, 0, angle));

                scrapLeftToSpawn--;
            }
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
}
