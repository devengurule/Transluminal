using UnityEngine;

public class SalvageScript : MonoBehaviour
{
    private SalvageData salvageData;
    private AlienData alienData;
    public int value {  get; set; }

    public void Initialize(TierData tier, SalvageData salvageData, AlienData alienData, float maxScale)
    {
        // Save Salvage and Alien Data
        this.salvageData = salvageData;
        this.alienData = alienData;

        value = Random.Range(tier.minValue, tier.maxValue);

        // Larger scale = larger value
        float scale = (value / (tier.maxValue - tier.minValue)) + 1;
        SetScale(scale);

        GetComponent<SpriteRenderer>().sprite = salvageData.sprite;
    }

    public SalvageData GetSalvageData()
    {
        return salvageData;
    }

    public AlienData GetAlienData()
    {
        return alienData;
    }

    private void SetScale(float scale)
    {
        transform.localScale *= scale;
    }
}
