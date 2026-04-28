using UnityEngine;

public class HighlightScript : MonoBehaviour
{
    [SerializeField] private float alphaFadeSpeed;
    private SpriteRenderer sr;
    private float currentAlpha;
    private float targetAlpha;
    private Vector4 colorVector = new Vector4(1,1,1,1);

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(currentAlpha != targetAlpha)
        {
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, alphaFadeSpeed);
            colorVector.w = currentAlpha;
            sr.color = colorVector;
        }
    }

    public void TurnOnHighLight()
    {
        targetAlpha = 1f;
    }

    public void TurnOffHighLight()
    {
        targetAlpha = 0f;
    }
}
