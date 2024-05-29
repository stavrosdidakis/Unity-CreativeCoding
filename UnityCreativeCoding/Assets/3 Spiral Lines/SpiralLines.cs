using UnityEngine;

public class SpiralLines : MonoBehaviour
{
    public int numberOfLines = 20;
    public int pointsPerLine = 200;
    public Material lineMaterial;
    public Gradient lineColorGradient;
    public float lineSpacing = 0.5f;
    public float baseAmplitude = 0.5f;
    public float baseFrequency = 1.0f;
    public float noiseScale = 0.1f;
    public float noiseIntensity = 0.5f;
    public float phaseShift = 0.2f;

    private LineRenderer[] lineRenderers;
    private float[] initialPhases;

    void Start()
    {
        // Set the parent GameObject's initial position
        transform.position = new Vector3(0, -5, 12);

        lineRenderers = new LineRenderer[numberOfLines];
        initialPhases = new float[numberOfLines];

        for (int i = 0; i < numberOfLines; i++)
        {
            GameObject lineObj = new GameObject("Line" + i);
            lineObj.transform.parent = transform;
            lineObj.transform.localPosition = Vector3.zero; // Ensure the line object has no offset

            LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.positionCount = pointsPerLine;
            lineRenderer.material = lineMaterial;
            lineRenderer.colorGradient = lineColorGradient;
            lineRenderer.useWorldSpace = false;
            lineRenderers[i] = lineRenderer;

            initialPhases[i] = Random.Range(0f, Mathf.PI * 2);
        }
    }

    void Update()
    {
        float time = Time.time;

        for (int i = 0; i < numberOfLines; i++)
        {
            UpdateLinePositions(i, time);
        }
    }

    void UpdateLinePositions(int lineIndex, float time)
    {
        Vector3[] positions = new Vector3[pointsPerLine];
        float phase = initialPhases[lineIndex] + lineIndex * phaseShift;

        for (int i = 0; i < pointsPerLine; i++)
        {
            float t = i / (float)(pointsPerLine - 1);
            positions[i] = GetSpiralPosition(t, phase, time, lineIndex);
        }

        lineRenderers[lineIndex].widthCurve = new AnimationCurve(
            new Keyframe(0, 0.02f),
            new Keyframe(0.5f, 0.05f),
            new Keyframe(1, 0.02f)
        );

        lineRenderers[lineIndex].SetPositions(positions);
    }

    Vector3 GetSpiralPosition(float t, float phase, float time, int lineIndex)
    {
        float angle = t * Mathf.PI * 4 + phase + time * baseFrequency;
        float radius = t * 5;
        float x = radius * Mathf.Cos(angle);
        float y = lineIndex * lineSpacing;
        float z = radius * Mathf.Sin(angle);

        float noise = Mathf.PerlinNoise(t * noiseScale, lineIndex * noiseScale) * noiseIntensity;
        y += Mathf.Sin(time * baseFrequency + t * Mathf.PI * 2) * baseAmplitude + noise;

        return new Vector3(x, y, z);
    }
}
