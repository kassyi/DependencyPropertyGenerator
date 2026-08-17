$ErrorActionPreference = "Stop"

$workspaceRoot = Split-Path -Parent $MyInvocation.MyCommand.Path | Split-Path -Parent
$snapshotsDir = Join-Path $workspaceRoot "tests" | Join-Path -ChildPath "Kassyi.Generators.DependencyProperty.SnapshotTests" | Join-Path -ChildPath "Snapshots"

Write-Host "Accepting new snapshots..."
$receivedFiles = Get-ChildItem -Path $snapshotsDir -Filter "*.received.*" -Recurse
foreach ($receivedFile in $receivedFiles) {
    $verifiedFile = $receivedFile.FullName -replace '\.received\.', '.verified.'
    Move-Item -Path $receivedFile.FullName -Destination $verifiedFile -Force
    Write-Host "Accepted: $(Split-Path $verifiedFile -Leaf)"
}