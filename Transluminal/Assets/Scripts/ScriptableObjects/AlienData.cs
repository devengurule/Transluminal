using UnityEngine;

[CreateAssetMenu(fileName = "AlienData", menuName = "Scriptable Objects/AlienData")]
public class AlienData : ScriptableObject
{
    public Sprite densityImage;
    public Sprite camoflaugeImage;
    public float fluidSignature;
    public FluidType fluidSignatureType;
    public AlienType alienType;
}
