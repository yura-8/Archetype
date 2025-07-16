using UnityEditor;
using UnityEngine;

public class FindMissingScripts : EditorWindow
{
    [MenuItem("Tools/Find Missing Scripts")]
    public static void ShowWindow()
    {
        GetWindow(typeof(FindMissingScripts));
    }

    public void OnGUI()
    {
        if (GUILayout.Button("シーン内のMissing Scriptを探す"))
        {
            FindInScene();
        }
    }

    private static void FindInScene()
    {
        int count = 0;
        // ▼▼▼ この行を修正しました ▼▼▼
        GameObject[] goArray = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

        foreach (GameObject go in goArray)
        {
            Component[] components = go.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    count++;
                    Debug.LogError($"Missing Script がアタッチされています: {go.name}", go);
                }
            }
        }
        Debug.Log($"シーンをスキャンしました。{count}個のMissing Scriptが見つかりました。");
    }
}