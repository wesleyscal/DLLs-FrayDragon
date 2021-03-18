using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Drawing;

namespace Fd_DBC
{
    public class DBC
    {
        //MySQL
        private MySqlCommand MyCMD = new MySqlCommand();
        private MySqlConnection ConectarBanco = new MySqlConnection();
        private string KeyCrip = "";

        #region XML
        public void CriarXML(string NomeDoArquivo)
        {
            //Caminho do XML e Nome
            string path = Directory.GetCurrentDirectory();
            string Arquivo = "\\" + NomeDoArquivo + ".xml";

            //Cria o XML
            XmlTextWriter xml;
            xml = new XmlTextWriter(path + Arquivo, Encoding.UTF8);
            xml.Dispose();

            //Instacia o root
            XElement Rxml = new XElement("Root");
            Rxml.Save(path + Arquivo);

        }
        #endregion

        #region Banco De Dados
        //Configurar Banco
        public void ConfigConexao(string Server, string Port, string User, string Banco, string Senha)
        {
            //Testa as configuraçoes
            string Config = "server= " + Server + ";port= " + Port + ";User id= " + User + ";database= " + Banco + ";password= " + Senha + ";CharSet= utf8";
            ConectarBanco.ConnectionString = Config;
            ConectarBanco.Open();
            ConectarBanco.Close();

            //Caminho do XML
            string path = Directory.GetCurrentDirectory() + "\\BancoConfig.xml";

            //Criar XML caso não exista
            if (File.Exists(path) == false)
            {
                CriarXML("BancoConfig");
            }

            //Carregar XML
            XElement Rxml = XElement.Load(path);

            //Criptografar dados
            Server = Encrypt(Server, KeyCrip);
            Port = Encrypt(Port, KeyCrip);
            User = Encrypt(User, KeyCrip);
            Banco = Encrypt(Banco, KeyCrip);
            Senha = Encrypt(Senha, KeyCrip);

            //configura
            Rxml = new XElement("Config",
                new XElement("Servidor", Server),
                new XElement("Port", Port),
                new XElement("User", User),
                new XElement("Banco", Banco),
                new XElement("Senha", Senha)
                );

            //Salva Configuração
            Rxml.Save("BancoConfig.xml");
        }

        //conectar banco de dados
        public void ConectarBancoDeDados()
        {
            //Caminho do XML
            string path = Directory.GetCurrentDirectory() + "\\BancoConfig.xml";

            string Key = "";

            string Server = "";
            string Port = "";
            string User = "";
            string Banco = "";
            string Senha = "";

            XmlTextReader xml = new XmlTextReader(path);

            while (xml.Read())
            {
                switch (xml.NodeType)
                {
                    case XmlNodeType.Element: //Pega Nome do Elemento de abertura
                        Key = xml.Name;
                        break;

                    case XmlNodeType.Text: //Pega o valor do elemento
                        if (Key == "Servidor")
                        {
                            Server = xml.Value;
                        }
                        if (Key == "Port")
                        {
                            Port = xml.Value;
                        }
                        if (Key == "User")
                        {
                            User = xml.Value;
                        }
                        if (Key == "Banco")
                        {
                            Banco = xml.Value;
                        }
                        if (Key == "Senha")
                        {
                            Senha = xml.Value;
                        }
                        break;

                        /*
                        
                        Codigo Off, So para estudos
                        
                        case XmlNodeType.EndElement: //Pega o nome do Elemento de fechamento
                        break;
                        */
                }
            }

            xml.Close();

            //Descriptografar XML
            Server = DesEncrypt(Server, KeyCrip);
            Port = DesEncrypt(Port, KeyCrip);
            User = DesEncrypt(User, KeyCrip);
            Banco = DesEncrypt(Banco, KeyCrip);
            Senha = DesEncrypt(Senha, KeyCrip);

            //Conecta ao banco
            ConectarBanco.Close();
            string Config = "server= " + Server + ";port= " + Port + ";User id= " + User + ";database= " + Banco + ";password= " + Senha + ";CharSet= utf8";
            ConectarBanco.ConnectionString = Config;
            ConectarBanco.Open();

        }

