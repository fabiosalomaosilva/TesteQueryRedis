using StackExchange.Redis;
using System.Text.Json;

namespace TesteQueryRedis
{
    internal class Program
    {
        private static ConnectionMultiplexer _redis;
        private static IDatabase _db;

        static async Task Main(string[] args)
        {
            _redis = ConnectionMultiplexer.Connect("localhost");
            _db = _redis.GetDatabase();

            // Exemplo de uso
            var cpfExemplo = "12345678900";
            //var cpfExemplo2 = "22345678911";
            //CriarRegistro(cpfExemplo, new { Nome = "Cristiano", Idade = 30 });
            //CriarRegistro(cpfExemplo, new { Nome = "Ronaldo", Idade = 25 });
            //CriarRegistro("64532165498", new { Nome = "Clementina", Idade = 69 });
            //CriarRegistro("75395145698", new { Nome = "Roberta", Idade = 39 });
            //CriarRegistro(cpfExemplo2, new { Nome = "Messi", Idade = 37 });
            //CriarRegistro(cpfExemplo2, new { Nome = "Halland", Idade = 25 });


            // Consultar todos os registros associados ao CPF de exemplo
            var registros = ConsultarRegistrosPorCpf(cpfExemplo);
            foreach (var registro in registros)
            {
                Console.WriteLine(JsonSerializer.Serialize(registro));
            }
        }

        static void CriarRegistro(string cpf, object dados)
        {
            // Gerar um UUID único
            var uuidRegistro = Guid.NewGuid().ToString();
            // Criar a chave no formato CPF:UUID e armazenar os dados associados
            var chave = $"{cpf}:{uuidRegistro}";
            _db.StringSet(chave, JsonSerializer.Serialize(dados));
        }

        // Função para consultar registros por CPF
        static List<object> ConsultarRegistrosPorCpf(string cpf)
        {
            var registros = new List<object>();
            // Recuperar todas as chaves do Redis
            var keys = _db.Execute("KEYS", $"{cpf}:*");
            foreach (var key in (RedisResult[])keys)
            {
                // Para cada chave encontrada, recuperar os dados associados e adicionar à lista de registros
                string dadosJson = _db.StringGet((string)key);
                registros.Add(JsonSerializer.Deserialize<object>(dadosJson));
            }
            return registros;
        }
    }
}
