using UnityEngine;
using UnityEditor;
using System;

namespace TobyFredson.Drawers
{
	public class TTFE_DrawerTitle : MaterialPropertyDrawer
	{
		public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
		{
			// Adjust position for the title
			position.width -= 24; // Match previous drawers' alignment
			position.height = EditorGUIUtility.singleLineHeight + 4; // Slightly taller for big text
			position.x += 12; // Offset to center within inspector width

			// Create style for the title
			GUIStyle titleStyle = new GUIStyle(EditorStyles.label);
			titleStyle.fontStyle = FontStyle.Bold; // Bold text
			titleStyle.fontSize = 14; // Big text (14pt)
			titleStyle.normal.textColor = Color.white; // White text for contrast
			titleStyle.alignment = TextAnchor.MiddleCenter; // Center horizontally and vertically
			titleStyle.margin = new RectOffset(0, 0, 5, 10); // Margin for spacing

			// Draw the label in all caps, centered
			EditorGUI.LabelField(position, label.ToUpper(), titleStyle);
		}

		// Set the height for the drawer
		public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
		{
			return EditorGUIUtility.singleLineHeight + 4; // Height for big text
		}
	}
}