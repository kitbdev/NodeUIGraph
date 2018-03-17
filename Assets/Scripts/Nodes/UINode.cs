using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINode : Node {
	
	// inputs
	Rect rect;
	// todo anchoring
	GUIStyle style;
	
	// no outputs?

	public override void Start() {

	}

	public override void OnGUI() {
		GUI.Box(rect, "", style);
	}
}
