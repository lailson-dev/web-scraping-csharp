using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace web_scraping
{
    class Program
    {
        static void Main(string[] args)
        {
            getHtmlAsync();

            Console.ReadKey();
        }

        private static async void getHtmlAsync()
        {
            Console.WriteLine("==== Bem vindo ao sistema de Scraping ====");
         
            string url = "http://www.b3.com.br/pt_br/solucoes/plataformas/puma-trading-system/para-participantes-e-traders/calendario-de-negociacao/feriados/";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            string IdPanel = string.Empty;
            string filePath = @"C:\WebScraping\file.txt";

            Information info = new Information();

            var ulMonths = htmlDocument.DocumentNode.SelectNodes("//ul[@class='accordion']");

            var liMonths = ulMonths.Descendants("li")
                .Where(x => x.GetAttributeValue("class", "")
                .Equals("accordion-navigation")).ToList();

            foreach(var li in liMonths)
            {
                IdPanel = li.Descendants("a")
                    .FirstOrDefault(x => x.GetAttributeValue("href", "")
                    .Contains("panel"))
                    .Attributes["href"].Value.Replace("#", "");

             
                info.Month = li.FirstChild.InnerText;
                Console.WriteLine(info.Month);

                var divs = li.Descendants("div")
                    .Where(x => x.GetAttributeValue("id", "")
                    .Equals(IdPanel)).ToList();

                foreach (var div in divs)
                {
                    foreach(var tr in div.ChildNodes.Descendants("tbody"))
                    {
                        List<HtmlNode> x = tr.Elements("tr").ToList();

                        foreach(HtmlNode item in x)
                        {
                            info.Day = WebUtility.HtmlDecode(item.ChildNodes[1].InnerText.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("  ", ""));
                            info.Event = WebUtility.HtmlDecode(item.ChildNodes[3].InnerText.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("  ", ""));
                            info.Description = WebUtility.HtmlDecode(item.ChildNodes[7].InnerText.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("  ", ""));

                            string jsonOutput = JsonConvert.SerializeObject(info);

                            Console.WriteLine(jsonOutput);
                            Console.WriteLine();

                            try
                            {
                                StreamWriter sw = new StreamWriter(filePath, true);
                                sw.WriteLine(jsonOutput);
                                sw.Close();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }

                        }
                    }
                }   

            }
            Console.WriteLine();
            Console.WriteLine("O arquivo foi gerado no caminho C:\\WebScraping\\file.txt");
            Console.WriteLine();
            Console.WriteLine("Pressione qualquer tecla para sair...");
        }
    }
}
