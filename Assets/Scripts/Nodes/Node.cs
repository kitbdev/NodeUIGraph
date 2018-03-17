using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Node : ScriptableObject {

	public Rect nodeUIRect;
	public string nodeUITitle;
	private bool isDragged;
	protected bool isSelected;
	protected NodeUIGraph graph;
	public List<NodeOutput> outputs;

	//testing
	public string prop;
	public int prop2;
	public GameObject proprtyNumberThree;

	public Node() {}

	public void ProcessEvents(Event e) {
		switch (e.type) {
			case EventType.MouseDown:
				if (nodeUIRect.Contains(e.mousePosition)) {
					if (e.button == 0) {
						isDragged = true;
						isSelected = true;
						GUI.changed = true;
					} else if (e.button == 1) {
						isSelected = true;
						GUI.changed = true;
						GenericMenu contextMenu = new GenericMenu();
						HandleContextMenu(contextMenu, e);
					}
				} else {
					if (e.button == 0) {
						isSelected = false;
						GUI.changed = true;
					}
				}
				break;
			case EventType.MouseUp:
				isDragged = false;
				break;
			case EventType.MouseDrag:
				if (e.button == 0 && isDragged) {
					Drag(e.delta);
					e.Use();
					GUI.changed = true;
				}
				break;
		}
	}
	private void Drag(Vector2 delta) {
		nodeUIRect.position += delta;
	}
	protected void HandleContextMenu(GenericMenu contextMenu, Event e) {
		contextMenu.AddItem(new GUIContent("Remove Node"), false, () => { graph.RemoveNode(this); });

		contextMenu.ShowAsContext();
	}

	public void AddOutput<T>(string name, T defaultValue = default(T)) {
		outputs.Add(new NodeOutput<T>(name, defaultValue));
	}

	public void SetOutput<T>(string name, T value) {
		NodeOutput nodeOutput = outputs.Find((no) => { return no.name == name; });
		if (nodeOutput != null) {
			nodeOutput.SetValue(value);
		}
	}

	public virtual void Start() {}
	public virtual void Update() {}
	public virtual void OnGUI() {}
}
public abstract class NodeOutput {

	public string name;
	public Node connection;
	public NodeOutput(string name) {
		this.name = name;
	}
	public abstract T GetValue<T>();
	public abstract void SetValue<T>(T t);
}
public class NodeOutput<T> : NodeOutput {
	public T value;
	public T defaultValue;
	public NodeOutput(string name, T defaultValue) : base(name) {
		this.defaultValue = defaultValue;
		this.value = this.defaultValue;
	}

	public override T1 GetValue<T1>() {
		try {
			if (typeof(T1) == typeof(T) || typeof(T1).IsSubclassOf(typeof(T))) {
				return (T1) Convert.ChangeType(value, typeof(T1));
			}
		} catch (Exception e) {}
		Debug.LogError("Invalid Get Type '" + typeof(T1).ToString() + "' on node output " + name);
		return default(T1);
	}

	public override void SetValue<T1>(T1 t) {
		try {
			if (typeof(T1) == typeof(T) || typeof(T1).IsSubclassOf(typeof(T))) {
				value = (T) Convert.ChangeType(t, typeof(T));
			}
		} catch (Exception e) {}
		Debug.LogError("Invalid Set Type '" + typeof(T1).ToString() + "' of Value '" + t.ToString() + "' on node output " + name);
	}
}