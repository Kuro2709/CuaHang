using System.Net.Http;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace CuaHang.Helpers
{
    public static class HttpClientExtensions
    {
        public static async Task<T> ReadContentAsync<T>(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new HttpRequestException($"Something went wrong calling the API: {response.ReasonPhrase}. Content: {errorContent}");
            }

            var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            try
            {
                var result = JsonSerializer.Deserialize<T>(
                    dataAsString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                return result;
            }
            catch (JsonException ex)
            {
                throw new ApplicationException($"Error deserializing response content: {dataAsString}", ex);
            }
        }
    }
}