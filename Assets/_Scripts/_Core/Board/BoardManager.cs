using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : BaseBehaviour
{
    [SerializeField] protected int width = 8;
    [SerializeField] protected int height = 8;

    public GameObject[] gemPrefabs;

    private GameObject[,] grid;

    protected override void Start()
    {
        grid = new GameObject[width, height];
        SpawnGrid();
    }
    void SpawnGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int rand = Random.Range(0, gemPrefabs.Length);
                Vector2 pos = new Vector2(x, y);

                GameObject gem = Instantiate(gemPrefabs[rand], pos, Quaternion.identity);
                grid[x, y] = gem;
            }
        }
    }
}
