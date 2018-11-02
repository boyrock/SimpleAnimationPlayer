using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class AnimatorStateImporterWindow : EditorWindow
{
    static string capacityStr = "";
    static int capacity
    {
        get
        {
            int i = 0;
            int.TryParse(capacityStr, out i);
            return i;
        }
    }

    AnimatorStateImporter stateImporter;

    [MenuItem("Window/Animator State Importer")]
    static void CreateController()
    {
        var window = GetWindow<AnimatorStateImporterWindow>("Animator State Importer");
    }

    string successTime;

    bool _importResult;
    bool importResult
    {
        get
        {
            return _importResult;
        }
        set
        {
            if(value == true)
            {
                successTime = string.Format("({0})", System.DateTime.Now.ToString());
            }
            else
            {
                successTime = "";
            }

            _importResult = value;
        }
    }

    GUIStyle style;

    private void OnGUI()
    {
        if(style == null)
            style = new GUIStyle();

        if(stateImporter == null)
            stateImporter = new AnimatorStateImporter();

        using (new GUILayout.VerticalScope())
        {
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Animator", GUILayout.Width(180));
                stateImporter.controller = (AnimatorController)EditorGUILayout.ObjectField(stateImporter.controller, typeof(AnimatorController), false, GUILayout.Width(200));
                GUILayout.ExpandWidth(true);
            }

            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Animation FBX Files Directory", GUILayout.Width(180));
                stateImporter.directory = (DefaultAsset)EditorGUILayout.ObjectField(stateImporter.directory, typeof(DefaultAsset), false, GUILayout.Width(200));
                GUILayout.ExpandWidth(true);
            }

            using(new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Animation FBX File Count", GUILayout.Width(180));
                capacityStr = EditorGUILayout.TextField(capacityStr, GUILayout.Width(180));
                GUILayout.ExpandWidth(true);
            }

            if (stateImporter.fbxFiles == null || (stateImporter.fbxFiles != null && stateImporter.fbxFiles.Length != capacity))
                stateImporter.fbxFiles = new GameObject[capacity];

            for (int i = 0; i < stateImporter.fbxFiles.Length; i++)
            {
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(string.Format("FBX File ({0})", i + 1), GUILayout.Width(150));
                    stateImporter.fbxFiles[i] = (GameObject)EditorGUILayout.ObjectField(stateImporter.fbxFiles[i], typeof(GameObject), false, GUILayout.Width(200));
                    GUILayout.ExpandWidth(true);
                }
            }

            stateImporter.isClearOnStartUp = EditorGUILayout.ToggleLeft("ClearOnStartup", stateImporter.isClearOnStartUp, GUILayout.Width(120));
            stateImporter.isOverwrite = EditorGUILayout.ToggleLeft("Overwrite", stateImporter.isOverwrite, GUILayout.Width(120));

            if (GUILayout.Button("Import", GUILayout.Width(100)))
            {
                importResult = stateImporter.Import();
                stateImporter.Alignment();
            }

            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;
            style.fontSize = 30;
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.Label(importResult == true ? "SUCCESS..!!" : "", style);
            GUILayout.Label(successTime);
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }
    }
}
