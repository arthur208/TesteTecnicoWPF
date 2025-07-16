namespace TesteTecnicoWPF.Models.BrasilApi
{
    /// <summary>
    /// Representa o endereço retornado pela API de CEP.
    /// </summary>
    public class Endereco
    {
        public string Cep { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Neighborhood { get; set; }
        public string Street { get; set; }
    }
}