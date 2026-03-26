namespace TobyFredson
{
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(TobyGlobalShadersController))]
	public class TobyGlobalShadersController_Editor : Editor
	{
		private Texture2D HFGUI_GrassifyHeader;
		private SerializedProperty windType;
		private SerializedProperty windStrength;
		private SerializedProperty windSpeed;
		private SerializedProperty season;

		void OnEnable()
		{
			HFGUI_GrassifyHeader = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Toby Fredson/The Toby Foliage Engine/(TTFE)_Core/Resources/(TTFE) GLOBAL CONTROLLER/Scripts/Editor/TTFE_Graphic Editor.png");

			windType = serializedObject.FindProperty("windType");
			windStrength = serializedObject.FindProperty("windStrength");
			windSpeed = serializedObject.FindProperty("windSpeed");
			season = serializedObject.FindProperty("season");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(HFGUI_GrassifyHeader, GUILayout.Width(192), GUILayout.Height(96));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUILayout.LabelField("The Toby Foliage Engine version 1.1.0", EditorStyles.centeredGreyMiniLabel);

			GUIStyle textStyle = new GUIStyle(EditorStyles.label)
			{
				alignment = TextAnchor.MiddleLeft,
				fontStyle = FontStyle.Bold,
				fontSize = 12,
				normal = { textColor = Color.white }
			};
			Color rectColor = new Color(0.303f, 0.364f, 0.303f, 1f);

			Rect windRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight + 6);
			windRect.width -= 24;
			windRect.height = EditorGUIUtility.singleLineHeight + 6;
			EditorGUI.DrawRect(windRect, rectColor);
			Rect windTextRect = new Rect(windRect.x + 12, windRect.y + 1, windRect.width - 24, EditorGUIUtility.singleLineHeight + 2);
			EditorGUI.LabelField(windTextRect, "Global Wind Parameters", textStyle);

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(windType);
			EditorGUILayout.PropertyField(windStrength);
			EditorGUILayout.PropertyField(windSpeed);
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			EditorGUILayout.Space();

			Rect seasonRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight + 6);
			seasonRect.width -= 24;
			seasonRect.height = EditorGUIUtility.singleLineHeight + 6;
			EditorGUI.DrawRect(seasonRect, rectColor);
			Rect seasonTextRect = new Rect(seasonRect.x + 12, seasonRect.y + 1, seasonRect.width - 24, EditorGUIUtility.singleLineHeight + 2);
			EditorGUI.LabelField(seasonTextRect, "Season Control", textStyle);

			EditorGUILayout.PropertyField(season);
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			EditorGUILayout.Space();

			var controllerTarget = (TobyGlobalShadersController)target;
			EditorGUILayout.LabelField("Material Counts", EditorStyles.boldLabel);
			EditorGUILayout.LabelField($"Grass Foliage: {controllerTarget.GetMaterialCount("Grass")}");
			EditorGUILayout.LabelField($"Tree Bark: {controllerTarget.GetMaterialCount("Bark")}");
			EditorGUILayout.LabelField($"Tree Foliage: {controllerTarget.GetMaterialCount("Foliage")}");
			EditorGUILayout.LabelField($"Tree Billboard: {controllerTarget.GetMaterialCount("Billboard")}");
			EditorGUILayout.LabelField($"Global Controller: {controllerTarget.GetMaterialCount("Controller")}");

			EditorGUILayout.EndVertical();

			EditorGUILayout.HelpBox("Select a wind type: 'Gentle Breeze' for subtle wind effects or 'Wind Off' to disable wind. Only one wind type can be active at a time.", MessageType.Info);

			serializedObject.ApplyModifiedProperties();
		}
	}
}