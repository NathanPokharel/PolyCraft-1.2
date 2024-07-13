using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public GameObject terrainPrefab;
    public Transform player;
    public int chunkSize = 20;
    public int viewDistance = 2;

    private Dictionary<Vector2, GameObject> terrainChunks = new Dictionary<Vector2, GameObject>();
    private Vector2 currentChunk;

    void Start()
    {
        UpdateTerrainChunks();
    }

    void Update()
    {
        Vector2 newChunk = GetChunkCoordinate(player.position);
        if (newChunk != currentChunk)
        {
            currentChunk = newChunk;
            UpdateTerrainChunks();
        }
    }

    void UpdateTerrainChunks()
    {
        List<Vector2> chunksToRemove = new List<Vector2>(terrainChunks.Keys);
        Vector2 playerChunk = GetChunkCoordinate(player.position);

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                Vector2 chunkCoord = new Vector2(playerChunk.x + x, playerChunk.y + y);
                if (!terrainChunks.ContainsKey(chunkCoord))
                {
                    CreateChunk(chunkCoord);
                }
                chunksToRemove.Remove(chunkCoord);
            }
        }

        foreach (Vector2 chunkCoord in chunksToRemove)
        {
            Destroy(terrainChunks[chunkCoord]);
            terrainChunks.Remove(chunkCoord);
        }
    }

    void CreateChunk(Vector2 chunkCoord)
    {
        GameObject newChunk = Instantiate(terrainPrefab, new Vector3(chunkCoord.x * chunkSize, 0, chunkCoord.y * chunkSize), Quaternion.identity);
        TerrainGenerator terrainGenerator = newChunk.GetComponent<TerrainGenerator>();
        terrainGenerator.GenerateTerrain(chunkCoord * chunkSize);
        terrainChunks.Add(chunkCoord, newChunk);
    }

    Vector2 GetChunkCoordinate(Vector3 position)
    {
        return new Vector2(Mathf.FloorToInt(position.x / chunkSize), Mathf.FloorToInt(position.z / chunkSize));
    }
}

