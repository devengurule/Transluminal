using UnityEngine;

[CreateAssetMenu(fileName = "SalvageData", menuName = "Scriptable Objects/SalvageData")]
public class SalvageData : ScriptableObject
{
    [Header("Salvage Stuff")]
    public float fluidAmount;
    public Sprite sprite;
    public Sprite densityImage;
    public Sprite deepFluidDensityImage;
    public FluidType fluidType;
    public PartType partType;
}