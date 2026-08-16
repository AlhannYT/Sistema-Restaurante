<?php
$rutaArchivo = "C:\\SistemaArchivos\\Conexion\\ConexionesSQL.txt";

if (!file_exists($rutaArchivo)) {
    die("Error: No se encontró el archivo de conexión en: " . $rutaArchivo);
}

$lineas = file($rutaArchivo, FILE_IGNORE_NEW_LINES | FILE_SKIP_EMPTY_LINES);
$lineaDefecto = null;

for ($i = count($lineas) - 1; $i >= 0; $i--) {
    $partes = explode('|', trim($lineas[$i]));
    if (count($partes) >= 5 && trim($partes[4]) === '1') {
        $lineaDefecto = $partes;
        break;
    }
}

if ($lineaDefecto === null) {
    die("Error: No se encontró una conexión por defecto en el archivo.");
}

$serverName = $lineaDefecto[0];
$connectionOptions = array(
    "Database"               => $lineaDefecto[1],
    "UID"                    => $lineaDefecto[2],
    "PWD"                    => $lineaDefecto[3],
    "CharacterSet"           => "UTF-8",
    "TrustServerCertificate" => true
);

$conn = sqlsrv_connect($serverName, $connectionOptions);

if ($conn === false) {
    echo "<pre>";
    print_r(sqlsrv_errors());
    echo "</pre>";
    exit;
}
?>