using UnityEngine;

public class AutoResize : MonoBehaviour
{
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        AdjustSize();
    }

    void AdjustSize()
    {
        Camera mainCamera = Camera.main;
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        float rectTransformWidth = rectTransform.rect.width;
        float rectTransformHeight = rectTransform.rect.height;
        if (cameraWidth / cameraHeight < rectTransformWidth / rectTransformHeight)
        {
            this.transform.localScale = new Vector3(cameraWidth / rectTransformWidth, cameraWidth / rectTransformWidth, 1);
        } else
        {
            this.transform.localScale = Vector3.one;
        }
    }

    /*private void Update()
    {
        AdjustSize();
    }*/
}
