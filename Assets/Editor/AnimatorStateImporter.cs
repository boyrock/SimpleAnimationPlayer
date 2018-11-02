using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class AnimatorStateImporter : Editor {

    AnimatorStateMachine rootStateMachine;
    ChildAnimatorState[] states;

    public AnimatorController controller;

    public DefaultAsset directory { get; set; }
    public GameObject[] fbxFiles { get; set; }
    public bool isClearOnStartUp { get; set; }
    public bool isOverwrite { get; set; }

    public void Alignment()
    {
        if (rootStateMachine == null)
            return;

        float yOffset = 200;
        var childStates = rootStateMachine.states;

        Vector3 position = Vector3.zero;
        for (int i = 0; i < childStates.Length; i++)
        {
            position.x = i % 5 * 210;
            position.y = yOffset + i / 5 * 50;

            var childState = childStates[i];
            childState.position = position;
            childStates[i] = childState;
        }

        rootStateMachine.states = childStates;
    }

    public bool Import()
    {
        if (controller == null)
            return false;

        if (directory == null && fbxFiles.Length == 0)
            return false;

        rootStateMachine = controller.layers[0].stateMachine;
        states = rootStateMachine.states;

        if (isClearOnStartUp == true)
        {
            // Clear States
            for (int i = 0; i < states.Length; i++)
            {
                rootStateMachine.RemoveState(states[i].state);
            }
        }

        states = rootStateMachine.states;

        AnimatorState readyState = FindReadyState();

        if (readyState == null)
            readyState = rootStateMachine.AddState("Ready");

        rootStateMachine.defaultState = readyState;

        #region selection

        //Object[] selectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
        //foreach (Object obj in selectedAsset)
        //{
        //    string selectionPath = AssetDatabase.GetAssetPath(obj);
        //    if (Directory.Exists(selectionPath))
        //    {
        //        //Debug.Log("<color=blue>dir : " + obj + "</color>");
        //        directories.Add(selectionPath);
        //    }
        //    else
        //    {
        //        //Debug.Log("obj : " + obj);
        //    }
        //}

        #endregion

        List<string> directories = null;

        if (directory != null)
        {
            directories = new List<string>();
            string selectionPath = AssetDatabase.GetAssetPath(directory);

            //if (string.IsNullOrEmpty(selectionPath))
            //    return false;

            directories.Add(selectionPath);

            var guids = AssetDatabase.FindAssets("t:Model", directories.ToArray());

            foreach (var guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                InsertFbxAnimationClips(assetPath);
            }
            return true;
        }

        if (fbxFiles != null)
        {
            for (int i = 0; i < fbxFiles.Length; i++)
            {
                var fbx = fbxFiles[i];
                var assetPath = AssetDatabase.GetAssetPath(fbx);
                InsertFbxAnimationClips(assetPath);
            }
            return true;
        }

        return false;
    }

    AnimatorState FindReadyState()
    {
        for (int i = 0; i < states.Length; i++)
        {
            if (states[i].state.name.Equals("Ready"))
            {
                return states[i].state;
            }
        }

        return null;
    }

    void InsertFbxAnimationClips(string fbxAssetPath)
    {
        Object[] objects = AssetDatabase.LoadAllAssetsAtPath(fbxAssetPath);
        foreach (Object obj in objects)
        {
            AnimationClip clip = obj as AnimationClip;
            if (clip != null && clip.name.Contains("_preview") == false)
            {
                InsertAnimationClip(clip);
            }
        }
    }

    bool InsertAnimationClip(AnimationClip clip)
    {
        if (clip == null)
            return false;

        var duplicateAnimation = GetDuplicatedAnimation(clip.name, states);

        if (isOverwrite == true)
        {
            if (duplicateAnimation != null)
            {
                duplicateAnimation.motion = clip;
                Debug.Log("<color=White>overwrite clip : " + clip + "</color>");
                return false;
            }
        }
        else
        {
            if (duplicateAnimation != null)
                return false;
        }

        var state = rootStateMachine.AddState(clip.name);

        state.motion = clip;

        Debug.Log("<color=green>add clip : " + clip + "</color>");
        return true;
    }

    AnimatorState GetDuplicatedAnimation(string newClipName, ChildAnimatorState[] states)
    {
        for (int i = 0; i < states.Length; i++)
        {
            if (newClipName.Equals(states[i].state.name))
                return states[i].state;
        }

        return null;
    }
}
