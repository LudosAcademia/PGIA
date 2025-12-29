using UnityEngine;

public class PreviewSystem : MonoBehaviour
{

    [SerializeField] private float previewYOffset = 0.06f;
    [SerializeField] private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField] private Material previewMaterialsPrefab;
    private Material previewMaterialInstance;

    private Renderer cellIndicatorRenderer;

    private void Awake()
    {
        previewMaterialInstance = new Material(previewMaterialsPrefab);
        cellIndicator.gameObject.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartShowingCursor(Vector2Int size)
    {
        PrepareCursor(size);
        cellIndicator.SetActive(true);
    }

    public void UpdateCursor(Vector3 position)
    {
        MoveCursor(position);
    }

    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }

    public void SetCursorColor(Color color)
    {
        cellIndicatorRenderer.material.color = color;
    }


    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position;
    }


}
