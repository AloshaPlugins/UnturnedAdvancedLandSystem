using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedHouseSystem.Models;
using Rocket.API;

namespace AdvancedHouseSystem
{
    public class Config : IRocketPluginConfiguration
    {

        public ushort MainEffect, InputEffect, SellStruct;
        public bool DoNotLandOutsideDeployAllItems = false;
        public string Name, LandOutsideDeployBypassPermission = "alosha.bypass";
        public List<Land> Lands = new List<Land>();
        public void LoadDefaults()
        {
            Name = "Alosha Plugins";
            MainEffect = 57557;
            InputEffect = 57558;
            SellStruct = 57559;
            DoNotLandOutsideDeployAllItems = false;
            LandOutsideDeployBypassPermission = "alosha.bypass";
            Lands = new List<Land>();
        }
    }
}
