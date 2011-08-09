namespace Microsoft.Samples.Tankster.GamePlay.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IUserProvider
    {
        string UserId { get; }
    }
}
