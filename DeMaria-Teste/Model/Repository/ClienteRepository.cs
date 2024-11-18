using System;
using System.Configuration;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace DeMaria_Teste.Model.Repository
{
    public class ClienteRepository
    {
        private readonly string _connectionString;
        public ClienteRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public void EditarCliente(long cpf, string novoNome, string novoSobrenome, string novoTelefone, string novoEmail, string novoEndereco)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    string comandoSql = "UPDATE cliente SET nome = @nome, sobrenome = @sobrenome, telefone = @telefone, email = @email, endereco = @endereco WHERE cpf = @cpf";

                    using (var cmd = new NpgsqlCommand(comandoSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@cpf", cpf);
                        cmd.Parameters.AddWithValue("@nome", novoNome);
                        cmd.Parameters.AddWithValue("@sobrenome", novoSobrenome);
                        cmd.Parameters.AddWithValue("@telefone", novoTelefone);
                        cmd.Parameters.AddWithValue("@email", novoEmail);
                        cmd.Parameters.AddWithValue("@endereco", novoEndereco);

                        int linhasAfetadas = cmd.ExecuteNonQuery();

                        if (linhasAfetadas > 0)
                        {
                            MessageBox.Show("Cliente atualizado com sucesso.");
                        }
                        else
                        {
                            MessageBox.Show("Cliente não encontrado ou não houve alteração.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao atualizar cliente: " + ex.Message);
                }
            }
        }
        public void DeletarCliente(long cpf)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    string comandoSql = "DELETE FROM cliente WHERE cpf = @cpf";

                    using (var cmd = new NpgsqlCommand(comandoSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@cpf", cpf);

                        int linhasAfetadas = cmd.ExecuteNonQuery();

                        if (linhasAfetadas > 0)
                        {
                            MessageBox.Show("Cliente excluído com sucesso.");
                        }
                        else
                        {
                            MessageBox.Show("Cliente não encontrado.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao excluir cliente: " + ex.Message);
                }
            }
        }
        public DataTable GetData()
        {

            string selectSql = "SELECT * FROM cliente";
            DataSet ds = new DataSet();

            NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(selectSql, conn);
            da.Fill(ds);
            conn.Close();
            return ds.Tables[0];

        }
        public void AdicionarCliente(long cpf, string nome, string sobrenome, string telefone, string email, string endereco)
        {


            var sqlVerificarCpf = "SELECT COUNT(*) FROM cliente WHERE cpf = @cpf";

            var sqlInserirCliente = @"
        INSERT INTO cliente (cpf, nome, sobrenome, telefone, email, endereco)
        VALUES (@cpf, @nome, @sobrenome, @telefone, @email, @endereco)";

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                using (var cmdVerificar = new NpgsqlCommand(sqlVerificarCpf, conn))
                {
                    cmdVerificar.Parameters.AddWithValue("cpf", cpf);
                    var count = (long)cmdVerificar.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("CPF ja registrado");
                    }
                    else
                    {
                        using (var cmdInserir = new NpgsqlCommand(sqlInserirCliente, conn))
                        {
                            cmdInserir.Parameters.AddWithValue("cpf", cpf);
                            cmdInserir.Parameters.AddWithValue("nome", nome);
                            cmdInserir.Parameters.AddWithValue("sobrenome", sobrenome);
                            cmdInserir.Parameters.AddWithValue("telefone", telefone);
                            cmdInserir.Parameters.AddWithValue("email", email);
                            cmdInserir.Parameters.AddWithValue("endereco", endereco);

                            cmdInserir.ExecuteNonQuery();

                            conn.Close();

                            MessageBox.Show("Registro Concluido");
                        }
                    }
                }
            }
        }
    }
}
