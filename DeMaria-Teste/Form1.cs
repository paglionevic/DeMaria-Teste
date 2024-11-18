using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DeMaria_Teste.Model;
using DeMaria_Teste.Model.Repository;

namespace DeMaria_Teste
{
    public partial class Form1 : Form
    {

        //implementar interfaces
        private readonly ClienteRepository _clienteRepository;
        private readonly ProdutoRepository _produtoRepository;
        private readonly VendaRepository _vendaRepository;

        private IList<Produto> _carrinho;
        RelatorioVendas _relatorioVendas;
        private BindingSource bindingSourceCarrinhoLive = new BindingSource();
        private Guid _idProdutoSelectedInGrid;
        public Form1()
        {
            InitializeComponent();
            _clienteRepository = new ClienteRepository();
            _produtoRepository = new ProdutoRepository();
            _vendaRepository = new VendaRepository();

            _carrinho = new List<Produto>();
            bindingSourceCarrinhoLive.DataSource = _carrinho;
            CarrinhoLive.DataSource = bindingSourceCarrinhoLive;

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        #region Cliente

        private void RegistrarCliente(object sender, EventArgs e)
        {
            _clienteRepository.AdicionarCliente(
                Convert.ToInt64(CPFClienteInsert.Text),
                NomeClienteInsert.Text,
                SobrenomeClienteInsert.Text,
                TelefoneClienteInsert.Text,
                EmailClienteInsert.Text,
                EnderecoClienteInsert.Text);

            ClienteGridView.DataSource = _clienteRepository.GetData();

        }

        private void GridViewSelection(object sender, DataGridViewCellEventArgs e)
        {
            NomeClienteInsert.Text = Convert.ToString(ClienteGridView.Rows[e.RowIndex].Cells[0].Value);
            SobrenomeClienteInsert.Text = Convert.ToString(ClienteGridView.Rows[e.RowIndex].Cells[1].Value);
            TelefoneClienteInsert.Text = Convert.ToString(ClienteGridView.Rows[e.RowIndex].Cells[2].Value);
            EmailClienteInsert.Text = Convert.ToString(ClienteGridView.Rows[e.RowIndex].Cells[3].Value);
            EnderecoClienteInsert.Text = Convert.ToString(ClienteGridView.Rows[e.RowIndex].Cells[4].Value);
            CPFClienteInsert.Text = Convert.ToString(ClienteGridView.Rows[e.RowIndex].Cells[5].Value);
        }

        private void DeletarCliente(object sender, EventArgs e)
        {
            _clienteRepository.DeletarCliente(Convert.ToInt64(CPFClienteInsert.Text));
            ClienteGridView.DataSource = _clienteRepository.GetData();
        }

        private void EditarCliente(object sender, EventArgs e)
        {
            _clienteRepository.EditarCliente(
               Convert.ToInt64(CPFClienteInsert.Text),
                NomeClienteInsert.Text,
                SobrenomeClienteInsert.Text,
                TelefoneClienteInsert.Text,
                EmailClienteInsert.Text,
                EnderecoClienteInsert.Text);
            ClienteGridView.DataSource = _clienteRepository.GetData();
        }

        private void LimparCamposCliente(object sender, EventArgs e)
        {
            NomeClienteInsert.Text = string.Empty;
            SobrenomeClienteInsert.Text = string.Empty;
            EmailClienteInsert.Text = string.Empty;
            CPFClienteInsert.Text = string.Empty;
            EnderecoClienteInsert.Text = string.Empty;
            TelefoneClienteInsert.Text = string.Empty;
        }

        #endregion

        #region Produto
        private void RegistrarProduto(object sender, EventArgs e)
        {
            _produtoRepository.AdicionarProduto(
                NomeProdutoInsert.Text,
                DescricaoProdutoInsert.Text,
                Convert.ToInt32(EstoqueProdutoInsert.Text),
                Convert.ToDouble(PrecoProdutoInsert.Text));
            ProdutoGridView.DataSource = _produtoRepository.GetData();
        }
        private void ProdutoGridViewSelection(object sender, DataGridViewCellEventArgs e)
        {
            _idProdutoSelectedInGrid = Guid.Parse(Convert.ToString(ProdutoGridView.Rows[e.RowIndex].Cells[0].Value));
            NomeProdutoInsert.Text = Convert.ToString(ProdutoGridView.Rows[e.RowIndex].Cells[1].Value);
            DescricaoProdutoInsert.Text = Convert.ToString(ProdutoGridView.Rows[e.RowIndex].Cells[2].Value);
            EstoqueProdutoInsert.Text = Convert.ToString(ProdutoGridView.Rows[e.RowIndex].Cells[3].Value);
            PrecoProdutoInsert.Text = Convert.ToString(ProdutoGridView.Rows[e.RowIndex].Cells[4].Value);

        }
        private void LimparCamposProduto(object sender, EventArgs e)
        {
            NomeProdutoInsert.Text = string.Empty;
            DescricaoProdutoInsert.Text = string.Empty;
            EstoqueProdutoInsert.Text = string.Empty;
            PrecoProdutoInsert.Text = string.Empty;
        }
        private void DeletarProduto(object sender, EventArgs e)
        {
            if (_idProdutoSelectedInGrid == Guid.Empty || _idProdutoSelectedInGrid == null)
                MessageBox.Show("Selecione um registro antes de efetuar esta operação!");
            else
            {
                _produtoRepository.DeletarProduto(_idProdutoSelectedInGrid);
                ProdutoGridView.DataSource = _produtoRepository.GetData();
            }

        }
        private void EditarProduto(object sender, EventArgs e)
        {
            if (_idProdutoSelectedInGrid == Guid.Empty || _idProdutoSelectedInGrid == null)
                MessageBox.Show("Selecione um registro antes de efetuar esta operação!");
            else
            {
                _produtoRepository.EditarProduto(
                  _idProdutoSelectedInGrid,
                   NomeProdutoInsert.Text,
                   DescricaoProdutoInsert.Text,
                   Convert.ToInt32(EstoqueProdutoInsert.Text),
                   Convert.ToDouble(PrecoProdutoInsert.Text));
                ProdutoGridView.DataSource = _produtoRepository.GetData();
            }

        }
        #endregion
        private void SelecionarRegistroDeVenda(object sender, DataGridViewCellEventArgs e)
        {
            CPFClienteViewVenda.Text = Convert.ToString(VendasGridView.Rows[e.RowIndex].Cells[0].Value);
            Valortotalview.Text = Convert.ToString(VendasGridView.Rows[e.RowIndex].Cells[1].Value);
            DataVendaView.Text = Convert.ToString(VendasGridView.Rows[e.RowIndex].Cells[2].Value);
            CarrinhoViewReg.DataSource = _vendaRepository.GetDataCarrinho(Guid.Parse(Convert.ToString(VendasGridView.Rows[e.RowIndex].Cells[3].Value)));
        }

        private void PesquisarProdutoPorNome(object sender, EventArgs e)
        {
            ProdutosPesquisadosView.DataSource = _produtoRepository.GetProdutoPorNome(NomeProdutoSearchbar.Text);

        }

        private void AdicionarProdutoAoCarrinho(object sender, EventArgs e)
        {
            if (QuantidadeProduto.Value <= 0)
            {
                MessageBox.Show("Quantidade minima deve ser igual ou maior a 1!");
                return;
            }

            if (ProdutosPesquisadosView.SelectedRows.Count > 0)
            {
                var selectedRow = ProdutosPesquisadosView.SelectedRows[0];

                //TODO: adicionar calculo de produtos ja adicionados ao carrinho
                //quantidade no banco - (quantidade do carrinho + quantidade solicitada)
                if (Convert.ToInt32(selectedRow.Cells["Quantidade"].Value) <= (int)QuantidadeProduto.Value)
                {
                    MessageBox.Show("Produto sem estoque suficiente!");
                    return;
                }

                Produto produtoSelecionado = new Produto
                {
                    idproduto = Guid.Parse(selectedRow.Cells["IdProduto"].Value.ToString()),
                    Nome = selectedRow.Cells["Nome"].Value.ToString(),
                    Preco = Convert.ToDouble(selectedRow.Cells["Preco"].Value),
                    Quantidade = (int)QuantidadeProduto.Value
                };

                _carrinho.Add(produtoSelecionado);
                bindingSourceCarrinhoLive.ResetBindings(false);

                Valortotalcomprashow.Text = _carrinho.Sum(x => x.Preco * x.Quantidade).ToString();

            }
            else
            {
                MessageBox.Show("Por favor, selecione um Produto da lista de pesquisa!");
            }
        }

        private void CarrinhoLive_ClickRemove(object sender, DataGridViewCellEventArgs e)
        {
            var idproduto = Convert.ToString(CarrinhoLive.Rows[e.RowIndex].Cells[0].Value);
            _carrinho.Remove(_carrinho.Single(x => x.idproduto == Guid.Parse(idproduto)));
            bindingSourceCarrinhoLive.ResetBindings(false);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CPFClienteVendaInsert.Text))
            {
                MessageBox.Show("Insira o CPF de um cliente antes de realizar a venda");
                return;
            }
            if (_carrinho.Count <= 0)
            {
                MessageBox.Show("Adicione pelo menos um produto ao carrinho antes de realizar a venda");
                return;
            }

            var ret = _vendaRepository.RegistrarVenda(CPFClienteVendaInsert.Text, Valortotalcomprashow.Text);

            if (ret != null)
            {
                foreach (var produto in _carrinho)
                {
                    _vendaRepository.RegistrarCarrinho(produto.idproduto, produto.Preco, produto.Quantidade, ret);
                    _produtoRepository.DeduzirEstoqueProduto(produto.idproduto, produto.Quantidade);
                }
            }

            MessageBox.Show("Venda Registrada");
            _carrinho.Clear();
            bindingSourceCarrinhoLive.ResetBindings(false);
        }

        private void TabSwitchTrigger(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 1:
                    ProdutoGridView.DataSource = _produtoRepository.GetData();
                    break;
                case 2:
                    ClienteGridView.DataSource = _clienteRepository.GetData();
                    break;
                case 3:
                    VendasGridView.DataSource = _vendaRepository.GetData();
                    break;
                case 4:
                    _relatorioVendas = _vendaRepository.ObterRelatorioDeVendas();
                    if (_relatorioVendas != null)
                    {
                        //lógica para manipular o relatório de vendas
                    }
                    break;
                default:
                    break;
            }

        }
    }
}
