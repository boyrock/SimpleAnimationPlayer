using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(AnimationPlayer))]
public class AnimationPlayerEditor : Editor {

    private ReorderableList list;

    private bool _isAllCheck;
    private bool isAllCheck
    {
        get
        {
            return _isAllCheck;
        }
        set
        {
            if(_isAllCheck != value)
            {
                SetIsEnable(value);
            }

            _isAllCheck = value;
        }
    }


    private void OnEnable()
    {
        AnimationPlayer ap = target as AnimationPlayer;

        if (ap.playlist != null)
        {
            if (ap.playlist.Count == 0)
            {
                ap.playlist = GetAnimatorStateList(ap.GetComponent<Animator>());
            }
        }

        var prop = serializedObject.FindProperty("playlist");

        if(list == null)
        {
            list = new ReorderableList(serializedObject, prop, true, true, true, true);
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                EditorGUI.LabelField(new Rect(rect.x, rect.y, 30, EditorGUIUtility.singleLineHeight), (index + 1).ToString());
                EditorGUI.PropertyField(new Rect(rect.x + 30, rect.y, 200, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("name"), GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + 240, rect.y, 30, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("loopCount"), GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + 290, rect.y, 30, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("isEnable"), GUIContent.none);
            };

            list.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(new Rect(rect.x + 40, rect.y, 100, EditorGUIUtility.singleLineHeight), "animation name");
                EditorGUI.LabelField(new Rect(rect.x + 250, rect.y, 30, EditorGUIUtility.singleLineHeight), "loop count");
                //EditorGUI.LabelField(new Rect(rect.x + 300, rect.y, 60, EditorGUIUtility.singleLineHeight), "isEnable");
                isAllCheck = EditorGUI.ToggleLeft(new Rect(rect.x + 300, rect.y, 150, EditorGUIUtility.singleLineHeight), "isEnaled", isAllCheck);
            };
        }
    }

    private List<AnimationPlaylistItem> GetAnimatorStateList(Animator animator)
    {
        List<AnimationPlaylistItem> list = new List<AnimationPlaylistItem>();
        if (animator != null)
        {
            UnityEditor.Animations.AnimatorController ac = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

            if (ac != null)
            {
                for (int i = 0; i < ac.animationClips.Length; i++)
                {
                    var clip = ac.animationClips[i];
                    list.Add(new AnimationPlaylistItem(clip.name, 1));
                }
            }
        }

        return list;
    }

    private void SetIsEnable(bool value)
    {
        AnimationPlayer ap = target as AnimationPlayer;
        for (int i = 0; i < ap.playlist.Count; i++)
        {
            ap.playlist[i].isEnable = value;
        }
    }

    public override void OnInspectorGUI()
    {
        AnimationPlayer ap = target as AnimationPlayer;

        ap.playOnStart = EditorGUILayout.Toggle("PlayOnStart", ap.playOnStart);
        ap.isRepeatPlay = EditorGUILayout.Toggle("IsRepeatPlay", ap.isRepeatPlay);

        EditorUtility.SetDirty(target);

        serializedObject.Update();

        list.DoLayoutList();

        List<AnimationPlaylistItem> appendItems = new List<AnimationPlaylistItem>();
        if (GUILayout.Button("Update Animation Playlist"))
        {
            // delete files

            var animatorStates = GetAnimatorStateList(ap.GetComponent<Animator>());

            AnimationPlaylistItem[] copied = new AnimationPlaylistItem[ap.playlist.Count];

            ap.playlist.CopyTo(copied);

            ap.playlist.Clear();

            for (int i = 0; i < animatorStates.Count; i++)
            {

                var state = animatorStates[i];

                for (int j = 0; j < copied.Length; j++)
                {
                    //var state = animatorStates[i];
                    if (state.name.Equals(copied[j].name))
                    {
                        copied[j].CopyTo(state);
                        break;
                    }
                }

                appendItems.Add(state);
            }
        }

        ap.playlist.AddRange(appendItems);

        serializedObject.ApplyModifiedProperties();

    }
}
