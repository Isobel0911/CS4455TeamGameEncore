using UnityEngine;
using UnityEditor;

public class MeshSaverEditor : MonoBehaviour
{
    // 在Inspector中设置这个数组
    public Mesh[] meshesToSave;

    // 调用这个方法来保存所有Meshes
    public void SaveMeshes()
    {
        // 确定保存路径，需要在Assets文件夹内
        string savePath = "Assets/Meshes/";

        foreach (Mesh mesh in meshesToSave)
        {
            if (mesh != null)
            {
                // 为每个mesh创建一个唯一的文件名
                string fileName = mesh.name + ".asset";
                string fullPath = savePath + fileName;

                // 检查同名文件是否已存在
                if (AssetDatabase.LoadAssetAtPath<Mesh>(fullPath) == null)
                {
                    // 如果不存在同名文件，则创建新的asset
                    Debug.Log("Saving Mesh to " + fullPath);
                    AssetDatabase.CreateAsset(mesh, fullPath);
                }
                else
                {
                    // 如果存在同名文件，则输出提示
                    Debug.LogWarning("File '" + fullPath + "' already exists. Skipping save.");
                }
            }
        }

        // 保存更改到磁盘
        AssetDatabase.SaveAssets();
    }

    // 在Unity编辑器中为该脚本添加一个按钮
    [CustomEditor(typeof(MeshSaverEditor))]
    public class MeshSaverEditorInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MeshSaverEditor script = (MeshSaverEditor)target;
            if (GUILayout.Button("Save Meshes"))
            {
                script.SaveMeshes();
            }
        }
    }
}
