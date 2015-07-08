using UnityEngine;
using System.Collections;

public class Object_script : MonoBehaviour {

    private float targetX = 0;
    private float targetY = 0;

    public void setTargetPos(float x, float y)
    {
        targetX = x;
        targetY = y;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.x != targetX)
        {
			float newX = (transform.position.x * 3 + targetX) / 4;
			transform.position = new Vector3(newX, transform.position.y, 0);
			
			//transform.position = new Vector3(targetX, transform.position.y, 0);
        }

		if (transform.position.y != targetY)
        {
			float newY = (transform.position.y * 3 + targetY) / 4;
			transform.position = new Vector3(transform.position.x, newY, 0);
			
			//transform.position = new Vector3(transform.position.x, targetY, 0);
        }
	}
}
