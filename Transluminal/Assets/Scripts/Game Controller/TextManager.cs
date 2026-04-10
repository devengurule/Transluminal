using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class TextManager : MonoBehaviour
{
    [SerializeField] TextAsset file;
    private string jsonText;
    public static Dictionary<string, List<string>> text { get; private set; }

    private void Awake()
    {
        jsonText = file.text;
        text = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonText);
    }
}
