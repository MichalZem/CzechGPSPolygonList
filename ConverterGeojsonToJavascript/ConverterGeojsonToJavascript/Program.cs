using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            string nameDistrict = args[1].Replace("-", "");

            StringBuilder sbOutput = new StringBuilder($"var {nameDistrict}=[\r\n");

            string inputContent = File.ReadAllText(args[0]);

            string re1 = @"(\[\d*\.\d*,\d*.\d*\])";

            Regex r = new Regex(re1, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection matches = r.Matches(inputContent);

            int counter = matches.Count;
            foreach (Match item in matches)
            {
                string Gps = item.Groups[1].Value;

                Regex r1 = new Regex(@"(\d*\.\d*)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

                MatchCollection matches1 = r1.Matches(Gps);
                string lng = matches1[0].Value;
                string lat = matches1[1].Value;
                sbOutput.Append($"{{lng: {lng}, lat:{lat}}}");

                if (counter-- > 1) sbOutput.AppendLine(",");

                Console.Write("\rZbývá ke konverzi:{0}", counter);
            }

            sbOutput.AppendLine("");
            sbOutput.Append("];");

            sbOutput.Append($@"
                    var {nameDistrict}Area = new google.maps.Polygon({{
                              paths: {nameDistrict},
                              strokeColor: '#0000FF',
                              strokeOpacity: 0.8,
                              strokeWeight: 2,
                              fillColor: '#FFFFFF',
                              fillOpacity: 0.35
                            }});
                    {nameDistrict}Area.setMap(map);");

            File.WriteAllText(nameDistrict + ".js", sbOutput.ToString());

            Console.Write("\r                            ");
            Console.WriteLine("\rHotovo.");
        }
    }
}