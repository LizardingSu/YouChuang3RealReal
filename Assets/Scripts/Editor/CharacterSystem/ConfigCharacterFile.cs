using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class ExcelImportHelper:EditorWindow
{
    private string path;

    [MenuItem("ImportList/charactorList")]
    static void Init()
    {
        var window = (ExcelImportHelper)GetWindow(typeof(ExcelImportHelper));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        {
            GUILayout.BeginHorizontal();
            {
                path = GUILayout.TextField(path);
                if (GUILayout.Button("浏览", GUILayout.Width(50f)))
                {
                    path = EditorUtility.OpenFilePanel("CSV文件", Application.dataPath + "/Resources/Text", "csv");
                }
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("生成ScriptObject"))
            {
                var realPath = path.Substring(path.IndexOf("Assets"));
                var textList = AssetDatabase.LoadAssetAtPath<TextAsset>(realPath).text.Split('\n');
                var config = ScriptableObject.CreateInstance<ConfigCharacterFile>();
                config.characterList = new List<CData>();
                for(int i = 1; i < textList.Length-1; i++)
                {
                    var ta = textList[i].Split(',');
                    config.characterList.Add(new CData(uint.Parse(ta[0]), ta[1]));
                }

                var outname = "CharacterList";
                var dirpath = "Assets/Scripts/Settings/";
                var outpath = dirpath+outname+".asset";

                Debug.Log(config.characterList.Count);

                for (uint i = 1; File.Exists(outpath); i++)
                    outpath = dirpath + outname + "(" + i + ")" + ".asset";

                AssetDatabase.CreateAsset(config, outpath);
            }
        }
        GUILayout.EndVertical();
    }
}