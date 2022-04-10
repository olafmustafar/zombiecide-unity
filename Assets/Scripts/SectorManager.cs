using UnityEngine;

public class SectorManager : MonoBehaviour
{
    int[][] sectors;
    Vector3 origin;
    Vector3 boardSize;
    Vector3 scale;

    int width;
    int height;

    void Start()
    {
        sectors = GetComponentInParent<BoardManager>().matrix;
        origin = GameObject.Find("Board").transform.position;
        scale = GetComponentInParent<BoardManager>().scale;
        width = sectors.Length;
        height = sectors[0].Length;
    }

    public int SectorOf(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / scale.x);
        int y = Mathf.RoundToInt(position.z / scale.z);
        return sectors[x][y];
    }
}
