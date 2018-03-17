using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// contains all info about the node ui graph
// including all nodes and their positions and their connections
public class NodeUIGraph : ScriptableObject {

	private Vector2 defaultNodeSize = new Vector2(200, 50);
	public List<Node> nodes;
	public Node tNode;

	void OnEnable()
	{
		
		// get node ui graph object
		// AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(this));
		Object[] allObjects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
		foreach (Object obj in allObjects)
		{
			if (obj.GetType()==typeof(Node)||obj.GetType().IsSubclassOf(typeof(Node))) {
				((Node)obj).graph = this;
			}
		}
	}

	public void AddNodeAt(Vector2 pos) {
		if (nodes == null) {
			nodes = new List<Node>();
		}
		Node node = ScriptableObject.CreateInstance<Node>();
		node.nodeUIRect = new Rect(pos, defaultNodeSize);
		node.graph = this;
		nodes.Add(node);
		Debug.Log("Added node at " + pos);
		AssetDatabase.AddObjectToAsset(node, this);
		AssetDatabase.SaveAssets();	
	}
	public void RemoveNode(Node node) {
		if (nodes.Contains(node)) {
			nodes.Remove(node);
		}
		// remove connections
		DestroyImmediate(node, true);
		AssetDatabase.SaveAssets();
	}
	public void Save() {
		AssetDatabase.SaveAssets();
	}

	public bool CanConnect(NodeConnector input, NodeConnector output) {
		// todo

		return false;
	}
	public void Connect(NodeConnector input, NodeConnector output) {
		// todo
		AssetDatabase.SaveAssets();
	}

	public SerializedObject GetSerializedNode(Node node) {
		SerializedObject snode = new SerializedObject(node);
		return snode;
	}

	public int GetNumInputsOnNode(Node node) {
		// serialize and count the properties
		return 0;
	}
}
