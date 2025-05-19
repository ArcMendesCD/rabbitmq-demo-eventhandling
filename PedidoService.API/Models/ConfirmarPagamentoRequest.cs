namespace PedidoService.API.Models;

public class ConfirmarPagamentoRequest
{
    public string PedidoId { get; set; } = string.Empty;
    public string StatusPagamento { get; set; } = "confirmado";
    public decimal ValorPago { get; set; }
    public DateTime DataConfirmacao { get; set; }
}


public class ProcessarPagamentoRequest
{
    public string ClienteId { get; set; } = string.Empty;
    public List<ItemPedidoRequest> Itens { get; set; } = new();
    public decimal ValorTotal { get; set; }
}
