using UnityEngine;
using UnityEngine.UI;

public class LineScript : MonoBehaviour
{
    private bool drawLine;
    private Vector3 start;
    private Vector3 end;
    private RectTransform line;
    

    private void Start()
    {
        line = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (drawLine)
        {
            GetComponent<Image>().enabled = true;

            Vector3 direction = end - start;
            float distance = direction.magnitude;
            
            line.position = start + direction / 2f;

            line.sizeDelta = new Vector2(distance, line.sizeDelta.y);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            line.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            GetComponent<Image>().enabled = false;
        }
    }

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
}
