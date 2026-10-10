Workspace Clínico Fonoaudiológico (Assistente IA)

Acesse o projeto online: https://fonoaudiologia-app.runasp.net/

Sobre o Projeto

O Workspace Clínico Fonoaudiológico é uma aplicação web completa desenvolvida para auxiliar estudantes e profissionais de Fonoaudiologia na elaboração, revisão e estruturação de relatórios e laudos clínicos. Integrado com Inteligência Artificial, o sistema analisa casos clínicos e gera pareceres detalhados com base nas normativas do Conselho Federal de Fonoaudiologia (CFFa), otimizando o tempo de diagnóstico e planejamento terapêutico.

Funcionalidades e Destaques Técnicos

O foco do desenvolvimento foi criar uma ferramenta segura, escalável e inteligente para o dia a dia clínico:

Integração Avançada com IA (Groq API): O usuário submete textos de anamnese ou arquivos, e o sistema se comunica com a API da Groq para processar os dados e retornar um laudo estruturado, indicando hipóteses diagnósticas e planos de terapia.

Segurança e Autenticação: Sistema de Login e Registro robusto. Rotas privadas protegidas pelo atributo [Authorize], garantindo que apenas profissionais cadastrados acessem a ferramenta de análise e o histórico de pacientes.

Banco de Dados Relacional: Utilização do MySQL com Entity Framework Core (Code-First) para armazenar com segurança o histórico de documentos gerados, permitindo reprocessamento e exclusão de análises anteriores.

Leitura Dinâmica de Documentos: Suporte nativo via C# para upload e extração de texto de arquivos, facilitando a submissão de casos clínicos extensos.

Interface Moderna: Layout limpo e responsivo projetado para uso tanto em computadores de clínicas quanto em dispositivos móveis.

Tecnologias Utilizadas

Back-end: C# com ASP.NET Core MVC.

Banco de Dados: MySQL integrado via Entity Framework Core (Pomelo).

Inteligência Artificial: Groq Cloud API (Modelos LLaMA 3 / Mixtral).

Front-end: Razor Pages (HTML5/C#), CSS3 e Bootstrap.

Hospedagem & CI/CD: Deploy automatizado conectado ao GitHub e hospedado na plataforma MonsterASP.NET (Windows Server).

O que aprendi com este projeto

Durante o desenvolvimento desta aplicação full-stack, consegui elevar minhas habilidades arquiteturais e de infraestrutura:

Conexão e manipulação de bancos de dados relacionais em nuvem usando Entity Framework Core.

Consumo de APIs RESTful externas (Groq) e gerenciamento seguro de chaves de API (appsettings.json).

Implementação de segurança a nível de rotas e controladores ([Authorize]) no ecossistema .NET.

Resolução de problemas de infraestrutura em servidores Windows (IIS), como a configuração do arquivo web.config para resolver conflitos de métodos HTTP (Erro 405 - WebDAV).

Configuração de pipelines de Continuous Integration / Continuous Deployment (CI/CD) via GitHub.

Como testar o projeto localmente

Clone ou baixe a pasta do repositório:

git clone https://github.com/Andryws121/Base-de-dados-Fonoaudiologia.git


Abra o projeto no Visual Studio ou VS Code.

No arquivo appsettings.Development.json, configure a string de conexão do seu banco de dados MySQL local e insira a sua chave da API do Groq.

Abra o Console do Gerenciador de Pacotes (ou terminal) e rode as migrações para criar o banco de dados:

dotnet ef database update


Execute o projeto (F5 no Visual Studio ou dotnet run no VS Code). O sistema abrirá no seu navegador local (localhost).

Contato

Desenvolvido por Andryws Pedro.

LinkedIn: Andryws Pedro

GitHub: Andryws121

WhatsApp: +55 (31) 98529-4671
