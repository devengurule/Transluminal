using UnityEngine;
using UnityEngine.UI;

public class LineScript : MonoBehaviour
{
    #region Variables
    private Vector3 start;
    private Vector3 end;
    private RectTransform line;
    private float distance;
    #endregion

    #region Unity Methods
    private void Start()
    {
        // Get line component
        line = GetComponent<RectTransform>();
    }
    #endregion

    #region Methods
    public float DrawLine()
    {
        // line is visible
        GetComponent<Image>().enabled = true;

        // get direction and length of line
        Vector3 direction = end - start;
        distance = direction.magnitude;

        // set the line object inbetween two points
        line.position = start + direction / 2f;

        // set the correct length of the line
        line.sizeDelta = new Vector2(distance, line.sizeDelta.y);

        // calculate the anlge of the line from the direction vector
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // set the lines angle
        line.rotation = Quaternion.Euler(0, 0, angle);

        return distance;
    }

    public void StopDrawingLine()
    {
        GetComponent<Image>().enabled = false;
    }

    public void SetStart(Vector3 start)
    {
        this.start = start;
    }

    public void SetEnd(Vector3 end)
    {
        this.end = end;
    }

    public float GetDistance()
    {
        return distance;
    }
    #endregion
}
