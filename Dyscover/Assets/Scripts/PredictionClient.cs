using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class PredictionClient : MonoBehaviour
{
    public string serverIP = "10.160.118.22";  // IP Python server
    public int serverPort = 5005;

    public void SendFeatures(float[] fitur)
    {
        string jsonData = JsonUtility.ToJson(new FeatureData(fitur));
        byte[] dataToSend = Encoding.UTF8.GetBytes(jsonData);

        using (TcpClient client = new TcpClient(serverIP, serverPort))
        using (NetworkStream stream = client.GetStream())
        {
            stream.Write(dataToSend, 0, dataToSend.Length);
            stream.Flush();
            client.Client.Shutdown(SocketShutdown.Send);

            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Debug.Log("Hasil Prediksi: " + response);
        }
    }

    [System.Serializable]
    public class FeatureData
    {
        public float[] fitur;
        public FeatureData(float[] f) { fitur = f; }
    }
}
