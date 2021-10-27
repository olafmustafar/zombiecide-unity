using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

    public int width = 10;
    public int height = 10;
    public GameObject floor;
    public GameObject wall;

    private Transform boardHolder;
    private Transform floorTransform;// = floor.GetComponent<Transform>();


    public void SetupScene(int level)
    {
        floorTransform = floor.GetComponent<Transform>();

        ZombieTiles zt = new ZombieTiles();
        zt.GenerateDugeon(width, height);

        int[][] matrix = zt.GetDungeonMatrix();

        boardHolder = new GameObject("Board").transform;
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {

                int tile = matrix[x][y];
                if (tile != ZombieTiles.EMPTY_TILE)
                {
                    Vector3 pos = new Vector3(x, 0f, y);
                    pos = pos * 10;
                    pos.Scale(floorTransform.localScale);
                    GameObject tileObject = Instantiate(floor, pos, Quaternion.identity);
                    tileObject.transform.SetParent(boardHolder);
                }
            }
        }

        Wall[] walls = zt.GetWalls();
        Vector3 offset = floor.transform.localScale*5;

        foreach (Wall pos in walls)
        {
            GameObject wallObject = Instantiate(wall, new Vector3(pos.a.x - offset.x, 0f, pos.a.y - offset.z), Quaternion.identity);
            Stretch2Target stretch2Target = wallObject.GetComponent<Stretch2Target>();
            stretch2Target.target = new Vector3(pos.b.x - offset.x, 0f, pos.b.y - offset.z);
            wallObject.transform.SetParent(boardHolder);
        }
    }
}
