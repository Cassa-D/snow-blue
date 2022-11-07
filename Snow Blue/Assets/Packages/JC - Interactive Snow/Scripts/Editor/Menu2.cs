using UnityEngine;
using UnityEditor;

namespace JC.Snow
{
    public static class Menu2
    {
        [MenuItem("JC - Interactive Ground/Snow")]
        private static void CreateSnow()
        {
            Selection.activeGameObject = MeshView.CreateView();
            SceneView.lastActiveSceneView.FrameSelected();
        }
    }
}
