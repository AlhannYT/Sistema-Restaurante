<?php
require __DIR__ . '/conexion_gloria.php';

$token = $_GET['token'] ?? '';
if ($token === '') {
    die('Token no proporcionado.');
}

$sql = "
SELECT
    l.IdLinkResena,
    l.IdPedido,
    l.Usado,
    l.Expirado,
    p.NombreCliente,
    p.ZonaEntrega,
    p.VehiculoAsignado,
    p.MotivoAsignacion,
    r.Nombre AS NombreRepartidor
FROM dbo.LinkResenaDelivery l
INNER JOIN dbo.Pedido p ON l.IdPedido = p.IdPedido
LEFT JOIN dbo.Repartidor r ON p.IdRepartidor = r.IdEmpleado
WHERE l.Token = ?
";

$stmt = sqlsrv_query($conn, $sql, [$token]);
if ($stmt === false) {
    echo "<pre>";
    print_r(sqlsrv_errors());
    echo "</pre>";
    exit;
}

$data = sqlsrv_fetch_array($stmt, SQLSRV_FETCH_ASSOC);

if (!$data) {
    die('El enlace no es válido.');
}
if ((int)$data['Usado'] === 1) {
    die('Este enlace de reseña ya fue utilizado.');
}
if ((int)$data['Expirado'] === 1) {
    die('Este enlace de reseña ha expirado.');
}
?>
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>Valora tu entrega</title>
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet">
  <link href="https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css" rel="stylesheet">
  <style>
    body { background: #f6f7fb; }
    .page-wrap { max-width: 900px; margin: 32px auto; }
    .card { border-radius: 14px; }
    .form-title { font-weight: 600; }
    .info-box {
      background: #f8f9fa;
      border-radius: 12px;
      padding: 16px;
    }
    
    /* Contenedor de estrellas */
    .stars {
      display: inline-flex;
      flex-direction: row-reverse;
      position: relative;
    }
    .stars input {
      position: absolute;
      opacity: 0;
      width: 1px;
      height: 1px;
      pointer-events: none;
    }
    .stars label {
      font-size: 2.2rem;
      color: #d1d5db;
      cursor: pointer;
      padding: 0 4px;
      transition: color 0.15s ease, transform 0.15s ease;
    }
    .stars label:hover,
    .stars label:hover ~ label,
    .stars input:checked ~ label {
      color: #f59e0b;
    }
    .stars label:hover {
      transform: scale(1.15);
    }
    .rating-hint {
      font-size: 0.95rem;
      font-weight: 500;
      min-height: 24px;
    }
    .req-badge {
      font-size: 0.85rem;
      color: #6c757d;
    }
  </style>
</head>
<body>
  <div class="page-wrap px-3">
    <div class="d-flex align-items-center justify-content-between mb-3">
      <h3 class="m-0">
        <i class='bx bx-star me-2 text-warning'></i>Valora tu entrega
      </h3>
      <span class="req-badge"><span class="text-danger">*</span> Campos obligatorios</span>
    </div>
    
    <div class="row g-3">
      <div class="col-12">
        <div class="card shadow-sm border-0">
          <div class="card-body p-4">
            <h6 class="form-title mb-3">Información del pedido</h6>
            
            <div class="info-box mb-4">
              <div class="row g-3">
                <div class="col-md-6">
                  <strong>Pedido:</strong> #<?= htmlspecialchars($data['IdPedido']) ?>
                </div>
                <div class="col-md-6">
                  <strong>Cliente:</strong> <?= htmlspecialchars($data['NombreCliente']) ?>
                </div>
                <div class="col-md-6">
                  <strong>Repartidor:</strong> <?= htmlspecialchars($data['NombreRepartidor'] ?? 'No especificado') ?>
                </div>
                <div class="col-md-6">
                  <strong>Vehículo:</strong> <?= htmlspecialchars($data['VehiculoAsignado'] ?? 'No especificado') ?>
                </div>
                <div class="col-md-6">
                  <strong>Zona:</strong> <?= htmlspecialchars($data['ZonaEntrega'] ?? 'No especificada') ?>
                </div>
                <div class="col-md-6">
                  <strong>Motivo de asignación:</strong> <?= htmlspecialchars($data['MotivoAsignacion'] ?? 'No especificado') ?>
                </div>
              </div>
            </div>

            <form id="formResena" action="guardar_resena.php" method="POST">
              <input type="hidden" name="token" value="<?= htmlspecialchars($token) ?>">
              <input type="hidden" name="idpedido" value="<?= htmlspecialchars($data['IdPedido']) ?>">
              <input type="hidden" name="idlinkresena" value="<?= htmlspecialchars($data['IdLinkResena']) ?>">

              <!-- Puntuación General -->
              <div class="mb-4">
                <label class="form-label fw-bold d-block">
                  Puntuación general <span class="text-danger">*</span>
                </label>
                <div class="d-flex align-items-center gap-3">
                  <div class="stars">
                    <input type="radio" id="star5" name="puntuacion" value="5" data-texto="5 estrellas - ¡Excelente servicio!">
                    <label for="star5" title="5 estrellas"><i class='bx bxs-star'></i></label>

                    <input type="radio" id="star4" name="puntuacion" value="4" data-texto="4 estrellas - Muy buen servicio">
                    <label for="star4" title="4 estrellas"><i class='bx bxs-star'></i></label>

                    <input type="radio" id="star3" name="puntuacion" value="3" data-texto="3 estrellas - Servicio regular">
                    <label for="star3" title="3 estrellas"><i class='bx bxs-star'></i></label>

                    <input type="radio" id="star2" name="puntuacion" value="2" data-texto="2 estrellas - Mal servicio">
                    <label for="star2" title="2 estrellas"><i class='bx bxs-star'></i></label>

                    <input type="radio" id="star1" name="puntuacion" value="1" data-texto="1 estrella - Muy mal servicio">
                    <label for="star1" title="1 estrella"><i class='bx bxs-star'></i></label>
                  </div>
                  <span id="puntuacionTexto" class="rating-hint text-muted">Selecciona una calificación</span>
                </div>
                <div id="errorPuntuacion" class="text-danger small mt-1 d-none">
                  <i class='bx bx-error-circle'></i> Por favor selecciona una puntuación de 1 a 5 estrellas.
                </div>
              </div>

              <!-- Criterios de Evaluación -->
              <div class="row g-3 mb-4">
                <div class="col-md-4">
                  <label for="calidad" class="form-label fw-semibold">
                    Calidad <span class="text-danger">*</span>
                  </label>
                  <select class="form-select" id="calidad" name="calidad" required>
                    <option value="">Seleccione una opción</option>
                    <option value="1">1 - Muy mala</option>
                    <option value="2">2 - Mala</option>
                    <option value="3">3 - Regular</option>
                    <option value="4">4 - Buena</option>
                    <option value="5">5 - Excelente</option>
                  </select>
                </div>
                <div class="col-md-4">
                  <label for="amabilidad" class="form-label fw-semibold">
                    Amabilidad <span class="text-danger">*</span>
                  </label>
                  <select class="form-select" id="amabilidad" name="amabilidad" required>
                    <option value="">Seleccione una opción</option>
                    <option value="1">1 - Muy mala</option>
                    <option value="2">2 - Mala</option>
                    <option value="3">3 - Regular</option>
                    <option value="4">4 - Buena</option>
                    <option value="5">5 - Excelente</option>
                  </select>
                </div>
                <div class="col-md-4">
                  <label for="puntualidad" class="form-label fw-semibold">
                    Puntualidad <span class="text-danger">*</span>
                  </label>
                  <select class="form-select" id="puntualidad" name="puntualidad" required>
                    <option value="">Seleccione una opción</option>
                    <option value="1">1 - Muy mala</option>
                    <option value="2">2 - Mala</option>
                    <option value="3">3 - Regular</option>
                    <option value="4">4 - Buena</option>
                    <option value="5">5 - Excelente</option>
                  </select>
                </div>
              </div>

              <!-- Comentario -->
              <div class="mb-4">
                <label for="comentario" class="form-label fw-semibold">
                  Comentario <span class="text-muted fw-normal">(Opcional)</span>
                </label>
                <textarea class="form-control" id="comentario" name="comentario" rows="4"
                  placeholder="Cuéntanos cómo fue tu experiencia con la entrega..."></textarea>
              </div>

              <!-- Botón de Envío -->
              <div class="d-flex justify-content-end gap-2 mt-4">
                <button type="submit" class="btn btn-primary px-4 py-2 fw-semibold">
                  <i class='bx bx-send me-1'></i> Enviar reseña
                </button>
              </div>
            </form>

          </div>
        </div>
      </div>
    </div>
  </div>

  <script>
    const form = document.getElementById('formResena');
    const starInputs = document.querySelectorAll('input[name="puntuacion"]');
    const labelTexto = document.getElementById('puntuacionTexto');
    const errorPuntuacion = document.getElementById('errorPuntuacion');

    // Actualizar texto al seleccionar estrellas
    starInputs.forEach(input => {
      input.addEventListener('change', () => {
        labelTexto.textContent = input.getAttribute('data-texto');
        labelTexto.className = 'rating-hint text-warning fw-bold';
        errorPuntuacion.classList.add('d-none');
      });
    });

    // Validar antes de enviar que se haya elegido al menos 1 estrella
    form.addEventListener('submit', function (e) {
      const estrellaSeleccionada = document.querySelector('input[name="puntuacion"]:checked');
      if (!estrellaSeleccionada) {
        e.preventDefault();
        errorPuntuacion.classList.remove('d-none');
        document.querySelector('.stars').scrollIntoView({ behavior: 'smooth', block: 'center' });
      }
    });
  </script>
</body>
</html>