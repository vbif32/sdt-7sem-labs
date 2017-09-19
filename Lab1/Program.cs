using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            var path = @"C:\Users\Vbif3\Downloads\E05_aanderaa_all_1769_d432_5004.json";
            var json = File.ReadAllText(path);

            var characteristics = new Dictionary<string, dynamic>
            {
                {"start_date",null},
                {"end_date",null},
                {"num_records",0},
                {"min_COLUMN",float.MaxValue},
                {"min_time",null},
                {"max_COLUMN",float.MinValue},
                {"max_time",null},
                {"avg_COLUMN",0.00}
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
            foreach (var param in parameters)
            {
                var tmp = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(JsonConvert.SerializeObject(characteristics));
                result.Add(param, tmp);
            }
                

            for (var i = 0; i < rows.Count(); i++)
                for (var j = 0; j < columnNumber.Count; j++)
                    if (rows[i].Value<float>(columnNumber[j] + 1) == 0)
                    {
                        var date = rows[i][dateColumn].Value<DateTime>();
                        var value = rows[i][columnNumber[j]].Value<float>();
                        if (i == 0)
                            result[parameters[j]]["start_date"] = date;


                        if (result[parameters[j]]["min_COLUMN"] > value)
                        {
                            result[parameters[j]]["min_COLUMN"] = value;
                            result[parameters[j]]["min_time"] = date;
                        }
                        if (result[parameters[j]]["max_COLUMN"] < value)
                        {
                            result[parameters[j]]["max_COLUMN"] = value;
                            result[parameters[j]]["max_time"] = date;
                        }

                        result[parameters[j]]["num_records"]++;
                        result[parameters[j]]["avg_COLUMN"] += value;
                        result[parameters[j]]["end_date"] = date;
                    }

            foreach(var param in parameters)
                result[param]["avg_COLUMN"] /= result[param]["num_records"];

            var resultJson = JsonConvert.SerializeObject(result);
            resultJson = resultJson.Replace("{", "{\n");
            resultJson = resultJson.Replace("}", "\n}");
            resultJson = resultJson.Replace(",", ",\n");
            Console.WriteLine(resultJson);
            Console.Read();
        }
    }
}