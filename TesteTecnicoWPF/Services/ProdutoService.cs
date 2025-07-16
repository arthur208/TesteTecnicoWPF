using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TesteTecnicoWPF.Models;

namespace TesteTecnicoWPF.Services
{
    public class ProdutoService
    {
        private readonly string _filePath;

        public ProdutoService()
        {
            string dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }
            _filePath = Path.Combine(dataDir, "produtos.json");
        }

        public IEnumerable<Produto> CarregarProdutos()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Produto>();
            }

            try
            {
                var json = File.ReadAllText(_filePath);
                return JsonConvert.DeserializeObject<List<Produto>>(json) ?? new List<Produto>();
            }
            catch (Exception)
            {
                return new List<Produto>();
            }
        }

        public void SalvarProdutos(IEnumerable<Produto> produtos)
        {
            var json = JsonConvert.SerializeObject(produtos, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}