using UnityEngine;

[CreateAssetMenu(fileName = "TierData", menuName = "Scriptable Objects/TierData")]
public class TierData : ScriptableObject
{
    public string tier;
    public int minValue;
    public int maxValue;
}
