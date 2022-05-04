using UnityEngine;
using UnityEngine.AI;

public class BoardManager : MonoBehaviour
{

    public int width = 10;
    public int height = 10;
    public GameObject[] floorList;
    public GameObject[] enemySprites;
    public GameObject wall;
    public GameObject enemy;
    public GameObject player;
    public GameObject fogVolume;
    public NavMeshSurface surface;
    public int[][] matrix;
    public int[][] distances;
    public Vector3 scale;

    Transform boardHolder;
    Transform enemiesHolder;


    void Start()
    {
        surface = GameObject.FindObjectOfType<NavMeshSurface>();
        surface.BuildNavMesh();
    }

    public void SetupScene(string level)
    {
        Transform floorTransform = floorList[0].GetComponent<Transform>();
        scale = new Vector3(floorTransform.localScale.x, 1.0f, floorTransform.localScale.y);
        boardHolder = new GameObject("Board").transform;

        ZombieTiles zt = new ZombieTiles();
        zt.GenerateDugeon( level, width, height);
        matrix = zt.GetDungeonMatrix();
        distances = zt.DistanceGraph;
        PlaceTiles(zt.Rooms);
        PlaceWalls(zt.GetWalls());
        PlacePlayer(zt.Player);
        PlaceEnemies(zt.Enemies);
        print(zt.GetDescription());
    }

    void PlaceTiles(ZtRoom[] rooms)
    {
        Vector3 offset = scale * 0.5f;

        int i = 0;
        foreach (ZtRoom room in rooms)
        {
            float renderOffset = 0.001f * i * (room.placement_type == ZtPlacementType.T ? 1 : -1);
            Vector3 pos = new Vector3(room.x + (room.width * 0.5f), 0.5f + renderOffset, room.y + (room.height * 0.5f));
            pos.Scale(scale);
            pos -= offset;

            Vector3 newLocalScale = new Vector3(room.width * scale.x, room.height * scale.z, 1f);

            GameObject tileObject = Instantiate(floorList[Random.Range(0, floorList.Length)], pos, Quaternion.Euler(90, 0, 0));
            tileObject.transform.localScale = newLocalScale;
            tileObject.transform.SetParent(boardHolder);
            i++;
        }

        for (int x = 0; x < matrix.Length; x++)
        {
            for (int y = 0; y < matrix[x].Length; y++)
            {
                if (matrix[x][y] == -1)
                {
                    continue;
                }
                Vector3 pos = VectorConverter.Convert(new Vector2(x,y));
                pos.Scale(scale);
                GameObject instance = Instantiate(fogVolume, pos, Quaternion.identity );
                instance.transform.SetParent(boardHolder);
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

    void PlaceEnemies(ZtEnemy[] enemies)
    {
        enemiesHolder = new GameObject("Enemies").transform;

        foreach (ZtEnemy e in enemies)
        {
            Vector3 pos = new Vector3(e.position.x, 1f, e.position.y);
            pos.Scale(scale);

            GameObject instance = Instantiate(enemy, pos, Quaternion.identity);

            Enemy enemyScript = instance.GetComponent<Enemy>();
            enemyScript.health = e.health;
            enemyScript.damage = e.damage;
            enemyScript.attackCooldown = 6f - (5f * (e.attackCooldown / 100f));
            enemyScript.velocity = 5f + (15f * (e.velocity / 100f));
            instance.transform.SetParent(enemiesHolder);

            GameObject sprite = Instantiate(
                enemySprites[Random.Range(0, enemySprites.Length)],
                instance.transform.position,
                Quaternion.identity,
                instance.transform);

            sprite.transform.localScale = new Vector3(11, 11, 0);
        }
    }

    void PlacePlayer(ZtEntity p)
    {
        Vector3 pos = new Vector3(p.position.x, 1f, p.position.y);
        pos.Scale(scale);
        Instantiate(player, pos, Quaternion.identity);
    }
}
