import socket
import json
import joblib

HOST = '0.0.0.0'
PORT = 5006

# Load model numerik
model = joblib.load("model_disleksia.pkl")

def hitung_kata_benar(teks_hasil, teks_asli):
    kata_hasil = teks_hasil.split()
    kata_asli = teks_asli.split()
    benar = 0
    salah = 0
    panjang = max(len(kata_asli), len(kata_hasil))
    for i in range(panjang):
        if i < len(kata_asli) and i < len(kata_hasil):
            if kata_hasil[i] == kata_asli[i]:
                benar += 1
            else:
                salah += 1
        else:
            # Kata ekstra di salah satu list dihitung salah
            salah += 1
    return benar, salah


def ekstrak_fitur(durasi, kata_benar, kata_salah):
    total_kata = kata_benar + kata_salah
    akurasi = (kata_benar / total_kata * 100) if total_kata > 0 else 0
    kecepatan_baca = (total_kata / durasi) if durasi > 0 else 0
    return [durasi, kata_benar, kata_salah, total_kata, akurasi, kecepatan_baca]

def start_server():
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.bind((HOST, PORT))
        s.listen(5)
        print(f"Menunggu koneksi di {HOST}:{PORT}...")
        
        while True:
            conn, addr = s.accept()
            print(f"Koneksi dari: {addr}")

            with conn:
                file_path = "hasil_transkripsi.json"
                with open(file_path, 'wb') as f:
                    while True:
                        data = conn.recv(4096)
                        if not data:
                            break
                        f.write(data)
                print("File JSON diterima.")

                # Baca dan proses JSON
                with open(file_path, 'r', encoding='utf-8') as f:
                    json_data = json.load(f)

                teks = json_data.get("teks", "")
                durasi = json_data.get("durasi", 0)
                teks_asli = json_data.get("teks_asli", "")

                kata_benar, kata_salah = hitung_kata_benar(teks, teks_asli)
                fitur = ekstrak_fitur(durasi, kata_benar, kata_salah)
                prediksi = model.predict([fitur])[0]
                status = "disleksia" if prediksi == 1 else "non-disleksia"

                hasil = {
                    "durasi": durasi,
                    "teks": teks,
                    "kata_benar": kata_benar,
                    "kata_salah": kata_salah,
                    "prediksi": status
                }

                print("Hasil:", hasil)
                conn.sendall(json.dumps(hasil).encode('utf-8'))

if __name__ == "__main__":
    start_server()
