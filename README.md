# 📦 Teste Técnico - Gestão de Cadastros e Pedidos em WPF

Projeto de um sistema desktop para gestão de clientes, produtos e pedidos, desenvolvido com C# e WPF como parte de um processo de avaliação técnica. A aplicação demonstra o uso do padrão MVVM, persistência de dados em JSON, manipulação de coleções com LINQ e uma interface de usuário reativa.

## ✨ Funcionalidades Principais

### 🧑‍💼 Gestão de Pessoas
*   **CRUD Completo:** Inclusão, edição e exclusão de cadastros.
*   **Filtros Dinâmicos:** Pesquisa em tempo real por Nome e CPF.
*   **Visualização Integrada:** Grade de pedidos vinculados a cada cliente.
*   **Atalhos de Ação:** Botão para incluir um novo pedido rapidamente para o cliente selecionado.

### 🛍️ Gestão de Produtos
*   **CRUD Completo:** Gerenciamento total do catálogo de produtos.
*   **Filtros por Nome e Código:** Localize produtos de forma rápida e eficiente.
*   **Controle de Estoque Implícito:** A base para um futuro controle de estoque está pronta.

### 📝 Gestão de Pedidos
*   **Criação Intuitiva:** Um formulário dedicado para criar novos pedidos, selecionando cliente e adicionando múltiplos produtos com suas quantidades.
*   **Cálculo de Total Automático:** O valor total do pedido é atualizado em tempo real.
*   **Gerenciamento de Status:**
    *   Pedidos são criados como "Pendente" por padrão.
    *   O status pode ser alterado diretamente na grade para "Pago", "Enviado" ou "Recebido".
    *   A alteração é salva automaticamente.
*   **Filtro Avançado de Status:** Permite selecionar múltiplos status para uma visualização customizada dos pedidos.

## 🛠️ Arquitetura e Tecnologias
A aplicação foi projetada com foco em boas práticas, resultando em um código limpo, organizado e de fácil manutenção.

| Componente             | Tecnologia/Padrão              | Descrição                                                                                                |
| ---------------------- | ------------------------------ | -------------------------------------------------------------------------------------------------------- |
| **Arquitetura**        | MVVM (Model-View-ViewModel)    | Separação total entre a interface (View), a lógica de apresentação (ViewModel) e os dados (Model).         |
| **Linguagem**          | C#                             | Toda a lógica de negócio e de apresentação foi escrita em C#.                                              |
| **Interface**          | WPF (.NET Framework 4.6)       | Construção de uma interface desktop rica, reativa e moderna.                                             |
| **Manipulação de Dados** | LINQ                           | Utilizado para todas as consultas, filtros e manipulações de coleções de dados em memória.               |
| **Persistência**       | JSON (Newtonsoft.Json)         | Os dados de pessoas e produtos são salvos em arquivos .json legíveis e de fácil manutenção.              |
| **Interatividade**     | Microsoft.Xaml.Behaviors.Wpf   | Implementação de interações complexas na UI (como o filtro de status) de forma elegante e desacoplada.   |
| **Ícones**             | MahApps.Metro.IconPacks.Material | Biblioteca de ícones vetoriais para uma interface mais amigável e profissional.                          |

## 🚀 Como Executar o Projeto

### 📋 Pré-requisitos
*   Visual Studio 2017 ou superior.
*   .NET Framework 4.6 (workload de "Desenvolvimento para desktop com .NET").

### ⚙️ Instalação
1.  Clone este repositório para a sua máquina.
2.  Abra o arquivo da solução (`.sln`) no Visual Studio.
3.  Abra o **Console do Gerenciador de Pacotes** (`Ferramentas > Gerenciador de Pacotes do NuGet > Console do Gerenciador de Pacotes`).
4.  Execute os comandos da seção de dependências abaixo para instalar as bibliotecas exatas do projeto.
5.  Pressione `F5` para compilar e rodar a aplicação! 🎉

## 📦 Dependências (Comandos para o Console NuGet)
```powershell
# Biblioteca de ícones para a interface
Install-Package MahApps.Metro.IconPacks.Material -Version 4.6.0

# Biblioteca para interações avançadas no XAML
Install-Package Microsoft.Xaml.Behaviors.Wpf -Version 1.1.3

# Biblioteca para manipulação de arquivos JSON
Install-Package Newtonsoft.Json -Version 13.0.3
