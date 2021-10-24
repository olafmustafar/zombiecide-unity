using UnityEngine;
using System;
using System.Runtime.InteropServices;

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
public struct Wall
{
    public Point a;
    public Point b;
}

public class ZombieTiles
{
    private const string zombietilesdll = @"/home/rei-arthur/.config/unity3d/DefaultCompany/zombiecide/libzombietiles.so";
    // private const string zombietilesdll = @"/home/rei-arthur/zteste/libzombietiles.so";
    // private const string zombietilesdll = @"/home/rei-arthur/zteste/libzombietiles.so";

    [DllImport(zombietilesdll,CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr generate_dungeon(Int32 width, Int32 height);

    [DllImport(zombietilesdll,CallingConvention = CallingConvention.Cdecl)]
    public static extern void get_dungeon_matrix(IntPtr dungeon, out int width, out int height,  [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] out IntPtr[] matrix);

    [DllImport(zombietilesdll,CallingConvention = CallingConvention.Cdecl)]
    public static extern void get_wall_array(IntPtr dungeon, out int size, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] out Wall[] array);

}