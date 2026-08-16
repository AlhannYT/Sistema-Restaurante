using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Proyecto_restaurante
{
    public partial class ConsProductos : Form
    {
        public ConsProductos()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
        }

        string conexionString = ConexionBD.ConexionSQL();

        private System.Windows.Forms.ToolTip toolTip1;

        private string CodigoProductoActual;
        private int idProducto = 0;
        private int PorcGanancia = 0;
        private int ingrediente = 0;
        private byte[] imagenBytesProducto = null;

        private void tabladatos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (tabladatos.SelectedCells.Count == 0) return;

            idProducto = Convert.ToInt32(tabladatos.SelectedCells[0].Value);

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                try
                {
                    conexion.Open();
                    string query = "SELECT ImagenProducto FROM ProductoVenta WHERE IdProducto = @IdProducto";
                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@IdProducto", idProducto);
                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value && ((byte[])result).Length > 0)
                        {
                            byte[] bytes = (byte[])result;
                            imagenBytesProducto = bytes;
                            using (MemoryStream ms = new MemoryStream(bytes))
                            {
                                imagenproducto.Image = Image.FromStream(ms);
                                if (imagenprod != null)
                                    imagenprod.Image = Image.FromStream(new MemoryStream(bytes));
                            }
                        }
                        else
                        {
                            imagenBytesProducto = null;
                            imagenproducto.Image = Proyecto_restaurante.Properties.Resources.paisaje;
                            if (imagenprod != null)
                                imagenprod.Image = Proyecto_restaurante.Properties.Resources.paisaje;
                        }
                    }
                }
                catch
                {
                    imagenBytesProducto = null;
                    imagenproducto.Image = Proyecto_restaurante.Properties.Resources.paisaje;
                    if (imagenprod != null)
                        imagenprod.Image = Proyecto_restaurante.Properties.Resources.paisaje;
                }
            }
        }

        private void ConsProductos_Load(object sender, EventArgs e)
        {
            toolTip1 = new System.Windows.Forms.ToolTip();
            toolTip1.SetToolTip(recargarbtn, "Recargar");
            toolTip1.SetToolTip(autoCalcular, "Calcular Automaticamente");
            toolTip1.SetToolTip(eliminarbtn, "Limpiar filtros");
            toolTip1.SetToolTip(filtrotodos, "Todos");
            toolTip1.SetToolTip(filtroingredientes, "Ingredientes");
            toolTip1.SetToolTip(filtroplatos, "Platos");
            toolTip1.SetToolTip(filtroadicion, "Adiciones");
            toolTip1.SetToolTip(filtrobebida, "Bebidas");

            string consultaUltimoID = "SELECT ISNULL(MAX(IdProducto), 0) + 1 FROM ProductoVenta";

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();
                using (SqlCommand cmd = new SqlCommand(consultaUltimoID, conexion))
                {
                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null && resultado != DBNull.Value)
                    {
                        ultimoID.Text = resultado.ToString();
                    }
                    else
                    {
                        ultimoID.Text = "1";
                    }
                }
            }

            string ConsultaNCF = @"SELECT TOP 1 GenerarNCF FROM ConfiguracionSistema";

            using (SqlConnection con = new SqlConnection(conexionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(ConsultaNCF, con))
                {
                    object resultado = cmd.ExecuteScalar();

                    bool generarNCF = (resultado != null && resultado != DBNull.Value) ? Convert.ToBoolean(resultado) : false;

                    if (!generarNCF)
                    {
                        ITBIS.Enabled = false;
                        ITBIS.SelectedIndex = 2;
                    }
                    else
                    {
                        ITBIS.Enabled = true;
                    }
                }
            }

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            txtbuscador.TextChanged -= txtbuscador_TextChanged;
            txtbuscador.TextChanged += txtbuscador_TextChanged;

            eliminarbtn.Click -= eliminarbtn_Click;
            eliminarbtn.Click += eliminarbtn_Click;

            CargarProductosConsulta();

            CargarTiposProducto(conexionString);
            CargarConfiguracion();

            recetaingredientes.CellValueChanged += (s, e) => ActualizarPrecioVenta();
            recetaingredientes.RowsRemoved += (s, e) => ActualizarPrecioVenta();
            recetaingredientes.UserDeletedRow += (s, e) => ActualizarPrecioVenta();
        }

        private void CargarTiposProducto(string conexionString)
        {
            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                string consultaTipos = "SELECT IdProductoTipo, Nombre FROM ProductoTipo WHERE Activo = 1";
                string consultaUniMedida = "SELECT IdUnidadMedida, Nombre FROM UnidadMedida WHERE Activo = 1";
                SqlDataAdapter da = new SqlDataAdapter(consultaTipos, conexion);
                DataTable dtTipos = new DataTable();
                da.Fill(dtTipos);

                SqlDataAdapter uni = new SqlDataAdapter(consultaUniMedida, conexion);
                DataTable dtUni = new DataTable();
                uni.Fill(dtUni);

                tipoproductocmbx.DisplayMember = "Nombre";
                tipoproductocmbx.ValueMember = "IdProductoTipo";
                tipoproductocmbx.DataSource = dtTipos;

                unidadmedida.DisplayMember = "Nombre";
                unidadmedida.ValueMember = "IdUnidadMedida";
                unidadmedida.DataSource = dtUni;
            }

            tipoproductocmbx.SelectedIndexChanged += tipoproductocmbx_SelectedIndexChanged;
        }

        private void tipoproductocmbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tipoproductocmbx.SelectedValue == null)
                return;

            if (tipoproductocmbx.SelectedValue is DataRowView)
                return;

            int idTipo = Convert.ToInt32(tipoproductocmbx.SelectedValue);

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                string consulta = "SELECT Ingrediente FROM ProductoTipo WHERE IdProductoTipo = @Id";
                SqlCommand cmd = new SqlCommand(consulta, conexion);
                cmd.Parameters.AddWithValue("@Id", idTipo);

                conexion.Open();
                object resultado = cmd.ExecuteScalar();
                conexion.Close();

                if (resultado != null && resultado != DBNull.Value)
                {
                    ingrediente = Convert.ToInt32(resultado);
                }
                else
                {
                    ingrediente = 0;
                }
            }

            if (ingrediente == 1)
            {
                txtcodigo_barras.Enabled = true;
                codigobarrarandombtn.Enabled = true;
                txtnombre_prod.Enabled = true;
                txtprecio_compra.Enabled = true;
                buscarcateg.Enabled = true;
                unidadmedida.Enabled = true;
                txtprecio_venta.Enabled = false;
                autoCalcular.Checked = false;
                autoCalcular.Enabled = false;
                guardarbtn.Enabled = true;
                limpiarbtn.Enabled = true;

                seleccionpanel.Enabled = false;
                seleccionpanel.Visible = false;

                ingredientesconsulta.DataSource = null;

                recetaingredientes.Columns.Clear();
                recetaingredientes.Rows.Clear();
            }
            else
            {
                txtcodigo_barras.Enabled = true;
                codigobarrarandombtn.Enabled = true;
                txtnombre_prod.Enabled = true;
                txtprecio_compra.Enabled = true;
                buscarcateg.Enabled = true;
                unidadmedida.Enabled = true;
                autoCalcular.Checked = true;
                autoCalcular.Enabled = true;
                buscarIngredienteReceta.Enabled = true;

                guardarbtn.Enabled = true;
                limpiarbtn.Enabled = true;

                seleccionpanel.Enabled = true;
                seleccionpanel.Visible = true;
                txtprecio_venta.Enabled = true;

                using (SqlConnection conexion = new SqlConnection(conexionString))
                {
                    string consulta = @"
                    SELECT
                        PV.IdProducto,
                        PV.Nombre,
                        PV.PrecioCompra as Costo,
                        UM.Nombre as Medida
                    FROM ProductoVenta PV
                    INNER JOIN ProductoTipo PT
                    ON PV.IdProductoTipo = PT.IdProductoTipo
                    INNER JOIN UnidadMedida UM
                    ON PV.IdUnidadMedida = UM.IdUnidadMedida
                    WHERE PT.Ingrediente = 1;";
                    SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexionString);
                    DataTable dt = new DataTable();
                    adaptador.Fill(dt);
                    ingredientesconsulta.DataSource = dt;

                    ingredientesconsulta.Columns["IdProducto"].HeaderText = "ID";
                    ingredientesconsulta.Columns["Nombre"].HeaderText = "Nombre";
                    ingredientesconsulta.Columns["Medida"].HeaderText = "Medida";
                    ingredientesconsulta.Columns["Costo"].HeaderText = "Costo";

                    recetaingredientes.Columns.Clear();

                    recetaingredientes.Columns.Add("ID", "ID");
                    recetaingredientes.Columns.Add("Ingrediente", "Ingrediente");
                    recetaingredientes.Columns.Add("Medida", "Medida");
                    recetaingredientes.Columns.Add("Costo", "Costo");
                    recetaingredientes.Columns.Add("Cantidad", "Cantidad");
                }

                autoCalcular_CheckedChanged(sender, e);
            }
        }

        private void agregar_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            tipoproductocmbx.Focus();
            limpiarbtn_Click(sender, e);
        }

        private void CargarConfiguracion()
        {
            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();

                string query = @"
                SELECT TOP 1 PorcentajeGanancia 
                FROM ConfiguracionSistema";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        PorcGanancia = Convert.ToInt32(result);
                    }
                    else
                    {
                        PorcGanancia = 0;
                    }
                }
            }
        }

        private void recargarbtn_Click(object sender, EventArgs e)
        {
            CargarProductosConsulta();
        }

        private void Editar_Click(object sender, EventArgs e)
        {
            int idProd = 0;

            if (tabladatos.SelectedRows.Count > 0)
            {
                idProd = Convert.ToInt32(tabladatos.SelectedRows[0].Cells["IdProducto"].Value);
            }
            else if (tabladatos.SelectedCells.Count > 0)
            {
                int rowIndex = tabladatos.SelectedCells[0].RowIndex;
                idProd = Convert.ToInt32(tabladatos.Rows[rowIndex].Cells["IdProducto"].Value);
            }
            else if (idProducto > 0)
            {
                idProd = idProducto;
            }

            if (idProd > 0)
            {
                idProducto = idProd;
                CodigoProductoActual = idProd.ToString();
                CargarDatosProducto(idProd);
                tabControl1.SelectedIndex = 1;
                txtnombre_prod.Focus();
            }
            else
            {
                MessageBox.Show("Seleccione un producto para editar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CargarDatosProducto(int idProd)
        {
            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                try
                {
                    conexion.Open();

                    string query = @"
                        SELECT 
                            PV.IdProducto,
                            PV.Nombre,
                            PV.IdCategoria,
                            C.Nombre AS NombreCategoria,
                            PV.IdProductoTipo,
                            PV.Activo,
                            PV.PrecioCompra,
                            PV.PrecioVenta,
                            PV.Itbis,
                            PV.CodigoBarra,
                            PV.IdUnidadMedida,
                            PV.ImagenProducto
                        FROM ProductoVenta PV
                        LEFT JOIN CategoriaProducto C ON PV.IdCategoria = C.IdCategoria
                        WHERE PV.IdProducto = @IdProducto";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@IdProducto", idProd);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                ultimoID.Text = dr["IdProducto"].ToString();
                                txtcodigo_barras.Text = dr["CodigoBarra"] != DBNull.Value ? dr["CodigoBarra"].ToString() : "";
                                txtnombre_prod.Text = dr["Nombre"] != DBNull.Value ? dr["Nombre"].ToString() : "";
                                idcategoriatxt.Text = dr["IdCategoria"] != DBNull.Value ? dr["IdCategoria"].ToString() : "";
                                categoriatxt.Text = dr["NombreCategoria"] != DBNull.Value ? dr["NombreCategoria"].ToString() : "";

                                if (dr["IdProductoTipo"] != DBNull.Value)
                                    tipoproductocmbx.SelectedValue = Convert.ToInt32(dr["IdProductoTipo"]);

                                if (dr["IdUnidadMedida"] != DBNull.Value)
                                    unidadmedida.SelectedValue = Convert.ToInt32(dr["IdUnidadMedida"]);

                                txtprecio_compra.Text = dr["PrecioCompra"] != DBNull.Value ? Convert.ToDecimal(dr["PrecioCompra"]).ToString("N2") : "0";
                                txtprecio_venta.Text = dr["PrecioVenta"] != DBNull.Value ? Convert.ToDecimal(dr["PrecioVenta"]).ToString("N2") : "0";

                                if (dr["Itbis"] != DBNull.Value)
                                {
                                    decimal valItbisDec = Convert.ToDecimal(dr["Itbis"]);
                                    if (valItbisDec == 0m)
                                    {
                                        int indexEx = ITBIS.FindString("Excento");
                                        if (indexEx < 0) indexEx = ITBIS.FindString("Exento");
                                        if (indexEx >= 0)
                                            ITBIS.SelectedIndex = indexEx;
                                    }
                                    else
                                    {
                                        string valItbis = valItbisDec.ToString("G29");
                                        int indexItbis = ITBIS.FindString(valItbis);
                                        if (indexItbis >= 0)
                                            ITBIS.SelectedIndex = indexItbis;
                                    }
                                }

                                estadochk.Checked = dr["Activo"] != DBNull.Value && Convert.ToInt32(dr["Activo"]) == 1;

                                if (dr["ImagenProducto"] != DBNull.Value && dr["ImagenProducto"] != null)
                                {
                                    byte[] bytes = (byte[])dr["ImagenProducto"];
                                    imagenBytesProducto = bytes;
                                    using (MemoryStream ms = new MemoryStream(bytes))
                                    {
                                        if (imagenprod != null)
                                            imagenprod.Image = Image.FromStream(ms);
                                        if (imagenproducto != null)
                                            imagenproducto.Image = Image.FromStream(new MemoryStream(bytes));
                                    }
                                }
                                else
                                {
                                    imagenBytesProducto = null;
                                    if (imagenprod != null)
                                        imagenprod.Image = Proyecto_restaurante.Properties.Resources.paisaje;
                                    if (imagenproducto != null)
                                        imagenproducto.Image = Proyecto_restaurante.Properties.Resources.paisaje;
                                }
                            }
                        }
                    }

                    // Habilitar campos para edición
                    txtcodigo_barras.Enabled = true;
                    codigobarrarandombtn.Enabled = true;
                    txtnombre_prod.Enabled = true;
                    txtprecio_compra.Enabled = true;
                    buscarcateg.Enabled = true;
                    unidadmedida.Enabled = true;
                    guardarbtn.Enabled = true;
                    limpiarbtn.Enabled = true;

                    if (ingrediente == 1)
                    {
                        txtprecio_venta.Enabled = false;
                        autoCalcular.Checked = false;
                        autoCalcular.Enabled = false;
                        seleccionpanel.Enabled = false;
                        seleccionpanel.Visible = false;
                        recetaingredientes.Rows.Clear();
                        ingredientesconsulta.DataSource = null;
                    }
                    else
                    {
                        txtprecio_venta.Enabled = true;
                        autoCalcular.Enabled = true;
                        seleccionpanel.Enabled = true;
                        seleccionpanel.Visible = true;

                        // Cargar receta del producto si aplica
                        if (recetaingredientes.Columns.Count == 0)
                        {
                            recetaingredientes.Columns.Add("ID", "ID");
                            recetaingredientes.Columns.Add("Ingrediente", "Ingrediente");
                            recetaingredientes.Columns.Add("Medida", "Medida");
                            recetaingredientes.Columns.Add("Costo", "Costo");
                            recetaingredientes.Columns.Add("Cantidad", "Cantidad");
                        }

                        recetaingredientes.Rows.Clear();
                        string queryReceta = @"
                            SELECT 
                                R.IdIngrediente,
                                P.Nombre AS Ingrediente,
                                UM.Nombre AS Medida,
                                P.PrecioCompra AS Costo,
                                R.Cantidad
                            FROM Receta R
                            INNER JOIN ProductoVenta P ON R.IdIngrediente = P.IdProducto
                            INNER JOIN UnidadMedida UM ON R.IdUnidadMedida = UM.IdUnidadMedida
                            WHERE R.IdProducto = @IdProducto AND R.Activo = 1";

                        using (SqlCommand cmdR = new SqlCommand(queryReceta, conexion))
                        {
                            cmdR.Parameters.AddWithValue("@IdProducto", idProd);
                            using (SqlDataReader drR = cmdR.ExecuteReader())
                            {
                                while (drR.Read())
                                {
                                    recetaingredientes.Rows.Add(
                                        drR["IdIngrediente"].ToString(),
                                        drR["Ingrediente"].ToString(),
                                        drR["Medida"].ToString(),
                                        drR["Costo"] != DBNull.Value ? Convert.ToDecimal(drR["Costo"]).ToString("N2") : "0",
                                        drR["Cantidad"] != DBNull.Value ? Convert.ToDecimal(drR["Cantidad"]).ToString("N2") : "1"
                                    );
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los datos del producto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private int ObtenerIdUnidadMedida(SqlConnection conexion, string nombreUM)
        {
            string query = "SELECT IdUnidadMedida FROM UnidadMedida WHERE Nombre = @Nombre";

            using (SqlCommand cmd = new SqlCommand(query, conexion))
            {
                cmd.Parameters.AddWithValue("@Nombre", nombreUM);
                object result = cmd.ExecuteScalar();

                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        private decimal ObtenerValorItbis()
        {
            if (ITBIS == null || ITBIS.SelectedItem == null)
                return 0m;

            string texto = ITBIS.SelectedItem.ToString().Trim();

            if (string.Equals(texto, "Excento", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(texto, "Exento", StringComparison.OrdinalIgnoreCase))
            {
                return 0m;
            }

            if (decimal.TryParse(texto, out decimal valor))
            {
                return valor;
            }

            return 0m;
        }

        private void guardarbtn_Click(object sender, EventArgs e)
        {
            Regex numerosRegex = new Regex(@"^\d+([.,]\d+)?$");

            if (ingrediente == 1)
            {
                if (string.IsNullOrWhiteSpace(txtcodigo_barras.Text) ||
                    string.IsNullOrWhiteSpace(txtnombre_prod.Text) ||
                    ITBIS.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(idcategoriatxt.Text) ||
                    unidadmedida.SelectedValue == null)
                {
                    MessageBox.Show("Debe completar todos los campos obligatorios para ingredientes.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!numerosRegex.IsMatch(txtprecio_compra.Text))
                {
                    MessageBox.Show("El precio de compra solo admite números.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txtcodigo_barras.Text) ||
                    string.IsNullOrWhiteSpace(txtnombre_prod.Text) ||
                    ITBIS.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(idcategoriatxt.Text) ||
                    unidadmedida.SelectedValue == null)
                {
                    MessageBox.Show("Debe completar todos los campos obligatorios del producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!numerosRegex.IsMatch(txtprecio_compra.Text) ||
                    !numerosRegex.IsMatch(txtprecio_venta.Text))
                {
                    MessageBox.Show("Los precios solo admiten números.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                try
                {
                    conexion.Open();

                    if (string.IsNullOrEmpty(CodigoProductoActual))
                    {
                        decimal valorItbis = ObtenerValorItbis();
                        decimal precioItbis = 0m;

                        if (valorItbis > 0 && !string.IsNullOrWhiteSpace(txtprecio_venta.Text) && decimal.TryParse(txtprecio_venta.Text, out decimal pVenta))
                        {
                            precioItbis = pVenta * (valorItbis / 100);
                        }

                        string queryInsertar = @"
                        INSERT INTO ProductoVenta
                        (Nombre, IdCategoria, IdProductoTipo, Activo, PrecioCompra, PrecioVenta, Itbis, CodigoBarra, IdUnidadMedida, Existencia, ItbisPrecio, ImagenProducto)
                        OUTPUT INSERTED.IdProducto
                        VALUES (@Nombre, @IdCategoria, @IdProductoTipo, @Activo, @PrecioCompra, @PrecioVenta, @Itbis, @CodigoBarra, @IdUnidadMedida, @Existencia, @ItbisPrecio, @ImagenProducto)";

                        using (SqlCommand insertarCommand = new SqlCommand(queryInsertar, conexion))
                        {
                            insertarCommand.Parameters.AddWithValue("@Nombre", txtnombre_prod.Text);
                            insertarCommand.Parameters.AddWithValue("@IdCategoria", Convert.ToInt32(idcategoriatxt.Text));
                            insertarCommand.Parameters.AddWithValue("@IdProductoTipo", Convert.ToInt32(tipoproductocmbx.SelectedValue));
                            insertarCommand.Parameters.AddWithValue("@Activo", estadochk.Checked ? 1 : 0);
                            insertarCommand.Parameters.AddWithValue("@PrecioCompra", Convert.ToDecimal(txtprecio_compra.Text));
                            insertarCommand.Parameters.AddWithValue("@CodigoBarra", txtcodigo_barras.Text);
                            insertarCommand.Parameters.AddWithValue("@IdUnidadMedida", Convert.ToInt32(unidadmedida.SelectedValue));
                            insertarCommand.Parameters.AddWithValue("@Existencia", 0);
                            insertarCommand.Parameters.AddWithValue("@Itbis", valorItbis);
                            insertarCommand.Parameters.AddWithValue("@ItbisPrecio", precioItbis);
                            insertarCommand.Parameters.AddWithValue("@PrecioVenta",
                                string.IsNullOrWhiteSpace(txtprecio_venta.Text)
                                ? (object)DBNull.Value
                                : Convert.ToDecimal(txtprecio_venta.Text));
                            insertarCommand.Parameters.Add("@ImagenProducto", SqlDbType.VarBinary).Value = (object)imagenBytesProducto ?? DBNull.Value;

                            int nuevoIdProducto = (int)insertarCommand.ExecuteScalar();

                            foreach (DataGridViewRow fila in recetaingredientes.Rows)
                            {
                                if (fila.IsNewRow) continue;

                                int idIngrediente = Convert.ToInt32(fila.Cells[0].Value);
                                string medidaTexto = fila.Cells[2].Value.ToString();
                                decimal cantidad = Convert.ToDecimal(fila.Cells[3].Value);

                                int idUnidadMedida = ObtenerIdUnidadMedida(conexion, medidaTexto);

                                string queryReceta = @"
                                INSERT INTO Receta (IdProducto, IdIngrediente, IdUnidadMedida, Cantidad, Activo)
                                VALUES (@IdProducto, @IdIngrediente, @IdUnidadMedida, @Cantidad, 1)";

                                using (SqlCommand cmdReceta = new SqlCommand(queryReceta, conexion))
                                {
                                    cmdReceta.Parameters.AddWithValue("@IdProducto", nuevoIdProducto);
                                    cmdReceta.Parameters.AddWithValue("@IdIngrediente", idIngrediente);
                                    cmdReceta.Parameters.AddWithValue("@IdUnidadMedida", idUnidadMedida);
                                    cmdReceta.Parameters.AddWithValue("@Cantidad", cantidad);

                                    cmdReceta.ExecuteNonQuery();
                                }
                            }

                            MessageBox.Show("Producto registrado con éxito.");

                            limpiarbtn_Click(sender, e);
                            ConsProductos_Load(sender, e);
                        }
                    }
                    else
                    {
                        decimal valorItbis = ObtenerValorItbis();

                        string queryActualizar = @"
                        UPDATE ProductoVenta SET
                            Nombre = @Nombre,
                            IdCategoria = @IdCategoria,
                            IdProductoTipo = @IdProductoTipo,
                            Activo = @Activo,
                            PrecioCompra = @PrecioCompra,
                            PrecioVenta = @PrecioVenta,
                            Itbis = @Itbis,
                            CodigoBarra = @CodigoBarra,
                            IdUnidadMedida = @IdUnidadMedida,
                            ImagenProducto = @ImagenProducto
                        WHERE IdProducto = @IdProducto";

                        using (SqlCommand actualizarCommand = new SqlCommand(queryActualizar, conexion))
                        {
                            actualizarCommand.Parameters.AddWithValue("@IdProducto", idProducto);
                            actualizarCommand.Parameters.AddWithValue("@Nombre", txtnombre_prod.Text);
                            actualizarCommand.Parameters.AddWithValue("@IdCategoria", Convert.ToInt32(idcategoriatxt.Text));
                            actualizarCommand.Parameters.AddWithValue("@IdProductoTipo", Convert.ToInt32(tipoproductocmbx.SelectedValue));
                            actualizarCommand.Parameters.AddWithValue("@Activo", estadochk.Checked ? 1 : 0);
                            actualizarCommand.Parameters.AddWithValue("@PrecioCompra", Convert.ToDecimal(txtprecio_compra.Text));
                            actualizarCommand.Parameters.AddWithValue("@PrecioVenta",
                                string.IsNullOrWhiteSpace(txtprecio_venta.Text)
                                ? (object)DBNull.Value
                                : Convert.ToDecimal(txtprecio_venta.Text));
                            actualizarCommand.Parameters.AddWithValue("@Itbis", valorItbis);
                            actualizarCommand.Parameters.AddWithValue("@CodigoBarra", txtcodigo_barras.Text);
                            actualizarCommand.Parameters.AddWithValue("@IdUnidadMedida", Convert.ToInt32(unidadmedida.SelectedValue));
                            actualizarCommand.Parameters.Add("@ImagenProducto", SqlDbType.VarBinary).Value = (object)imagenBytesProducto ?? DBNull.Value;

                            int rowsAffected = actualizarCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Producto actualizado con éxito.");
                                limpiarbtn_Click(sender, e);
                                ConsProductos_Load(sender, e);
                            }
                            else
                            {
                                MessageBox.Show("No se pudo actualizar el producto.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error: {ex.Message}");
                }
            }
        }

        private void ActualizarUltimoID()
        {
            try
            {
                string consultaUltimoID = "SELECT ISNULL(MAX(IdProducto), 0) + 1 FROM ProductoVenta";
                using (SqlConnection conexion = new SqlConnection(conexionString))
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand(consultaUltimoID, conexion))
                    {
                        object resultado = cmd.ExecuteScalar();
                        if (resultado != null && resultado != DBNull.Value)
                        {
                            ultimoID.Text = resultado.ToString();
                        }
                        else
                        {
                            ultimoID.Text = "1";
                        }
                    }
                }
            }
            catch
            {
                ultimoID.Text = "1";
            }
        }

        private void limpiarbtn_Click(object sender, EventArgs e)
        {
            idProducto = 0;
            CodigoProductoActual = string.Empty;
            imagenBytesProducto = null;
            ingrediente = 0;

            if (imagenproducto != null)
                imagenproducto.Image = Proyecto_restaurante.Properties.Resources.paisaje;
            if (imagenprod != null)
                imagenprod.Image = Proyecto_restaurante.Properties.Resources.paisaje;

            txtcodigo_barras.Clear();
            txtnombre_prod.Clear();
            txtprecio_compra.Clear();
            txtprecio_venta.Clear();
            idcategoriatxt.Clear();
            categoriatxt.Clear();

            idprodreceta.Clear();
            nombreprodreceta.Clear();
            unimedidareceta.Clear();
            costoIng.Clear();
            if (numCantidad != null)
                numCantidad.Value = numCantidad.Minimum;

            tipoproductocmbx.SelectedIndex = -1;
            ITBIS.SelectedIndex = -1;
            unidadmedida.SelectedIndex = -1;

            estadochk.Checked = true;

            if (ingredientesconsulta != null)
            {
                if (ingredientesconsulta.DataSource != null)
                    ingredientesconsulta.DataSource = null;
                else
                    ingredientesconsulta.Rows.Clear();
            }

            if (recetaingredientes != null)
            {
                recetaingredientes.Rows.Clear();
            }

            txtcodigo_barras.Enabled = false;
            codigobarrarandombtn.Enabled = false;
            txtnombre_prod.Enabled = false;
            txtprecio_compra.Enabled = false;
            txtprecio_venta.Enabled = false;
            buscarcateg.Enabled = false;
            unidadmedida.Enabled = false;
            guardarbtn.Enabled = false;
            seleccionpanel.Enabled = false;
            seleccionpanel.Visible = false;

            ActualizarUltimoID();
        }

        private string CodBarraRandom()
        {
            const string caracteresPermitidos = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            Random random = new Random();

            int longitudCodBarras = 12;

            char[] codigoBarrasArray = new char[longitudCodBarras];
            for (int i = 0; i < longitudCodBarras; i++)
            {
                codigoBarrasArray[i] = caracteresPermitidos[random.Next(caracteresPermitidos.Length)];
            }

            return new string(codigoBarrasArray);
        }

        private void codigobarrarandombtn_Click(object sender, EventArgs e)
        {
            string codigoBarras = CodBarraRandom();

            txtcodigo_barras.Text = codigoBarras;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ITBIS.Items.Clear();
            ConsProductos_Load(sender, e);
        }

        private void seleccionimagenbtn_Click(object sender, EventArgs e)
        {
            buscarcatedt = 0;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Archivos de imagen (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
                openFileDialog.Title = "Seleccionar imagen de producto";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        byte[] bytes = File.ReadAllBytes(openFileDialog.FileName);
                        imagenBytesProducto = bytes;

                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            if (imagenprod != null)
                                imagenprod.Image = Image.FromStream(ms);
                            if (imagenproducto != null)
                                imagenproducto.Image = Image.FromStream(new MemoryStream(bytes));
                        }

                        MessageBox.Show("Imagen cargada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al cargar la imagen: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public int buscarcatedt = 1;

        private void buscarcateg_Click(object sender, EventArgs e)
        {
            if (buscarcatedt == 1)
            {
                string categoria = "select IdCategoria, Nombre from CategoriaProducto where Activo = 1";

                SqlDataAdapter adaptador = new SqlDataAdapter(categoria, conexionString);

                DataTable dt = new DataTable();

                adaptador.Fill(dt);

                categoriaconsulta.DataSource = dt;

                categoriaconsulta.Columns["IdCategoria"].HeaderText = "ID";
                categoriaconsulta.Columns["Nombre"].HeaderText = "Nombre";

                buscarcateg.Image = Proyecto_restaurante.Properties.Resources.cancelar1;
                categoriapanel.Location = new Point(263, 175);
                categoriapanel.BringToFront();
                categoriapanel.Visible = true;
                buscarcatedt = 0;
            }
            else
            {
                buscarcateg.Image = Proyecto_restaurante.Properties.Resources.busqueda1;
                categoriapanel.Visible = false;
                categoriapanel.Location = new Point(263, 175);

                buscarcatedt = 1;
            }
        }

        private void ConfigurarColumnasTabla()
        {
            if (tabladatos == null || tabladatos.Columns.Count == 0) return;

            tabladatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            if (tabladatos.Columns.Contains("IdProducto"))
            {
                tabladatos.Columns["IdProducto"].HeaderText = "ID";
                tabladatos.Columns["IdProducto"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            if (tabladatos.Columns.Contains("CodigoBarra"))
            {
                tabladatos.Columns["CodigoBarra"].HeaderText = "Código";
                tabladatos.Columns["CodigoBarra"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            if (tabladatos.Columns.Contains("PrecioCompra"))
            {
                tabladatos.Columns["PrecioCompra"].HeaderText = "Costo";
                tabladatos.Columns["PrecioCompra"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                tabladatos.Columns["PrecioCompra"].DefaultCellStyle.Format = "N2";
            }
            if (tabladatos.Columns.Contains("PrecioVenta"))
            {
                tabladatos.Columns["PrecioVenta"].HeaderText = "Precio";
                tabladatos.Columns["PrecioVenta"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                tabladatos.Columns["PrecioVenta"].DefaultCellStyle.Format = "N2";
            }
            if (tabladatos.Columns.Contains("Existencia"))
            {
                tabladatos.Columns["Existencia"].HeaderText = "Existencia";
                tabladatos.Columns["Existencia"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                tabladatos.Columns["Existencia"].DefaultCellStyle.Format = "N2";
            }
            if (tabladatos.Columns.Contains("Nombre"))
            {
                tabladatos.Columns["Nombre"].HeaderText = "Nombre";
                tabladatos.Columns["Nombre"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                tabladatos.Columns["Nombre"].MinimumWidth = 180;
            }
        }

        private void CargarProductosConsulta()
        {
            string busqueda = txtbuscador.Text.Trim();

            StringBuilder sql = new StringBuilder();
            sql.Append(@"
                SELECT 
                    PV.IdProducto, 
                    PV.CodigoBarra, 
                    PV.Nombre, 
                    PV.PrecioCompra, 
                    PV.PrecioVenta, 
                    PV.Existencia 
                FROM ProductoVenta PV
                LEFT JOIN ProductoTipo PT ON PV.IdProductoTipo = PT.IdProductoTipo
                WHERE 1 = 1 ");

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                sql.Append(@" AND (
                    PV.Nombre LIKE @Busqueda OR 
                    PV.CodigoBarra LIKE @Busqueda OR 
                    CAST(PV.IdProducto AS VARCHAR) LIKE @Busqueda
                )");
            }

            if (filtroingredientes.Checked)
            {
                sql.Append(" AND PT.Ingrediente = 1");
            }
            else if (filtroplatos.Checked)
            {
                sql.Append(" AND (PT.Ingrediente = 0 OR PT.Ingrediente IS NULL) AND (PT.Bebida = 0 OR PT.Bebida IS NULL) AND (PT.Adicion = 0 OR PT.Adicion IS NULL)");
            }
            else if (filtrobebida.Checked)
            {
                sql.Append(" AND PT.Bebida = 1");
            }
            else if (filtroadicion.Checked)
            {
                sql.Append(" AND PT.Adicion = 1");
            }

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                try
                {
                    conexion.Open();
                    using (SqlCommand cmd = new SqlCommand(sql.ToString(), conexion))
                    {
                        if (!string.IsNullOrWhiteSpace(busqueda))
                        {
                            cmd.Parameters.AddWithValue("@Busqueda", "%" + busqueda + "%");
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            tabladatos.DataSource = dt;
                        }
                    }

                    ConfigurarColumnasTabla();
                }
                catch
                {
                    // Manejo silencioso en consulta
                }
            }
        }

        private void txtbuscador_TextChanged(object sender, EventArgs e)
        {
            CargarProductosConsulta();
        }

        private void eliminarbtn_Click(object sender, EventArgs e)
        {
            txtbuscador.Clear();
            filtrotodos.Checked = true;
            CargarProductosConsulta();
        }

        private void filtrotodos_CheckedChanged(object sender, EventArgs e)
        {
            if (filtrotodos.Checked == true)
            {
                filtroingredientes.Checked = false;
                filtroplatos.Checked = false;
                filtrobebida.Checked = false;
                filtroadicion.Checked = false;
            }
            CargarProductosConsulta();
        }

        private void filtroplatos_CheckedChanged(object sender, EventArgs e)
        {
            if (filtroplatos.Checked == true)
            {
                filtrotodos.Checked = false;
                filtroingredientes.Checked = false;
                filtrobebida.Checked = false;
                filtroadicion.Checked = false;
            }
            CargarProductosConsulta();
        }

        private void filtroingredientes_CheckedChanged(object sender, EventArgs e)
        {
            if (filtroingredientes.Checked == true)
            {
                filtrotodos.Checked = false;
                filtroplatos.Checked = false;
                filtroadicion.Checked = false;
                filtrobebida.Checked = false;
            }
            CargarProductosConsulta();
        }

        private void filtroadicion_CheckedChanged(object sender, EventArgs e)
        {
            if (filtroadicion.Checked == true)
            {
                filtrotodos.Checked = false;
                filtroplatos.Checked = false;
                filtroingredientes.Checked = false;
                filtrobebida.Checked = false;
            }
            CargarProductosConsulta();
        }

        private void filtrobebida_CheckedChanged(object sender, EventArgs e)
        {
            if (filtrobebida.Checked == true)
            {
                filtrotodos.Checked = false;
                filtroplatos.Checked = false;
                filtroingredientes.Checked = false;
                filtroadicion.Checked = false;
            }
            CargarProductosConsulta();
        }

        private void categoriaconsulta_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            idcategoriatxt.Text = categoriaconsulta.SelectedCells[0].Value.ToString();
            categoriatxt.Text = categoriaconsulta.SelectedCells[1].Value.ToString();
            buscarcateg_Click(sender, e);
        }

        private void ingredientesconsulta_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            idprodreceta.Text = ingredientesconsulta.SelectedCells[0].Value.ToString();
            nombreprodreceta.Text = ingredientesconsulta.SelectedCells[1].Value.ToString();
            costoIng.Text = ingredientesconsulta.SelectedCells[2].Value.ToString();
            unimedidareceta.Text = ingredientesconsulta.SelectedCells[3].Value.ToString();

            numCantidad.Focus();
        }

        private void agregarbtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(idprodreceta.Text))
            {
                MessageBox.Show("Seleccione el ingrediente para agregar.");
                return;
            }

            recetaingredientes.Rows.Add(
                idprodreceta.Text,
                nombreprodreceta.Text,
                unimedidareceta.Text,
                costoIng.Text,
                numCantidad.Value
            );

            idprodreceta.Clear();
            nombreprodreceta.Clear();
            unimedidareceta.Clear();
            costoIng.Clear();
            numCantidad.Value = 1;

            ActualizarPrecioVenta();
        }

        private void numCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                agregarbtn_Click(sender, e);
                e.Handled = true;
            }
        }

        private void autoCalcular_CheckedChanged(object sender, EventArgs e)
        {
            bool auto = autoCalcular.Checked;

            txtprecio_compra.Enabled = !auto;
            txtprecio_venta.Enabled = !auto;

            if (auto)
            {
                ActualizarPrecioVenta();
            }
            else
            {
                txtprecio_venta.Clear();
                txtprecio_compra.Clear();
            }
        }


        private decimal CalcularCostoPlato()
        {
            decimal costoTotal = 0m;

            using (SqlConnection cn = new SqlConnection(conexionString))
            {
                cn.Open();

                foreach (DataGridViewRow fila in recetaingredientes.Rows)
                {
                    if (fila.IsNewRow) continue;

                    int idProductoIng = Convert.ToInt32(fila.Cells["ID"].Value);
                    decimal cantidadReceta = Convert.ToDecimal(fila.Cells["Cantidad"].Value);

                    string sql = @"
                    SELECT PV.PrecioCompra, UM.Valor
                    FROM ProductoVenta PV
                    INNER JOIN UnidadMedida UM ON PV.IdUnidadMedida = UM.IdUnidadMedida
                    WHERE PV.IdProducto = @idProducto";

                    using (SqlCommand cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.AddWithValue("@idProducto", idProductoIng);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                decimal precioCompra = Convert.ToDecimal(dr["PrecioCompra"]);
                                decimal valorUnidad = Convert.ToDecimal(dr["Valor"]);

                                decimal costoUnitario = precioCompra / valorUnidad;
                                decimal costoIngrediente = costoUnitario * cantidadReceta;

                                costoTotal += costoIngrediente;
                            }
                        }
                    }
                }
            }

            txtprecio_compra.Text = costoTotal.ToString("N2");

            return costoTotal;
        }

        private decimal CalcularPrecioVenta(decimal costoPlato)
        {
            decimal porcentaje = PorcGanancia;
            return costoPlato * (1 + (porcentaje / 100));
        }

        private void ActualizarPrecioVenta()
        {
            if (!autoCalcular.Checked)
                return;

            decimal costo = CalcularCostoPlato();
            decimal precioVenta = CalcularPrecioVenta(costo);

            txtprecio_venta.Text = precioVenta.ToString("N2");
        }

        private void categoriaconsulta_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}