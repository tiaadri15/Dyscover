using System.Diagnostics;
using System.IO;
using UnityEngine;

public class PythonServerLauncher : MonoBehaviour
{
    private Process serverProcess;
    private Process predictProcess;

    void Start()
    {
        StartPythonServer("server.py");
        StartPythonServer("predict.py");
    }

    void StartPythonServer(string scriptName)
    {
        string pythonPath = "python"; // Ganti jika pakai path spesifik (contoh: "C:\\Python39\\python.exe")
        string scriptPath = Path.Combine(Application.streamingAssetsPath, scriptName);

        if (!File.Exists(scriptPath))
        {
            UnityEngine.Debug.LogError("Script tidak ditemukan: " + scriptPath);
            return;
        }

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"\"{scriptPath}\"",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        Process process = new Process { StartInfo = startInfo };
        process.OutputDataReceived += (sender, args) => UnityEngine.Debug.Log($"[{scriptName}] {args.Data}");
        process.ErrorDataReceived += (sender, args) => UnityEngine.Debug.LogError($"[{scriptName} ERROR] {args.Data}");

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        if (scriptName.Contains("server"))
            serverProcess = process;
        else if (scriptName.Contains("predict"))
            predictProcess = process;
    }

    void OnApplicationQuit()
    {
        if (serverProcess != null && !serverProcess.HasExited)
            serverProcess.Kill();
        if (predictProcess != null && !predictProcess.HasExited)
            predictProcess.Kill();
    }
}
