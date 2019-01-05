using UnityEditor;

[CustomEditor(typeof(SetParam))]
public class SetParamEditor : Editor {
	public override void OnInspectorGUI() {
		//serializedObject.ShowScriptInput();

		serializedObject.Update();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("when"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("what"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("paramName"));

		switch ((SetParam.Type) serializedObject.FindProperty("what").intValue) {
			case SetParam.Type.Bool:
				EditorGUILayout.PropertyField(serializedObject.FindProperty("wantedBool"));
				break;
			case SetParam.Type.Trigger:
				EditorGUILayout.PropertyField(serializedObject.FindProperty("wantedBool"));
				break;
			case SetParam.Type.Int:
				EditorGUILayout.PropertyField(serializedObject.FindProperty("wantedInt"));
				break;
			case SetParam.Type.Float:
				EditorGUILayout.PropertyField(serializedObject.FindProperty("wantedFloat"));
				break;
		}

		serializedObject.ApplyModifiedProperties();
	}
}
