using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StreamBot.Api.Cache
{
    [Serializable]
    public class GameCache
    {
        public required string Game { get; set; }
        public required string Url { get; set; }
    }
}
