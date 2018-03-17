using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Node {

	public Rect node_rect;
	public string node_title;
	public GUIStyle node_style;

	private List<NodeInputField> inputs;
	// Dictionary<string, NodeInputField> inputs;
	private List<NodeOutputField> outputs;

	public Node() {}

	public void Drag(Vector2 delta) {
		node_rect.position += delta;
	}

	public void DrawNode() {
		GUI.Box(node_rect, node_title, node_style);
		// todo draw inputs and outputs
		// EditorGUI.
	}

	public void AddInput<T>(string propertyName, T defval = default(T)) {

		NodeInputField nif = new NodeInputField<T>(propertyName, defval);
		inputs.Add(nif);
	}
	public T GetInputFor<T>(string propertyName) {
		foreach (NodeInputField ni in inputs) {
			if (ni.propertyName == propertyName) {
				return ni.GetValue<T>();
			}
		}
		Debug.LogError("Invalid Property Name '" + propertyName + "' on node " + node_title);
		return default(T);
	}
	public void AddOutput<T>(string propertyName) {

		NodeOutputField nof = new NodeOutputField<T>(propertyName);
		outputs.Add(nof);
	}

	public virtual void Start() {}
	public virtual void Update() {}
	public virtual void OnGUI() {}
}

public abstract class NodeInputField {
	public string propertyName;
	public NodeInputField(string propertyName) {
		this.propertyName = propertyName;
	}
	public abstract T GetValue<T>();
	// public abstract void SetValue<T>(T t);
}

public class NodeInputField<T> : NodeInputField {

	public T data;
	public T defaultValue;
	public NodeOutputField connection;

	public NodeInputField(string propertyName, T defaultValue) : base(propertyName) {
		this.defaultValue = defaultValue;
	}

	public NodeInputField(string propertyName) : base(propertyName) {}

	public override T1 GetValue<T1>() {
		if (typeof(T1) == typeof(T) || typeof(T1).IsSubclassOf(typeof(T)))
			return (T1) Convert.ChangeType(data, typeof(T1));
		return default(T1);
	}
}

public abstract class NodeOutputField {
	public string propertyName;
	public NodeOutputField(string propertyName) {
		this.propertyName = propertyName;
	}
}

public class NodeOutputField<T> : NodeOutputField {

	public NodeOutputField(string propertyName) : base(propertyName) {}

	// public NodeOutputField(string propertyName) : base(propertyName) {}

}