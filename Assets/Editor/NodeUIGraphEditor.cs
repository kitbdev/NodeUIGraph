using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeUIGraphEditor : EditorWindow {

	string assetFilePath = "";
	ScriptableObject currentAsset;
	[MenuItem("Window/Node UI Graph Editor")]
	private static void OpenWindow() {
		NodeUIGraphEditor window = GetWindow<NodeUIGraphEditor>();
		window.titleContent = new GUIContent("Node UI Graph Editor");
	}
	private void OnFocus() {
		if (Selection.activeObject as NodeUIGraph != null) {
			currentAsset = Selection.activeObject as NodeUIGraph;
		}
	}
	private void OnGUI() {
		

		if (currentAsset==null) {
			// if ()
		}
		DrawAllNodes();


		if (GUI.changed) Repaint();
	}
	private void DrawAllNodes() {

	}
	private void ProcessInput(Event e) {

	}
	public void SaveAsset() {
		if (assetFilePath.Length==0) {
			CreateAsset();
		}
	}
	[MenuItem("Assets/Create/NodeUIGraph")]
	private static string CreateAsset() {
		string filePath = AssetDatabase.GetAssetPath(Selection.activeObject);
		filePath = filePath + "/nodeuigraph.asset";
		ScriptableObject asset = ScriptableObject.CreateInstance<NodeUIGraph>();
		ProjectWindowUtil.CreateAsset(asset, filePath);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		return filePath;
	}
	private void UpdateAsset() {
		// currentAsset = 

		AssetDatabase.Refresh();
	}
	[UnityEditor.Callbacks.OnOpenAsset(1)]
	public static bool OnOpenAsset(int instanceID, int line) {
		if (Selection.activeObject as NodeUIGraph != null) {
			OpenWindow();
			return true;
		}
		return false;
	}
}
