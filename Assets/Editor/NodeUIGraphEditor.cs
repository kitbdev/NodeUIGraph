using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NodeUIGraphEditor : EditorWindow {

	Vector2 offset;

	GUIStyle nodeStyle;
	GUIStyle nodeSelectedStyle;

	private Rect menuBar;

	NodeUIGraph nodeUIGraph;

	[MenuItem("Window/Node UI Graph Editor")]
	private static void OpenWindow() {
		NodeUIGraphEditor window = GetWindow<NodeUIGraphEditor>();
		window.titleContent = new GUIContent("Node UI Graph Editor");
	}
	void OnEnable() {
		// setup style
		nodeStyle = new GUIStyle();
		nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
		nodeStyle.border = new RectOffset(12, 12, 12, 12);
		
		nodeSelectedStyle = new GUIStyle();
		nodeSelectedStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
		nodeSelectedStyle.border = new RectOffset(12, 12, 12, 12);
		
	}
	private void OnFocus() {
		// select the current object
		if (Selection.activeObject as NodeUIGraph != null) {
			nodeUIGraph = Selection.activeObject as NodeUIGraph;
		}
	}
	private void OnGUI() {

		DrawGrid(20, 0.2f, Color.grey);
		DrawGrid(100, 0.4f, Color.grey);
		DrawMenuBar();
		Debug.Log(nodeUIGraph);
		if (nodeUIGraph == null) {
			// if ()
			// new and load buttons
		}
		DrawAllNodes();


		ProcessNodeEvents(Event.current);
		ProcessEvents(Event.current);

		if (GUI.changed) Repaint();
	}
	private void DrawGrid(float spacing, float opacity, Color color) {
		int numHorizontalBars = Mathf.CeilToInt(position.height / spacing);
		int numVerticalBars = Mathf.CeilToInt(position.width / spacing);
		Handles.BeginGUI();
		Handles.color = new Color(color.r, color.g, color.b, opacity);
		

		Vector2 drawOffset = new Vector2(offset.x % spacing, offset.y % spacing);

		for (int i = 0; i < numHorizontalBars; i++)
		{
			Handles.DrawLine(new Vector2(-spacing, i*spacing)+drawOffset, new Vector2(position.width, i*spacing)+drawOffset);
		}
		for (int i = 0; i < numVerticalBars; i++)
		{
			Handles.DrawLine(new Vector2(i*spacing, -spacing)+drawOffset, new Vector2(i*spacing, position.height)+drawOffset);
		}

		Handles.color = Color.white;
		Handles.EndGUI();
	}
	private void DrawMenuBar() {
		menuBar = new Rect(0, 0, position.width, 20f);
		GUILayout.BeginArea(menuBar, EditorStyles.toolbar);
		GUILayout.BeginHorizontal();

		GUILayout.Button("Load", EditorStyles.toolbarButton, GUILayout.Width(35));
		GUILayout.Space(5);
		GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(35));
		GUILayout.Space(5);
		GUILayout.Button("Options", EditorStyles.toolbarButton, GUILayout.Width(55));

		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	private void DrawAllNodes() {
		if (nodeUIGraph != null && nodeUIGraph.nodes != null) {
			for (int i = 0; i < nodeUIGraph.nodes.Count; i++) {

				DrawNode(nodeUIGraph.GetSerializedNode(nodeUIGraph.nodes[i]));
			}
		}
	}
	private void DrawNode(SerializedObject node) {
		// node.Update();
		Debug.Log ("Drawing "+node.GetType());
		Rect rect = node.FindProperty("nodeUIRect").rectValue;
		GUI.Box(rect, node.FindProperty("nodeUITitle").stringValue, nodeStyle);
		SerializedProperty sp = node.GetIterator();
		sp.Next(true);
		sp.NextVisible(true); // ignore Script
		sp.NextVisible(true); // ignore title
		sp.NextVisible(false); // ignore rect
		float lineHeight = EditorGUIUtility.singleLineHeight;
		Rect nextPropRect = new Rect(rect.x + 10, rect.y + 5, rect.width / 5, lineHeight);
		while (sp.NextVisible(false)) {
			Debug.Log("showing " + sp.displayName + " at " + nextPropRect);

			EditorGUI.PropertyField(nextPropRect, sp);
			nextPropRect.y += lineHeight + 2;
		}
		// rect.height = nextPropRect.y - rect.y;
	}
	private void ProcessNodeEvents(Event e) {
		for (int i = nodeUIGraph.nodes.Count - 1; i >= 0; i--) {
			nodeUIGraph.nodes[i].ProcessEvents(e);
		}
	}
	private void ProcessEvents(Event e) {
		switch (e.type) {
			case EventType.MouseDown:
				if (e.button == 1) {
					GenericMenu contextMenu = new GenericMenu();
					contextMenu.AddItem(new GUIContent("Add Node"), false, () => { nodeUIGraph.AddNodeAt(e.mousePosition); });
					contextMenu.ShowAsContext();
				}
				break;
		}
	}
	public void UpdateNodes() {

	}
	public void SaveNodes() {
		// EditorUtility.SetDirty();
		//todo
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