        //Retornar Configuração do banco
        public void RetornarConfigBanco(DataTable dt)
        {

            //Caminho do XML
            string path = Directory.GetCurrentDirectory() + "\\BancoConfig.xml";

            string Key = "";

            string Server = "";
            string Port = "";
            string User = "";
            string Banco = "";
            string Senha = "";

            //Retorna Vazio, caso XML não exista
            if (File.Exists(path) == false)
            {
                dt.Columns.Add("Config");
                dt.Rows.Add(Server);
                dt.Rows.Add(Port);
                dt.Rows.Add(User);
                dt.Rows.Add(Banco);
                dt.Rows.Add(Senha);
                return;
            }

            XmlTextReader xml = new XmlTextReader(path);

            while (xml.Read())
            {
                switch (xml.NodeType)
                {
                    case XmlNodeType.Element: //Pega Nome do Elemento de abertura
                        Key = xml.Name;
                        break;

                    case XmlNodeType.Text: //Pega o valor do elemento
                        if (Key == "Servidor")
                        {
                            Server = xml.Value;
                        }
                        if (Key == "Port")
                        {
                            Port = xml.Value;
                        }
                        if (Key == "User")
                        {
                            User = xml.Value;
                        }
                        if (Key == "Banco")
                        {
                            Banco = xml.Value;
                        }
                        if (Key == "Senha")
                        {
                            Senha = xml.Value;
                        }
                        break;
                }
            }

            xml.Close();

            //Descriptografar XML
            Server = DesEncrypt(Server, KeyCrip);
            Port = DesEncrypt(Port, KeyCrip);
            User = DesEncrypt(User, KeyCrip);
            Banco = DesEncrypt(Banco, KeyCrip);
            Senha = DesEncrypt(Senha, KeyCrip);

            dt.Columns.Add("Config");
            dt.Rows.Add(Server);
            dt.Rows.Add(Port);
            dt.Rows.Add(User);
            dt.Rows.Add(Banco);
            dt.Rows.Add(Senha);

        }

        //Executa comando no MySql
        public void ExecutarComandoSql(string cmd)
        {
            ConectarBancoDeDados();

            MyCMD.CommandText = cmd;
            MyCMD.CommandType = CommandType.Text;
            MyCMD.Connection = ConectarBanco;
            MyCMD.ExecuteNonQuery();

            ConectarBanco.Close();
        }

        //retorna valores do sql
        public string RetornarValorSQL()
        {
            //Instanciar um novo DataTable (Objeto que receberão as infonrmações do banco SQL)
            DataTable dt = new DataTable();

            //Objeto que retornam os valores da pesquisa SQL
            MySqlDataAdapter adp = new MySqlDataAdapter(MyCMD);

            //Método que inserem as informações da pesquisa SQL
            adp.Fill(dt);

            return dt.Rows[0][0].ToString();
        }

        //retorna os dados do datatable
        public void RetornarDadosDataTable(DataTable dt)
        {
            //Objeto que retornam os valores da pesquisa SQL
            MySqlDataAdapter adp = new MySqlDataAdapter(MyCMD);

            //Método que inserem as informações da pesquisa SQL
            adp.Fill(dt);
        }
        #endregion

        #region DataGreedView
        //Exibe dados no DataGreedView
        public void ExibirDGV(DataGridView dgv)
        {
            //Instanciar um novo DataTable (Objeto que receberão as infonrmações do banco SQL)
            DataTable dt = new DataTable();

            //Objeto que retornam os valores da pesquisa SQL
            MySqlDataAdapter adp = new MySqlDataAdapter(MyCMD);

            //Método que inserem as informações da pesquisa SQL
            adp.Fill(dt);

            //Exibe os dados no DataGridView usando como fonte de dados o DataTable (dt)
            dgv.DataSource = dt;
        }

        //Formatar DataGridView
        public void FormatarDGV(DataGridView dgv)
        {
            //Básico
            dgv.AllowUserToAddRows = false;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToResizeRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgv.MultiSelect = false;
            dgv.EnableHeadersVisualStyles = false;

            //Somente leitura
            dgv.ReadOnly = true;

            //Fonte dos Cabeçalhos das Colunas
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 9F, FontStyle.Bold);

            //Alternar Cores Linhas
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;

            //Cor Cabeçalho
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.LightGray;
        }
        #endregion

        #region Formatar
        //Converte para formata de data do MySql
        public string DataMySQL(string DataNormal)
        {
            string _DataNormal = DataNormal.Replace("/", "-");
            string N_DataMySQL = _DataNormal.Substring(6, 4) +
                                 _DataNormal.Substring(5, 1) +
                                 _DataNormal.Substring(3, 2) +
                                 _DataNormal.Substring(2, 1) +
                                 _DataNormal.Substring(0, 2);
            return N_DataMySQL;
        }

