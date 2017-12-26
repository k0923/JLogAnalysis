using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tools.Models
{
    public class JLogModel
    {
        public string LogData { get; set; }

        public ConvertType ConvertType { get; set; } = ConvertType.Json;

        public string Result { get; set; }
    }

    public enum ConvertType
    {
        Xml,
        Json
    }
}
