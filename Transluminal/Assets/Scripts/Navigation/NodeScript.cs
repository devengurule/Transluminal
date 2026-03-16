using UnityEditor;
using UnityEngine;

public class NodeScript : MonoBehaviour
{
    [SerializeField] private bool isHomeNode;
    [SerializeField] private SceneAsset targetShipScene;
    private string nodeName;

    private void Start()
    {
        nodeName = gameObject.name;
    }

    public bool IsHomeNode()
    {
        return isHomeNode;
    }

    public SceneAsset TargetShipScene()
    {
        return targetShipScene;
    }
}
