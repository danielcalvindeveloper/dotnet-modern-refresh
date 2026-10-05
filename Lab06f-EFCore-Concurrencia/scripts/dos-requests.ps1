param(
    [ValidateSet('sin-control', 'con-control')]
    [string] $Modo = 'sin-control',
    [int] $TurnoId = 1,
    [string] $BaseUrl = 'http://localhost:5093'
)

$ErrorActionPreference = 'Stop'
$uri = "$BaseUrl/api/concurrencia/reservar-$Modo/$TurnoId"
# Dar tiempo a que arranquen ambos procesos; esta coordinación es solo del cliente.
$inicio = [DateTime]::UtcNow.AddSeconds(8)
$jobs = @()
try {
    foreach ($operacion in @('A', 'B')) {
        $jobs += Start-Job -ArgumentList $uri, $operacion, $inicio -ScriptBlock {
            param($uri, $operacion, $inicio)
            # Windows PowerShell debe decodificar el JSON UTF-8 de curl correctamente.
            [Console]::OutputEncoding = [Text.UTF8Encoding]::new($false)
            while ([DateTime]::UtcNow -lt $inicio) { Start-Sleep -Milliseconds 50 }
            $response = curl.exe -sS -i -X POST -H "X-Operacion: $operacion" $uri
            if ($LASTEXITCODE -ne 0) { throw "[$operacion] Error de transporte curl: $LASTEXITCODE" }
            $text = $response -join "`n"
            [pscustomobject]@{
                Operacion = $operacion
                HTTP = [int][regex]::Match($text, '^HTTP/1\.1 (\d{3})').Groups[1].Value
                Cuerpo = [regex]::Match($text, '(?s)\r?\n\r?\n(.*)$').Groups[1].Value.Trim()
            }
        }
    }
    $jobs | Wait-Job | Out-Null
    $jobs | Receive-Job
}
finally {
    $jobs | Remove-Job -Force
}
