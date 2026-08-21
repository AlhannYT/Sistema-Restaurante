using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using AutoUpdaterDotNET;

namespace Proyecto_restaurante
{
    public partial class inicio : Form
    {
        public inicio()
        {
            InitializeComponent();
            this.KeyPreview = true;
        }

        private static string rutaUsuario = @"C:\SistemaArchivos\Usuarios\Usuarios.txt";
        private static string rutaActivacion = @"C:\SistemaArchivos\Configuracion\Activacion.txt";
        public string rutaArchivo = @"C:\SistemaArchivos\Conexion\ConexionesSQL.txt";
        public int idUsuario = 0;
        bool esAdmin = false;
        public string UsuarioAdmin = "A";
        public string PassAdmin = "A";
        string conexionString = ConexionBD.ConexionSQL();
        private UpdateInfoEventArgs? infoActualizacion;

        private void inicio_Load(object sender, EventArgs e)
        {
            VerificarActivacionMaquina();

            AutoUpdater.AppTitle = "PDVRestaurant";
            AutoUpdater.RunUpdateAsAdmin = true;
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            AutoUpdater.Start("https://raw.githubusercontent.com/AlhannYT/Sistema-Restaurante/master/Version/version.xml");

            string rutaBase = @"C:\SistemaArchivos";

            string rutaReportes = @"C:\SistemaArchivos\Reportes";

            string rutaConexion = Path.Combine(rutaBase, "Conexion", "ConexionesSQL.txt");

            string[] carpetas = {
                "Conexion",
                "Configuracion",
                "Facturas",
                "DGIITXT",
                "Usuarios"
            };

            string[] carpetasReportes = {
                "Ventas",
                "PlatosVendidos",
                "Stock",
                "Compras",
                "Clientes",
                "Proveedores",
                "Empleados"
            };

            try
            {
                if (!Directory.Exists(rutaBase))
                    Directory.CreateDirectory(rutaBase);

                if (!Directory.Exists(rutaReportes))
                    Directory.CreateDirectory(rutaReportes);

                foreach (string carpeta in carpetas)
                {
                    string rutaCompleta = Path.Combine(rutaBase, carpeta);
                    if (!Directory.Exists(rutaCompleta))
                        Directory.CreateDirectory(rutaCompleta);
                }

                foreach (string carpetaRep in carpetasReportes)
                {
                    string rutaRepCompleta = Path.Combine(rutaReportes, carpetaRep);
                    if (!Directory.Exists(rutaRepCompleta))
                        Directory.CreateDirectory(rutaRepCompleta);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear las carpetas requeridas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!File.Exists(rutaConexion))
            {
                iniciobtn.Enabled = false;

                AvisoDBIMG.Visible = true;

                System.Windows.Forms.Timer timerParpadeo = new System.Windows.Forms.Timer();
                timerParpadeo.Interval = 400;
                int contador = 0;

                timerParpadeo.Tick += (s, args) =>
                {
                    AvisoDBIMG.Visible = !AvisoDBIMG.Visible;
                    contador++;

                    if (contador >= 10)
                    {
                        timerParpadeo.Stop();
                        alerta.Visible = true;
                    }
                };

                timerParpadeo.Start();
            }
            else
            {
                AvisoDBIMG.Visible = false;
                iniciobtn.Enabled = true;
            }

            try
            {
                if (File.Exists(rutaUsuario))
                {
                    string usuarioGuardado = File.ReadAllText(rutaUsuario).Trim();

                    if (!string.IsNullOrEmpty(usuarioGuardado))
                    {
                        txtusuario.Text = usuarioGuardado;
                        recordarchk.Checked = true;
                        recordarchk.BackColor = Color.LightGreen;
                        txtpass.Focus();
                    }
                }
                else
                {
                    txtusuario.Text = string.Empty;
                    recordarchk.BackColor = SystemColors.Window;
                    recordarchk.Checked = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo leer el usuario recordado: " + ex.Message);
            }
        }

        private void iniciobtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtusuario.Text) || string.IsNullOrEmpty(txtpass.Text))
            {
                MessageBox.Show("Error: Campos vacíos.");
                return;
            }

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                try
                {
                    conexion.Open();

                    string queryUsuario = @"
                    SELECT IdUsuario 
                    FROM Usuario 
                    WHERE Login = @usuario AND Contrasena = @pass AND Activo = 1";

                    using (SqlCommand cmd = new SqlCommand(queryUsuario, conexion))
                    {
                        cmd.Parameters.AddWithValue("@usuario", txtusuario.Text);
                        cmd.Parameters.AddWithValue("@pass", txtpass.Text);

                        object resultado = cmd.ExecuteScalar();

                        if (resultado == null)
                        {
                            if (txtusuario.Text == UsuarioAdmin && txtpass.Text == PassAdmin)
                            {
                                idUsuario = 1;
                                resultado = 1;
                                esAdmin = true;
                            }
                            else
                            {
                                MessageBox.Show("Usuario o contrase�a incorrectos / Inactivo.");
                                return;
                            }
                        }

                        idUsuario = Convert.ToInt32(resultado);
                    }

                    string queryPermiso = @"
                    SELECT Admin 
                    FROM PermisosUsuario 
                    WHERE IdUsuario = @IdUsuario";

                    using (SqlCommand cmdPermiso = new SqlCommand(queryPermiso, conexion))
                    {
                        cmdPermiso.Parameters.AddWithValue("@IdUsuario", idUsuario);
                        object adminResult = cmdPermiso.ExecuteScalar();

                        if (adminResult != null)
                            esAdmin = Convert.ToBoolean(adminResult);
                    }

                    string nombrePC = Environment.MachineName;

                    string queryExiste = "SELECT COUNT(*) FROM Configuracion WHERE NombrePC = @NombrePC";
                    using (SqlCommand cmdExiste = new SqlCommand(queryExiste, conexion))
                    {
                        cmdExiste.Parameters.AddWithValue("@NombrePC", nombrePC);
                        int existe = (int)cmdExiste.ExecuteScalar();

                        if (existe == 0)
                        {
                            string insertarQuery = "INSERT INTO Configuracion (NombrePC, Activado) VALUES (@NombrePC, 1)";
                            using (SqlCommand cmdInsertar = new SqlCommand(insertarQuery, conexion))
                            {
                                cmdInsertar.Parameters.AddWithValue("@NombrePC", nombrePC);
                                cmdInsertar.ExecuteNonQuery();
                            }
                        }
                    }

                    menu menu = new menu();

                    if (esAdmin)
                    {
                        menu.administrador = 1;
                        menu.panel5.BackColor = Color.Gold;
                    }
                    else
                    {
                        menu.administrador = 0;
                        menu.panel5.BackColor = Color.Green;
                    }

                    menu.usuariolabel.Text = $"USUARIO ACTUAL:\n{txtusuario.Text}";
                    menu.usuarioActual = txtusuario.Text;
                    menu.IdUsuarioActual = idUsuario;

                    Directory.CreateDirectory(Path.GetDirectoryName(rutaUsuario));

                    if (recordarchk.Checked)
                    {
                        try
                        {
                            File.WriteAllText(rutaUsuario, txtusuario.Text);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("No se pudo guardar el usuario recordado: " + ex.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            if (File.Exists(rutaUsuario))
                                File.Delete(rutaUsuario);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("No se pudo eliminar el archivo del usuario recordado: " + ex.Message);
                        }
                    }

                    menu.Show();
                    this.Hide();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void passView_CheckedChanged(object sender, EventArgs e)
        {
            if (passView.Checked == true)
            {
                passView.Image = Proyecto_restaurante.Properties.Resources.ojos_cruzados;
                txtpass.UseSystemPasswordChar = false;
            }
            else if (passView.Checked == false)
            {
                passView.Image = Proyecto_restaurante.Properties.Resources.ojo;
                txtpass.UseSystemPasswordChar = true;
            }
        }

        private void txtusuario_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                txtpass.Focus();
                e.Handled = true;
            }
        }

