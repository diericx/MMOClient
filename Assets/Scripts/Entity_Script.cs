using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Script : MonoBehaviour {

	public int id = -1;
	public bool isClient = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (id != -1) {
			Entity e = Net_tcp.getEntity(id, isClient);
			transform.position = e.pos;

			// deal with rotation
			Transform fpsChar = transform.GetChild(0);
			Vector3 newRot = new Vector3(fpsChar.rotation.x, transform.rotation.y, 0);
			if (newRot != e.rot) {
				e.rot = newRot;
				e.rotChanged = true;
			}
			Net_tcp.updateEntity(id, e);
			
		}
	}
}
