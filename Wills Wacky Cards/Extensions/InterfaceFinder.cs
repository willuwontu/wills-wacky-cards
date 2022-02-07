using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WWC.Extensions
{
    public static class FindInterfaces
    {
        public static List<T> Find<T>()
        {
            List<T> interfaces = new List<T>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                GameObject[] rootGameObjects = SceneManager.GetSceneAt(i).GetRootGameObjects();
                foreach (var rootGameObject in rootGameObjects)
                {
                    T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>();
                    interfaces.AddRange(childrenInterfaces);
                }
            }
            return interfaces;
        }
    }
}
