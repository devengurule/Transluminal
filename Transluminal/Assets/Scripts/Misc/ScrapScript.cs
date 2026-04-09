using UnityEngine;
using UnityEngine.UI;


public class ScrapScript : MonoBehaviour
{
    public int value { get; set; }

    public void Initialize(TierData tier, Vector2 scaleRange, Sprite sprite)
    {
        value = Random.Range(tier.minValue, tier.maxValue);
        SetSprite(sprite);

        // Larger scale = larger value
        float a = (float)(value - tier.minValue) / (float)(tier.maxValue - tier.minValue);
        float b = a * (scaleRange.y - scaleRange.x);

        float scale = b + scaleRange.x;

        SetScale(scale);
    }

    public void SetScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
    }
    
    public void SetSprite(Sprite sprite)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
}
