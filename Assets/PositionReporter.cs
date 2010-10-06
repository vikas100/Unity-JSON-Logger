using UnityEngine;
using System.Collections;

public class PositionReporter : MonoBehaviour {
	private JsonLogger logger;

	// Use this for initialization
	void Start () {
			getALogger();
	}
	
	// Update is called once per frame
	void Update () {
		getALogger();
		Vector3 screenSpace = Camera.main.WorldToScreenPoint(transform.position);
		if (logger != null)
			logger.reportState(Time.time, screenSpace.x, screenSpace.z, 0, 0, 0 );
	}
	
	private void getALogger()
	{
		if (logger == null)
		{
			logger = (GetComponent("JsonHelper") as JsonHelper).getJsonLogger();
			if (logger == null) Debug.Log("logger is null!!");
		}
	}
}
