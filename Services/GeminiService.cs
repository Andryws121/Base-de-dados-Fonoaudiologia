using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Base_de_dados_Fonoaudiologia.Services
{
    public class GeminiService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public GeminiService(IConfiguration config, HttpClient httpClient)
        {
            _apiKey = config["Gemini:ApiKey"];
            _httpClient = httpClient;
        }

        public async Task<string> AnalisarDocumentoClinico(string instrucaoAluno, string conteudoExtraidoDoArquivo)
        {
            int maxTentativas = 3; // O sistema vai tentar 3 vezes antes de desistir

            for (int tentativa = 1; tentativa <= maxTentativas; tentativa++)
            {
                try
                {
                    var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.8-flash:generateContent?key={_apiKey}";

                    var promptRestrito = $@"
                    ATUE COMO: Um Supervisor Clínico de Fonoaudiologia rigoroso e didático.
                    DIRETRIZES: 
                    1. Baseie-se EXCLUSIVAMENTE nas normativas do Conselho Federal de Fonoaudiologia (CFFa) e literatura científica da área.
                    2. Se o aluno perguntar ou inserir temas fora da Fonoaudiologia, recuse-se a responder e peça foco clínico.
                    
                    PEDIDO DO ALUNO: {instrucaoAluno}
                    
                    CONTEÚDO DO DOCUMENTO ANEXADO:
                    {conteudoExtraidoDoArquivo}";

                    var requestBody = new
                    {
                        contents = new[]
                        {
                            new { parts = new[] { new { text = promptRestrito } } }
                        }
                    };

                    var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync(url, content);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        using var jsonDocument = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                        var textoResposta = jsonDocument.RootElement
                            .GetProperty("candidates")[0]
                            .GetProperty("content")
                            .GetProperty("parts")[0]
                            .GetProperty("text").GetString();

                        return textoResposta;
                    }
                    else if ((int)response.StatusCode == 503)
                    {
                        // Se for erro 503 (Servidor ocupado) e não for a última tentativa, o sistema espera
                        if (tentativa < maxTentativas)
                        {
                            await Task.Delay(3000); // Espera 3 segundos (3000 milissegundos) e tenta de novo
                            continue; 
                        }
                        else
                        {
                            return "Os servidores da inteligência artificial estão muito congestionados no momento. Por favor, aguarde alguns minutos e tente novamente.";
                        }
                    }
                    else
                    {
                        var erroDetalhado = await response.Content.ReadAsStringAsync();
                        return $"Erro do Google ({response.StatusCode}): {erroDetalhado}";
                    }
                }
                catch (Exception ex)
                {
                    return $"Erro interno no sistema: {ex.Message}";
                }
            }
            
            return "Falha ao processar a requisição.";
        }
    }
}