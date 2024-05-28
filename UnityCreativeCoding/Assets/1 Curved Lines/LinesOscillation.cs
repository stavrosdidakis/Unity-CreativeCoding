using UnityEngine;

public class LinesOscillation : MonoBehaviour
{
    public int numberOfLines = 20;
    public int pointsPerLine = 200;
    public float baseAmplitude = 0.5f;
    public float baseFrequency = 1.0f;
    public float lineSpacing = 0.5f;
    public Material lineMaterial;  // Ensure this uses the RotationalHueShader_URP material
    public float phaseShift = 0.2f;
    public float amplitudeFrequency = 0.2f;
    public float amplitudeVariation = 0.3f;
    public float noiseScale = 0.1f;
    public float noiseIntensity = 0.5f;
    public float zAmplitude = 0.5f;
    public float zFrequency = 0.3f;

    private LineRenderer[] lineRenderers;
    private Vector3[][] controlPoints;

    void Start()
    {
        lineRenderers = new LineRenderer[numberOfLines];
        controlPoints = new Vector3[numberOfLines][];

        for (int i = 0; i < numberOfLines; i++)
        {
            GameObject lineObj = new GameObject("Line" + i);
            lineObj.transform.parent = transform;
            LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.positionCount = pointsPerLine;
            lineRenderer.material = lineMaterial;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.useWorldSpace = false;
            lineRenderers[i] = lineRenderer;

            GenerateControlPoints(i);
        }
    }

    void Update()
    {
        for (int i = 0; i < numberOfLines; i++)
        {
            UpdateLinePositions(i);
        }
    }

    void GenerateControlPoints(int lineIndex)
    {
        int controlPointCount = pointsPerLine / 10;
        controlPoints[lineIndex] = new Vector3[controlPointCount];

        for (int i = 0; i < controlPointCount; i++)
        {
            float x = i * 1.0f / controlPointCount * 10;
            float y = lineIndex * lineSpacing;
            controlPoints[lineIndex][i] = new Vector3(x, y, 0);
        }
    }

    void UpdateLinePositions(int lineIndex)
    {
        Vector3[] positions = new Vector3[pointsPerLine];
        float time = Time.time * baseFrequency + lineIndex * phaseShift;

        float currentAmplitude = baseAmplitude + Mathf.Sin(Time.time * amplitudeFrequency) * amplitudeVariation;

        for (int i = 0; i < pointsPerLine; i++)
        {
            float t = i * 1.0f / pointsPerLine;
            positions[i] = GetTwistedPosition(lineIndex, t, time, currentAmplitude);
        }

        lineRenderers[lineIndex].SetPositions(positions);
    }

    Vector3 GetTwistedPosition(int lineIndex, float t, float time, float currentAmplitude)
    {
        int numSections = controlPoints[lineIndex].Length - 3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * numSections), numSections - 1);
        float u = t * numSections - currPt;

        Vector3 a = controlPoints[lineIndex][currPt];
        Vector3 b = controlPoints[lineIndex][currPt + 1];
        Vector3 c = controlPoints[lineIndex][currPt + 2];
        Vector3 d = controlPoints[lineIndex][currPt + 3];

        Vector3 pos = 0.5f * (
            (-a + 3f * b - 3f * c + d) * (u * u * u) +
            (2f * a - 5f * b + 4f * c - d) * (u * u) +
            (-a + c) * u +
            2f * b
        );

        float noise = Mathf.PerlinNoise(pos.x * noiseScale, pos.y * noiseScale) * noiseIntensity;
        pos.y += Mathf.Sin(time + pos.x) * (currentAmplitude + noise); // Oscillation effect with noise

        pos.z = Mathf.Sin(time + pos.x * zFrequency) * zAmplitude; // Adding Z-axis oscillation

        return pos;
    }
}
