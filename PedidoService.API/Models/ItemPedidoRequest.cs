namespace PedidoService.API.Models;

public class ItemPedidoRequest
{
    public string ProdutoId { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}