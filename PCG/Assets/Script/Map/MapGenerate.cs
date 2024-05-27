
#define COASTLINE_GENERATE
#define COASTLINE_GENERATE_2
#define MAKE_MAP_BIGGER
#define WALL_GENERATE 
#define ROAD_GENERATE
#define SHADOW_GENERATE
#define PLANT_GENERATE
#define PROP_GENERATE
#define MAIN_PROP_GENERATE

using Complete;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = System.Random;
namespace MyMap
{
    public class MapGenerate : MonoBehaviour
    {

        static int unitPerChunk = 20;
        private static int seed = TypeConstants.seed;

        /// Gets the 4 points adjacent to a point
        /// The four points adjacent to that point, for example (0,0), will return (0,1),(0,-1),(1,0),(-1,0).
        public static Vector2Int[] GetNeighbors4(Vector2Int pos)
        {
            var result = new Vector2Int[4];
            result[0] = new Vector2Int(pos[0], pos[1] + 1);
            result[1] = new Vector2Int(pos[0], pos[1] - 1);
            result[2] = new Vector2Int(pos[0] + 1, pos[1]);
            result[3] = new Vector2Int(pos[0] - 1, pos[1]);
            return result;
        }

        /// Get four points diagonally across a point
        /// The four points adjacent to this point, for example (0,0), will return (-1,-1),(1,1),(-1,1),(-1,1).
        public static Vector2Int[] GetCorner4(Vector2Int pos)
        {
            var result = new Vector2Int[4];
            result[0] = new Vector2Int(pos[0] - 1, pos[1] - 1);
            result[1] = new Vector2Int(pos[0] + 1, pos[1] - 1);
            result[2] = new Vector2Int(pos[0] + 1, pos[1] + 1);
            result[3] = new Vector2Int(pos[0] - 1, pos[1] + 1);
            return result;
        }

        /// Generate a random map plate layout - layer1
        public static Vector2Int[] RandomChunkLayout(int count)
        {
            Vector2Int[] result = new Vector2Int[count];
            result[0] = new Vector2Int(0, 0);// Take (0,0) as the starting point
            List<Vector2Int> edge = new List<Vector2Int>(); // The 4 directions of the existing points are taken as edges, and one edge point is randomly selected each time as a new point
            edge.AddRange(GetNeighbors4(result[0]));


            var random = new Random(seed);
            // random is used to randomly select a point
            for (int i = 1; i < count; i++)
            {
                var curr = edge[random.Next(0, edge.Count)];
                result[i] = curr;
                edge.Remove(curr);

                // To check the 4 points adjacent to the new point, you need to filter out the adjacent points that already exist in the result and edge
                foreach (var neighbor in GetNeighbors4(curr))
                {
                    if (Array.Exists(result, v => v.Equals(neighbor))) continue;
                    if (edge.Contains(neighbor)) continue;
                    edge.Add(neighbor);
                }
            }

            // Zero out the smallest point
            for (int i = 0; i < result.Length; i++)
            {
                result[i].x -= result.Min(p => p.x);
                result[i].y -= result.Min(p => p.y);
            }


            return result;
        }

