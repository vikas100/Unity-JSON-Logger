using System.Collections;
using System.Net;
using System.IO;

public class HTTPUtil {
	public static string MakeHTTPRequestAsJSON(string url, string requestString, string method)
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
