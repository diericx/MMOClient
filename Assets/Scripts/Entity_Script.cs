using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Script : MonoBehaviour {

	public int id = -1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (id != -1) {
			Entity e = Net_tcp.getEntity(id);
			transform.position = e.pos;
		}
	}
}
