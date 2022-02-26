using System.Collections.Generic;
using UnityEngine;


public struct Quadi
{
    public Vector3Int ul, ur, ll, lr;

    public Quadi(Vector3Int ul, Vector3Int ur, Vector3Int ll, Vector3Int lr)
    {
        this.ul = ul;
        this.ur = ur;
        this.ll = ll;
        this.lr = lr;
    }
    public override string ToString()
    {
        return "[ll = " + ll + ", ul = " + ul + ", ur = " + ur + ", lr = " + lr + "]";
    }
}


public struct Quad
{
    public Vector3 ul, ur, ll, lr;

    public Quad(Vector3 ul, Vector3 ur, Vector3 ll, Vector3 lr)
    {
        this.ul = ul;
        this.ur = ur;
        this.ll = ll;
        this.lr = lr;
    }
}


public struct Coords
{
    public int x, z;
    public Coords(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
}


public class TestMap : MonoBehaviour
{
    const int SIZE = 14;       // Actually width and length, which are equal.
    const int RADIUS = SIZE / 2; // Max distance from center (probably 0, 0, 0) on X and Z.
    const int AREA = SIZE * SIZE;

    public GameObject floor;
    public GameObject walls;
    public GameObject pillars;
    public GameObject ceiling;
    public GameObject liquids;

    // Room identity
    int[] room =  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
                    0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
                    0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
                    0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
                    0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
                    0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
                    0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
                    0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
                    0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
                    0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
                    0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
                    0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    // Basic Heights
    int[] floorHeight = { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
                          4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
                          4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
                          4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
                          4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4,
                          4, 4, 4, 4, 3, 2, 2, 2, 2, 3, 4, 4, 4, 4,
                          4, 4, 4, 4, 3, 2, 6, 6, 2, 3, 4, 4, 4, 4,
                          4, 4, 4, 4, 3, 2, 6, 6, 2, 3, 4, 4, 4, 4,
                          4, 4, 4, 4, 3, 2, 2, 2, 2, 3, 4, 4, 4, 4,
                          4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4,
                          4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
                          4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
                          4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
                          4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 };

    int[] ceilHeight =  { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
                          6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
                          6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
                          6, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 6,
                          6, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 6,
                          6, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 6,
                          6, 8, 8, 8, 8, 8, 7, 7, 8, 8, 8, 8, 8, 6,
                          6, 8, 8, 8, 8, 8, 7, 7, 8, 8, 8, 8, 8, 6,
                          6, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 6,
                          6, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 6,
                          6, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 6,
                          6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
                          6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
                          6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 };

    // Other data
    bool[] isWall = { false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                      false, true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  false,
                      false, true,  false, false, false, false, false, false, false, false, false, false, true,  false,
                      false, true,  false, false, false, false, false, false, false, false, false, false, true,  false,
                      false, true,  false, false, false, false, false, false, false, false, false, false, true,  false,
                      false, true,  false, false, false, false, false, false, false, false, false, false, true,  false,
                      false, true,  false, false, false, false, false, false, false, false, false, false, true,  false,
                      false, true,  false, false, false, false, false, false, false, false, false, false, true,  false,
                      false, true,  false, false, false, false, false, false, false, false, false, false, true,  false,
                      false, true,  false, false, false, false, false, false, false, false, false, false, true,  false,
                      false, true,  false, false, false, false, false, false, false, false, false, false, true,  false,
                      false, true,  false, false, false, false, false, false, false, false, false, false, true,  false,
                      false, true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  false,
                      false, false, false, false, false, false, false, false, false, false, false, false, false, false };

    bool[] isPillar = { false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                        false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                        false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                        false, false, false, true,  false, false, false, false, false, false, true,  false, false, false,
                        false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                        false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                        false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                        false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                        false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                        false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                        false, false, false, true,  false, false, false, false, false, false, true,  false, false, false,
                        false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                        false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                        false, false, false, false, false, false, false, false, false, false, false, false, false, false };

    int[] isPool = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                     0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0,
                     0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0,
                     0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0,
                     0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0,
                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    int[] doors = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0,
                    0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };


    // Wokring Scratchpad
    bool[] used = new bool[AREA];


