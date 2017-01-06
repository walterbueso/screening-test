using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using System.Text;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //I screwed up by rushing and comapring results with test case
            string path = @"access.log";
            StringBuilder csvContent = new StringBuilder();
            string csvPath = @"C:\Users\walte\Desktop\screening-test\report.csv";
            StreamReader file = new StreamReader(File.OpenRead(path));
            string ex = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            Regex reg = new Regex(ex);
            int counter = 0;
            string line;
            string[] bline;
            var ips = new List<string>();


            while ((line = file.ReadLine()) != null)
            {

                bline = line.Split(' ');
                /*
                Add current ip to ips list if these conditions are met:
                -Current line length being read is = 21
                -element on bline[2] (client ip) matches the regular expression
                -the current ip does not start with 207.114
                -current port = 80
                -current method = GET
                */
                if (bline.Length == 21 && reg.IsMatch(bline[2]) && !bline[2].StartsWith("207.114") && bline[7].Equals("80") && bline[8].Equals("GET"))
                {
                    ips.Add(bline[2]);
                }
                counter++;

            }


            
            var by_request = ips.GroupBy(x => x)
            .Select(g => new { Value = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Where(x => x.Count > 1);

            var by_octet = ips.OrderByDescending(Version.Parse).Distinct().ToList();

           

            foreach (var r in by_request)
            {
                System.Console.WriteLine("Value: " + r.Value + " Count: " + r.Count);
                csvContent.AppendLine(r.Count +","+r.Value);

            }

            foreach(var o in by_octet)
            {
                csvContent.AppendLine("1," +"\""+ o + "\"");
            }

            
            File.AppendAllText(csvPath, csvContent.ToString());

            file.Dispose();
        }
    }
}
