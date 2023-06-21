using PdxSlots.IGPClient.Dtos;

namespace PdxSlots.IGPClient.Services
{
    public interface IGCPClientService
    {
        Task<VerificationDto> GetVerification(string userId, string vendorId, string sesionId);
        Task<VoidTransactionDto> VoidTransaction(string userId, string vendorId, string roundId);
        Task<dynamic> GameMath(string gameMathUrl);
        Task<StartMultiGameCycleDto> StartMultiGameCycle(decimal wager);
        Task<EndMultiGameCycleDto> EndMultiGameCycle(decimal wager, string gameResult);
        Task<StartGameCycleDto> StartGameCycle(int roundId, string userId, string vendorId, string gameId, 
            string payTableId, string sessionId, decimal wager, decimal denomination);
        Task<MoneyTransactionsDto> MoneyTransactions(int roundId, string userId, string vendorId, string sessionId, string gameResult, decimal win);
        Task<EndGameCycleDto> EndGameCycle(int roundId, string userId, string vendorId, string sessionId);
    }
}
