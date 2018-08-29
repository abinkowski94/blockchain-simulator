using BlockchainSimulator.Hub.BusinessLogic.Model;
using BlockchainSimulator.Hub.BusinessLogic.Model.Responses;
using BlockchainSimulator.Hub.BusinessLogic.Storage;
using System;
using System.Linq;

namespace BlockchainSimulator.Hub.BusinessLogic.Services
{
    public class SimulationService : ISimulationService
    {
        private readonly ISimulationRunnerService _simulationRunnerService;
        private readonly ISimulationStorage _simulationStorage;

        public SimulationService(ISimulationStorage simulationStorage, ISimulationRunnerService simulationRunnerService)
        {
            _simulationStorage = simulationStorage;
            _simulationRunnerService = simulationRunnerService;
        }

        public BaseResponse<Simulation> AddNode(Guid scenarioId, ServerNode serverNode)
        {
            if (serverNode == null)
            {
                return new ErrorResponse<Simulation>("The server node cannot be null!", null);
            }

            if (serverNode.Id == null)
            {
                return new ErrorResponse<Simulation>("The server node id cannot be null!", null);
            }

            if (serverNode.HttpAddress == null)
            {
                return new ErrorResponse<Simulation>("The server node http address cannot be null!", null);
            }

            var simulation = _simulationStorage.GetSimulation(scenarioId);
            if (simulation == null)
            {
                return new ErrorResponse<Simulation>($"Could not find simulation with scenario id:{scenarioId}", null);
            }

            if (simulation.Status != SimulationStatuses.ReadyToRun)
            {
                return new ErrorResponse<Simulation>("You can not make changes while simulation is running",
                    simulation);
            }

            if (simulation.ServerNodes.Any(n => n.Id == serverNode.Id))
            {
                return new ErrorResponse<Simulation>($"There is already server node with id:{serverNode.Id}!",
                    simulation);
            }

            simulation.ServerNodes.Add(serverNode);
            _simulationStorage.SaveChanges();

            return new SuccessResponse<Simulation>("The node has been added!", simulation);
        }

        public BaseResponse<Simulation> ChangeConfiguration(Guid scenarioId, BlockchainConfiguration configuration)
        {
            if (configuration == null)
            {
                return new ErrorResponse<Simulation>("The configuration cannot be null!", null);
            }

            var simulation = _simulationStorage.GetSimulation(scenarioId);
            if (simulation == null)
            {
                return new ErrorResponse<Simulation>($"Could not find simulation with scenario id:{scenarioId}", null);
            }

            if (simulation.Status != SimulationStatuses.ReadyToRun)
            {
                return new ErrorResponse<Simulation>("You can not make changes while simulation is running",
                    simulation);
            }

            simulation.BlockchainConfiguration = configuration;
            _simulationStorage.SaveChanges();

            return new SuccessResponse<Simulation>("The configuration has been changed!", simulation);
        }

        public BaseResponse<Simulation> ConnectNodes(Guid scenarioId, string nodeId1, string nodeId2)
        {
            var simulation = _simulationStorage.GetSimulation(scenarioId);
            if (simulation == null)
            {
                return new ErrorResponse<Simulation>($"Could not find simulation with scenario id:{scenarioId}", null);
            }

            if (simulation.Status != SimulationStatuses.ReadyToRun)
            {
                return new ErrorResponse<Simulation>("You can not make changes while simulation is running",
                    simulation);
            }

            var node1 = simulation.ServerNodes.FirstOrDefault(n => n.Id == nodeId1);
            if (node1 == null)
            {
                return new ErrorResponse<Simulation>($"Could not find node with id:{nodeId1}", simulation);
            }

            var node2 = simulation.ServerNodes.FirstOrDefault(n => n.Id == nodeId2);
            if (node2 == null)
            {
                return new ErrorResponse<Simulation>($"Could not find node with id:{nodeId2}", simulation);
            }

            node1.ConnectedTo.Add(nodeId2);
            node2.ConnectedTo.Add(nodeId1);
            _simulationStorage.SaveChanges();

            return new SuccessResponse<Simulation>("The nodes have been connected!", simulation);
        }

        public BaseResponse<Simulation> DeleteNode(Guid scenarioId, string nodeId)
        {
            var simulation = _simulationStorage.GetSimulation(scenarioId);
            if (simulation == null)
            {
                return new ErrorResponse<Simulation>($"Could not find simulation with scenario id:{scenarioId}", null);
            }

            if (simulation.Status != SimulationStatuses.ReadyToRun)
            {
                return new ErrorResponse<Simulation>("You can not make changes while simulation is running",
                    simulation);
            }

            var node = simulation.ServerNodes.FirstOrDefault(n => n.Id == nodeId);
            if (node == null)
            {
                return new ErrorResponse<Simulation>($"Could not find node with id:{nodeId}", simulation);
            }

            simulation.ServerNodes.Remove(node);
            _simulationStorage.SaveChanges();

            return new SuccessResponse<Simulation>("The node has been removed!", simulation);
        }

        public BaseResponse<Simulation> StartSimulation(Guid scenarioId, SimulationSettings settings)
        {
            if (settings == null)
            {
                return new ErrorResponse<Simulation>("The settings can not be null!", null);
            }

            if (settings.NodesAndTransactions == null)
            {
                return new ErrorResponse<Simulation>("The settings nodes and transaction count cannot be null!", null);
            }

            var simulation = _simulationStorage.GetSimulation(scenarioId);
            if (simulation == null)
            {
                return new ErrorResponse<Simulation>($"Could not find simulation with scenario id:{scenarioId}", null);
            }

            if (simulation.Status != SimulationStatuses.ReadyToRun)
            {
                var errorMessage =
                    $"This simulation has been already started, please wait for results! Current status: {simulation.Status}";
                return new ErrorResponse<Simulation>(errorMessage, simulation);
            }

            _simulationRunnerService.RunSimulation(simulation, settings);

            return new SuccessResponse<Simulation>("Simulation has been started", simulation);
        }
    }
}