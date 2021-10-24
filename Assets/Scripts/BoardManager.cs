using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class BoardManager : MonoBehaviour
{
    public void SetupScene(int level)
    {
        IntPtr dungeon = ZombieTiles.generate_dungeon(10, 10);

        int w;
        int h;
        IntPtr[] matrix_ptr;
        ZombieTiles.get_dungeon_matrix(dungeon, out w, out h, out matrix_ptr);

        int[][] matrix = new int[w][];
        for (int i = 0; i < w; ++i)
        {
            matrix[i] = new int[w];
            Marshal.Copy(matrix_ptr[i], matrix[i], 0, h);
        }

        string d = "";
        for (int y = 0; y < h; ++y)
        {
            for (int x = 0; x < w; ++x)
            {
                d += matrix[x][y];
            }
            d += "\n";
        }
        print(d);

        int size2;
        Wall[] walls;
        ZombieTiles.get_wall_array(dungeon, out size2, out walls);

        for (int i = 0; i < size2; i++)
        {
            print($"({walls[i].a.x}, {walls[i].a.y}) "
                + $"-> ({walls[i].b.x}, {walls[i].b.y})");
        }
    }
}
