namespace PedidoService.API.Models.Events;

public class PedidoCriadoEvent
{
    public string pedido_id { get; set; } = string.Empty;
    public string cliente_id { get; set; } = string.Empty;
    public List<ItemPedido> itens { get; set; } = new();
    public decimal valor_total { get; set; }
}


public class ItemPedido
{
    public string produto_id { get; set; } = string.Empty;
    public int quantidade { get; set; }
}
