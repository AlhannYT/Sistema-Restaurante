using System.IO;
using System.Linq;
using System.Collections.Generic;

public static class ConexionBD
{
    private static string rutaArchivo = @"C:\SistemaArchivos\Conexion\ConexionesSQL.txt";

    //private static string BaseDeDatos = "GloriaRestaurant";

    private static Dictionary<string, string> datosConexion = new Dictionary<string, string>();

    private static void LeerArchivo()
    {
        if (!File.Exists(rutaArchivo))
            throw new FileNotFoundException("No se encontró el archivo de conexión.");

        datosConexion.Clear();

        var lineas = File.ReadAllLines(rutaArchivo);

        var lineaDefecto = lineas.Reverse().FirstOrDefault(l => l.Split('|').Length >= 5 && l.Split('|')[4] == "1");

        if (lineaDefecto == null)
        {
            MessageBox.Show("No se encontró una conexion por defecto.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
        var partes = lineaDefecto.Split('|');

        datosConexion["Servidor"] = partes[0];
        datosConexion["BaseDeDatos"] = partes[1];
        datosConexion["Usuario"] = partes[2];
        datosConexion["Contrasena"] = partes[3];
    }

    public static string ConexionSQL()
    {
        LeerArchivo();

        string servidor = datosConexion.ContainsKey("Servidor") ? datosConexion["Servidor"] : "";
        string baseDeDatos = datosConexion.ContainsKey("BaseDeDatos") ? datosConexion["BaseDeDatos"] : "";
        string usuario = datosConexion.ContainsKey("Usuario") ? datosConexion["Usuario"] : "";
        string contrasena = datosConexion.ContainsKey("Contrasena") ? datosConexion["Contrasena"] : "";

        return $"Server={servidor};Database={baseDeDatos};User Id={usuario};Password={contrasena};";
    }
}
