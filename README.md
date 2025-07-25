## 📌 Tech
- **.NET 8.0**  
- **PostgreSQL**  
- **Docker**


## 📌 Uygulama Akışı

#### **1. Uygulama çalıştırıldığında kullanıcıya şu seçenekler sunulur:**
- **`Y`** → Dosya yükleme işlemini başlatır.  
- **`Q`** → Uygulamadan çıkar.  

#### **2. Dosya Yükleme (Upload):**
- Kullanıcı `Y` tuşuna bastığında**, ekrandan yüklenecek dosyaların **tam path'leri** virgülle ayrılmış şekilde girilir ve `FileUploadAsync` metodu çalıştırılır.  
  **Örnek:**
  -/app/chunksroot/files/case1.pdf,/app/chunksroot/files/case2.pdf

#### **3. Dosya İndirme ve Doğrulama (Download & Verify):**
- Yükleme tamamlandıktan sonra FileDownloadAsync metodu otomatik olarak çalışır.
- Dosya indirildikten sonra orijinal dosya ile bütünlük doğrulaması (checksum) yapılır. 

#### **4. Tekrar dosya yükleme:**
- **  İşlem tamamlandıktan sonra uygulama tekrar başa döner.
	**	Kullanıcı yeni dosya yükleyebilir veya Q tuşuna basarak uygulamadan çıkabilir.


## Projeyi Çalıştırma
  
#### 1. Veritabanı servisini arka planda başlat:

- docker compose up -d db

#### 2. Console uygulamasını interaktif modda çalıştır: 

- docker compose run --rm --service-ports -it --build app

## Test işlemleri

#### 1. Uygulamanın çalıştırılması:

<img width="2420" height="1166" alt="image" src="https://github.com/user-attachments/assets/d29f4878-935e-4d3e-97d2-19fd820fdeba" />

#### 2. Dosya yükleme işlemi:

