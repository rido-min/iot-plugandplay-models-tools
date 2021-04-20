using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ResolutionSample
{
    class Resolver
    {
        //const string _repositoryEndpoint = "https://raw.githubusercontent.com/iotmodels/iot-plugandplay-models/rido/more"; // "https://devicemodels.azure.com";

        List<string> _repos = new List<string>();
        readonly HttpClient _httpClient;

        public Resolver() : this(new string[] { })
        {

        }

        public Resolver(string repoUrl) : this(new string[] { repoUrl })
        {

        }

        public Resolver(string[] reposUrls)
        {
            if (reposUrls.Length > 0)
            {
                _repos = reposUrls.ToList<string>();
            } 
            else
            {
                _repos.Add("https://devicemodels.azure.com");
            }
            _httpClient = new HttpClient();
        }

        public async Task<string> ResolveAsync(string dtmi)
        {
            string modelContent = string.Empty;
            foreach (var repo in _repos)
            {
                var fullyQualifiedPath = $"{repo}{DtmiToPath(dtmi)}";
                Console.WriteLine($"LOG:  {dtmi} --> {fullyQualifiedPath}");
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
                Console.WriteLine($"404: {url}");
            }
            return null;
        }

        string DtmiToPath(string dtmi)
        {
            if (!IsValidDtmi(dtmi))
            {
                return null;
            }
            // dtmi:com:example:Thermostat;1 -> dtmi/com/example/thermostat-1.json
            return $"/{dtmi.ToLowerInvariant().Replace(":", "/").Replace(";", "-")}.json";
        }

        bool IsValidDtmi(string dtmi)
        {
            // Regex defined at https://github.com/Azure/digital-twin-model-identifier#validation-regular-expressions
            Regex rx = new Regex(@"^dtmi:[A-Za-z](?:[A-Za-z0-9_]*[A-Za-z0-9])?(?::[A-Za-z](?:[A-Za-z0-9_]*[A-Za-z0-9])?)*;[1-9][0-9]{0,8}$");
            return rx.IsMatch(dtmi);
        }
    }
}
