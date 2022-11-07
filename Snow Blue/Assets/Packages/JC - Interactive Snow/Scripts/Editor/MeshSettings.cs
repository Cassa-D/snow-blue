using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace JC.Snow
{
   [CustomEditor(typeof(MeshGenerator))]
    public class MeshSettings : Editor
    {
        public override void OnInspectorGUI()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode) return;

            MeshGenerator meshGenerator = (MeshGenerator)target;

            Transform groundTransform = meshGenerator.transform;

            GUILayout.Label("Mesh");
            using (new EditorGUILayout.VerticalScope("HelpBox"))
            {
                meshGenerator.Detail = (int)EditorGUILayout.Slider("Detail", meshGenerator.Detail, 32, 255);
                meshGenerator.Scale = (int)EditorGUILayout.Slider("Scale", meshGenerator.Scale, 1, 32);

                if(meshGenerator.Scale < 32)
                {
                    meshGenerator.Connections = 1;
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.Slider("Connections", meshGenerator.Connections, 1, 8);
                    EditorGUI.EndDisabledGroup();
                }

                else
                {
                    meshGenerator.Connections = (int)EditorGUILayout.Slider("Connections", meshGenerator.Connections, 1, 8);
                }

                meshGenerator.Lod0 = EditorGUILayout.Slider("Lod 0", meshGenerator.Lod0, 0, 0.9f);
                meshGenerator.Lod1 = EditorGUILayout.Slider("Lod 1", meshGenerator.Lod1, 0, meshGenerator.Lod0 - 0.1f);
                meshGenerator.Lod2 = EditorGUILayout.Slider("Lod 2", meshGenerator.Lod2, 0, meshGenerator.Lod1 - 0.1f);

                if(GUILayout.Button("Generate Ground"))
                {
                    while(groundTransform.childCount > 0) DestroyImmediate(groundTransform.GetChild(groundTransform.childCount - 1).gameObject);

                    ScriptableConnection scriptableConnection = ScriptableObject.CreateInstance<ScriptableConnection>();
                    scriptableConnection.mainCamera = Camera.main.transform;
                    scriptableConnection.renderDistance = (Mathf.Sqrt(meshGenerator.Scale * meshGenerator.Scale + meshGenerator.Scale * meshGenerator.Scale) - (meshGenerator.Scale * 0.05f * 2)) * 1.25f;

                    meshGenerator.CreateGlobalMeshes();
                    meshGenerator.GenerateCollider();

                    for (int i = 0; i < meshGenerator.MaxColumns; i++)
                    {
                        for (int j = 0; j < meshGenerator.MaxColumns; j++)
                        {
                            Transform groundConnection = new GameObject("Snow Connection").transform;
                            groundConnection.SetParent(groundTransform);
                            
                            UpdateConnection updateConnection = groundConnection.gameObject.AddComponent<UpdateConnection>();
                            updateConnection.ScriptableConnection = scriptableConnection;

                            Transform meshTransform = new GameObject("Ground Mesh").transform;
                            meshTransform.SetParent(groundConnection);

                            Transform renderTransform = new GameObject("Ground Render").transform;
                            renderTransform.SetParent(groundConnection);

                            meshGenerator.GenerateMesh(meshTransform, groundTransform.position - meshGenerator.GroundPositions[i, j], renderTransform);
                        }
                    }

                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
            }
            GUILayout.Space(15);

            GUILayout.Label("Renderer");
            using (new EditorGUILayout.VerticalScope("HelpBox"))
            {
                EditorGUI.BeginChangeCheck();
                meshGenerator.LayerName = EditorGUILayout.TextField("Layer Name", meshGenerator.LayerName);
                if(EditorGUI.EndChangeCheck())
                {
                    for (int i = 0; i < groundTransform.childCount; i++)
                    {
                        meshGenerator.UpgradeRenderer(groundTransform.GetChild(i).GetChild(1).gameObject);
                    }

                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
            }
            GUILayout.Space(15);

            GUILayout.Label("Shader");
            using (new EditorGUILayout.VerticalScope("HelpBox"))
            {
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Noise Texture"); 
                meshGenerator.NoiseTexture = (Texture2D)EditorGUILayout.ObjectField(meshGenerator.NoiseTexture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
                EditorGUILayout.EndHorizontal();

                meshGenerator.GroundScale = EditorGUILayout.Slider("Ground Scale", meshGenerator.GroundScale, 0, 1);
                meshGenerator.GroundHeight = EditorGUILayout.Slider("Ground Height", meshGenerator.GroundHeight, 0, 10);
                meshGenerator.SnowDetail = EditorGUILayout.Slider("Snow Detail", meshGenerator.SnowDetail, 0, 1);
                meshGenerator.TrailIntensity = EditorGUILayout.Slider("Trail Intensity", meshGenerator.TrailIntensity, 0, 1);
                meshGenerator.SnowColor = EditorGUILayout.ColorField(new GUIContent("Snow Color"), meshGenerator.SnowColor, false, true, true);                
                if(EditorGUI.EndChangeCheck())
                {
                    for (int i = 0; i < groundTransform.childCount; i++)
                    {
                        meshGenerator.UpgradeShader(groundTransform.GetChild(i).GetChild(0).GetChild(0).gameObject);
                    }

                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
            }
        }
    } 
}
