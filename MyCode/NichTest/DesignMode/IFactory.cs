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
                case "TestVccDmi":
                    myTest = new TestVccDmi();
                    break;
                case "TestIcc":
                    myTest = new TestIcc();
                    break;
                case "TestIBiasDmi":
                    myTest = new TestIBiasDmi();
                    break;
                case "TestTxEye":
                    myTest = new TestTxEye();
                    break;
                case "TestBer":
                    myTest = new TestBer();
                    break;
                case "TestRXLosAD":
                    myTest = new TestRxLos();
                    break;
                case "TestTxPowerDmi":
                    myTest = new TestTxPowerDmi();
                    break;
                case "TestRxPowerDmi":
                    myTest = new TestRxPowerDmi();
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
                case "FLEX86100":
                    myEquipment = new Flex86100();
                    break;
                case "MP1800PPG":
                    myEquipment = new MP1800PPG();
                    break;
                case "MP1800ED":
                    myEquipment = new MP1800ED();
                    break;
                case "TPO4300":
                    myEquipment = new TPO4300();
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
