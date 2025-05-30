using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void mulaiBelajar()
    {
        SceneManager.LoadScene("MenuBelajarScene");
    }

    public void kembali()
    {
        SceneManager.LoadScene("HomeScene");
    }

    public void belajarHuruf() 
    {
        SceneManager.LoadScene("belajarHurufB");
        
    }

    public void belajarKata()
    {
        SceneManager.LoadScene("belajarKata");
    }

    public void belajarTeks()
    {
        SceneManager.LoadScene("belajarTeks");
    }

    public void deteksi()
    {
        SceneManager.LoadScene("DetectScene");
    }
}
