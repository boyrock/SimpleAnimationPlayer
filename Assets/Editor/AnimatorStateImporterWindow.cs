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
    
    private void OnGUI()
    {
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
                stateImporter.Import();
                stateImporter.Alignment();
            }
        }
    }
}
