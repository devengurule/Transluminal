using System.Drawing;
using UnityEngine;

[CreateAssetMenu(fileName = "ScrapData", menuName = "Scriptable Objects/ScrapData")]
public class ScrapData : ScriptableObject
{
    public TierData tier;
    public Sprite sprite;
    public int scale;
}
