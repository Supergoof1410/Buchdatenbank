using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace Buchdatenbank
{
    // Ein Prozess der den Html-Code über eine URI holt. Dieser Prozess wird
    // nur einmal pro Anwendung gestartet, damit es nicht zu einer SocketException kommt.
    // Nähere Informationen gibt es unter: 
    // https://learn.microsoft.com/de-de/dotnet/api/system.net.http.httpclient?view=net-7.0
    internal class HttpClientHtml
    {
        private static readonly HttpClient htmlClient = new();
        internal static async Task<string> GetHtmlCode(string path)
        {
            try
            {
                string responseHtml = await htmlClient.GetStringAsync(path).ConfigureAwait(false);
                return responseHtml;
            }
            catch (HttpRequestException e)
            {
                MessageBox.Show(e.Message);
            }
            return "";
        }
    }
}
