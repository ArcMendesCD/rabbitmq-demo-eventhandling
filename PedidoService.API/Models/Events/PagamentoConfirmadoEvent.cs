namespace PedidoService.API.Models.Events;

public class PagamentoConfirmadoEvent
{
    public string pedido_id { get; set; } = string.Empty;
    public string status_pagamento { get; set; } = "confirmado";
    public decimal valor_pago { get; set; }
    public DateTime data_confirmacao { get; set; }
}