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
    public NavMeshSurface surface;

    private Transform boardHolder;
    private Transform floorTransform;

    void Start(){
        surface = GameObject.FindObjectOfType<NavMeshSurface>();
        surface.BuildNavMesh();
    }

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
        Vector3 offset = floorTransform.localScale * 5;
        offset.y = 0;
        Vector3 floorScale = floorTransform.localScale * 10;

        foreach (Wall pos in walls)
        {
            Vector3 origin = new Vector3(pos.a.x, 0f, pos.a.y);
            origin.Scale(floorScale);
            origin -= offset;
            GameObject wallObject = Instantiate(wall, origin, Quaternion.identity);

            Stretch2Target stretch2Target = wallObject.GetComponent<Stretch2Target>();

            Vector3 target = new Vector3(pos.b.x, 0f, pos.b.y);
            target.Scale(floorScale);
            target -= offset;
            stretch2Target.target = target;

            wallObject.transform.SetParent(boardHolder);
        }
        // InstantiateEnemy(new Vector3(0,0,0), 20, 1, 50, 20);

    }

    void InstantiateEnemy(Vector3 pos, float health, float damage, float attackCooldown, float velocity )
    {
        GameObject instanse = Instantiate(enemy, pos, Quaternion.identity);

        Enemy enemyScript = instanse.GetComponent<Enemy>();
        enemyScript.health = health;
        enemyScript.damage = damage;
        enemyScript.attackCooldown = attackCooldown;
        enemyScript.velocity = velocity;
    }
}
