@echo off
chcp 65001 >nul
title Actualizador de Version - PDVRestaurant
cls
echo ========================================================
echo        ACTUALIZADOR DE VERSION - PDVRESTAURANT
echo ========================================================
echo.
set /p NUEVA_VERSION="Escribir nueva version: "

if "%NUEVA_VERSION%"=="" (
    echo.
    echo [ERROR] No se ingreso ninguna version. Operacion cancelada.
    echo.
    pause
    exit /b
)

echo.
echo Actualizando archivos a la version %NUEVA_VERSION%...
echo.

powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "$v = '%NUEVA_VERSION%'.Trim(); " ^
    "$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path; " ^
    "if (-not $scriptDir) { $scriptDir = (Get-Location).Path }; " ^
    "$csprojPath = Join-Path $scriptDir 'Proyecto restaurante.csproj'; " ^
    "$xmlPath = Join-Path $scriptDir 'Version\version.xml'; " ^
    "if (Test-Path $csprojPath) { " ^
    "    $content = Get-Content $csprojPath -Raw; " ^
    "    $content = $content -replace '<Version>[^<]*</Version>', ('<Version>' + $v + '</Version>'); " ^
    "    $content = $content -replace '<AssemblyVersion>[^<]*</AssemblyVersion>', ('<AssemblyVersion>' + $v + '.0</AssemblyVersion>'); " ^
    "    $content = $content -replace '<FileVersion>[^<]*</FileVersion>', ('<FileVersion>' + $v + '.0</FileVersion>'); " ^
    "    [System.IO.File]::WriteAllText($csprojPath, $content, [System.Text.Encoding]::UTF8); " ^
    "    Write-Host '  [OK] Proyecto restaurante.csproj actualizado a version' $v; " ^
    "} else { " ^
    "    Write-Host '  [ERROR] No se encontro Proyecto restaurante.csproj' -ForegroundColor Red; " ^
    "}; " ^
    "if (Test-Path (Split-Path $xmlPath)) { " ^
    "    $xmlContent = '<?xml version=\"1.0\" encoding=\"utf-8\" ?>' + [Environment]::NewLine + " ^
    "                  '<item>' + [Environment]::NewLine + " ^
    "                  '    <version>' + $v + '</version>' + [Environment]::NewLine + " ^
    "                  '    <url>https://github.com/AlhannYT/Sistema-Restaurante/releases/download/v' + $v + '/PDVRestaurant.Setup_v' + $v + '.exe</url>' + [Environment]::NewLine + " ^
    "                  '    <changelog>https://github.com/AlhannYT/Sistema-Restaurante/releases/tag/v' + $v + '</changelog>' + [Environment]::NewLine + " ^
    "                  '    <mandatory>true</mandatory>' + [Environment]::NewLine + " ^
    "                  '</item>'; " ^
    "    [System.IO.File]::WriteAllText($xmlPath, $xmlContent, [System.Text.Encoding]::UTF8); " ^
    "    Write-Host '  [OK] Version\version.xml actualizado a version' $v; " ^
    "} else { " ^
    "    Write-Host '  [ERROR] No se encontro la carpeta Version' -ForegroundColor Red; " ^
    "};"

echo.
echo ========================================================
echo   [EXITO] Version %NUEVA_VERSION% aplicada correctamente.
echo ========================================================
echo.
echo Proximos pasos:
echo  1. Compilar/Publicar en Visual Studio.
echo  2. Generar el Setup en Inno Setup (ej: PDVRestaurant.Setup_v%NUEVA_VERSION%.exe).
echo  3. Subir el release v%NUEVA_VERSION% a GitHub junto con el XML actualizado.
echo.
pause
