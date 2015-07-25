using UnityEngine;
using System.Collections;

public class Object_script : MonoBehaviour {

    private float targetX = 0;
    private float targetY = 0;
    public bool shouldRotate = false;
    private float rotateSpeed;
    private Vector3 rot;

    public void setTargetPos(float x, float y)
    {
        targetX = x;
        targetY = y;
    }

	// Use this for initialization
	void Start () {
        rotateSpeed = Random.Range(0.09f, 0.49f);

        int rotX = Client.r.Next(0, 26);
        int rotY = Client.r.Next(0, 26);
        int rotZ = Client.r.Next(0, 26);

        rot = new Vector3(rotX, rotY, rotZ);
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

        if (shouldRotate)
        {
            transform.Rotate(rot.x * Time.deltaTime, rot.y * Time.deltaTime, rot.z * Time.deltaTime);
        }
	}
}
