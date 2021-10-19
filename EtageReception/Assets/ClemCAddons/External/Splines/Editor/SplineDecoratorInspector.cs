using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineDecorator))]
public class SplineDecoratorInspector : Editor
{
	private SplineDecorator decorator;
	
	private BezierSpline spline;
	private int frequency;



	public override void OnInspectorGUI()
	{
		decorator = target as SplineDecorator;
		var sd = new SerializedObject(decorator);
		EditorGUILayout.PropertyField(sd.FindProperty("spline"), new GUIContent("Spline"));
		EditorGUILayout.PropertyField(sd.FindProperty("frequency"), new GUIContent("Frequency"));
		sd.ApplyModifiedProperties();
		EditorGUI.BeginChangeCheck();
		bool forward = EditorGUILayout.Toggle("Look Forward", decorator.lookForward);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(decorator, "Look Forward");
			EditorUtility.SetDirty(decorator);
			decorator.lookForward = forward;
		}
		EditorGUILayout.PropertyField(sd.FindProperty("items"), new GUIContent("Items"));
		sd.ApplyModifiedProperties();
		EditorGUI.BeginChangeCheck();
		bool spawn = EditorGUILayout.Toggle("Spawn on Play", decorator.spawnOnPlay);
		if (GUILayout.Button("Spawn Now"))
		{
			decorator.SpawnNow();
			EditorUtility.SetDirty(decorator);
		}
		if (GUILayout.Button("Clear"))
		{
			decorator.Clear();
			EditorUtility.SetDirty(decorator);
		}
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(decorator, "Spawn on Play");
			EditorUtility.SetDirty(decorator);
			decorator.spawnOnPlay = spawn;
		}
	}
}
