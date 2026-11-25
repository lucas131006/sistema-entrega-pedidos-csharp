// =====================================================================
//  SISTEMA DE GERENCIAMENTO DE ENTREGA DE PEDIDOS
//  Projeto acadêmico - Estrutura de Dados (Aplicação de Console / C#)
//
//  Estruturas lineares utilizadas:
//    - List<Pedido>  : armazena TODOS os pedidos do sistema
//    - Queue<Pedido> : controla a ordem de despacho das entregas (FIFO)
//
//  Algoritmos implementados manualmente (sem LINQ):
//    - Busca Linear por Código e por Nome (parcial, case-insensitive)
//    - Bubble Sort por Prioridade e Selection Sort por Valor
//    - Seleção/Filtragem por Status, por Bairro e por Prioridade
// =====================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SistemaEntregaPedidos
{
    // -----------------------------------------------------------------
    // MODELO DE DADOS
    // -----------------------------------------------------------------
    public class Pedido
    {
        public int Codigo { get; set; }
        public string NomeCliente { get; set; }
        public string Bairro { get; set; }
        public int Prioridade { get; set; }      // 1 - Alta, 2 - Média, 3 - Baixa
        public decimal Valor { get; set; }
        public string Status { get; set; }       // "Pendente", "Em Rota", "Entregue"

        public Pedido(int codigo, string nomeCliente, string bairro,
                      int prioridade, decimal valor, string status = "Pendente")
        {
            Codigo = codigo;
            NomeCliente = nomeCliente;
            Bairro = bairro;
            Prioridade = prioridade;
            Valor = valor;
            Status = status;
        }

        // Converte o número da prioridade em texto legível
        public string PrioridadeTexto()
        {
            if (Prioridade == 1) return "Alta";
            if (Prioridade == 2) return "Média";
            return "Baixa";
        }
    }

    public class Program
    {
        // -------------------------------------------------------------
        // ESTRUTURAS DE DADOS GLOBAIS
        // -------------------------------------------------------------
        static List<Pedido> pedidos = new List<Pedido>();          // Lista geral
        static Queue<Pedido> filaEntregas = new Queue<Pedido>();   // Fila FIFO de despacho
        static int proximoCodigo = 1;                              // Gerador de códigos
        static CultureInfo cultura = CultureInfo.InvariantCulture;

        // -------------------------------------------------------------
        // PONTO DE ENTRADA
        // -------------------------------------------------------------
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            try { cultura = new CultureInfo("pt-BR"); } catch { cultura = CultureInfo.InvariantCulture; }

            CarregarMassaDeDados();

            int opcao;
            do
            {
                MostrarMenuPrincipal();
                opcao = LerInteiro("Escolha uma opção: ", 0, 6);
                Console.WriteLine();

                switch (opcao)
                {
                    case 1: CadastrarPedido(); break;
                    case 2: ListarTodos(); break;
                    case 3: MenuBuscar(); break;
                    case 4: MenuOrdenar(); break;
                    case 5: MenuFiltrar(); break;
                    case 6: MenuFila(); break;
                    case 0: Console.WriteLine("Encerrando o sistema. Até logo!"); break;
                }

                if (opcao != 0) Pausar();
            } while (opcao != 0);
        }

        // -------------------------------------------------------------
        // MASSA DE DADOS INICIAL (8 pedidos fictícios)
        // -------------------------------------------------------------
        static void CarregarMassaDeDados()
        {
            AdicionarPedido("Ana Souza",       "Centro",        2, 150.00m, "Pendente");
            AdicionarPedido("Bruno Lima",      "Jardim Norte",  1, 320.50m, "Pendente");
            AdicionarPedido("Carla Mendes",    "Vila Nova",     3,  89.90m, "Pendente");
            AdicionarPedido("Diego Ferreira",  "Centro",        1, 450.00m, "Pendente");
            AdicionarPedido("Eduarda Alves",   "Santa Clara",   2, 210.75m, "Pendente");
            AdicionarPedido("Felipe Rocha",    "Jardim Norte",  3,  64.30m, "Pendente");
            AdicionarPedido("Gabriela Nunes",  "Vila Nova",     1, 275.00m, "Em Rota");
            AdicionarPedido("Henrique Castro", "Santa Clara",   2, 130.40m, "Entregue");
        }

        // Cria o pedido, adiciona na Lista e, se Pendente, coloca na Fila
0        static Pedido AdicionarPedido(string nome, string bairro, int prioridade,
                                      decimal valor, string status)
        {
            Pedido p = new Pedido(proximoCodigo++, nome, bairro, prioridade, valor, status);
            pedidos.Add(p);                         // Lista: armazena todos
            if (p.Status == "Pendente")
                filaEntregas.Enqueue(p);            // Fila: apenas os pendentes (FIFO)
            return p;
        }

        // -------------------------------------------------------------
        // MENU PRINCIPAL
        // -------------------------------------------------------------
        static void MostrarMenuPrincipal()
        {
            Console.Clear();
            Console.WriteLine("==============================================");
            Console.WriteLine("   SISTEMA DE GERENCIAMENTO DE ENTREGAS");
            Console.WriteLine("==============================================");
            Console.WriteLine("  1. Cadastrar novo pedido");
            Console.WriteLine("  2. Listar todos os pedidos");
            Console.WriteLine("  3. Buscar pedido");
            Console.WriteLine("  4. Ordenar pedidos");
            Console.WriteLine("  5. Selecionar/Filtrar pedidos");
            Console.WriteLine("  6. Gerenciar fila de entregas");
            Console.WriteLine("  0. Sair");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine($"  Pedidos: {pedidos.Count} | Na fila: {filaEntregas.Count}");
            Console.WriteLine("==============================================");
        }

        // -------------------------------------------------------------
        // 1. CADASTRAR PEDIDO
        // -------------------------------------------------------------
        static void CadastrarPedido()
        {
            Console.WriteLine("--- CADASTRAR NOVO PEDIDO ---");
            string nome = LerTexto("Nome do cliente: ");
            string bairro = LerTexto("Bairro: ");
            int prioridade = LerInteiro("Prioridade (1-Alta, 2-Média, 3-Baixa): ", 1, 3);
            decimal valor = LerDecimal("Valor do pedido (R$): ");

            // Todo pedido novo nasce "Pendente" e entra na Lista e na Fila
            Pedido p = AdicionarPedido(nome, bairro, prioridade, valor, "Pendente");

            Console.WriteLine($"\nPedido #{p.Codigo} cadastrado e adicionado à fila de entregas.");
        }

        // -------------------------------------------------------------
        // 2. LISTAR TODOS
        // -------------------------------------------------------------
        static void ListarTodos()
        {
            Console.WriteLine("--- TODOS OS PEDIDOS ---");
            ExibirTabela(pedidos);
        }

        // -------------------------------------------------------------
        // 3. BUSCA
        // -------------------------------------------------------------
        static void MenuBuscar()
        {
            Console.WriteLine("--- BUSCAR PEDIDO ---");
            Console.WriteLine("1. Por Código");
            Console.WriteLine("2. Por Nome do Cliente");
            Console.WriteLine("0. Voltar");
            int op = LerInteiro("Opção: ", 0, 2);
            Console.WriteLine();

            if (op == 1)
            {
                int codigo = LerInteiro("Informe o código: ", 1, int.MaxValue);
                Pedido encontrado = BuscaLinearPorCodigo(pedidos, codigo);

                if (encontrado == null)
                    Console.WriteLine("Nenhum pedido encontrado com esse código.");
                else
                {
                    List<Pedido> resultado = new List<Pedido>();
                    resultado.Add(encontrado);
                    ExibirTabela(resultado);
                }
            }
            else if (op == 2)
            {
                string trecho = LerTexto("Informe o nome (ou parte dele): ");
                List<Pedido> resultado = BuscaLinearPorNome(pedidos, trecho);
                Console.WriteLine($"\n{resultado.Count} pedido(s) encontrado(s):");
                ExibirTabela(resultado);
            }
        }

        // BUSCA LINEAR 1: percorre a lista do início ao fim comparando o Código.
        // Para no primeiro elemento encontrado. Complexidade: O(n).
        static Pedido BuscaLinearPorCodigo(List<Pedido> lista, int codigo)
        {
            for (int i = 0; i < lista.Count; i++)
            {
                if (lista[i].Codigo == codigo)
                    return lista[i];
            }
            return null; // não encontrado
        }

        // BUSCA LINEAR 2: percorre toda a lista e coleta os pedidos cujo nome
        // CONTÉM o trecho informado, ignorando maiúsculas/minúsculas. O(n).
        static List<Pedido> BuscaLinearPorNome(List<Pedido> lista, string trecho)
        {
            List<Pedido> encontrados = new List<Pedido>();
            string trechoMinusculo = trecho.ToLowerInvariant();

            for (int i = 0; i < lista.Count; i++)
            {
                string nomeMinusculo = lista[i].NomeCliente.ToLowerInvariant();
                if (nomeMinusculo.Contains(trechoMinusculo))
                    encontrados.Add(lista[i]);
            }
            return encontrados;
        }

        // -------------------------------------------------------------
        // 4. ORDENAÇÃO
        // -------------------------------------------------------------
        static void MenuOrdenar()
        {
            Console.WriteLine("--- ORDENAR PEDIDOS ---");
            Console.WriteLine("1. Por Prioridade (1-Alta -> 3-Baixa) [Bubble Sort]");
            Console.WriteLine("2. Por Valor (maior -> menor)         [Selection Sort]");
            Console.WriteLine("0. Voltar");
            int op = LerInteiro("Opção: ", 0, 2);
            Console.WriteLine();

            if (op == 0) return;

            // Ordena uma CÓPIA, preservando a ordem original da lista de cadastro
            List<Pedido> copia = new List<Pedido>(pedidos);

            if (op == 1)
            {
                BubbleSortPorPrioridade(copia);
                Console.WriteLine("Pedidos ordenados por PRIORIDADE (mais urgente primeiro):");
            }
            else
            {
                SelectionSortPorValorDecrescente(copia);
                Console.WriteLine("Pedidos ordenados por VALOR (maior para o menor):");
            }
            ExibirTabela(copia);
        }

        // BUBBLE SORT por Prioridade (crescente: 1, 2, 3).
        // Compara pares adjacentes e troca se estiverem fora de ordem.
        // A cada passagem, o maior elemento "flutua" para o final.
        // Estável (mantém a ordem de cadastro em empates). O(n²).
        static void BubbleSortPorPrioridade(List<Pedido> lista)
        {
            int n = lista.Count;
            for (int i = 0; i < n - 1; i++)
            {
                bool trocou = false;
                for (int j = 0; j < n - 1 - i; j++)
                {
                    if (lista[j].Prioridade > lista[j + 1].Prioridade)
                    {
                        Pedido temp = lista[j];
                        lista[j] = lista[j + 1];
                        lista[j + 1] = temp;
                        trocou = true;
                    }
                }
                if (!trocou) break; // já está ordenada: encerra antecipadamente
            }
        }

        // SELECTION SORT por Valor (decrescente).
        // A cada passo, seleciona o MAIOR valor do trecho não ordenado
        // e o coloca na posição i. O(n²).
        static void SelectionSortPorValorDecrescente(List<Pedido> lista)
        {
            int n = lista.Count;
            for (int i = 0; i < n - 1; i++)
            {
                int indiceMaior = i;
                for (int j = i + 1; j < n; j++)
                {
                    if (lista[j].Valor > lista[indiceMaior].Valor)
                        indiceMaior = j;
                }
                if (indiceMaior != i)
                {
                    Pedido temp = lista[i];
                    lista[i] = lista[indiceMaior];
                    lista[indiceMaior] = temp;
                }
            }
        }

        // -------------------------------------------------------------
        // 5. SELEÇÃO / FILTRAGEM
        // -------------------------------------------------------------
        static void MenuFiltrar()
        {
            Console.WriteLine("--- SELECIONAR / FILTRAR PEDIDOS ---");
            Console.WriteLine("1. Por Status");
            Console.WriteLine("2. Por Bairro");
            Console.WriteLine("3. Por Nível de Prioridade");
            Console.WriteLine("0. Voltar");
            int op = LerInteiro("Opção: ", 0, 3);
            Console.WriteLine();

            if (op == 1)
            {
                Console.WriteLine("1. Pendente | 2. Em Rota | 3. Entregue");
                int s = LerInteiro("Status: ", 1, 3);
                string status = s == 1 ? "Pendente" : (s == 2 ? "Em Rota" : "Entregue");

                List<Pedido> resultado = FiltrarPorStatus(pedidos, status);
                Console.WriteLine($"\nPedidos com status \"{status}\": {resultado.Count}");
                ExibirTabela(resultado);
            }
            else if (op == 2)
            {
                string bairro = LerTexto("Informe o bairro: ");
                List<Pedido> resultado = FiltrarPorBairro(pedidos, bairro);
                Console.WriteLine($"\nPedidos no bairro \"{bairro}\": {resultado.Count}");
                ExibirTabela(resultado);
            }
            else if (op == 3)
            {
                int pr = LerInteiro("Prioridade (1-Alta, 2-Média, 3-Baixa): ", 1, 3);
                List<Pedido> resultado = FiltrarPorPrioridade(pedidos, pr);
                Console.WriteLine($"\nPedidos com prioridade {pr}: {resultado.Count}");
                ExibirTabela(resultado);
            }
        }

        // FILTRO 1: seleciona somente os pedidos com o Status informado. O(n).
        static List<Pedido> FiltrarPorStatus(List<Pedido> lista, string status)
        {
            List<Pedido> selecionados = new List<Pedido>();
            for (int i = 0; i < lista.Count; i++)
            {
                if (lista[i].Status == status)
                    selecionados.Add(lista[i]);
            }
            return selecionados;
        }

        // FILTRO 2: seleciona os pedidos do Bairro informado (ignora maiúsculas). O(n).
        static List<Pedido> FiltrarPorBairro(List<Pedido> lista, string bairro)
        {
            List<Pedido> selecionados = new List<Pedido>();
            for (int i = 0; i < lista.Count; i++)
            {
                if (string.Equals(lista[i].Bairro, bairro, StringComparison.OrdinalIgnoreCase))
                    selecionados.Add(lista[i]);
            }
            return selecionados;
        }

        // FILTRO 3 (extra): seleciona os pedidos de um nível de Prioridade. O(n).
        static List<Pedido> FiltrarPorPrioridade(List<Pedido> lista, int prioridade)
        {
            List<Pedido> selecionados = new List<Pedido>();
            for (int i = 0; i < lista.Count; i++)
            {
                if (lista[i].Prioridade == prioridade)
                    selecionados.Add(lista[i]);
            }
            return selecionados;
        }

        // -------------------------------------------------------------
        // 6. GERENCIAR FILA DE ENTREGAS (FIFO)
        // -------------------------------------------------------------
        static void MenuFila()
        {
            int op;
            do
            {
                Console.Clear();
                Console.WriteLine("--- GERENCIAR FILA DE ENTREGAS ---");
                Console.WriteLine("1. Visualizar fila atual");
                Console.WriteLine("2. Despachar próximo pedido (Pendente -> Em Rota)");
                Console.WriteLine("3. Confirmar entrega (Em Rota -> Entregue)");
                Console.WriteLine("0. Voltar ao menu principal");
                op = LerInteiro("Opção: ", 0, 3);
                Console.WriteLine();

                if (op == 1) VisualizarFila();
                else if (op == 2) DespacharProximo();
                else if (op == 3) ConfirmarEntrega();

                if (op != 0) Pausar();
            } while (op != 0);
        }

        static void VisualizarFila()
        {
            Console.WriteLine($"FILA DE DESPACHO (FIFO) - {filaEntregas.Count} pedido(s)");
            if (filaEntregas.Count == 0)
            {
                Console.WriteLine("A fila está vazia.");
                return;
            }

            Console.WriteLine("(o primeiro da lista é o próximo a sair)\n");
            int posicao = 1;
            foreach (Pedido p in filaEntregas) // enumera na ordem FIFO sem remover
            {
                Console.WriteLine($"{posicao}º -> #{p.Codigo} | {p.NomeCliente} | {p.Bairro} | " +
                                  $"Prioridade {p.PrioridadeTexto()} | {FormatarValor(p.Valor)}");
                posicao++;
            }
        }

        // Dequeue: remove o pedido que está há mais tempo na fila (FIFO)
        static void DespacharProximo()
        {
            if (filaEntregas.Count == 0)
            {
                Console.WriteLine("Não há pedidos na fila para despachar.");
                return;
            }

            Pedido p = filaEntregas.Dequeue();
            p.Status = "Em Rota";
            Console.WriteLine($"Pedido #{p.Codigo} ({p.NomeCliente} - {p.Bairro}) saiu para entrega. Status: Em Rota.");
            Console.WriteLine($"Restam {filaEntregas.Count} pedido(s) na fila.");
        }

        // Confirma a entrega de um pedido que está "Em Rota"
        static void ConfirmarEntrega()
        {
            List<Pedido> emRota = FiltrarPorStatus(pedidos, "Em Rota");
            if (emRota.Count == 0)
            {
                Console.WriteLine("Não há pedidos em rota no momento.");
                return;
            }

            Console.WriteLine("Pedidos em rota:");
            ExibirTabela(emRota);

            int codigo = LerInteiro("\nCódigo do pedido entregue (0 para cancelar): ", 0, int.MaxValue);
            if (codigo == 0) return;

            Pedido p = BuscaLinearPorCodigo(emRota, codigo);
            if (p == null)
            {
                Console.WriteLine("Esse código não corresponde a um pedido em rota.");
                return;
            }

            p.Status = "Entregue";
            Console.WriteLine($"Entrega do pedido #{p.Codigo} confirmada. Status: Entregue.");
        }

        // -------------------------------------------------------------
        // EXIBIÇÃO EM TABELA
        // -------------------------------------------------------------
        static void ExibirTabela(List<Pedido> lista)
        {
            if (lista.Count == 0)
            {
                Console.WriteLine("(nenhum pedido para exibir)");
                return;
            }

            string linha = new string('-', 83);
            Console.WriteLine(linha);
            Console.WriteLine($"{"Cód",-5}{"Cliente",-20}{"Bairro",-16}{"Prioridade",-12}{"Valor",-14}{"Status",-10}");
            Console.WriteLine(linha);

            for (int i = 0; i < lista.Count; i++)
            {
                Pedido p = lista[i];
                Console.WriteLine($"{p.Codigo,-5}{Cortar(p.NomeCliente, 19),-20}{Cortar(p.Bairro, 15),-16}" +
                                  $"{p.PrioridadeTexto(),-12}{FormatarValor(p.Valor),-14}{p.Status,-10}");
            }
            Console.WriteLine(linha);
        }

        static string Cortar(string texto, int max)
        {
            return texto.Length <= max ? texto : texto.Substring(0, max);
        }

        static string FormatarValor(decimal valor)
        {
            return "R$ " + valor.ToString("N2", cultura);
        }

        // -------------------------------------------------------------
        // UTILITÁRIOS DE ENTRADA (com validação)
        // -------------------------------------------------------------
        static string LerTexto(string mensagem)
        {
            while (true)
            {
                Console.Write(mensagem);
                string entrada = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(entrada))
                    return entrada.Trim();
                Console.WriteLine("Entrada inválida. O campo não pode ficar vazio.");
            }
        }

        static int LerInteiro(string mensagem, int min, int max)
        {
            while (true)
            {
                Console.Write(mensagem);
                string entrada = Console.ReadLine();
                if (int.TryParse(entrada, out int valor) && valor >= min && valor <= max)
                    return valor;
                Console.WriteLine("Valor inválido. Tente novamente.");
            }
        }

        static decimal LerDecimal(string mensagem)
        {
            while (true)
            {
                Console.Write(mensagem);
                string entrada = Console.ReadLine();
                if (entrada != null)
                {
                    // Aceita tanto "150,50" quanto "150.50"
                    entrada = entrada.Trim().Replace(',', '.');
                    if (decimal.TryParse(entrada, NumberStyles.Number,
                                         CultureInfo.InvariantCulture, out decimal valor) && valor > 0)
                        return valor;
                }
                Console.WriteLine("Valor inválido. Informe um número maior que zero.");
            }
        }

        static void Pausar()
        {
            Console.WriteLine("\nPressione ENTER para continuar...");
            Console.ReadLine();
        }
    }
}

