using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace JC.Snow
{
    public static class DestroyInBuild {
        private static readonly Type[] typesToDeleteOnBuild = {
            typeof(MeshGenerator)
        };
        
        [PostProcessScene]
        public static void DeleteObjects() {
            if (BuildPipeline.isBuildingPlayer) {
                foreach (var type in typesToDeleteOnBuild) {
                    Debug.Log($"Destroying all instances of {type.Name} on build!");

                    foreach (var obj in FindObjectsOfTypeAll(type, true)) {
                        Object.DestroyImmediate(obj);
                    }
                }
            }
        }

        public static List<Component> FindObjectsOfTypeAll(Type type, bool findInactive = false)
        {
            var results = new List<Component>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var s = SceneManager.GetSceneAt(i);
                if (s.isLoaded)
                {
                    var allGameObjects = s.GetRootGameObjects();
                    for (int j = 0; j < allGameObjects.Length; j++)
                    {
                        var go = allGameObjects[j];
                        results.AddRange(go.GetComponentsInChildren(type, findInactive));
                    }
                }
            }
            return results;
        }
    }
}
