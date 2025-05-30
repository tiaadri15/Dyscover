using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;

public class TCPClientSender : MonoBehaviour
{
    public string serverIP = "10.160.118.22";  // Atur sesuai IP server
    public int serverPort = 5005;

    [SerializeField]
    public TMP_Text textHasil;

    public string teksAsli; // Teks yang seharusnya diucapkan

    public Action<TranskripsiHasil> OnTranscriptionReceived { get; internal set; }

    int panjang, benar, salah;
    string tes;

    void BandingkanKata(string expected, string actual)
    {
        string[] kataAsli = expected.ToLower().Split(' ');
        string[] kataHasil = actual.ToLower().Split(' ');

        benar = 0;
        salah = 0;
        panjang = Mathf.Max(kataAsli.Length, kataHasil.Length);
        for (int i = 0; i < panjang; i++)
        {
            if (i < kataAsli.Length && i < kataHasil.Length)
            {
                if (kataAsli[i] == kataHasil[i])
                {
                    tes += "-" + kataAsli[i];
                    benar++;
                }
                else 
                    salah++;
            }
            else
            {
                salah++;
            }
        }

        Debug.Log($"Total kata: {panjang}");
        Debug.Log($"Benar: {benar}, Salah: {salah}");
        textHasil.text = "Total kata: " + panjang + " \n Benar: " + benar + ", Salah: " + salah + "\n" + tes;
        textHasil.gameObject.SetActive(true);
    }

    public void SendWavFile(string path)
    {
        byte[] fileData = File.ReadAllBytes(path);

        try
        {
            using (TcpClient client = new TcpClient())
            {
                Debug.Log($"Menghubungkan ke {serverIP}:{serverPort}...");
                client.Connect(serverIP, serverPort);  // Connect ke server

                using (NetworkStream stream = client.GetStream())
                {
                    // Kirim WAV
                    stream.Write(fileData, 0, fileData.Length);
                    stream.Flush();
                    client.Client.Shutdown(SocketShutdown.Send);

                    // Terima JSON
                    MemoryStream ms = new MemoryStream();
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        ms.Write(buffer, 0, bytesRead);

                    string json = Encoding.UTF8.GetString(ms.ToArray());
                    Debug.Log("Hasil dari Python: " + json);
                    TranskripsiHasil hasil = JsonUtility.FromJson<TranskripsiHasil>(json);
                    //textHasil.text = json;
                    BandingkanKata(teksAsli, hasil.teks);

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
}