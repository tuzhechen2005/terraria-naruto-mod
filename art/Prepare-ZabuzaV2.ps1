Add-Type -AssemblyName System.Drawing

$sourceRoot = Join-Path $PSScriptRoot 'zabuza-v2'
$outputRoot = Join-Path $PSScriptRoot '..\ShinobiPrototype\Content\NPCs'
$projectileOutputRoot = Join-Path $PSScriptRoot '..\ShinobiPrototype\Content\Projectiles'
$poses = @(
    @{ Source = 'idle-source.png'; Output = 'ZabuzaIdleV2.png' },
    @{ Source = 'windup-source.png'; Output = 'ZabuzaWindupV2.png' },
    @{ Source = 'slash-source.png'; Output = 'ZabuzaSlashV2.png' },
    @{ Source = 'seal-source.png'; Output = 'ZabuzaSealV2.png' },
    @{ Source = 'run-source.png'; Output = 'ZabuzaRunV2.png' },
    @{ Source = 'leap-source.png'; Output = 'ZabuzaLeapV2.png' },
    @{ Source = 'water-dragon-source.png'; Output = 'ZabuzaWaterDragon.png'; Projectile = $true; MaxWidth = 72; MaxHeight = 40 }
)

foreach ($pose in $poses) {
    $inputPath = Join-Path $sourceRoot $pose.Source
    $destinationRoot = if ($pose.Projectile) { $projectileOutputRoot } else { $outputRoot }
    $outputPath = Join-Path $destinationRoot $pose.Output
    if (Test-Path -LiteralPath $outputPath) {
        Write-Output "Keeping existing $($pose.Output)"
        continue
    }

    $source = [System.Drawing.Bitmap]::new($inputPath)
    try {
        $left = $source.Width
        $top = $source.Height
        $right = -1
        $bottom = -1
        for ($y = 0; $y -lt $source.Height; $y++) {
            for ($x = 0; $x -lt $source.Width; $x++) {
                if ($source.GetPixel($x, $y).A -gt 32) {
                    $left = [Math]::Min($left, $x)
                    $top = [Math]::Min($top, $y)
                    $right = [Math]::Max($right, $x)
                    $bottom = [Math]::Max($bottom, $y)
                }
            }
        }
        if ($right -lt $left) { throw "No visible pixels in $inputPath" }

        $width = $right - $left + 1
        $height = $bottom - $top + 1
        $maxWidth = if ($pose.MaxWidth) { $pose.MaxWidth } else { 100 }
        $maxHeight = if ($pose.MaxHeight) { $pose.MaxHeight } else { 88 }
        $scale = [Math]::Min($maxWidth / $width, $maxHeight / $height)
        $drawWidth = [Math]::Max(1, [int][Math]::Round($width * $scale))
        $drawHeight = [Math]::Max(1, [int][Math]::Round($height * $scale))
        $result = [System.Drawing.Bitmap]::new($drawWidth + 4, $drawHeight + 4,
            [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
        try {
            $graphics = [System.Drawing.Graphics]::FromImage($result)
            try {
                $graphics.Clear([System.Drawing.Color]::Transparent)
                $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
                $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::Half
                $graphics.DrawImage($source,
                    [System.Drawing.Rectangle]::new(2, 2, $drawWidth, $drawHeight),
                    [System.Drawing.Rectangle]::new($left, $top, $width, $height),
                    [System.Drawing.GraphicsUnit]::Pixel)
            }
            finally { $graphics.Dispose() }
            $result.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
            Write-Output "$($pose.Output): $($result.Width)x$($result.Height)"
        }
        finally { $result.Dispose() }
    }
    finally { $source.Dispose() }
}
