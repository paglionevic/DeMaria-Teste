using System;
using System.Configuration;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace DeMaria_Teste.Model.Repository
{
    public class VendaRepository
    {
        private readonly string _connectionString;
        public VendaRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public DataTable GetData()
        {

            string selectSql = "SELECT cpf, valortotal, datavenda, idvenda FROM Venda";
            DataSet ds = new DataSet();

            NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(selectSql, conn);
            da.Fill(ds);
            conn.Close();
            return ds.Tables[0];

        }

        public DataTable GetDataCarrinho(Guid idVenda)
        {
            var querySelect = @"
    SELECT 
        p.nome AS NomeProduto,
        c.quantidade AS Quantidade,
        p.preco AS Preco
    FROM 
        carrinho c
    INNER JOIN 
        produto p ON c.idproduto = p.idproduto
    WHERE 
        c.idvenda = @idVenda";

            DataSet ds = new DataSet();

            using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand(querySelect, conn))
                {
                    // Adicionar o parâmetro de forma segura
                    cmd.Parameters.AddWithValue("idVenda", idVenda);

                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
            }

            return ds.Tables[0];
        }

        public Guid RegistrarVenda(string cpf, string valortotal)
        {

            var idvenda = Guid.NewGuid();
            var dataAtual = DateTime.Now;

            var sqlInserirCliente = @"
        INSERT INTO venda (idvenda,valortotal,datavenda,cpf)
        VALUES (@idvenda, @valortotal, @datavenda, @cpf)";

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                using (var cmdInserir = new NpgsqlCommand(sqlInserirCliente, conn))
                {
                    cmdInserir.Parameters.AddWithValue("idvenda", idvenda);
                    cmdInserir.Parameters.AddWithValue("valortotal", Convert.ToDouble(valortotal));
                    cmdInserir.Parameters.AddWithValue("datavenda", NpgsqlDbType.Timestamp).Value = dataAtual;
                    cmdInserir.Parameters.AddWithValue("cpf", Convert.ToInt64(cpf));

                    cmdInserir.ExecuteNonQuery();

                    conn.Close();
                }
            }
            return idvenda;
        }

        public void RegistrarCarrinho(Guid idproduto, double preco, int quantidade, Guid idvenda)
        {

            var sqlInserirCliente = @"
        INSERT INTO carrinho (idvenda,idproduto,preco,quantidade)
        VALUES (@idvenda, @idproduto, @preco, @quantidade)";

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                using (var cmdInserir = new NpgsqlCommand(sqlInserirCliente, conn))
                {
                    cmdInserir.Parameters.AddWithValue("idvenda", idvenda);
                    cmdInserir.Parameters.AddWithValue("idproduto", idproduto);
                    cmdInserir.Parameters.AddWithValue("preco", preco);
                    cmdInserir.Parameters.AddWithValue("quantidade", quantidade);

                    cmdInserir.ExecuteNonQuery();

                    conn.Close();
                }
            }

        }


        public RelatorioVendas ObterRelatorioDeVendas()
        {
            string query = @"
        SELECT 
            COUNT(DISTINCT v.idvenda) AS NumeroDeVendas,
            SUM(c.quantidade * p.preco) AS ValorTotalVendido,
            p.nome AS ProdutoMaisVendido
        FROM 
            venda v
        INNER JOIN 
            carrinho c ON v.idvenda = c.idvenda
        INNER JOIN 
            produto p ON c.idproduto = p.idproduto
        WHERE 
            EXTRACT(MONTH FROM v.datavenda) = EXTRACT(MONTH FROM CURRENT_DATE)
            AND EXTRACT(YEAR FROM v.datavenda) = EXTRACT(YEAR FROM CURRENT_DATE)
        GROUP BY 
            p.nome
        ORDER BY 
            SUM(c.quantidade) DESC
        LIMIT 1";

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            RelatorioVendas relatorio = new RelatorioVendas
                            {
                                numeroDeVendas = reader.GetInt32(0),
                                produtoMaisVendido = reader.GetString(2),
                                valorTotalVendido = reader.GetDecimal(1)
                            };

                            return relatorio;
                        }
                        return null;
                    }
                }
            }
        }
    }
}
