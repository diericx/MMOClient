using UnityEngine;
using System.Collections;

public class spawner : MonoBehaviour {

	float RADIUS = 5f;
	float SPAWN_RATE = 0.5f;
	
	public GameObject enemyPrefab;
	
	bool isAlive = true;

	// Use this for initialization
	void Start () {
		StartCoroutine(spawnEnemy());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	IEnumerator spawnEnemy() {
		while (isAlive) {
			float angle = Random.Range(1, 360);
			float rad = angle * Mathf.Deg2Rad;
			
			float x = RADIUS * Mathf.Cos(rad);
			float z = RADIUS * Mathf.Sin(rad);
			
			GameObject newEnemy = (GameObject)Instantiate(enemyPrefab, new Vector3(x + transform.position.x, transform.position.y, z + transform.position.z), Quaternion.identity);
			
			yield return new WaitForSeconds(SPAWN_RATE);
		}

	}
}
