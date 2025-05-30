using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static TCPClientSender;

public class hasilBenar : MonoBehaviour
{
    public GameObject canvasObject; // Drag your Canvas GameObject here in the inspector
    public TMP_Text txtSalah;
    public TMP_Text txtDurasi;
    public TMP_Text txtBenar;
    public TMP_Text txtTotal;
    // Start is called before the first frame update
    void Start()
    {
        HasilTranskripsiLengkap hasil = TranskripsiBridge.Data;
        txtBenar.text = hasil.benar.ToString();
        txtTotal.text = hasil.panjang.ToString();
        txtDurasi.text = hasil.durasi.ToString();
        txtSalah.text = hasil.salah.ToString();
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
