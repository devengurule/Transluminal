using UnityEngine;

[CreateAssetMenu(fileName = "SalvageData", menuName = "Scriptable Objects/SalvageData")]
public class SalvageData : ScriptableObject
{
    public TierData tier;
    public float scale;
    public float fluidAmount;
    public Sprite densityImage;
    public Sprite deepFluidDensityImage;
    public FluidType fluidType;
    public PartType partType;
}