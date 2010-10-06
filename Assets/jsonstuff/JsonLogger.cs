using UnityEngine;
using System.Collections;

using System.Net;
using System.IO;
using System;
using System.Threading;
using System.Timers;

using System.Text;


public class JsonLogger {
	
	string url;
	
	private string session_id;
	
	private ArrayList states = new ArrayList();
	
	private bool useDotNetHTTP;
			
	public JsonLogger(string url, bool useDotNetHTTP)
	{
		this.url = url;
		this.useDotNetHTTP = useDotNetHTTP;
		
		if (startSession())
		{
			startEventReportingTimer();
		}
	}
	
	private void startEventReportingTimer()
	{
		System.Timers.Timer reportingTimer = new System.Timers.Timer();
		reportingTimer.Interval = 1000;
		reportingTimer.Elapsed += new ElapsedEventHandler(this.deliverOutstandingEvents);
		reportingTimer.Start();
	}

	private bool startSession()
	{
		Hashtable requestHash = new Hashtable();
		requestHash["hey"]="dude";
		string requestString = MiniJSON.JsonEncode(requestHash);
		//Debug.Log("encoded request as: "+requestString);
		string response = MakeHTTPRequestAsJSON(url+"sessions.json", requestString, "POST", useDotNetHTTP);
		//Debug.Log("got response: "+response);
		Hashtable session_data = (Hashtable) MiniJSON.JsonDecode(response);
		this.session_id = (string) session_data["id"];
		Debug.Log("Registered session with id: "+session_id);
		return true;
	}
	
	public void reportState(float time, float x, float y, float dx, float dy, float tilt)
	{
		Hashtable requestHash = new Hashtable();
		requestHash["time"]=time;
		requestHash["x"]=x;
		requestHash["y"]=y;
		requestHash["dx"]=dx;
		requestHash["dy"]=dy;
		requestHash["tilt"]=tilt;
		lock(states)
		{
			states.Add(requestHash);
		}
		return ;
	}
	
	private void deliverOutstandingEvents( object source, ElapsedEventArgs e )
	{
		deliverOutstandingEvents();
	}
	private void deliverOutstandingEvents()
	{
		ArrayList events = popAllEvents();
		if (events.Count > 0)
		{
			Debug.Log(events.Count + " events to deliver.");
			Hashtable wrapper = new Hashtable();
			wrapper["events"] = events;
			makeJSONRequest("sessions/"+session_id+"/bulk_events.json", wrapper, "POST");
		}else
		{
			Debug.Log("No events to deliver");
		}
	}
	
	private ArrayList popAllEvents()
	{
		ArrayList ret;
		lock(states)
		{
			ret = (ArrayList) this.states.Clone();
			this.states.Clear();
		}
		return ret;
	}
	
	private object makeJSONRequest(string url, object o, string method)
	{
		string http_ret;
		string encoded = MiniJSON.JsonEncode(o);
		
		Debug.Log("makeJSONRequest called with data: "+encoded);
		
		http_ret = MakeHTTPRequestAsJSON(this.url + url, encoded, method, useDotNetHTTP);
		Debug.Log("JSONRequest returned: "+http_ret);
		return DecodeJSON(http_ret);
	}

	private static object DecodeJSON(string json)
	{
		return  MiniJSON.JsonDecode(json);
	}
	
	private static string MakeHTTPRequestAsJSON(string url, string requestString, string method, bool useDotNetHTTP)
	{
		string ret;
		
		Debug.Log("\t\tMakeHTTPRequestAsJSON called with request: "+ requestString);
		
		if (useDotNetHTTP){
			Debug.Log("\t\t\tMaking Direct HTTP Request with body: "+requestString);
			ret = MakeDirectHTTPRequestAsJSON(url, requestString, method);
		}else{
			Debug.Log("HTTP requests not implemented on this platform.");
			ret = "ERROR";
		}
				
		Debug.Log("!!!! http request returned: "+ ret);
		return ret;
	}
		
	private static string MakeDirectHTTPRequestAsJSON(string url, string requestString, string method)
	{
		HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(url);
		request.ContentType = "application/json; charset=utf-8";
		request.Accept = "application/json, text/javascript, */*";
		request.Method = method;
		request.Timeout = 2000;
		StreamWriter writer = new StreamWriter(request.GetRequestStream());
		writer.Write(requestString);
		writer.Close();
		
		HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
        Stream dataStream = response.GetResponseStream ();
        StreamReader reader = new StreamReader (dataStream);
        string responseFromServer = reader.ReadToEnd();
		response.Close();
		return responseFromServer;
	}
}

