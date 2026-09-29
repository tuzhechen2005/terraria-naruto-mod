Add-Type -AssemblyName System.Drawing

$projectRoot = Split-Path -Parent $PSScriptRoot
$sourceRoot = Join-Path $PSScriptRoot 'source'
$contentRoot = Join-Path $projectRoot 'ShinobiPrototype\Content'
$poses = @(
    @{ Source = 'zabuza-slash-windup-v2.png'; Target = 'NPCs\ZabuzaSlashWindup.png'; Width = 64; Height = 64; MaxWidth = 58; MaxHeight = 55; Left = 3 },
    @{ Source = 'zabuza-water-seal-v2.png'; Target = 'NPCs\ZabuzaWaterSeal.png'; Width = 48; Height = 64; MaxWidth = 42; MaxHeight = 55; Left = 3 },
    @{ Source = 'zabuza-slash-release-v2.png'; Target = 'NPCs\ZabuzaSlashRelease.png'; Width = 120; Height = 64; MaxWidth = 114; MaxHeight = 55; Left = 3 },
    @{ Source = 'zabuza-water-wave-v2.png'; Target = 'Projectiles\ZabuzaWaterWave.png'; Width = 48; Height = 32; MaxWidth = 44; MaxHeight = 28; Left = 2 }
)

foreach ($pose in $poses) {
    $sourcePath = Join-Path $sourceRoot $pose.Source
    $targetPath = Join-Path $contentRoot $pose.Target
    $source = [System.Drawing.Bitmap]::new($sourcePath)
    try {
        $left = $source.Width
        $top = $source.Height
        $right = -1
        $bottom = -1
        for ($y = 0; $y -lt $source.Height; $y++) {
            for ($x = 0; $x -lt $source.Width; $x++) {
                if ($source.GetPixel($x, $y).A -ge 80) {
                    $left = [Math]::Min($left, $x)
                    $right = [Math]::Max($right, $x)
                    $top = [Math]::Min($top, $y)
                    $bottom = [Math]::Max($bottom, $y)
                }
            }
        }
        if ($right -lt 0) { throw "No visible pixels found: $sourcePath" }

        $cropWidth = $right - $left + 1
        $cropHeight = $bottom - $top + 1
        $scale = [Math]::Min($pose.MaxWidth / $cropWidth, $pose.MaxHeight / $cropHeight)
        $drawWidth = [Math]::Max(1, [int][Math]::Round($cropWidth * $scale))
        $drawHeight = [Math]::Max(1, [int][Math]::Round($cropHeight * $scale))
        $output = [System.Drawing.Bitmap]::new($pose.Width, $pose.Height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
        try {
            $graphics = [System.Drawing.Graphics]::FromImage($output)
            try {
                $graphics.Clear([System.Drawing.Color]::Transparent)
                $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
                $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::Half
                $graphics.DrawImage($source,
                    [System.Drawing.Rectangle]::new($pose.Left, $pose.Height - $drawHeight - 3, $drawWidth, $drawHeight),
                    [System.Drawing.Rectangle]::new($left, $top, $cropWidth, $cropHeight),
                    [System.Drawing.GraphicsUnit]::Pixel)
            } finally {
                $graphics.Dispose()
            }
            $output.Save($targetPath, [System.Drawing.Imaging.ImageFormat]::Png)
        } finally {
            $output.Dispose()
        }
        Write-Output "Prepared $targetPath ($drawWidth x $drawHeight)"
    } finally {
        $source.Dispose()
    }
}