        /// Generate a random map plate layout - layer2
        public static byte[,] RandomChunkLayoutSecondLayer(Vector2Int[] layer1)
        {
            List<Vector2Int> r = new List<Vector2Int>();
            var sMaxX = (layer1.Max(p => p.x) - layer1.Min(p => p.x) + 1) * 2;
            var sMaxY = (layer1.Max(p => p.y) - layer1.Min(p => p.y) + 1) * 2;


            var maxX = (layer1.Max(p => p.x) - layer1.Min(p => p.x) + 1) * unitPerChunk / 2;
            var maxY = (layer1.Max(p => p.y) - layer1.Min(p => p.y) + 1) * unitPerChunk / 2;

            int scale = maxX / sMaxX;
            // Record whether a point has been used
            byte[,] map = new byte[sMaxX, sMaxY];
            byte[,] layer2Map = new byte[maxX, maxY];
            for (int i = 0; i < sMaxX; i++)
            {
                for (int j = 0; j < sMaxY; j++)
                {
                    map[i, j] = 255;
                }
            }

            for (int i = 0; i < maxX; i++)
            {
                for (int j = 0; j < maxY; j++)
                {
                    layer2Map[i, j] = 255;
                }
            }

            var random = new Random(seed);
            // A point is divided into four parts, randomly pick a point to start
            foreach (var layout1 in layer1)
            {
                for (int x = 0; x < 2; x++)
                {
                    for (int y = 0; y < 2; y++)
                    {
                        int i = layout1.x * 2 + x;
                        int j = layout1.y * 2 + y;
                        if (i == 0 || i == sMaxX - 1)
                        {
                            continue;
                        }
                        if (!Array.Exists(layer1, v => v.Equals(new Vector2Int(layout1.x + (x == 0 ? -1 : 1), layout1.y))))
                        {
                            continue;
                        }
                        if (random.Next(0, 100) < 50 && map[i, j] == 255)
                        {

                            bool flag = false;

                            int pro = 80;
                            while (random.Next(0, 100) < pro && map[i, j] == 255)
                            {
                                if (!flag)
                                {
                                    flag = true;
                                    map[i, j] = 1;
                                }
                                else
                                {
                                    map[i, j] = 0;
                                }

                                r.Add(new Vector2Int(i, j));
                                // Grow in a random direction, not down
                                Vector2Int d;
                                do
                                {
                                    int direction = random.Next(0, 4);
                                    d = new Vector2Int(direction % 2 == 0 ? direction - 1 : 0,
                                        direction % 2 == 0 ? 0 : direction - 2);
                                } while (i + d.x < 0 || j + d.y < 0 || i + d.x >= sMaxX || j + d.y >= sMaxY);

                                i += d.x;
                                j += d.y == -1 ? 0 : d.y;
                                pro -= 20;
                            }

                        }
                    }
                }
            }
            // Finish two level entrance
            for (int i = 0; i < sMaxX; i++)
            {
                for (int j = 0; j < sMaxY; j++)
                {
                    if (j == 0)
                    {
                        map[i, j] = 255;
                        continue;
                    }

                    if (map[i, j] <= 1)
                    {
                        for (int u = i * scale; u < i * scale + scale; u++)
                        {
                            for (int v = j * scale; v < j * scale + scale; v++)
                            {
                                layer2Map[u, v] = 0;
                            }
                        }

                        if (map[i, j] == 1)
                        {
                            if (map[i, j - 1] <= 1)
                            {
                                map[i, j] = 0;
                            }
                            else
                            {
                                int door = random.Next(i * scale + 1, i * scale + scale - 1);
                                // The entrance value is 100, and the lattice near the entrance is 101, which acts as protection when generating the shoreline
                                layer2Map[door, j * scale] = 100;
                                layer2Map[door - 1, j * scale] = 101;
                                layer2Map[door + 1, j * scale] = 101;
                                layer2Map[door - 1, j * scale + 1] = 101;
                                layer2Map[door, j * scale + 1] = 101;
                                layer2Map[door + 1, j * scale + 1] = 101;
                            }
                        }
                    }
                }
            }

            map = layer2Map;
            // Coastline
            #region CoastlineGenerate
#if COASTLINE_GENERATE
            for (int i = 0; i < 2; i++)
            {
                byte[,] writing = (byte[,])map.Clone();
                for (int x = 0; x < maxX; x++)
                {
                    for (int y = 0; y < maxY; y++)
                    {
                        if (map[x, y] == 100 || map[x, y] == 101)
                        {
                            continue;
                        }
                        var neighbors = GetNeighbors4(new Vector2Int(x, y)).Where(p => p.x < 0 || p.y < 0 || p.x >= maxX || p.y >= maxY || map[p.x, p.y] == 255);
                        if (neighbors.Count() != 1)
                            continue;
                        float noise = Mathf.PerlinNoise(x / (float)maxX * 50, y / (float)maxY * 50);

                        if (noise < 0.4f)
                        {
                            // 30% chance of becoming a wall
                            writing[x, y] = 255;
                        }
                    }
                }
                map = writing;


                writing = (byte[,])map.Clone();
                for (int x = 0; x < maxX; x++)
                {
                    for (int y = 0; y < maxY; y++)
                    {
                        if (map[x, y] == 100 || map[x, y] == 101)
                        {
                            continue;
                        }
                        var neighbors = GetNeighbors4(new Vector2Int(x, y)).Where(p => p.x < 0 || p.y < 0 || p.x >= maxX || p.y >= maxY || map[p.x, p.y] == 255);
                        int neighCount = neighbors.Count();
                        if (neighCount == 1)
                            continue;
                        if (neighCount == 2 && random.Next(0, 100) < 30)
                        {
                            writing[x, y] = 255;
                        }
                        else if (neighCount == 3 && random.Next(0, 100) < 70)
                        {
                            writing[x, y] = 255;
                        }
                        else if (neighCount == 4)
                        {
                            writing[x, y] = 255;
                        }
                    }
                }
                map = writing;
            }

            byte[,] writing2 = (byte[,])map.Clone();
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (map[x, y] == 100 || map[x, y] == 101)
                    {
                        continue;
                    }
                    var neighbors = GetNeighbors4(new Vector2Int(x, y)).Where(p => p.x < 0 || p.y < 0 || p.x >= maxX || p.y >= maxY || map[p.x, p.y] == 255);
                    int neighCount = neighbors.Count();
                    if (neighCount == 1)
                        continue;
                    if (neighCount == 2 && random.Next(0, 100) < 30)
                    {
                        writing2[x, y] = 255;
                    }
                    else if (neighCount == 3 && random.Next(0, 100) < 70)
                    {
                        writing2[x, y] = 255;
                    }
                    else if (neighCount == 4)
                    {
                        writing2[x, y] = 255;
                    }
                }
            }
            map = writing2;
#endif
            #endregion
            // BiggerMap
            byte[,] temp;
            #region MakeMapBigger
#if MAKE_MAP_BIGGER
            temp = new byte[maxX * 2, maxY * 2];
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    // Restore map protection
                    if (map[x, y] == 101)
                    {
                        map[x, y] = 0;
                    }

