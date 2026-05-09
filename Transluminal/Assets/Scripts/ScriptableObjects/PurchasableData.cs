using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "PurchasableData", menuName = "Scriptable Objects/PurchasableData")]
public class PurchasableData : ScriptableObject
{
    public enum UpgradeType
    {
        MRScan,
        PartTypeScan,
        FasterFluidType,
        HigherAccuracyFluidType,
        FasterEngine,
        FTLEngine,
        IncreasedFuel,
        StrongerHull
    }

    public UpgradeType upgradeType;
    public string title;
    public string description;
    public int price;
    public Sprite sprite;
}
