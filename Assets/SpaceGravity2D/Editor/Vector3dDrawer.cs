using UnityEngine;
using UnityEditor;

namespace SpaceGravity2D
{
    /// <summary>
    /// Unity editor extension - property drawer for Vector3d type.
    /// </summary>
    /// <seealso cref="UnityEditor.PropertyDrawer" />
    [CustomPropertyDrawer(typeof(Vector3d))]
    public class Vector3dDrawer : PropertyDrawer
    {
        private const float SHRINK_WIDTH = 550;
        private const float DEFAULT_HEIGHT = 16f;
        private const float SHRINKED_ADDITIONAL_HEIGHT = 18f;
        private const float LABEL_WIDTH = 14f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Screen.width < SHRINK_WIDTH ? DEFAULT_HEIGHT + SHRINKED_ADDITIONAL_HEIGHT : DEFAULT_HEIGHT;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            Rect contentRect = EditorGUI.PrefixLabel(position, label);
            if (position.height > DEFAULT_HEIGHT)
            {
                position.height = DEFAULT_HEIGHT;
                EditorGUI.indentLevel++;
                contentRect = EditorGUI.IndentedRect(position);
                contentRect.y += SHRINKED_ADDITIONAL_HEIGHT;
            }
            contentRect.width /= 3f;
            EditorGUI.indentLevel = 0;
            EditorGUIUtility.labelWidth = LABEL_WIDTH;
            EditorGUI.PropertyField(contentRect, property.FindPropertyRelative("x"), new GUIContent("x"));
            contentRect.x += contentRect.width;
            EditorGUI.PropertyField(contentRect, property.FindPropertyRelative("y"), new GUIContent("y"));
            contentRect.x += contentRect.width;
            EditorGUI.PropertyField(contentRect, property.FindPropertyRelative("z"), new GUIContent("z"));
            EditorGUI.EndProperty();
        }
    }
}