                    if (map[x, y] == 100)
                    {
                        temp[x * 2, y * 2] = map[x, y];
                        temp[x * 2 + 1, y * 2] = 101;
                        temp[x * 2, y * 2 - 1] = 101;
                        temp[x * 2 + 1, y * 2 - 1] = 101;
                    }
                    else
                    {
                        temp[x * 2, y * 2] = map[x, y];
                        temp[x * 2 + 1, y * 2] = map[x, y];
                        temp[x * 2, y * 2 + 1] = map[x, y];
                        temp[x * 2 + 1, y * 2 + 1] = map[x, y];
                    }
                }
            }
            maxX *= 2;
            maxY *= 2;
            map = temp;
#endif
            #endregion
            // Wall
            #region WallGenerate
#if WALL_GENERATE
            temp = (byte[,])map.Clone();
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (map[x, y] == 255 || map[x, y] == 100 || map[x, y] == 101)
                        continue;
                    var neighbors = GetNeighbors4(new Vector2Int(x, y)).Where(p => p.x < 0 || p.y < 0 || p.x >= maxX || p.y >= maxY || map[p.x, p.y] == 255);
                    int neighCount = neighbors.Count();
                    if (neighCount is >= 1 and < 4)
                    {
                        temp[x, y] = 1;
                    }
                    if (neighCount == 0)
                    {
                        neighbors = GetCorner4(new Vector2Int(x, y)).Where(p => p.x < 0 || p.y < 0 || p.x >= maxX || p.y >= maxY || map[p.x, p.y] == 255);
                        if (neighbors.Count() == 1)
                        {
                            temp[x, y] = 1;
                        }
                    }
                }
            }

            map = temp;

            // Divide the walls, start with the upper and lower walls
            temp = (byte[,])map.Clone();
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (map[x, y] != TypeConstants.WALL)
                        continue;
                    // Upper wall
                    if (y + 1 >= maxY || map[x, y + 1] == TypeConstants.NOTHING)
                    {
                        if (x + 1 >= maxX || map[x + 1, y] == TypeConstants.NOTHING)
                        {
                            temp[x, y] = TypeConstants.UP_RIGHT_WALL;
                        }
                        else if (x - 1 < 0 || map[x - 1, y] == TypeConstants.NOTHING)
                        {
                            temp[x, y] = TypeConstants.UP_LEFT_WALL;
                        }
                        else
                        {
                            temp[x, y] = TypeConstants.UP_WALL;
                        }
                    }
                    // Lower wall
                    if (y - 1 < 0 || map[x, y - 1] == TypeConstants.NOTHING)
                    {
                        if (x + 1 >= maxX || map[x + 1, y] == TypeConstants.NOTHING)
                        {
                            temp[x, y] = TypeConstants.BOTTOM_RIGHT_WALL;
                        }
                        else if (x - 1 < 0 || map[x - 1, y] == TypeConstants.NOTHING)
                        {
                            temp[x, y] = TypeConstants.BOTTOM_LEFT_WALL;
                        }
                        else
                        {
                            temp[x, y] = TypeConstants.BOTTOM_WALL;
                        }
                    }
                }
            }
            map = temp;

            // Left and right
            temp = (byte[,])map.Clone();
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (map[x, y] != TypeConstants.WALL)
                        continue;
                    // Left
                    if (x - 1 < 0 || map[x - 1, y] == TypeConstants.NOTHING)
                    {
                        temp[x, y] = TypeConstants.LEFT_WALL;
                        //Connecting the upper and lower walls
                        if (x - 1 >= 0 && y + 1 < maxY && map[x - 1, y + 1] == TypeConstants.BOTTOM_WALL)
                        {
                            temp[x, y + 1] = TypeConstants.OUT_TO_LEFT_TILE;
                        }
                        if (x - 1 >= 0 && y - 1 >= 0 && map[x - 1, y - 1] == TypeConstants.UP_WALL)
                        {
                            temp[x, y - 1] = TypeConstants.RIGHT_BOTTOM_TILE;
                        }
                    }
                    // Right
                    if (x + 1 >= maxX || map[x + 1, y] == TypeConstants.NOTHING)
                    {
                        temp[x, y] = TypeConstants.RIGHT_WALL;

                        if (x + 1 < maxX && y + 1 < maxY && map[x + 1, y + 1] == TypeConstants.BOTTOM_WALL)
                        {
                            temp[x, y + 1] = TypeConstants.OUT_TO_RIGHT_TILE;
                            temp[x + 1, y + 1] = TypeConstants.SHADOW_WALL_TILE;
                        }
                        if (x + 1 < maxX && y - 1 >= 0 && map[x + 1, y - 1] == TypeConstants.UP_WALL)
                        {
                            temp[x, y - 1] = TypeConstants.LEFT_BOTTOM_TILE;
                        }
                    }
                }
            }
            map = temp;

            temp = (byte[,])map.Clone();
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (map[x, y] == TypeConstants.BOTTOM_WALL && random.Next(0, 100) < 20)
                    {
                        temp[x, y] = TypeConstants.WINDOW_TILE;
                    }
                }
            }

            map = temp;
