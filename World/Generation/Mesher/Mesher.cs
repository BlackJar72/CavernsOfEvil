using UnityEngine;


namespace DLD {

    public class Mesher : MonoBehaviour
    {
        public Substance substance;


        public Substance Substance
        {
            get => substance;
            set
            {
                substance = value;
                gameObject.GetComponent<MeshRenderer>().sharedMaterial = substance.Material;
            }
        }


        #region meshing
        public void BuildFlatsMesh(ref Quad[] quads, Vector3 normal)
        {
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            Mesh mesh = new Mesh();

            Vector3[] verts = new Vector3[quads.Length * 4];
            Vector3[] norms = new Vector3[quads.Length * 4];
            Vector2[] uvs = new Vector2[quads.Length * 4];
            int[] corners = new int[quads.Length * 6];

            int q, t;
            float u, v;
            for (int i = 0; i < quads.Length; i++)
            {
                q = i * 4;
                u = (float)(quads[i].ll.x % 4) / 4f;
                v = (float)(quads[i].ll.x % 4) / 4f;
                verts[q] = quads[i].ll;
                uvs[q] = new Vector2(u, v);
                q++;
                verts[q] = quads[i].ul;
                uvs[q] = new Vector2(u, v + 1f);
                q++;
                verts[q] = quads[i].lr;
                uvs[q] = new Vector2(u + 1f, v);
                q++;
                verts[q] = quads[i].ur;
                uvs[q] = new Vector2(u + 1f, v + 1f);
            }
            if (normal.y > 0)
            {
                for (int i = 0; i < quads.Length; i++)
                {
                    q = i * 6;
                    t = i * 4;
                    corners[q] = t;
                    corners[q + 1] = t + 1;
                    corners[q + 2] = t + 2;
                    corners[q + 3] = t + 1;
                    corners[q + 4] = t + 3;
                    corners[q + 5] = t + 2;
                }
            }
            else
            {
                for (int i = 0; i < quads.Length; i++)
                {
                    q = i * 6;
                    t = i * 4;
                    corners[q] = t + 2;
                    corners[q + 1] = t + 1;
                    corners[q + 2] = t;
                    corners[q + 3] = t + 2;
                    corners[q + 4] = t + 3;
                    corners[q + 5] = t + 1;
                }
            }
            mesh.vertices = verts;
            mesh.normals = norms;
            mesh.uv = uvs;
            mesh.triangles = corners;
            meshFilter.mesh = mesh;
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            MeshCollider collider = GetComponent<MeshCollider>();
            collider.sharedMesh = mesh;
        }


        public void BuildCaveFloorMesh(Cave room)
        {
            MeshFilter meshFilter = room.floor.GetComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            room.BuildCaveFloor(mesh);
            meshFilter.mesh = mesh;
            MeshCollider collider = GetComponent<MeshCollider>();
            collider.sharedMesh = mesh;
        }


        public void BuildCaveRoofMesh(Cave room)
        {
            MeshFilter meshFilter = room.ceiling.GetComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            room.BuildCaveRoof(mesh);
            meshFilter.mesh = mesh;
            MeshCollider collider = GetComponent<MeshCollider>();
            collider.sharedMesh = mesh;
        }


        public void BuildWallsMesh(ref Quad[] quads)
        {
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            Mesh mesh = new Mesh();

            Vector3[] verts = new Vector3[quads.Length * 4];
            Vector3[] norms = new Vector3[quads.Length * 4];
            Vector2[] uvs = new Vector2[quads.Length * 4];
            int[] corners = new int[quads.Length * 6];

            int q, t;
            float u, v;
            for (int i = 0; i < quads.Length; i++)
            {
                q = i * 4;
                u = (float)(quads[i].ll.x % 4) / 4f;
                v = (float)(quads[i].ll.x % 4) / 4f;
                verts[q] = quads[i].ll;
                uvs[q] = new Vector2(u, v);
                q++;
                verts[q] = quads[i].ul;
                uvs[q] = new Vector2(u, v + 1f);
                q++;
                verts[q] = quads[i].lr;
                uvs[q] = new Vector2(u + 1f, v);
                q++;
                verts[q] = quads[i].ur;
                uvs[q] = new Vector2(u + 1f, v + 1f);
            }
            for (int i = 0; i < quads.Length; i++)
            {
                q = i * 6;
                t = i * 4;
                corners[q] = t;
                corners[q + 1] = t + 1;
                corners[q + 2] = t + 2;
                corners[q + 3] = t + 1;
                corners[q + 4] = t + 3;
                corners[q + 5] = t + 2;
            }
            mesh.vertices = verts;
            mesh.normals = norms;
            mesh.uv = uvs;
            mesh.triangles = corners;
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            meshFilter.mesh = mesh;
            MeshCollider collider = GetComponent<MeshCollider>();
            collider.sharedMesh = mesh;
        }


        internal void BuildLiquidMesh(ref Quad[] quads)
        {
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            Mesh mesh = new Mesh();

            Vector3[] verts = new Vector3[quads.Length * 4];
            Vector3[] norms = new Vector3[quads.Length * 4];
            Vector2[] uvs = new Vector2[quads.Length * 4];
            int[] corners = new int[quads.Length * 12];

            int q, t;
            float u, v;
            for (int i = 0; i < quads.Length; i++)
            {
                q = i * 4;
                u = (float)(quads[i].ll.x % 4) / 4f;
                v = (float)(quads[i].ll.x % 4) / 4f;
                verts[q] = quads[i].ll;
                norms[q] = Vector3.up;
                uvs[q] = new Vector2(u, v);
                q++;
                verts[q] = quads[i].ul;
                norms[q] = Vector3.up;
                uvs[q] = new Vector2(u, v + 1f);
                q++;
                verts[q] = quads[i].lr;
                norms[q] = Vector3.up;
                uvs[q] = new Vector2(u + 1f, v);
                q++;
                verts[q] = quads[i].ur;
                norms[q] = Vector3.up;
                uvs[q] = new Vector2(u + 1f, v + 1f);
            }
            for (int i = 0; i < quads.Length; i++)
            {
                q = i * 12;
                t = i * 4;
                corners[q] = t;
                corners[q + 1] = t + 1;
                corners[q + 2] = t + 2;
                corners[q + 3] = t + 1;
                corners[q + 4] = t + 3;
                corners[q + 5] = t + 2;
                corners[q + 6] = t + 2;
                corners[q + 7] = t + 1;
                corners[q + 8] = t;
                corners[q + 9] = t + 2;
                corners[q + 10] = t + 3;
                corners[q + 11] = t + 1;
            }
            for (int i = 0; i < quads.Length; i++)
            {
            }
            mesh.vertices = verts;
            mesh.normals = norms;
            mesh.uv = uvs;
            mesh.triangles = corners;
            meshFilter.mesh = mesh;
            MeshCollider collider = GetComponent<MeshCollider>();
            collider.sharedMesh = mesh;
        }
        #endregion



    }
}
