using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace DovizKurApiUygulama.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeRatesController : ControllerBase
    { 
        // API'den döviz kuru verilerini çekmek için HttpClient kullanıyoruz
        private readonly HttpClient _httpClient;

        public ExchangeRatesController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        //Döviz kurlarını tcmb sitesinden çektiğim  method
        [HttpGet]
        public async Task<IActionResult> GetExchangeRates()
        {
            string url = "https://evds2.tcmb.gov.tr/service/evds/series=TP.DK.USD.A-TP.DK.EUR.A-TP.DK.CHF.A-TP.DK.GBP.A-TP.DK.JPY.A&startDate=03-02-2023&endDate=05-03-2023&type=xml&key=IGWa5JleON";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                //aldığımız veriyi stringe çevirir
                string xmlString = await response.Content.ReadAsStringAsync();
                //stringe dönüştürdüğümüz veriyi xml formatına çeviriyoruz
                XDocument doc = XDocument.Parse(xmlString);
               //çektiğimiz api dökümünanının sarmaladığı items etiketi altındaki verilere erişip onlarda boş değerler varsa sıfır atıyoruz
                var exchangeRates = doc.Descendants("items").Select(item => new
                {
                    date = item.Element("Tarih").Value,
                    usd = string.IsNullOrEmpty(item.Element("TP_DK_USD_A").Value) ? "0" : item.Element("TP_DK_USD_A").Value,
                    eur = string.IsNullOrEmpty(item.Element("TP_DK_EUR_A").Value) ? "0" : item.Element("TP_DK_EUR_A").Value,
                    chf = string.IsNullOrEmpty(item.Element("TP_DK_CHF_A").Value) ? "0" : item.Element("TP_DK_CHF_A").Value,
                    gbp = string.IsNullOrEmpty(item.Element("TP_DK_CHF_A").Value) ? "0" : item.Element("TP_DK_GBP_A").Value,
                    jpy = string.IsNullOrEmpty(item.Element("TP_DK_CHF_A").Value) ? "0" : item.Element("TP_DK_JPY_A").Value,
                });
                //verilerin etiket isimlerini değiştiriyoruz yani örneğin TP_DK_USD_A değilde usd olarak 
                XElement root = new XElement("ExchangeRates",
                    from ex in exchangeRates
                    select new XElement("items",
                        new XElement("date", ex.date),
                        new XElement("usd", ex.usd),
                        new XElement("eur", ex.eur),
                        new XElement("che", ex.chf),
                        new XElement("gbp", ex.gbp),
                        new XElement("jpy", ex.jpy)
                    )
                );
                //xml formatında gönderiyoruz
                return Content(root.ToString(), "application/xml");
            }
            else
            {
                return StatusCode((int)response.StatusCode, response.ReasonPhrase);
            }
        }
    }
}
