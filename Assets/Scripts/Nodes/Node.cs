using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Node : ScriptableObject {

	public string nodeUITitle = "Node";
	public Rect nodeUIRect;
	private bool isDragged;
	protected bool isSelected;
	[NonSerialized]
	public NodeUIGraph graph;
	public List<NodeConnector> inputConnections;
	public List<NodeConnector> outputs;


	protected GUIStyle normalStyle;
	protected GUIStyle selectedStyle;
	protected GUIStyle inputConnectorStyle;
	protected GUIStyle inputConnectorConnectedStyle;
	protected GUIStyle outputConnectorStyle;
	protected GUIStyle outputConnectorConnectedStyle;

	//testing
	public string prop;
	public int prop2;
	public GameObject proprtyNumberThree;

	public Node() {}

	private void Awake() {
		
		// setup style
		normalStyle = new GUIStyle();
		normalStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
		normalStyle.border = new RectOffset(12, 12, 12, 12);

		selectedStyle = new GUIStyle();
		selectedStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 pn.png") as Texture2D;
		selectedStyle.border = new RectOffset(15, 15, 15, 15);
		inputConnectorStyle = new GUIStyle();
		inputConnectorConnectedStyle = new GUIStyle();
		outputConnectorStyle = new GUIStyle();
		outputConnectorConnectedStyle = new GUIStyle();
	}

	public void NodeDraw(SerializedObject me) {
		me.Update();
		// if (node.GetType().IsSubclassOf(typeof(PropertyNode)) || node.GetType()==typeof(PropertyNode))
		Rect rect = me.FindProperty("nodeUIRect").rectValue;
		GUI.Box(rect, me.FindProperty("nodeUITitle").stringValue);//, isSelected ? selectedStyle : normalStyle);
		SerializedProperty sp = me.GetIterator();
		sp.Next(true);
		sp.NextVisible(true); // ignore Script
		sp.NextVisible(false); // ignore title
		sp.NextVisible(false); // ignore rect
		sp.NextVisible(false); // ignore input connections
		sp.NextVisible(false); // ignore outputs
		float lineHeight = EditorGUIUtility.singleLineHeight;
		Rect nextPropRect = new Rect(rect.x + 10, rect.y + 5, rect.width / 5, lineHeight);
		float boxHeight = 10;
		Rect boxRect = new Rect(nextPropRect.x - 10, nextPropRect.y + boxHeight/2, boxHeight, boxHeight);
		while (sp.NextVisible(false)) {
			// NodeDrawProperty(nextPropRect, sp);
			GUI.Box(boxRect, GUIContent.none);//, inputConnectorStyle);
			EditorGUI.LabelField(nextPropRect, sp.displayName);
			// EditorGUI.PropertyField(nextPropRect, sp, GUIContent.none);
			nextPropRect.y += lineHeight + 2;
			boxRect.y+=lineHeight+2;
		}
		rect.height = nextPropRect.y - rect.y;
		me.FindProperty("nodeUIRect").rectValue = rect;
		me.ApplyModifiedProperties();
		// me.Update();
		graph.Save();
	}
	public void NodeDrawProperty(Rect rect, SerializedProperty prop) {
		// Debug.Log("showing " + prop.type +" "+ prop.displayName + " at " + rect);

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
						e.Use();
					} else if (e.button == 1) {
						isSelected = true;
						GUI.changed = true;
						GenericMenu contextMenu = new GenericMenu();
						HandleContextMenu(contextMenu, e);
						e.Use();
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
		NodeConnector nodeOutput = outputs.Find((no) => { return no.propertyName == name; });
		if (nodeOutput != null) {
			nodeOutput.SetValue(value);
		}
	}

	public void GetConnectionValues() {

	}

	public virtual void Start() {}
	public virtual void OnGUI() {}
}

[Serializable]
public class NodeConnector : ScriptableObject {

	public string propertyName;
	public Node connection;
	public NodeConnector(string name) {
		this.propertyName = name;
	}
	public virtual T GetValue<T>() {return default(T);}
	public virtual void SetValue<T>(T t) {}
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
		} catch (Exception) {}
		Debug.LogError("Invalid Get Type '" + typeof(T1).ToString() + "' on node output " + propertyName);
		return default(T1);
	}

	public override void SetValue<T1>(T1 t) {
		try {
			if (typeof(T1) == typeof(T) || typeof(T1).IsSubclassOf(typeof(T))) {
				value = (T) Convert.ChangeType(t, typeof(T));
			}
		} catch (Exception) {}
		Debug.LogError("Invalid Set Type '" + typeof(T1).ToString() + "' of Value '" + t.ToString() + "' on node output " + propertyName);
	}
}