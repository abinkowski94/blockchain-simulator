namespace BlockchainSimulator.Node.DataAccess.Repositories
{
    public static class CacheKeys
    {
        public static string BlockchainTree => "_BlockchainTree";
        public static string LongestBlockchain => "_LongestBlockchain";
        public static string LastBlock => "_LastBlock";
        public static string BlockId(string uniqueId) => $"_BlockId::{uniqueId}";
    }
}