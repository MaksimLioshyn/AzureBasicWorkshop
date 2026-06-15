using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WebApp.Controllers;

public class InfoController : Controller
{
    public IActionResult Index()
    {
        var model = new SystemInfoViewModel
        {
            // Docker info
            IsRunningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true",
            HostName = Environment.MachineName,

            // Azure AD config
            AzureAdInstance = Environment.GetEnvironmentVariable("AzureAd__Instance") ?? "Not configured",
            AzureAdTenantId = Environment.GetEnvironmentVariable("AzureAd__TenantId") ?? "Not configured",
            AzureAdClientId = Environment.GetEnvironmentVariable("AzureAd__ClientId") ?? "Not configured",
            HasClientSecret = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AzureAd__ClientSecret")),

            // System info
            OSDescription = RuntimeInformation.OSDescription,
            FrameworkDescription = RuntimeInformation.FrameworkDescription,
            ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
            AspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",

            // Process info
            ProcessId = Environment.ProcessId,
            WorkingSet = Environment.WorkingSet / 1024 / 1024, // MB
            StartTime = Process.GetCurrentProcess().StartTime,
            Uptime = DateTime.Now - Process.GetCurrentProcess().StartTime
        };

        return View(model);
    }
}

public class SystemInfoViewModel
{
    public bool IsRunningInDocker { get; set; }
    public string HostName { get; set; } = string.Empty;

    public string AzureAdInstance { get; set; } = string.Empty;
    public string AzureAdTenantId { get; set; } = string.Empty;
    public string AzureAdClientId { get; set; } = string.Empty;
    public bool HasClientSecret { get; set; }

    public string OSDescription { get; set; } = string.Empty;
    public string FrameworkDescription { get; set; } = string.Empty;
    public string ProcessArchitecture { get; set; } = string.Empty;
    public string AspNetCoreEnvironment { get; set; } = string.Empty;

    public int ProcessId { get; set; }
    public long WorkingSet { get; set; }
    public DateTime StartTime { get; set; }
    public TimeSpan Uptime { get; set; }
}
