using Microsoft.Azure.DigitalTwins.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ResolutionSample
{
    class Resolver
    {
        readonly List<string> _repos = new();

        readonly HttpClient _httpClient;

        public static string DtmiToPath(string dtmi) => $"/{dtmi.ToLowerInvariant().Replace(":", "/").Replace(";", "-")}.json";

        public Resolver() : this(Array.Empty<string>()) { }

        public Resolver(string repoUrl) : this(new string[] { repoUrl }) { }

        public Resolver(string[] reposUrls)
        {
            if (reposUrls.Length > 0)
            {
                _repos = reposUrls.ToList();
            }
            else
            {
                _repos.Add("https://devicemodels.azure.com");
            }
            _httpClient = new HttpClient();
        }

        public async Task<IEnumerable<string>> ResolveCallback(IReadOnlyCollection<Dtmi> dtmis)
        {
            List<string> result = new List<string>();

            foreach (Dtmi dtmi in dtmis)
            {
                string content = await this.ResolveAsync(dtmi.ToString());
                result.Add(content);
            }

            return result;
        }

        async Task<string> ResolveAsync(string dtmi)
        {
            string modelContent = string.Empty;
            foreach (var repo in _repos)
            {
                var fullyQualifiedPath = $"{repo}{DtmiToPath(dtmi)}";
                System.Diagnostics.Debug.WriteLine($"LOG:  {dtmi} --> {fullyQualifiedPath}");
                modelContent = await FetchAsync(fullyQualifiedPath);
                if (!string.IsNullOrEmpty(modelContent))
                {
                    return modelContent;
                }
            }
            if (string.IsNullOrEmpty(modelContent))
            {
                throw new ApplicationException("Dtmi not resolved in any of the configured repos.");
            }
            return modelContent;
        }

        async Task<string> FetchAsync(string url)
        {
            try
            {
                return await _httpClient.GetStringAsync(url);
            }
            catch (HttpRequestException hex) when (hex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                System.Diagnostics.Debug.WriteLine($"404: {url}");
            }
            return null;
        }
    }
}
