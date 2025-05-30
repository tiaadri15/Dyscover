using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class HasilTranskripsiLengkap
{
    public string teks;
    public float durasi;
    public int benar;
    public int salah;
    public int panjang;
    public int akurasi;
    public int kecepatan;
}

public class TCPClientSender : MonoBehaviour
{
    public string serverIP = "10.160.118.22";
    public int serverPort = 5005;

    //[SerializeField]
    //public TMP_Text textHasil;

    public PredictDyslexia tcpPredict;

    public string teksAsli;

    public Action<TranskripsiHasil> OnTranscriptionReceived { get; internal set; }

    int panjang, benar, salah, akurasi, kecepatan;
    string tes;

    void BandingkanKata(string expected, string actual)
    {
        tes = "";  // reset sebelumnya
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
                {
                    salah++;
                }
            }
            else
            {
                salah++;
            }
        }

        akurasi = (int)(((float)benar / panjang) * 100f);

        Debug.Log($"Total kata: {panjang}");
        Debug.Log($"Benar: {benar}, Salah: {salah}");
        //textHasil.text = $"Total kata: {panjang}\nBenar: {benar}, Salah: {salah}\n{tes}";
        //textHasil.gameObject.SetActive(true);
    }

    public static class TranskripsiBridge
    {
        public static HasilTranskripsiLengkap Data;
    }

    private void Start()
    {
        string jsonPath = Path.Combine(Application.persistentDataPath, "hasil_transkripsi.json");

        if (File.Exists(jsonPath))
        {
            Debug.Log("File hasil_transkripsi.json ditemukan. Menghapus...");
            File.Delete(jsonPath);
        }
    }
    public void SendWavFile(string path)
    {
        byte[] fileData = File.ReadAllBytes(path);

        try
        {
            using (TcpClient client = new TcpClient())
            {
                Debug.Log($"Menghubungkan ke {serverIP}:{serverPort}...");
                client.Connect(serverIP, serverPort);

                using (NetworkStream stream = client.GetStream())
                {
                    stream.Write(fileData, 0, fileData.Length);
                    stream.Flush();
                    client.Client.Shutdown(SocketShutdown.Send);

                    MemoryStream ms = new MemoryStream();
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        ms.Write(buffer, 0, bytesRead);

                    string json = Encoding.UTF8.GetString(ms.ToArray());
                    Debug.Log("Hasil dari Python: " + json);
                    TranskripsiHasil hasil = JsonUtility.FromJson<TranskripsiHasil>(json);

                    BandingkanKata(teksAsli, hasil.teks);

                    if (int.TryParse(hasil.durasi, out int durasiDetik) && durasiDetik > 0)
                        kecepatan = panjang / durasiDetik;
                    else
                        kecepatan = 0;

                    // Buat data lengkap
                    HasilTranskripsiLengkap hasilLengkap = new HasilTranskripsiLengkap
                    {
                        teks = hasil.teks,
                        durasi = float.Parse(hasil.durasi),
                        benar = benar,
                        salah = salah,
                        panjang = panjang,
                        akurasi = akurasi,
                        kecepatan = kecepatan
                    };

                    TranskripsiBridge.Data = hasilLengkap;
                    // Serialisasi dan simpan ke file
                    string finalJson = JsonUtility.ToJson(hasilLengkap, true);
                    string localJsonPath = Path.Combine(Application.persistentDataPath, "hasil_transkripsi.json");
                    File.WriteAllText(localJsonPath, finalJson);

                    Debug.Log("JSON disimpan ke: " + localJsonPath);
                    tcpPredict.enabled = true;

                    //tcpPredict.enabled = false;

                    SceneManager.LoadScene("hasilBenar");
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
