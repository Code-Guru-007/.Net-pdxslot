using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PdxSlots.IGPClient.Dtos;
using System.Dynamic;

namespace PdxSlots.IGPClient.Services
{
    public class GCPClientService : IGCPClientService
    {
        private readonly string _baseUrl = "https://rgsdev.pdxslots.com/wallet";
        private readonly ILogger _logger;

        public GCPClientService(ILogger<GCPClientService> logger)
        {
            _logger = logger;
        }

        public async Task<EndGameCycleDto> EndGameCycle(int roundId, string userId, string vendorId, string sessionId)
        {
            // https://rgsdev.pdxslots.com/wallet/endGameCycle.php?rgs_round_id=7260&user_id=chousel&vendor_id=23&session_id=123

            using (HttpClient client = GetClient())
            {
                var url = $"{_baseUrl}/endGameCycle.php?rgs_round_id={roundId}&user_id={userId}&" +
                    $"vendor_id={vendorId}&session_id={sessionId}";

                string responseBody = await Get(client, url);

                return JsonConvert.DeserializeObject<EndGameCycleDto>(responseBody);
            }
        }

        private async Task<string> Get(HttpClient client, string url)
        {
            _logger.LogInformation(url);

            string responseBody = await client.GetStringAsync(url);

            _logger.LogInformation(responseBody);

            return responseBody;
        }

        public async Task<dynamic> GameMath(string gameMathUrl)
        {
            using (HttpClient client = GetClient())
            {
                string responseBody = await Get(client, gameMathUrl);

                return JsonConvert.DeserializeObject<dynamic>(responseBody);
            }
        }

        public async Task<VerificationDto> GetVerification(string userId, string vendorId, string sesionId)
        {
            using (HttpClient client = GetClient())
            {
                var url = $"{_baseUrl}/verification.php?user_id={userId}&vendor_id={vendorId}&session_id={sesionId}";

                string responseBody = await Get(client, url);

                return JsonConvert.DeserializeObject<VerificationDto>(responseBody);
            }
        }

        public async Task<MoneyTransactionsDto> MoneyTransactions(int roundId, string userId, string vendorId, string sessionId, string gameResult, decimal win)
        {
            // https://rgsdev.pdxslots.com/wallet/moneyTransactions.php?rgs_round_id=7260&user_id=chousel&vendor_id=23&session_id=123&game_result=test&win=0.10
            using (HttpClient client = GetClient())
            {
                var url = $"{_baseUrl}/moneyTransactions.php?rgs_round_id={roundId}&user_id={userId}" +
                    $"&vendor_id={vendorId}&session_id={sessionId}&game_result={gameResult}&win={win}";

                string responseBody = await Get(client, url);

                return JsonConvert.DeserializeObject<MoneyTransactionsDto>(responseBody);
            }
        }

        public async Task<StartMultiGameCycleDto> StartMultiGameCycle(decimal wager)
        {
            // https://rgsdev.pdxslots.com/games/game_math/deal.php?wager=100
            using (HttpClient client = GetClient())
            {
                var gameUrl = "https://rgsdev.pdxslots.com/games/game_math/deal.php";

                var url = $"{gameUrl}?wager={wager}";

                string responseBody = await Get(client, url);

                return JsonConvert.DeserializeObject<StartMultiGameCycleDto>(responseBody);
            }
        }

        public async Task<StartGameCycleDto> StartGameCycle(int roundId, string userId, string vendorId,
            string gameId, string payTableId, string sessionId, decimal wager, decimal denomination)
        {
            // https://rgsdev.pdxslots.com/wallet/startGameCycle.php?rgs_round_id=1&user_id=chousel&vendor_id=23&game_id=luckypiggy&paytable_id=/luckypiggy96.php&session_id=123&wager=100&denom=0.01
            using (HttpClient client = GetClient())
            {

                var url = $"{_baseUrl}/startGameCycle.php?rgs_round_id={roundId}&user_id={userId}" +
                    $"&vendor_id={vendorId}&game_id={gameId}&paytable_id={payTableId}&session_id={sessionId}&wager={wager}&denom={denomination}";

                string responseBody = await Get(client, url);

                return JsonConvert.DeserializeObject<StartGameCycleDto>(responseBody);
            }
        }

        public async Task<VoidTransactionDto> VoidTransaction(string userId, string vendorId, string roundId)
        {
            using (HttpClient client = GetClient())
            {
                var url = $"{_baseUrl}/voidTransaction.php?user_id={userId}&vendor_id={vendorId}&rgs_round_id={roundId}";

                string responseBody = await Get(client, url);

                return JsonConvert.DeserializeObject<VoidTransactionDto>(responseBody);
            }
        }

        public async Task<EndMultiGameCycleDto> EndMultiGameCycle(decimal wager, string gameResult)
        {
            // https://rgsdev.pdxslots.com/games/game_math/deal.php?wager=100&game_result={\"R\":1,\"H1\":\"5H,KD,7C,4S,2H\",\"B1\":\"6S,2D,KD\",\"C1\":0}&S1=1S,6S

            using (HttpClient client = GetClient())
            {
                var gameUrl = "https://rgsdev.pdxslots.com/games/game_math/deal.php";

                var url = $"{gameUrl}?wager={wager}&game_result={gameResult}";

                string responseBody = await Get(client, url);

                return JsonConvert.DeserializeObject<EndMultiGameCycleDto>(responseBody);
            }
        }

        private static HttpClient GetClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("cache-control", "max-age=0");

            return client;
        }
    }
}