    // Start is called before the first frame update
    void Start()
    {
        MeshRoom();
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void ResetUsed()
    {
        for (int i = 0; i < AREA; i++)
        {
            used[i] = room[i] < 1;
        }
    }



    private void MeshRoom()
    {
        List<Quad> floorQuads = new List<Quad>();
        List<Quad> ceilingQuads = new List<Quad>();
        MeshWalls(ref floorQuads, ref ceilingQuads);
        MeshFlatsGreedily(Vector3.up, ref floorQuads);
        MeshFlatsGreedily(Vector3.down, ref ceilingQuads);
        MeshLiquidsGreedily();
    }


    #region Mesh Floor

    private void MeshFlatsGreedily(Vector3 normal, ref List<Quad> quads)
    {
        int count = 0;
        ResetUsed();
        int x = 0, z = 0, index, height = 0;
        bool more, available;
        Vector3Int ul, ur, ll, lr;
        Mesher mesher;
        int[] flats;
        Coords coords = new Coords(0, 0);
        if (normal.y > 0)
        {
            flats = floorHeight;
            mesher = floor.GetComponent<Mesher>();
        }
        else
        {
            flats = ceilHeight;
            mesher = ceiling.GetComponent<Mesher>();
        }
        do
        {
            more = false;
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    index = (j * SIZE) + i;
                    if (!used[index])
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
                index = (z * SIZE + x);
            } while ((x < SIZE) && !used[index] && (flats[index] == height));
            // Find length of face
            do
            {
                z++;
                available = true;
                for (int i = coords.x; available && (i < x); i++)
                {
                    index = (z * SIZE) + i;
                    available = (z < SIZE) && !used[index] && (flats[index] == height);
                }
            } while (available);
            for (int i = coords.x; i < x; i++)
                for (int j = coords.z; j < z; j++)
                {
                    used[(j * SIZE) + i] = true;
                }
            ll = new Vector3Int(coords.x, height, coords.z);
            ul = new Vector3Int(coords.x, height, z);
            ur = new Vector3Int(x, height, z);
            lr = new Vector3Int(x, height, coords.z);
            quads.Add(new Quad(ll, ul, lr, ur));
            count++;
        } while (more);
        Quad[] qArray = quads.ToArray();
        mesher.BuildFlatsMesh(ref qArray, normal);
    }


    private void MeshLiquidsGreedily()
    {
        int count = 0;
        ResetUsed();
        float fheight;
        int x = 0, z = 0, index, height = 0;
        bool more, available;
        Vector3 ul, ur, ll, lr;
        List<Quad> quads = new List<Quad>();
        Mesher mesher;
        int[] flats;
        Coords coords = new Coords(0, 0);
        flats = floorHeight;
        mesher = liquids.GetComponent<Mesher>();
        do
        {
            more = false;
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    index = (j * SIZE) + i;
                    if (!used[index] && (isPool[index] > 0))
                    {
                        coords.x = x = i;
                        coords.z = z = j;
                        more = used[index] = true;
                        height = flats[index] + isPool[index];
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
                index = (z * SIZE + x);
            } while ((x < SIZE) && !used[index] && (isPool[index] > 0) && (flats[index] + isPool[index] == height));
            // Find length of face
            do
            {
                z++;
                available = true;
                for (int i = coords.x; available && (i < x); i++)
                {
                    index = (z * SIZE) + i;
                    available = (z < SIZE) && !used[index] && (isPool[index] > 0) && (flats[index] + isPool[index] == height);
                }
            } while (available);
            for (int i = coords.x; i < x; i++)
                for (int j = coords.z; j < z; j++)
                {
                    used[(j * SIZE) + i] = true;
                }
            fheight = height - 0.1f;
            ll = new Vector3(coords.x, fheight, coords.z);
            ul = new Vector3(coords.x, fheight, z);
            ur = new Vector3(x, fheight, z);
            lr = new Vector3(x, fheight, coords.z);
            quads.Add(new Quad(ll, ul, lr, ur));
            count++;
        } while (more);
        Quad[] qArray = quads.ToArray();
        mesher.BuildLiquidMesh(ref qArray);
    }


    #endregion

