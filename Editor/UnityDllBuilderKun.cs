using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UTJ.UnityCommandLineTools;

// <summary>
// ScriptからDLLを作成します
// Programed by Katsumasa.Kimura
// </summary>
public class UnityDllBuilderKun : EditorWindow
{
    static class Styles
    {        
        public static readonly GUIContent OpenFolderContents = new GUIContent((Texture2D)EditorGUIUtility.Load("d_OpenedFolder Icon"));
        public static readonly GUIContent UnityEngineDll = new GUIContent("UnityEngine.dll", "UnityEngine.dllを使用するか否か");
        public static readonly GUIContent UnityEditorDll = new GUIContent("UnityEditor.dll", "UnityEditor.dllを使用するか否か");
        public static readonly GUIContent SrcFileContent = new GUIContent("Add Script File");
        public static readonly GUIContent DllFileContent = new GUIContent("As DLL File");
        public static readonly GUIContent AddContents = new GUIContent((Texture2D)EditorGUIUtility.Load("d_CreateAddNew@2x"));
        public static readonly GUIContent MinusContents = new GUIContent((Texture2D)EditorGUIUtility.Load("d_ol_minus_act"));
        public static readonly GUIContent DebugContent = new GUIContent("Debug Settion", "DLLを使用するコードをデバックする為にmdb ファイルを生成します。");
    }

    bool mIsUnityEngineDll = true;
    bool mIsUnityEditorDll = false;
    bool mIsDebug = false;
    string mDllFilePath;
    List<string> mSrcFilePaths;

    [MenuItem("Window/UnityDllBuilderKun")]
    static void Int()
    {
        var window = (UnityDllBuilderKun)EditorWindow.GetWindow(typeof(UnityDllBuilderKun));
        window.Show();
    }

    private void OnEnable()
    {
        if(mSrcFilePaths == null)
        {
            mSrcFilePaths = new List<string>();
        }
    }


    private void OnGUI()
    {

        EditorGUILayout.LabelField("Build Options");
        using (new EditorGUI.IndentLevelScope())
        {
            EditorGUILayout.LabelField("Check Use DLL", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                mIsUnityEngineDll = EditorGUILayout.Toggle(Styles.UnityEngineDll, mIsUnityEngineDll);
                mIsUnityEditorDll = EditorGUILayout.Toggle(Styles.UnityEditorDll, mIsUnityEditorDll);
            }
            EditorGUILayout.Separator();
            mIsDebug = EditorGUILayout.Toggle(Styles.DebugContent, mIsDebug);
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            {
                Vector2 contentSize;
                contentSize = EditorStyles.label.CalcSize(Styles.OpenFolderContents);
                if (GUILayout.Button(Styles.OpenFolderContents, GUILayout.MaxWidth(contentSize.x + 10)))
                {
                    mDllFilePath = EditorUtility.SaveFilePanel("Save DLL File", "", "", "dll");
                }
                contentSize = EditorStyles.label.CalcSize(Styles.DllFileContent);
                EditorGUILayout.LabelField(Styles.DllFileContent, GUILayout.MaxWidth(contentSize.x + 10));

                EditorGUILayout.LabelField(new GUIContent(mDllFilePath), EditorStyles.textArea);
                //GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.Separator();

        var removeList = new List<string>();

        foreach (var fname in mSrcFilePaths)
        {
            EditorGUILayout.BeginHorizontal();
            {
                Vector2 contentSize;
                contentSize = EditorStyles.label.CalcSize(Styles.MinusContents);
                if (GUILayout.Button(Styles.MinusContents, GUILayout.MaxWidth(contentSize.x + 10)))
                {
                    removeList.Add(fname);
                }
                EditorGUILayout.LabelField(fname, EditorStyles.textField);
            }
            EditorGUILayout.EndHorizontal();
        }

        // 削除依頼のあったファイルを取り除く
        foreach(var fname in removeList)
        {
            mSrcFilePaths.Remove(fname);
        }
        

        EditorGUILayout.BeginHorizontal();
        {
            Vector2 contentSize;
            contentSize = EditorStyles.label.CalcSize(Styles.AddContents);
            if (GUILayout.Button(Styles.AddContents, GUILayout.MaxWidth(contentSize.x + 10)))
            {
                var fname = EditorUtility.OpenFilePanel("Open Script File", "", "cs");
                if (!string.IsNullOrEmpty(fname))
                {
                    if (!mSrcFilePaths.Contains(fname))
                    {
                        mSrcFilePaths.Add(fname);
                    }
                }
            }
            contentSize = EditorStyles.label.CalcSize(Styles.SrcFileContent);
            EditorGUILayout.LabelField(Styles.SrcFileContent, GUILayout.MaxWidth(contentSize.x + 10));
        }
        EditorGUILayout.EndHorizontal();


        // Do Build !!
        EditorGUILayout.BeginHorizontal();        
        GUILayout.FlexibleSpace();                
        EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(mDllFilePath) || mSrcFilePaths.Count == 0);
        if(GUILayout.Button("Build", GUILayout.ExpandWidth(false)))
        {
            var mcs = new McsExec();
            var result = mcs.Exec(mIsUnityEngineDll, mIsUnityEditorDll, mIsDebug, mDllFilePath, mSrcFilePaths.ToArray());
            EditorUtility.DisplayDialog(result == 0 ? "Success" : "Fail", mcs.output, "OK");            
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();


    }
}
