using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions; // Adicionado para usar Regex

namespace TesteTecnicoWPF.Models
{
    public class Pessoa : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        private string _nome;
        public string Nome
        {
            get { return _nome; }
            set { _nome = value; OnPropertyChanged(); }
        }

        private string _cpf;
        public string CPF
        {
            get { return _cpf; }
            set
            {
                // === NOVA LÓGICA DE MÁSCARA DE CPF ===
                // 1. Pega apenas os números do valor digitado
                var numbersOnly = Regex.Replace(value ?? "", @"[^\d]", "");

                if (numbersOnly.Length > 11) numbersOnly = numbersOnly.Substring(0, 11);
                _cpf = numbersOnly;
                OnPropertyChanged();
            }
        }

        private string _cep;
        public string CEP
        {
            get { return _cep; }
            set
            {
                // === NOVA LÓGICA DE MÁSCARA DE CEP ===
                // 1. Pega apenas os números
                var numbersOnly = Regex.Replace(value ?? "", @"[^\d]", "");
                if (numbersOnly.Length > 8) numbersOnly = numbersOnly.Substring(0, 8);
                

                _cep = numbersOnly;
                OnPropertyChanged();
            }
        }

        private string _logradouro;
        public string Logradouro
        {
            get { return _logradouro; }
            set { _logradouro = value; OnPropertyChanged(); }
        }

        private string _numero;
        public string Numero
        {
            get { return _numero; }
            set { _numero = value; OnPropertyChanged(); }
        }

        private string _bairro;
        public string Bairro
        {
            get { return _bairro; }
            set { _bairro = value; OnPropertyChanged(); }
        }

        private string _complemento;
        public string Complemento
        {
            get { return _complemento; }
            set { _complemento = value; OnPropertyChanged(); }
        }

        private string _cidade;
        public string Cidade
        {
            get { return _cidade; }
            set { _cidade = value; OnPropertyChanged(); }
        }

        private string _estado;
        public string Estado
        {
            get { return _estado; }
            set { _estado = value; OnPropertyChanged(); }
        }

        public Pessoa()
        {
          
        }

         public Pessoa Clone()
        {
            return (Pessoa)this.MemberwiseClone();
        }
    }
}