    #region Mesh Walls
    private void MeshWalls(ref List<Quad> floorQuads, ref List<Quad> ceilingQuads)
    {
        Mesher mesher = walls.GetComponent<Mesher>();
        List<Quad> quads = new List<Quad>();
        MeshWallW(ref quads);
        MeshWallE(ref quads);
        MeshWallN(ref quads);
        MeshWallS(ref quads);
        MeshDoors(ref quads);
        MeshLowerX(ref quads, ref floorQuads);
        MeshLowerZ(ref quads, ref floorQuads);
        MeshUpperX(ref quads, ref ceilingQuads);
        MeshUpperZ(ref quads, ref ceilingQuads);
        MeshPillars();
        Quad[] qArray = quads.ToArray();
        mesher.BuildWallsMesh(ref qArray);
    }


    #region Mesh Walls Proper
    private void MeshWallE(ref List<Quad> quads)
    {
        ResetUsed();
        Vector3Int ul, ur, ll, lr;
        Coords c1 = new Coords(0, 0);
        int j2, fheight, cheight, door;
        for (int j = 0; j < SIZE; j++)
            for (int i = 0; i < SIZE; i++)
            {
                int index = (j * SIZE) + i;
                if (room[index] == 0) continue;
                if (!used[index] && isWall[index] && (((i + 1) >= SIZE) || !isWall[index + 1]))
                {
                    c1.x = i + 1;
                    c1.z = j;
                    j2 = j;
                    fheight = floorHeight[index];
                    cheight = ceilHeight[index];
                    door = doors[index];
                    do
                    {
                        used[index] = true;
                        j2++;
                        index = (j2 * SIZE) + i;
                    } while ((j2 < SIZE) && isWall[index] && ((c1.x >= SIZE) || !isWall[index + 1])
                            && (floorHeight[index] == fheight) && (ceilHeight[index] == cheight) && (door == doors[index]));
                    if (cheight > (fheight + door))
                    {
                        lr = new Vector3Int(c1.x, fheight + door, j2);
                        ur = new Vector3Int(c1.x, cheight, j2);
                        ul = new Vector3Int(c1.x, cheight, c1.z);
                        ll = new Vector3Int(c1.x, fheight + door, c1.z);
                        quads.Add(new Quad(ll, ul, lr, ur));
                    }
                }
            }
    }


    private void MeshWallW(ref List<Quad> quads)
    {
        ResetUsed();
        Vector3Int ul, ur, ll, lr;
        Coords c1 = new Coords(0, 0);
        int j2, fheight, cheight, door;
        for (int j = 0; j < SIZE; j++)
            for (int i = 0; i < SIZE; i++)
            {
                int index = (j * SIZE) + i;
                if (room[index] == 0) continue;
                if (!used[index] && isWall[index] && (((i - 1) < 0) || !isWall[index - 1]))
                {
                    c1.x = i;
                    c1.z = j;
                    j2 = j;
                    fheight = floorHeight[index];
                    cheight = ceilHeight[index];
                    door = doors[index];
                    do
                    {
                        used[index] = true;
                        j2++;
                        index = (j2 * SIZE) + i;
                    } while ((j2 < SIZE) && !used[index] && isWall[index] && (((i - 1) <= 0) || !isWall[index - 1])
                            && (floorHeight[index] == fheight) && (ceilHeight[index] == cheight) && (door == doors[index]));
                    if (cheight > (fheight + door))
                    {
                        ll = new Vector3Int(c1.x, fheight + door, j2);
                        ul = new Vector3Int(c1.x, cheight, j2);
                        ur = new Vector3Int(c1.x, cheight, c1.z);
                        lr = new Vector3Int(c1.x, fheight + door, c1.z);
                        quads.Add(new Quad(ll, ul, lr, ur));
                    }
                }
            }
    }


