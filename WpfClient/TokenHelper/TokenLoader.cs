using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WpfClient.TokenHelper
{
    /// <summary>
    /// Token request helper
    /// </summary>>
    public static class TokenLoader
    {
        public static Task RequestToken(string name,
                                        string pass,
                                        string servUrl)
        {
            var content = GetContent(name, pass);
            return GetTokenAsync(content, servUrl);
        }

        private static StringContent GetContent(string name,
                                                string pass)
        {
            var authModel = new
            {
                Login = name,
                Password = pass
            };

            string json = JsonSerializer.Serialize(authModel);

            return new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        }


        private static async Task<string> GetTokenAsync(StringContent content, string servUrl)
        {
            string responseResult;
            using (HttpClient httpCl = new HttpClient())
            {
                var response = await httpCl.PostAsync($"{servUrl}", content);
                if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    var responseText = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseText))
                    {
                        Console.WriteLine(responseText);
                        return null;
                    }
                }

                response.EnsureSuccessStatusCode();
                responseResult = await response.Content.ReadAsStringAsync();
            }
            try
            {
                if (!string.IsNullOrEmpty(responseResult))
                {
                    return JsonSerializer.Deserialize<dynamic>(responseResult);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }
    }
}