        public string DataHoraMySQL(string DataNormal)
        {
            string _DataNormal = DataNormal.Replace("/", "-");
            string N_DataMySQL = _DataNormal.Substring(6, 4) +
                                 _DataNormal.Substring(5, 1) +
                                 _DataNormal.Substring(3, 2) +
                                 _DataNormal.Substring(2, 1) +
                                 _DataNormal.Substring(0, 2) +
                                 _DataNormal.Substring(10, 1) +
                                 _DataNormal.Substring(11, 8);
            return N_DataMySQL;
        }

        //TEXBOX EM FORMATO NUMÉRICO "MOEDA"
        public void Moeda(TextBox TXT)
        {
            string n = string.Empty;
            double v = 0;

            try
            {
                n = TXT.Text.Replace(",", "").Replace(".", "");

                if (n.Equals(""))
                {
                    n = "000";
                }

                n = n.PadLeft(3, '0');

                if (n.Length > 3 & n.Substring(0, 1) == "0")
                {
                    n = n.Substring(1, n.Length - 1);
                }

                v = Convert.ToDouble(n) / 100;

                TXT.Text = string.Format("{0:N}", v);
                TXT.SelectionStart = TXT.Text.Length;
            }
            catch
            {
            }
        }

        //Formatar DataTimePicker
        public void FormatarDTP(DateTimePicker DTP)
        {
            //Data Minima e Maxima
            DTP.Value = DateTime.Now;
            DTP.MinDate = new DateTime(2000, 6, 20);
            DTP.MaxDate = DateTime.Now;

            //Data Formato
            DTP.Format = DateTimePickerFormat.Custom;
            DTP.CustomFormat = "dd/MM/yyyy HH:mm:ss";
        }

        /// <summary>
        /// Adicionar no evento KeyPresss
        /// </summary>
        public bool ValidarNumero(string obj)
        {
            double n;
            string e = obj.ToString();
            bool IN = double.TryParse(e, out n);

            if (IN)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SomenteLetra(TextBox txt, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

        #endregion Formatar

        #region Criptografia
        //Gera a Key
        private string GerarKey()
        {
            DESCryptoServiceProvider desCrypto = (DESCryptoServiceProvider)DESCryptoServiceProvider.Create();
            return ASCIIEncoding.ASCII.GetString(desCrypto.Key);
        }
        //Faz a Criptografia
        private string Encrypt(string Texto, string key)
        {
            TripleDESCryptoServiceProvider TDC = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] bTexto, bHash;

            bHash = md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key));
            bTexto = ASCIIEncoding.ASCII.GetBytes(Texto);

            md5.Clear();

            TDC.Key = bHash;
            TDC.Mode = CipherMode.ECB;

