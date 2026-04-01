using UnityEngine;

public class SalvageScript : MonoBehaviour
{
    private SalvageData salvageData;
    public int value {  get; set; }

    private SalvageType salvageType;

    public void Initialize(SalvageData data)
    {
        SalvageType[] types = (SalvageType[])System.Enum.GetValues(typeof(SalvageType));
        
        salvageType = types[Random.Range(0, types.Length)];

        salvageData = data;

        value = Random.Range(salvageData.tier.minValue, salvageData.tier.maxValue + 1);

        transform.localScale *= salvageData.scale + Random.Range(0, 0.2f);
    }

    public SalvageData GetSalvageData()
    {
        return salvageData;
    }

    public SalvageType GetSalvageType()
    {
        return salvageType;
    }

    public void SetScale(float scale)
    {
        transform.localScale *= scale;
    }

    public void SetSprite(Sprite sprite)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void SetSalvageType(SalvageType type)
    {
        salvageType = type;
    }
}
