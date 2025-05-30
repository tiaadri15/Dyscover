using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;


public class TranskripsiHasil
{
    public string durasi;
    public string teks;
}

public class SpeechChecker : MonoBehaviour
{
    int panjang, benar, salah;
    // Start is called before the first frame update
    public string teksAsli = "halo nama saya nia saya juara 1 heketon competition device di itk amin"; // Teks yang seharusnya diucapkan

    public string jsonPath = @"E:\Kuliah\Season 6\Hackathon\Dyscover\PythonServer\hasil.json"; // File JSON dari server.py

    [SerializeField]
    public TMP_Text textHasil;
    void Start()
    {
        
    }

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
                    benar++;
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
        textHasil.text = "Total kata: " + panjang + " \n Benar: " + benar + ", Salah: " + salah;
        textHasil.gameObject.SetActive(true);
    }

    void Update()
    {
        string path = Path.Combine(Application.persistentDataPath, jsonPath);

#if UNITY_EDITOR
        // Jika di Editor, coba baca langsung dari folder project
        path = Path.Combine(Directory.GetCurrentDirectory(), jsonPath);
#endif

        if (File.Exists(path))
        {
            string jsonContent = File.ReadAllText(path);
            TranskripsiHasil hasil = JsonUtility.FromJson<TranskripsiHasil>(jsonContent);
             
            Debug.Log("Teks hasil transkripsi: " + hasil.teks);
            Debug.Log("Durasi audio: " + hasil.durasi + " detik");
            textHasil.text = hasil.teks + "durasi = " + hasil.durasi;

            BandingkanKata(teksAsli, hasil.teks);
        }
        else
        {
            Debug.LogError("File hasil.json tidak ditemukan di: " + path);
        }

        //textHasil.text = "Total kata: " + panjang + " \n Benar: " + benar + ", Salah: " + salah;
        //textHasil.gameObject.SetActive(true);
    }
}
