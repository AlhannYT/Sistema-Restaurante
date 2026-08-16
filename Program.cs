namespace Proyecto_restaurante
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Inicializa la configuración visual y DPI de la aplicación
            ApplicationConfiguration.Initialize();

            // 1. Iniciar el servidor PHP en segundo plano
            ServidorWebManager.Iniciar();

            try
            {
                // 2. Ejecutar tu formulario inicial
                Application.Run(new inicio());
            }
            finally
            {
                // 3. Al cerrar la aplicación por completo, apagar el proceso PHP
                ServidorWebManager.Detener();
            }
        }
    }
}