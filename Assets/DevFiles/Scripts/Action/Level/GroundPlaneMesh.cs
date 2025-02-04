using clrev01.Bases;
using System.Collections.Generic;
using UnityEngine;

namespace clrev01.ClAction.Level
{
    public class GroundPlaneMesh : BaseOfCL
    {
        [Range(0.1f, 128)]
        public float vertexPerMeter = 8;
        public Material material;
        public PhysicsMaterial physicMaterial;
        public List<BlendPerlinNoize> perlinNoizes = new List<BlendPerlinNoize>();


        public void GenerateMesh(float meshSize)
        {
            foreach (var pn in perlinNoizes)
            {
                pn.Initialize();
            }
            int vertexNumOfOneSide = (int)(meshSize / vertexPerMeter);
            Vector3[] vertices = new Vector3[vertexNumOfOneSide * vertexNumOfOneSide];
            Vector2[] uvs = new Vector2[vertexNumOfOneSide * vertexNumOfOneSide];
            float vertexDistance = meshSize / vertexNumOfOneSide;
            for (int z = 0; z < vertexNumOfOneSide; z++)
            {
                for (int x = 0; x < vertexNumOfOneSide; x++)
                {
                    float yf = 0;
                    foreach (var pn in perlinNoizes)
                    {
                        yf += pn.Value((float)x / (vertexNumOfOneSide - 1) * vertexDistance, (float)z / (vertexNumOfOneSide - 1) * vertexDistance);
                    }
                    float xf = x * vertexDistance - meshSize / 2;
                    float zf = -z * vertexDistance + meshSize / 2;
                    vertices[z * vertexNumOfOneSide + x] = new Vector3(xf, yf, zf);
                    uvs[z * vertexNumOfOneSide + x] = new Vector2(xf, zf);
                }
            }

            int triangleIndex = 0;
            int[] triangles = new int[(vertexNumOfOneSide - 1) * (vertexNumOfOneSide - 1) * 6];
            for (int z = 0; z < vertexNumOfOneSide - 1; z++)
            {
                for (int x = 0; x < vertexNumOfOneSide - 1; x++)
                {
                    int a = z * vertexNumOfOneSide + x;
                    int b = a + 1;
                    int c = a + vertexNumOfOneSide;
                    int d = c + 1;

                    triangles[triangleIndex] = a;
                    triangles[triangleIndex + 1] = b;
                    triangles[triangleIndex + 2] = c;

                    triangles[triangleIndex + 3] = c;
                    triangles[triangleIndex + 4] = b;
                    triangles[triangleIndex + 5] = d;

                    triangleIndex += 6;
                }
            }

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            if (!meshFilter) meshFilter = gameObject.AddComponent<MeshFilter>();

            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (!meshRenderer) meshRenderer = gameObject.AddComponent<MeshRenderer>();

            MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
            if (!meshCollider) meshCollider = gameObject.AddComponent<MeshCollider>();

            meshFilter.mesh = mesh;
            meshRenderer.sharedMaterial = material;
            meshCollider.sharedMesh = mesh;
            meshCollider.sharedMaterial = physicMaterial;
        }
    }
}