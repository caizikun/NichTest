using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NichTest
{
    public class DUT
    {
        protected Log log;
        protected ChipControlByPN dataTable_ChipControlByPN;
        protected ChipDefaultValueByPN dataTable_ChipDefaultValueByPN;
        protected EEPROMDefaultValueByTestPlan dataTable_EEPROMDefaultValueByTestPlan;
        protected DUTCoeffControlByPN dataTable_DUTCoeffControlByPN;
        private static object syncRoot = new Object();//used for thread synchronization

        public enum NameOfADC : int
        {
            VCCADC,
            VCC2ADC,
            VCC3ADC,
            TEMPERATUREADC,
            TEMPERATURE2ADC,
            TXBIASADC,
            TXPOWERADC,
            RXPOWERADC,
            TECTEMPERATUREADC,
            TECCURRENTADC,
            TECVOLTAGEADC
        }

        public enum NameOfChipDAC : int
        {
            BIASDAC,
            MODDAC,
            VC,
            EA,
            CROSSINGDAC,
            LOSDAC,
            APDDAC
        }

        public enum apctype : byte
        {
            none,
            OpenLoop,
            CloseLoop,
            PIDCloseLoop,
            Reserved
        }

        public enum APCMODE : byte
        {
            IBAISandIMODON,
            IBAISandIMODOFF,
            IBIASONandIMODOFF,
            IBIASOFFandIMODON
        }

        public enum ChipOperation : int
        {
            Write = 0,
            Store = 1,
            Read = 2
        }

        public enum Coeff : int
        {
            DMIVCCCOEFC,
            DMIVCC2COEFC,
            DMIVCC3COEFC,
            DMIVCCCOEFB,
            DMIVCC2COEFB,
            DMIVCC3COEFB
        }

        protected struct DebugInterface
        {
            public static byte EngPage;

            public static int StartAddress;
        }

        public virtual bool Initial(ChipControlByPN tableA, ChipDefaultValueByPN tableB, EEPROMDefaultValueByTestPlan tableC, DUTCoeffControlByPN tableD) { return true; }

        public virtual bool FullFunctionEnable()
        {
            return true;
        }

        public virtual string ReadSN()
        {
            return Algorithm.MyNaN.ToString();
        }

        public virtual string ReadFW()
        {
            return Algorithm.MyNaN.ToString();
        }

        public virtual bool InitialChip()
        {
            return false;
        }

        public virtual bool InitialChip(DUTCoeffControlByPN coeffControl, ChipDefaultValueByPN chipDefaultValue)
        {
            return false;
        }

        public virtual bool SetSoftTxDis()
        {
            return true;
        }

        public virtual bool TxAllChannelEnable()
        {
            return true;
        }

        public virtual bool SetSoftTxDis(int channel)
        {
            return true;
        }

        public virtual double ReadDmiTemp()
        {
            return Algorithm.MyNaN;
        }

        public virtual double ReadDmiVcc()
        {
            return Algorithm.MyNaN;
        }

        public virtual double ReadDmiBias(int channel)
        {
            return Algorithm.MyNaN;
        }

        public double[] ReadDmiTxP()
        {
            lock (syncRoot)
            {
                double[] dmi = new double[GlobalParaByPN.TotalChCount];
                for (int i = 0; i < GlobalParaByPN.TotalChCount; i++)
                {
                    dmi[i] = this.ReadDmiTxP(i);
                }
                return dmi;
            }
        }

        public virtual double ReadDmiTxP(int channel)
        {
            return Algorithm.MyNaN;
        }

        public double[] ReadDmiRxP()
        {
            lock (syncRoot)
            {
                double[] dmi = new double[GlobalParaByPN.TotalChCount];
                for (int i = 0; i < GlobalParaByPN.TotalChCount; i++)
                {
                    dmi[i] = this.ReadDmiRxP(i);
                }
                return dmi;
            }
        }

        public virtual double ReadDmiRxP(int channel)
        {
            return Algorithm.MyNaN;
        }

        public virtual ushort ReadADC(NameOfADC enumName, int channel)
        {
            return Algorithm.MyNaN;
        }

        public ushort[] ReadADC(NameOfADC enumName)
        {
            lock (syncRoot)
            {
                ushort[] valueADC = new ushort[GlobalParaByPN.TotalChCount];

                for (int i = 0; i < GlobalParaByPN.TotalChCount; i++)
                {
                    valueADC[i] = this.ReadADC(enumName, i + 1);
                }
                return valueADC;
            }
        }

        public virtual bool WriteChipDAC(NameOfChipDAC enumName, int channel, object writeDAC)
        {
            return false;
        }

        public bool WriteChipDAC(NameOfChipDAC enumName, object writeDAC)
        {
            lock (syncRoot)
            {
                for (int i = 0; i < GlobalParaByPN.TotalChCount; i++)
                {
                    if (!this.WriteChipDAC(enumName, i + 1, writeDAC))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public virtual bool ReadChipDAC(NameOfChipDAC enumName, int channel, out int readDAC)
        {
            readDAC = 0;
            return false;
        }

        public bool ReadChipDAC(NameOfChipDAC enumName, out int readDAC)
        {
            lock (syncRoot)
            {
                readDAC = 0;
                for (int i = 0; i < GlobalParaByPN.TotalChCount; i++)
                {
                    if (!this.ReadChipDAC(enumName, i + 1, out readDAC))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public virtual double CalRxRes(double inputPower_dBm, int channel, double ratio, double U_Ref, double resolution, double R_rssi)
        {
            return Algorithm.MyNaN;
        }

        public virtual bool CloseAndOpenAPC(byte mode)
        {
            return true;
        }

        public virtual bool SetCoeff(Coeff coeff, int channel, string value)
        {
            return false;
        }

        public virtual string GetCoeff(Coeff coeff, int channel)
        {
            return Algorithm.MyNaN.ToString();
        }

        public virtual bool ChkRxLos(int channel) { return false; }
    }
}
