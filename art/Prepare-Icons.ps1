Add-Type -AssemblyName System.Drawing

$projectRoot = Split-Path -Parent $PSScriptRoot
$sourceRoot = Join-Path $PSScriptRoot 'source'
$contentRoot = Join-Path $projectRoot 'ShinobiPrototype\Content'
$icons = @(
    @{ Source = 'profile-scroll.png'; Target = 'Items\NinjaProfileScroll.png'; CanvasWidth = 40; CanvasHeight = 40; MaxWidth = 32; MaxHeight = 32 },
    @{ Source = 'training-kunai.png'; Target = 'Items\TrainingKunai.png'; CanvasWidth = 40; CanvasHeight = 40; MaxWidth = 32; MaxHeight = 32 },
    @{ Source = 'ninjutsu-manual.png'; Target = 'Items\NinjutsuManual.png'; CanvasWidth = 40; CanvasHeight = 40; MaxWidth = 32; MaxHeight = 32 },
    @{ Source = 'mission-scroll.png'; Target = 'Items\MissionScroll.png'; CanvasWidth = 40; CanvasHeight = 40; MaxWidth = 32; MaxHeight = 32 },
    @{ Source = 'mist-insignia.png'; Target = 'Items\MistInsignia.png'; CanvasWidth = 40; CanvasHeight = 40; MaxWidth = 32; MaxHeight = 32 },
    @{ Source = 'wave-medal.png'; Target = 'Items\WaveCountryMedal.png'; CanvasWidth = 40; CanvasHeight = 40; MaxWidth = 32; MaxHeight = 32 },
    @{ Source = 'chakra-technique.png'; Target = 'Items\ChakraPalm.png'; CanvasWidth = 40; CanvasHeight = 40; MaxWidth = 32; MaxHeight = 32 },
    @{ Source = 'exam-admission.png'; Target = 'Items\ExamAdmissionScroll.png'; CanvasWidth = 40; CanvasHeight = 40; MaxWidth = 32; MaxHeight = 32 },
    @{ Source = 'heaven-scroll.png'; Target = 'Items\HeavenScroll.png'; CanvasWidth = 40; CanvasHeight = 40; MaxWidth = 32; MaxHeight = 32 },
    @{ Source = 'earth-scroll.png'; Target = 'Items\EarthScroll.png'; CanvasWidth = 40; CanvasHeight = 40; MaxWidth = 32; MaxHeight = 32 },
    @{ Source = 'chunin-headband.png'; Target = 'Items\ChuninHeadband.png'; CanvasWidth = 40; CanvasHeight = 40; MaxWidth = 32; MaxHeight = 32 },
    @{ Source = 'mist-scout.png'; Target = 'NPCs\MistScout.png'; CanvasWidth = 36; CanvasHeight = 52; MaxWidth = 34; MaxHeight = 50 },
    @{ Source = 'haku.png'; Target = 'NPCs\HakuBoss.png'; CanvasWidth = 36; CanvasHeight = 52; MaxWidth = 34; MaxHeight = 50 },
    @{ Source = 'zabuza.png'; Target = 'NPCs\ZabuzaBoss.png'; CanvasWidth = 40; CanvasHeight = 56; MaxWidth = 38; MaxHeight = 54 },
    @{ Source = 'forest-candidate.png'; Target = 'NPCs\ForestExamCandidate.png'; CanvasWidth = 36; CanvasHeight = 52; MaxWidth = 34; MaxHeight = 50 },
    @{ Source = 'arena-rival.png'; Target = 'NPCs\ArenaRivalBoss.png'; CanvasWidth = 40; CanvasHeight = 56; MaxWidth = 38; MaxHeight = 54 }
)

foreach ($icon in $icons) {
    $sourcePath = Join-Path $sourceRoot $icon.Source
    $targetPath = Join-Path $contentRoot $icon.Target
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
        if ($right -lt 0) {
            throw "No visible pixels found: $sourcePath"
        }

        $width = $right - $left + 1
        $height = $bottom - $top + 1
        $scale = [Math]::Min($icon.MaxWidth / $width, $icon.MaxHeight / $height)
        $drawWidth = [int][Math]::Round($width * $scale)
        $drawHeight = [int][Math]::Round($height * $scale)
        $output = [System.Drawing.Bitmap]::new($icon.CanvasWidth, $icon.CanvasHeight, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
        try {
            $graphics = [System.Drawing.Graphics]::FromImage($output)
            try {
                $graphics.Clear([System.Drawing.Color]::Transparent)
                $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
                $graphics.DrawImage($source,
                    [System.Drawing.Rectangle]::new([int](($icon.CanvasWidth - $drawWidth) / 2), [int](($icon.CanvasHeight - $drawHeight) / 2), $drawWidth, $drawHeight),
                    [System.Drawing.Rectangle]::new($left, $top, $width, $height),
                    [System.Drawing.GraphicsUnit]::Pixel)
            } finally {
                $graphics.Dispose()
            }
            $output.Save($targetPath, [System.Drawing.Imaging.ImageFormat]::Png)
        } finally {
            $output.Dispose()
        }
        Write-Output "Prepared $targetPath"
    } finally {
        $source.Dispose()
    }
}
