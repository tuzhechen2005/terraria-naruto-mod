Add-Type -AssemblyName System.Drawing

$root = Join-Path $PSScriptRoot '..\ShinobiPrototype\Content\NPCs'
$projectRoot = Join-Path $PSScriptRoot '..\ShinobiPrototype'
$bossSource = Get-Content -LiteralPath (Join-Path $root 'HakuBoss.cs') -Raw
if ($bossSource -notmatch 'BossHeadTexture\s*=>\s*"ShinobiPrototype/Content/NPCs/HakuBoss_Head_Boss"') {
    throw 'Haku boss head path does not match its map icon asset'
}
if (Test-Path -LiteralPath (Join-Path $projectRoot 'Common\Systems\WaveDuoBarSystem.cs')) {
    throw 'Custom duo boss bar must stay removed in favor of vanilla bars'
}
foreach ($bar in @('HakuBossBar.cs', 'ZabuzaBossBar.cs')) {
    $barSource = Get-Content -LiteralPath (Join-Path $root $bar) -Raw
    if ($barSource -match 'ModifyInfo\s*\(') {
        throw "$bar still suppresses the vanilla boss bar"
    }
}
$needleCode = Get-Content -LiteralPath (Join-Path $projectRoot 'Content\Projectiles\HakuNeedle.cs') -Raw
if ($needleCode -match 'MagicPixel' -or $needleCode -notmatch 'HakuNeedleV2') {
    throw 'Haku needle still uses a placeholder rectangle'
}
$assets = @(
    @{ Name = 'HakuPoseAtlasV2.png'; Width = 1254; Height = 1254 },
    @{ Name = 'HakuPortraitV2.png'; Width = 1254; Height = 1254 },
    @{ Name = 'HakuIceMirrorV2.png'; Width = 1024; Height = 1536 },
    @{ Name = 'HakuBoss_Head_Boss.png'; Width = 40; Height = 40 }
)

foreach ($asset in $assets) {
    $path = Join-Path $root $asset.Name
    if (-not (Test-Path -LiteralPath $path)) { throw "Missing Haku art: $path" }
    $bitmap = [System.Drawing.Bitmap]::new((Resolve-Path -LiteralPath $path).Path)
    try {
        if ($bitmap.Width -ne $asset.Width -or $bitmap.Height -ne $asset.Height) {
            throw "Wrong Haku art dimensions: $($asset.Name)"
        }
        $visible = $false
        for ($y = 0; $y -lt $bitmap.Height -and -not $visible; $y += 16) {
            for ($x = 0; $x -lt $bitmap.Width; $x += 16) {
                if ($bitmap.GetPixel($x, $y).A -gt 200) { $visible = $true; break }
            }
        }
        if (-not $visible) { throw "Empty Haku art: $($asset.Name)" }
        Write-Output "PASS $($asset.Name) $($bitmap.Width)x$($bitmap.Height)"
    }
    finally { $bitmap.Dispose() }
}

$needlePath = Join-Path $projectRoot 'Content\Projectiles\HakuNeedleV2.png'
if (-not (Test-Path -LiteralPath $needlePath)) { throw "Missing Haku needle art: $needlePath" }
$needle = [System.Drawing.Bitmap]::new((Resolve-Path -LiteralPath $needlePath).Path)
try {
    if ($needle.Width -ne 2172 -or $needle.Height -ne 724 -or
        $needle.GetPixel(0, 0).A -ne 0 -or $needle.GetPixel(1000, 350).A -lt 200) {
        throw 'Haku needle art is malformed'
    }
    Write-Output "PASS HakuNeedleV2.png $($needle.Width)x$($needle.Height)"
}
finally { $needle.Dispose() }
