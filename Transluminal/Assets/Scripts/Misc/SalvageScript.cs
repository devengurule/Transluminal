using UnityEngine;
using UnityEngine.UI;

public class SalvageScript : MonoBehaviour
{
    private SalvageData salvageData;
    private AlienData alienData;
    public int value {  get; set; }

    public void Initialize(TierData tier, SalvageData salvageData, AlienData alienData, Vector2 scaleRange)
    {
        // Save Salvage and Alien Data
        this.salvageData = salvageData;
        this.alienData = alienData;

        value = Random.Range(tier.minValue, tier.maxValue);
        GetComponent<SpriteRenderer>().sprite = salvageData.sprite;

        // Larger scale = larger value
        float a = (float)(value - tier.minValue) / (float)(tier.maxValue - tier.minValue);
        float b = a * (scaleRange.y - scaleRange.x);

        float scale = b + scaleRange.x;

        SetScale(scale);
    }

    public SalvageData GetSalvageData()
    {
        return salvageData;
    }

    public AlienData GetAlienData()
    {
        return alienData;
    }

    public void SetAlienData(AlienData alienData)
    {
        this.alienData = alienData;
    }
    
    public void SetSalvageData(SalvageData salvageData)
    {
        this.salvageData = salvageData;
    }
    
    public void SetScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
    }
}
