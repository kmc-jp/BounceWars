using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

public class Client : MonoBehaviour
{
    Thread _responseThread;
    public Simulator simulator;
    // Start is called before the first frame update
    void Awake()
    {
        _responseThread = new Thread(ResponseThread);
        _responseThread.Start(); // start the response thread
    }
    private void OnApplicationQuit()
    {
        _responseThread.Abort();
    }
    void ResponseThread()
    {
        while (true)
        {
            WebRequest request = WebRequest.Create("http://localhost:5000");
            // If required by the server, set the credentials.  
            request.Credentials = CredentialCache.DefaultCredentials;

            request.ContentType = "text/json";

            request.Method = "POST";

            PayloadFromClient fromClient = new PayloadFromClient();
            fromClient.commands = simulator.commands;
            

            byte[] binary = Encoding.UTF8.GetBytes(JsonUtility.ToJson(fromClient));
            //manager.SetCommandSent(); server does this
            using (Stream postStream = request.GetRequestStream())
            {
                // Send the data.
                postStream.Write(binary, 0, binary.Length);
                postStream.Close();
            }
            simulator.SetCommandsSent();

            // Get the response.  
            WebResponse response = request.GetResponse();
            // Display the status.  
            Debug.Log(((HttpWebResponse)response).StatusDescription);


            // Get the stream containing content returned by the server. 
            // The using block ensures the stream is automatically closed. 
            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                Debug.Log(responseFromServer);
                PayloadFromHost data = JsonUtility.FromJson<PayloadFromHost>(responseFromServer);
                if (data == null)
                {
                    Debug.Log("null");
                }
                else
                {
                    simulator.units = data.units;
                }
            }

            // Close the response.  
            response.Close();
        }
    }
}
public class PayloadFromClient
{
    public List<Command> commands;
}