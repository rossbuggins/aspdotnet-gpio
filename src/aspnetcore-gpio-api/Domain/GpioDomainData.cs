namespace aspnetcore_gpio.Domain
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    public record GpioDomainData (int Number, bool State);

    public class GpioDomainDatas : List<GpioDomainData>
    {
        public GpioDomainDatas(IEnumerable<GpioDomainData> data)
        {
            this.AddRange(data);
        }
        
        public override string ToString()
        {
            return string.Join(",", this.Select(_x=>_x.ToString()).ToArray());
        }
    }
}