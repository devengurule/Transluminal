using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class NodeScript : MonoBehaviour
{
    [SerializeField] private bool isHomeNode;

#if UNITY_EDITOR
    [SerializeField] private SceneAsset targetShipScene;
#endif

    [SerializeField] private string targetShipSceneName;

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (targetShipScene != null)
        {
            targetShipSceneName = targetShipScene.name;
        }
#endif
    }

    private void Start()
    {
        if (isHomeNode)
        {
            GetComponent<Image>().color = Color.rebeccaPurple;
        }
    }

    public bool IsHomeNode()
    {
        return isHomeNode;
    }

    public string TargetShipScene()
    {
        return targetShipSceneName;
    }
}