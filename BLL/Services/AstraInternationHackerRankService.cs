using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace BLL.Services
{
    public class AstraInternationHackerRankService
    {
        public static string GetApi(string ApiUrl)
        {

            var responseString = "";
            var request = (HttpWebRequest)WebRequest.Create(ApiUrl);
            request.Method = "GET";
            request.ContentType = "application/json";

            using (var response1 = request.GetResponse())
            {
                using (var reader = new StreamReader(response1.GetResponseStream()))
                {
                    responseString = reader.ReadToEnd();
                }
            }
            return responseString;

        }
        public List<string> getUsernames(int threshold)
        {
            List<string> result = new List<string>();
            var count = 1;
            var total = 0;

            var dataFirstPage = GetApi("https://jsonmock.hackerrank.com/api/article_users?page=1");
            var rootFirstPage =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<AstraInternationHackerRankDTO>(dataFirstPage);
            total = rootFirstPage.total_pages;

            while (count <= total)
            {
                var responseString = GetApi($"https://jsonmock.hackerrank.com/api/article_users?page={count}");
                var rootObject =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<AstraInternationHackerRankDTO>(responseString);
                foreach (var user in rootObject.data)
                {
                    if (user.submission_count > threshold)
                    {
                        result.Add(user.username);
                    }
                }
                count++;
            }
            return result;

        }
    }
    class AstraInternationHackerRankDTO
    {
        public int page { get; set; }
        public int per_page { get; set; }
        public int total { get; set; }
        public int total_pages { get; set; }
        public List<AstraInternationHackerRankDataDTO> data { get; set; }
    }

    class AstraInternationHackerRankDataDTO
    {
        public string username { get; set; }
        public int submission_count { get; set; }
    }
}
