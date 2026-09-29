Add-Type -AssemblyName System.Drawing

$inputPath = Join-Path $PSScriptRoot 'zabuza-v3\water-needle-source.png'
$outputPath = Join-Path $PSScriptRoot '..\ShinobiPrototype\Content\Projectiles\ZabuzaWaterNeedle.png'
if (Test-Path -LiteralPath $outputPath) { throw "Output already exists: $outputPath" }

$source = [System.Drawing.Bitmap]::new($inputPath)
try {
    $left = $source.Width; $top = $source.Height; $right = -1; $bottom = -1
    for ($y = 0; $y -lt $source.Height; $y++) {
        for ($x = 0; $x -lt $source.Width; $x++) {
            if ($source.GetPixel($x, $y).A -gt 32) {
                $left = [Math]::Min($left, $x); $top = [Math]::Min($top, $y)
                $right = [Math]::Max($right, $x); $bottom = [Math]::Max($bottom, $y)
            }
        }
    }
    if ($right -lt $left) { throw 'No visible needle pixels' }
    $result = [System.Drawing.Bitmap]::new(32, 14, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    try {
        $graphics = [System.Drawing.Graphics]::FromImage($result)
        try {
            $graphics.Clear([System.Drawing.Color]::Transparent)
            $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
            $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::Half
            $graphics.DrawImage($source,
                [System.Drawing.Rectangle]::new(2, 2, 28, 10),
                [System.Drawing.Rectangle]::new($left, $top, $right - $left + 1, $bottom - $top + 1),
                [System.Drawing.GraphicsUnit]::Pixel)
        }
        finally { $graphics.Dispose() }
        $result.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
        Write-Output "Created $outputPath (32x14)"
    }
    finally { $result.Dispose() }
}
finally { $source.Dispose() }