#endif
            #endregion

            return map;
        }

        /// Generate a random map block layout
        public static List<byte[,]> RandomMap(int count)
        {
            List<byte[,]> results = new List<byte[,]>();
            var layout = RandomChunkLayout(count);
            var maxX = (layout.Max(p => p.x) - layout.Min(p => p.x) + 1) * unitPerChunk / 2;
            var maxY = (layout.Max(p => p.y) - layout.Min(p => p.y) + 1) * unitPerChunk / 2;
            byte[,] map = new byte[maxX, maxY];
            for (int i = 0; i < maxX; i++)
            {
                for (int j = 0; j < maxY; j++)
                {
                    map[i, j] = 255;
                }
            }

            foreach (var layoutPosition in layout)
            {
                var leftBottom = new Vector2Int(layoutPosition.x * unitPerChunk / 2, layoutPosition.y * unitPerChunk / 2);
                // Starting at the bottom left corner of each block, set the cell in it to 0 as a "valid area"
                for (var x = leftBottom.x; x < leftBottom.x + unitPerChunk / 2; x++)
                    for (var y = leftBottom.y; y < leftBottom.y + unitPerChunk / 2; y++)
                    {
                        map[x, y] = 0;
                    }
            }

            var random = new Random(seed);
            // Coastline
            #region CoastlineGenerate
#if COASTLINE_GENERATE
            for (int i = 0; i < 3; i++)
            {
                byte[,] writing = (byte[,])map.Clone();
                for (int x = 0; x < maxX; x++)
                {
                    for (int y = 0; y < maxY; y++)
                    {
                        var neighbors = GetNeighbors4(new Vector2Int(x, y)).Where(p => p.x < 0 || p.y < 0 || p.x >= maxX || p.y >= maxY || map[p.x, p.y] == 255);
                        if (neighbors.Count() != 1)
                            continue;
                        float noise = Mathf.PerlinNoise(x / (float)maxX * 50, y / (float)maxY * 50);
                        // map[x, y] = noise > threshold;
                        if (noise < 0.4f)
                        {

                            writing[x, y] = 255;
                        }
                    }
                }
                map = writing;


                writing = (byte[,])map.Clone();
                for (int x = 0; x < maxX; x++)
                {
                    for (int y = 0; y < maxY; y++)
                    {
                        var neighbors = GetNeighbors4(new Vector2Int(x, y)).Where(p => p.x < 0 || p.y < 0 || p.x >= maxX || p.y >= maxY || map[p.x, p.y] == 255);
                        int neighCount = neighbors.Count();
                        if (neighCount == 1)
                            continue;
                        if (neighCount == 2 && random.Next(0, 100) < 30)
                        {
                            writing[x, y] = 255;
                        }
                        else if (neighCount == 3 && random.Next(0, 100) < 70)
                        {
                            writing[x, y] = 255;
                        }
                        else if (neighCount == 4)
                        {
                            writing[x, y] = 255;
                        }
                    }
                }
                map = writing;
            }

            byte[,] writing2 = (byte[,])map.Clone();
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    var neighbors = GetNeighbors4(new Vector2Int(x, y)).Where(p => p.x < 0 || p.y < 0 || p.x >= maxX || p.y >= maxY || map[p.x, p.y] == 255);
                    int neighCount = neighbors.Count();
                    if (neighCount == 1)
                        continue;
                    if (neighCount == 2 && random.Next(0, 100) < 30)
                    {
                        writing2[x, y] = 255;
                    }
                    else if (neighCount == 3 && random.Next(0, 100) < 70)
                    {
                        writing2[x, y] = 255;
                    }
                    else if (neighCount == 4)
                    {
                        writing2[x, y] = 255;
                    }
                }
            }
            map = writing2;
#endif
            #endregion
            // BiggerMap
            byte[,] temp;
            #region MakeMapBigger
#if MAKE_MAP_BIGGER
            temp = new byte[maxX * 2, maxY * 2];
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    temp[x * 2, y * 2] = map[x, y];
                    temp[x * 2 + 1, y * 2] = map[x, y];
                    temp[x * 2, y * 2 + 1] = map[x, y];
                    temp[x * 2 + 1, y * 2 + 1] = map[x, y];
                }
            }
            maxX *= 2;
            maxY *= 2;
            map = temp;
#endif
            #endregion
            // Wall
            #region WallGenerate
