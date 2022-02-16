using System;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections.Generic;

public enum EntityType
{
    PLAYER,
    ENEMY
}

public enum ZtPlacementType
{
    T, // Top
    U // Under
}

[StructLayout(LayoutKind.Sequential)]
public struct Point
{
    public int x;
    public int y;
}

[StructLayout(LayoutKind.Sequential)]
public struct Line
{
    public Point a;
    public Point b;
}

[StructLayout(LayoutKind.Sequential)]
public struct ZtRoom
{
    public uint x;
    public uint y;
    public uint width;
    public uint height;
    public ZtPlacementType placement_type;
}

[StructLayout(LayoutKind.Sequential)]
public struct Wall
{
    public Point a;
    public Point b;
}

public interface ZtEntity
{
    EntityType type { get; set; }
    Point position { get; set; }
}

[StructLayout(LayoutKind.Sequential)]
public struct ZtPlayer : ZtEntity{
    public EntityType type { get; set; }
    public Point position { get; set; }
}


[StructLayout(LayoutKind.Sequential)]
public struct ZtEnemy : ZtEntity
{
    public EntityType type { get; set; }
    public Point position { get; set; }
    public uint health { get; set; }
    public uint damage { get; set; }
    public uint attackCooldown { get; set; }
    public uint velocity { get; set; }
}


public class ZombieTiles
{

    // private const string zombietilesdll = @"/home/rei-arthur/.config/unity3d/DefaultCompany/zombiecide/libzombietiles.so";
    private const string zombietilesdll = @"/home/rei-arthur/.config/unity3d/ArthurTcc/zombiecide/libzombietiles.so";

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr generate_dungeon(Int32 width, Int32 height);

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void free_dungeon(IntPtr dungeon);

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void generate_dungeon_enemies(  IntPtr dungeon, out int size, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] out ZtEnemy[] array);

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void generate_dungeon_player(IntPtr dungeon, out IntPtr player);

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void generate_dungeon_walls(IntPtr dungeon, out int size, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] out Wall[] array);

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void free_wall_array(Wall[] array);

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void get_dungeon_matrix(IntPtr dungeon, out int width, out int height, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] out IntPtr[] matrix);

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void generate_dungeon_description(IntPtr dungeon, out int size, out IntPtr str);

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void generate_dungeon_rooms(IntPtr dungeon, out int size, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] out ZtRoom[] array);

    public static readonly int EMPTY_TILE = -1;
    private IntPtr dungeon;
    private Wall[] walls;
    private String description;
    private ZtRoom[] rooms;
    private ZtEnemy[] enemies;
    private ZtEntity player;

    public ZtRoom[] Rooms { get => rooms; set => rooms = value; }
    public ZtEntity Player { get => player; set => player = value; }
    public ZtEnemy[] Enemies { get => enemies; set => enemies = value; }

    ~ZombieTiles()
    {
        free_dungeon(dungeon);
    }

    public void GenerateDugeon(Int32 width, Int32 height)
    {
        dungeon = generate_dungeon(width, height);

        int size;
        generate_dungeon_walls(dungeon, out size, out walls);

        int size2;
        generate_dungeon_enemies(dungeon, out size2, out enemies);

        IntPtr playerPtr;
        generate_dungeon_player(dungeon, out playerPtr);
        player = Marshal.PtrToStructure<ZtPlayer>(playerPtr);
        
        int size3;
        IntPtr str;
        generate_dungeon_description(dungeon, out size3, out str);
        description = Marshal.PtrToStringAnsi(str, size3);

        int size4;
        generate_dungeon_rooms(dungeon, out size4, out rooms);
    }

    public Wall[] GetWalls()
    {
        return walls;
    }

    public string GetDescription()
    {
        return description;
    }

    public int[][] GetDungeonMatrix()
    {
        int w;
        int h;
        IntPtr[] matrix_ptr;

        get_dungeon_matrix(dungeon, out w, out h, out matrix_ptr);

        int[][] matrix = new int[w][];

        for (int i = 0; i < w; ++i)
        {
            matrix[i] = new int[w];
            Marshal.Copy(matrix_ptr[i], matrix[i], 0, h);
        }

        return matrix;
    }

}