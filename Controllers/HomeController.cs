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
    [Authorize] // <-- AQUI ESTÁ A TRANCA DE SEGURANÇA QUE BLOQUEIA TUDO!
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

        [HttpPost]
        public async Task<IActionResult> ProcessarDocumento(IFormFile arquivoUpload, string textoLivre, string parametrosRevisao)
        {
            // 1. Usa o texto colado pelo aluno ou uma mensagem padrão se estiver vazio
            string conteudoArquivo = string.IsNullOrWhiteSpace(textoLivre) 
                                     ? "Nenhum arquivo ou texto fornecido." 
                                     : textoLivre;

            // 2. Se um arquivo for enviado, ele sobrepõe o texto colado
            if (arquivoUpload != null && arquivoUpload.Length > 0)
            {
                // Se for PDF, usamos o iText7 para extrair o texto real
                if (arquivoUpload.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    using (var stream = arquivoUpload.OpenReadStream())
                    using (var pdfReader = new PdfReader(stream))
                    using (var pdfDocument = new PdfDocument(pdfReader))
                    {
                        var textBuilder = new StringBuilder();
                        // Percorre todas as páginas do PDF
                        for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                        {
                            var page = pdfDocument.GetPage(i);
                            var text = PdfTextExtractor.GetTextFromPage(page, new SimpleTextExtractionStrategy());
                            textBuilder.AppendLine(text);
                        }
                        conteudoArquivo = textBuilder.ToString();
                    }
                }
                // Se for outro tipo de ficheiro (como .txt), lê normalmente
                else
                {
                    using (var reader = new StreamReader(arquivoUpload.OpenReadStream()))
                    {
                        conteudoArquivo = await reader.ReadToEndAsync();
                    }
                }
            }

            // 3. Chama a IA
            var instrucao = string.IsNullOrWhiteSpace(parametrosRevisao) 
                            ? "Faça uma análise clínica padrão fonoaudiológica." 
                            : parametrosRevisao;
                            
            var resposta = await _iaService.AnalisarDocumentoClinico(instrucao, conteudoArquivo);

            // 4. Salva no Banco de Dados
            var novoDocumento = new DocumentoGerado
            {
                AlunoId = 1,
                Categoria = "Avaliação Clínica",
                ConteudoTexto = conteudoArquivo, // Agora salva o texto correto!
                ParecerIA = resposta, 
                DataCriacao = DateTime.Now
            };

            _context.DocumentosGerados.Add(novoDocumento);
            await _context.SaveChangesAsync(); 

            ViewBag.AvisoBaseDados = $"Documento guardado com sucesso no sistema (ID: {novoDocumento.Id}).";
            ViewBag.TextoOriginal = conteudoArquivo;
            ViewBag.ResultadoIA = resposta;
            
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ReprocessarIA(int id)
        {
            var documento = _context.DocumentosGerados.Find(id);
            if (documento == null) return NotFound("Documento não encontrado.");

            ViewBag.TextoOriginal = documento.ConteudoTexto;
            
            // 1. GERA O NOVO PARECER
            var resposta = await _iaService.AnalisarDocumentoClinico(
                "Faça uma análise clínica padrão fonoaudiológica indicando possíveis erros.", 
                documento.ConteudoTexto);

            // 2. ATUALIZA O BANCO DE DADOS COM A NOVA RESPOSTA
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
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}