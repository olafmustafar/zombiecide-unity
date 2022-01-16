// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

public class BoardManager : MonoBehaviour
{

    public int width = 10;
    public int height = 10;
    public GameObject floor;
    public GameObject wall;
    public GameObject enemy;
    public GameObject player;
    public NavMeshSurface surface;

    Transform boardHolder;
    Transform enemiesHolder;
    Vector3 scale;

    void Start()
    {
        surface = GameObject.FindObjectOfType<NavMeshSurface>();
        surface.BuildNavMesh();
    }

    public void SetupScene(int level)
    {

        Transform floorTransform = floor.GetComponent<Transform>();
        scale = new Vector3(floorTransform.localScale.x, 1.0f, floorTransform.localScale.y);
        boardHolder = new GameObject("Board").transform;

        ZombieTiles zt = new ZombieTiles();
        zt.GenerateDugeon(width, height);

        PlaceTiles(zt.GetDungeonMatrix());
        PlaceWalls(zt.GetWalls());
        PlaceEntities(zt.GetEntities());
        print ( zt.GetDescription() );
    }

    void PlaceTiles(int[][] matrix)
    {
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                int tile = matrix[x][y];
                if (tile != ZombieTiles.EMPTY_TILE)
                {
                    Vector3 pos = new Vector3(x, 0f, y);
                    pos.Scale(scale);
                    GameObject tileObject = Instantiate(floor, pos, Quaternion.Euler(90, 0, 0));
                    tileObject.transform.SetParent(boardHolder);
                }
            }
        }
    }

    void PlaceWalls(Wall[] walls)
    {
        Vector3 offset = scale * 0.5f;
        offset.y = 0;

        foreach (Wall pos in walls)
        {
            Vector3 origin = new Vector3(pos.a.x, 0f, pos.a.y);
            origin.Scale(scale);
            origin -= offset;
            GameObject wallObject = Instantiate(wall, origin, Quaternion.identity);

            Stretch2Target stretch2Target = wallObject.GetComponent<Stretch2Target>();

            Vector3 target = new Vector3(pos.b.x, 0f, pos.b.y);
            target.Scale(scale);
            target -= offset;
            stretch2Target.target = target;

            wallObject.transform.SetParent(boardHolder);
        }
    }

    void PlaceEntities(ZtEntity[] entities)
    {
        enemiesHolder = new GameObject("Enemies").transform;

        foreach (ZtEntity e in entities)
        {
            Vector3 pos = new Vector3(e.position.x, 1f, e.position.y);
            pos.Scale(scale);
            
            if (e.type == EntityType.PLAYER)
            {
                Instantiate(player,pos, Quaternion.identity);
                continue;
            }

            GameObject instance = Instantiate(enemy, pos, Quaternion.identity);

            Enemy enemyScript = instance.GetComponent<Enemy>();
            enemyScript.health = 100;
            enemyScript.damage = 100;
            enemyScript.attackCooldown = 5;
            enemyScript.velocity = 10;
            instance.transform.SetParent(enemiesHolder);
        }
    }
}
