namespace TesteTecnicoWPF.Models.BrasilApi
{
    /// <summary>
    /// Representa um Estado (UF) retornado pela BrasilAPI.
    /// </summary>
    public class Estado
    {
        public int Id { get; set; }
        public string Sigla { get; set; }
        public string Nome { get; set; }
    }
}