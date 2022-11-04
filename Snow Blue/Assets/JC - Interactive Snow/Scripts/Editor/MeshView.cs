using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace JC.Snow
{
    public class MeshView
    {
        public static GameObject CreateView()
        {
            GameObject groundObject = new GameObject("Interactive Snow");
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            groundObject.AddComponent<MeshGenerator>();

            return groundObject;
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
        private static void DrawGizmo(MeshGenerator meshGenerator, GizmoType gizmoType)
        {
            int maxColumns = meshGenerator.Connections + (meshGenerator.Connections - 1);
            Vector3[,] groundPositions = new Vector3[maxColumns, maxColumns];

            Gizmos.color = new Color(meshGenerator.SnowColor.r * 0.3f, meshGenerator.SnowColor.g * 0.3f, meshGenerator.SnowColor.b * 0.3f, 1);

            for (int i = 0; i < maxColumns; i++)
            {
                for (int j = 0; j < maxColumns; j++)
                {
                    Vector3 groundPosition = new Vector3((i - maxColumns / 2) * meshGenerator.Scale * 0.95f, 0, (j - maxColumns / 2) * meshGenerator.Scale * 0.95f);
                    Gizmos.DrawWireCube((meshGenerator.transform.position - groundPosition), new Vector3(meshGenerator.Scale, 0, meshGenerator.Scale));
                    groundPositions[i, j] = groundPosition;
                }
            }

            meshGenerator.GroundPositions = groundPositions;
            meshGenerator.MaxColumns = maxColumns;
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
        private static void DrawDistanceGizmo(MeshGenerator meshGenerator, GizmoType gizmoType)
        {
            if(meshGenerator.transform.childCount < 1) return;

            ScriptableConnection scriptableConnection = meshGenerator.transform.GetChild(0).GetComponent<UpdateConnection>().ScriptableConnection;

            Gizmos.color = new Color(meshGenerator.SnowColor.r * 0.3f, meshGenerator.SnowColor.g * 0.3f, meshGenerator.SnowColor.b * 0.3f, 1);

            for (int i = 0; i < meshGenerator.transform.childCount; i++)
            {
                Vector2 xzPosition = new Vector2(meshGenerator.transform.GetChild(i).GetChild(0).GetChild(0).position.x, meshGenerator.transform.GetChild(i).GetChild(0).GetChild(0).position.z);
                Vector2 xzMainCamera = new Vector2(scriptableConnection.mainCamera.position.x, scriptableConnection.mainCamera.position.z);

                if(Vector2.Distance(xzPosition, xzMainCamera) < scriptableConnection.renderDistance)
                {
                    Gizmos.DrawLine(meshGenerator.transform.GetChild(i).GetChild(0).GetChild(0).position, scriptableConnection.mainCamera.position);
                }
            }
        }
    }
}
