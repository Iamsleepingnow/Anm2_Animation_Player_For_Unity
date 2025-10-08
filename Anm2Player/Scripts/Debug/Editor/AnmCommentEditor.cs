using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Iamsleepingnow.Anm2Player
{
    [CustomEditor(typeof(AnmComment))]
    public class AnmCommentEditor : Editor
    {
        SerializedProperty inEdit;
        SerializedProperty comment;
        GUIContent textContent, editButtonContent;
        GUIStyle window, iconStyle, textStyle;

        private void OnEnable() {
            inEdit = serializedObject.FindProperty("inEdit");
            comment = serializedObject.FindProperty("comment");
            textContent = new GUIContent();
            editButtonContent = new GUIContent("Edit", null, "Enable or Disable Edit Mode");
        }
        
        private void OnDisable() {
            inEdit.boolValue = false; if (serializedObject != null && serializedObject.targetObject != null) serializedObject.ApplyModifiedProperties();
        }
        
        public override bool UseDefaultMargins() {
            return false;
        }
        
        public override void OnInspectorGUI() {
            serializedObject.Update();
            GUIStyle style = new GUIStyle();
            style.fontSize = 18;
            textStyle = new GUIStyle(style);
            window = new GUIStyle(style);
            iconStyle = new GUIStyle(style);
            //
            window.alignment = TextAnchor.UpperLeft;
            window.padding = new RectOffset(20, 20, 0, 30);
            EditorGUILayout.BeginVertical(window);
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Box(new GUIContent(), iconStyle, GUILayout.Height(30));
                    GUILayout.Space(-10);
                    if (GUILayout.Button(editButtonContent, GUIStyle.none, GUILayout.Width(30), GUILayout.Height(30))) {
                        GenericMenu menu = new GenericMenu();
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent(!inEdit.boolValue ? "Edit Comment" : "Exit Edit"), false, () => { inEdit.boolValue = !inEdit.boolValue; serializedObject.ApplyModifiedProperties(); });
                        menu.AddSeparator("");
                        menu.ShowAsContext();
                    }
                }
                GUILayout.EndHorizontal();
                if (inEdit.boolValue) {
                    EditorGUILayout.PropertyField(comment, GUIContent.none);
                }
                else {
                    if (textContent != null && textStyle != null) {
                        textStyle.richText = true;
                        textStyle.normal.background = null;
                        textStyle.normal.textColor = new Color(0.45f, 0.45f, 0.45f);
                        textStyle.wordWrap = true;
                        textStyle.font = window.font;
                        textStyle.fontStyle = window.fontStyle;
                        textStyle.fontSize = window.fontSize;
                        textStyle.alignment = window.alignment;
                        GUILayout.Box(comment.stringValue, textStyle);
                    }
                }
            }
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
    }
}