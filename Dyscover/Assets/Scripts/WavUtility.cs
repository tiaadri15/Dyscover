using System;
using System.IO;
using UnityEngine;

public static class WavUtility
{
    public static byte[] FromAudioClip(AudioClip clip)
    {
        MemoryStream stream = new MemoryStream();
        const int headerSize = 44; 

        // Get audio data
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        byte[] bytesData = ConvertTo16Bit(samples);

        // RIFF header
        stream.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"), 0, 4);
        stream.Write(BitConverter.GetBytes(headerSize + bytesData.Length - 8), 0, 4);
        stream.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"), 0, 4);

        // fmt subchunk
        stream.Write(System.Text.Encoding.UTF8.GetBytes("fmt "), 0, 4);
        stream.Write(BitConverter.GetBytes(16), 0, 4); // Subchunk1Size
        stream.Write(BitConverter.GetBytes((ushort)1), 0, 2); // AudioFormat (1 = PCM)
        stream.Write(BitConverter.GetBytes((ushort)clip.channels), 0, 2);
        stream.Write(BitConverter.GetBytes(clip.frequency), 0, 4);
        int byteRate = clip.frequency * clip.channels * 2;
        stream.Write(BitConverter.GetBytes(byteRate), 0, 4);
        ushort blockAlign = (ushort)(clip.channels * 2);
        stream.Write(BitConverter.GetBytes(blockAlign), 0, 2);
        ushort bitsPerSample = 16;
        stream.Write(BitConverter.GetBytes(bitsPerSample), 0, 2);

        // data subchunk
        stream.Write(System.Text.Encoding.UTF8.GetBytes("data"), 0, 4);
        stream.Write(BitConverter.GetBytes(bytesData.Length), 0, 4);
        stream.Write(bytesData, 0, bytesData.Length);

        return stream.ToArray();
    }

    public static void Save(string filePath, AudioClip clip)
    {
        var wavData = FromAudioClip(clip);
        File.WriteAllBytes(filePath, wavData);
        Debug.Log("File WAV disimpan ke: " + filePath);
    }

    private static byte[] ConvertTo16Bit(float[] samples)
    {
        byte[] bytes = new byte[samples.Length * 2];
        int i = 0;
        foreach (var sample in samples)
        {
            short value = (short)(sample * short.MaxValue);
            byte[] byteArr = BitConverter.GetBytes(value);
            bytes[i++] = byteArr[0];
            bytes[i++] = byteArr[1];
        }
        return bytes;
    }
}