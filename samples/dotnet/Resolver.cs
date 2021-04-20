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
            Console.WriteLine($"Attempting to resolve: {dtmi}");

            string fullyQualifiedPath = $"{_repos[0]}{DtmiToPath(dtmi.ToString())}";
            Console.WriteLine($"Fully qualified model path: {fullyQualifiedPath}");

            // Make request
            string modelContent = await _httpClient.GetStringAsync(fullyQualifiedPath);

            // Output string model content to stdout
            Console.WriteLine("Received content...");
            // Console.WriteLine(modelContent);

            return modelContent;
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
