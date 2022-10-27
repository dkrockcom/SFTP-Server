using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NLog;
using Nuane.Net;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFTP
{
	public class UserInfo
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string Directory { get; set; }
	}

	public class Worker : IHostedService
	{
		private SftpServer sftpServer;
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public async Task StopAsync(CancellationToken stoppingToken)
		{
			logger.Trace("SFTP Service is stopping.");
			sftpServer.Stop();
			sftpServer = null;
			await Task.CompletedTask;
		}

		Task IHostedService.StartAsync(CancellationToken cancellationToken)
		{
			// generate server keys (in practice, load them from a saved file
			SshKey rsaKey = SshKey.Generate(SshKeyAlgorithm.RSA, 1024);
			SshKey dssKey = SshKey.Generate(SshKeyAlgorithm.DSS, 1024);

			// add keys, bindings and users
			sftpServer = new SftpServer();
			sftpServer.LogLevel = Nuane.Logging.LogLevel.Detailed;
			sftpServer.Log = Console.Out;

			sftpServer.Keys.Add(rsaKey);
			sftpServer.Keys.Add(dssKey);
			sftpServer.Bindings.Add(IPAddress.Any, Convert.ToInt32(Program.AppSetting["ServerPort"]));

			List<UserInfo> users = Program.AppSetting.GetSection("Users").Get<List<UserInfo>>();
			foreach (var user in users)
			{
				sftpServer.Users.Add(new SshUser(user.Username, user.Password, user.Directory));
			}
			sftpServer.Start();
			logger.Trace("SFTP Service Started");
			return Task.CompletedTask;
		}
	}
}
