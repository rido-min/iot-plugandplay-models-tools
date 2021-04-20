// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.DigitalTwins.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ResolutionSample
{
    class Program
    {
        static Resolver _resolver;

        static Program()
        {
            // HttpClient is intended to be instantiated once per application, rather than per-use.
            // "https://raw.githubusercontent.com/iotmodels/iot-plugandplay-models/rido/more"
            _resolver = new Resolver();
        }

        static async Task Main(string[] args)
        {
            // Target DTMI for resolution.
            //  string toParseDtmi = args.Length == 0 ? "dtmi:com:example:TemperatureController;1" : args[0];

            // Initiate first Resolve for the target dtmi to pass content to parser
            //  string dtmiContent = await Resolve(toParseDtmi);

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
                  ""schema"": ""dtmi:azure:DeviceManagement:DeviceInformation;1""
                }
              ]
            }
            ";

            // Assign the callback
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
            Console.WriteLine("ResolveCallback invoked!");
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
