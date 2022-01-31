using System;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;

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

[StructLayout(LayoutKind.Sequential)]
public struct ZtEntity
{
    public EntityType type;
    public Point position;
}


public class ZombieTiles
{

    private const string zombietilesdll = @"/home/rei-arthur/.config/unity3d/DefaultCompany/zombiecide/libzombietiles.so";

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr generate_dungeon(Int32 width, Int32 height);

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void free_dungeon(IntPtr dungeon);

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void generate_dungeon_entities(IntPtr dungeon, out int size, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] out ZtEntity[] array);

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
    private ZtEntity[] entities;
    private String description;
    private ZtRoom[] rooms;

    public ZtRoom[] Rooms { get => rooms; set => rooms = value; }

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
        generate_dungeon_entities(dungeon, out size2, out entities);

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

    public ZtEntity[] GetEntities()
    {
        return entities;
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