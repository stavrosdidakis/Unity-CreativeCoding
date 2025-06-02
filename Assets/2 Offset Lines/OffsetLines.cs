using UnityEngine;

public class OffsetLines : MonoBehaviour
{
    public int numberOfLines = 20;
    public int pointsPerLine = 200;
    public Material lineMaterial; // Ensure this uses the RotationalHueShader_URP material
    public Gradient lineColorGradient;
    public float baseAmplitude = 0.5f;
    public float baseFrequency = 1.0f;
    public float lineSpacing = 0.5f;
    public float phaseShift = 0.2f;
    public float noiseScale = 0.1f;
    public float noiseIntensity = 0.5f;
    public float zAmplitude = 0.5f;
    public float zFrequency = 0.3f;

    private LineRenderer[] lineRenderers;
    private Vector3[][] controlPoints;
    private float[] randomAmplitudes;
    private float[] randomFrequencies;

    void Start()
    {
        transform.position = new Vector3(0, -5, 8);

        InitializeLineRenderers();
        GenerateAllControlPoints();
        InitializeRandomParameters();
    }

    void Update()
    {
        UpdateAllLinePositions();
    }

    void InitializeLineRenderers()
    {
        lineRenderers = new LineRenderer[numberOfLines];
        controlPoints = new Vector3[numberOfLines][];

        for (int i = 0; i < numberOfLines; i++)
        {
            GameObject lineObj = new GameObject("Line" + i);
            lineObj.transform.parent = transform;
            lineObj.transform.localPosition = Vector3.zero;

            LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.positionCount = pointsPerLine;
            lineRenderer.material = lineMaterial;
            lineRenderer.widthCurve = new AnimationCurve(new Keyframe(0, 0.05f), new Keyframe(1, 0.05f));
            lineRenderer.colorGradient = lineColorGradient;
            lineRenderer.useWorldSpace = false;
            lineRenderers[i] = lineRenderer;
        }
    }

    void GenerateAllControlPoints()
    {
        for (int i = 0; i < numberOfLines; i++)
        {
            GenerateControlPoints(i);
        }
    }

    void InitializeRandomParameters()
    {
        randomAmplitudes = new float[numberOfLines];
        randomFrequencies = new float[numberOfLines];

        for (int i = 0; i < numberOfLines; i++)
        {
            randomAmplitudes[i] = Random.Range(0.5f * baseAmplitude, 1.5f * baseAmplitude);
            randomFrequencies[i] = Random.Range(0.5f * baseFrequency, 1.5f * baseFrequency);
        }
    }

    void UpdateAllLinePositions()
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

        // Calculate wider total width beyond camera view
        float zDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        float cameraWidth = 2f * zDistance * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad) * Camera.main.aspect;
        float totalWidth = cameraWidth * 1.5f; // Multiplied to extend beyond the edges

        for (int i = 0; i < controlPointCount; i++)
        {
            float x = i * (totalWidth / controlPointCount);
            float y = lineIndex * lineSpacing;
            controlPoints[lineIndex][i] = new Vector3(x - totalWidth / 2f, y, 0);
        }
    }

    void UpdateLinePositions(int lineIndex)
    {
        Vector3[] positions = new Vector3[pointsPerLine];
        float time = Time.time * randomFrequencies[lineIndex] + lineIndex * phaseShift;

        for (int i = 0; i < pointsPerLine; i++)
        {
            float t = i * 1.0f / pointsPerLine;
            positions[i] = GetBezierPosition(lineIndex, t, time);
        }

        lineRenderers[lineIndex].SetPositions(positions);
    }

    Vector3 GetBezierPosition(int lineIndex, float t, float time)
    {
        int numSections = controlPoints[lineIndex].Length - 3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * numSections), numSections - 1);
        float u = t * numSections - currPt;

        Vector3 a = controlPoints[lineIndex][currPt];
        Vector3 b = controlPoints[lineIndex][currPt + 1];
        Vector3 c = controlPoints[lineIndex][currPt + 2];
        Vector3 d = controlPoints[lineIndex][currPt + 3];

        Vector3 pos = Mathf.Pow(1 - u, 3) * a +
                      3 * Mathf.Pow(1 - u, 2) * u * b +
                      3 * (1 - u) * Mathf.Pow(u, 2) * c +
                      Mathf.Pow(u, 3) * d;

        float noise = Mathf.PerlinNoise(pos.x * noiseScale, pos.y * noiseScale) * noiseIntensity;
        pos.y += Mathf.Sin(time + pos.x) * (randomAmplitudes[lineIndex] + noise);
        pos.x += Mathf.Sin(time + pos.y) * zAmplitude;
        pos.z = Mathf.Sin(time + pos.x * zFrequency) * zAmplitude;

        return pos;
    }
}