#if WALL_GENERATE
            temp = (byte[,])map.Clone();
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (map[x, y] == 255)
                        continue;
                    var neighbors = GetNeighbors4(new Vector2Int(x, y)).Where(p => p.x < 0 || p.y < 0 || p.x >= maxX ||
 p.y >= maxY || map[p.x, p.y] == 255);
                    int neighCount = neighbors.Count();
                    if (neighCount is >= 1 and < 4)
                    {
                        temp[x, y] = 1;
                    }
                    if (neighCount == 0)
                    {
                        neighbors = GetCorner4(new Vector2Int(x, y)).Where(p => p.x < 0 || p.y < 0 || p.x >= maxX ||
p.y >= maxY || map[p.x, p.y] == 255);
                        if (neighbors.Count() == 1)
                        {
                            temp[x, y] = 1;
                        }
                    }
                }
            }

            map = temp;

            temp = (byte[,])map.Clone();
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (map[x, y] != TypeConstants.WALL)
                        continue;

                    if (y + 1 >= maxY || map[x, y + 1] == TypeConstants.NOTHING)
                    {
                        if (x + 1 >= maxX || map[x + 1, y] == TypeConstants.NOTHING)
                        {
                            temp[x, y] = TypeConstants.UP_RIGHT_WALL;
                        }
                        else if (x - 1 < 0 || map[x - 1, y] == TypeConstants.NOTHING)
                        {
                            temp[x, y] = TypeConstants.UP_LEFT_WALL;
                        }
                        else
                        {
                            temp[x, y] = TypeConstants.UP_WALL;
                        }
                    }

                    if (y - 1 < 0 || map[x, y - 1] == TypeConstants.NOTHING)
                    {
                        if (x + 1 >= maxX || map[x + 1, y] == TypeConstants.NOTHING)
                        {
                            temp[x, y] = TypeConstants.BOTTOM_RIGHT_WALL;
                        }
                        else if (x - 1 < 0 || map[x - 1, y] == TypeConstants.NOTHING)
                        {
                            temp[x, y] = TypeConstants.BOTTOM_LEFT_WALL;
                        }
                        else
                        {
                            temp[x, y] = TypeConstants.BOTTOM_WALL;
                        }
                    }
                }
            }
            map = temp;

            temp = (byte[,])map.Clone();
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (map[x, y] != TypeConstants.WALL)
                        continue;

                    if (x - 1 < 0 || map[x - 1, y] == TypeConstants.NOTHING)
                    {
                        temp[x, y] = TypeConstants.LEFT_WALL;

                        if (x - 1 >= 0 && y + 1 < maxY && map[x - 1, y + 1] == TypeConstants.BOTTOM_WALL)
                        {
                            temp[x, y + 1] = TypeConstants.OUT_TO_LEFT_TILE;
                        }
                        if (x - 1 >= 0 && y - 1 >= 0 && map[x - 1, y - 1] == TypeConstants.UP_WALL)
                        {
                            temp[x, y - 1] = TypeConstants.RIGHT_BOTTOM_TILE;
                        }
                    }

                    if (x + 1 >= maxX || map[x + 1, y] == TypeConstants.NOTHING)
                    {
                        temp[x, y] = TypeConstants.RIGHT_WALL;

                        if (x + 1 < maxX && y + 1 < maxY && map[x + 1, y + 1] == TypeConstants.BOTTOM_WALL)
                        {
                            temp[x, y + 1] = TypeConstants.OUT_TO_RIGHT_TILE;
                            temp[x + 1, y + 1] = TypeConstants.SHADOW_WALL_TILE;
                        }
                        if (x + 1 < maxX && y - 1 >= 0 && map[x + 1, y - 1] == TypeConstants.UP_WALL)
                        {
                            temp[x, y - 1] = TypeConstants.LEFT_BOTTOM_TILE;
                        }
                    }
                }
            }
            map = temp;

            temp = (byte[,])map.Clone();
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (map[x, y] == TypeConstants.BOTTOM_WALL && random.Next(0, 100) < 20)
                    {
                        temp[x, y] = TypeConstants.WINDOW_TILE;
                    }
                }
            }

            map = temp;
