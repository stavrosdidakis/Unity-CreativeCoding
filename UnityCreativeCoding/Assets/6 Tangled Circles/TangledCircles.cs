using UnityEngine;
using System.Collections.Generic;

public class TangledCircles : MonoBehaviour
{
    public int numberOfLines = 100;
    public int pointsPerLine = 500;
    public float lineRadius = 5.0f;
    public float noiseScale = 0.1f;
    public float noiseSpeed = 0.01f;
    public Material lineMaterial;

    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private float[] noiseOffsets;

    void Start()
    {
        noiseOffsets = new float[numberOfLines];
        for (int i = 0; i < numberOfLines; i++)
        {
            noiseOffsets[i] = Random.Range(0f, 1000f);
            
            GameObject lineObj = new GameObject("Line" + i);
            lineObj.transform.parent = transform;

            LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.positionCount = pointsPerLine;

            // Create a new material instance and assign it a random color
            Material newMaterial = new Material(lineMaterial);
            Color randomColor = new Color(Random.value, Random.value, Random.value);
            newMaterial.color = randomColor;
            lineRenderer.material = newMaterial;

            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.useWorldSpace = true;

            lineRenderers.Add(lineRenderer);
        }

        GenerateTangledLines();
    }

    void Update()
    {
        GenerateTangledLines();
    }

    void GenerateTangledLines()
    {
        for (int i = 0; i < numberOfLines; i++)
        {
            Vector3[] positions = new Vector3[pointsPerLine];
            Vector3 center = transform.position;

            for (int j = 0; j < pointsPerLine; j++)
            {
                float t = (float)j / (pointsPerLine - 1);
                float theta = t * Mathf.PI * 2;

                float x = Mathf.Cos(theta) * lineRadius;
                float y = Mathf.Sin(theta) * lineRadius;
                float z = 0;

                float noiseX = Mathf.PerlinNoise(x * noiseScale + noiseOffsets[i], y * noiseScale + Time.time * noiseSpeed);
                float noiseY = Mathf.PerlinNoise(x * noiseScale + noiseOffsets[i] + 100f, y * noiseScale + Time.time * noiseSpeed + 100f);
                float noiseZ = Mathf.PerlinNoise(x * noiseScale + noiseOffsets[i] + 200f, y * noiseScale + Time.time * noiseSpeed + 200f);

                positions[j] = center + new Vector3(x + (noiseX - 0.5f) * lineRadius, y + (noiseY - 0.5f) * lineRadius, (noiseZ - 0.5f) * lineRadius);
            }

            lineRenderers[i].SetPositions(positions);
        }
    }
}
