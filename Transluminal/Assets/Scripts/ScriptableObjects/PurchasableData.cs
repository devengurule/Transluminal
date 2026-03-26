using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "PurchasableData", menuName = "Scriptable Objects/PurchasableData")]
public class PurchasableData : ScriptableObject
{
    public string title;
    public string description;
    public int price;
    public Sprite sprite;
}
