using Microsoft.AspNetCore.Mvc;
using Base_de_dados_Fonoaudiologia.Models;
using Base_de_dados_Fonoaudiologia.Services;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.Text;

namespace Base_de_dados_Fonoaudiologia.Controllers
{
    [Authorize] // <-- TRANCA DE SEGURANÇA GERAL: Obriga a fazer Login para aceder a qualquer página
    public class HomeController : Controller
    {
        private readonly GroqService _iaService; 
        private readonly AppDbContext _context;

        public HomeController(GroqService iaService, AppDbContext context)
        {
            _iaService = iaService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Historico()
        {
            var documentos = _context.DocumentosGerados
                                     .OrderByDescending(d => d.DataCriacao)
                                     .ToList();
                                     
            return View(documentos);
        }

        // =====================================================================================
        // ROTA 1: MODO DIGITADOR (ESTRITO) -> GRAVA NA BASE DE DADOS
        // =====================================================================================
        [HttpPost]
        public async Task<IActionResult> ExtrairRelatorio(IFormFile arquivoUpload, string textoLivre, string parametrosRevisao)
        {
            string conteudoArquivo = await ExtrairConteudoDoArquivo(arquivoUpload, textoLivre);

            // O Prompt bloqueador que criámos para evitar invenções clínicas
            string promptDigitador = @"Você é um DIGITADOR E FORMATADOR DE DADOS CLÍNICOS estritamente passivo e literal. A sua única função é transcrever, organizar e formatar o texto bruto de um relatório fonoaudiológico.
⚠️ REGRAS DE BLOQUEIO ABSOLUTO:
1. PROIBIDO ATUAR COMO MÉDICO/SUPERVISOR: Não avalie a coerência clínica, não sugira próximos passos e não altere diagnósticos.
2. PROIBIDO FAZER MATEMÁTICA: NÃO recalcule escores. 
3. PROIBIDO CRIAR PLANOS: É expressamente proibido gerar Planos Terapêuticos.
4. O FIM DO DOCUMENTO: O documento termina rigorosamente na secção Recomendações.
5. ESCALAS FÍSICAS: Escreva apenas os movimentos observados e o Escore total. Não crie pontuações individuais adivinhadas.
Use tabelas Markdown e texto em negrito para destacar os dados. Formate da melhor maneira profissional. ";

            // Junta as instruções padrão com alguma instrução extra que o aluno digite
            var instrucaoFinal = promptDigitador + (parametrosRevisao ?? "");
                            
            var resposta = await _iaService.AnalisarDocumentoClinico(instrucaoFinal, conteudoArquivo);

            // SALVA NA BASE DE DADOS
            var novoDocumento = new DocumentoGerado
            {
                AlunoId = 1,
                Categoria = "Avaliação Clínica",
                ConteudoTexto = conteudoArquivo,
                ParecerIA = resposta, 
                DataCriacao = DateTime.Now
            };

            _context.DocumentosGerados.Add(novoDocumento);
            await _context.SaveChangesAsync(); 

            ViewBag.AvisoBaseDados = $"Documento guardado com sucesso no sistema (ID: {novoDocumento.Id}). O registo original não foi alterado.";
            ViewBag.TextoOriginal = conteudoArquivo;
            ViewBag.ResultadoIA = resposta;
            
            return View("Index");
        }

        // =====================================================================================
        // ROTA 2: MODO SUPERVISOR (CRIATIVO) -> NÃO GRAVA NA BASE DE DADOS (APENAS ESTUDO)
        // =====================================================================================
        [HttpPost]
        public async Task<IActionResult> GerarPlanoTerapeutico(IFormFile arquivoUpload, string textoLivre, string parametrosRevisao)
        {
            string conteudoArquivo = await ExtrairConteudoDoArquivo(arquivoUpload, textoLivre);

            // Prompt focado no ensino e planeamento criativo
            string promptSupervisor = @"Você é um Fonoaudiólogo Clínico Supervisor com vasta experiência. O aluno forneceu os dados de um paciente. 
A sua tarefa é criar um PLANO DE INTERVENÇÃO TERAPÊUTICA completo, estruturado e inovador para fins de estudo acadêmico. 
Sugira técnicas baseadas em evidências (ex: CFFa, ASHA), divisão de metas (curto, médio e longo prazo), possíveis encaminhamentos multidisciplinares (psicologia, etc.) e dicas práticas para o estagiário atuar. 
Use formato Markdown profissional, estruturado com tópicos e tabelas didáticas. ";

            var instrucaoFinal = promptSupervisor + (parametrosRevisao ?? "");
                            
            var resposta = await _iaService.AnalisarDocumentoClinico(instrucaoFinal, conteudoArquivo);

            // ATENÇÃO: NÃO HÁ _context.SaveChanges() AQUI! O PLANO INVENTADO NÃO CONTAMINA A BD.

            ViewBag.AvisoBaseDados = "⚠️ Atenção: Este Plano Terapêutico foi gerado como material didático e de estudo para o estagiário. NÃO foi guardado na base de dados médica.";
            ViewBag.TextoOriginal = conteudoArquivo;
            ViewBag.ResultadoIA = resposta;
            
            return View("Index");
        }


        // =====================================================================================
        // MÉTODO AUXILIAR: LÊ O FICHEIRO PDF OU TXT (EVITA REPETIÇÃO DE CÓDIGO)
        // =====================================================================================
        private async Task<string> ExtrairConteudoDoArquivo(IFormFile arquivoUpload, string textoLivre)
        {
            string conteudoArquivo = string.IsNullOrWhiteSpace(textoLivre) 
                                     ? "Nenhum arquivo ou texto fornecido." 
                                     : textoLivre;

            if (arquivoUpload != null && arquivoUpload.Length > 0)
            {
                if (arquivoUpload.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    using (var stream = arquivoUpload.OpenReadStream())
                    using (var pdfReader = new PdfReader(stream))
                    using (var pdfDocument = new PdfDocument(pdfReader))
                    {
                        var textBuilder = new StringBuilder();
                        for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                        {
                            var page = pdfDocument.GetPage(i);
                            var text = PdfTextExtractor.GetTextFromPage(page, new SimpleTextExtractionStrategy());
                            textBuilder.AppendLine(text);
                        }
                        conteudoArquivo = textBuilder.ToString();
                    }
                }
                else
                {
                    using (var reader = new StreamReader(arquivoUpload.OpenReadStream()))
                    {
                        conteudoArquivo = await reader.ReadToEndAsync();
                    }
                }
            }
            return conteudoArquivo;
        }


        // =====================================================================================
        // MÉTODOS DE HISTÓRICO E GESTÃO
        // =====================================================================================
        [HttpPost]
        public async Task<IActionResult> ReprocessarIA(int id)
        {
            var documento = _context.DocumentosGerados.Find(id);
            if (documento == null) return NotFound("Documento não encontrado.");

            ViewBag.TextoOriginal = documento.ConteudoTexto;
            
            var resposta = await _iaService.AnalisarDocumentoClinico(
                "Faça uma revisão ortográfica e formatação clínica do documento.", 
                documento.ConteudoTexto);

            documento.ParecerIA = resposta;
            _context.DocumentosGerados.Update(documento);
            await _context.SaveChangesAsync();

            ViewBag.ResultadoIA = resposta;
            ViewBag.AvisoBaseDados = $"Reprocessamento concluído com sucesso e atualizado no histórico.";
            
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeletarDocumento(int id)
        {
            var documento = _context.DocumentosGerados.Find(id);
            if (documento != null)
            {
                _context.DocumentosGerados.Remove(documento);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Historico");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous] 
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}