namespace PagamentoService.Consumer.Models;

public class PedidoCriadoEvent
{
    public string pedido_id { get; set; }
    public string cliente_id { get; set; }
    public List<ItemPedido> itens { get; set; }
    public decimal valor_total { get; set; }
}

public class ItemPedido
{
    public string produto_id { get; set; }
    public int quantidade { get; set; }
}