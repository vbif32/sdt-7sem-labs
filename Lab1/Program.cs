using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Lab1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //const string link =
            //    @"http://www.neracoos.org/erddap/tabledap/E05_aanderaa_all.json?station%2Cmooring_site_desc%2Cwater_depth%2Ctime%2Ccurrent_speed%2Ccurrent_speed_qc%2Ccurrent_direction%2Ccurrent_direction_qc%2Ccurrent_u%2Ccurrent_u_qc%2Ccurrent_v%2Ccurrent_v_qc%2Ctemperature%2Ctemperature_qc%2Cconductivity%2Cconductivity_qc%2Csalinity%2Csalinity_qc%2Csigma_t%2Csigma_t_qc%2Ctime_created%2Ctime_modified%2Clongitude%2Clatitude%2Cdepth&time%3E=2015-08-25T15%3A00%3A00Z&time%3C=2016-12-05T14%3A00%3A00Z";
            //var wc = new WebClient();
            //var json = wc.DownloadString(link);
            var path = @"Загрузки\E05_aanderaa_all_1769_d432_5004";
            var json = File.ReadAllText(path);

            var characteristics = new Dictionary<string, dynamic>
            {
                {"start_date",DateTime.MaxValue},
                {"end_date",DateTime.MinValue},
                {"num_records",0},
                {"min_COLUMN",float.MaxValue},
                {"min_time",DateTime.MinValue},
                {"max_COLUMN",float.MinValue},
                {"max_time",DateTime.MinValue},
                {"avg_COLUMN",0},
            };
            var parameters = new[] { "current_speed", "temperature", "salinity" };
            var columnNumber = new List<int>(parameters.Length);
            var dateColumn = 3;

            var token = JObject.Parse(json);
            var names = token.SelectToken("table.columnNames").Values<string>().ToList();
            var rows = token.SelectToken("table.rows");

            for (var i = 0; i < names.Count; i++)
                if (parameters.Contains(names.ElementAt(i)))
                    columnNumber.Add(i);

            var result = new Dictionary<string, Dictionary<string, dynamic>>(parameters.Length);
            foreach (string t in parameters)
                result.Add(t, characteristics);

            for (var i = 0; i < rows.Count(); i++)
            for (var j = 0; j < columnNumber.Count; j++)
                if (Math.Abs(rows[i].Value<float>(columnNumber[j] + 1)) < 0.1)
                {
                    var date = rows[i][dateColumn].Value<float>();
                    var value = rows[i][columnNumber[j]].Value<float>();
                }


            Console.Read();
        }

    }
}