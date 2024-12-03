using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
public class OpenPersistentDataPathWindow : EditorWindow
{
    [MenuItem("Tools/Open Persistent data path")]
    static void OpenPersistentDataPath()
    {
        string path = Application.persistentDataPath;

        if (Directory.Exists(path))
        {
            EditorUtility.RevealInFinder(path);
        }
        else
        {
            Debug.LogError($"Persistent data path does not exist: {path}");
        }
    }
}
#endif