    private void MeshWallN(ref List<Quad> quads)
    {
        ResetUsed();
        Vector3Int ul, ur, ll, lr;
        Coords c1 = new Coords(0, 0);
        int i2, fheight, cheight, door;
        for (int i = 0; i < SIZE; i++)
            for (int j = 0; j < SIZE; j++)
            {
                int index = (j * SIZE) + i;
                if (room[index] == 0) continue;
                if (!used[index] && isWall[index] && (((j + 1) >= SIZE) || !isWall[index + SIZE]))
                {
                    c1.x = i;
                    c1.z = j + 1;
                    i2 = i;
                    fheight = floorHeight[index];
                    cheight = ceilHeight[index];
                    door = doors[index];
                    do
                    {
                        used[index] = true;
                        i2++;
                        index = (j * SIZE) + i2;
                    } while ((i2 < SIZE) && isWall[index] && ((c1.z >= SIZE) || !isWall[index + SIZE])
                            && (floorHeight[index] == fheight) && (ceilHeight[index] == cheight) && (door == doors[index]));
                    if (cheight > (fheight + door))
                    {
                        ll = new Vector3Int(i2, fheight, c1.z);
                        ul = new Vector3Int(i2, cheight, c1.z);
                        ur = new Vector3Int(c1.x, cheight, c1.z);
                        lr = new Vector3Int(c1.x, fheight, c1.z);
                        quads.Add(new Quad(ll, ul, lr, ur));
                    }
                }
            }
    }


    private void MeshWallS(ref List<Quad> quads)
    {
        ResetUsed();
        Vector3Int ul, ur, ll, lr;
        Coords c1 = new Coords(0, 0);
        int i2, fheight, cheight, door;
        for (int i = 0; i < SIZE; i++)
            for (int j = 0; j < SIZE; j++)
            {
                int index = (j * SIZE) + i;
                if (room[index] == 0) continue;
                if (!used[index] && isWall[index] && (((j - 1) < 0) || !isWall[index - SIZE]))
                {
                    c1.x = i;
                    c1.z = j;
                    i2 = i;
                    fheight = floorHeight[index];
                    cheight = ceilHeight[index];
                    door = doors[index];
                    do
                    {
                        used[index] = true;
                        i2++;
                        index = (j * SIZE) + i2;
                    } while ((i2 < SIZE) && isWall[index] && ((c1.z <= 0) || !isWall[index - SIZE])
                            && (floorHeight[index] == fheight) && (ceilHeight[index] == cheight) && (door == doors[index]));
                    if (cheight > (fheight + door))
                    {
                        lr = new Vector3Int(i2, fheight, c1.z);
                        ur = new Vector3Int(i2, cheight, c1.z);
                        ul = new Vector3Int(c1.x, cheight, c1.z);
                        ll = new Vector3Int(c1.x, fheight, c1.z);
                        quads.Add(new Quad(ll, ul, lr, ur));
                    }
                }
            }
    }
    #endregion


