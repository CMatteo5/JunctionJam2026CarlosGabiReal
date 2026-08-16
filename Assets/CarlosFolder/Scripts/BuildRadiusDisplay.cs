using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class BuildRadiusDisplay : MonoBehaviour
{
    [SerializeField] private charecter player;
    [SerializeField] private float placementRadius = 7f;
    [SerializeField] private int segments = 60;
    [SerializeField] private float lineWidth = 0.1f;

    private LineRenderer lineRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = segments;
        DrawCircle();
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.enabled = player.buildMode;
    }

    private void DrawCircle()
    {
        float angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.Deg2Rad * (angleStep * i);
            float x = Mathf.Cos(angle) * placementRadius;
            float y = Mathf.Sin(angle) * placementRadius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
}