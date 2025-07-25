## Projeyi Çalıştırma

1. Veritabanı servisini arka planda başlat:

docker compose up -d db

2. Console uygulamasını interaktif modda çalıştır:

docker compose run --rm --service-ports -it --build app