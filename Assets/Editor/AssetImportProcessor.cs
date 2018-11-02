using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class AssetImportProcessor : AssetPostprocessor
{
    private const string PREFAB_DESTINATION_DIRECTORY = "Assets/Prefabs/";
    private const string ANIMATOR_DESTINATION_DIRECTORY = "Assets/Animator/";

    void OnPreprocessModel()
    {
        ModelImporter modelImporter = assetImporter as ModelImporter;
        modelImporter.materialLocation = ModelImporterMaterialLocation.External;
    }

    void OnPreprocessAnimation()
    {
        ModelImporter modelImporter = assetImporter as ModelImporter;
        if (modelImporter.animationType == ModelImporterAnimationType.Legacy)
            modelImporter.animationType = ModelImporterAnimationType.Generic;

        modelImporter.animationCompression = ModelImporterAnimationCompression.Off;

        ModelImporterClipAnimation[] clipAnimations = modelImporter.clipAnimations;

        if (clipAnimations.Length == 0) clipAnimations = modelImporter.defaultClipAnimations;

        foreach (ModelImporterClipAnimation clip in clipAnimations)
        {
            if (clip.name.Contains("_loop"))
            {
                clip.loopTime = true;
            }
        }

        modelImporter.clipAnimations = clipAnimations;
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        for (int i = 0; i < importedAssets.Length; i++)
        {
            var importAssetPath = importedAssets[i];

            if (importAssetPath.ToLower().Contains("fbx/avatar") == false && importAssetPath.ToLower().Contains("fbx/animations/props") == false)
                continue;

            if (importAssetPath.ToLower().Contains(".fbx") == false)
                continue;

            GameObject modelAsset = AssetDatabase.LoadAssetAtPath<GameObject>(importAssetPath);

            if (modelAsset != null)
            {
                var prefab = CreatePrefabFromModel(modelAsset);

                ImportAnimatorState(modelAsset, prefab);

                var AnimationPlayer = prefab.GetComponent<AnimationPlayer>();

                List<AnimationPlaylistItem> playList = new List<AnimationPlaylistItem>();

                Object[] objects = AssetDatabase.LoadAllAssetsAtPath(importAssetPath);
                foreach (Object obj in objects)
                {
                    AnimationClip clip = obj as AnimationClip;
                    if (clip != null && clip.name.Contains("_preview") == false)
                    {
                        playList.Add(new AnimationPlaylistItem(clip.name, 1));
                    }
                }

                if (AnimationPlayer == null)
                    AddAnimationPlayerComponent(prefab, playList);
                else
                    AnimationPlayer.SetPlaylist(playList);
            }
        }
    }

    private static GameObject CreatePrefabFromModel(GameObject modelAsset)
    {
        if (Directory.Exists(PREFAB_DESTINATION_DIRECTORY) == false)
            Directory.CreateDirectory(PREFAB_DESTINATION_DIRECTORY);

        string destinationPath = PREFAB_DESTINATION_DIRECTORY + modelAsset.name + ".prefab";

        GameObject prefab = null;

        if (!File.Exists(destinationPath))
        {
            prefab = PrefabUtility.CreatePrefab(
                destinationPath,
                modelAsset);
        }
        else
        {
            prefab = PrefabUtility.ReplacePrefab(
                modelAsset,
                AssetDatabase.LoadAssetAtPath<GameObject>(destinationPath), ReplacePrefabOptions.ReplaceNameBased);
        }

        return prefab;
    }

    private static void ImportAnimatorState(GameObject modelAsset, GameObject prefab)
    {
        if (Directory.Exists(ANIMATOR_DESTINATION_DIRECTORY) == false)
            Directory.CreateDirectory(ANIMATOR_DESTINATION_DIRECTORY);

        if (prefab.GetComponent<AnimationController>() == null)
            prefab.AddComponent<AnimationController>();

        AnimatorController controller;

        Animator animator = prefab.GetComponent<Animator>();

        if (animator == null)
        {
            animator = prefab.gameObject.AddComponent<Animator>();
            //animator.avatar = modelImporter.sourceAvatar;
        }

        if (animator.runtimeAnimatorController == null)
        {
            controller = CreateAnimationController(prefab);
            animator.runtimeAnimatorController = (RuntimeAnimatorController)controller;
        }
        else
        {
            controller = (AnimatorController)animator.runtimeAnimatorController;
        }

        AnimatorStateImporter importer = new AnimatorStateImporter();
        importer.isClearOnStartUp = true;
        importer.controller = controller;
        importer.fbxFiles = new GameObject[1] { modelAsset };
        importer.Import();
        importer.Alignment();
    }

    static AnimatorController CreateAnimationController(GameObject modelAsset)
    {
        string path = ANIMATOR_DESTINATION_DIRECTORY + modelAsset.name + ".controller";

        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path);
        return controller;
    }

    static void AddAnimationPlayerComponent(GameObject prefab, List<AnimationPlaylistItem> playlist)
    {
        var component = prefab.AddComponent<AnimationPlayer>();
        component.SetPlaylist(playlist);
    }
}
