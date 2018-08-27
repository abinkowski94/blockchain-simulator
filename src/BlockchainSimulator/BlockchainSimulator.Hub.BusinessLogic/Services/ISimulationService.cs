using System;
using BlockchainSimulator.Hub.BusinessLogic.Model;
using BlockchainSimulator.Hub.BusinessLogic.Model.Responses;

namespace BlockchainSimulator.Hub.BusinessLogic.Services
{
    public interface ISimulationService
    {
        BaseResponse<Simulation> ChangeConfiguration(Guid scenarioId, BlockchainConfiguration configuration);
        BaseResponse<Simulation> AddNode(Guid scenarioId, ServerNode serverNode);
        BaseResponse<Simulation> DeleteNode(Guid scenarioId, string nodeId);
        BaseResponse<Simulation> ConnectNodes(Guid scenarioId, string nodeId1, string nodeId2);
        BaseResponse<Simulation> StartSimulation(Guid scenarioId, SimulationSettings settings);
    }
}