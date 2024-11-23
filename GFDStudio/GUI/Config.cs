using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFDStudio
{
    public class Config
    {
        public bool DarkMode { get; set; } = true;
        public bool RetainTextureNames { get; set; } = true;
        public bool RetainMaterialColors { get; set; } = false;
        public bool SaveReplacedTexturesExternally { get; set; } = true;

        public void SaveJson( Config settings )
        {
            File.WriteAllText( "Config.json", JsonConvert.SerializeObject( settings, Newtonsoft.Json.Formatting.Indented ) );
        }

        public Config LoadJson()
        {
            if ( !File.Exists( "Config.json" ) )
                return new Config();

            return JsonConvert.DeserializeObject<Config>( File.ReadAllText( "Config.json" ) );
        }
    }
    
}