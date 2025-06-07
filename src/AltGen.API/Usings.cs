global using System.ComponentModel.DataAnnotations;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.Reflection;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Threading.RateLimiting;

global using AltGen.API.Common;
global using AltGen.API.Generate;
global using AltGen.API.Generate.Providers;
global using AltGen.API.Generate.Providers.Gemini;

global using Microsoft.AspNetCore.Mvc;

global using OpenTelemetry.Exporter;
global using OpenTelemetry.Logs;
global using OpenTelemetry.Resources;
global using OpenTelemetry.Trace;
