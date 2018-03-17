using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINode : Node {
	
	// inputs
	// positioning
	Rect rect;
	// anchoring
	// style
	GUIStyle style;
	
	// outputs
	public override void Start() {
		AddInput<Rect>("rect");
		AddInput<GUIStyle>("style");
	}

	public override void OnGUI() {
		GUI.Box(GetInputFor<Rect>("rect"), "", GetInputFor<GUIStyle>("style"));
	}
}
