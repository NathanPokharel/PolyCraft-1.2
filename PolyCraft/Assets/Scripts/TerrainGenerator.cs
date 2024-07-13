using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class TerrainGenerator : MonoBehaviour
{
    Mesh mesh;
    MeshCollider meshCollider;
    
    Vector3[] vertices;
    int[] triangles;
    public int xSize = 20;
    public int zSize = 20;
    public float noiseScale = 0.3f;
    public float heightMultiplier = 2f;

    public GameObject treePrefab; // Single prefab for trees
    public GameObject rockPrefab; // Single prefab for rocks
    public GameObject grassPrefab; // Single prefab for grass
    public GameObject flowerPrefab; // Single prefab for grass
    public int treeCount = 10;
    public int rockCount = 5;
    public int grassCount = 50; // Number of grass patches to place
    public int flowerCount = 20; // Number of grass patches to place

    void Awake()
    {
        mesh = new Mesh();
        meshCollider = GetComponent<MeshCollider>();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void GenerateTerrain(Vector2 offset)
    {
        CreateShape(offset);
        UpdateMesh();
        PlaceObjects(offset);
    }

    void CreateShape(Vector2 offset)
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        int i = 0;
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise((x + offset.x) * noiseScale, (z + offset.y) * noiseScale) * heightMultiplier;
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        int vert = 0;
        int tris = 0;
        triangles = new int[xSize * zSize * 6];

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[0 + tris] = 0 + vert;
                triangles[1 + tris] = vert + xSize + 1;
                triangles[2 + tris] = vert + 1;
                triangles[3 + tris] = vert + 1;
                triangles[4 + tris] = vert + xSize + 1;
                triangles[5 + tris] = vert + xSize + 2;
                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        meshCollider.sharedMesh = mesh; // Update the MeshCollider
    }

    void PlaceObjects(Vector2 offset)
    {
        PlaceTrees(offset);
        PlaceRocks(offset);
        PlaceGrass(offset); // Call the method to place grass
        PlaceFlower(offset); // Call the method to place grass
    }

    void PlaceTrees(Vector2 offset)
    {
        for (int i = 0; i < treeCount; i++)
        {
            Vector3 position = GetRandomPosition(offset);
            GameObject tree = Instantiate(treePrefab, position, Quaternion.identity);
            tree.transform.parent = this.transform;
        }
    }

    void PlaceRocks(Vector2 offset)
    {
        for (int i = 0; i < rockCount; i++)
        {
            Vector3 position = GetRandomPosition(offset);
            GameObject rock = Instantiate(rockPrefab, position, Quaternion.identity);
            rock.transform.parent = this.transform;
        }
    }

    void PlaceGrass(Vector2 offset)
    {
        for (int i = 0; i < grassCount; i++)
        {
            Vector3 position = GetRandomPosition(offset);
            GameObject grass = Instantiate(grassPrefab, position, Quaternion.identity);
            grass.transform.parent = this.transform;
        }
    }

    void PlaceFlower(Vector2 offset)
    {
        for (int i = 0; i < flowerCount; i++)
        {
            Vector3 position = GetRandomPosition(offset);
            GameObject grass = Instantiate(flowerPrefab, position, Quaternion.identity);
            grass.transform.parent = this.transform;
        }
    }

    Vector3 GetRandomPosition(Vector2 offset)
    {
        float x = Random.Range(0, xSize) + offset.x;
        float z = Random.Range(0, zSize) + offset.y;
        float y = Mathf.PerlinNoise(x * noiseScale, z * noiseScale) * heightMultiplier;
        return new Vector3(x, y, z);
    }
}

