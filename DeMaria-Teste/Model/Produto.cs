using System;

namespace DeMaria_Teste.Model
{
    public class Produto
    {
        public Guid idproduto {  get; set; }
        public string Nome{ get; set; }
        public double Preco{ get; set; }
        public int Quantidade{ get; set; }

    }
}
