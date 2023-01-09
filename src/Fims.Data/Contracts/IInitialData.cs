using System;
using System.Collections.Generic;


namespace Fims.Data.Contracts
{
    public interface IInitialData
    {
        Type EntityType { get; }

        IEnumerable<object> GetData();
    }
}
