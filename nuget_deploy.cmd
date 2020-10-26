pushd "%~dp0"

mkdir "%~dp0NuGetPackages" > nul 2>&1
del "%~dp0NuGetPackages*.nupkg" > nul 2>&1
del "%~dp0NuGetPackages*.snupkg" > nul 2>&1

dotnet pack -o "%~dp0NuGetPackages"

pushd "%~dp0NuGetPackages"
nuget push *.nupkg ^
	-Source "https://api.nuget.org/v3/index.json" ^
	-ApiKey %NUGET_KEY%

popd
popd