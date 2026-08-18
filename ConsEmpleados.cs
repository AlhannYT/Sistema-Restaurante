using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Proyecto_restaurante
{
    public partial class ConsEmpleados : Form
    {
        public ConsEmpleados()
        {
            InitializeComponent();

            toolTip1 = new System.Windows.Forms.ToolTip();
            toolTip1.SetToolTip(recargarbtn, "Recargar");
            toolTip1.SetToolTip(filtro, "Estado");
            toolTip1.SetToolTip(eliminarbtn, "Limpiar filtros");
        }

        public int PersonaID;
        private int EmpleadoID;
        public int EditarEmpleado = 0;
        public int DirActivado = 0;
        public int TelActivado = 0;
        public int EliminarNum = 0;
        public int EliminarDir = 0;

        string conexionString = ConexionBD.ConexionSQL();
        private bool imagenSeleccionada = false;

        private void CargarPuestos()
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(conexionString))
                {
                    string query = "SELECT IdPuesto, Nombre FROM Puesto";
                    SqlDataAdapter adaptador = new SqlDataAdapter(query, conexion);
                    DataTable dt = new DataTable();
                    adaptador.Fill(dt);

                    puestoCmbx.DisplayMember = "Nombre";
                    puestoCmbx.ValueMember = "IdPuesto";
                    puestoCmbx.DataSource = dt;
                    puestoCmbx.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los puestos: {ex.Message}");
            }
        }

        private void CargarVehiculos()
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(conexionString))
                {
                    string query = @"
                    SELECT IdVehiculo, 
                           ISNULL(Marca, '') + ' ' + ISNULL(Modelo, '') + ' ' + ISNULL(Color, '') + ' (' + ISNULL(Matricula, '') + ')' AS VehiculoDescripcion
                    FROM dbo.VehiculoEmpleados
                    WHERE EstadoVehiculo = 'Activo'
                    ORDER BY Marca, Modelo";

                    SqlDataAdapter adaptador = new SqlDataAdapter(query, conexion);
                    DataTable dt = new DataTable();
                    adaptador.Fill(dt);

                    vehiculoEmpleadocmbx.DisplayMember = "VehiculoDescripcion";
                    vehiculoEmpleadocmbx.ValueMember = "IdVehiculo";
                    vehiculoEmpleadocmbx.DataSource = dt;
                    vehiculoEmpleadocmbx.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los vehículos: {ex.Message}");
            }
        }

        private void rolcmbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool esRepartidor = (rolcmbx.SelectedItem != null && rolcmbx.SelectedItem.ToString() == "Repartidor");
            vehiculoEmpleadocmbx.Enabled = esRepartidor;
            label9.Enabled = true;
            if (!esRepartidor)
            {
                vehiculoEmpleadocmbx.SelectedIndex = -1;
                label9.Enabled = false;
            }
        }

        private void ConsEmpleados_Load(object sender, EventArgs e)
        {
            CargarPuestos();
            CargarVehiculos();
            string conexionString = ConexionBD.ConexionSQL();

            try
            {
                string consultaEmpleados = @"
                SELECT 
                    e.IdEmpleado,
                    p.NombreCompleto,
                    pd.Numero AS Cedula
                FROM Empleado e
                LEFT JOIN Persona p ON e.IdPersona = p.IdPersona
                LEFT JOIN PersonaDocumento pd ON p.IdPersona = pd.IdPersona
                WHERE e.Activo = 1 AND p.Activo = 1;";

                using (SqlDataAdapter adaptador = new SqlDataAdapter(consultaEmpleados, conexionString))
                {
                    DataTable dt = new DataTable();
                    adaptador.Fill(dt);
                    tabladatos.DataSource = dt;
                }

                string consultaUltimoID = "SELECT ISNULL(MAX(IdEmpleado) + 1, 0) FROM Empleado";

                using (SqlConnection conexion = new SqlConnection(conexionString))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand(consultaUltimoID, conexion))
                    {
                        object resultado = cmd.ExecuteScalar();

                        if (resultado != null && resultado != DBNull.Value)
                        {
                            idUltimoEmpleado.Text = resultado.ToString();
                        }
                        else
                        {
                            idUltimoEmpleado.Text = "?";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al cargar los datos: {ex.Message}");
            }
        }

        private void guardarbtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtnombre.Text) || string.IsNullOrEmpty(txtapellido.Text) ||
            string.IsNullOrEmpty(txtcedula.Text) || string.IsNullOrEmpty(txtsueldo.Text) ||
            puestoCmbx.SelectedValue == null || puestoCmbx.SelectedIndex < 0)
            {
                MessageBox.Show("Error: No deje campos vacíos.");
                return;
            }

            byte[] imagenBytes = null;
            if (imagenSeleccionada && imagenempleado.Image != null)
            {
                using (Bitmap bmp = new Bitmap(imagenempleado.Image))
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    imagenBytes = ms.ToArray();
                }
            }

            object idVehiculoVal = DBNull.Value;
            if (vehiculoEmpleadocmbx.Enabled && vehiculoEmpleadocmbx.SelectedValue != null && vehiculoEmpleadocmbx.SelectedIndex >= 0)
            {
                idVehiculoVal = Convert.ToInt32(vehiculoEmpleadocmbx.SelectedValue);
            }

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();
                SqlTransaction trans = conexion.BeginTransaction();

                try
                {
                    if (EditarEmpleado == 0)
                    {
                        string nuevaPersona = @"
                        INSERT INTO Persona (Nombre, Apellido, Email, Activo, CreadoEn)
                        VALUES (@Nombre, @Apellido, @Email, @Activo, GETDATE());
                        SELECT SCOPE_IDENTITY();";

                        using (SqlCommand insertarPersona = new SqlCommand(nuevaPersona, conexion, trans))
                        {
                            insertarPersona.Parameters.AddWithValue("@Nombre", txtnombre.Text);
                            insertarPersona.Parameters.AddWithValue("@Apellido", txtapellido.Text);
                            insertarPersona.Parameters.AddWithValue("@Email", emailtxt.Text);
                            insertarPersona.Parameters.AddWithValue("@Activo", estadochk.Checked ? 1 : 0);

                            PersonaID = Convert.ToInt32(insertarPersona.ExecuteScalar());
                        }

                        string nuevoEmpleado = @"
                        INSERT INTO Empleado (IdPersona, IdPuesto, FechaIngreso, Activo, Sueldo, TipoSueldo, IdRolempleado, ImagenEmpleado, IdVehiculoAsignado)
                        VALUES (@IdPersona, @IdPuesto, @FechaIngreso, @Activo, @Sueldo, @TipoSueldo, @IdRol, @ImagenEmpleado, @IdVehiculoAsignado)";

                        using (SqlCommand insertarEmpleado = new SqlCommand(nuevoEmpleado, conexion, trans))
                        {
                            insertarEmpleado.Parameters.AddWithValue("@IdPersona", PersonaID);
                            insertarEmpleado.Parameters.AddWithValue("@IdPuesto", Convert.ToInt32(puestoCmbx.SelectedValue));
                            insertarEmpleado.Parameters.AddWithValue("@Sueldo", Convert.ToDecimal(txtsueldo.Text));
                            insertarEmpleado.Parameters.AddWithValue("@FechaIngreso", fechaingreso.Value);
                            insertarEmpleado.Parameters.AddWithValue("@TipoSueldo", tiposueldocmbx.SelectedIndex >= 0 ? tiposueldocmbx.SelectedIndex + 1 : 1);
                            insertarEmpleado.Parameters.AddWithValue("@IdRol", rolcmbx.SelectedIndex >= 0 ? rolcmbx.SelectedIndex : 0);
                            insertarEmpleado.Parameters.AddWithValue("@Activo", estadochk.Checked ? 1 : 0);
                            insertarEmpleado.Parameters.Add("@ImagenEmpleado", SqlDbType.VarBinary).Value = (object)imagenBytes ?? DBNull.Value;
                            insertarEmpleado.Parameters.AddWithValue("@IdVehiculoAsignado", idVehiculoVal);

                            insertarEmpleado.ExecuteNonQuery();
                        }

                        string nuevoDoc = @"
                        INSERT INTO PersonaDocumento (IdPersona, IdTipoDocumento, Numero, EsPrincipal)
                        VALUES (@IdPersona, 1, @Numero, 1)";

                        using (SqlCommand insertarDocumento = new SqlCommand(nuevoDoc, conexion, trans))
                        {
                            insertarDocumento.Parameters.AddWithValue("@IdPersona", PersonaID);
                            insertarDocumento.Parameters.AddWithValue("@Numero", txtcedula.Text);

                            insertarDocumento.ExecuteNonQuery();
                        }

                        foreach (DataGridViewRow fila in numeroEmpleado.Rows)
                        {
                            if (fila.IsNewRow) continue;

                            string nombre = fila.Cells["nombre"].Value?.ToString();
                            string numero = fila.Cells["numero"].Value?.ToString();
                            int esPrincipal = Convert.ToBoolean(fila.Cells["principal"].Value) ? 1 : 0;

                            string queryTelefono = @"
                            INSERT INTO PersonaTelefono (IdPersona, Numero, EsPrincipal, NombreTelefono)
                            VALUES (@IdPersona, @Numero, @EsPrincipal, @NombreTelefono)";

                            using (SqlCommand cmdTelefono = new SqlCommand(queryTelefono, conexion, trans))
                            {
                                cmdTelefono.Parameters.AddWithValue("@IdPersona", PersonaID);
                                cmdTelefono.Parameters.AddWithValue("@Numero", numero);
                                cmdTelefono.Parameters.AddWithValue("@EsPrincipal", esPrincipal);
                                cmdTelefono.Parameters.AddWithValue("@NombreTelefono", nombre);

                                cmdTelefono.ExecuteNonQuery();
                            }
                        }

                        foreach (DataGridViewRow fila in direccionEmpleado.Rows)
                        {
                            if (fila.IsNewRow) continue;

                            string nombre = fila.Cells["nombre"].Value?.ToString();
                            string direccion = fila.Cells["direccion"].Value?.ToString();
                            int esPrincipal = Convert.ToBoolean(fila.Cells["principal"].Value) ? 1 : 0;

                            string queryDireccion = @"
                            INSERT INTO PersonaDireccion (IdPersona, Direccion, EsPrincipal, Nombre)
                            VALUES (@IdPersona, @Direccion, @EsPrincipal, @Nombre)";

                            using (SqlCommand cmdDireccion = new SqlCommand(queryDireccion, conexion, trans))
                            {
                                cmdDireccion.Parameters.AddWithValue("@IdPersona", PersonaID);
                                cmdDireccion.Parameters.AddWithValue("@Nombre", nombre);
                                cmdDireccion.Parameters.AddWithValue("@Direccion", direccion);
                                cmdDireccion.Parameters.AddWithValue("@EsPrincipal", esPrincipal);

                                cmdDireccion.ExecuteNonQuery();
                            }
                        }

                        trans.Commit();
                        MessageBox.Show("Empleado registrado con éxito.");
                    }
                    else if (EditarEmpleado == 1)
                    {
                        string actualizarPersona = @"
                        UPDATE Persona 
                        SET Nombre = @Nombre, Apellido = @Apellido, Email = @Email, Activo = @Activo 
                        WHERE IdPersona = @IdPersona";

                        using (SqlCommand actualizarCommand = new SqlCommand(actualizarPersona, conexion, trans))
                        {
                            actualizarCommand.Parameters.AddWithValue("@IdPersona", PersonaID);
                            actualizarCommand.Parameters.AddWithValue("@Nombre", txtnombre.Text);
                            actualizarCommand.Parameters.AddWithValue("@Apellido", txtapellido.Text);
                            actualizarCommand.Parameters.AddWithValue("@Email", emailtxt.Text);
                            actualizarCommand.Parameters.AddWithValue("@Activo", estadochk.Checked ? 1 : 0);
                            actualizarCommand.ExecuteNonQuery();
                        }

                        string actualizarEmpleado = @"
                        UPDATE Empleado 
                        SET IdPuesto = @IdPuesto, Sueldo = @Sueldo, FechaIngreso = @FechaIngreso, 
                            TipoSueldo = @TipoSueldo, Activo = @Activo, IdRolempleado = @IdRol, 
                            ImagenEmpleado = @ImagenEmpleado, IdVehiculoAsignado = @IdVehiculoAsignado 
                        WHERE IdEmpleado = @IdEmpleado";

                        using (SqlCommand actualizarCommand = new SqlCommand(actualizarEmpleado, conexion, trans))
                        {
                            actualizarCommand.Parameters.AddWithValue("@IdEmpleado", EmpleadoID);
                            actualizarCommand.Parameters.AddWithValue("@IdPuesto", Convert.ToInt32(puestoCmbx.SelectedValue));
                            actualizarCommand.Parameters.AddWithValue("@Sueldo", Convert.ToDecimal(txtsueldo.Text));
                            actualizarCommand.Parameters.AddWithValue("@FechaIngreso", fechaingreso.Value);
                            actualizarCommand.Parameters.AddWithValue("@TipoSueldo", tiposueldocmbx.SelectedIndex >= 0 ? tiposueldocmbx.SelectedIndex + 1 : 1);
                            actualizarCommand.Parameters.AddWithValue("@IdRol", rolcmbx.SelectedIndex >= 0 ? rolcmbx.SelectedIndex : 0);
                            actualizarCommand.Parameters.AddWithValue("@Activo", estadochk.Checked ? 1 : 0);
                            actualizarCommand.Parameters.Add("@ImagenEmpleado", SqlDbType.VarBinary).Value = (object)imagenBytes ?? DBNull.Value;
                            actualizarCommand.Parameters.AddWithValue("@IdVehiculoAsignado", idVehiculoVal);
                            actualizarCommand.ExecuteNonQuery();
                        }

                        string actualizarDoc = @"
                        UPDATE PersonaDocumento
                        SET Numero = @Numero
                        WHERE IdPersona = @IdPersona AND EsPrincipal = 1";

                        using (SqlCommand cmdDoc = new SqlCommand(actualizarDoc, conexion, trans))
                        {
                            cmdDoc.Parameters.AddWithValue("@IdPersona", PersonaID);
                            cmdDoc.Parameters.AddWithValue("@Numero", txtcedula.Text);
                            cmdDoc.ExecuteNonQuery();
                        }

                        foreach (DataGridViewRow fila in numeroEmpleado.Rows)
                        {
                            if (fila.IsNewRow) continue;

                            string nombre = fila.Cells["nombre"].Value?.ToString();
                            string numero = fila.Cells["numero"].Value?.ToString();
                            int esPrincipal = Convert.ToBoolean(fila.Cells["principal"].Value) ? 1 : 0;

                            string queryTelefono = @"
                            IF EXISTS (SELECT 1 FROM PersonaTelefono WHERE IdPersona = @IdPersona AND Numero = @Numero)
                            UPDATE PersonaTelefono
                            SET NombreTelefono = @NombreTelefono, EsPrincipal = @EsPrincipal
                            WHERE IdPersona = @IdPersona AND Numero = @Numero
                            ELSE
                            INSERT INTO PersonaTelefono (IdPersona, Numero, EsPrincipal, NombreTelefono)
                            VALUES (@IdPersona, @Numero, @EsPrincipal, @NombreTelefono)";

                            using (SqlCommand cmdTelefono = new SqlCommand(queryTelefono, conexion, trans))
                            {
                                cmdTelefono.Parameters.AddWithValue("@IdPersona", PersonaID);
                                cmdTelefono.Parameters.AddWithValue("@Numero", numero);
                                cmdTelefono.Parameters.AddWithValue("@EsPrincipal", esPrincipal);
                                cmdTelefono.Parameters.AddWithValue("@NombreTelefono", nombre);

                                cmdTelefono.ExecuteNonQuery();
                            }
                        }

                        foreach (DataGridViewRow fila in direccionEmpleado.Rows)
                        {
                            if (fila.IsNewRow) continue;

                            string nombre = fila.Cells["nombre"].Value?.ToString();
                            string direccion = fila.Cells["direccion"].Value?.ToString();
                            int esPrincipal = Convert.ToBoolean(fila.Cells["principal"].Value) ? 1 : 0;

                            string queryDireccion = @"
                            IF EXISTS (SELECT 1 FROM PersonaDireccion WHERE IdPersona = @IdPersona AND Direccion = @Direccion)
                            UPDATE PersonaDireccion
                            SET Nombre = @Nombre, EsPrincipal = @EsPrincipal
                            WHERE IdPersona = @IdPersona AND Direccion = @Direccion
                            ELSE
                            INSERT INTO PersonaDireccion (IdPersona, Direccion, EsPrincipal, Nombre)
                            VALUES (@IdPersona, @Direccion, @EsPrincipal, @Nombre)";

                            using (SqlCommand cmdDireccion = new SqlCommand(queryDireccion, conexion, trans))
                            {
                                cmdDireccion.Parameters.AddWithValue("@IdPersona", PersonaID);
                                cmdDireccion.Parameters.AddWithValue("@Nombre", nombre);
                                cmdDireccion.Parameters.AddWithValue("@Direccion", direccion);
                                cmdDireccion.Parameters.AddWithValue("@EsPrincipal", esPrincipal);

                                cmdDireccion.ExecuteNonQuery();
                            }
                        }

                        trans.Commit();
                        MessageBox.Show("Empleado actualizado con éxito.");
                        limpiarbtn_Click(sender, e);
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show($"Ocurrió un error: {ex.Message}");
                }
            }
        }

        public int Estadobuscarpuesto = 1;

        private void buscarpuesto_Click(object sender, EventArgs e)
        {
            string conexionString = ConexionBD.ConexionSQL();
            string puesto = "select IdPuesto, Nombre from Puesto";

            SqlDataAdapter adaptador = new SqlDataAdapter(puesto, conexionString);

            DataTable dt = new DataTable();

            adaptador.Fill(dt);
        }

        private void txtcedula_TextChanged(object sender, EventArgs e)
        {
            string posicion = txtcedula.Text;
            posicion = posicion.Replace("-", "");

            if (posicion.Length > 11)
            {
                posicion = posicion.Substring(0, 11);
            }

            if (posicion.Length > 3)
            {
                posicion = posicion.Insert(3, "-");
            }

            if (posicion.Length > 11)
            {
                posicion = posicion.Insert(11, "-");
            }

            txtcedula.Text = posicion;
            txtcedula.SelectionStart = txtcedula.Text.Length;
        }

        private void agregar_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            txtcedula.Focus();
            EmpleadoID = 0;
            PersonaID = 0;
            imagenSeleccionada = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
            limpiarbtn_Click(sender, e);
        }

        private void limpiarbtn_Click(object sender, EventArgs e)
        {
            txtcedula.Clear();
            txtnombre.Clear();
            txtapellido.Clear();
            emailtxt.Clear();
            txtsueldo.Clear();
            puestoCmbx.SelectedIndex = -1;
            fechaingreso.Value = DateTime.Now;
            tiposueldocmbx.SelectedIndex = -1;
            rolcmbx.SelectedIndex = -1;
            vehiculoEmpleadocmbx.SelectedIndex = -1;
            vehiculoEmpleadocmbx.Enabled = false;
            estadochk.Checked = true;
            numeroEmpleado.Rows.Clear();
            direccionEmpleado.Rows.Clear();
            imagenempleado.Image = Proyecto_restaurante.Properties.Resources.perfilcliente;
            imagenSeleccionada = false;
        }

        private void seleccionimagenbtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Archivos de imagen (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
                openFileDialog.Title = "Seleccionar imagen";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        byte[] bytes = File.ReadAllBytes(openFileDialog.FileName);
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            imagenempleado.Image = Image.FromStream(ms);
                        }
                        imagenSeleccionada = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al cargar la imagen: " + ex.Message);
                    }
                }
            }
        }

        private void tabladatos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = tabladatos.Rows[e.RowIndex];
                int idEmpleado = Convert.ToInt32(fila.Cells["IdEmpleado"].Value);

                using (SqlConnection conexion = new SqlConnection(conexionString))
                {
                    try
                    {
                        conexion.Open();
                        string query = "SELECT ImagenEmpleado FROM Empleado WHERE IdEmpleado = @IdEmpleado";
                        using (SqlCommand cmd = new SqlCommand(query, conexion))
                        {
                            cmd.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                            object result = cmd.ExecuteScalar();

                            if (result != null && result != DBNull.Value && ((byte[])result).Length > 0)
                            {
                                byte[] bytes = (byte[])result;
                                using (MemoryStream ms = new MemoryStream(bytes))
                                {
                                    empleadoimg.Image = Image.FromStream(ms);
                                }
                            }
                            else
                            {
                                empleadoimg.Image = Proyecto_restaurante.Properties.Resources.perfilcliente;
                            }
                        }
                    }
                    catch
                    {
                        empleadoimg.Image = Proyecto_restaurante.Properties.Resources.perfilcliente;
                    }
                }
            }
        }

        private void bajarTelefono_Click(object sender, EventArgs e)
        {
            if (nombrenumerotxt.Text == "" || numerotxt.Text == "")
            {
                MessageBox.Show("Campos Vacíos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(numeroEmpleado);

            row.Cells[0].Value = nombrenumerotxt.Text;
            row.Cells[1].Value = numerotxt.Text;
            row.Cells[2].Value = numPrincipalcmbx.Checked;

            numeroEmpleado.Rows.Add(row);

            if (TelActivado == 1)
            {
                numPrincipalcmbx.Checked = false;
                numPrincipalcmbx.Enabled = false;
            }

            nombrenumerotxt.Clear();
            numerotxt.Clear();
            numPrincipalcmbx.Checked = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (numeroEmpleado.SelectedRows.Count > 0)
            {
                int fila = numeroEmpleado.SelectedRows[0].Index;
                numeroEmpleado.Rows.RemoveAt(fila);
                EliminarNum = 0;

                if (numeroEmpleado.Rows.Count == 0)
                {
                    TelActivado = 0;
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar una fila.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (direccionEmpleado.SelectedRows.Count > 0)
            {
                int fila = direccionEmpleado.SelectedRows[0].Index;
                direccionEmpleado.Rows.RemoveAt(fila);
                EliminarDir = 0;

                if (direccionEmpleado.Rows.Count == 0)
                {
                    DirActivado = 0;
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar una fila.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void numeroEmpleado_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            EliminarNum = 1;
        }

        private void direccionEmpleado_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            EliminarDir = 1;
        }

        private void ConsEmpleados_KeyDown(object sender, KeyEventArgs e)
        {
            if (tabControl1.SelectedIndex == 1 && EliminarNum == 1 && e.KeyCode == Keys.Delete)
            {
                button6.PerformClick();
            }

            if (tabControl1.SelectedIndex == 1 && EliminarDir == 1 && e.KeyCode == Keys.Delete)
            {
                button7.PerformClick();
            }
        }

        private void recargarbtn_Click(object sender, EventArgs e)
        {
            ConsEmpleados_Load(sender, e);
        }

        private void eliminarbtn_Click(object sender, EventArgs e)
        {
            txtbuscador.Clear();
            filtro.Checked = true;
        }

        private void Editar_Click(object sender, EventArgs e)
        {
            if (tabladatos.SelectedRows.Count > 0)
            {
                int idEmpleado = Convert.ToInt32(tabladatos.SelectedRows[0].Cells["IdEmpleado"].Value);

                EditarEmpleado = 1;

                CargarDatosEmpleado(idEmpleado);

                tabControl1.SelectedIndex = 1;

                txtcedula.Focus();
            }
            else
            {
                MessageBox.Show("Seleccione un empleado para editar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CargarDatosEmpleado(int idEmpleado)
        {
            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();

                string query = @"
                SELECT 
                    e.IdEmpleado,
                    e.IdPersona,
                    e.IdPuesto,
                    pu.Nombre AS NombrePuesto,
                    e.Sueldo,
                    e.FechaIngreso,
                    e.TipoSueldo,
                    e.IdRolempleado,
                    e.ImagenEmpleado,
                    e.IdVehiculoAsignado,
                    p.Nombre,
                    p.Apellido,
                    p.Email,
                    p.Activo
                FROM Empleado e
                INNER JOIN Persona p ON e.IdPersona = p.IdPersona
                LEFT JOIN Puesto pu ON e.IdPuesto = pu.IdPuesto
                WHERE e.IdEmpleado = @IdEmpleado";

                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@IdEmpleado", idEmpleado);

                SqlDataReader dr = cmd.ExecuteReader();

                if (!dr.Read())
                {
                    MessageBox.Show("Empleado no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                EmpleadoID = idEmpleado;
                PersonaID = Convert.ToInt32(dr["IdPersona"]);

                txtnombre.Text = dr["Nombre"].ToString();
                txtapellido.Text = dr["Apellido"].ToString();
                emailtxt.Text = dr["Email"].ToString();
                estadochk.Checked = Convert.ToBoolean(dr["Activo"]);

                if (dr["IdPuesto"] != DBNull.Value)
                {
                    puestoCmbx.SelectedValue = Convert.ToInt32(dr["IdPuesto"]);
                }
                else
                {
                    puestoCmbx.SelectedIndex = -1;
                }

                txtsueldo.Text = dr["Sueldo"] != DBNull.Value ? dr["Sueldo"].ToString() : "";

                if (dr["FechaIngreso"] != DBNull.Value)
                {
                    fechaingreso.Value = Convert.ToDateTime(dr["FechaIngreso"]);
                }

                if (dr["TipoSueldo"] != DBNull.Value)
                {
                    int valTipo = Convert.ToInt32(dr["TipoSueldo"]);
                    int indexTipo = (valTipo > 0) ? valTipo - 1 : valTipo;
                    if (indexTipo >= 0 && indexTipo < tiposueldocmbx.Items.Count)
                        tiposueldocmbx.SelectedIndex = indexTipo;
                    else
                        tiposueldocmbx.SelectedIndex = -1;
                }
                else
                {
                    tiposueldocmbx.SelectedIndex = -1;
                }

                if (dr["IdRolempleado"] != DBNull.Value)
                {
                    int indexRol = Convert.ToInt32(dr["IdRolempleado"]);
                    if (indexRol >= 0 && indexRol < rolcmbx.Items.Count)
                        rolcmbx.SelectedIndex = indexRol;
                    else
                        rolcmbx.SelectedIndex = -1;
                }
                else
                {
                    rolcmbx.SelectedIndex = -1;
                }

                rolcmbx_SelectedIndexChanged(null, null);

                if (dr["IdVehiculoAsignado"] != DBNull.Value)
                {
                    vehiculoEmpleadocmbx.SelectedValue = Convert.ToInt32(dr["IdVehiculoAsignado"]);
                }
                else
                {
                    vehiculoEmpleadocmbx.SelectedIndex = -1;
                }

                if (dr["ImagenEmpleado"] != DBNull.Value && dr["ImagenEmpleado"] != null)
                {
                    byte[] bytes = (byte[])dr["ImagenEmpleado"];
                    if (bytes.Length > 0)
                    {
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            imagenempleado.Image = Image.FromStream(ms);
                        }
                        imagenSeleccionada = true;
                    }
                    else
                    {
                        imagenempleado.Image = Proyecto_restaurante.Properties.Resources.perfilcliente;
                        imagenSeleccionada = false;
                    }
                }
                else
                {
                    imagenempleado.Image = Proyecto_restaurante.Properties.Resources.perfilcliente;
                    imagenSeleccionada = false;
                }

                dr.Close();

                string queryDoc = @"
                SELECT Numero 
                FROM PersonaDocumento
                WHERE IdPersona = @IdPersona AND EsPrincipal = 1";

                SqlCommand cmdDoc = new SqlCommand(queryDoc, conexion);
                cmdDoc.Parameters.AddWithValue("@IdPersona", PersonaID);

                object numeroDoc = cmdDoc.ExecuteScalar();
                txtcedula.Text = numeroDoc?.ToString() ?? "";

                numeroEmpleado.Rows.Clear();

                string queryTels = @"
                SELECT NombreTelefono, Numero, EsPrincipal
                FROM PersonaTelefono
                WHERE IdPersona = @IdPersona";

                SqlCommand cmdTels = new SqlCommand(queryTels, conexion);
                cmdTels.Parameters.AddWithValue("@IdPersona", PersonaID);

                SqlDataReader drTels = cmdTels.ExecuteReader();

                while (drTels.Read())
                {
                    numeroEmpleado.Rows.Add(
                        drTels["NombreTelefono"].ToString(),
                        drTels["Numero"].ToString(),
                        Convert.ToBoolean(drTels["EsPrincipal"])
                    );
                }

                drTels.Close();

                direccionEmpleado.Rows.Clear();

                string queryDire = @"
                SELECT Nombre, Direccion, EsPrincipal
                FROM PersonaDireccion
                WHERE IdPersona = @IdPersona";

                SqlCommand cmdDire = new SqlCommand(queryDire, conexion);
                cmdDire.Parameters.AddWithValue("@IdPersona", PersonaID);

                SqlDataReader drDir = cmdDire.ExecuteReader();

                while (drDir.Read())
                {
                    direccionEmpleado.Rows.Add(
                        drDir["Nombre"].ToString(),
                        drDir["Direccion"].ToString(),
                        Convert.ToBoolean(drDir["EsPrincipal"])
                    );
                }

                idUltimoEmpleado.Text = idEmpleado.ToString();

                drDir.Close();
            }
        }

        private void bajarDireccion_Click(object sender, EventArgs e)
        {
            if (nombredirecciontxt.Text == "" || direcciontxt.Text == "")
            {
                MessageBox.Show("Campos Vacíos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(direccionEmpleado);

            row.Cells[0].Value = nombredirecciontxt.Text;
            row.Cells[1].Value = direcciontxt.Text;
            row.Cells[2].Value = principalDireccion.Checked;

            direccionEmpleado.Rows.Add(row);

            if (DirActivado == 1)
            {
                principalDireccion.Checked = false;
                principalDireccion.Enabled = false;
            }

            nombredirecciontxt.Clear();
            direcciontxt.Clear();
            principalDireccion.Checked = false;
        }

        private void numerotxt_TextChanged(object sender, EventArgs e)
        {
            string posNum = numerotxt.Text;
            posNum = posNum.Replace("-", "");

            if (posNum.Length > 10)
            {
                posNum = posNum.Substring(0, 10);
            }

            if (posNum.Length > 3)
            {
                posNum = posNum.Insert(3, "-");
            }

            if (posNum.Length > 7)
            {
                posNum = posNum.Insert(7, "-");
            }

            numerotxt.Text = posNum;
            numerotxt.SelectionStart = numerotxt.Text.Length;
        }
    }
}