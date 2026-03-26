using UnityEngine;
using UnityEditor;
using System;

namespace TobyFredson.Drawers
{
	public class TTFE_DrawerFeatureBorder : MaterialPropertyDrawer
	{
		public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
		{
			// Adjust position for the rectangle
			position.width -= 24;
			position.height = EditorGUIUtility.singleLineHeight + 12; // Height for bigger text

			// Draw a solid desaturated green rectangle (matching pink's vibe)
			Color rectColor = new Color(0.229f, 0.504f, 0.229f, 1f); // 4% lighter green
			EditorGUI.DrawRect(position, rectColor); // Fill the entire rectangle

			// Draw the text inside, centered vertically and aligned left
			Rect textRect = new Rect(position.x + 12, position.y + 3, position.width - 24, EditorGUIUtility.singleLineHeight + 6);
			GUIStyle textStyle = new GUIStyle(EditorStyles.label);
			textStyle.alignment = TextAnchor.MiddleLeft; // Center vertically, align left
			textStyle.fontStyle = FontStyle.Bold; // Bold text
			textStyle.fontSize = 12; // Bigger text
			textStyle.normal.textColor = Color.white; // White text for contrast
			EditorGUI.LabelField(textRect, label, textStyle);
		}

		// Set the height for the drawer
		public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
		{
			return EditorGUIUtility.singleLineHeight + 12; // Height for green rectangle and bigger text
		}
	}
}