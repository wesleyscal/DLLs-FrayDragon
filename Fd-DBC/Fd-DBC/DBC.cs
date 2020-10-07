using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace Fd_DBC
{
    public class DBC
    {
        //MySQL
        private MySqlCommand MyCMD = new MySqlCommand();

        private MySqlConnection ConectarBanco = new MySqlConnection();

        //Variabel Conexao Banco
        private static string Server = "";

        private static string Port = "";
        private static string User = "";
        private static string Banco = "";
        private static string Senha = "";

        //Configurar Banco
        public void ConfigConexao(string Server, string Port, string User, string Banco, string Senha)
        {
            DBC.Server = Server;
            DBC.Port = Port;
            DBC.User = User;
            DBC.Banco = Banco;
            DBC.Senha = Senha;
        }

        //conectar banco de dados
        public void ConectarBancoDeDados()
        {
            try
            {
                ConectarBanco.Close();
                string Config = "server=" + Server + ";port=" + Port + ";User id=" + User + ";database=" + Banco + ";password=" + Senha + ";CharSet=utf8";
                ConectarBanco.ConnectionString = Config;
                ConectarBanco.Open();
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Não foi possivel se conectar ao banco: " + e.Message);
            }
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
    }
}