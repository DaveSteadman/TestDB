@echo off
cd /d E:\Data\03-CodeProjects\22-GithubRepos\TestDB\client
set REACT_APP_API_URL=http://localhost:5001/api
set PORT=5000
set PATH=C:\Program Files\nodejs;%PATH%
"C:\Program Files\nodejs\npm.cmd" start
