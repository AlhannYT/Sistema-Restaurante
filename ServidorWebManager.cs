using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Proyecto_restaurante
{
    public static class ServidorWebManager
    {
        private static Process _phpProcess;

        public static void Iniciar()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;

                // 1. Buscar la ruta real de php_runtime y WebResenas
                string localPhpExe = ObtenerRutaExistente(baseDir, "php_runtime\\php.exe");
                string webResenasPath = ObtenerRutaExistente(baseDir, "WebResenas");

                if (string.IsNullOrEmpty(localPhpExe) || !File.Exists(localPhpExe))
                {
                    MessageBox.Show(
                        $"No se encontró 'php.exe'. Asegúrate de que la carpeta 'php_runtime' exista en el proyecto.\n\nBuscado en:\n{Path.Combine(baseDir, "php_runtime\\php.exe")}",
                        "Servidor Web", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(webResenasPath) || !Directory.Exists(webResenasPath))
                {
                    MessageBox.Show(
                        $"No se encontró la carpeta 'WebResenas'.\n\nBuscado en:\n{Path.Combine(baseDir, "WebResenas")}",
                        "Servidor Web", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Iniciar el proceso apuntando al ejecutable exacto
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = localPhpExe,
                    Arguments = $"-S 0.0.0.0:8080 -t \"{webResenasPath}\"",
                    WorkingDirectory = webResenasPath,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                _phpProcess = Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo iniciar el servicio de reseñas local.\nError: " + ex.Message,
                                "Servidor Web", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string ObtenerRutaExistente(string baseDir, string rutaRelativa)
        {
            // Busca en la carpeta de ejecución (bin/Debug/...)
            string ruta1 = Path.Combine(baseDir, rutaRelativa);
            if (File.Exists(ruta1) || Directory.Exists(ruta1)) return ruta1;

            // Busca subiendo hasta la raíz del proyecto (3 niveles arriba)
            string ruta2 = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\", rutaRelativa));
            if (File.Exists(ruta2) || Directory.Exists(ruta2)) return ruta2;

            return null;
        }

        public static void Detener()
        {
            try
            {
                if (_phpProcess != null && !_phpProcess.HasExited)
                {
                    _phpProcess.Kill();
                    _phpProcess.Dispose();
                    _phpProcess = null;
                }
            }
            catch
            {
                // Ignorar excepciones al cerrar
            }
        }
    }
}