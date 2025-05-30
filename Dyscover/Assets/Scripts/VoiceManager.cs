using UnityEngine;
using System.IO;

public class VoiceManager : MonoBehaviour
{
    public TCPClientSender tcpClient;
    private AudioClip clip;
    private string wavPath;
    private int sampleRate = 16000;
    private int maxLengthSeconds = 300;

    void Start()
    {
        wavPath = Path.Combine(Application.persistentDataPath, "temp.wav");
    }

    public void StartRecording()
    {
        clip = Microphone.Start(null, false, maxLengthSeconds, 16000);
        Debug.Log("Rekaman dimulai...");
    }

    public void StopRecording()
    {
        int pos = Microphone.GetPosition(null);
        Microphone.End(null);
        Debug.Log("Rekaman dihentikan.");
        // Create a new clip with the exact length of the recording
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        float[] recordedSamples = new float[pos * clip.channels];
        System.Array.Copy(samples, recordedSamples, recordedSamples.Length);

        AudioClip trimmedClip = AudioClip.Create("clip", pos, clip.channels, sampleRate, false);
        trimmedClip.SetData(recordedSamples, 0);

        // Use trimmedClip as your actual recorded audio clip
        Debug.Log($"Recording stopped. Length in seconds: {(float)pos / sampleRate}");
        WavUtility.Save(wavPath, trimmedClip);
        tcpClient.SendWavFile(wavPath);
    }
}