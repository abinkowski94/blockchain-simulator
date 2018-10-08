using BlockchainSimulator.Common.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace BlockchainSimulator.Node.BusinessLogic.Hubs
{
    public class SimulationHub : Hub<ISiumlationClient>
    {
    }
}