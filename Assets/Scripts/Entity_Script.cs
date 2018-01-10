using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Script : MonoBehaviour {

	public GameObject sprite;
	public int id = -1;
	public bool isClient = false;
	// Private variables

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// Get current mouse angle

		if (id != -1) {
			Entity e = Net_tcp.getEntity(id, isClient);
			transform.position = e.pos;

			// deal with rotation
			int currentMouseAngle = GetMouseAngle();
			if (e.mouseAngle != currentMouseAngle) {
				e.mouseAngle = currentMouseAngle;
				e.mouseAngleChanged = true;
			}
			Net_tcp.updateEntity(id, e);
			
		}
	}

	int GetMouseAngle() {
		Vector3 v3Pos = Camera.main.WorldToScreenPoint(sprite.transform.position);
		v3Pos = Input.mousePosition - v3Pos;
		float fAngle = Mathf.Atan2 (v3Pos.y, v3Pos.x)* Mathf.Rad2Deg;
		if (fAngle < 0.0f) fAngle += 360.0f;
		return (int)fAngle;
	}
}
