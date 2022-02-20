using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace WinFormClient
{
    internal static class Extensions
    {
        internal static string PrettifyJsonString(this string json)
        {
            var jsonObject = JsonSerializer.Deserialize<dynamic>(json);
            return JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { 
                WriteIndented = true 
            });
        }
    }
}
