using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;

public class PredictDyslexia : MonoBehaviour
{
    public string serverIP = "10.160.118.22";  // Atur sesuai IP server
    public int serverPort = 5006;

    public Action<TranskripsiHasil> OnTranscriptionReceived { get; internal set; }


    public void SendJsonFile(string jsonFilePath)
    {
        try
        {
            // Baca isi file JSON
            string jsonToSend = File.ReadAllText(jsonFilePath, Encoding.UTF8);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonToSend);

            using (TcpClient client = new TcpClient())
            {
                Debug.Log($"Menghubungkan ke {serverIP}:{serverPort}...");
                client.Connect(serverIP, serverPort);  // Connect ke server

                using (NetworkStream stream = client.GetStream())
                {
                    // Kirim data JSON ke server
                    stream.Write(jsonBytes, 0, jsonBytes.Length);
                    stream.Flush();
                    client.Client.Shutdown(SocketShutdown.Send);  // Tanda kirim selesai

                    // Baca balasan JSON dari server
                    MemoryStream ms = new MemoryStream();
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        ms.Write(buffer, 0, bytesRead);

                    string receivedJson = Encoding.UTF8.GetString(ms.ToArray());
                    Debug.Log("Hasil dari Python: " + receivedJson);

                }
            }
        }
        catch (SocketException se)
        {
            Debug.LogError("Gagal koneksi ke server: " + se.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("Terjadi error: " + ex.Message);
        }
    }

    void Start()
    {
        string localJsonPath = Path.Combine(Application.persistentDataPath, "hasil_transkripsi.json");

        if (File.Exists(localJsonPath))
        {
            Debug.Log("Mengirim file JSON ke server: " + localJsonPath);
            SendJsonFile(localJsonPath);
        }
        else
        {
            Debug.LogWarning("File JSON tidak ditemukan: " + localJsonPath);
        }
    }


}