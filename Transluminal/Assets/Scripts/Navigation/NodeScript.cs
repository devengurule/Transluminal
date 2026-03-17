using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class NodeScript : MonoBehaviour
{
    #region Variables
    [SerializeField] private bool isHomeNode;
    [SerializeField] private SceneAsset targetShipScene;
    #endregion

    private void Start()
    {
        if (isHomeNode)
        {
            GetComponent<Image>().color = Color.rebeccaPurple;
        }
    }

    #region Methods
    public bool IsHomeNode()
    {
        return isHomeNode;
    }

    public SceneAsset TargetShipScene()
    {
        return targetShipScene;
    }
    #endregion
}
