param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$Output = "publish"
)

$ErrorActionPreference = "Stop"

Write-Host "Publishing AgnesAIImageEdit ($Runtime, $Configuration) -> $Output"
dotnet publish AgnesAIImageEdit/AgnesAIImageEdit.csproj `
    -c $Configuration `
    -r $Runtime `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:PublishTrimmed=false `
    -o $Output

if (!(Test-Path "$Output/AgnesAIImageEdit.exe")) {
    throw "Publish failed: AgnesAIImageEdit.exe not found in $Output"
}
Write-Host "Publish complete."
