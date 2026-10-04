using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Base_de_dados_Fonoaudiologia.Services
{
    public class GroqService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public GroqService(IConfiguration config, HttpClient httpClient)
        {
            _apiKey = config["Groq:ApiKey"];
            _httpClient = httpClient;
        }

        public async Task<string> AnalisarDocumentoClinico(string instrucaoAluno, string conteudoExtraidoDoArquivo)
        {
            try
            {
                var url = "https://api.groq.com/openai/v1/chat/completions";

                // O Groq permite separar claramente o que é "Regra do Sistema" e o que é "Texto do Utilizador"
                var systemPrompt = @"ATUE COMO: Um Supervisor Clínico de Fonoaudiologia rigoroso e didático.
                DIRETRIZES: 
                1. Baseie-se EXCLUSIVAMENTE nas normativas do Conselho Federal de Fonoaudiologia (CFFa) e literatura científica da área.
                2. Se o aluno perguntar ou inserir temas fora da Fonoaudiologia, recuse-se a responder e peça foco clínico.";

                var userPrompt = $"PEDIDO DO ALUNO: {instrucaoAluno}\n\nCONTEÚDO DO DOCUMENTO: {conteudoExtraidoDoArquivo}";

                var requestBody = new
                {
                    model = "openai/gpt-oss-120b", 
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = userPrompt }
                    }
                };

                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(requestMessage);
                
                if (response.IsSuccessStatusCode)
                {
                    using var jsonDocument = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                    return jsonDocument.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content").GetString();
                }
                else
                {
                    var erroDetalhado = await response.Content.ReadAsStringAsync();
                    return $"Erro do Groq ({response.StatusCode}): {erroDetalhado}";
                }
            }
            catch (Exception ex)
            {
                return $"Erro interno no sistema: {ex.Message}";
            }
        }
    }
}