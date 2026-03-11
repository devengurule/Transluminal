using UnityEngine;

public class TimeManager : MonoBehaviour
{
    #region Variables
    public static float timeScale { get; set; } = 1f;

    public static float deltaTime { get; set; }
    #endregion

    #region Unity Methods
    private void Update()
    {
        deltaTime = Time.deltaTime * timeScale;
    }
    #endregion
}
