// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.DigitalTwins.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResolutionSample
{
    class Program
    {
        static Resolver _resolver;

        static Program()
        {
            _resolver = new Resolver(new string[] {
                "https://devicemodels.azure.com",
                "https://raw.githubusercontent.com/iotmodels/iot-plugandplay-models/rido/more",
                "https://modelsrepositorytest.azureedge.net/"
            });
        }

        static async Task Main(string[] args)
        {
            var model = @"
            {
              ""@context"": ""dtmi:dtdl:context;2"",
              ""@id"": ""dtmi:com:example:d1;1"",
              ""@type"": ""Interface"",
              ""displayName"": ""d1"",
              ""contents"": [
                {
                  ""@type"": ""Component"",
                  ""name"": ""d1"",
                  ""schema"": ""dtmi:azure:DeviceManagement:DeviceInformation;3""
                }
              ]
            }
            ";

            ModelParser parser = new ModelParser
            {
                DtmiResolver = ResolveCallback
            };
            var res = await parser.ParseAsync(new List<string> { model });

            Console.WriteLine("Parsing success! \n\n");
            res.ToList().ForEach(k => Console.WriteLine(k.Key));
        }

        static async Task<IEnumerable<string>> ResolveCallback(IReadOnlyCollection<Dtmi> dtmis)
        {
            List<string> result = new List<string>();

            foreach (Dtmi dtmi in dtmis)
            {
                string content = await _resolver.ResolveAsync(dtmi.ToString());
                result.Add(content);
            }

            return result;
        }
    }
}
