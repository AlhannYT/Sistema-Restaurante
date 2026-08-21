namespace Proyecto_restaurante
{
    partial class inicio
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(inicio));
            pictureBox1 = new PictureBox();
            txtusuario = new TextBox();
            txtpass = new TextBox();
            iniciobtn = new Button();
            panel1 = new Panel();
            passView = new CheckBox();
            button1 = new Button();
            iniciolabel = new Label();
            button2 = new Button();
            sqlbtn = new Button();
            toolTip1 = new ToolTip(components);
            recordarchk = new CheckBox();
            alerta = new PictureBox();
            AvisoDBIMG = new Panel();
            pictureBox3 = new PictureBox();
            conexionpanel = new Panel();
            progressBar1 = new ProgressBar();
            CrearDBbtn = new Button();
            button6 = new Button();
            button3 = new Button();
            defectochk = new CheckBox();
            contservidortxt = new TextBox();
            DBTxt = new TextBox();
            usuarioservidortxt = new TextBox();
            servidortxt = new TextBox();
            salirsqlbtn = new Button();
            guardarbtn = new Button();
            label8 = new Label();
            label5 = new Label();
            label4 = new Label();
            label6 = new Label();
            label3 = new Label();
            txtsql = new DataGridView();
            conexiones = new Panel();
            borrarconex = new Button();
            button5 = new Button();
            button4 = new Button();
            usuarioimagen = new PictureBox();
            contraimagen = new PictureBox();
            autorizar = new Panel();
            salirAutorBTN = new Button();
            autorizarBTN = new Button();
            label7 = new Label();
            contAdmin = new TextBox();
            usuAdmin = new TextBox();
            label1 = new Label();
            label2 = new Label();
            panelSesion = new Panel();
            panelSerial = new Panel();
            quitarPanelSerial = new Button();
            generarSerial = new Button();
            pegarClipb = new Button();
            validarSerial = new Button();
            label9 = new Label();
            label10 = new Label();
            codigoSerial = new TextBox();
            actualizarSistema = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)alerta).BeginInit();
            AvisoDBIMG.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            conexionpanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtsql).BeginInit();
            conexiones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)usuarioimagen).BeginInit();
            ((System.ComponentModel.ISupportInitialize)contraimagen).BeginInit();
            autorizar.SuspendLayout();
            panelSesion.SuspendLayout();
            panelSerial.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Image = Properties.Resources.comidapedido2;
            pictureBox1.Location = new Point(43, 55);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(178, 197);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // txtusuario
            // 
            txtusuario.CharacterCasing = CharacterCasing.Upper;
            txtusuario.Font = new Font("Segoe UI", 12F);
            txtusuario.Location = new Point(315, 115);
            txtusuario.Name = "txtusuario";
            txtusuario.PlaceholderText = "Usuario";
            txtusuario.Size = new Size(218, 29);
            txtusuario.TabIndex = 1;
            txtusuario.TextChanged += txtusuario_TextChanged;
            txtusuario.KeyPress += txtusuario_KeyPress;
            // 
            // txtpass
            // 
            txtpass.CharacterCasing = CharacterCasing.Upper;
            txtpass.Font = new Font("Segoe UI", 12F);
            txtpass.Location = new Point(315, 157);
            txtpass.Name = "txtpass";
            txtpass.PlaceholderText = "Contraseña";
            txtpass.Size = new Size(218, 29);
            txtpass.TabIndex = 2;
            txtpass.UseSystemPasswordChar = true;
            txtpass.KeyPress += txtpass_KeyPress;
            // 
            // iniciobtn
            // 
            iniciobtn.Cursor = Cursors.Hand;
            iniciobtn.Image = Properties.Resources.entrar1;
            iniciobtn.ImageAlign = ContentAlignment.MiddleLeft;
            iniciobtn.Location = new Point(343, 213);
            iniciobtn.Name = "iniciobtn";
            iniciobtn.Size = new Size(142, 29);
            iniciobtn.TabIndex = 3;
            iniciobtn.Text = "Iniciar Sesión";
            iniciobtn.UseVisualStyleBackColor = true;
            iniciobtn.Click += iniciobtn_Click;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Highlight;
            panel1.Location = new Point(253, 55);
            panel1.Name = "panel1";
            panel1.Size = new Size(5, 197);
            panel1.TabIndex = 4;
            // 
            // passView
            // 
            passView.Appearance = Appearance.Button;
            passView.BackColor = SystemColors.Window;
            passView.Cursor = Cursors.Hand;
            passView.FlatStyle = FlatStyle.Flat;
            passView.ForeColor = SystemColors.Window;
            passView.Image = Properties.Resources.ojo;
            passView.Location = new Point(504, 158);
            passView.Name = "passView";
            passView.Size = new Size(27, 26);
            passView.TabIndex = 6;
            passView.UseVisualStyleBackColor = false;
            passView.CheckedChanged += passView_CheckedChanged;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(64, 64, 64);
            button1.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            button1.FlatStyle = FlatStyle.Flat;
            button1.Image = Properties.Resources.minimizar_ventana__1_;
            button1.ImageAlign = ContentAlignment.TopCenter;
            button1.Location = new Point(494, 3);
            button1.Name = "button1";
            button1.Size = new Size(29, 27);
            button1.TabIndex = 3;
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // iniciolabel
            // 
            iniciolabel.AutoSize = true;
            iniciolabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            iniciolabel.ForeColor = Color.White;
            iniciolabel.Location = new Point(349, 68);
            iniciolabel.Name = "iniciolabel";
            iniciolabel.Size = new Size(130, 21);
            iniciolabel.TabIndex = 8;
            iniciolabel.Text = "Inicio de Sesión";
            // 
            // button2
            // 
            button2.BackColor = Color.FromArgb(64, 64, 64);
            button2.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            button2.FlatStyle = FlatStyle.Flat;
            button2.Image = Properties.Resources.cruz__1_;
            button2.ImageAlign = ContentAlignment.TopCenter;
            button2.Location = new Point(528, 3);
            button2.Name = "button2";
            button2.Size = new Size(29, 27);
            button2.TabIndex = 3;
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // sqlbtn
            // 
            sqlbtn.Cursor = Cursors.Hand;
            sqlbtn.Image = Properties.Resources.sql;
            sqlbtn.Location = new Point(13, 9);
            sqlbtn.Name = "sqlbtn";
            sqlbtn.Size = new Size(36, 36);
            sqlbtn.TabIndex = 9;
            toolTip1.SetToolTip(sqlbtn, "Conexion SQL");
            sqlbtn.UseVisualStyleBackColor = true;
            sqlbtn.Visible = false;
            sqlbtn.Click += sqlbtn_Click;
            // 
            // recordarchk
            // 
            recordarchk.Appearance = Appearance.Button;
            recordarchk.BackColor = SystemColors.Window;
            recordarchk.Cursor = Cursors.Hand;
            recordarchk.FlatStyle = FlatStyle.Flat;
            recordarchk.Font = new Font("Segoe UI", 10F);
            recordarchk.ForeColor = SystemColors.Window;
            recordarchk.Image = Properties.Resources.disco;
            recordarchk.Location = new Point(504, 116);
            recordarchk.Name = "recordarchk";
            recordarchk.Size = new Size(27, 26);
            recordarchk.TabIndex = 13;
            recordarchk.Text = "   ";
            toolTip1.SetToolTip(recordarchk, "Recordar Usuario");
            recordarchk.UseVisualStyleBackColor = false;
            recordarchk.CheckedChanged += recordarchk_CheckedChanged;
            // 
            // alerta
            // 
            alerta.Image = Properties.Resources.exclamacion;
            alerta.Location = new Point(38, 10);
            alerta.Name = "alerta";
            alerta.Size = new Size(24, 24);
            alerta.SizeMode = PictureBoxSizeMode.AutoSize;
            alerta.TabIndex = 14;
            alerta.TabStop = false;
            toolTip1.SetToolTip(alerta, "Base de Datos Faltante");
            // 
            // AvisoDBIMG
            // 
            AvisoDBIMG.Controls.Add(pictureBox3);
            AvisoDBIMG.Controls.Add(alerta);
            AvisoDBIMG.Location = new Point(17, 5);
            AvisoDBIMG.Name = "AvisoDBIMG";
            AvisoDBIMG.Size = new Size(69, 43);
            AvisoDBIMG.TabIndex = 15;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = Properties.Resources.servidor_sql2;
            pictureBox3.Location = new Point(7, 10);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(24, 24);
            pictureBox3.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox3.TabIndex = 14;
            pictureBox3.TabStop = false;
            // 
            // conexionpanel
            // 
            conexionpanel.BackColor = Color.Gray;
            conexionpanel.Controls.Add(progressBar1);
            conexionpanel.Controls.Add(CrearDBbtn);
            conexionpanel.Controls.Add(button6);
            conexionpanel.Controls.Add(button3);
            conexionpanel.Controls.Add(defectochk);
            conexionpanel.Controls.Add(contservidortxt);
            conexionpanel.Controls.Add(DBTxt);
            conexionpanel.Controls.Add(usuarioservidortxt);
            conexionpanel.Controls.Add(servidortxt);
            conexionpanel.Controls.Add(salirsqlbtn);
            conexionpanel.Controls.Add(guardarbtn);
            conexionpanel.Controls.Add(label8);
            conexionpanel.Controls.Add(label5);
            conexionpanel.Controls.Add(label4);
            conexionpanel.Controls.Add(label6);
            conexionpanel.Controls.Add(label3);
            conexionpanel.Location = new Point(605, 45);
            conexionpanel.Name = "conexionpanel";
            conexionpanel.Size = new Size(564, 368);
            conexionpanel.TabIndex = 10;
            conexionpanel.Visible = false;
            // 
            // progressBar1
            // 
            progressBar1.ForeColor = Color.FromArgb(0, 192, 0);
            progressBar1.Location = new Point(76, 310);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(412, 23);
            progressBar1.TabIndex = 7;
            progressBar1.Visible = false;
            // 
            // CrearDBbtn
            // 
            CrearDBbtn.Cursor = Cursors.Hand;
            CrearDBbtn.Image = (Image)resources.GetObject("CrearDBbtn.Image");
            CrearDBbtn.ImageAlign = ContentAlignment.MiddleLeft;
            CrearDBbtn.Location = new Point(425, 15);
            CrearDBbtn.Name = "CrearDBbtn";
            CrearDBbtn.Size = new Size(136, 32);
            CrearDBbtn.TabIndex = 6;
            CrearDBbtn.Text = "Crear Base de datos";
            CrearDBbtn.TextAlign = ContentAlignment.MiddleRight;
            CrearDBbtn.UseVisualStyleBackColor = true;
            CrearDBbtn.Click += CrearDBbtn_Click;
            // 
            // button6
            // 
            button6.Image = Properties.Resources.limpio;
            button6.Location = new Point(442, 61);
            button6.Name = "button6";
            button6.Size = new Size(31, 25);
            button6.TabIndex = 5;
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // button3
            // 
            button3.Image = Properties.Resources.busqueda;
            button3.Location = new Point(17, 13);
            button3.Name = "button3";
            button3.Size = new Size(32, 32);
            button3.TabIndex = 4;
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // defectochk
            // 
            defectochk.AutoSize = true;
            defectochk.CheckAlign = ContentAlignment.MiddleRight;
            defectochk.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            defectochk.ForeColor = Color.White;
            defectochk.Location = new Point(204, 207);
            defectochk.Name = "defectochk";
            defectochk.Size = new Size(194, 25);
            defectochk.TabIndex = 3;
            defectochk.Text = "Conexion por defecto";
            defectochk.UseVisualStyleBackColor = true;
            // 
            // contservidortxt
            // 
            contservidortxt.Location = new Point(183, 172);
            contservidortxt.Name = "contservidortxt";
            contservidortxt.Size = new Size(253, 23);
            contservidortxt.TabIndex = 2;
            contservidortxt.UseSystemPasswordChar = true;
            // 
            // DBTxt
            // 
            DBTxt.Location = new Point(183, 100);
            DBTxt.Name = "DBTxt";
            DBTxt.Size = new Size(253, 23);
            DBTxt.TabIndex = 2;
            // 
            // usuarioservidortxt
            // 
            usuarioservidortxt.Location = new Point(183, 136);
            usuarioservidortxt.Name = "usuarioservidortxt";
            usuarioservidortxt.Size = new Size(253, 23);
            usuarioservidortxt.TabIndex = 2;
            // 
            // servidortxt
            // 
            servidortxt.Location = new Point(183, 64);
            servidortxt.Name = "servidortxt";
            servidortxt.Size = new Size(253, 23);
            servidortxt.TabIndex = 2;
            // 
            // salirsqlbtn
            // 
            salirsqlbtn.Image = Properties.Resources.salida;
            salirsqlbtn.ImageAlign = ContentAlignment.MiddleLeft;
            salirsqlbtn.Location = new Point(320, 242);
            salirsqlbtn.Name = "salirsqlbtn";
            salirsqlbtn.Size = new Size(104, 41);
            salirsqlbtn.TabIndex = 1;
            salirsqlbtn.Text = "Salir";
            salirsqlbtn.TextAlign = ContentAlignment.MiddleRight;
            salirsqlbtn.UseVisualStyleBackColor = true;
            salirsqlbtn.Click += salirsqlbtn_Click;
            // 
            // guardarbtn
            // 
            guardarbtn.Image = Properties.Resources.disco;
            guardarbtn.ImageAlign = ContentAlignment.MiddleLeft;
            guardarbtn.Location = new Point(150, 242);
            guardarbtn.Name = "guardarbtn";
            guardarbtn.Size = new Size(104, 41);
            guardarbtn.TabIndex = 1;
            guardarbtn.Text = "Guardar";
            guardarbtn.TextAlign = ContentAlignment.MiddleRight;
            guardarbtn.UseVisualStyleBackColor = true;
            guardarbtn.Click += guardarbtn_Click;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label8.ForeColor = Color.White;
            label8.Location = new Point(61, 99);
            label8.Name = "label8";
            label8.Size = new Size(118, 21);
            label8.TabIndex = 0;
            label8.Text = "Base De Datos";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label5.ForeColor = Color.White;
            label5.Location = new Point(61, 171);
            label5.Name = "label5";
            label5.Size = new Size(96, 21);
            label5.TabIndex = 0;
            label5.Text = "Contraseña";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label4.ForeColor = Color.White;
            label4.Location = new Point(61, 135);
            label4.Name = "label4";
            label4.Size = new Size(69, 21);
            label4.TabIndex = 0;
            label4.Text = "Usuario";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.ForeColor = Color.White;
            label6.Location = new Point(172, 12);
            label6.Name = "label6";
            label6.Size = new Size(221, 32);
            label6.TabIndex = 0;
            label6.Text = "Conexion con SQL\r\n";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label3.ForeColor = Color.White;
            label3.Location = new Point(61, 63);
            label3.Name = "label3";
            label3.Size = new Size(75, 21);
            label3.TabIndex = 0;
            label3.Text = "Servidor";
            // 
            // txtsql
            // 
            txtsql.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            txtsql.Location = new Point(9, 9);
            txtsql.MultiSelect = false;
            txtsql.Name = "txtsql";
            txtsql.RowHeadersWidth = 51;
            txtsql.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            txtsql.Size = new Size(482, 243);
            txtsql.TabIndex = 11;
            txtsql.CellDoubleClick += txtsql_CellDoubleClick;
            // 
            // conexiones
            // 
            conexiones.Controls.Add(borrarconex);
            conexiones.Controls.Add(txtsql);
            conexiones.Controls.Add(button5);
            conexiones.Controls.Add(button4);
            conexiones.Location = new Point(605, 416);
            conexiones.Name = "conexiones";
            conexiones.Size = new Size(564, 368);
            conexiones.TabIndex = 12;
            conexiones.Visible = false;
            // 
            // borrarconex
            // 
            borrarconex.Image = Properties.Resources.basura;
            borrarconex.Location = new Point(497, 9);
            borrarconex.Name = "borrarconex";
            borrarconex.Size = new Size(52, 41);
            borrarconex.TabIndex = 12;
            borrarconex.UseVisualStyleBackColor = true;
            borrarconex.Click += borrarconex_Click;
            // 
            // button5
            // 
            button5.Image = Properties.Resources.cancelar1;
            button5.ImageAlign = ContentAlignment.MiddleLeft;
            button5.Location = new Point(320, 269);
            button5.Name = "button5";
            button5.Size = new Size(104, 41);
            button5.TabIndex = 1;
            button5.Text = "Cancelar";
            button5.TextAlign = ContentAlignment.MiddleRight;
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button4
            // 
            button4.Image = Properties.Resources.check;
            button4.ImageAlign = ContentAlignment.MiddleLeft;
            button4.Location = new Point(150, 269);
            button4.Name = "button4";
            button4.Size = new Size(104, 41);
            button4.TabIndex = 1;
            button4.Text = "Seleccionar";
            button4.TextAlign = ContentAlignment.MiddleRight;
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // usuarioimagen
            // 
            usuarioimagen.Image = Properties.Resources.persona2;
            usuarioimagen.Location = new Point(290, 116);
            usuarioimagen.Name = "usuarioimagen";
            usuarioimagen.Size = new Size(22, 28);
            usuarioimagen.SizeMode = PictureBoxSizeMode.Zoom;
            usuarioimagen.TabIndex = 14;
            usuarioimagen.TabStop = false;
            // 
            // contraimagen
            // 
            contraimagen.Image = Properties.Resources.clave;
            contraimagen.Location = new Point(290, 158);
            contraimagen.Name = "contraimagen";
            contraimagen.Size = new Size(22, 28);
            contraimagen.SizeMode = PictureBoxSizeMode.Zoom;
            contraimagen.TabIndex = 14;
            contraimagen.TabStop = false;
            // 
            // autorizar
            // 
            autorizar.BackColor = Color.DimGray;
            autorizar.Controls.Add(salirAutorBTN);
            autorizar.Controls.Add(autorizarBTN);
            autorizar.Controls.Add(label7);
            autorizar.Controls.Add(contAdmin);
            autorizar.Controls.Add(usuAdmin);
            autorizar.Controls.Add(label1);
            autorizar.Controls.Add(label2);
            autorizar.Location = new Point(57, 404);
            autorizar.Margin = new Padding(3, 2, 3, 2);
            autorizar.Name = "autorizar";
            autorizar.Size = new Size(393, 244);
            autorizar.TabIndex = 16;
            autorizar.Visible = false;
            // 
            // salirAutorBTN
            // 
            salirAutorBTN.Cursor = Cursors.Hand;
            salirAutorBTN.Image = Properties.Resources.salida;
            salirAutorBTN.ImageAlign = ContentAlignment.MiddleLeft;
            salirAutorBTN.Location = new Point(220, 179);
            salirAutorBTN.Name = "salirAutorBTN";
            salirAutorBTN.Size = new Size(154, 41);
            salirAutorBTN.TabIndex = 8;
            salirAutorBTN.Text = "Salir";
            salirAutorBTN.UseVisualStyleBackColor = true;
            salirAutorBTN.Click += salirAutorBTN_Click;
            // 
            // autorizarBTN
            // 
            autorizarBTN.Cursor = Cursors.Hand;
            autorizarBTN.Image = Properties.Resources.llave__1_;
            autorizarBTN.ImageAlign = ContentAlignment.MiddleLeft;
            autorizarBTN.Location = new Point(10, 179);
            autorizarBTN.Name = "autorizarBTN";
            autorizarBTN.Size = new Size(154, 41);
            autorizarBTN.TabIndex = 8;
            autorizarBTN.Text = "Autorizar";
            autorizarBTN.UseVisualStyleBackColor = true;
            autorizarBTN.Click += autorizarBTN_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.ForeColor = Color.White;
            label7.Location = new Point(100, 25);
            label7.Name = "label7";
            label7.Size = new Size(193, 32);
            label7.TabIndex = 7;
            label7.Text = "Clave de Admin";
            // 
            // contAdmin
            // 
            contAdmin.CharacterCasing = CharacterCasing.Upper;
            contAdmin.Location = new Point(116, 125);
            contAdmin.Name = "contAdmin";
            contAdmin.Size = new Size(258, 23);
            contAdmin.TabIndex = 5;
            contAdmin.KeyPress += contAdmin_KeyPress;
            // 
            // usuAdmin
            // 
            usuAdmin.CharacterCasing = CharacterCasing.Upper;
            usuAdmin.Location = new Point(116, 79);
            usuAdmin.Name = "usuAdmin";
            usuAdmin.Size = new Size(258, 23);
            usuAdmin.TabIndex = 6;
            usuAdmin.KeyPress += usuAdmin_KeyPress;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label1.ForeColor = Color.White;
            label1.Location = new Point(10, 124);
            label1.Name = "label1";
            label1.Size = new Size(96, 21);
            label1.TabIndex = 3;
            label1.Text = "Contraseña";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label2.ForeColor = Color.White;
            label2.Location = new Point(10, 78);
            label2.Name = "label2";
            label2.Size = new Size(69, 21);
            label2.TabIndex = 4;
            label2.Text = "Usuario";
            // 
            // panelSesion
            // 
            panelSesion.Controls.Add(sqlbtn);
            panelSesion.Controls.Add(AvisoDBIMG);
            panelSesion.Controls.Add(contraimagen);
            panelSesion.Controls.Add(usuarioimagen);
            panelSesion.Controls.Add(recordarchk);
            panelSesion.Controls.Add(pictureBox1);
            panelSesion.Controls.Add(iniciolabel);
            panelSesion.Controls.Add(passView);
            panelSesion.Controls.Add(panel1);
            panelSesion.Controls.Add(actualizarSistema);
            panelSesion.Controls.Add(iniciobtn);
            panelSesion.Controls.Add(txtpass);
            panelSesion.Controls.Add(txtusuario);
            panelSesion.Location = new Point(7, 36);
            panelSesion.Name = "panelSesion";
            panelSesion.Size = new Size(551, 336);
            panelSesion.TabIndex = 17;
            // 
            // panelSerial
            // 
            panelSerial.BackColor = Color.Gray;
            panelSerial.Controls.Add(quitarPanelSerial);
            panelSerial.Controls.Add(generarSerial);
            panelSerial.Controls.Add(pegarClipb);
            panelSerial.Controls.Add(validarSerial);
            panelSerial.Controls.Add(label9);
            panelSerial.Controls.Add(label10);
            panelSerial.Controls.Add(codigoSerial);
            panelSerial.Location = new Point(1189, 45);
            panelSerial.Name = "panelSerial";
            panelSerial.Size = new Size(564, 368);
            panelSerial.TabIndex = 18;
            panelSerial.Visible = false;
            // 
            // quitarPanelSerial
            // 
            quitarPanelSerial.Cursor = Cursors.Hand;
            quitarPanelSerial.Image = Properties.Resources.salida;
            quitarPanelSerial.ImageAlign = ContentAlignment.MiddleLeft;
            quitarPanelSerial.Location = new Point(290, 262);
            quitarPanelSerial.Name = "quitarPanelSerial";
            quitarPanelSerial.Size = new Size(154, 41);
            quitarPanelSerial.TabIndex = 8;
            quitarPanelSerial.Text = "Salir";
            quitarPanelSerial.UseVisualStyleBackColor = true;
            quitarPanelSerial.Click += quitarPanelSerial_Click;
            // 
            // generarSerial
            // 
            generarSerial.Cursor = Cursors.Hand;
            generarSerial.Image = Properties.Resources.bloqueo_binario__1_;
            generarSerial.Location = new Point(27, 18);
            generarSerial.Name = "generarSerial";
            generarSerial.Size = new Size(36, 36);
            generarSerial.TabIndex = 9;
            generarSerial.UseVisualStyleBackColor = true;
            generarSerial.Visible = false;
            generarSerial.Click += generarSerial_Click;
            // 
            // pegarClipb
            // 
            pegarClipb.Image = Properties.Resources.pegar;
            pegarClipb.Location = new Point(453, 174);
            pegarClipb.Name = "pegarClipb";
            pegarClipb.Size = new Size(29, 29);
            pegarClipb.TabIndex = 4;
            pegarClipb.UseVisualStyleBackColor = true;
            pegarClipb.Visible = false;
            pegarClipb.Click += pegarClipb_Click;
            // 
            // validarSerial
            // 
            validarSerial.Image = Properties.Resources.check;
            validarSerial.ImageAlign = ContentAlignment.MiddleLeft;
            validarSerial.Location = new Point(121, 262);
            validarSerial.Name = "validarSerial";
            validarSerial.Size = new Size(163, 41);
            validarSerial.TabIndex = 1;
            validarSerial.Text = "Validar";
            validarSerial.UseVisualStyleBackColor = true;
            validarSerial.Click += validarSerial_Click;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label9.ForeColor = Color.White;
            label9.Location = new Point(201, 15);
            label9.Name = "label9";
            label9.Size = new Size(163, 32);
            label9.TabIndex = 0;
            label9.Text = "Validar Serial";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label10.ForeColor = Color.White;
            label10.Location = new Point(85, 131);
            label10.Name = "label10";
            label10.Size = new Size(158, 21);
            label10.TabIndex = 0;
            label10.Text = "Digite el serial aquí";
            // 
            // codigoSerial
            // 
            codigoSerial.CharacterCasing = CharacterCasing.Upper;
            codigoSerial.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            codigoSerial.Location = new Point(85, 174);
            codigoSerial.Name = "codigoSerial";
            codigoSerial.Size = new Size(397, 29);
            codigoSerial.TabIndex = 0;
            codigoSerial.UseSystemPasswordChar = true;
            // 
            // actualizarSistema
            // 
            actualizarSistema.BackColor = Color.Gold;
            actualizarSistema.Cursor = Cursors.Hand;
            actualizarSistema.Image = Properties.Resources.nube_descargar_alt;
            actualizarSistema.ImageAlign = ContentAlignment.MiddleLeft;
            actualizarSistema.Location = new Point(13, 300);
            actualizarSistema.Name = "actualizarSistema";
            actualizarSistema.Size = new Size(166, 29);
            actualizarSistema.TabIndex = 3;
            actualizarSistema.Text = "Actualización Disponible!";
            actualizarSistema.TextAlign = ContentAlignment.MiddleRight;
            actualizarSistema.UseVisualStyleBackColor = false;
            actualizarSistema.Visible = false;
            actualizarSistema.Click += actualizarSistema_Click;
            // 
            // inicio
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            ClientSize = new Size(565, 377);
            Controls.Add(panelSerial);
            Controls.Add(autorizar);
            Controls.Add(conexiones);
            Controls.Add(conexionpanel);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(panelSesion);
            ForeColor = SystemColors.ControlText;
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "inicio";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Inicio De Sesion";
            Load += inicio_Load;
            Shown += inicio_Shown;
            KeyDown += inicio_KeyDown;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)alerta).EndInit();
            AvisoDBIMG.ResumeLayout(false);
            AvisoDBIMG.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            conexionpanel.ResumeLayout(false);
            conexionpanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)txtsql).EndInit();
            conexiones.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)usuarioimagen).EndInit();
            ((System.ComponentModel.ISupportInitialize)contraimagen).EndInit();
            autorizar.ResumeLayout(false);
            autorizar.PerformLayout();
            panelSesion.ResumeLayout(false);
            panelSesion.PerformLayout();
            panelSerial.ResumeLayout(false);
            panelSerial.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private TextBox txtusuario;
        private TextBox txtpass;
        private Button iniciobtn;
        private Panel panel1;
        private CheckBox passView;
        private Button button1;
        private Label iniciolabel;
        private Button button2;
        private Button sqlbtn;
        private ToolTip toolTip1;
        private Panel conexionpanel;
        private Label label5;
        private Label label4;
        private Label label3;
        private Button salirsqlbtn;
        private Button guardarbtn;
        private TextBox contservidortxt;
        private TextBox usuarioservidortxt;
        private TextBox servidortxt;
        private Label label6;
        private CheckBox defectochk;
        private Button button3;
        private DataGridView txtsql;
        private Panel conexiones;
        private Button button5;
        private Button button4;
        private Button button6;
        private Button borrarconex;
        private CheckBox recordarchk;
        private PictureBox alerta;
        private PictureBox usuarioimagen;
        private PictureBox contraimagen;
        private Button CrearDBbtn;
        private Panel autorizar;
        private Button salirAutorBTN;
        private Button autorizarBTN;
        private Label label7;
        private TextBox contAdmin;
        private TextBox usuAdmin;
        private Label label1;
        private Label label2;
        private ProgressBar progressBar1;
        private Panel panelSesion;
        private PictureBox pictureBox3;
        private Panel AvisoDBIMG;
        private TextBox DBTxt;
        private Label label8;
        private Panel panelSerial;
        private TextBox codigoSerial;
        private Button validarSerial;
        private Label label9;
        private Button generarSerial;
        private Label label10;
        private Button quitarPanelSerial;
        private Button pegarClipb;
        private Button actualizarSistema;
    }
}
