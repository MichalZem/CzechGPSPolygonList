using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using GeoJSON.Net.Converters;
using Newtonsoft.Json;

namespace ConsoleApp2
{
    public class Program
    {

        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        static void Main(string[] args)
        {

            System.IO.StreamReader file = new System.IO.StreamReader("TMMESTSKECASTI_P.json");
            string content = file.ReadToEnd();
            file.Close();

            dynamic deserialized = JsonConvert.DeserializeObject(content);
            foreach (var item in deserialized.features)
            {
                var coordinates = ((String)item.geometry.coordinates.ToString())
                            .Replace("\r\n", "").Replace("] ]", "").Replace(" ", "").Replace("[[", "").Replace("]]", "")
                            .Split("],[");

                for (int i = 0; i < coordinates.Length; i++)
                {
                    coordinates[i] = coordinates[i].Replace("[", "").Replace("]", "");
                }

                string name = RemoveDiacritics((item.properties.NAZEV_MC.ToString()).ToLower()).Replace("-", "").Replace(" ", "");


                StringBuilder sbOutput = new StringBuilder($"var {name}=[\r\n");
                bool first = true;
                foreach (var coordinate in coordinates)
                {
                    if (first) { first = false; } else { sbOutput.Append(","); }

                    sbOutput.AppendLine($"{{lng: {coordinate.Split(",")[0]}, lat:{coordinate.Split(",")[1]}}}");
                }
                sbOutput.AppendLine("];");

                File.WriteAllText(name + ".js", sbOutput.ToString());


            }
        }
    }
}
