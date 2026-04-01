using UnityEngine;

public class SalvageScript : MonoBehaviour
{
    private SalvageData salvageData;
    public int value {  get; set; }

    public void Initialize(SalvageData data)
    {
        salvageData = data;

        value = Random.Range(salvageData.tier.minValue, salvageData.tier.maxValue + 1);

        transform.localScale *= salvageData.scale + Random.Range(0, 0.2f);

        GetComponent<SpriteRenderer>().sprite = salvageData.sprite;
    }

    public SalvageData GetSalvageData()
    {
        return salvageData;
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
