using Fd_DBC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ordem_de_servico
{
    public partial class ConfigurarBanco : Form
    {
        DBC CG = new DBC();

        public ConfigurarBanco()
        {
            InitializeComponent();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            string Servidor = txtServidor.Text;
            string Port = txtPorta.Text;
            string User = txtUser.Text;
            string Banco = txtBanco.Text;
            string Senha = txtSenha.Text;
            try
            {
                CG.ConfigConexao(Servidor, Port, User, Banco, Senha);
                CG.ConectarBancoDeDados();
                Close();
            }
            catch
            {
                MessageBox.Show("Não foi possivel conectar ao banco");
            }

        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            txtBanco.Clear();
            txtPorta.Clear();
            txtSenha.Clear();
            txtServidor.Clear();
            txtUser.Clear();
        }
    }
}
