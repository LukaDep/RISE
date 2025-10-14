using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

// Deze klasse haalt roosterdata op van TimeEdit via HTTP.
// Gebruik het in een service layer of controller om data binnen te trekken.
namespace Rise.Services.TimeEdit
{
    public class TimeEditClient
    {
        private readonly HttpClient _httpClient;

        public TimeEditClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Haalt het lessenrooster op via de opgegeven TimeEdit-URL.
        /// </summary>
        /// <param name="url">De volledige JSON-URL (bv. https://cloud.timeedit.net/.../riXXXX.json)</param>
        /// <returns>TimeEditResponse object met kolommen en reservaties</returns>
        public async Task<TimeEditResponse?> GetScheduleAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<TimeEditResponse>(url);
                return response;
            }
            catch (HttpRequestException ex)
            {
                // Log eventueel hier
                throw new Exception($"TimeEdit ophalen mislukt: {ex.Message}");
            }
        }
    }
}
