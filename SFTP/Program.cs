using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SFTP
{
	class Program
	{
		public static IConfigurationRoot AppSetting { get; set; }
		private static string AppSettingsPath => Path.Join(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "appsettings.json");
		private static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
		private static Logger logger = LogManager.GetCurrentClassLogger();

		static async Task Main(string[] args)
		{
			try
			{
				logger.Trace("Main Program Starting");
				var isService = !(Debugger.IsAttached || Environment.GetCommandLineArgs().Contains<string>("-console"));
				var builder = IsLinux ? Host.CreateDefaultBuilder(args) : new HostBuilder();
				builder.ConfigureAppConfiguration((context, config) =>
				{
					config.Sources.Clear();
					AppSetting = config.AddJsonFile(AppSettingsPath, optional: true, reloadOnChange: true).Build();
					config.AddEnvironmentVariables();
					if (args != null)
					{
						config.AddCommandLine(args);
					}
				});
				builder.ConfigureServices((hostContext, services) =>
				{
					services.AddHostedService<Worker>();
				});
				if (IsLinux)
				{
					builder.UseSystemd();
					builder.Build().Run();
				}
				else
				{
					if (isService)
					{
						logger.Trace("Window Service");
						await builder.UseWindowsService().RunAsServiceAsync();
					}
					else
					{
						logger.Trace("Debug or Console Service");
						await builder.RunConsoleAsync();
					}
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				throw ex;
			}
		}
	}
}