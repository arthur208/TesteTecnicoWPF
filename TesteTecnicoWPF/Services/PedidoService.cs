using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TesteTecnicoWPF.Models;

namespace TesteTecnicoWPF.Services
{
    public class PedidoService
    {
        private readonly string _filePath;

        public PedidoService()
        {
            string dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }
            _filePath = Path.Combine(dataDir, "pedidos.json");
        }

        public IEnumerable<Pedido> CarregarPedidos()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Pedido>();
            }

            try
            {
                var json = File.ReadAllText(_filePath);
                return JsonConvert.DeserializeObject<List<Pedido>>(json) ?? new List<Pedido>();
            }
            catch (Exception)
            {
                return new List<Pedido>();
            }
        }

        public void SalvarPedidos(IEnumerable<Pedido> pedidos)
        {
            var json = JsonConvert.SerializeObject(pedidos, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}