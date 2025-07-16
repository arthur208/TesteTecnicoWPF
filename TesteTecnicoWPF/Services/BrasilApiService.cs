using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TesteTecnicoWPF.Models.BrasilApi;

namespace TesteTecnicoWPF.Services
{
    
    /// Serviço para se comunicar com a BrasilAPI.
    
    public class BrasilApiService
    {
        // Usamos HttpClient para fazer requisições HTTP.
        private static readonly HttpClient client = new HttpClient();
        private const string baseUrl = "https://brasilapi.com.br/api";

        /// <summary>
        /// Busca todos os estados brasileiros na API.
        /// </summary>
        /// <returns>Uma lista de estados ordenada por nome.</returns>
        public async Task<List<Estado>> GetEstados()
        {
            try
            {
                var response = await client.GetAsync($"{baseUrl}/ibge/uf/v1");
                response.EnsureSuccessStatusCode(); // Lança uma exceção se a resposta não for de sucesso (código 2xx).
                var responseBody = await response.Content.ReadAsStringAsync();
                var estados = JsonConvert.DeserializeObject<List<Estado>>(responseBody);
                return estados.OrderBy(e => e.Nome).ToList(); // Retorna a lista ordenada alfabeticamente.
            }
            catch (HttpRequestException e)
            {
               
                Console.WriteLine($"Erro na requisição de estados: {e.Message}");
                return new List<Estado>(); // Retorna lista vazia em caso de erro.
            }
        }

        /// <summary>
        /// Busca os municípios de um determinado estado (UF).
        /// </summary>
        /// <param name="uf">A sigla do estado (ex: "SP").</param>
        /// <returns>Uma lista de municípios formatada e ordenada.</returns>
        public async Task<List<Municipio>> GetMunicipiosPorUF(string uf)
        {
            if (string.IsNullOrWhiteSpace(uf)) return new List<Municipio>();

            try
            { // Verifica se a UF é válida (2 letras)
                var response = await client.GetAsync($"{baseUrl}/ibge/municipios/v1/{uf}?providers=dados-abertos-br,gov,wikipedia");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                var municipios = JsonConvert.DeserializeObject<List<Municipio>>(responseBody);

                // Formata o nome das cidades para "Title Case" (Ex: SÃO PAULO -> São Paulo)
                TextInfo textInfo = new CultureInfo("pt-BR", false).TextInfo;
                foreach (var municipio in municipios)
                {
                    municipio.Nome = textInfo.ToTitleCase(municipio.Nome.ToLower());
                }

                return municipios.OrderBy(m => m.Nome).ToList();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Erro na requisição de municípios: {e.Message}");
                return new List<Municipio>();
            }
        }

        /// <summary>
        /// Busca um endereço completo a partir de um CEP.
        /// </summary>
        /// <param name="cep">O CEP a ser consultado (apenas números).</param>
        /// <returns>Um objeto de endereço ou null se não for encontrado ou der erro.</returns>
        public async Task<Endereco> GetEnderecoPorCep(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep) || cep.Length != 8) return null;

            try
            { 
                var response = await client.GetAsync($"{baseUrl}/cep/v1/{cep}");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Endereco>(responseBody);
            }
            catch (HttpRequestException)
            {
                // Erro comum aqui é CEP não encontrado (retorna 404), então retornamos null.
                return null;
            }
        }
    }
}