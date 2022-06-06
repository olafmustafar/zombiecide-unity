using System;
using System.Runtime.InteropServices;

public class ZombieTiles {
    private const string zombietilesdll = @"libzombietiles.so";

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr generate_dungeon(Int32 width, Int32 height);

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr load_dungeon(string path);

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void free_dungeon(IntPtr dungeon);

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void generate_dungeon_enemies(IntPtr dungeon, out int size, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] out ZtEnemy[] array);

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void generate_dungeon_player(IntPtr dungeon, out IntPtr player);

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)] private static extern void generate_dungeon_walls(IntPtr dungeon, out int size, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] out Wall[] array);

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void free_wall_array(Wall[] array);

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void get_dungeon_matrix(IntPtr dungeon, out int width, out int height, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] out IntPtr[] matrix);

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void get_dungeon_distances_graph(IntPtr dungeon, out int width, out int height, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] out IntPtr[] graph);

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void generate_dungeon_description(IntPtr dungeon, out int size, out IntPtr str);

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void generate_dungeon_rooms(IntPtr dungeon, out int size, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] out ZtRoom[] array);

    [DllImport(zombietilesdll, CallingConvention = CallingConvention.Cdecl)]
    private static extern int ping();

    public static readonly int EMPTY_TILE = -1;
    private IntPtr dungeon;
    private Wall[] walls; private String description;
    private ZtRoom[] rooms;
    private ZtEnemy[] enemies;
    private ZtEntity player;
    private int[][] matrix;
    private int[][] distanceGraph;

    public ZtRoom[] Rooms { get => rooms; set => rooms = value; }
    public ZtEntity Player { get => player; set => player = value; }
    public ZtEnemy[] Enemies { get => enemies; set => enemies = value; }
    public int[][] Matrix { get => matrix; set => matrix = value; }
    public int[][] DistanceGraph { get => distanceGraph; set => distanceGraph = value; }
    
    public static int Ping(){
        return ping();
    }

    ~ZombieTiles()
    {
        free_dungeon(dungeon);
    }

    public void GenerateDugeon( string level, Int32 width, Int32 height)
    {
        dungeon = load_dungeon(level);
        {
            int size;
            generate_dungeon_walls(dungeon, out size, out walls);
        }

        {
            int size;
            generate_dungeon_enemies(dungeon, out size, out enemies);
        }

        {
            IntPtr playerPtr;
            generate_dungeon_player(dungeon, out playerPtr);
            player = Marshal.PtrToStructure<ZtPlayer>(playerPtr);
        }

        {
            int size;
            IntPtr str;
            generate_dungeon_description(dungeon, out size, out str);
            description = Marshal.PtrToStringAnsi(str, size);
        }

        {
            int size;
            generate_dungeon_rooms(dungeon, out size, out rooms);
        }

        {
            int w;
            int h;
            IntPtr[] matrixPtr;

            get_dungeon_matrix(dungeon, out w, out h, out matrixPtr);

            matrix = new int[w][];

            for (int x = 0; x < w; ++x)
            {
                matrix[x] = new int[h];
                Marshal.Copy(matrixPtr[x], matrix[x], 0, h );
            }
        }
        
        {
            int w;
            int h;
            IntPtr[] distanceGraphPtr;

            get_dungeon_distances_graph(dungeon, out w, out h, out distanceGraphPtr);

            distanceGraph = new int[w][];

            for (int x = 0; x < w; ++x)
            {
                distanceGraph[x] = new int[h];
                Marshal.Copy(distanceGraphPtr[x], distanceGraph[x], 0, h);
            }
        }
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
        return matrix;
    }
}
