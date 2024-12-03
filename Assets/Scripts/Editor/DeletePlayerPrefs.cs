using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class DeletePlayerPrefsScript : EditorWindow
{

    [MenuItem("Tools/Delete PlayerPrefs (All)")]
    static void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
#endif