using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TesteTecnicoWPF.Models;

namespace TesteTecnicoWPF.Services
{
    /// <summary>
    /// Serviço responsável por salvar e carregar os dados de Pessoas em um arquivo JSON.
    /// </summary>
    public class PessoaService
    {
        private readonly string _filePath;

        public PessoaService()
        {
            // Define o caminho do arquivo dentro da pasta 'Data' do projeto.
            string dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

            // Garante que a pasta 'Data' exista.
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }

            _filePath = Path.Combine(dataDir, "pessoas.json");
        }

        /// <summary>
        /// Carrega a lista de pessoas do arquivo JSON.
        /// </summary>
        /// <returns>Uma lista de pessoas. Retorna uma lista vazia se o arquivo não existir.</returns>
        public IEnumerable<Pessoa> CarregarPessoas()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Pessoa>();
            }

            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<Pessoa>>(json) ?? new List<Pessoa>();
        }

        /// <summary>
        /// Salva uma coleção de pessoas no arquivo JSON.
        /// </summary>
        /// <param name="pessoas">A lista de pessoas a ser salva.</param>
        public void SalvarPessoas(IEnumerable<Pessoa> pessoas)
        {
            var json = JsonConvert.SerializeObject(pessoas, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}