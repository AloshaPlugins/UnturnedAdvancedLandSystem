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
        public LandType DefaultLandType;
        public List<LandType> LandTypes = new List<LandType>();
        public List<Land> Lands = new List<Land>();
        public void LoadDefaults()
        {
            Name = "Alosha Plugins";
            MainEffect = 57557;
            InputEffect = 57558;
            SellStruct = 57559;
            DoNotLandOutsideDeployAllItems = false;
            LandOutsideDeployBypassPermission = "alosha.bypass";
            DefaultLandType = new LandType()
            {
                Id = 1,
                Price = 70000,
                Name = "Default",
                UpTaxPrice = 30,
                AllowDay = 3,
                MaxTax = 100000,
                TaxDurationForMinutes = 3
            };
            LandTypes = new List<LandType>()
            {
                new LandType()
                {
                    Id = 2,
                    Price = 500000,
                    Name = "Home1",
                    UpTaxPrice = 20,
                    AllowDay = 6,
                    MaxTax = 50000,
                    TaxDurationForMinutes = 1
                }
            };
            Lands = new List<Land>();
        }
    }
}
