using BlockchainSimulator.Hub.BusinessLogic.Model.Consensus;
using BlockchainSimulator.Hub.BusinessLogic.Model.Responses;
using BlockchainSimulator.Hub.BusinessLogic.Model.Scenarios;
using System;

namespace BlockchainSimulator.Hub.BusinessLogic.Services
{
    public interface ISimulationService
    {
        BaseResponse<Simulation> AddNode(Guid scenarioId, ServerNode serverNode);

        BaseResponse<Simulation> ChangeConfiguration(Guid scenarioId, BlockchainConfiguration configuration);

        BaseResponse<Simulation> ConnectNodes(Guid scenarioId, string nodeId1, string nodeId2);

        BaseResponse<Simulation> DeleteNode(Guid scenarioId, string nodeId);

        BaseResponse<Simulation> StartSimulation(Guid scenarioId, SimulationSettings settings);
    }
}