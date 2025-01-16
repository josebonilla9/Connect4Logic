using System.Text.Json.Nodes;
using GroqApiLibrary;

public class AIProgram
{
    private readonly GroqApiClient groqApi;
    public JsonArray Messages { get; }

    public AIProgram(string apiKey)
    {
        groqApi = new GroqApiClient(apiKey);
        Messages = new JsonArray();
    }

    public async Task<string> GetAIResponseAsync(string boardState)
    {
        Messages.Add(new JsonObject
        {
            ["role"] = "user",
            ["content"] = $"¿Dónde debería jugar la IA?, solo tiene que poner la columna. El tablero actual es: {boardState}"
        });

        var request = new JsonObject
        {
            ["model"] = "llama3-70b-8192",
            ["messages"] = new JsonArray(Messages.Select(m => JsonNode.Parse(m.ToJsonString())).ToArray())
        };

        var result = await groqApi.CreateChatCompletionAsync(request);
        var response = result?["choices"]?[0]?["message"]?["content"]?.ToString();

        if (string.IsNullOrEmpty(response))
        {
            throw new Exception("No se obtuvo respuesta de la IA.");
        }

        Messages.Add(new JsonObject
        {
            ["role"] = "assistant",
            ["content"] = response
        });

        return response;
    }
}