    private void MeshDoors(ref List<Quad> quads)
    {
        int index1, index2, i1, j1, top;
        Vector3Int ul, ur, ll, lr;
        for (int i = 0; i < SIZE; i++)
            for (int j = 0; j < SIZE; j++)
            {
                index1 = (j * SIZE) + i;
                if (room[index1] == 0) continue;
                if (doors[index1] > 0)
                {
                    i1 = i + 1;
                    j1 = j + 1;
                    top = floorHeight[index1] + doors[index1];
                    index2 = index1 + 1;
                    if (((i1 >= SIZE) || (index2 >= AREA)) || (isWall[index2] && (doors[index2] == 0)))
                    {
                        lr = new Vector3Int(i1, floorHeight[index1], j);
                        ur = new Vector3Int(i1, top, j);
                        ul = new Vector3Int(i1, top, j1);
                        ll = new Vector3Int(i1, floorHeight[index1], j1);
                        quads.Add(new Quad(ll, ul, lr, ur));
                    }
                    index2 = index1 + SIZE;
                    if (((j1 >= SIZE) || (index2 >= AREA)) || (isWall[index2] && (doors[index2] == 0)))
                    {
                        lr = new Vector3Int(i1, floorHeight[index1], j1);
                        ur = new Vector3Int(i1, top, j1);
                        ul = new Vector3Int(i, top, j1);
                        ll = new Vector3Int(i, floorHeight[index1], j1);
                        quads.Add(new Quad(ll, ul, lr, ur));
                    }
                    index2 = index1 - 1;
                    if (((i < 1) || (index2 < 0)) || (isWall[index2] && (doors[index2] == 0)))
                    {
                        ll = new Vector3Int(i, floorHeight[index1], j);
                        ul = new Vector3Int(i, top, j);
                        ur = new Vector3Int(i, top, j1);
                        lr = new Vector3Int(i, floorHeight[index1], j1);
                        quads.Add(new Quad(ll, ul, lr, ur));
                    }
                    index2 = index1 - SIZE;
                    if (((j < 1) || (index2 < 0)) || (isWall[index2] && (doors[index2] == 0)))
                    {
                        ll = new Vector3Int(i1, floorHeight[index1], j);
                        ul = new Vector3Int(i1, top, j);
                        ur = new Vector3Int(i, top, j);
                        lr = new Vector3Int(i, floorHeight[index1], j);
                        quads.Add(new Quad(ll, ul, lr, ur));
                    }
                    if (top < ceilHeight[index1])
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


    private void MeshPillars()
    {
        Mesher mesher = pillars.GetComponent<Mesher>();
        List<Quad> quads = new List<Quad>();
        Vector3Int ul, ur, ll, lr;
        int index, i1, j1;
        for (int i = 0; i < SIZE; i++)
            for (int j = 0; j < SIZE; j++)
            {
                index = (j * SIZE) + i;
                if ((room[index] > 0) && isPillar[index])
                {
                    i1 = i + 1;
                    j1 = j + 1;

                    ll = new Vector3Int(i1, floorHeight[index], j);
                    ul = new Vector3Int(i1, ceilHeight[index], j);
                    ur = new Vector3Int(i1, ceilHeight[index], j1);
                    lr = new Vector3Int(i1, floorHeight[index], j1);
                    quads.Add(new Quad(ll, ul, lr, ur));

                    ll = new Vector3Int(i, floorHeight[index], j);
                    ul = new Vector3Int(i, ceilHeight[index], j);
                    ur = new Vector3Int(i1, ceilHeight[index], j);
                    lr = new Vector3Int(i1, floorHeight[index], j);
                    quads.Add(new Quad(ll, ul, lr, ur));

                    ll = new Vector3Int(i, floorHeight[index], j1);
                    ul = new Vector3Int(i, ceilHeight[index], j1);
                    ur = new Vector3Int(i, ceilHeight[index], j);
                    lr = new Vector3Int(i, floorHeight[index], j);
                    quads.Add(new Quad(ll, ul, lr, ur));

                    ll = new Vector3Int(i1, floorHeight[index], j1);
                    ul = new Vector3Int(i1, ceilHeight[index], j1);
                    ur = new Vector3Int(i, ceilHeight[index], j1);
                    lr = new Vector3Int(i, floorHeight[index], j1);
                    quads.Add(new Quad(ll, ul, lr, ur));
                }
            }
        Quad[] qArray = quads.ToArray();
        mesher.BuildWallsMesh(ref qArray);
    }


    #region Mesh Sides
    private void MeshLowerX(ref List<Quad> quads, ref List<Quad> floorQuads)
    {
        ResetUsed();
        Vector3 ul, ur, ll, lr;
        int[] flats;
        flats = floorHeight;
        Coords c1 = new Coords(0, 0);
        float h2;
        int next, end, h0, h1,index0, index1, width1 = SIZE - 1;
        for (int i = 0; i < width1; i++)
            for (int j = 0; j < width1; j++)
            {
                next = i + 1;
                index0 = (j * SIZE) + i;
                index1 = (j * SIZE) + next;
                if (!used[index0] && (flats[index0] > flats[index1]))
                {
                    c1.z = end = j;
                    h0 = flats[index0];
                    h1 = flats[index1];
                    do
                    {
                        end++;
                        index0 = (end * SIZE) + i;
                        index1 = (end * SIZE) + next;
                    } while ((end < SIZE) && !used[index0] && (flats[index0] == h0) && (flats[index1] == h1));
                    for (int k = j; k < end; k++)
                    {
                        used[(k * SIZE) + i] = true;
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
                else if (!used[index0] && (flats[index0] < flats[index1]))
                {
                    c1.z = end = j;
                    h0 = flats[index0];
                    h1 = flats[index1];
                    do
                    {
                        end++;
                        index0 = (end * SIZE) + i;
                        index1 = (end * SIZE) + next;
                    } while ((end < SIZE) && !used[index0] && (flats[index0] == h0) && (flats[index1] == h1));
                    for (int k = j; k < end; k++)
                    {
                        used[(k * SIZE) + i] = true;
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


    private void MeshLowerZ(ref List<Quad> quads, ref List<Quad> floorQuads)
    {
        ResetUsed();
        Vector3 ul, ur, ll, lr;
        int[] flats;
        flats = floorHeight;
        Coords c1 = new Coords(0, 0);
        float h2;
        int next, end, h0, h1, index0, index1, width1 = SIZE - 1;
        for (int i = 0; i < width1; i++)
            for (int j = 0; j < width1; j++)
            {
                next = j + 1;
                index0 = (j * SIZE) + i;
                index1 = (next * SIZE) + i;
                if (!used[index0] && (flats[index0] > flats[index1]))
                {
                    c1.x = end = i;
                    h0 = flats[index0];
                    h1 = flats[index1];
                    do
                    {
                        end++;
                        index0 = (j * SIZE) + end;
                        index1 = (next * SIZE) + end;
                    } while ((end < SIZE) && !used[index0] && (flats[index0] == h0) && (flats[index1] == h1));
                    for (int k = i; k < end; k++)
                    {
                        used[(j * SIZE) + k] = true;
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
                else if (!used[index0] && (flats[index0] < flats[index1]))
                {
                    c1.x = end = i;
                    h0 = flats[index0];
                    h1 = flats[index1];
                    do
                    {
                        end++;
                        index0 = (j * SIZE) + end;
                        index1 = (next * SIZE) + end;
                    } while ((end < SIZE) && !used[index0] && (flats[index0] == h0) && (flats[index1] == h1));
                    for (int k = i; k < end; k++)
                    {
                        used[(j * SIZE) + k] = true;
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


    private void MeshUpperX(ref List<Quad> quads, ref List<Quad> ceilingQuads)
    {
        ResetUsed();
        Vector3 ul, ur, ll, lr;
        int[] flats;
        flats = ceilHeight;
        Coords c1 = new Coords(0, 0);
        float h2;
        int next, end, h0, h1, index0, index1, width1 = SIZE - 1;
        for (int i = 0; i < width1; i++)
            for (int j = 0; j < width1; j++)
            {
                next = i + 1;
                index0 = (j * SIZE) + i;
                index1 = (j * SIZE) + next;
                if (!used[index0] && (flats[index0] > flats[index1]))
                {
                    c1.z = end = j;
                    h0 = flats[index0];
                    h1 = flats[index1];
                    do
                    {
                        end++;
                        index0 = (end * SIZE) + i;
                        index1 = (end * SIZE) + next;
                    } while ((end < SIZE) && !used[index0] && (flats[index0] == h0) && (flats[index1] == h1));
                    for (int k = j; k < end; k++)
                    {
                        used[(k * SIZE) + i] = true;
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
                        index0 = (end * SIZE) + i;
                        index1 = (end * SIZE) + next;
                    } while ((end < SIZE) && !used[index0] && (flats[index0] == h0) && (flats[index1] == h1));
                    for (int k = j; k < end; k++)
                    {
                        used[(k * SIZE) + i] = true;
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


    private void MeshUpperZ(ref List<Quad> quads, ref List<Quad> ceilingQuads)
    {
        ResetUsed();
        Vector3 ul, ur, ll, lr;
        int[] flats;
        flats = ceilHeight;
        Coords c1 = new Coords(0, 0);
        float h2;
        int next, end, h0, h1, index0, index1, width1 = SIZE - 1;
        for (int i = 0; i < width1; i++)
            for (int j = 0; j < width1; j++)
            {
                next = j + 1;
                index0 = (j * SIZE) + i;
                index1 = (next * SIZE) + i;
                if (!used[index0] && (flats[index0] > flats[index1]))
                {
                    c1.x = end = i;
                    h0 = flats[index0];
                    h1 = flats[index1];
                    do
                    {
                        end++;
                        index0 = (j * SIZE) + end;
                        index1 = (next * SIZE) + end;
                    } while ((end < SIZE) && !used[index0] && (flats[index0] == h0) && (flats[index1] == h1));
                    for (int k = i; k < end; k++)
                    {
                        used[(j * SIZE) + k] = true;
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
                        index0 = (j * SIZE) + end;
                        index1 = (next * SIZE) + end;
                    } while ((end < SIZE) && !used[index0] && (flats[index0] == h0) && (flats[index1] == h1));
                    for (int k = i; k < end; k++)
                    {
                        used[(j * SIZE) + k] = true;
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



}
