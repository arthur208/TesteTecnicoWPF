using System;

namespace TesteTecnicoWPF.Models
{
    /// <summary>
    /// Representa a entidade Pessoa com seus dados cadastrais.
    /// </summary>
    public class Pessoa
    {
        // Identificador único da pessoa.
        public Guid Id { get; set; }

        // Nome completo da pessoa.
        public string Nome { get; set; }

        // CPF da pessoa.
        public string CPF { get; set; }

        // CEP do endereço.
        public string CEP { get; set; }

        // Logradouro (Rua, Avenida, etc.).
        public string Logradouro { get; set; }

        // Número do imóvel.
        public string Numero { get; set; }

        // Bairro.
        public string Bairro { get; set; }

        // Complemento do endereço (Apto, Bloco, etc.).
        public string Complemento { get; set; }

        // Cidade.
        public string Cidade { get; set; }

        // Estado (UF).
        public string Estado { get; set; }

        /// <summary>
        /// Construtor padrão que gera um novo Id para cada instância.
        /// </summary>
        public Pessoa()
        {
            Id = Guid.NewGuid(); // Garante que cada nova pessoa tenha um Id único.
        }
    }
}