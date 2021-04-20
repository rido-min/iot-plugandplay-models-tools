// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.DigitalTwins.Parser;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ResolutionSample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var resolver = new Resolver(
                "https://devicemodels.azure.com",
                "https://raw.githubusercontent.com/iotmodels/iot-plugandplay-models/rido/more",
                "https://modelsrepositorytest.azureedge.net"
            );

            ModelParser parser = new() { DtmiResolver =  resolver.ResolveCallback };
            var res = await parser.ParseAsync(new string[] { model });

            Console.WriteLine("Parsing succeed ! DTMIs found: \n\n");
            res.ToList().ForEach(k => Console.WriteLine(k.Key));
        }

        const string model = @"
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
        }";
    }
}
