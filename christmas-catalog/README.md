### Christmas APP: Catálogo de Presentes Temático (.NET MAUI)

Este projeto é um aplicativo móvel cross-platform desenvolvido com .NET MAUI (Multi-platform App UI), focado em demonstrar a criação de interfaces de usuário (UI) temáticas e layouts complexos usando controles nativos. Ele simula uma tela de catálogo de presentes festivos.
🌟 Visão Geral do Projeto

O objetivo principal deste aplicativo é criar uma experiência de usuário visualmente agradável, replicando o design de um catálogo de itens temáticos de Natal. A arquitetura de UI foca na separação clara entre elementos de cabeçalho fixos e conteúdo de lista rolável.
Telas Implementadas:

    CategoriesPage: Tela principal de catálogo.

🛠️ Tecnologias Utilizadas

    Framework: .NET MAUI (Multi-platform App UI)

    Linguagem: C# e XAML

    Plataforma Alvo: Android (Principalmente), iOS, Windows.

### Estrutura da Interface (CategoriesPage.xaml)

A tela principal utiliza uma Grid mestra com 4 linhas para organizar o conteúdo de forma responsiva:
Linha (Row)	Conteúdo	Propriedade	Objetivo
0	Header da Página	Auto	Fixo (Menu, Título "Gifts", Carrinho).
1	Banner Promocional	Auto	Fixo (Border "Elves Help" e Imagem da Árvore).
2	Título da Lista	Auto	Fixo (Label "Categories").
3	Lista de Categorias	*	Rolável (CollectionView), ocupando o espaço restante.
Destaques do XAML:

    Navegação Interativa: O Border dentro do CollectionView utiliza um TapGestureRecognizer para tornar cada item clicável (Tapped="CategoriaTapped"), permitindo a navegação para a tela de detalhes.

    Layout Fixo vs. Rolável: A combinação de Grid.Row="1" e Grid.Row="3" garante que o banner promocional permaneça visível no topo, enquanto a lista (CollectionView) rola de forma independente.

### Como Rodar o Projeto

Siga os passos abaixo para clonar e executar o aplicativo no seu ambiente Linux.
Pré-requisitos

    .NET SDK (Versão 8 ou superior, idealmente a versão que você está usando - 10.0.x).

    Android SDK configurado (necessário para o deployment no emulador/dispositivo).

    Git instalado e configurado (o que já foi feito).

### 1. Clonar o Repositório

Como a autenticação SSH já foi configurada, use o URL SSH para clonar o projeto:
Bash

git clone git@github.com:cardos0s/Christmas-APP.git
cd Christmas-APP/Christmas

### 2. Restauração e Build

Entre no diretório do projeto (.NET MAUI) e compile. Certifique-se de que o caminho do Android SDK esteja correto para o seu sistema:
Bash

dotnet restore
dotnet build -f net10.0-android /p:AndroidSdkDirectory="/home/julia-cardoso/Android/Sdk"

### 3. Executar no Android

Se o build for bem-sucedido, use o comando dotnet run para instalar e iniciar o aplicativo no seu dispositivo ou emulador conectado via ADB:
Bash

dotnet run -f net10.0-android /p:AndroidSdkDirectory="/home/julia-cardoso/Android/Sdk"

###  Contribuições

Sinta-se à vontade para sugerir melhorias, corrigir bugs ou adicionar novos recursos temáticos!

Desenvolvido por cardos0s.
