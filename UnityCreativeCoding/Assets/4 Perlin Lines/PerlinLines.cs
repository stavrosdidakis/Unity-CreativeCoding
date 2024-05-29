using UnityEngine;

public class PerlinLines : MonoBehaviour
{
    public int numberOfLines = 20;
    public int pointsPerLine = 100;
    public float growthSpeed = 0.5f;
    public float maxAmplitude = 1.0f;
    public float frequency = 1.0f;
    public Material lineMaterial;
    public Gradient colorGradient;
    public float noiseScale = 0.1f;
    public float noiseIntensity = 0.5f;

    private LineRenderer[] lineRenderers;

    void Start()
    {
        InitializeLineRenderers();
    }

    void Update()
    {
        float time = Time.time;

        for (int i = 0; i < numberOfLines; i++)
        {
            UpdateLinePositions(i, time);
        }
    }

    void InitializeLineRenderers()
    {
        lineRenderers = new LineRenderer[numberOfLines];

        for (int i = 0; i < numberOfLines; i++)
        {
            GameObject lineObj = new GameObject("Line" + i);
            lineObj.transform.parent = transform;

            LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.positionCount = pointsPerLine;
            lineRenderer.material = lineMaterial;
            lineRenderer.colorGradient = colorGradient;
            lineRenderer.widthCurve = new AnimationCurve(new Keyframe(0, 0.05f), new Keyframe(1, 0.02f));
            lineRenderer.useWorldSpace = false;  // Use local space
            lineRenderers[i] = lineRenderer;
        }
    }

    void UpdateLinePositions(int lineIndex, float time)
    {
        Vector3[] positions = new Vector3[pointsPerLine];

        for (int i = 0; i < pointsPerLine; i++)
        {
            float t = (float)i / (pointsPerLine - 1);
            positions[i] = GetGrowingPosition(lineIndex, t, time);
        }

        lineRenderers[lineIndex].SetPositions(positions);
    }

    Vector3 GetGrowingPosition(int lineIndex, float t, float time)
    {
        float angle = t * Mathf.PI * 2;
        float radius = Mathf.Lerp(0, maxAmplitude, t) + Mathf.PerlinNoise(lineIndex * noiseScale, t * noiseScale + time) * noiseIntensity;
        float height = Mathf.Lerp(0, 10, t) + Mathf.Sin(time * frequency + t * Mathf.PI * 2) * maxAmplitude;

        float x = Mathf.Cos(angle + lineIndex * Mathf.PI / numberOfLines) * radius;
        float y = height;
        float z = Mathf.Sin(angle + lineIndex * Mathf.PI / numberOfLines) * radius;

        return new Vector3(x, y, z);
    }
}
