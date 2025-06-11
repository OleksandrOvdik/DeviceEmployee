using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models;

namespace Services.Helpers.Middleware;

public class DeviceMiddleWare
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DeviceMiddleWare> _logger;

    public DeviceMiddleWare(RequestDelegate next, IConfiguration configuration, ILogger<DeviceMiddleWare> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("Device middleware started: {Path}, method: {Method}", context.Request.Path, context.Request.Method);
        try
        {

            var dbContext = context.RequestServices.GetRequiredService<MasterContext>();

            if ((context.Request.Path.StartsWithSegments("/api/devices") &&
                 (context.Request.Method == "POST" || context.Request.Method == "PUT")))
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                var json = JsonDocument.Parse(body).RootElement;
                int typeId = json.GetProperty("typeId").GetInt32();
                bool isEnabled = json.TryGetProperty("isEnabled", out var isEn) && isEn.GetBoolean();
                var additionalPropsRaw = json.GetProperty("additionalProperties").ToString();

                var deviceType = await dbContext.DeviceTypes.FindAsync(typeId);
                if (deviceType == null)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Device type not found");
                    return;
                }

                string typeName = deviceType.Name;

                var rulesJson = await File.ReadAllTextAsync("../../validation/example_validation_rules.json");
                var root = JsonDocument.Parse(rulesJson).RootElement;
                var validations = root.GetProperty("validations").EnumerateArray();

                JsonElement? typeValidation = null;
                foreach (var validation in validations)
                {
                    if (validation.TryGetProperty("type", out var typeProp) && typeProp.GetString() == typeName)
                    {
                        typeValidation = validation;
                        break;
                    }
                }

                if (typeValidation == null)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync($"No validation rules for device type '{typeName}'.");
                    return;
                }

                var preRequestName = typeValidation.Value.GetProperty("preRequestName").GetString();
                var preRequestValue = typeValidation.Value.GetProperty("preRequestValue").GetString();
                bool preRequestPassed = false;
                if (preRequestName == "isEnabled")
                    preRequestPassed = isEnabled.ToString().ToLower() == preRequestValue.ToLower();

                if (!preRequestPassed)
                {
                    await _next(context);
                    return;
                }

                var addPropsDict = JsonSerializer.Deserialize<Dictionary<string, object>>(additionalPropsRaw) ?? new();

                foreach (var rule in typeValidation.Value.GetProperty("rules").EnumerateArray())
                {
                    var paramName = rule.GetProperty("paramName").GetString();
                    var regexProp = rule.GetProperty("regex");

                    if (!addPropsDict.TryGetValue(paramName, out var value))
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync(
                            $"Missing required field '{paramName}' for device type '{typeName}'.");
                        return;
                    }

                    var valueStr = value?.ToString() ?? "";

                    if (regexProp.ValueKind == JsonValueKind.Array)
                    {
                        bool found = false;
                        foreach (var allowed in regexProp.EnumerateArray())
                        {
                            if (allowed.GetString() == valueStr)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            context.Response.StatusCode = 400;
                            await context.Response.WriteAsync(
                                $"Validation failed for '{paramName}': value '{valueStr}' is not allowed for device type '{typeName}'."
                            );
                            return;
                        }
                    }
                    else if (regexProp.ValueKind == JsonValueKind.String)
                    {
                        var regex = regexProp.GetString() ?? "";
                        if (regex.StartsWith("/") && regex.EndsWith("/"))
                            regex = regex.Substring(1, regex.Length - 2);
                        if (!Regex.IsMatch(valueStr, regex))
                        {
                            context.Response.StatusCode = 400;
                            await context.Response.WriteAsync(
                                $"Validation failed for '{paramName}' in device type '{typeName}'. Value '{valueStr}' does not match '{regex}'."
                            );
                            return;
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync(
                            $"Invalid validation rule for '{paramName}' in device type '{typeName}'."
                        );
                        return;
                    }
                }
            }
            await _next(context);
            _logger.LogInformation("Device middleware completed: {Path}", context.Request.Path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Device middleware error: {Path}, method: {Method}, error: {Error}", context.Request.Path, context.Request.Method, ex.Message);
        }
    }
}
