using System;
using Newtonsoft.Json;

namespace BlockchainSimulator.Common.Models.Statistics
{
    /// <summary>
    /// The block info
    /// </summary>
    public class BlockInfo
    {
        /// <summary>
        /// The id of the block
        /// </summary>
        [JsonProperty("id", Order = 1)]
        public string Id { get; set; }

        /// <summary>
        /// The timestamp of the block
        /// </summary>
        [JsonProperty("timeStamp", Order = 2)]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// The nonce value
        /// </summary>
        [JsonProperty("nonce", Order = 3)]
        public string Nonce { get; set; }

        /// <summary>
        /// The equality operator
        /// </summary>
        /// <param name="blockInfo1">The block info one</param>
        /// <param name="blockInfo2">The block info two</param>
        /// <returns>True if they are equal</returns>
        public static bool operator ==(BlockInfo blockInfo1, BlockInfo blockInfo2)
        {
            if (blockInfo1 == null && blockInfo2 == null)
            {
                return true;
            }

            if (blockInfo1 == null)
            {
                return false;
            }

            if (blockInfo2 == null)
            {
                return false;
            }

            return blockInfo1.Id == blockInfo2.Id && blockInfo1.Nonce == blockInfo2.Nonce &&
                   blockInfo1.TimeStamp == blockInfo2.TimeStamp;
        }

        /// <summary>
        /// The non-equality operator
        /// </summary>
        /// <param name="blockInfo1">The block info one</param>
        /// <param name="blockInfo2">The block info two</param>
        /// <returns>True if they are not equal</returns>
        public static bool operator !=(BlockInfo blockInfo1, BlockInfo blockInfo2)
        {
            return !(blockInfo1 == blockInfo2);
        }
    }
}