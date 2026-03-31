using UnityEngine;

public class ScannerSelectorData : MonoBehaviour
{
    [SerializeField] private GameObject upSelect;
    [SerializeField] private GameObject downSelect;
    [SerializeField] private GameObject rightSelect;
    [SerializeField] private GameObject leftSelect;

    public GameObject UpSelect { get { return upSelect; } }
    public GameObject DownSelect { get {return downSelect; } }
    public GameObject LeftSelect { get {return leftSelect; } }
    public GameObject RightSelect { get {return rightSelect; } }
}
