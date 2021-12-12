using UnityEngine;
using UnityEngine;
using UnityEngine.AI;

public class VectorConverter
{
    public static Vector2 Convert(Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }

    public static Vector3 Convert(Vector2 vector)
    {
        return new Vector3(vector.x, 0, vector.y);
    }

    public static Vector3 Convert(Vector2 vector, float y)
    {
        return new Vector3(vector.x, y, vector.y);
    }

    public static bool CalculateNavmeshPath(Vector2 aPos, Vector2 bPos, NavMeshPath path)
    {
        bool result = NavMesh.CalculatePath(VectorConverter.Convert(aPos, 0), VectorConverter.Convert(bPos, 0), NavMesh.AllAreas, path);
        
        // for (int i = 0; i < path.corners.Length - 1; i++)
        // {
        //     Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
        // }

        return result;
    }
}