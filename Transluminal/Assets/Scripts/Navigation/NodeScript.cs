using UnityEngine;

public class NodeScript : MonoBehaviour
{
    [SerializeField] private bool isHomeNode;
    [SerializeField] private string targetShipSceneName;
    [SerializeField] private ValueTier valueTier;
    [SerializeField] private float chanceForAlien;

    public bool IsHomeNode() => isHomeNode;

    public string TargetShipScene()
    {
        return targetShipSceneName;
    }

    public ValueTier ValueTier() => valueTier;

    public float ChanceForAlien() => chanceForAlien;
}