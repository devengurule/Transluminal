using UnityEngine;
using UnityEngine.UI;

public class LineScript : MonoBehaviour
{
    #region Variables
    private bool drawLine;
    private Vector3 start;
    private Vector3 end;
    private RectTransform line;
    #endregion

    #region Unity Methods
    private void Start()
    {
        // Get line component
        line = GetComponent<RectTransform>();
    }

    private void Update()
    {
        // draw a line only if it is allowed
        if (drawLine)
        {
            // line is visible
            GetComponent<Image>().enabled = true;

            // get direction and length of line
            Vector3 direction = end - start;
            float distance = direction.magnitude;
            
            // set the line object inbetween two points
            line.position = start + direction / 2f;

            // set the correct length of the line
            line.sizeDelta = new Vector2(distance, line.sizeDelta.y);

            // calculate the anlge of the line from the direction vector
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // set the lines angle
            line.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // if line draw is not allowed set the line to invisible
            GetComponent<Image>().enabled = false;
        }
    }
    #endregion

    #region Methods
    public void SetDrawLine(bool drawLine)
    {
        this.drawLine = drawLine;
    }

    public void SetStart(Vector3 start)
    {
        this.start = start;
    }

    public void SetEnd(Vector3 end)
    {
        this.end = end;
    }
    #endregion
}
