Get-ChildItem -Recurse -Filter *.received.* | ForEach-Object { Move-Item -Force $_.FullName ($_.FullName -replace "\.received\.", ".verified.") }
