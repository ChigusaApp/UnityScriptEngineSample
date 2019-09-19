//  (C)2019 Chigusa
using ScriptEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using VirtualMachine;

/// <summary>
/// コンパイル用のEditor拡張
/// </summary>
public class CreateCks : EditorWindow
{
    string TargetFolder { get; set; } = "Games/CK";


    [MenuItem("Chigusa/for CK/Create Cks", false, 205)]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CreateCks), false, "Create Cks");
    }

    private void OnEnable()
    {
        TargetFolder = EditorPrefs.GetString(this.GetType().FullName + ".TargetFolder", "Games/CK");
    }

    void OnGUI()
    {
        //  タイトル
        //ToolsForEditorWindow.TitleLabel("Create Cks", null);


        using (new EditorGUILayout.VerticalScope())
        {
            TargetFolder = EditorGUILayout.TextField("Target Folder", TargetFolder);

            //  コンパイルからリンク出力まで一括処理
            if (GUILayout.Button("Create File"))
            {
                Loader.AutoBaseHeaderCreate(TargetFolder, false);
                Loader.AutoBaseCreate<CustomCompiler, CustomCpu>(TargetFolder, false);
                Loader.AutoCreate<CustomCompiler, CustomCpu>(TargetFolder, false);
                AssetDatabase.Refresh();
                EditorPrefs.SetString(this.GetType().FullName + ".TargetFolder", TargetFolder);
            }

            //  コンパイルからリンク出力まで一括処理
            if (GUILayout.Button("Force Create File"))
            {
                Loader.AutoBaseHeaderCreate(TargetFolder, true);
                Loader.AutoBaseCreate<CustomCompiler, CustomCpu>(TargetFolder, true);
                Loader.AutoCreate<CustomCompiler, CustomCpu>(TargetFolder, true);
                AssetDatabase.Refresh();
                EditorPrefs.SetString(this.GetType().FullName + ".TargetFolder", TargetFolder);
            }

            //  指定のファイルをコンパイルして結果を出力するのみ
            if (GUILayout.Button("Compile Check File"))
            {
                var targetFolder = Path.Combine(Application.dataPath, TargetFolder, "CkScripts");
                var openFile = EditorUtility.OpenFilePanelWithFilters("Compile Check File", targetFolder, new string[] { "ck files", "ck", "All files", "*" });
                if (!string.IsNullOrWhiteSpace(openFile))
                {
                    Data vmData = new Data();
                    var compiler = new CustomCompiler();
                    bool compileResut = compiler.Compile(openFile, vmData);
                    if (compileResut)
                    {
                        Debug.Log("コンパイルに成功\n" + openFile);
                        Debug.Log(compiler.GetDebugDump());
                    }
                    else
                    {
                        Debug.LogWarning("コンパイルに失敗\n" + openFile);
                        foreach (var errorMessage in compiler.ErrorMessageList)
                        {
                            Debug.LogWarning("コンパイル結果：" + errorMessage);
                        }
                    }
                }
            }

        }

    }




}
