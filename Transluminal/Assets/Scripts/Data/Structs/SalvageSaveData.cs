using UnityEngine;

public struct SalvageSaveData
{
    // Transform data
    public Vector2 position;
    public Vector3 eulerRotation;
    public float scale;

    // Other data
    public int value;
    public AlienData alienData;
    public SalvageData salvageData;
}
