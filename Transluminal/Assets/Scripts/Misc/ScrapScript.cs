using UnityEngine;

public class ScrapScript : MonoBehaviour
{
    private ScrapData scrapData;
    public int value { get; set; }

    public void Initialize(ScrapData data)
    {
        scrapData = data;

        value = Random.Range(scrapData.tier.minValue, scrapData.tier.maxValue + 1);

        GetComponent<SpriteRenderer>().sprite = scrapData.sprite;
        
        transform.localScale *= scrapData.scale + Random.Range(0, 0.2f);
    }

    public ScrapData GetScrapData()
    {
        return scrapData;
    }

    public void SetScale(float scale)
    {
        transform.localScale *= scale;
    }

    public void SetSprite(Sprite sprite)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
}
