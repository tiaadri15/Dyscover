using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PredictDyslexia;
using static TCPClientSender;

public class hasilPrediksi : MonoBehaviour
{
    public GameObject canvasObject; // Drag your Canvas GameObject here in the inspector
    public TMP_Text txtBenar;
    public TMP_Text txtSalah;
    public TMP_Text txtDurasi;
    public TMP_Text txtAkurasi;
    public TMP_Text txtKecepatan;
    public TMP_Text txtPredikat;
    // Start is called before the first frame update
    void Start()
    {
        HasilTranskripsiLengkap hasil = TranskripsiBridge.Data;
        //HasilLengkap prediksi = PrediksiBridge.Data;
        //txtBenar.text = prediksi.benar.ToString();
        //txtSalah.text = prediksi.salah.ToString();
        //txtDurasi.text = prediksi.durasi.ToString();
        //txtAkurasi.text = hasil.akurasi.ToString();
        //txtKecepatan.text = hasil.kecepatan.ToString();
        //txtPredikat.text = prediksi.prediksi;
    }

    public void kembali()
    {
        SceneManager.LoadScene("HomeScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
