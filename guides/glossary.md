# 📖 Glossário — os termos que travam a leitura

> Toda área tem seu vocabulário, e MAUI tem o dele. Quando um termo aparecer num guia e você não lembrar, é aqui. Em ordem alfabética, em português claro.

### AOT (Ahead-of-Time)
Compilar o código pra linguagem de máquina **antes** de rodar (no build), em vez de durante a execução. Deixa o app abrir mais rápido. O oposto é o JIT.

### BindingContext
O objeto de onde um elemento da tela busca seus dados. Quando você escreve `{Binding Nome}`, ele procura `Nome` no `BindingContext`. Ver [data binding](./data-binding.md).

### Code-behind
O arquivo C# que acompanha um XAML (`MainPage.xaml` → `MainPage.xaml.cs`). O XAML descreve a aparência; o code-behind, o comportamento.

### CollectionView
O controle para mostrar listas de dados, com rolagem e reciclagem eficiente. Substitui o antigo `ListView`.

### Command (Comando)
Uma forma de ligar uma ação (ex.: clique de botão) a um método numa ViewModel, em vez de no code-behind. A base pra deixar a lógica testável.

### CommunityToolkit.Mvvm
Biblioteca oficial que gera o código chato do MVVM (propriedades observáveis, comandos) a partir de atributos. O que faz `[ObservableProperty]` e `[RelayCommand]` existirem.

### Compiled Binding
Binding resolvido em tempo de compilação (ativado com `x:DataType`), em vez de por reflection em runtime. Mais rápido e pega erros no build.

### ContentPage
O tipo mais comum de tela. Tem **um** conteúdo (geralmente um layout, que por sua vez tem vários elementos).

### DataTemplate
O "molde" de como cada item de uma lista aparece. Você define o template uma vez; o `CollectionView` aplica a cada item.

### Dependency Injection (DI) / Injeção de Dependência
Padrão onde um objeto **recebe** o que precisa de fora (pelo construtor) em vez de criar por conta própria. Torna o código desacoplado e testável. Ver [arquitetura](./architecture.md).

### Handler
A peça que conecta um controle MAUI ao controle nativo de cada plataforma. É onde você customiza comportamento de baixo nível (substituiu os "Renderers" do Xamarin).

### INotifyPropertyChanged
A interface que faz um objeto "avisar" quando uma propriedade muda, pra que o binding atualize a tela. Você raramente implementa na mão — o CommunityToolkit.Mvvm faz por você.

### JIT (Just-in-Time)
Compilar o código durante a execução, sob demanda. Mais lento no startup que o AOT, mas é o padrão em modo de desenvolvimento.

### Layout
Um container que **posiciona** outros elementos sem aparecer na tela: `Grid`, `StackLayout`, `FlexLayout`, `ScrollView`.

### MAUI
Multi-platform App UI. O framework da Microsoft pra fazer apps Android/iOS/Windows/macOS com um código C#/XAML só. Sucessor do Xamarin.Forms.

### MVVM (Model-View-ViewModel)
Padrão de arquitetura: a **View** (XAML) mostra; a **ViewModel** segura o estado e a lógica da tela; o **Model** são os dados/domínio. Separa a aparência da lógica. Ver [arquitetura](./architecture.md).

### NavigationPage
A forma clássica de navegar entre telas, com pilha e botão "voltar". Ver [navegação](./navigation.md).

### ObservableObject
A classe base (do CommunityToolkit) que uma ViewModel herda pra ganhar a capacidade de "avisar quando mudou".

### ObservableCollection
Uma lista que avisa a UI quando itens são adicionados/removidos — então um `CollectionView` ligado a ela se atualiza sozinho. Uma `List` comum não faz isso.

### partial (classe parcial)
Uma classe cuja definição é dividida em mais de um lugar — parte escrita por você, parte gerada (pelo XAML ou pelo CommunityToolkit). Por isso páginas e ViewModels precisam ser `partial`.

### Resource / ResourceDictionary
Um valor nomeado e reutilizável (cor, tamanho, estilo) guardado num dicionário. Você define uma vez e usa com `{StaticResource Nome}`. Ver [estilo](./styling-theming.md).

### Shell
A forma moderna de estruturar navegação: rotas (tipo URLs), abas e menu lateral prontos. Ver [navegação](./navigation.md).

### Style
Um conjunto nomeado de propriedades (cor + tamanho + raio...) aplicável a vários controles de uma vez. Evita repetir as mesmas propriedades.

### Trimming
Remover do app final o código que ele não usa, deixando o binário menor e o startup mais rápido.

### View
Em dois sentidos: (1) qualquer elemento visual da tela; (2) no MVVM, a camada de apresentação (o XAML).

### ViewModel
O objeto que segura o estado e os comandos de uma tela, sem depender do MAUI — o que a torna testável. O "cérebro" de uma tela.

### Workload
Um pacote de ferramentas extra que se instala no .NET SDK. O `maui` workload é o que ensina o SDK a fazer apps MAUI.

### XAML
A linguagem de marcação (parecida com HTML) usada pra descrever telas. Cada tag corresponde a um objeto C#. Ver [XAML do zero](./xaml-basics.md).

### x:DataType
Atributo que declara o tipo do BindingContext de um escopo XAML, ativando compiled bindings. O hábito que pega erros de binding no build.

### XAML Hot Reload
Recurso que aplica mudanças no XAML no app **rodando**, sem recompilar tudo. Acelera muito o ajuste visual.
