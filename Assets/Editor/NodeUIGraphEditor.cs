using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NodeUIGraphEditor : EditorWindow {

	Vector2 offset;

	private Rect menuBar;
	
	// SerializedProperty nodes;
	public NodeUIGraph nodeUIGraph;

	[MenuItem("Window/Node UI Graph Editor")]
	private static void OpenWindow() {
		NodeUIGraphEditor window = GetWindow<NodeUIGraphEditor>();
		window.titleContent = new GUIContent("Node UI Graph Editor");
	}
	void OnEnable() {
	}
	private void Awake() {
		OnSelectionChange();
	}
	private void OnSelectionChange() {
		// select the current object
		if (Selection.activeObject as NodeUIGraph != null) {
			nodeUIGraph = Selection.activeObject as NodeUIGraph;
		}
		Repaint();
	}
	private void OnGUI() {

		DrawGrid(20, 0.2f, Color.grey);
		DrawGrid(100, 0.4f, Color.grey);
		DrawMenuBar();
		
		if (nodeUIGraph != null) {
			DrawAllNodes();

			ProcessNodeEvents(Event.current);
		}
		ProcessEvents(Event.current);

		if (GUI.changed) Repaint();
	}
	private void DrawGrid(float spacing, float opacity, Color color) {
		int numHorizontalBars = Mathf.CeilToInt(position.height / spacing);
		int numVerticalBars = Mathf.CeilToInt(position.width / spacing);
		Handles.BeginGUI();
		Handles.color = new Color(color.r, color.g, color.b, opacity);

		Vector2 drawOffset = new Vector2(offset.x % spacing, offset.y % spacing);

		for (int i = 0; i < numHorizontalBars; i++) {
			Handles.DrawLine(new Vector2(-spacing, i * spacing) + drawOffset, new Vector2(position.width, i * spacing) + drawOffset);
		}
		for (int i = 0; i < numVerticalBars; i++) {
			Handles.DrawLine(new Vector2(i * spacing, -spacing) + drawOffset, new Vector2(i * spacing, position.height) + drawOffset);
		}

		Handles.color = Color.white;
		Handles.EndGUI();
	}
	private void DrawMenuBar() {
		menuBar = new Rect(0, 0, position.width, 20f);
		GUILayout.BeginArea(menuBar, EditorStyles.toolbar);
		GUILayout.BeginHorizontal();

		if (nodeUIGraph != null) {
			GUILayout.Label(nodeUIGraph.name, GUILayout.ExpandWidth(false));
			GUILayout.Space(5);
			GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(35));
			GUILayout.Space(5);
		} else {
			GUILayout.Label("Select a node ui graph", GUILayout.ExpandWidth(false));
			GUILayout.Space(5);
		}
		GUILayout.Button("Load", EditorStyles.toolbarButton, GUILayout.Width(35));
		GUILayout.Space(5);
		GUILayout.Button("Options", EditorStyles.toolbarButton, GUILayout.Width(55));

		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	private void DrawAllNodes() {
		if (nodeUIGraph.nodes != null) {
			for (int i = 0; i < nodeUIGraph.nodes.Count; i++) {
				nodeUIGraph.nodes[i].NodeDraw(nodeUIGraph.GetSerializedNode(nodeUIGraph.nodes[i]));
			}
		}
	}
	private void ProcessNodeEvents(Event e) {
		if (nodeUIGraph.nodes != null) {
			for (int i = nodeUIGraph.nodes.Count - 1; i >= 0; i--) {
				nodeUIGraph.nodes[i].ProcessEvents(e);
			}
		}
	}
	private void ProcessEvents(Event e) {
		switch (e.type) {
			case EventType.MouseDown:
				if (e.button == 0) {
					Selection.activeObject = nodeUIGraph;
				}
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

	[MenuItem("Assets/Create/NodeUIGraph")]
	private static void CreateAsset() {
		string filePath = AssetDatabase.GetAssetPath(Selection.activeObject);
		if (Selection.activeObject && Selection.activeObject.GetType() != typeof(UnityEngine.Object)) {
			filePath = filePath.Substring(0, filePath.LastIndexOf("/"));
		}
		filePath = filePath + "/nodeuigraph.asset";
		ScriptableObject asset = ScriptableObject.CreateInstance<NodeUIGraph>();
		ProjectWindowUtil.CreateAsset(asset, filePath);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
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