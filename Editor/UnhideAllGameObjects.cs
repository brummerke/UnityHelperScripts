using UnityEditor;
using UnityEngine;

public class UnhideAllGameObjects : MonoBehaviour {

	[MenuItem("GameObject/Unhide Object(s)")]
	static void UnhideAll() {
		var a = Selection.objects;
		if (a == null || a.Length == 0) a = FindObjectsOfType<Object>();
		for (int i = 0; i < a.Length; i++) {
			a[i].hideFlags &= ~(HideFlags.HideInHierarchy | HideFlags.HideInHierarchy);
		}
		EditorApplication.RepaintHierarchyWindow();
	}

	[MenuItem("GameObject/Hide Object(s)")]
	static void Hide() {
		var a = Selection.objects;
		for (int i = 0; i < a.Length; i++) {
			a[i].hideFlags |= HideFlags.HideInHierarchy | HideFlags.HideInInspector;
		}
		EditorApplication.RepaintHierarchyWindow();
	}
}
