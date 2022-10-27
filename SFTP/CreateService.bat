:: Stop If Already Installed Service
sc stop sftp

:: Delete If Already Installed Service
sc delete sftp

SET currentPath=%~dp0
echo "%currentPath%SFTP.exe"
:: Create a Windows Service
sc create sftp DisplayName="SFTP" binPath="%currentPath%SFTP.exe"

:: Start a Windows sftp Service
sc start sftp