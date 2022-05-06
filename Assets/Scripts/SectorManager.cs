using UnityEngine;

public class SectorManager : MonoBehaviour
{
    int[][] sectors;
    int[][] distances;
    Vector3 origin;
    Vector3 scale;

    int width;
    int height;

    void Start()
    {
        sectors = GetComponentInParent<BoardManager>().matrix;
        distances = GetComponentInParent<BoardManager>().distances;
        origin = GameObject.Find("Board").transform.position;
        scale = GetComponentInParent<BoardManager>().scale;
        width = sectors.Length;
        height = sectors[0].Length;

        string str = "";
        foreach (int[] l in distances)
        {
            foreach (int i in l)
            {
                str += $"[{i}]";
            }
            str += "\n";
        }
        print(str);
    }

    public int SectorOf(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / scale.x);
        int y = Mathf.RoundToInt(position.z / scale.z);
        return sectors[x][y];
    }

    public int SectorDistanceOf(Vector3 a, Vector3 b)
    {
        return distances[SectorOf(a)][SectorOf(b)];
    }
}
