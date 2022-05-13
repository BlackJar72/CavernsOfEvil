using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public struct BoxBounds 
    {
        public int minx, maxx, miny, maxy, minz, maxz;
        public BoxBounds(int minx, int maxx, int miny, int maxy, int minz, int maxz) 
        {
            this.minx = minx;
            this.maxx = maxx;
            this.miny = miny;
            this.maxy = maxy;
            this.minz = minz;
            this.maxz = maxz;
        }
    }


    public partial class MapMatrix
    {
        public void ResetUsed()
        {
            for (int i = 0; i < size.area; i++)
            {
                used[i] = false;
            }
        }


        public void ResetUsed(int startx, int startz, int endx, int endz)
        {
            for (int i = startx; i < endx; i++)
                for (int j = startz; j < endz; j++)
                {
                    used[(j * size.width) + i] = false;
                }
        }


        public void ResetUsed(Room room)
        {
            for (int i = Mathf.Max(room.beginX - 1, 0); i <= room.endX; i++)
                for (int j = Mathf.Max(room.beginZ - 1, 0); j <= room.endZ; j++)
                {
                    used[(j * size.width) + i] = false;
                }
        }


        public void MeshRoom(Room room)
        {
            if(room is Cave)
            {
                MeshCave(ref room);
            }
            else
            {
                /*
                   List<Quad> floorQuads = new List<Quad>();
                   List<Quad> ceilingQuads = new List<Quad>();
                   MeshWalls(ref room, ref floorQuads, ref ceilingQuads);
                   MeshFlatsGreedily(ref room, Vector3.up, ref floorQuads);
                   MeshFlatsGreedily(ref room, Vector3.down, ref ceilingQuads);
                   MeshLiquidsGreedily(ref room);
                */
                List<Quad> floorQuads = new List<Quad>();
                List<Quad> ceilingQuads = new List<Quad>();
                List<BoxBounds> wallsBoxes = new List<BoxBounds>();
                MeshWallBoxily(ref room, ref wallsBoxes);
                MeshFlatsBoxily(ref room, Vector3.up, ref floorQuads);
                MeshFlatsBoxily(ref room, Vector3.down, ref ceilingQuads);
                MeshPillarsBoxily(ref room);
                MeshLiquidsGreedily(ref room);

            }
        }


        public void MeshCave(ref Room room)
        {
            Mesher mesher = room.walls.GetComponent<Mesher>();
            List<Quad> quads = new List<Quad>();
            MeshWallW(ref room, ref quads);
            MeshWallE(ref room, ref quads);
            MeshWallN(ref room, ref quads);
            MeshWallS(ref room, ref quads);
            MeshDoors(ref room, ref quads);
            MeshPillars(ref room);
            Quad[] qArray = quads.ToArray();
            mesher.BuildWallsMesh(ref qArray);
            room.floor.GetComponent<Mesher>().BuildCaveFloorMesh(room as Cave);
            room.ceiling.GetComponent<Mesher>().BuildCaveRoofMesh(room as Cave);
        }


        #region Mesh Floor

        private void MeshFlatsGreedily(ref Room room, Vector3 normal, ref List<Quad> quads)
        {
            int count = 0;
            ResetUsed(room);
            int x = 0, z = 0, index, height = 0;
            bool more, available;
            Vector3Int ul, ur, ll, lr;
            Mesher mesher;
            int[] flats;
            Coords coords = new Coords(0, 0);
            if (normal.y > 0)
            {
                flats = floorY;
                mesher = room.floor.GetComponent<Mesher>();
            }
            else
            {
                flats = ceilY;
                mesher = room.ceiling.GetComponent<Mesher>();
            }
            do
            {
                more = false;
                for (int i = Mathf.Max(room.beginX - 1, 0); i <= room.endX; i++)
                {
                    for (int j = Mathf.Max(room.beginZ - 1, 0); j <= room.endZ; j++)
                    {
                        index = (j * size.width) + i;
                        if (!used[index] && (rooms[index] == room.id))
                        {
                            coords.x = x = i;
                            coords.z = z = j;
                            more = used[index] = true;
                            height = flats[index];
                            break;
                        }
                        if (more) break;
                    }
                }
                if (!more) break;
                // Find width of face
                do
                {
                    x++;
                    index = ((z * size.width) + x);
                } while ((x <= room.endX) && !used[index] && (flats[index] == height) 
                                          && (rooms[index] == room.id));
                // Find length of face
                do
                {
                    z++;
                    available = true;
                    for (int i = coords.x; available && (i < x); i++)
                    {
                        index = ((z * size.width)) + i;
                        available = (z <= room.endZ) && !used[index] 
                                    && (flats[index] == height) && (rooms[index] == room.id);
                    }
                } while (available);
                for (int i = coords.x; i < x; i++)
                    for (int j = coords.z; j < z; j++)
                    {
                        used[(j * size.width) + i] = true;
                    }
                ll = new Vector3Int(coords.x, height, coords.z);
                ul = new Vector3Int(coords.x, height, z);
                ur = new Vector3Int(x, height, z);
                lr = new Vector3Int(x, height, coords.z);
                quads.Add(new Quad(ll, ul, lr, ur, height));
                count++;
            } while (more);
            Quad[] qArray = quads.ToArray();
            mesher.BuildFlatsMesh(ref qArray, normal);
        }


        private void MeshLiquidsGreedily(ref Room room)
        {
            int count = 0;
            ResetUsed(room);
            float fheight;
            int x = 0, z = 0, index, height = 0;
            bool more, available;
            Vector3 ul, ur, ll, lr;
            List<Quad> quads = new List<Quad>();
            Mesher mesher;
            int[] flats;
            Coords coords = new Coords(0, 0);
            flats = floorY;
            mesher = room.liquids.GetComponent<Mesher>();
            do
            {
                more = false;
                for (int i = Mathf.Max(room.beginX - 1, 0); i <= room.endX; i++)
                {
                    for (int j = Mathf.Max(room.beginZ - 1, 0); j <= room.endZ; j++)
                    {
                        index = (j * size.width) + i;
                        if (!used[index] && (pools[index] > 0) && (rooms[index] == room.id))
                        {
                            coords.x = x = i;
                            coords.z = z = j;
                            more = used[index] = true;
                            height = flats[index] + pools[index];
                            break;
                        }
                    }
                    if (more) break;
                }
                if (!more) break;
                // Find width of face
                do
                {
                    x++;
                    index = (z * size.width) + x;
                } while ((x <= room.endX) && !used[index] && (pools[index] > 0) 
                        && (flats[index] + pools[index] == height) && (rooms[index] == room.id));
                // Find length of face
                do
                {
                    z++;
                    available = true;
                    for (int i = coords.x; available && (i < x); i++)
                    {
                        index = (z * size.width) + i;
                        available = (z <= room.endZ) && !used[index] && (pools[index] > 0) 
                                    && (flats[index] + pools[index] == height) && (rooms[index] == room.id);
                    }
                } while (available);
                for (int i = coords.x; i < x; i++)
                    for (int j = coords.z; j < z; j++)
                    {
                        used[(j * size.width) + i] = true;
                    }
                fheight = height - 0.1f;
                ll = new Vector3(coords.x, fheight, coords.z);
                ul = new Vector3(coords.x, fheight, z);
                ur = new Vector3(x, fheight, z);
                lr = new Vector3(x, fheight, coords.z);
                quads.Add(new Quad(ll, ul, lr, ur, height));
                count++;
            } while (more);
            Quad[] qArray = quads.ToArray();
            mesher.BuildLiquidMesh(ref qArray);
        }


        #endregion

        #region Mesh Walls
        private void MeshWalls(ref Room room, ref List<Quad> floorQuads, ref List<Quad> ceilingQuads)
        {
            Mesher mesher = room.walls.GetComponent<Mesher>();
            List<Quad> quads = new List<Quad>();
            MeshWallW(ref room, ref quads);
            MeshWallE(ref room, ref quads);
            MeshWallN(ref room, ref quads);
            MeshWallS(ref room, ref quads);
            MeshDoors(ref room, ref quads);
            MeshLowerX(ref room, ref quads, ref floorQuads);
            MeshLowerZ(ref room, ref quads, ref floorQuads);
            MeshUpperX(ref room, ref quads, ref ceilingQuads);
            MeshUpperZ(ref room, ref quads, ref ceilingQuads);
            MeshPillars(ref room);
            Quad[] qArray = quads.ToArray();
            mesher.BuildWallsMesh(ref qArray);
        }


        #region Mesh Walls Proper
        private void MeshWallE(ref Room room, ref List<Quad> quads)
        {
            ResetUsed(room);
            Vector3Int ul, ur, ll, lr;
            Coords c1 = new Coords(0, 0);
            int j2, fheight, cheight, door;
            for (int j = room.beginZ; j <= room.endZ; j++)
                for (int i = room.beginX; i <= room.endX; i++)
                {
                    int index = (j * size.width) + i;
                    if (rooms[index] != room.id) continue;
                    if (!used[index] && isWall[index] 
                        && (((i + 1) >= room.endX) || !isWall[index + 1]))
                    {
                        c1.x = i + 1;
                        c1.z = j;
                        j2 = j;
                        fheight = floorY[index];
                        cheight = ceilY[index];
                        door = doors[index];
                        do
                        {
                            used[index] = true;
                            j2++;
                            index = (j2 * size.width) + i;
                        } while ((j2 <= room.endZ) && isWall[index] 
                                && ((c1.x >= room.endX) || !isWall[index + 1])
                                && (floorY[index] == fheight) && (ceilY[index] == cheight) && (door == doors[index])
                                && (rooms[index] == room.id));
                        if (cheight > (fheight + door))
                        {
                            lr = new Vector3Int(c1.x, fheight + door, j2);
                            ur = new Vector3Int(c1.x, cheight, j2);
                            ul = new Vector3Int(c1.x, cheight, c1.z);
                            ll = new Vector3Int(c1.x, fheight + door, c1.z);
                            quads.Add(new Quad(ll, ul, lr, ur, 0));
                        }
                    }
                }
        }


        private void MeshWallW(ref Room room, ref List<Quad> quads)
        {
            ResetUsed(room);
            Vector3Int ul, ur, ll, lr;
            Coords c1 = new Coords(0, 0);
            int j2, fheight, cheight, door;
            for (int j = room.beginZ; j <= room.endZ; j++)
                for (int i = room.beginX; i <= room.endX; i++)
                {
                    int index = (j * size.width) + i;
                    if (rooms[index] != room.id) continue;
                    if (!used[index] && isWall[index] 
                        && (((i - 1) < room.beginX) || !isWall[index - 1]))
                    {
                        c1.x = i;
                        c1.z = j;
                        j2 = j;
                        fheight = floorY[index];
                        cheight = ceilY[index];
                        door = doors[index];
                        do
                        {
                            used[index] = true;
                            j2++;
                            index = (j2 * size.width) + i;
                        } while ((j2 <= room.endZ) && !used[index] && isWall[index]
                                && (((i - 1) <= room.beginX) || !isWall[index - 1])
                                && (floorY[index] == fheight) && (ceilY[index] == cheight) && (door == doors[index]) 
                                && (rooms[index] == room.id));
                        if (cheight > (fheight + door))
                        {
                            ll = new Vector3Int(c1.x, fheight + door, j2);
                            ul = new Vector3Int(c1.x, cheight, j2);
                            ur = new Vector3Int(c1.x, cheight, c1.z);
                            lr = new Vector3Int(c1.x, fheight + door, c1.z);
                            quads.Add(new Quad(ll, ul, lr, ur, 0));
                        }
                    }
                }
        }


        private void MeshWallN(ref Room room, ref List<Quad> quads)
        {
            ResetUsed(room);
            Vector3Int ul, ur, ll, lr;
            Coords c1 = new Coords(0, 0);
            int i2, fheight, cheight, door;
            for (int i = room.beginX; i <= room.endX; i++)
                for (int j = room.beginZ; j <= room.endZ; j++)
                {
                    int index = (j * size.width) + i;
                    if (rooms[index] != room.id) continue;
                    if (!used[index] && isWall[index] 
                        && (((j + 1) >= room.endZ) || !isWall[index + size.width]))
                    {
                        c1.x = i;
                        c1.z = j + 1;
                        i2 = i;
                        fheight = floorY[index];
                        cheight = ceilY[index];
                        door = doors[index];
                        do
                        {
                            used[index] = true;
                            i2++;
                            index = (j * size.width) + i2;
                        } while ((i2 <= room.endX) && isWall[index] 
                                && ((c1.z >= room.endZ) || !isWall[index + size.width])
                                && (floorY[index] == fheight) && (ceilY[index] == cheight) && (door == doors[index]) 
                                && (rooms[index] == room.id));
                        if (cheight > (fheight + door))
                        {
                            ll = new Vector3Int(i2, fheight + door, c1.z);
                            ul = new Vector3Int(i2, cheight, c1.z);
                            ur = new Vector3Int(c1.x, cheight, c1.z);
                            lr = new Vector3Int(c1.x, fheight + door, c1.z);
                            quads.Add(new Quad(ll, ul, lr, ur, 0));
                        }
                    }
                }
        }


        private void MeshWallS(ref Room room, ref List<Quad> quads)
        {
            ResetUsed(room);
            Vector3Int ul, ur, ll, lr;
            Coords c1 = new Coords(0, 0);
            int i2, fheight, cheight, door;
            for (int i = room.beginX; i <= room.endX; i++)
                for (int j = room.beginZ; j <= room.endZ; j++)
                {
                    int index = (j * size.width) + i;
                    if (rooms[index] != room.id) continue;
                    if (!used[index] && isWall[index] 
                        && (((j - 1) < room.beginZ) || !isWall[index - size.width]))
                    {
                        c1.x = i;
                        c1.z = j;
                        i2 = i;
                        fheight = floorY[index];
                        cheight = ceilY[index];
                        door = doors[index];
                        do
                        {
                            used[index] = true;
                            i2++;
                            index = (j * size.width) + i2;
                        } while ((i2 <= room.endX) && isWall[index] 
                                && ((c1.z <= room.beginZ) || !isWall[index - size.width])
                                && (floorY[index] == fheight) && (ceilY[index] == cheight) && (door == doors[index]) 
                                && (rooms[index] == room.id));
                        if (cheight > (fheight + door))
                        {
                            lr = new Vector3Int(i2, fheight + door, c1.z);
                            ur = new Vector3Int(i2, cheight, c1.z);
                            ul = new Vector3Int(c1.x, cheight, c1.z);
                            ll = new Vector3Int(c1.x, fheight + door, c1.z);
                            quads.Add(new Quad(ll, ul, lr, ur, 0));
                        }
                    }
                }
        }
        #endregion


        private void MeshDoors(ref Room room, ref List<Quad> quads)
        {
            int index1, index2, i1, j1, top;
            Vector3Int ul, ur, ll, lr;
            for (int i = room.beginX; i <= room.endX; i++)
                for (int j = room.beginZ; j <= room.endZ; j++)
                {
                    index1 = (j * size.width) + i;
                    if (!isWall[index1] || (rooms[index1] != room.id)) continue;
                    if (doors[index1] > 0)
                    {
                        i1 = i + 1;
                        j1 = j + 1;
                        top = floorY[index1] + doors[index1];
                        index2 = index1 + 1;
                        if (((i1 >= size.width) || (index2 >= size.area))
                            || (isWall[index2] && (doors[index2] == 0) && (rooms[index2] == room.id)) 
                            || (rooms[index2] < 1))
                        {
                            lr = new Vector3Int(i1, floorY[index1], j);
                            ur = new Vector3Int(i1, top, j);
                            ul = new Vector3Int(i1, top, j1);
                            ll = new Vector3Int(i1, floorY[index1], j1);
                            quads.Add(new Quad(ll, ul, lr, ur));
                        }
                        index2 = index1 + size.width;
                        if (((j1 >= size.width) || (index2 >= size.area))
                            || (isWall[index2] && (doors[index2] == 0) && (rooms[index2] == room.id)) 
                            || (rooms[index2] < 1))
                        {
                            lr = new Vector3Int(i1, floorY[index1], j1);
                            ur = new Vector3Int(i1, top, j1);
                            ul = new Vector3Int(i, top, j1);
                            ll = new Vector3Int(i, floorY[index1], j1);
                            quads.Add(new Quad(ll, ul, lr, ur));
                        }
                        index2 = index1 - 1;
                        if (((i < 1) || (index2 < 0)) 
                            || (isWall[index2] && (doors[index2] == 0) && (rooms[index2] == room.id))
                            || (rooms[index2] < 1))
                        {
                            ll = new Vector3Int(i, floorY[index1], j);
                            ul = new Vector3Int(i, top, j);
                            ur = new Vector3Int(i, top, j1);
                            lr = new Vector3Int(i, floorY[index1], j1);
                            quads.Add(new Quad(ll, ul, lr, ur));
                        }
                        index2 = index1 - size.width;
                        if (((j < 1) || (index2 < 0)) 
                            || (isWall[index2] && (doors[index2] == 0) && (rooms[index2] == room.id))
                            || (rooms[index2] < 1))
                        {
                            ll = new Vector3Int(i1, floorY[index1], j);
                            ul = new Vector3Int(i1, top, j);
                            ur = new Vector3Int(i, top, j);
                            lr = new Vector3Int(i, floorY[index1], j);
                            quads.Add(new Quad(ll, ul, lr, ur));
                        }
                        if (top < ceilY[index1])
                        {
                            ll = new Vector3Int(i1, top, j);
                            ul = new Vector3Int(i1, top, j1);
                            ur = new Vector3Int(i, top, j1);
                            lr = new Vector3Int(i, top, j);
                            quads.Add(new Quad(ll, ul, lr, ur));
                        }
                    }
                }
        }


        private void MeshPillars(ref Room room)
        {
            Mesher mesher = room.pillars.GetComponent<Mesher>();
            List<Quad> quads = new List<Quad>();
            Vector3Int ul, ur, ll, lr;
            int index, i1, j1;
            for (int i = room.beginX; i <= room.endX; i++)
                for (int j = room.beginZ; j <= room.endZ; j++)
                {
                    index = (j * size.width) + i;
                    if ((rooms[index] == room.id) && isPillar[index])
                    {
                        i1 = i + 1;
                        j1 = j + 1;

                        ll = new Vector3Int(i1, floorY[index], j);
                        ul = new Vector3Int(i1, ceilY[index], j);
                        ur = new Vector3Int(i1, ceilY[index], j1);
                        lr = new Vector3Int(i1, floorY[index], j1);
                        quads.Add(new Quad(ll, ul, lr, ur));

                        ll = new Vector3Int(i, floorY[index], j);
                        ul = new Vector3Int(i, ceilY[index], j);
                        ur = new Vector3Int(i1, ceilY[index], j);
                        lr = new Vector3Int(i1, floorY[index], j);
                        quads.Add(new Quad(ll, ul, lr, ur));

                        ll = new Vector3Int(i, floorY[index], j1);
                        ul = new Vector3Int(i, ceilY[index], j1);
                        ur = new Vector3Int(i, ceilY[index], j);
                        lr = new Vector3Int(i, floorY[index], j);
                        quads.Add(new Quad(ll, ul, lr, ur));

                        ll = new Vector3Int(i1, floorY[index], j1);
                        ul = new Vector3Int(i1, ceilY[index], j1);
                        ur = new Vector3Int(i, ceilY[index], j1);
                        lr = new Vector3Int(i, floorY[index], j1);
                        quads.Add(new Quad(ll, ul, lr, ur));
                    }
                }
            Quad[] qArray = quads.ToArray();
            mesher.BuildWallsMesh(ref qArray);
        }


        #region Mesh Sides
        private void MeshLowerX(ref Room room, ref List<Quad> quads, ref List<Quad> floorQuads)
        {
            ResetUsed(room);
            Vector3 ul, ur, ll, lr;
            int[] flats;
            flats = floorY;
            Coords c1 = new Coords(0, 0);
            float h2;
            int next, end, h0, h1, index0, index1, width1 = room.endX, length1 = room.endZ;
            for (int i = room.beginX; i < width1; i++)
                for (int j = room.beginZ; j < length1; j++)
                {
                    if ((i >= size.width) || (j < 0) || (i >= size.width) || (j >= size.width)) continue;
                    next = i + 1;
                    index0 = (j * size.width) + i;
                    index1 = (j * size.width) + next;
                    if ((next < size.width) && !used[index0] && (flats[index0] > flats[index1]))
                    {
                        c1.z = end = j;
                        h0 = flats[index0];
                        h1 = flats[index1];
                        do
                        {
                            end++;
                            index0 = (end * size.width) + i;
                            index1 = (end * size.width) + next;
                        } while ((end < size.width) && (end <= room.endZ) && !used[index0] && (flats[index0] == h0) 
                                && (flats[index1] == h1));
                        for (int k = j; k < end; k++)
                        {
                            used[(k * size.width) + i] = true;
                        }
                        h2 = h0 - 0.25f;
                        ll = new Vector3(next, h1, c1.z);
                        ul = new Vector3(next, h2, c1.z);
                        ur = new Vector3(next, h2, end);
                        lr = new Vector3(next, h1, end);
                        quads.Add(new Quad(ll, ul, lr, ur));
                        ll = new Vector3(next, h2, c1.z);
                        ul = new Vector3(next, h0, c1.z);
                        ur = new Vector3(next, h0, end);
                        lr = new Vector3(next, h2, end);
                        floorQuads.Add(new Quad(ll, ul, lr, ur));
                    }
                    else if ((next < size.width) && !used[index0] && (flats[index0] < flats[index1]))
                    {
                        c1.z = end = j;
                        h0 = flats[index0];
                        h1 = flats[index1];
                        do
                        {
                            end++;
                            index0 = (end * size.width) + i;
                            index1 = (end * size.width) + next;
                        } while ((end < size.width) && !used[index0] && (flats[index0] == h0) 
                                && (flats[index1] == h1));
                        for (int k = j; k < end; k++)
                        {
                            used[(k * size.width) + i] = true;
                        }
                        h2 = h1 - 0.25f;
                        ll = new Vector3(next, h0, end);
                        ul = new Vector3(next, h2, end);
                        ur = new Vector3(next, h2, c1.z);
                        lr = new Vector3(next, h0, c1.z);
                        quads.Add(new Quad(ll, ul, lr, ur));
                        ll = new Vector3(next, h2, end);
                        ul = new Vector3(next, h1, end);
                        ur = new Vector3(next, h1, c1.z);
                        lr = new Vector3(next, h2, c1.z);
                        floorQuads.Add(new Quad(ll, ul, lr, ur));
                    }
                    else
                    {
                        used[index0] = true;
                    }
                }
        }


        private void MeshLowerZ(ref Room room, ref List<Quad> quads, ref List<Quad> floorQuads)
        {
            ResetUsed(room);
            Vector3 ul, ur, ll, lr;
            int[] flats;
            flats = floorY;
            Coords c1 = new Coords(0, 0);
            float h2;
            int next, end, h0, h1, index0, index1, width1 = room.endX, length1 = room.endZ;
            for (int i = room.beginX; i < width1; i++)
                for (int j = room.beginZ; j < length1; j++)
                {
                    if ((i >= size.width) || (j >= size.width)) continue;
                    next = j + 1;
                    index0 = (j * size.width) + i;
                    index1 = (next * size.width) + i;
                    if ((next < size.width) && !used[index0] && (flats[index0] > flats[index1]))
                    {
                        c1.x = end = i;
                        h0 = flats[index0];
                        h1 = flats[index1];
                        do
                        {
                            end++;
                            index0 = (j * size.width) + end;
                            index1 = (next * size.width) + end;
                        } while ((end < size.width) && !used[index0] && (flats[index0] == h0) 
                                && (flats[index1] == h1));
                        for (int k = i; k < end; k++)
                        {
                            used[(j * size.width) + k] = true;
                        }
                        h2 = h0 - 0.25f;
                        ul = new Vector3(c1.x, h1, next);
                        ll = new Vector3(c1.x, h2, next);
                        lr = new Vector3(end, h2, next);
                        ur = new Vector3(end, h1, next);
                        quads.Add(new Quad(ll, ul, lr, ur));
                        ul = new Vector3(c1.x, h2, next);
                        ll = new Vector3(c1.x, h0, next);
                        lr = new Vector3(end, h0, next);
                        ur = new Vector3(end, h2, next);
                        floorQuads.Add(new Quad(ll, ul, lr, ur));
                    }
                    else if ((next < size.width) && !used[index0] && (flats[index0] < flats[index1]))
                    {
                        c1.x = end = i;
                        h0 = flats[index0];
                        h1 = flats[index1];
                        do
                        {
                            end++;
                            index0 = (j * size.width) + end;
                            index1 = (next * size.width) + end;
                        } while ((end < size.width) && (end <= room.endX) && !used[index0] && (flats[index0] == h0) 
                                && (flats[index1] == h1));
                        for (int k = i; k < end; k++)
                        {
                            used[(j * size.width) + k] = true;
                        }
                        h2 = h1 - 0.25f;
                        ul = new Vector3(end, h0, next);
                        ll = new Vector3(end, h2, next);
                        lr = new Vector3(c1.x, h2, next);
                        ur = new Vector3(c1.x, h0, next);
                        quads.Add(new Quad(ll, ul, lr, ur));
                        ul = new Vector3(end, h2, next);
                        ll = new Vector3(end, h1, next);
                        lr = new Vector3(c1.x, h1, next);
                        ur = new Vector3(c1.x, h2, next);
                        floorQuads.Add(new Quad(ll, ul, lr, ur));
                    }
                    else
                    {
                        used[index0] = true;
                    }
                }
        }


        private void MeshUpperX(ref Room room, ref List<Quad> quads, ref List<Quad> ceilingQuads)
        {
            ResetUsed(room);
            Vector3 ul, ur, ll, lr;
            int[] flats;
            flats = ceilY;
            Coords c1 = new Coords(0, 0);
            float h2;
            int next, end, h0, h1, index0, index1, width1 = room.endX, length1 = room.endZ;
            for (int i = room.beginX; i < width1; i++)
                for (int j = room.beginZ; j < length1; j++)
                {
                    next = i + 1;
                    index0 = (j * size.width) + i;
                    index1 = (j * size.width) + next;
                    if (!used[index0] && (flats[index0] > flats[index1]))
                    {
                        c1.z = end = j;
                        h0 = flats[index0];
                        h1 = flats[index1];
                        do
                        {
                            end++;
                            index0 = (end * size.width) + i;
                            index1 = (end * size.width) + next;
                        } while ((end < size.width) && !used[index0] && (flats[index0] == h0) && (flats[index1] == h1));
                        for (int k = j; k < end; k++)
                        {
                            used[(k * size.width) + i] = true;
                        }
                        h2 = h1 + 0.25f;
                        lr = new Vector3(next, h2, c1.z);
                        ur = new Vector3(next, h0, c1.z);
                        ul = new Vector3(next, h0, end);
                        ll = new Vector3(next, h2, end);
                        quads.Add(new Quad(ll, ul, lr, ur));
                        ll = new Vector3(next, h1, c1.z);
                        ul = new Vector3(next, h2, c1.z);
                        ur = new Vector3(next, h2, end);
                        lr = new Vector3(next, h1, end);
                        ceilingQuads.Add(new Quad(ll, ul, lr, ur));
                    }
                    else if (!used[index0] && (flats[index0] < flats[index1]))
                    {
                        c1.z = end = j;
                        h0 = flats[index0];
                        h1 = flats[index1];
                        do
                        {
                            end++;
                            index0 = (end * size.width) + i;
                            index1 = (end * size.width) + next;
                        } while ((end < size.width) && !used[index0] && (flats[index0] == h0) && (flats[index1] == h1));
                        for (int k = j; k < end; k++)
                        {
                            used[(k * size.width) + i] = true;
                        }
                        h2 = h0 + 0.25f;
                        lr = new Vector3(next, h2, end);
                        ur = new Vector3(next, h1, end);
                        ul = new Vector3(next, h1, c1.z);
                        ll = new Vector3(next, h2, c1.z);
                        quads.Add(new Quad(ll, ul, lr, ur));
                        ll = new Vector3(next, h0, end);
                        ul = new Vector3(next, h2, end);
                        ur = new Vector3(next, h2, c1.z);
                        lr = new Vector3(next, h0, c1.z);
                        ceilingQuads.Add(new Quad(ll, ul, lr, ur));
                    }
                    else
                    {
                        used[index0] = true;
                    }
                }
        }


        private void MeshUpperZ(ref Room room, ref List<Quad> quads, ref List<Quad> ceilingQuads)
        {
            ResetUsed(room);
            Vector3 ul, ur, ll, lr;
            int[] flats;
            flats = ceilY;
            Coords c1 = new Coords(0, 0);
            float h2;
            int next, end, h0, h1, index0, index1, width1 = room.endX, length1 = room.endZ;
            for (int i = room.beginX; i < width1; i++)
                for (int j = room.beginZ; j < length1; j++)
                {
                    next = j + 1;
                    index0 = (j * size.width) + i;
                    index1 = (next * size.width) + i;
                    if (!used[index0] && (flats[index0] > flats[index1]))
                    {
                        c1.x = end = i;
                        h0 = flats[index0];
                        h1 = flats[index1];
                        do
                        {
                            end++;
                            index0 = (j * size.width) + end;
                            index1 = (next * size.width) + end;
                        } while ((end < size.width) && !used[index0] && (flats[index0] == h0) && (flats[index1] == h1));
                        for (int k = i; k < end; k++)
                        {
                            used[(j * size.width) + k] = true;
                        }
                        h2 = h1 + 0.25f;
                        ll = new Vector3(c1.x, h2, next);
                        ul = new Vector3(c1.x, h0, next);
                        ur = new Vector3(end, h0, next);
                        lr = new Vector3(end, h2, next);
                        quads.Add(new Quad(ll, ul, lr, ur));
                        lr = new Vector3(c1.x, h1, next);
                        ur = new Vector3(c1.x, h2, next);
                        ul = new Vector3(end, h2, next);
                        ll = new Vector3(end, h1, next);
                        ceilingQuads.Add(new Quad(ll, ul, lr, ur));
                    }
                    else if (!used[index0] && (flats[index0] < flats[index1]))
                    {
                        c1.x = end = i;
                        h0 = flats[index0];
                        h1 = flats[index1];
                        do
                        {
                            end++;
                            index0 = (j * size.width) + end;
                            index1 = (next * size.width) + end;
                        } while ((end < size.width) && !used[index0] && (flats[index0] == h0) && (flats[index1] == h1));
                        for (int k = i; k < end; k++)
                        {
                            used[(j * size.width) + k] = true;
                        }
                        h2 = h0 + 0.25f;
                        ll = new Vector3(end, h2, next);
                        ul = new Vector3(end, h1, next);
                        ur = new Vector3(c1.x, h1, next);
                        lr = new Vector3(c1.x, h2, next);
                        quads.Add(new Quad(ll, ul, lr, ur));
                        lr = new Vector3(end, h0, next);
                        ur = new Vector3(end, h2, next);
                        ul = new Vector3(c1.x, h2, next);
                        ll = new Vector3(c1.x, h0, next);
                        ceilingQuads.Add(new Quad(ll, ul, lr, ur));
                    } 
                    else
                    {
                        used[index0] = true;
                    }
                }
        }

        #endregion
        #endregion


        #region New Wall Mesher (boxy)
	   
	   
        private void MeshWallBoxily(ref Room room, ref List<BoxBounds> boxes)
        {
            int count = 0;
            ResetUsed(room);
            int x = 0, z = 0, index, bottom = 0, top = 0;
            bool more, available;
            Vector3Int ul, ur, ll, lr;
            Mesher mesher;
            Coords coords = new Coords(0, 0);
            do
            {
                more = false;
                for (int i = Mathf.Max(room.beginX - 1, 0); i <= room.endX; i++)
                {
                    for (int j = Mathf.Max(room.beginZ - 1, 0); j <= room.endZ; j++)
                    {
                        index = (j * size.width) + i;
                        if (isWall[index] && !used[index] && (rooms[index] == room.id))
                        {
                            coords.x = x = i;
                            coords.z = z = j;
                            more = used[index] = true;
                            bottom = GetWallBottom(index);
                            top = GetCeilY(index);
                            break;
                        }
                        if (more) break;
                    }
                }
                if (!more) break;
                // Find width of face
                do
                {
                    x++;
                    index = ((z * size.width) + x);
                } while ((x <= room.endX) && isWall[index] && !used[index] && (GetWallBottom(index) == bottom) 
                                          && (GetCeilY(index) == top) && (rooms[index] == room.id));
                // Find length of face
                do
                {
                    z++;
                    available = true;
                    for (int i = coords.x; available && (i < x); i++)
                    {
                        index = ((z * size.width)) + i;
                        available = (z <= room.endZ) && isWall[index] && !used[index] 
                                    && (GetWallBottom(index) == bottom) && (GetCeilY(index) == top) 
                                    && (rooms[index] == room.id);
                    }
                } while (available);
                for (int i = coords.x; i < x; i++)
                    for (int j = coords.z; j < z; j++)
                    {
                        used[(j * size.width) + i] = true;
                    }
                if (bottom != top)
                {
                    boxes.Add(new BoxBounds(coords.x, x, bottom, top, coords.z, z));
                    count++;
                }
            } while (more);
            mesher = room.walls.GetComponent<Mesher>();
            BoxBounds[] boxArray = boxes.ToArray();
            mesher.BuildWallBoxes(ref boxArray, ref room);
        }


        private void MeshFlatsBoxily(ref Room room, Vector3 normal, ref List<Quad> quads)
        {
            int count = 0;
            ResetUsed(room);
            int x = 0, z = 0, index, height = 0;
            bool more, available;
            Vector3Int ul, ur, ll, lr;
            Mesher mesher;
            int[] flats;
            Coords coords = new Coords(0, 0);
            if (normal.y > 0)
            {
                flats = floorY;
                mesher = room.floor.GetComponent<Mesher>();
            }
            else
            {
                flats = ceilY;
                mesher = room.ceiling.GetComponent<Mesher>();
            }
            do
            {
                more = false;
                for (int i = Mathf.Max(room.beginX - 1, 0); i <= room.endX; i++)
                {
                    for (int j = Mathf.Max(room.beginZ - 1, 0); j <= room.endZ; j++)
                    {
                        index = (j * size.width) + i;
                        if (!used[index] && (rooms[index] == room.id))
                        {
                            coords.x = x = i;
                            coords.z = z = j;
                            more = used[index] = true;
                            height = flats[index];
                            break;
                        }
                        if (more) break;
                    }
                }
                if (!more) break;
                // Find width of face
                do
                {
                    x++;
                    index = ((z * size.width) + x);
                } while ((x <= room.endX) && !used[index] && (flats[index] == height)
                                          && (rooms[index] == room.id));
                // Find length of face
                do
                {
                    z++;
                    available = true;
                    for (int i = coords.x; available && (i < x); i++)
                    {
                        index = ((z * size.width)) + i;
                        available = (z <= room.endZ) && !used[index]
                                    && (flats[index] == height) && (rooms[index] == room.id);
                    }
                } while (available);
                for (int i = coords.x; i < x; i++)
                    for (int j = coords.z; j < z; j++)
                    {
                        used[(j * size.width) + i] = true;
                    }
                ll = new Vector3Int(coords.x, height, coords.z);
                ul = new Vector3Int(coords.x, height, z);
                ur = new Vector3Int(x, height, z);
                lr = new Vector3Int(x, height, coords.z);
                quads.Add(new Quad(ll, ul, lr, ur, height));
                count++;
            } while (more);
            Quad[] qArray = quads.ToArray();
            if(normal == Vector3.up)
                mesher.BuildFloorBoxes(ref qArray, ref room);
            else mesher.BuildCeilingBoxes(ref qArray, ref room);
        }


        private void MeshPillarsBoxily(ref Room room)
        {
            Mesher mesher = room.pillars.GetComponent<Mesher>();
            List<BoxBounds> boxes = new List<BoxBounds>();
            Vector3Int ul, ur, ll, lr;
            int index, i1, j1;
            for (int i = room.beginX; i <= room.endX; i++)
                for (int j = room.beginZ; j <= room.endZ; j++)
                {
                    index = (j * size.width) + i;
                    if ((rooms[index] == room.id) && isPillar[index])
                    {
                        boxes.Add(new BoxBounds(i, i + 1, GetFloorY(index), GetCeilY(index), j, j + 1));
                    }
                }
            BoxBounds[] qArray = boxes.ToArray();
            mesher.BuildWallBoxes(ref qArray, ref room);
        }




        #endregion

    }


}
