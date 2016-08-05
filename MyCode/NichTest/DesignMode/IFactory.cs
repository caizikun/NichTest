using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NichTest
{
    interface IFactory
    {
        ITest CreateTestItem(string name);

        IEquipment CreateEquipment(string name);

        DUT CreateDUT(string name);
    }

    class NateFactory : IFactory
    {
        public ITest CreateTestItem(string name)
        {
            ITest myTest = null;
            switch (name)
            {
                case "QuickCheck":
                    myTest = new QuickCheckTest();
                    break;
                default:
                    myTest = new QuickCheckTest();
                    break;
            }
            return myTest;
        }

        public IEquipment CreateEquipment(string name)
        {
            IEquipment myEquipment = null;
            switch (name)
            {
                case "E3631":
                    myEquipment = new E3631();
                    break;
                case "AQ2211POWERMETER":
                    myEquipment = new AQ2211PowerMeter();
                    break;
                case "AQ2211OPTICALSWITCH":
                    myEquipment = new AQ2211OpticalSwitch();
                    break;
                case "AQ2211ATTEN":
                    myEquipment = new AQ2211Atten();
                    break;
                default:
                    myEquipment = null;
                    break;
            }
            return myEquipment;
        }

        public DUT CreateDUT(string name)
        {
            DUT myDUT = null;
            switch (name)
            {
                case "QSFP28":
                    myDUT = new QSFP28();
                    break;
                default:
                    myDUT = new QSFP28();
                    break;
            }
            return myDUT;
        }
    }
}