            return Convert.ToBase64String(TDC.CreateEncryptor().TransformFinalBlock(bTexto, 0, bTexto.Length));

        }
        //Desfaz a Criptografia
        private string DesEncrypt(string Texto, string key)
        {
            TripleDESCryptoServiceProvider TDC = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] bTexto, bHash;

            bHash = md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key));
            bTexto = Convert.FromBase64String(Texto);

            md5.Clear();

            TDC.Key = bHash;
            TDC.Mode = CipherMode.ECB;

            return ASCIIEncoding.ASCII.GetString(TDC.CreateDecryptor().TransformFinalBlock(bTexto, 0, bTexto.Length));

        }
        #endregion

        #region InputBox
        //Configura visual da inputbox
        private void ConfigInputBox(InputBox frm)
        {
            //-----Botao-----
            //backColor
            if (Botao_BackColor_R == 220 && Botao_BackColor_G == 220 && Botao_BackColor_B == 220)
            {
                frm.btnOk.BackColor = Color.FromName(Botao_BackColor_Name);
                frm.btnCancelar.BackColor = Color.FromName(Botao_BackColor_Name);
            }
            else
            {
                frm.btnOk.BackColor = Color.FromArgb(Botao_BackColor_R, Botao_BackColor_G, Botao_BackColor_B);
                frm.btnCancelar.BackColor = Color.FromArgb(Botao_BackColor_R, Botao_BackColor_G, Botao_BackColor_B);
            }
            //ForeColor
            if (Botao_ForeColor_R == 0 && Botao_ForeColor_G == 0 && Botao_ForeColor_B == 0)
            {
                frm.btnOk.ForeColor = Color.FromName(Botao_ForeColor_Name);
                frm.btnCancelar.ForeColor = Color.FromName(Botao_ForeColor_Name);
            }
            else
            {
                frm.btnOk.ForeColor = Color.FromArgb(Botao_ForeColor_R, Botao_ForeColor_G, Botao_ForeColor_B);
                frm.btnCancelar.ForeColor = Color.FromArgb(Botao_ForeColor_R, Botao_ForeColor_G, Botao_ForeColor_B);
            }

            //-----Label-----
            //ForeColor
            if (Label_ForeColor_R == 0 && Label_ForeColor_G == 0 && Label_ForeColor_B == 0)
            {
                frm.lblTexto.ForeColor = Color.FromName(Label_ForeColor_Name);
            }
            else
            {
                frm.lblTexto.ForeColor = Color.FromArgb(Label_ForeColor_R, Label_ForeColor_G, Label_ForeColor_B);
            }

            //-----Form-----
            //backColor
            if (Form_BackColor_R == 245 && Form_BackColor_G == 245 && Form_BackColor_B == 245)
            {
                frm.BackColor = Color.FromName(Form_BackColor_Name);
            }
            else
            {
                frm.BackColor = Color.FromArgb(Form_BackColor_R, Form_BackColor_G, Form_BackColor_B);
            }
        }

        //Chama a inputBox
        public string InputBox(string Cabecalho, string Texto, string Valor)
        {
            InputBox frm = new InputBox(Cabecalho, Texto, Valor);
            ConfigInputBox(frm);
            frm.ShowDialog();
            string resultado = frm.InputResult();
            return resultado;
        }
        #endregion

        #region Visual
        //validar RGB 0 a 255
        internal int CorrigirRGB(int RGB)
        {
            if (RGB > 255)
            {
                RGB = 255;
                return RGB;
            }
            if (RGB < 0)
            {
                RGB = 0;
                return RGB;
            }

            return RGB;
        }

        #region Variavel
        //Botão BackColor
        internal int Botao_BackColor_R = 220;
        internal int Botao_BackColor_G = 220;
        internal int Botao_BackColor_B = 220;
        internal string Botao_BackColor_Name = "Gainsboro";
        //Botão ForeColor 
        internal int Botao_ForeColor_R = 0;
        internal int Botao_ForeColor_G = 0;
        internal int Botao_ForeColor_B = 0;
        internal string Botao_ForeColor_Name = "Black";
        //Label ForeColor
        internal int Label_ForeColor_R = 0;
        internal int Label_ForeColor_G = 0;
        internal int Label_ForeColor_B = 0;
        internal string Label_ForeColor_Name = "Black";
        //Form BackColor
        internal int Form_BackColor_R = 245;
        internal int Form_BackColor_G = 245;
        internal int Form_BackColor_B = 245;
        internal string Form_BackColor_Name = "WhiteSmoke";
        #endregion

        #region metodo
        //Botão
        public void Visual_InputBox_Botao_BackColor(int R, int G, int B)
        {
            Botao_BackColor_R = CorrigirRGB(R);
            Botao_BackColor_G = CorrigirRGB(G);
            Botao_BackColor_B = CorrigirRGB(B);
        }
        public void Visual_InputBox_Botao_BackColor(string Cor)
        {
            Botao_BackColor_Name = Cor;
        }
        public void Visual_InputBox_Botao_ForeColor(int R, int G, int B)
        {
            Botao_ForeColor_R = CorrigirRGB(R);
            Botao_ForeColor_G = CorrigirRGB(G);
            Botao_ForeColor_B = CorrigirRGB(B);
        }
        public void Visual_InputBox_Botao_ForeColor(string Cor)
        {
            Botao_ForeColor_Name = Cor;
        }

        //Label
        public void Visual_InputBox_Label_ForeColor(int R, int G, int B)
        {
            Label_ForeColor_R = CorrigirRGB(R);
            Label_ForeColor_G = CorrigirRGB(G);
            Label_ForeColor_B = CorrigirRGB(B);
        }
        public void Visual_InputBox_Label_ForeColor(string Cor)
        {
            Label_ForeColor_Name = Cor;
        }

        //Form
        public void Visual_InputBox_Form_BackColor(int R, int G, int B)
        {
            Form_BackColor_R = CorrigirRGB(R);
            Form_BackColor_G = CorrigirRGB(G);
            Form_BackColor_B = CorrigirRGB(B);
        }
        public void Visual_InputBox_Form_BackColor(string Cor)
        {
            Form_BackColor_Name = Cor;
        }
        #endregion

        #endregion

    }
}