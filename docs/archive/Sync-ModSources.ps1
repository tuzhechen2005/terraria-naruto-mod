$sourceDirectory = Join-Path $PSScriptRoot 'ShinobiPrototype'
$documentsDirectory = [Environment]::GetFolderPath('MyDocuments')
$profileSources = Join-Path $env:USERPROFILE 'Documents\My Games\Terraria\tModLoader\ModSources\ShinobiPrototype'
$targetDirectory = if (Test-Path -LiteralPath $profileSources) {
    $profileSources
} else {
    Join-Path $documentsDirectory 'My Games\Terraria\tModLoader\ModSources\ShinobiPrototype'
}

if (-not (Test-Path -LiteralPath $sourceDirectory)) {
    throw "Project source was not found: $sourceDirectory"
}

if (-not (Test-Path -LiteralPath $targetDirectory)) {
    New-Item -ItemType Directory -Path $targetDirectory | Out-Null
}

$sourceRoot = (Resolve-Path -LiteralPath $sourceDirectory).Path
$targetInfo = Get-Item -LiteralPath $targetDirectory -Force
if ($targetInfo.LinkType -eq 'Junction' -and
    [string]::Equals([string]$targetInfo.Target, $sourceRoot,
        [System.StringComparison]::OrdinalIgnoreCase)) {
    Write-Output "ModSources junction already points to $sourceRoot; no copy needed"
    return
}

Get-ChildItem -LiteralPath $sourceRoot -Recurse -File | Where-Object {
    $_.FullName -notmatch '[\\/](bin|obj|\.vs)[\\/]' -and
    $_.Name -ne 'ShinobiPrototype.csproj'
} | ForEach-Object {
    $relativePath = $_.FullName.Substring($sourceRoot.Length).TrimStart('\', '/')
    $targetPath = Join-Path $targetDirectory $relativePath
    $targetParent = Split-Path -Parent $targetPath
    if (-not (Test-Path -LiteralPath $targetParent)) {
        New-Item -ItemType Directory -Path $targetParent | Out-Null
    }
    Copy-Item -LiteralPath $_.FullName -Destination $targetPath -Force
}

Write-Output "Synced ShinobiPrototype to $targetDirectory"
