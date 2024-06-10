using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public GameObject planePrefab; // The plane prefab
    public int gridSize = 5; // The size of the grid (5x5)
    private List<Texture> textures; // List of textures to assign to planes
    private GameObject[,] gridPlanes; // 2D array to hold the planes
    private System.Random random = new System.Random();
    private float planeScaleFactor = 0.77f; // Scale to make each plane 770 pixels assuming 1 unit = 100 pixels
    private float nextActionTime = 0.0f; // Time to control the speed of texture change
    private float period = 0.01f; // Time interval between texture changes

    void Start()
    {
        if (planePrefab == null)
        {
            Debug.LogError("Plane prefab is not assigned!");
            return;
        }

        LoadTextures();
        
        if (textures == null || textures.Count == 0)
        {
            Debug.LogError("No textures found in Resources/Textures!");
            return;
        }

        gridPlanes = new GameObject[gridSize, gridSize];
        CreateGrid();
    }

    void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            ChangeTextureRandomly();
        }
    }

    void LoadTextures()
    {
        textures = new List<Texture>(Resources.LoadAll<Texture>("Textures"));
    }

    void CreateGrid()
    {
        float spacing = planeScaleFactor * 10; // Adjust spacing based on scale

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                // Calculate the correct position
                Vector3 position = new Vector3(x * spacing, 0, z * spacing);
                GameObject plane = Instantiate(planePrefab, position, Quaternion.identity);
                plane.transform.parent = transform; // Set the GridManager as parent
                plane.transform.localScale = new Vector3(planeScaleFactor, 1f, planeScaleFactor); // Set the scale of the plane
                gridPlanes[x, z] = plane;

                // Assign a random texture
                MeshRenderer renderer = plane.GetComponent<MeshRenderer>();
                renderer.material.mainTexture = textures[random.Next(textures.Count)];
            }
        }
    }

    void ChangeTextureRandomly()
    {
        // Select a random plane
        int x = random.Next(gridSize);
        int z = random.Next(gridSize);
        GameObject selectedPlane = gridPlanes[x, z];

        // Change its texture
        MeshRenderer renderer = selectedPlane.GetComponent<MeshRenderer>();
        renderer.material.mainTexture = textures[random.Next(textures.Count)];
    }
}
