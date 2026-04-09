using UnityEngine;


public class ScrapScript : MonoBehaviour
{
    public int value { get; set; }

    public void Initialize(TierData tier, float maxScale, Sprite sprite)
    {
        value = Random.Range(tier.minValue, tier.maxValue);

        // Larger scale = larger value
        float scale = (value / (tier.maxValue - tier.minValue)) + 1;
        SetScale(scale);

        SetSprite(sprite);
    }

    private void SetScale(float scale)
    {
        transform.localScale *= scale;
    }
    
    public void SetSprite(Sprite sprite)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
}
