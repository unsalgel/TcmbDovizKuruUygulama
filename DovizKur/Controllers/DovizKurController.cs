using ClosedXML.Excel;
using DovizKur.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace DovizKur.Controllers
{
    public class DovizKurController : Controller
    {
        private readonly HttpClient _httpClient;
        public DovizKurController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        //Apiden gelen veriyi düzenlediğim method
        public async Task<List<ExchangeRates>> GetExchangeRatesAsync()
        {
            var exchangeRates = new List<ExchangeRates>();
            var client = new HttpClient();
            var response = await client.GetAsync("https://localhost:44317/api/ExchangeRates");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var xmlDoc = XDocument.Parse(content);
                foreach (var item in xmlDoc.Descendants("items"))
                {
                    //hafta sonu veriler 0 geliyor sadece usd verisini kontrol ettirdim 0 sa hafta sonudur hafta sonu hariç listelemesi için 
                    if (item.Element("usd").Value != "0")
                    {
                        //replace ekranda döviz kurlarının virgüllü biçimde görünmesi için yazılmıştır.
                        var exchangeRate = new ExchangeRates
                        {
                            date = DateTime.Parse(item.Element("date").Value),
                            usd = decimal.Parse(item.Element("usd").Value.Replace(".", ",")),
                            eur = decimal.Parse(item.Element("eur").Value.Replace(".", ",")),
                            chf = decimal.Parse(item.Element("che").Value.Replace(".", ",")),
                            gbp = decimal.Parse(item.Element("gbp").Value.Replace(".", ",")),
                            jpy = decimal.Parse(item.Element("jpy").Value.Replace(".", ","))

                        };
                        exchangeRates.Add(exchangeRate);
                    }
                }
            }
            return exchangeRates;
        }
        //bütün döviz kurlarının listesi
        public async Task<IActionResult> Index()
        {
            var exchangeRates = await GetExchangeRatesAsync();

            return View(exchangeRates);
        }
        public async Task<IActionResult> DovizKurEnYuksek()
        {
            var items = await GetExchangeRatesAsync();
            // Döviz kurlarının  listesi
            var currencyRates = new Dictionary<string, List<(DateTime date, decimal? rate)>>();
            currencyRates.Add("USD", items.OrderByDescending(x => x.usd).Take(5)
                                          .Select(x => (x.date, x.usd)).ToList());
            currencyRates.Add("EUR", items.OrderByDescending(x => x.eur).Take(5)
                                          .Select(x => (x.date, x.eur)).ToList());
            currencyRates.Add("CHF", items.OrderByDescending(x => x.chf).Take(5)
                                          .Select(x => (x.date, x.chf)).ToList());
            currencyRates.Add("GBP", items.OrderByDescending(x => x.gbp).Take(5)
                                          .Select(x => (x.date, x.gbp)).ToList());
            currencyRates.Add("JPY", items.OrderByDescending(x => x.jpy).Take(5)
                                          .Select(x => (x.date, x.jpy)).ToList());
            return View(currencyRates);
        }
        //Döviz kurlarını  Grafik ile göstermek için yazdığım  method
        public async Task<IActionResult> DovizKurGrafik()
        {
            var items = await GetExchangeRatesAsync();
            // Döviz kurlarının en yüksek 5  güne ait değerlerini çektim
            var usdRates = items.OrderByDescending(x => x.usd).Take(5).Select(x => new { Date = x.date.ToShortDateString(), Rate = x.usd }).ToList();
            var eurRates = items.OrderByDescending(x => x.eur).Take(5).Select(x => new { Date = x.date.ToShortDateString(), Rate = x.eur }).ToList();
            var chfRates = items.OrderByDescending(x => x.chf).Take(5).Select(x => new { Date = x.date.ToShortDateString(), Rate = x.chf }).ToList();
            var gbpRates = items.OrderByDescending(x => x.gbp).Take(5).Select(x => new { Date = x.date.ToShortDateString(), Rate = x.gbp }).ToList();
            var jpyRates = items.OrderByDescending(x => x.jpy).Take(5).Select(x => new { Date = x.date.ToShortDateString(), Rate = x.jpy }).ToList();
            // Verileri JSON formatına çevirdim
            var jsonUsdRates = JsonConvert.SerializeObject(usdRates);
            var jsonEurRates = JsonConvert.SerializeObject(eurRates);
            var jsonChfRates = JsonConvert.SerializeObject(chfRates);
            var jsonGbpRates = JsonConvert.SerializeObject(gbpRates);
            var jsonJpyRates = JsonConvert.SerializeObject(jpyRates);
            // ViewData'ya verileri ekledim
            ViewData["UsdRates"] = jsonUsdRates;
            ViewData["EurRates"] = jsonEurRates;
            ViewData["ChfRates"] = jsonChfRates;
            ViewData["GbpRates"] = jsonGbpRates;
            ViewData["JpyRates"] = jsonJpyRates;
            return View();
        }
        //Excel olarak indirmek için
        public async Task<IActionResult> DovizKurExcel()
        {
            // Döviz kur verileri
            var items = await GetExchangeRatesAsync();
            var usdRates = items.OrderByDescending(x => x.usd).Take(5).Select(x => new { Date = x.date.ToShortDateString(), Rate = x.usd }).ToList();
            var eurRates = items.OrderByDescending(x => x.eur).Take(5).Select(x => new { Date = x.date.ToShortDateString(), Rate = x.eur }).ToList();
            var chfRates = items.OrderByDescending(x => x.chf).Take(5).Select(x => new { Date = x.date.ToShortDateString(), Rate = x.chf }).ToList();
            var gbpRates = items.OrderByDescending(x => x.gbp).Take(5).Select(x => new { Date = x.date.ToShortDateString(), Rate = x.gbp }).ToList();
            var jpyRates = items.OrderByDescending(x => x.jpy).Take(5).Select(x => new { Date = x.date.ToShortDateString(), Rate = x.jpy }).ToList();
            // Tüm döviz kurları
            var allRates = new List<Dictionary<string, object>>();
            allRates.AddRange(usdRates.Select(x => new Dictionary<string, object> { { "Döviz Cinsi", "USD" }, { "Tarih", x.Date }, { "Kur Değeri", x.Rate } }));
            allRates.AddRange(eurRates.Select(x => new Dictionary<string, object> { { "Döviz Cinsi", "EUR" }, { "Tarih", x.Date }, { "Kur Değeri", x.Rate } }));
            allRates.AddRange(chfRates.Select(x => new Dictionary<string, object> { { "Döviz Cinsi", "CHF" }, { "Tarih", x.Date }, { "Kur Değeri", x.Rate } }));
            allRates.AddRange(gbpRates.Select(x => new Dictionary<string, object> { { "Döviz Cinsi", "GBP" }, { "Tarih", x.Date }, { "Kur Değeri", x.Rate } }));
            allRates.AddRange(jpyRates.Select(x => new Dictionary<string, object> { { "Döviz Cinsi", "JPY" }, { "Tarih", x.Date }, { "Kur Değeri", x.Rate } }));

            // Excel dosyası oluşturma
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Doviz Kuru Verileri");
                // Başlık satırı
                worksheet.Cell("A1").Value = "Döviz Cinsi";
                worksheet.Cell("B1").Value = "Kur Değeri";
                // Veri satırları
                int row = 2;
                foreach (var rate in allRates)
                {
                    worksheet.Cell($"A{row}").Value = rate["Döviz Cinsi"].ToString();
                    worksheet.Cell($"B{row}").Value = rate["Kur Değeri"].ToString();
                    row++;
                }
                // Excel dosyasını stream olarak kaydettim ve kullanıcıya indirme işlemi için son aşama
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Doviz Kuru Verileri.xlsx");
                }
            }
        }
    }
}
