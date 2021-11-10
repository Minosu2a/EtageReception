using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineDecorator))]
public class SplineDecoratorInspector : Editor
{
	private SplineDecorator decorator;
	
	private BezierSpline spline;
	private int StepSize;



	public override void OnInspectorGUI()
	{
		decorator = target as SplineDecorator;
		var sd = new SerializedObject(decorator);
		EditorGUILayout.PropertyField(sd.FindProperty("spline"), new GUIContent("Spline"));
		EditorGUILayout.PropertyField(sd.FindProperty("StepSize"), new GUIContent("Step Size"));
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
		GUILayout.Label("Spawn");
		EditorGUI.BeginChangeCheck();
		bool spawn = EditorGUILayout.Toggle("On Play", decorator.spawnOnPlay);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(decorator, "On Play");
			EditorUtility.SetDirty(decorator);
			decorator.spawnOnPlay = spawn;
		}
		EditorGUI.BeginChangeCheck();
		bool spawnn = EditorGUILayout.Toggle("Only Selected Curves", decorator.spawnSelectedOnly);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(decorator, "Only Selected Curves");
			EditorUtility.SetDirty(decorator);
			decorator.spawnSelectedOnly = spawnn;
		}
		if (GUILayout.Button("Spawn Now"))
		{
			decorator.SpawnNow();
			EditorUtility.SetDirty(decorator);
		}
		EditorGUI.BeginChangeCheck();
		if (GUILayout.Button("Clear"))
		{
			decorator.Clear();
			EditorUtility.SetDirty(decorator);
		}
		
	}
}
