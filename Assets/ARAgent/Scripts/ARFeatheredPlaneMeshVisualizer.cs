using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace MillinAR
{
    [RequireComponent(typeof(ARPlaneMeshVisualizer), typeof(MeshRenderer), typeof(ARPlane))]
    public class ARFeatheredPlaneMeshVisualizer : MonoBehaviour
    {
        [Tooltip("The width of the texture feathering (in world units).")]
        [SerializeField]
        float m_FeatheringWidth = 0.2f;


        public float featheringWidth
        {
            get { return m_FeatheringWidth; }
            set { m_FeatheringWidth = value; }
        }

        void Awake()
        {
            m_PlaneMeshVisualizer = GetComponent<ARPlaneMeshVisualizer>();
            m_FeatheredPlaneMaterial = GetComponent<MeshRenderer>().material;
            m_Plane = GetComponent<ARPlane>();
        }

        void OnEnable()
        {
            m_Plane.boundaryChanged += ARPlane_boundaryUpdated;
        }

        void OnDisable()
        {
            m_Plane.boundaryChanged -= ARPlane_boundaryUpdated;
        }

        void ARPlane_boundaryUpdated(ARPlaneBoundaryChangedEventArgs eventArgs)
        {
            GenerateBoundaryUVs(m_PlaneMeshVisualizer.mesh);
        }
        void GenerateBoundaryUVs(Mesh mesh)
        {
            int vertexCount = mesh.vertexCount;

            // Reuse the list of UVs
            s_FeatheringUVs.Clear();
            if (s_FeatheringUVs.Capacity < vertexCount) { s_FeatheringUVs.Capacity = vertexCount; }

            mesh.GetVertices(s_Vertices);

            Vector3 centerInPlaneSpace = s_Vertices[s_Vertices.Count - 1];
            Vector3 uv = new Vector3(0, 0, 0);
            float shortestUVMapping = float.MaxValue;

            // Assume the last vertex is the center vertex.
            for (int i = 0; i < vertexCount - 1; i++)
            {
                float vertexDist = Vector3.Distance(s_Vertices[i], centerInPlaneSpace);

                // Remap the UV so that a UV of "1" marks the feathering boudary.
                // The ratio of featherBoundaryDistance/edgeDistance is the same as featherUV/edgeUV.
                // Rearrange to get the edge UV.
                float uvMapping = vertexDist / Mathf.Max(vertexDist - featheringWidth, 0.001f);
                uv.x = uvMapping;

                // All the UV mappings will be different. In the shader we need to know the UV value we need to fade out by.
                // Choose the shortest UV to guarentee we fade out before the border.
                // This means the feathering widths will be slightly different, we again rely on a fairly uniform plane.
                if (shortestUVMapping > uvMapping) { shortestUVMapping = uvMapping; }

                s_FeatheringUVs.Add(uv);
            }

            m_FeatheredPlaneMaterial.SetFloat("_ShortestUVMapping", shortestUVMapping);

            // Add the center vertex UV
            uv.Set(0, 0, 0);
            s_FeatheringUVs.Add(uv);

            mesh.SetUVs(1, s_FeatheringUVs);
            mesh.UploadMeshData(false);
        }

        static List<Vector3> s_FeatheringUVs = new List<Vector3>();

        static List<Vector3> s_Vertices = new List<Vector3>();

        ARPlaneMeshVisualizer m_PlaneMeshVisualizer;

        ARPlane m_Plane;

        Material m_FeatheredPlaneMaterial;
    }
}