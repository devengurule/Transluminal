using UnityEngine;
using UnityEngine.UI;

public class LineScript : MonoBehaviour
{
    #region Variables
    private RectTransform line;
    private float distance;
    #endregion

    #region Unity Methods
    private void Start()
    {
        // Get line component
        line = GetComponent<RectTransform>();
        StopDrawingLine();
    }
    #endregion

    #region Methods
    public float DrawLine(Vector2 start, Vector2 end)
    {
        // line is visible
        GetComponent<Image>().enabled = true;

        // get direction and length of line
        Vector2 direction = end - start;
        distance = direction.magnitude;

        // set the line object inbetween two points
        line.anchoredPosition = start + direction / 2f;

        // set the correct length of the line
        line.sizeDelta = new Vector2(distance, line.sizeDelta.y);

        // calculate the anlge of the line from the direction vector
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // set the lines angle
        line.localRotation = Quaternion.Euler(0, 0, angle);

        return distance;
    }

    public void StopDrawingLine()
    {
        GetComponent<Image>().enabled = false;
    }

    public Vector2 GetLocalPosition(RectTransform node)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, node.position);

        RectTransform parentTransform = GetComponent<RectTransform>().parent.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentTransform, screenPoint, null, out Vector2 localPoint);

        return localPoint;
    }
    #endregion
}
