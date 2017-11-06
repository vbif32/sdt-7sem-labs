using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lab1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string[] parameters = { "current_speed", "temperature", "salinity" };
            string resultJson = null;
            string startJson;

            try
            {
                const string path = @"C:\Users\нано\Downloads\E05_aanderaa_all_1769_d432_5004.json";
                startJson = File.ReadAllText(path);
                resultJson = NeracoosParse(startJson, parameters);
            }
            catch (Exception )
            {
                var retries = 3;
                while (retries >= 0)
                    try
                    {
                        const string link =
                            @"http://www.neracoos.org/erddap/tabledap/E05_aanderaa_all.json?station%2Cmooring_site_desc%2Cwater_depth%2Ctime%2Ccurrent_speed%2Ccurrent_speed_qc%2Ccurrent_direction%2Ccurrent_direction_qc%2Ccurrent_u%2Ccurrent_u_qc%2Ccurrent_v%2Ccurrent_v_qc%2Ctemperature%2Ctemperature_qc%2Cconductivity%2Cconductivity_qc%2Csalinity%2Csalinity_qc%2Csigma_t%2Csigma_t_qc%2Ctime_created%2Ctime_modified%2Clongitude%2Clatitude%2Cdepth&time%3E=2015-08-25T15%3A00%3A00Z&time%3C=2016-12-05T14%3A00%3A00Z";
                        var wc = new WebClient();
                        startJson = wc.DownloadString(link);
                        resultJson = NeracoosParse(startJson, parameters);
                    }
                    catch (Exception)
                    {
                        if (retries == 0)
                            throw;
                        retries--;
                        Thread.Sleep(1000);
                    }
            }
            Console.WriteLine(resultJson);
            Console.Read();
        }

        private static string NeracoosParse(string json, string[] parameters, Formatting formatting = Formatting.Indented)
        {
            var columnNumber = new List<int>(parameters.Length);
            const int dateColumn = 3;

            var token = JObject.Parse(json);
            var names = token.SelectToken("table.columnNames").Values<string>().ToList();
            var rows = token.SelectToken("table.rows");

            for (var i = 0; i < names.Count; i++)
                if (parameters.Contains(names.ElementAt(i)))
                    columnNumber.Add(i);

            var result = new Dictionary<string, Dictionary<string, dynamic>>(parameters.Length);
            foreach (var param in parameters)
                result.Add(param, new Dictionary<string, dynamic>
                {
                    {"start_date", null},
                    {"end_date", null},
                    {"num_records", 0},
                    {"min_"+param, float.MaxValue},
                    {"min_time", null},
                    {"max_"+param, float.MinValue},
                    {"max_time", null},
                    {"avg_"+param, 0.00}
                });
            
            for (var i = 0; i < rows.Count(); i++)
            for (var j = 0; j < columnNumber.Count; j++)
                if (Math.Abs(rows[i].Value<float>(columnNumber[j] + 1)) < 0.1)
                {
                    var date = rows[i][dateColumn].Value<DateTime>();
                    var value = rows[i][columnNumber[j]].Value<float>();
                    var param = parameters[j];
                    if (i == 0)
                        result[param]["start_date"] = date;
                    if (result[param]["min_"+param] > value)
                    {
                        result[param]["min_"+param] = value;
                        result[param]["min_time"] = date;
                    }
                    if (result[param]["max_"+param] < value)
                    {
                        result[param]["max_"+param] = value;
                        result[param]["max_time"] = date;
                    }
                    result[param]["num_records"]++;
                    result[param]["avg_"+param] += value;
                    result[param]["end_date"] = date;
                }
            foreach (var param in parameters)
                result[param]["avg_"+param] /= result[param]["num_records"];

            return JsonConvert.SerializeObject(result, formatting);
        }
    }
}