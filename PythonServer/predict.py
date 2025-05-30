import socket
import json
import wave
import speech_recognition as sr
import joblib
import os

HOST = '0.0.0.0'
PORT = 5005

# Load model numerik (tanpa vectorizer)
model = joblib.load("model_disleksia.pkl")  # Model kamu harus pakai fitur numerik, bukan text vector

def proses_audio(file_path):
    # Hitung durasi audio
    with wave.open(file_path, 'rb') as wf:
        frames = wf.getnframes()
        rate = wf.getframerate()
        duration = round(frames / float(rate), 2)

    # Konversi audio ke teks
    r = sr.Recognizer()
    with sr.AudioFile(file_path) as source:
        audio = r.record(source)
    try:
        teks = r.recognize_google(audio, language="id-ID").lower()
    except sr.UnknownValueError:
        teks = "(tidak dikenali)"
    except sr.RequestError as e:
        teks = f"Koneksi gagal: {e}"

    return teks, duration

def ekstrak_fitur(teks, durasi):
    # Fitur dummy berdasarkan teks: panjang teks, jumlah kata, dll.
    panjang_teks = len(teks)
    jumlah_kata = len(teks.split())
    jumlah_huruf_vokal = sum([1 for c in teks if c in 'aiueo'])

    return [durasi, panjang_teks, jumlah_kata, jumlah_huruf_vokal]

def start_server():
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.bind((HOST, PORT))
        s.listen(1)
        print(f"Menunggu koneksi di {HOST}:{PORT}...")
        conn, addr = s.accept()
        print(f"Koneksi dari: {addr}")

        with conn:
            file_path = "received.wav"
            with open(file_path, 'wb') as f:
                while True:
                    data = conn.recv(4096)
                    if not data:
                        break
                    f.write(data)
            print("Audio diterima.")

            teks, durasi = proses_audio(file_path)
            fitur = ekstrak_fitur(teks, durasi)
            prediksi = model.predict([fitur])[0]
            status = "disleksia" if prediksi == 1 else "non-disleksia"

            hasil = {
                "durasi": durasi,
                "teks": teks,
                "prediksi": status
            }

            print("Hasil:", hasil)
            conn.sendall(json.dumps(hasil).encode('utf-8'))

if __name__ == "__main__":
    start_server()
