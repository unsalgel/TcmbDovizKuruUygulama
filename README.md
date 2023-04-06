
# Döviz Kuru Takip Uygulaması

Bu proje, .NET Core Web API kullanarak **TCMB** üzerinden döviz satış kurlarını çekiyor ve bir aylık döviz kuru durumlarını güncel olarak **.NET Core MVC** projenizde listeleyerek kullanıcılarınıza sunuyor. Ayrıca, döviz kurlarının en yüksek beş güne göre değerlerini hem tablo hem de grafik olarak göstererek kullanıcıların daha net bir şekilde takip etmelerine yardımcı oluyor.

Bu proje, finansal verileri kullanarak bir dizi farklı grafik ve tablo yöntemleri kullanarak verileri görselleştirmeyi öğrenmek isteyen geliştiriciler için de harika bir örnek teşkil ediyor. Ayrıca, **.NET Core**'un esnekliğini kullanarak web API'leri ve **MVC** uygulamalarını birleştirmek ve verileri güncel tutmak gibi farklı uygulama senaryoları da mevcuttur.

## Kurulum
1. Bu repo'yu kopyalayın veya indirin.
2. Visual Studio'yu açın ve **DovizKuruTakip.sln** dosyasını açın.
3. Proje ana dizininde **ExchangeRatesController.cs** dosyasını açın ve url kısmına TCMB API anahtarınızı girin.
   Uygulamayı çalıştırın.

## Kullanım
Uygulama başlatıldığında, bir aylık döviz kuru durumlarını içeren bir tablo gösterilir. Ayrıca, döviz kurlarının en yüksek beş güne göre değerlerini gösteren bir grafik de mevcuttur. Kullanıcılar, tablo veya grafik aracılığıyla döviz kurlarını takip edebilir ve verileri güncel tutmak için sayfayı yenileyebilirler.
