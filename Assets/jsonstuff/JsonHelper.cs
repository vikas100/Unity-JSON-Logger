using UnityEngine;
using System.Collections;

public class JsonHelper:MonoBehaviour {
	private JsonLogger logger;
	public string serverURL;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this);
		getJsonLogger(); 
	}

	// Update is called once per frame
	void Update () {

	}

	public JsonLogger getJsonLogger()
	{
		if (this.logger == null)
		{
			this.logger = new JsonLogger(serverURL, UseDotNetHTTP());
		}
		return this.logger;
	}
	
	private static bool UseDotNetHTTP()
	{
		return !(Application.platform == RuntimePlatform.IPhonePlayer);
	}

}
