global using System.Collections;
global using System.Net;
global using System.Net.Http.Headers;
global using System.Net.Http.Json;
global using System.Text;

global using AltGen.API.Generate;
global using AltGen.API.Generate.Providers.Gemini;
global using AltGen.API.Tests.Fixtures;
global using AltGen.API.Tests.Utils;

global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.AspNetCore.TestHost;
global using Microsoft.Extensions.DependencyInjection;

global using RichardSzalay.MockHttp;