using UnityEngine;
using System.Collections;

public class Circler : MonoBehaviour {
	float dx=1;
	float dy=1;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = transform.position;
		

		
		if (pos.x<=0)
			dx = 1;
		if (pos.x>=40)
			dx = -1;
		
		if (pos.z <= 0)
			dy = 1;
		if (pos.z >= 40)
			dy = -1;
					
		pos.x+=dx;
		pos.z+=dy;
		
		transform.position=pos;
	}	
}
