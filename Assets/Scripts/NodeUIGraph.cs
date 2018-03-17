using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// contains all info about the node ui graph
// including all nodes and their positions and their connections
public class NodeUIGraph : ScriptableObject {

	private Vector2 defaultNodeSize = new Vector2(200, 50);
	public List<Node> nodes;

	[NonSerialized]
	public bool isConnecting;

	void OnEnable() {
		// get nodes and set unserialized properties
		// AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(this));
		UnityEngine.Object[] allObjects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
		foreach (UnityEngine.Object obj in allObjects) {
			if (obj.GetType() == typeof(Node) || obj.GetType().IsSubclassOf(typeof(Node))) {
				((Node) obj).graph = this;
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
	public NodeConnector CreateConnector<T>(string propertyName, T defaultValue = default(T)) {
		NodeConnector<T> nc = ScriptableObject.CreateInstance<NodeConnector<T>>();
		nc.propertyName = propertyName;
		nc.defaultValue = defaultValue;
		return nc;
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

	public void BringToFront(Node node) {
		if (nodes.Contains(node) && nodes[nodes.Count-1] != node) {
			nodes.Remove(node);
			nodes.Add(node);
			AssetDatabase.SaveAssets();
		}
	}
}