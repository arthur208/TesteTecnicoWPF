using Newtonsoft.Json;

namespace TesteTecnicoWPF.Models.BrasilApi
{
    /// <summary>
    /// Representa um Município (Cidade) retornado pela BrasilAPI.
    /// </summary>
    public class Municipio
    {
        public string Nome { get; set; }

        [JsonProperty("codigo_ibge")] // Mapeia o nome do JSON para nossa propriedade
        public string CodigoIbge { get; set; }
    }
}