namespace BlockchainSimulator.Hub.BusinessLogic.Helpers.Drawing
{
    public class NodeModel
    {
        public string Id { get; set; }

        public string ParentId { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}