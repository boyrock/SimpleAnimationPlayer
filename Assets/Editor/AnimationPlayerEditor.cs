using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(AnimationPlayer))]
public class AnimationPlayerEditor : Editor {

    private ReorderableList list;

    private void OnEnable()
    {
        var prop = serializedObject.FindProperty("playlist");

        list = new ReorderableList(serializedObject, prop, true, true, true, true);
        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            EditorGUI.LabelField(new Rect(rect.x, rect.y, 30, EditorGUIUtility.singleLineHeight), (index + 1).ToString());
            EditorGUI.PropertyField(new Rect(rect.x + 30, rect.y, 200, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("name"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + 240, rect.y, 30, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("loopCount"), GUIContent.none);
        };

        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(new Rect(rect.x + 40, rect.y, 100, EditorGUIUtility.singleLineHeight), "animation name");
            EditorGUI.LabelField(new Rect(rect.x + 250, rect.y, 100, EditorGUIUtility.singleLineHeight), "loop count");
        };
    }
    public override void OnInspectorGUI()
    {
        AnimationPlayer ap = target as AnimationPlayer;

        ap.playOnStart = EditorGUILayout.Toggle("PlayOnStart", ap.playOnStart);
        ap.isRepeatPlay = EditorGUILayout.Toggle("IsRepeatPlay", ap.isRepeatPlay);

        EditorUtility.SetDirty(target);

        serializedObject.Update();

        list.DoLayoutList();

        serializedObject.ApplyModifiedProperties();

    }
}