#endif
            #endregion
            // Road
            # region RoadGenerate

            #if ROAD_GENERATE
            int noiseScale = 8;
            foreach (var layoutPosition in layout)
            {
                foreach (var position in layout)
                {
                    if (Vector2Int.Distance(layoutPosition, position) - 1 < 0.01f)
                    {
                        int maxPx = Math.Max(layoutPosition.x, position.x) * unitPerChunk + unitPerChunk / 2;
                        int minPx = Math.Min(layoutPosition.x, position.x) * unitPerChunk + unitPerChunk / 2;
                        int maxPy = Math.Max(layoutPosition.y, position.y) * unitPerChunk + unitPerChunk / 2;
                        int minPy = Math.Min(layoutPosition.y, position.y) * unitPerChunk + unitPerChunk / 2;
                        if (maxPx == minPx)
                        {
                            minPx--;
                            for (int x = minPx; x <= maxPx; x++)
                            {
                                for (int y = minPy; y <= maxPy + random.Next(0, 5); y++)
                                {
                                    float noise = Mathf.PerlinNoise(x * 50 / (float)maxX, y * 50 / (float)maxY);

                                    if (noise < 0.3f)
                                    {
                                        map[x + (int)((noise - 0.5) * noiseScale), y] = TypeConstants.ROAD;
                                    }
                                    else if (noise < 0.7f)
                                    {
                                        map[x + (int)((noise - 0.5) * noiseScale), y] = TypeConstants.GRASS_ROAD_TILE;
                                    }
                                    else
                                    {
                                        map[x + (int)((noise - 0.5) * noiseScale), y] = TypeConstants.GRASS;
                                    }
                                }
                            }
                            // The damage effect of the road
                            if (random.Next(0, 100) < 30)
                            {
                                int x = random.Next(minPx, maxPx);
                                int brokenNum = random.Next(minPy, maxPy);
                                for (int y = minPy; y <= brokenNum; y++)
                                {
                                    map[x, y] = TypeConstants.GRASS;
                                }
                            }
                        }

                        if (maxPy == minPy)
                        {
                            minPy--;

                            for (int x = minPx; x <= maxPx + random.Next(0, 5); x++)
                            {
                                for (int y = minPy; y <= maxPy; y++)
                                {
                                    float noise = Mathf.PerlinNoise(x * 50 / (float)maxX, y * 50 / (float)maxY);

                                    if (noise < 0.3f)
                                    {
                                        map[x, y + (int)((noise - 0.5) * noiseScale)] = TypeConstants.ROAD;
                                    }
                                    else if (noise < 0.6f)
                                    {
                                        map[x, y + (int)((noise - 0.5) * noiseScale)] = TypeConstants.GRASS_ROAD_TILE;
                                    }
                                    else
                                    {
                                        map[x, y + (int)((noise - 0.5) * noiseScale)] = TypeConstants.GRASS;
                                    }
                                }
                            }

                            if (random.Next(0, 100) < 30)
                            {
                                int y = random.Next(minPy, maxPy);
                                int brokenNum = random.Next(minPx, maxPx);
                                for (int x = minPx; x <= brokenNum; x++)
                                {
                                    map[x, y] = TypeConstants.GRASS;
                                }
                            }
                        }
                    }
                }
            }
            #endif
            #endregion

            results.Add(map);
            results.Add(RandomChunkLayoutSecondLayer(layout));
            return results;
        }

        /// Generate the projection of the second layer on the first layer, the second layer and the first layer must be the same size
        public static byte[,] GenerateShadow(byte[,] layer1, byte[,] layer2, int offsetX = 0, int offsetY = 0)
        {
            int maxX = layer2.GetLength(0);
            int maxY = layer2.GetLength(1);

            byte[,] map = new byte[maxX, maxY];
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    map[x, y] = TypeConstants.NOTHING;
                }
            }

            #region ShadowGenerate
            #if SHADOW_GENERATE
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    if (layer2[x, y] == TypeConstants.RIGHT_WALL)
                    {
                        // There is no plot on the right side of the second floor
                        if (x + 1 < maxX && layer2[x + 1, y] != TypeConstants.NOTHING)
                        {
                            continue;
                        }
                        // Confirm if there is a first floor plot on the right
                        int layer1X = x + offsetX + 1;
                        int layer1Y = y + offsetY;
                        if (layer1X < 0 || layer1X >= maxX || layer1Y < 0 || layer1Y >= maxY || layer1[layer1X, layer1Y] == TypeConstants.NOTHING)
                        {
                            continue;
                        }

                        map[layer1X, layer1Y] = TypeConstants.SHADOW_HALF;

                    }
                    if (layer2[x, y] == TypeConstants.UP_RIGHT_WALL)
                    {

                        if (x + 1 < maxX && layer2[x + 1, y] != TypeConstants.NOTHING)
                        {
                            continue;
                        }

                        int layer1X = x + offsetX + 1;
                        int layer1Y = y + offsetY;
                        if (layer1X < 0 || layer1X >= maxX || layer1Y < 0 || layer1Y >= maxY || layer1[layer1X, layer1Y] == TypeConstants.NOTHING)
                        {
                            continue;
                        }

                        map[layer1X, layer1Y] = TypeConstants.SHADOW_QUARTER;

                    }
                    if (layer2[x, y] == TypeConstants.BOTTOM_RIGHT_WALL)
                    {

                        if (x + 1 < maxX && layer2[x + 1, y] != TypeConstants.NOTHING)
                        {
                            continue;
                        }

                        int layer1X = x + offsetX + 1;
                        int layer1Y = y + offsetY;
                        if (layer1X < 0 || layer1X >= maxX || layer1Y < 0 || layer1Y >= maxY || layer1[layer1X, layer1Y] == TypeConstants.NOTHING)
                        {
                            continue;
                        }

                        map[layer1X, layer1Y] = TypeConstants.SHADOW_HALF;
                        if (layer1Y - 1 < 0 || layer1[layer1X, layer1Y] == TypeConstants.NOTHING)
                        {
                            continue;
                        }
                        map[layer1X, layer1Y - 1] = TypeConstants.SHADOW_HALF_INCOMPLETE;

                    }

                }
            }


            #endif
            #endregion

            return map;
        }



        static List<int> canHaveProp = new List<int>()
            { TypeConstants.GRASS, TypeConstants.ROAD, TypeConstants.GRASS_ROAD_TILE };
        static List<int> outWall = new List<int>()
        { TypeConstants.BOTTOM_RIGHT_WALL, TypeConstants.BOTTOM_LEFT_WALL, TypeConstants.BOTTOM_WALL,
            TypeConstants.SHADOW_WALL_TILE, TypeConstants.WINDOW_TILE };

        /// Determine whether the floor of the first layer can place objects
        public static bool layer1CanUse(byte[,] map, byte[,] layer1, byte[,] layer2, int x, int y,
            int offsetX = 0, int offsetY = 0, int width = 1, int height = 1,
            int generateObject = -1)
        {
            int maxX = layer2.GetLength(0) + offsetX;
            int maxY = layer2.GetLength(1) + offsetY;
            for (int i = x - width / 2; i < x + width / 2 + width % 2; i++)
            {
                for (int j = y - height / 2; j < y + height / 2 + height % 2; j++)
                {

                    if (i >= maxX - offsetX || j >= maxY - offsetY || i < 0 || j < 0)
                    {
                        return false;
                    }
                    if (!canHaveProp.Contains(layer1[i, j]))
                    {
                        return false;
                    }

                    // Be occupied
                    if (map[i, j] != TypeConstants.OBJECT_GRASS_LAYER1 && map[i, j] != TypeConstants.NOTHING)
                    {
                        return false;
                    }
                    int layer2X = i - offsetX;
                    int layer2Y = j - offsetY;
                    if (layer2X >= 0 && layer2X < maxX && layer2Y >= 0 && layer2Y < maxY)
                    {
                        // This is the lower wall
                        if (layer2[layer2X, layer2Y] != TypeConstants.NOTHING ||
                            layer2Y + 1 < maxY && outWall.Contains(layer2[layer2X, layer2Y + 1]))
                        {
                            return false;
                        }
                    }
                }
            }

            if (generateObject >= 0)
            {
                for (int i = -width / 2; i < width / 2 + width % 2; i++)
                {
                    for (int j = -height / 2; j < height / 2 + height % 2; j++)
                    {
                        if (i == 0 && j == 0)
                        {

                            map[x + i, y + j] = (byte)generateObject;
                        }
                        else
                        {
                            map[x + i, y + j] = TypeConstants.OBJECT_USING;
                        }
                    }
                }
            }

            return true;
        }


        /// Determine whether the floor of the second layer can place objects
        public static bool layer2CanUse(byte[,] map, byte[,] layer2, int x, int y,
            int offsetX = 0, int offsetY = 0, int width = 1, int height = 1,
            int generateObject = -1)
        {
            int maxX = layer2.GetLength(0);
            int maxY = layer2.GetLength(1);
            for (int i = x - width / 2; i < x + width / 2 + width % 2; i++)
            {
                for (int j = y - height / 2; j < y + height / 2 + height % 2; j++)
                {
                    if (i >= maxX || j >= maxY || i < 0 || j < 0)
                    {
                        return false;
                    }
                    if (!canHaveProp.Contains(layer2[i, j]))
                    {
                        return false;
                    }

                    if (map[i + offsetX, j + offsetY] != TypeConstants.OBJECT_GRASS_LAYER2 && map[i + offsetX, j + offsetY] != TypeConstants.NOTHING)
                    {
                        return false;
                    }
                }
            }

            if (generateObject >= 0)
            {

                for (int i = -width / 2; i < width / 2 + width % 2; i++)
                {
                    for (int j = -height / 2; j < height / 2 + height % 2; j++)
                    {
                        if (i == 0 && j == 0)
                        {
                            map[x + offsetX + i, y + offsetY + j] = (byte)generateObject;
                        }
                        else
                        {
                            map[x + offsetX + i, y + offsetY + j] = TypeConstants.OBJECT_USING;
                        }
                    }
                }
            }

            return true;
        }


        /// Spawn grass and other items
        public static byte[,] GenerateProps(byte[,] layer1, byte[,] layer2, int offsetX = 0, int offsetY = 0)
        {
            int maxX = layer2.GetLength(0) + offsetX;
            int maxY = layer2.GetLength(1) + offsetY;
            // Expand the map
            byte[,] map = new byte[maxX, maxY];
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    map[x, y] = TypeConstants.NOTHING;
                }
            }

            // Layer1
            for (int x = 0; x < maxX - offsetX; x++)
            {
                for (int y = 0; y < maxY - offsetY; y++)
                {
                    if (!layer1CanUse(map, layer1, layer2, x, y, offsetX, offsetY))
                    {
                        continue;
                    }

                    float noise = Mathf.PerlinNoise(x / (float)maxX * 100, y / (float)maxY * 100);
                    if (noise < 0.15)
                    {
#if PLANT_GENERATE
                        map[x, y] = TypeConstants.OBJECT_GRASS_LAYER1;
#endif
                    }
                    else if (noise < 0.2)
                    {
#if PLANT_GENERATE
                        map[x, y] = TypeConstants.OBJECT_ONE_PLANT_LAYER1;
#endif
                    }
                    else if (noise < 0.225)
                    {
#if PLANT_GENERATE
                        layer1CanUse(map, layer1, layer2, x, y, offsetX, offsetY, 2, 2,
                            TypeConstants.OBJECT_TWO_PLANT_LAYER1);
#endif
                    }
                    else if (noise < 0.25)
                    {
#if PLANT_GENERATE
                        layer1CanUse(map, layer1, layer2, x, y, offsetX, offsetY, 4, 4,
                            TypeConstants.OBJECT_TREE_LAYER1);
#endif
                    }
                    else if (noise < 0.2625)
                    {
#if PROP_GENERATE
                        map[x, y] = TypeConstants.OBJECT_1X1_PROP_LAYER1;
#endif
                    }
                    else if (noise < 0.275)
                    {
#if PROP_GENERATE
                        map[x, y] = TypeConstants.OBJECT_1X1_PROP_GRASS_LAYER1;
#endif
                    }
                    else if (noise < 0.287)
                    {
#if PROP_GENERATE
                        layer1CanUse(map, layer1, layer2, x, y, offsetX, offsetY, 2, 2,
                            TypeConstants.OBJECT_2X2_PROP_LAYER1);
#endif
                    }
                    else if (noise < 0.3)
                    {
#if PROP_GENERATE
                        layer1CanUse(map, layer1, layer2, x, y, offsetX, offsetY, 2, 2,
                            TypeConstants.OBJECT_2X2_PROP_GRASS_LAYER1);
#endif
                    }
                    else if (noise < 0.315)
                    {
#if MAIN_PROP_GENERATE
                        layer1CanUse(map, layer1, layer2, x, y, offsetX, offsetY, 6, 6,
                            TypeConstants.OBJECT_6X6_PROP_GRASS_LAYER1);
#endif
                    }
                    else if (noise < 0.5)
                    {
#if MAIN_PROP_GENERATE
                        // Chair prerequisite, against the next level of the wall
                        if (x - offsetX >= 0 && y - offsetY + 2 >= 0 && y + 2 < maxX &&
                            layer2[x - offsetX, y - offsetY + 2] == TypeConstants.BOTTOM_WALL &&
                            x - 1 >= 0 && canHaveProp.Contains(layer1[x - 1, y]))
                        {
                            map[x, y] = TypeConstants.OBJECT_CHAIR;
                            map[x - 1, y] = TypeConstants.OBJECT_USING;
                        }
#endif
                    }
                }
            }
            // Layer2
            for (int x = 0; x < maxX - offsetX; x++)
            {
                for (int y = 0; y < maxY - offsetY; y++)
                {
                    if (!layer2CanUse(map, layer2, x, y, offsetX, offsetY))
                    {
                        continue;
                    }

                    float noise = Mathf.PerlinNoise(x / (float)maxX * 100, y / (float)maxY * 100);
                    if (noise < 0.15)
                    {
#if PLANT_GENERATE
                        map[x + offsetX, y + offsetY] = TypeConstants.OBJECT_GRASS_LAYER2;
#endif
                    }
                    else if (noise < 0.2)
                    {
#if PLANT_GENERATE
                        map[x + offsetX, y + offsetY] = TypeConstants.OBJECT_ONE_PLANT_LAYER2;
#endif
                    }
                    else if (noise < 0.225)
                    {
#if PLANT_GENERATE
                        layer2CanUse(map, layer2, x, y, offsetX, offsetY, 2, 2,
                            TypeConstants.OBJECT_TWO_PLANT_LAYER2);
#endif
                    }
                    else if (noise < 0.25)
                    {
#if PLANT_GENERATE
                        layer2CanUse(map, layer2, x, y, offsetX, offsetY, 4, 4,
                            TypeConstants.OBJECT_TREE_LAYER2);
#endif
                    }
                    else if (noise < 0.2625)
                    {
#if PROP_GENERATE
                        map[x + offsetX, y + offsetY] = TypeConstants.OBJECT_1X1_PROP_LAYER2;
#endif
                    }
                    else if (noise < 0.275)
                    {
#if PROP_GENERATE
                        map[x + offsetX, y + offsetY] = TypeConstants.OBJECT_1X1_PROP_GRASS_LAYER2;
#endif
                    }
                    else if (noise < 0.287)
                    {
#if PROP_GENERATE
                        layer2CanUse(map, layer2, x, y, offsetX, offsetY, 2, 2,
                            TypeConstants.OBJECT_2X2_PROP_LAYER2);
#endif
                    }
                    else if (noise < 0.3)
                    {
#if PROP_GENERATE
                        layer2CanUse(map, layer2, x, y, offsetX, offsetY, 2, 2,
                            TypeConstants.OBJECT_2X2_PROP_GRASS_LAYER2);
#endif
                    }
                    else if (noise < 0.312)
                    {
#if PROP_GENERATE
                        layer2CanUse(map, layer2, x, y, offsetX, offsetY, 6, 6,
                            TypeConstants.OBJECT_6X6_PROP_GRASS_LAYER2);
#endif
                    }
                }
            }

            return map;
        }

    }

}
