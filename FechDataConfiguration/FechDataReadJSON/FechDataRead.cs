using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FechDataConfiguration.FechDataConfigObj;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FechDataConfiguration.FechDataReadJSON
{
    public class FechDataRead
    {
        public static FechDataConf GetConfig()
        {
            //C:\Users\Administrador\source\repos\TestBlazorServerWithAth0\FechDataConfiguration\JsonConfig\FechDataJsonConfig.json
            string proyectPath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            JObject o1 = JObject.Parse(File.ReadAllText($@"{proyectPath}/FechDataConfiguration/JsonConfig/FechDataJsonConfig.json"));
            FechDataConf fechDataConf = o1.ToObject<FechDataConf>();
            return fechDataConf;
        }
    }
}
