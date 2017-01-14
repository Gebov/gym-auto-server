using System.Collections.Generic;

namespace Gym.Data.Contracts.Dto
{
    internal class CollectionResponse<T>
    {
        public CollectionResponse()
        {
        }
        
        public CollectionResponse(IEnumerable<T> data, int totalCount)
        {
            this.TotalCount = totalCount;
            this.Data = data;
        }

        public IEnumerable<T> Data { get; set; }
        public int TotalCount { get; set; }
    }
}