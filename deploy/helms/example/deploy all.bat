@ECHO OFF
setlocal enabledelayedexpansion
for /F "delims=" %%f in ('dir /a:d /b /s') do (
	echo %%f
	cd %%f
	%%f/deploy.bat
)