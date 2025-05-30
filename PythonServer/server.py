import socket
import json
import wave
import speech_recognition as sr
# from vosk import Model, KaldiRecognizer   # Uncomment untuk Vosk offline

HOST = '0.0.0.0'
PORT = 5005

# Jika pakai Vosk offline, load model:
# model = Model("model-id")

def proses_audio(file_path):
    # hitung durasi
    with wave.open(file_path, 'rb') as wf:
        frames = wf.getnframes()
        rate = wf.getframerate()
        duration = round(frames / float(rate), 2)

    # STT online (Google Web API)
    r = sr.Recognizer()
    with sr.AudioFile(file_path) as source:
        audio = r.record(source)
    try:
        teks = r.recognize_google(audio, language="id-ID").lower()
    except sr.UnknownValueError:
        teks = "(tidak dikenali)"
    except sr.RequestError as e:
        teks = f"Koneksi gagal: {e}"

    return {"durasi": str(duration), "teks": teks}


def start_server():
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.bind((HOST, PORT))
        s.listen(5)
        print(f"Menunggu koneksi di {HOST}:{PORT}...")
        conn, addr = s.accept()
        print(f"Koneksi dari: {addr}")

        with conn:
            # terima file WAV
            file_path = "received.wav"
            with open(file_path, 'wb') as f:
                while True:
                    data = conn.recv(4096)
                    if not data:
                        break
                    f.write(data)
            print("Audio diterima.") 
            
            # proses audio dan hasilkan JSON
            hasil = proses_audio(file_path)

            # simpan hasil ke file hasil.json
            with open("hasil.json", "w", encoding="utf-8") as json_file:
                json.dump(hasil, json_file, ensure_ascii=False, indent=4)

            payload = json.dumps(hasil).encode('utf-8')
            conn.sendall(payload)
            print("Hasil dikirim:", hasil)

if __name__ == "__main__":
    start_server()