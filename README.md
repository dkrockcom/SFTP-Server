# SFTP Server
## Custom SFTP Server Service for window and Linux server

- SFTP server which is supports both windows and Linux
- Easy to setup
- Create User with Specific directory with custom directory access
- JSON based users Credentials management
- Configurable server port

# Tech
- .Net Core

# Service Installation on Windows Server
- Goto SFTP Application Directory
- Click on CreateService.bat
- After that, you can check SFTP Service in windows service

# Create User's

```json
{
	"ServerPort": 2222,
	"Users": [
		{
			"Username": "admin",
			"Password": "admin",
			"Directory": "c:\\sftp\\web"
		}
	]
}
```

# Reference
https://nuane.com/sftp-lite/

## License
MIT
**Free Software**
