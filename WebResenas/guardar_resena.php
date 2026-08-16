<?php
require __DIR__ . '/conexion_gloria.php';

$token        = $_POST['token'] ?? '';
$idpedido     = $_POST['idpedido'] ?? '';
$idlinkresena = $_POST['idlinkresena'] ?? '';
$puntuacion   = $_POST['puntuacion'] ?? '';
$comentario   = $_POST['comentario'] ?? '';
$calidad      = $_POST['calidad'] ?? '';
$amabilidad   = $_POST['amabilidad'] ?? '';
$puntualidad  = $_POST['puntualidad'] ?? '';

if (
    $token === '' ||
    $idpedido === '' ||
    $idlinkresena === '' ||
    $puntuacion === '' ||
    $calidad === '' ||
    $amabilidad === '' ||
    $puntualidad === ''
) {
    die('Faltan datos obligatorios.');
}

/* 1. Validar el enlace */
$sqlValidar = "
SELECT Usado, Expirado
FROM dbo.LinkResenaDelivery
WHERE IdLinkResena = ? AND Token = ?
";
$stmtValidar = sqlsrv_query($conn, $sqlValidar, [$idlinkresena, $token]);
if ($stmtValidar === false) {
    echo "<pre>";
    print_r(sqlsrv_errors());
    echo "</pre>";
    exit;
}

$link = sqlsrv_fetch_array($stmtValidar, SQLSRV_FETCH_ASSOC);
if (!$link) {
    die('Enlace no válido.');
}
if ((int)$link['Usado'] === 1) {
    die('Este enlace ya fue utilizado.');
}
if ((int)$link['Expirado'] === 1) {
    die('Este enlace ha expirado.');
}

/* Iniciar Transacción para asegurar consistencia */
sqlsrv_begin_transaction($conn);

/* 2. Guardar la reseña */
$sqlInsert = "
INSERT INTO dbo.ResenaDelivery
(IdPedido, Puntuacion, Comentario, IdLinkResena, Calidad, Amabilidad, Puntualidad)
VALUES (?, ?, ?, ?, ?, ?, ?)
";
$stmtInsert = sqlsrv_query($conn, $sqlInsert, [
    $idpedido,
    $puntuacion,
    $comentario,
    $idlinkresena,
    $calidad,
    $amabilidad,
    $puntualidad
]);

/* 3. Marcar el link como usado */
$sqlUpdate = "
UPDATE dbo.LinkResenaDelivery
SET Usado = 1,
    FechaUso = SYSDATETIME()
WHERE IdLinkResena = ?
";
$stmtUpdate = sqlsrv_query($conn, $sqlUpdate, [$idlinkresena]);

/* 4. Cambiar estado del delivery a Reseñado */
$sqlPedido = "
UPDATE dbo.Pedido
SET EstadoDelivery = 'Reseñado'
WHERE IdPedido = ?
";
$stmtPedido = sqlsrv_query($conn, $sqlPedido, [$idpedido]);

if ($stmtInsert && $stmtUpdate && $stmtPedido) {
    sqlsrv_commit($conn);
} else {
    sqlsrv_rollback($conn);
    echo "<pre>";
    print_r(sqlsrv_errors());
    echo "</pre>";
    exit;
}

/* 5. Liberar y cerrar */
if ($stmtValidar) sqlsrv_free_stmt($stmtValidar);
if ($stmtInsert) sqlsrv_free_stmt($stmtInsert);
if ($stmtUpdate) sqlsrv_free_stmt($stmtUpdate);
if ($stmtPedido) sqlsrv_free_stmt($stmtPedido);
sqlsrv_close($conn);

/* 6. Redirigir */
header('Location: gracias.php');
exit;
?>