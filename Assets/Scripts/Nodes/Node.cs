using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Node : ScriptableObject {

	public string nodeUITitle;
	public Rect nodeUIRect;
	private bool isDragged;
	protected bool isSelected;
	protected NodeUIGraph graph;
	public List<NodeConnector> inputConnections;
	public List<NodeConnector> outputs;

	//testing
	public string prop;
	public int prop2;
	public GameObject proprtyNumberThree;

	public Node() {}

	public void NodeDraw(SerializedObject me) {
		// node.Update();
		Debug.Log ("Drawing "+me.GetType());
		// if (node.GetType().IsSubclassOf(typeof(PropertyNode)) || node.GetType()==typeof(PropertyNode))
		Rect rect = me.FindProperty("nodeUIRect").rectValue;
		GUI.Box(rect, me.FindProperty("nodeUITitle").stringValue, isSelected ? graph.nodeUISelectedStyle : graph.nodeUIStyle);
		SerializedProperty sp = me.GetIterator();
		sp.Next(true);
		sp.NextVisible(true); // ignore Script
		sp.NextVisible(false); // ignore title
		sp.NextVisible(false); // ignore rect
		sp.NextVisible(false); // ignore input connections
		sp.NextVisible(false); // ignore outputs
		float lineHeight = EditorGUIUtility.singleLineHeight;
		Rect nextPropRect = new Rect(rect.x + 10, rect.y + 5, rect.width / 5, lineHeight);
		while (sp.NextVisible(false)) {
			NodeDrawProperty(nextPropRect, sp);
			nextPropRect.y += lineHeight + 2;
		}
		// rect.height = nextPropRect.y - rect.y;
		me.ApplyModifiedProperties();
	}
	public void NodeDrawProperty(Rect rect, SerializedProperty prop) {
			Debug.Log("showing " + prop.type +" "+ prop.displayName + " at " + rect);
			
	}

	public void ProcessEvents(Event e) {
		switch (e.type) {
			case EventType.MouseDown:
				if (nodeUIRect.Contains(e.mousePosition)) {
					if (e.button == 0) {
						isDragged = true;
						isSelected = true;
						Selection.activeObject = this;
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
		NodeConnector nodeOutput = outputs.Find((no) => { return no.name == name; });
		if (nodeOutput != null) {
			nodeOutput.SetValue(value);
		}
	}

	public void GetConnectionValues() {

	}

	public virtual void Start() {}
	public virtual void OnGUI() {}
}
public abstract class NodeConnector {

	public string name;
	public Node connection;
	public NodeConnector(string name) {
		this.name = name;
	}
	public abstract T GetValue<T>();
	public abstract void SetValue<T>(T t);
}
public class NodeOutput<T> : NodeConnector {
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