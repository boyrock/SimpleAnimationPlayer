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

        var directory = stateImporter.directory;
        var fbxFiles = stateImporter.fbxFiles;
        var isClearOnStartUp = stateImporter.isClearOnStartUp;
        var isOverwrite = stateImporter.isOverwrite;


        using (new GUILayout.VerticalScope())
        {

            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Animator", GUILayout.Width(150));
                stateImporter.controller = (AnimatorController)EditorGUILayout.ObjectField(stateImporter.controller, typeof(AnimatorController), false, GUILayout.Width(200));
                GUILayout.ExpandWidth(true);
            }

            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("FBX Files Directory", GUILayout.Width(150));
                directory = (DefaultAsset)EditorGUILayout.ObjectField(directory, typeof(DefaultAsset), false, GUILayout.Width(200));
                GUILayout.ExpandWidth(true);
            }

            using(new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("FBX File Count", GUILayout.Width(150));
                capacityStr = EditorGUILayout.TextField(capacityStr, GUILayout.Width(150));
                GUILayout.ExpandWidth(true);
            }

            if (fbxFiles == null || (fbxFiles != null && fbxFiles.Length != capacity))
                fbxFiles = new GameObject[capacity];

            for (int i = 0; i < fbxFiles.Length; i++)
            {
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(string.Format("FBX File ({0})", i + 1), GUILayout.Width(150));
                    fbxFiles[i] = (GameObject)EditorGUILayout.ObjectField(fbxFiles[i], typeof(GameObject), false, GUILayout.Width(200));
                    GUILayout.ExpandWidth(true);
                }
            }

            isClearOnStartUp = EditorGUILayout.ToggleLeft("ClearOnStartup", isClearOnStartUp, GUILayout.Width(120));
            isOverwrite = EditorGUILayout.ToggleLeft("Overwrite", isOverwrite, GUILayout.Width(120));

            if (GUILayout.Button("Import", GUILayout.Width(100)))
            {
                stateImporter.Import();
                stateImporter.Alignment();
            }
        }
    }
}
