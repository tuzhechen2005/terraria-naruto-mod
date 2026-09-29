Add-Type -AssemblyName System.Drawing

$sourceRoot = Join-Path $PSScriptRoot 'zabuza-v4'
$npcRoot = Join-Path $PSScriptRoot '..\ShinobiPrototype\Content\NPCs'
$assets = @(
    @{ Source='run-mid-source.png'; Output='ZabuzaRunMidV4.png'; MaxWidth=100; MaxHeight=88 },
    @{ Source='run-alt-source.png'; Output='ZabuzaRunAltV4.png'; MaxWidth=100; MaxHeight=88 },
    @{ Source='portrait-source.png'; Output='ZabuzaPortraitV4.png'; MaxWidth=40; MaxHeight=40 },
    @{ Source='portrait-source.png'; Output='ZabuzaBoss_Head_Boss.png'; MaxWidth=36; MaxHeight=36 },
    @{ Source='portrait-demon-source.png'; Output='ZabuzaPortraitDemonV4.png'; MaxWidth=40; MaxHeight=40 },
    @{ Source='portrait-demon-source.png'; Output='ZabuzaBoss_Head_Boss_SecondStage.png'; MaxWidth=36; MaxHeight=36 }
)

foreach ($asset in $assets) {
    $inputPath = Join-Path $sourceRoot $asset.Source
    $outputPath = Join-Path $npcRoot $asset.Output
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
        if ($right -lt $left) { throw "No visible pixels: $inputPath" }
        $sourceWidth = $right - $left + 1; $sourceHeight = $bottom - $top + 1
        $scale = [Math]::Min($asset.MaxWidth / $sourceWidth, $asset.MaxHeight / $sourceHeight)
        $drawWidth = [Math]::Max(1, [int][Math]::Round($sourceWidth * $scale))
        $drawHeight = [Math]::Max(1, [int][Math]::Round($sourceHeight * $scale))
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
                    [System.Drawing.Rectangle]::new($left, $top, $sourceWidth, $sourceHeight),
                    [System.Drawing.GraphicsUnit]::Pixel)
            }
            finally { $graphics.Dispose() }
            $result.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
            Write-Output "$($asset.Output): $($result.Width)x$($result.Height)"
        }
        finally { $result.Dispose() }
    }
    finally { $source.Dispose() }
}
