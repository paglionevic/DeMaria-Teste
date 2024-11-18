using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace DeMaria_Teste.Model.Repository
{
    public class ProdutoRepository
    {
        private readonly string _connectionString;
        public ProdutoRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public void EditarProduto(Guid idproduto, string novoNome, string novoDescricao, int novoEstoque, double novoPreco)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    string comandoSql = "UPDATE produto SET nome = @novoNome, descricao = @novoDescricao, estoque = @novoEstoque, preco = @novoPreco WHERE idproduto = @idproduto";

                    using (var cmd = new NpgsqlCommand(comandoSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@idproduto", idproduto);
                        cmd.Parameters.AddWithValue("@novoNome", novoNome);
                        cmd.Parameters.AddWithValue("@novoDescricao", novoDescricao);
                        cmd.Parameters.AddWithValue("@novoEstoque", novoEstoque);
                        cmd.Parameters.AddWithValue("@novoPreco", novoPreco);

                        int linhasAfetadas = cmd.ExecuteNonQuery();

                        if (linhasAfetadas > 0)
                        {
                            MessageBox.Show("Produto atualizado com sucesso.");
                        }
                        else
                        {
                            MessageBox.Show("Produto não encontrado ou não houve alteração.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao atualizar Produto: " + ex.Message);
                }
            }
        }
        public void DeletarProduto(Guid idproduto)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    string comandoSql = "DELETE FROM produto WHERE idproduto = @idproduto";

                    using (var cmd = new NpgsqlCommand(comandoSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@idproduto", idproduto);

                        int linhasAfetadas = cmd.ExecuteNonQuery();

                        if (linhasAfetadas > 0)
                        {
                            MessageBox.Show("Produto excluído com sucesso.");
                        }
                        else
                        {
                            MessageBox.Show("Produto não encontrado.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao excluir produto: " + ex.Message);
                }
            }
        }
        public DataTable GetData()
        {

            string selectSql = "SELECT * FROM Produto";
            DataSet ds = new DataSet();

            NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(selectSql, conn);
            da.Fill(ds);
            conn.Close();
            return ds.Tables[0];

        }

        public List<Produto> GetProdutoPorNome(string nomeProduto)
        {
            var querySelect = @"
    SELECT 
        p.idproduto AS IdProduto,
        p.nome AS NomeProduto,
        p.descricao AS Descricao,
        p.estoque AS Estoque,
        p.preco AS Preco
    FROM 
        produto p
    WHERE 
        p.nome ILIKE @nomeProduto";

            DataSet ds = new DataSet();

            using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand(querySelect, conn))
                {
                    cmd.Parameters.AddWithValue("nomeProduto", $"%{nomeProduto}%");

                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
            }

            return ConvertDataTableToList(ds.Tables[0]);
        }
        public void AdicionarProduto(string nome, string descricao, int estoque, double preco)
        {
            var sqlProdutoPorNome = "SELECT COUNT(*) FROM produto WHERE nome = @nome";

            var sqlInserirProduto = @"
        INSERT INTO produto (idproduto, nome, descricao, estoque, preco)
        VALUES (@idproduto, @nome, @descricao, @estoque, @preco)";

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                using (var cmdVerificar = new NpgsqlCommand(sqlProdutoPorNome, conn))
                {
                    cmdVerificar.Parameters.AddWithValue("nome", nome);
                    var count = (long)cmdVerificar.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("Produto ja registrado");
                    }
                    else
                    {
                        using (var cmdInserir = new NpgsqlCommand(sqlInserirProduto, conn))
                        {
                            cmdInserir.Parameters.AddWithValue("idproduto", Guid.NewGuid());
                            cmdInserir.Parameters.AddWithValue("nome", nome);
                            cmdInserir.Parameters.AddWithValue("descricao", descricao);
                            cmdInserir.Parameters.AddWithValue("Estoque", estoque);
                            cmdInserir.Parameters.AddWithValue("preco", preco);

                            cmdInserir.ExecuteNonQuery();

                            conn.Close();

                            MessageBox.Show("Registro Concluido");

                        }
                    }
                }
            }
        }


        private List<Produto> ConvertDataTableToList(DataTable dt)
        {
            List<Produto> produtoList = new List<Produto>();

            foreach (DataRow row in dt.Rows)
            {
                Produto produto = new Produto
                {
                    idproduto = Guid.Parse(row["IdProduto"].ToString()),
                    Nome = row["NomeProduto"].ToString(),
                    Preco = Convert.ToDouble(row["Preco"]),
                    Quantidade = Convert.ToInt32(row["Estoque"]),
                };
                produtoList.Add(produto);
            }

            return produtoList;
        }


        public void DeduzirEstoqueProduto(Guid idProduto, int quantidadeComprada)
        {
            string query = @"
        UPDATE produto 
        SET estoque = estoque - @quantidadeComprada 
        WHERE idproduto = @idProduto AND estoque >= @quantidadeComprada";

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idProduto", idProduto);
                    cmd.Parameters.AddWithValue("@quantidadeComprada", (int)quantidadeComprada);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        //possivel implementar validação extra, como regra de negócio
                        return;
                    }
                }
            }
        }

    }
}
