Add-Type -AssemblyName System.Drawing

$npcRoot = Join-Path $PSScriptRoot '..\ShinobiPrototype\Content\NPCs'
$projectileRoot = Join-Path $PSScriptRoot '..\ShinobiPrototype\Content\Projectiles'
$assets = @(
    @{ Path = (Join-Path $npcRoot 'ZabuzaIdleV2.png'); MinHeight = 88; MaxWidth = 104 },
    @{ Path = (Join-Path $npcRoot 'ZabuzaRunV2.png'); MinHeight = 88; MaxWidth = 104 },
    @{ Path = (Join-Path $npcRoot 'ZabuzaLeapV2.png'); MinHeight = 88; MaxWidth = 104 },
    @{ Path = (Join-Path $npcRoot 'ZabuzaWindupV2.png'); MinHeight = 88; MaxWidth = 104 },
    @{ Path = (Join-Path $npcRoot 'ZabuzaSlashV2.png'); MinHeight = 88; MaxWidth = 104 },
    @{ Path = (Join-Path $npcRoot 'ZabuzaSealV2.png'); MinHeight = 88; MaxWidth = 104 },
    @{ Path = (Join-Path $npcRoot 'ZabuzaRunMidV4.png'); MinHeight = 88; MaxWidth = 104 },
    @{ Path = (Join-Path $npcRoot 'ZabuzaRunAltV4.png'); MinHeight = 88; MaxWidth = 104 },
    @{ Path = (Join-Path $npcRoot 'ZabuzaPortraitV4.png'); MinHeight = 40; MaxWidth = 48 },
    @{ Path = (Join-Path $npcRoot 'ZabuzaPortraitDemonV4.png'); MinHeight = 40; MaxWidth = 48 },
    @{ Path = (Join-Path $npcRoot 'ZabuzaBoss_Head_Boss.png'); MinHeight = 36; MaxWidth = 44 },
    @{ Path = (Join-Path $npcRoot 'ZabuzaBoss_Head_Boss_SecondStage.png'); MinHeight = 36; MaxWidth = 44 },
    @{ Path = (Join-Path $projectileRoot 'ZabuzaWaterDragon.png'); MinHeight = 24; MaxWidth = 80 },
    @{ Path = (Join-Path $projectileRoot 'ZabuzaWaterNeedle.png'); MinHeight = 14; MaxWidth = 32 }
)

foreach ($asset in $assets) {
    if (-not (Test-Path -LiteralPath $asset.Path)) { throw "Missing art: $($asset.Path)" }
    $image = [System.Drawing.Bitmap]::new($asset.Path)
    try {
        if ($image.Height -lt $asset.MinHeight -or $image.Width -gt $asset.MaxWidth) {
            throw "Wrong dimensions: $($asset.Path) $($image.Width)x$($image.Height)"
        }
        foreach ($point in @(@(0, 0), @(($image.Width - 1), 0), @(0, ($image.Height - 1)), @(($image.Width - 1), ($image.Height - 1)))) {
            if ($image.GetPixel($point[0], $point[1]).A -ne 0) {
                throw "Opaque corner in $($asset.Path)"
            }
        }
        $visible = $false
        for ($y = 2; $y -lt $image.Height - 2 -and -not $visible; $y += 4) {
            for ($x = 2; $x -lt $image.Width - 2; $x += 4) {
                if ($image.GetPixel($x, $y).A -gt 200) { $visible = $true; break }
            }
        }
        if (-not $visible) { throw "No solid sprite pixels in $($asset.Path)" }
        Write-Output "PASS $([System.IO.Path]::GetFileName($asset.Path)) $($image.Width)x$($image.Height)"
    }
    finally { $image.Dispose() }
}
