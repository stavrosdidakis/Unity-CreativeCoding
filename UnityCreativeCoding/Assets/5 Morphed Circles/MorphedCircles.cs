using UnityEngine;

public class MorphedCircles : MonoBehaviour
{
    public int numberOfLines = 5;
    public int pointsPerLine = 200;
    public float radius = 3.0f;
    public float noiseScale = 0.5f;
    public float noiseSpeed = 1.0f;
    public float lineWidth = 0.1f;
    public Material[] lineMaterials; // Array of materials/shaders for each line

    private LineRenderer[] lineRenderers;

    void Start()
    {
        // Set the parent GameObject's initial position
        transform.position = new Vector3(0, 0, 20);

        InitializeLineRenderers();
    }

    void Update()
    {
        float time = Time.time * noiseSpeed;

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
            lineObj.transform.localPosition = Vector3.zero; // Ensure the line object has no offset

            LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.positionCount = pointsPerLine;
            lineRenderer.material = lineMaterials[i % lineMaterials.Length]; // Assign material from the array
            lineRenderer.widthCurve = new AnimationCurve(new Keyframe(0, lineWidth), new Keyframe(1, lineWidth));
            lineRenderer.useWorldSpace = false;
            lineRenderers[i] = lineRenderer;
        }
    }

    void UpdateLinePositions(int lineIndex, float time)
    {
        Vector3[] positions = new Vector3[pointsPerLine];
        float offset = (float)lineIndex / numberOfLines * Mathf.PI * 2;

        for (int i = 0; i < pointsPerLine; i++)
        {
            float t = (float)i / (pointsPerLine - 1);
            float angle = t * Mathf.PI * 2;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            float z = 0;

            // Apply Perlin noise for morphing effect
            float noiseX = Mathf.PerlinNoise(x * noiseScale + time + offset, y * noiseScale + time + offset) * 2 - 1;
            float noiseY = Mathf.PerlinNoise(x * noiseScale - time + offset, y * noiseScale - time + offset) * 2 - 1;

            positions[i] = new Vector3(x + noiseX, y + noiseY, z);
        }

        lineRenderers[lineIndex].SetPositions(positions);
    }
}
