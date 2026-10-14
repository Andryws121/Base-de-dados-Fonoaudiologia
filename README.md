# 🎙️ Workspace Clínico Fonoaudiológico (Assistente IA)

![Status](https://img.shields.io/badge/Status-Concluído-success)
![C#](https://img.shields.io/badge/C%23-%23239120.svg?style=flat&logo=c-sharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-5C2D91?style=flat&logo=.net&logoColor=white)
![MySQL](https://img.shields.io/badge/MySQL-005C84?style=flat&logo=mysql&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-563D7C?style=flat&logo=bootstrap&logoColor=white)

> 🔗 **Acesse o projeto online:** [https://fonoaudiologia-app.runasp.net/](https://fonoaudiologia-app.runasp.net/)

## 💻 Sobre o Projeto
O **Workspace Clínico Fonoaudiológico** é uma aplicação web completa desenvolvida para auxiliar estudantes e profissionais de Fonoaudiologia na elaboração, revisão e estruturação de relatórios e laudos clínicos. Integrado com Inteligência Artificial, o sistema analisa casos clínicos e gera pareceres detalhados com base nas normativas do Conselho Federal de Fonoaudiologia (CFFa), otimizando o tempo de diagnóstico e planejamento terapêutico.

## 🚀 Funcionalidades e Destaques Técnicos
O foco do desenvolvimento foi criar uma ferramenta segura, escalável e inteligente para o dia a dia clínico:

- **🧠 Integração Avançada com IA (Groq API):** O usuário submete textos de anamnese ou arquivos, e o sistema se comunica com a API da Groq para processar os dados e retornar um laudo estruturado, indicando hipóteses diagnósticas e planos de terapia.
- **🔒 Segurança e Autenticação:** Sistema de Login e Registro robusto. Rotas privadas protegidas pelo atributo `[Authorize]`, garantindo que apenas profissionais cadastrados acessem a ferramenta de análise e o histórico de pacientes.
- **🗄️ Banco de Dados Relacional:** Utilização do MySQL com Entity Framework Core (Code-First) para armazenar com segurança o histórico de documentos gerados, permitindo reprocessamento e exclusão de análises anteriores.
- **📄 Leitura Dinâmica de Documentos:** Suporte nativo via C# para upload e extração de texto de arquivos, facilitando a submissão de casos clínicos extensos.
- **📱 Interface Moderna:** Layout limpo e responsivo projetado para uso tanto em computadores de clínicas quanto em dispositivos móveis.

## 🛠️ Tecnologias Utilizadas
- **Back-end:** C# com ASP.NET Core MVC.
- **Banco de Dados:** MySQL integrado via Entity Framework Core (Pomelo).
- **Inteligência Artificial:** Groq Cloud API (Modelos LLaMA 3 / Mixtral).
- **Front-end:** Razor Pages (HTML5/C#), CSS3 e Bootstrap.
- **Hospedagem & CI/CD:** Deploy automatizado conectado ao GitHub e hospedado na plataforma MonsterASP.NET (Windows Server).

## 💡 O que aprendi com este projeto
Durante o desenvolvimento desta aplicação full-stack, consegui elevar minhas habilidades arquiteturais e de infraestrutura:
- Conexão e manipulação de bancos de dados relacionais em nuvem usando Entity Framework Core.
- Consumo de APIs RESTful externas (Groq) e gerenciamento seguro de chaves de API (`appsettings.json`).
- Implementação de segurança a nível de rotas e controladores (`[Authorize]`) no ecossistema .NET.
- Resolução de problemas de infraestrutura em servidores Windows (IIS), como a configuração do arquivo `web.config` para resolver conflitos de métodos HTTP (Erro 405 - WebDAV).
- Configuração de pipelines de Continuous Integration / Continuous Deployment (CI/CD) via GitHub.

## ⚙️ Como testar o projeto localmente

1. Clone ou baixe a pasta do repositório:
```bash
git clone [https://github.com/Andryws121/Base-de-dados-Fonoaudiologia.git](https://github.com/Andryws121/Base-de-dados-Fonoaudiologia.git)
```
2. Abra o projeto no **Visual Studio** ou **VS Code**.
3. No arquivo `appsettings.Development.json`, configure a string de conexão do seu banco de dados MySQL local e insira a sua chave da API do Groq.
4. Abra o Console do Gerenciador de Pacotes (ou terminal) e rode as migrações para criar o banco de dados:
```bash
dotnet ef database update
```
5. Execute o projeto (`F5` no Visual Studio ou `dotnet run` no VS Code). O sistema abrirá no seu navegador local (localhost).

## 📞 Contato

Desenvolvido por **Andryws Pedro**.

- **LinkedIn:** [Andryws Pedro](https://www.linkedin.com/in/andryws-pedro-2b15b6275)
- **GitHub:** [Andryws121](https://github.com/Andryws121)
- **WhatsApp:** [+55 (31) 98529-4671](https://wa.me/5531985294671)
