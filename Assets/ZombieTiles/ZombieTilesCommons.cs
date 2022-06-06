using System.Runtime.InteropServices;
using System.Collections.Generic;
using Newtonsoft.Json;

// public class Dungeon
// {
//     public ZtEnemy[] enemies { get; set; }
//     public ZtRoom[] rooms { get; set; }
//     public ZtPlayer player { get; set; }
//     public Wall[] walls { get; set; }
//     public int[][] matrix { get; set; }
//     public int[][] distances { get; set; }
// }
public class Dungeon
{
    public List<ZtEnemy> enemies { get; set; }
    public List<ZtRoom> rooms { get; set; }
    public ZtPlayer player { get; set; }
    public List<Wall> walls { get; set; }
    public List<List<int>> matrix { get; set; }
    public List<List<int>> distances { get; set; }
}

[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum EntityType
{
    PLAYER,
    ENEMY
}

[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
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
public struct ZtPlayer : ZtEntity
{
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
