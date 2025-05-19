namespace PedidoService.API.Models;

public class CriarPedidoRequest
{
    public string ClienteId { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public List<ItemPedidoRequest> Itens { get; set; } = new();
}