using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// contains all info about the node ui graph
// including all nodes and their positions and their connections
public class NodeUIGraph : ScriptableObject {

	private Vector2 defaultNodeSize = new Vector2(200, 50);
	public List<Node> nodes {get;protected set;}
	public void AddNodeAt(Vector2 pos) {
		if (nodes == null) {
			nodes = new List<Node>();
		}
		Node node = ScriptableObject.CreateInstance<Node>();
		node.nodeUIRect = new Rect(pos, defaultNodeSize);
		nodes.Add(node);
		Debug.Log("Added node at " + pos);
	}
	public SerializedObject GetSerializedNode(Node node) {
		SerializedObject snode = new SerializedObject(node);
		return snode;
	}
	public void SErialize() {
// if (serializedNodes == null) {
// 			serializedNodes = new List<SerializedObject>();
// 		}

// 		SerializedObject snode = new SerializedObject(node);
// 		serializedNodes.Add(snode);
	}
	public void RemoveNode(Node node) {
		if (nodes.Contains(node)) {
			nodes.Remove(node);
		}
	}

}