        private void txtpass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                iniciobtn_Click(sender, e);
                e.Handled = true;
            }
        }

        private void txtusuario_TextChanged(object sender, EventArgs e)
        {
            int posicion = txtusuario.SelectionStart;

            txtusuario.Text = txtusuario.Text.ToUpper();

            txtusuario.SelectionStart = posicion;

            recordarchk.Checked = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void sqlbtn_Click(object sender, EventArgs e)
        {
            autorizar.Location = new Point(90, 71);
            autorizar.BringToFront();
            autorizar.Visible = true;

            panelSesion.Enabled = false;
        }

        private void salirsqlbtn_Click(object sender, EventArgs e)
        {
            conexionpanel.Location = new Point(605, 45);
            conexionpanel.Visible = false;
        }

        private void guardarbtn_Click(object sender, EventArgs e)
        {
            string servidor = servidortxt.Text.Trim();
            string basededatos = DBTxt.Text.Trim();
            string usuario = usuarioservidortxt.Text.Trim();
            string contra = contservidortxt.Text.Trim();
            string porDefecto = defectochk.Checked ? "1" : "0";

            if (string.IsNullOrEmpty(servidor) || string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contra))
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Validaci�n", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<string> lineas = new List<string>();
            bool reemplazoHecho = false;
            bool actualizada = false;

            if (File.Exists(rutaArchivo))
            {
                var lineasOriginales = File.ReadAllLines(rutaArchivo);

                foreach (var linea in lineasOriginales)
                {
                    var partes = linea.Split('|');
                    if (partes.Length == 4)
                    {
                        bool mismaConexion = partes[0] == servidor && partes[1] == usuario;

                        if (mismaConexion)
                        {
                            lineas.Add($"{servidor}|{basededatos}|{usuario}|{contra}|{porDefecto}");
                            actualizada = true;
                        }
                        else
                        {
                            if (partes[4] == "1" && porDefecto == "1" && !reemplazoHecho)
                            {
                                partes[4] = "0";
                                reemplazoHecho = true;
                            }

                            lineas.Add(string.Join("|", partes));
                        }
                    }
                }
            }

            if (!actualizada)
                lineas.Add($"{servidor}|{basededatos}|{usuario}|{contra}|{porDefecto}");

            File.WriteAllLines(rutaArchivo, lineas);

            MessageBox.Show(actualizada ? "Conexi�n actualizada correctamente." : "Conexi�n guardada correctamente.", "�xito", MessageBoxButtons.OK, MessageBoxIcon.Information);

            iniciobtn.Enabled = true;
            button6_Click(sender, e);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            conexiones.Location = new Point(0, 33);
            conexiones.BringToFront();
            conexiones.Visible = true;

            if (!File.Exists(rutaArchivo))
            {
                MessageBox.Show("El archivo de conexiones no existe.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tabla = new DataTable();
            tabla.Columns.Add("Servidor");
            tabla.Columns.Add("Base de Datos");
            tabla.Columns.Add("Usuario");
            tabla.Columns.Add("Contras");
            tabla.Columns.Add("PorDefecto");

            var lineas = File.ReadAllLines(rutaArchivo);

            foreach (var linea in lineas)
            {
                var partes = linea.Split('|');
                if (partes.Length == 5)
                {
                    tabla.Rows.Add(partes[0], partes[1], partes[2], partes[3], partes[4] == "1" ? "S�" : "No");
                }
            }

            txtsql.DataSource = tabla;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            conexiones.Location = new Point(605, 45);
            conexiones.Visible = false; conexiones.Location = new Point(605, 435);
            conexiones.Visible = false;
        }

        private void txtsql_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = txtsql.Rows[e.RowIndex];

                servidortxt.Text = fila.Cells["Servidor"].Value?.ToString();
                DBTxt.Text = fila.Cells["Base de Datos"].Value?.ToString();
                usuarioservidortxt.Text = fila.Cells["Usuario"].Value?.ToString();
                contservidortxt.Text = fila.Cells["Contra"].Value?.ToString();

                string porDefecto = fila.Cells["PorDefecto"].Value?.ToString();
                defectochk.Checked = porDefecto == "S�";
                button5_Click(sender, e);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            txtsql_CellDoubleClick(sender, new DataGridViewCellEventArgs(0, 0));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            servidortxt.Clear();
            DBTxt.Clear();
            usuarioservidortxt.Clear();
            contservidortxt.Clear();
            defectochk.Checked = false;
        }

        private void borrarconex_Click(object sender, EventArgs e)
        {
            if (txtsql.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona una conexi�n para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow fila = txtsql.SelectedRows[0];
            string servidor = fila.Cells["Servidor"].Value?.ToString();
            string basededatos = fila.Cells["Base de Datos"].Value?.ToString();
            string usuario = fila.Cells["Usuario"].Value?.ToString();
            string contras = fila.Cells["Contra"].Value?.ToString();
            string porDefecto = fila.Cells["PorDefecto"].Value?.ToString() == "S�" ? "1" : "0";

            string lineaEliminar = $"{servidor}|{usuario}|{contras}|{porDefecto}";

            var lineas = File.ReadAllLines(rutaArchivo).ToList();
            bool eliminada = lineas.Remove(lineaEliminar);

            if (eliminada)
            {
                File.WriteAllLines(rutaArchivo, lineas);
                MessageBox.Show("Conexi�n eliminada correctamente.", "�xito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                button3_Click(sender, e);
            }
            else
            {
                MessageBox.Show("No se pudo eliminar la conexi�n. Verifica los datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void inicio_Shown(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtusuario.Text))
                txtpass.Focus();
        }

        private void recordarchk_CheckedChanged(object sender, EventArgs e)
        {
            if (recordarchk.Checked)
            {
                recordarchk.BackColor = Color.LightGreen;
            }
            else
            {
                recordarchk.BackColor = SystemColors.Window;
            }
        }

        private async void CrearDBbtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(servidortxt.Text) || string.IsNullOrEmpty(DBTxt.Text) || string.IsNullOrEmpty(usuarioservidortxt.Text) || string.IsNullOrEmpty(contservidortxt.Text))
            {
                MessageBox.Show("Error: Campos vac�os.");
                return;
            }

            progressBar1.Visible = true;
            progressBar1.Value = 10;

            string servidor = servidortxt.Text;
            string nombreDB = DBTxt.Text;
            string usuario = usuarioservidortxt.Text;
            string passw = contservidortxt.Text;

            string conexion = $"Server={servidor};Database=master;User Id={usuario};Password={passw};TrustServerCertificate=True;";

            try
            {
                using (SqlConnection conn = new SqlConnection(conexion))
                {
                    await conn.OpenAsync();
                }

                progressBar1.Value = 30;
                MessageBox.Show("Conexi�n exitosa");

                bool existe = await Task.Run(() => ExisteBaseDatos(conexion, nombreDB));

                if (existe)
                {
                    progressBar1.Value = 0;
                    MessageBox.Show("La base de datos ya existe. No se puede crear nuevamente.");
                    return;
                }

                progressBar1.Value = 50;

                string scriptOriginal = ScriptDB.CrearBaseDatos;

                string scriptDinamico = scriptOriginal.Replace("GloriaRestaurant", nombreDB);

                await Task.Run(() => EjecutarScript(conexion, scriptDinamico));

                // Aplicar cualquier migración pendiente sobre la base recién creada
                try
                {
                    string conexionConDB = $"Server={servidor};Database={nombreDB};User Id={usuario};Password={passw};TrustServerCertificate=True;";
                    ScriptDB.AplicarMigraciones(conexionConDB);
                }
                catch { }

                progressBar1.Value = 90;

                MessageBox.Show("Base de datos creada correctamente");

                guardarbtn.PerformClick();

                progressBar1.Value = 100;
            }
            catch (Exception ex)
            {
                progressBar1.Value = 0;
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                await Task.Delay(500);
                progressBar1.Visible = false;
            }
        }

        private void EjecutarScript(string conexion, string script)
        {
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                conn.Open();

                string[] comandos = script.Split(
                    new[] { "\r\nGO", "\nGO", "GO" },
                    StringSplitOptions.RemoveEmptyEntries
                );

                foreach (string comando in comandos)
                {
                    if (!string.IsNullOrWhiteSpace(comando))
                    {
                        using (SqlCommand cmd = new SqlCommand(comando, conn))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private bool ExisteBaseDatos(string conexion, string nombreDB)
        {
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                conn.Open();

                string query = "SELECT COUNT(*) FROM sys.databases WHERE name = @nombre";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", nombreDB);

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private void autorizarBTN_Click(object sender, EventArgs e)
        {
            if (usuAdmin.Text == UsuarioAdmin && contAdmin.Text == PassAdmin)
            {
                esAdmin = true;

                //quita
                autorizar.Location = new Point(51, 436);
                autorizar.BringToFront();
                autorizar.Visible = true;

                //pone el otro
                conexionpanel.Location = new Point(0, 33);
                conexionpanel.BringToFront();
                conexionpanel.Visible = true;
                panelSesion.Enabled = true;
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos / Inactivo.");
                return;
            }
        }

        private void salirAutorBTN_Click(object sender, EventArgs e)
        {
            autorizar.Visible = false;
            autorizar.Location = new Point(51, 436);
            autorizar.BringToFront();

            panelSesion.Enabled = true;
        }

        private void inicio_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.B)
            {
                sqlbtn.Visible = true;
                sqlbtn.PerformClick();
                usuAdmin.Focus();
                e.SuppressKeyPress = true;
            }

            if (e.Control && e.Alt && e.KeyCode == Keys.P)
            {
                generarSerial.Visible = true;
            }
        }

        private void usuAdmin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                contAdmin.Focus();
                e.Handled = true;
            }
        }

        private void contAdmin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                autorizarBTN.PerformClick();
                autorizarBTN.Focus();
                e.Handled = true;
            }
        }

        private void VerificarActivacionMaquina()
        {
            try
            {
                string dirConfig = Path.GetDirectoryName(rutaActivacion);
                if (!Directory.Exists(dirConfig))
                    Directory.CreateDirectory(dirConfig);

                if (!File.Exists(rutaActivacion))
                {
                    File.WriteAllText(rutaActivacion, "INACTIVA");
                }

                bool activadaLocal = File.ReadAllText(rutaActivacion).Trim().ToUpper() == "ACTIVADO";
                bool activadaBD = false;

                try
                {
                    string connStr = ConexionBD.ConexionSQL();
                    if (!string.IsNullOrEmpty(connStr))
                    {
                        // 1. Aplicar automáticamente migraciones pendientes de la BD
                        try
                        {
                            ScriptDB.AplicarMigraciones(connStr);
                        }
                        catch { }

                        // 2. Verificar activación en base de datos
                        using (SqlConnection conexion = new SqlConnection(connStr))
                        {
                            conexion.Open();
                            string nombrePC = Environment.MachineName;
                            string queryCheck = "SELECT Activado FROM Configuracion WHERE NombrePC = @NombrePC";
                            using (SqlCommand cmd = new SqlCommand(queryCheck, conexion))
                            {
                                cmd.Parameters.AddWithValue("@NombrePC", nombrePC);
                                object res = cmd.ExecuteScalar();
                                if (res != null && res != DBNull.Value)
                                {
                                    activadaBD = Convert.ToInt32(res) == 1;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // Si la BD aún no está disponible, se maneja con la activación local
                }

                if (activadaBD && !activadaLocal)
                {
                    File.WriteAllText(rutaActivacion, "ACTIVADO");
                    activadaLocal = true;
                }

                if (activadaLocal || activadaBD)
                {
                    panelSerial.Visible = false;
                    panelSesion.Enabled = true;
                }
                else
                {
                    panelSerial.Location = new Point(0, 33);
                    panelSerial.BringToFront();
                    panelSerial.Visible = true;
                    panelSesion.Enabled = false;
                    codigoSerial.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al verificar el estado de activación: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static string GenerarPrefijoSerial(DateTime fecha)
        {
            int d = fecha.Day;
            int m = fecha.Month;
            int y = fecha.Year;

            // Genera 6 dígitos de verificación matemática
            int c1 = ((d * 739) + (m * 491) + (y * 127) + 3821) % 900 + 100;
            int c2 = ((d * 313) + (m * 857) + (y * 619) + 7159) % 900 + 100;
            return $"{c1:D3}{c2:D3}";
        }

        public static string GenerarSerial(DateTime fecha)
        {
            string fechaStr = fecha.ToString("ddMMyyyy");
            return $"{GenerarPrefijoSerial(fecha)}{fechaStr}";
        }

        public static bool ValidarSerial(string serial)
        {
            if (string.IsNullOrWhiteSpace(serial))
                return false;

            serial = serial.Trim();
            if (serial.Length != 14 || !long.TryParse(serial, out _))
                return false;

            string prefijoIngresado = serial.Substring(0, 6);
            string fechaStr = serial.Substring(6, 8);

            if (!DateTime.TryParseExact(fechaStr, "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime fecha))
                return false;

            string prefijoEsperado = GenerarPrefijoSerial(fecha);
            return prefijoIngresado == prefijoEsperado;
        }

        private void generarSerial_Click(object sender, EventArgs e)
        {
            string prefijo = GenerarPrefijoSerial(DateTime.Now);
            try
            {
                Clipboard.SetText(prefijo);
                MessageBox.Show("Prefijo de activación copiado al portapapeles con éxito.",
                                "Serial", MessageBoxButtons.OK, MessageBoxIcon.Information);
                pegarClipb.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo copiar al portapapeles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void validarSerial_Click(object sender, EventArgs e)
        {
            string serial = codigoSerial.Text.Trim();

            if (string.IsNullOrEmpty(serial))
            {
                MessageBox.Show("Por favor digite el serial de activación.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ValidarSerial(serial))
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(rutaActivacion));
                    File.WriteAllText(rutaActivacion, "ACTIVADO");

                    try
                    {
                        string connStr = ConexionBD.ConexionSQL();
                        if (!string.IsNullOrEmpty(connStr))
                        {
                            using (SqlConnection conexion = new SqlConnection(connStr))
                            {
                                conexion.Open();
                                string nombrePC = Environment.MachineName;
                                string queryExiste = "SELECT COUNT(*) FROM Configuracion WHERE NombrePC = @NombrePC";
                                using (SqlCommand cmdExiste = new SqlCommand(queryExiste, conexion))
                                {
                                    cmdExiste.Parameters.AddWithValue("@NombrePC", nombrePC);
                                    int existe = (int)cmdExiste.ExecuteScalar();

                                    if (existe == 0)
                                    {
                                        string insert = "INSERT INTO Configuracion (NombrePC, Activado) VALUES (@NombrePC, 1)";
                                        using (SqlCommand cmdIns = new SqlCommand(insert, conexion))
                                        {
                                            cmdIns.Parameters.AddWithValue("@NombrePC", nombrePC);
                                            cmdIns.ExecuteNonQuery();
                                        }
                                    }
                                    else
                                    {
                                        string update = "UPDATE Configuracion SET Activado = 1 WHERE NombrePC = @NombrePC";
                                        using (SqlCommand cmdUpd = new SqlCommand(update, conexion))
                                        {
                                            cmdUpd.Parameters.AddWithValue("@NombrePC", nombrePC);
                                            cmdUpd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Si la BD aún no está creada o conectada, la activación local queda registrada
                    }

                    MessageBox.Show("¡Sistema activado con éxito!", "Activación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    panelSerial.Visible = false;
                    panelSesion.Enabled = true;
                    panelSesion.BringToFront();
                    txtusuario.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al registrar la activación: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("El serial ingresado es inválido o incorrecto.", "Error de Activación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                codigoSerial.Focus();
                codigoSerial.SelectAll();
            }
        }

        private void quitarPanelSerial_Click(object sender, EventArgs e)
        {
            bool activada = File.Exists(rutaActivacion) && File.ReadAllText(rutaActivacion).Trim().ToUpper() == "ACTIVADO";
            if (activada)
            {
                panelSerial.Visible = false;
                panelSesion.Enabled = true;
            }
            else
            {
                Application.Exit();
            }
        }

        private void pegarClipb_Click(object sender, EventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    codigoSerial.Text = Clipboard.GetText().Trim();
                    codigoSerial.Focus();
                    codigoSerial.SelectionStart = codigoSerial.Text.Length;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo pegar desde el portapapeles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.Error == null)
            {
                if (args.IsUpdateAvailable)
                {
                    infoActualizacion = args;
                    actualizarSistema.Visible = true;
                    actualizarSistema.BringToFront();
                }
                else
                {
                    actualizarSistema.Visible = false;
                }
            }
            else
            {
                actualizarSistema.Visible = false;
            }
        }

        private void actualizarSistema_Click(object sender, EventArgs e)
        {
            if (infoActualizacion != null && infoActualizacion.IsUpdateAvailable)
            {
                DialogResult actualizar = MessageBox.Show(
                    $"Hay una nueva versión disponible ({infoActualizacion.CurrentVersion}).\n\n¿Seguro que desea descargar e instalar la actualización ahora?",
                    "Actualización Disponible",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (actualizar == DialogResult.Yes)
                {
                    try
                    {
                        if (AutoUpdater.DownloadUpdate(infoActualizacion))
                        {
                            Application.Exit();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("No se pudo iniciar la actualización: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Actualmente cuenta con la versión más reciente del sistema.", "Sistema al Día", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void historialCambios_Click(object sender, EventArgs e)
        {
            try
            {
                string url = "https://github.com/AlhannYT/Sistema-Restaurante/releases";
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir el navegador: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}