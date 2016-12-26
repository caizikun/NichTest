using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NichTest
{
    public class EEPROM
    {
        private EEPROM() { }//make sure it will not be instantiated
        public static double readdmitemp(int deviceIndex,int deviceAddress, int regAddress,int phycialAdress=0, int mdiomode=0)
        {
            byte[] buff = new byte[2];
            UInt16[] buffmdio=new UInt16[1];
            double temperature = 0.0;
            try
            {
                if (mdiomode == 1)
                {
                    buffmdio = IOPort.ReadMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, 1);

                    buff[0] = (byte)(buffmdio[0] / 256);
                    buff[1] = (byte)(buffmdio[0] & 0xFF);
                }
                else
                {
                    buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 2);
                }
                if (buff[0] > Convert.ToByte(127))
                {
                    temperature = (buff[0] + (buff[1] / 256.0)) - 256;
                }
                else
                {
                    temperature = (buff[0] + (buff[1] / 256.0));
                }
                temperature = Math.Round(temperature, 4);
                return temperature;

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return temperature;
            }
        }
        public static double readdmivcc(int deviceIndex, int deviceAddress, int regAddress, int phycialAdress = 0, int mdiomode = 0)
        {
            byte[] buff = new byte[2];
            UInt16[] buffmdio = new UInt16[1];
            double vcc = 0.0;
            try
            {
                if (mdiomode == 1)
                {
                    buffmdio = IOPort.ReadMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, 1);
                    vcc = buffmdio[0] / 10000.0;
                }
                else
                {
                    buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 2);
                    vcc = (buff[0] * 256 + buff[1]) / 10000.0;
                }
                
                vcc = Math.Round(vcc, 4);
                return vcc;

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return vcc;
            }
        }
        public static double readdmibias(int deviceIndex, int deviceAddress, int regAddress, int phycialAdress = 0, int mdiomode = 0)
        {
            byte[] buff = new byte[2];
            UInt16[] buffmdio = new UInt16[1];
            double bias = 0.0;
            try
            {
                if (mdiomode == 1)
                {
                    buffmdio = IOPort.ReadMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, 1);
                    bias = buffmdio[0] / 500.0;
                }
                else
                {
                    buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 2);
                    bias = (buff[0] * 256 + buff[1]) / 500.0;
                }
                bias = Math.Round(bias, 4);
                return bias;

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return bias;
            }
        }
        public static double readdmitxp(int deviceIndex, int deviceAddress, int regAddress, int phycialAdress = 0, int mdiomode = 0)
        {
            byte[] buff = new byte[2];
            UInt16[] buffmdio = new UInt16[1];
            double txp = 0.0;
            try
            {
                if (mdiomode == 1)
                {
                    buffmdio = IOPort.ReadMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, 1);
                    txp = 10 * (Math.Log10(buffmdio[0] * (1E-4)));
                }
                else
                {
                    buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 2);
                    txp = 10 * (Math.Log10((buff[0] * 256 + buff[1]) * (1E-4)));
                }
                txp = Math.Round(txp, 4);
                return txp;

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return txp;
            }
        }
        public static double readdmirxp(int deviceIndex, int deviceAddress, int regAddress, int phycialAdress = 0, int mdiomode = 0)
        {
            byte[] buff = new byte[2];
            UInt16[] buffmdio = new UInt16[1];
            double rxp = 0.0;
            try
            {
                if (mdiomode == 1)
                {
                    buffmdio = IOPort.ReadMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, 1);
                    rxp = 10 * (Math.Log10(buffmdio[0] * (1E-4)));
                }
                else
                {
                    buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 2);
                    rxp = 10 * (Math.Log10((buff[0] * 256 + buff[1]) * (1E-4)));
                }
                rxp = Math.Round(rxp, 4);
                return rxp;

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return rxp;
            }
        }

        //read a/w
        //temp  alarm/waring
        public static double readtempaw(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[2];
            double tempaw = 0.0;
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 2);
                if (buff[0] > Convert.ToByte(127))
                {
                    tempaw = ((buff[0] * 256 + buff[1]) - 65536) / 256.0;
                    
                }
                else
                {
                     tempaw = (buff[0] * 256 + buff[1]) / 256.0;

                }
                tempaw = Math.Round(tempaw, 4);
                return tempaw;

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return tempaw;
            }
        }

        //vcc alarm/waring
        public static double readvccaw(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[2];
            double vccaw = 0.0;
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 2);
                vccaw = (buff[0] * 256 + buff[1]) * 0.0001;
                vccaw = Math.Round(vccaw, 4);
                return vccaw;
            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return vccaw;
            }
        }

        //bias alarm/waring
        public static double readbiasaw(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[2];
            double biasaw = 0.0;
            try
            {

                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 2);
                biasaw = (buff[0] * 256 + buff[1]) * 0.002;
                biasaw = Math.Round(biasaw, 4);
                return biasaw;

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return biasaw;
            }
        }

        //txp alarm/waring
        public static double readtxpaw(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[2];
            double txpaw= 0.0;
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 2);
                txpaw = (buff[0] * 256 + buff[1]) * 0.1;
                //txpaw = 10 * (Math.Log10((buff[0] * 256 + buff[1]) * (1E-4)));
                txpaw = Math.Round(txpaw, 4);
                return txpaw;
               
            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return txpaw;
            }
        }
        //rxp alarm/waring
        public static double readrxpaw(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[2];
            double rxpaw = 0.0;
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 2);
                rxpaw = (buff[0] * 256 + buff[1]) * 0.1;
                //rxpaw = 10 * (Math.Log10((buff[0] * 256 + buff[1]) * (1E-4)));
                rxpaw = Math.Round(rxpaw, 4);
                return rxpaw;

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return rxpaw;
            }
        }
        
        //check temp high alarm
        public static bool ChkTempHA(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if ((buff[0] & 0x80) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        //check temp low alarm
        public static bool ChkTempLA(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if ((buff[0] & 0x40) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        //check vcc high alarm
        public static bool ChkVccHA(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if ((buff[0] & 0x20) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        //check vcc low alarm
        public static bool ChkVccLA(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if ((buff[0] & 0x10) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        //check bias high alarm
        public static bool ChkBiasHA(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if ((buff[0] & 0x08) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        //check bias low alarm
        public static bool ChkBiasLA(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if ((buff[0] & 0x04) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        //check txp high alarm
        public static bool ChkTxpHA(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if ((buff[0] & 0x02) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        //check txp low alarm
        public static bool ChkTxpLA(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if ((buff[0] & 0x01) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        //check rxp high alarm
        public static bool ChkRxpHA(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if ((buff[0] & 0x80) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        //check rxp low alarm
        public static bool ChkRxpLA(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if ((buff[0] & 0x40) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        //check temp high warning
        public static bool ChkTempHW(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if ((buff[0] & 0x80) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        //check temp low warning
        public static bool ChkTempLW(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if ((buff[0] & 0x40) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        //check vcc high warning
        public static bool ChkVccHW(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if ((buff[0] & 0x20) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        //check vcc low warning
        public static bool ChkVccLW(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if ((buff[0] & 0x10) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        //check bias high warning
        public static bool ChkBiasHW(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if ((buff[0] & 0x08) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        //check bias low warning
        public static bool ChkBiasLW(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if ((buff[0] & 0x04) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        //check txp high warning
        public static bool ChkTxpHW(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if ((buff[0] & 0x02) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        //check txp low warning
        public static bool ChkTxpLW(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if ((buff[0] & 0x01) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        //check rxp high warning
        public static bool ChkRxpHW(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if ((buff[0] & 0x80) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        //check rxp low warning
        public static bool ChkRxpLW(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if ((buff[0] & 0x40) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        //read optional status /control bit 110
        //tx disable
        public static bool ChkTxDis(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if (((buff[0] & 0x80) != 0) || ((buff[0] & 0x40) != 0))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        //tx fault
        public static bool ChkTxFault(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if ((buff[0] & 0x04) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        //rx los
        public static bool ChkRxLos(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if ((buff[0] & 0x02) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return false;
            }
        }
        
        //set temp  alarm warning
        public static void settempaw(int deviceIndex, int deviceAddress, int regAddress, decimal tempaw)
        {
            byte[] buff = new byte[2];
            Int64 itest1;
            try
            {
                if ((Convert.ToInt64(tempaw) * 256) >= 0)
                {
                    itest1 = Convert.ToInt64(tempaw) * 256;
                    buff[0] = Convert.ToByte(itest1 / 256);
                    buff[1] = Convert.ToByte(itest1 % 256);
                }
                else
                {
                    itest1 = 65536 - Math.Abs(Convert.ToInt64(tempaw) * 256);
                    buff[0] = Convert.ToByte(itest1 / 256);
                    buff[1] = Convert.ToByte(itest1 % 256);

                }
                IOPort.WrtieReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                System.Threading.Thread.Sleep(1000);


            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
            }
        }

        //set vcc alarm warning 
        public static void setvccaw(int deviceIndex, int deviceAddress, int regAddress, decimal vccaw)
        {
            byte[] buff = new byte[2];
            Int64 itest1;
            try
            {
                itest1 = Convert.ToInt64(vccaw) * 10000;
                buff[0] = Convert.ToByte(itest1 / 256);
                buff[1] = Convert.ToByte(itest1 % 256);
                IOPort.WrtieReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                System.Threading.Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
            }
        }

        //set bias alarm warning 
        public static void setbiasaw(int deviceIndex, int deviceAddress, int regAddress, decimal biasaw)
        {
            byte[] buff = new byte[2];
            Int64 itest1;
            try
            {
                itest1 = Convert.ToInt64(biasaw) * 500;
                buff[0] = Convert.ToByte(itest1 / 256);
                buff[1] = Convert.ToByte(itest1 % 256);
                IOPort.WrtieReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                System.Threading.Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
            }
        }

        //set txp alarm warning 
        public static void settxpaw(int deviceIndex, int deviceAddress, int regAddress, decimal txpaw)
        {
            byte[] buff = new byte[2];
            Int64 itest1;
            try
            {
                itest1 = Convert.ToInt64(txpaw) * 10;
                buff[0] = Convert.ToByte(itest1 / 256);
                buff[1] = Convert.ToByte(itest1 % 256);
                IOPort.WrtieReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                System.Threading.Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
            }
        }

        //set rxp alarm warning 
        public static void setrxpaw(int deviceIndex, int deviceAddress, int regAddress, decimal rxpaw)
        {
            byte[] buff = new byte[2];
            Int64 itest1;
            try
            {
                itest1 = Convert.ToInt64(rxpaw) * 10;
                buff[0] = Convert.ToByte(itest1 / 256);
                buff[1] = Convert.ToByte(itest1 % 256);
                IOPort.WrtieReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                System.Threading.Thread.Sleep(1000);

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
            }
        }
        
        //set txdis soft
        public static void SetSoftTxDis(int deviceIndex, int deviceAddress, int regAddress)
        {
            byte[] buff = new byte[1];
            try
            {
                buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                buff[0] = (byte)(buff[0] | 0x40);
                IOPort.WrtieReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                System.Threading.Thread.Sleep(1000);

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
            }
        }

        //w/r  sn/pn
        public static string ReadSn(int deviceIndex, int deviceAddress, int regAddress, int phycialAdress = 0, int mdiomode = 0)
        {
            byte[] buff1 = new byte[16];
            UInt16[] buff = new UInt16[16];
            string sn = "ffffffff";
            try
            {
                if (mdiomode == 1)
                {
                    buff = IOPort.ReadMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, 16);
                    for (int i = 0; i < 16; i++)
                    {
                        buff1[i] = (byte)(buff[i]);
                    }
                }
                else
                {
                    buff1 = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 16);
                }
                    sn = Convert.ToChar(Convert.ToInt64(buff1[0])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[1])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[2])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[3])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[4])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[5])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[6])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[7])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[8])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[9])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[10])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[11])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[12])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[13])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[14])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[15])).ToString();
               return sn.Trim();

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return sn;
            }
        }
        //read pn==sn
        public static string ReadPn(int deviceIndex, int deviceAddress, int regAddress, int phycialAdress = 0, int mdiomode = 0)
        {
            byte[] buff1 = new byte[16];
            UInt16[] buff = new UInt16[16];
            string pn = "ffffffff";
            try
            {
                if (mdiomode == 1)
                {
                    buff = IOPort.ReadMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, 16);
                    for (int i = 0; i < 16; i++)
                    {
                        buff1[i] = (byte)(buff[i]);
                    }
                }
                else
                {

                    buff1 = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 16);
                }
                pn = Convert.ToChar(Convert.ToInt64(buff1[0])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[1])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[2])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[3])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[4])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[5])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[6])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[7])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[8])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[9])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[10])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[11])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[12])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[13])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[14])).ToString() + Convert.ToChar(Convert.ToInt64(buff1[15])).ToString();
                return pn.Trim();

            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                return pn;
            }
        }
        //write sn
        public static void SetSn(int deviceIndex, int deviceAddress, int regAddress, string sn)
        {
            byte[] buff = new byte[16];
            try
            {


                buff = Encoding.Default.GetBytes(sn);
                IOPort.WrtieReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                System.Threading.Thread.Sleep(1000);
               

            }
            catch (Exception ex)
            {

                Log.SaveLogToTxt(ex.Message);
            }
        }
        //write pn
        public static void SetPn(int deviceIndex, int deviceAddress, int regAddress, string pn)
        {
            byte[] buff = new byte[16];
            try
            {


                buff = Encoding.Default.GetBytes(pn.Trim());
                IOPort.WrtieReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                System.Threading.Thread.Sleep(1000);
            }
            catch (Exception ex)
            {

                Log.SaveLogToTxt(ex.Message);
            }
        }
        //read manufacture data
        //read fwrev
        public static string ReadFWRev(int deviceIndex, int deviceAddress, int regAddress, int phycialAdress = 0, int mdiomode = 0)
        {
            byte[] buff = new byte[2];
            string fwrev = "ff";
            UInt16[] buff1 = new UInt16[1];
            try
            {
                if (mdiomode == 1)
                {
                    buff1 = IOPort.ReadMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, 2);
                    buff[0] = (byte)(buff1[0]);
                    buff[1] = (byte)(buff1[1]);
                }
                else
                {

                    buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 2);
                    
                }
                fwrev = Convert.ToString((buff[0] * 256 + buff[1]), 16);
                return fwrev.Trim();

            }
            catch (Exception ex)
            {

                Log.SaveLogToTxt(ex.Message);
                return fwrev;
            }
        }
        //read adc
        public static UInt16 readadc(int deviceIndex, int deviceAddress, int regAddress, int phycialAdress = 0, int mdiomode = 0)
        {
            byte[] buff = new byte[2];
            UInt16 adc = 0;
            UInt16[] buff1 = new UInt16[2];
            try
            {
                for (int i = 0; i < 4; i++)
                {
                    if (mdiomode == 1)
                    {
                        buff1 = IOPort.ReadMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, 2);
                        buff[0] = (byte)(buff1[0]);
                        buff[1] = (byte)(buff1[1]);
                    }
                    else
                    {
                        buff = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 2);
                    }
                    if (buff[0] != 0)
                        break;

                }

                adc = (UInt16)((buff[0]) * 256 + buff[1]);
                return adc;

            }
            catch (Exception ex)
            {

                Log.SaveLogToTxt(ex.Message);
                return adc;
            }
        }
       
        //coef ieee754
        public static bool floattoieee(int deviceIndex, int deviceAddress, int regAddress, float fcoef, int phycialAdress = 0, int mdiomode = 0)
        {
            bool flag = false;
            byte[] bcoef = new byte[4];
            UInt16[] bcoefmdio = new UInt16[4];
            System.Threading.Thread.Sleep(100);
            bcoef = BitConverter.GetBytes(fcoef);
            System.Threading.Thread.Sleep(100);
            bcoef.Reverse();
            //Array.Reverse(bcoef);
            if (mdiomode == 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    bcoefmdio[i]=(UInt16)(bcoef[i]);
                }
               
                IOPort.WriteMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, bcoefmdio);
                 System.Threading.Thread.Sleep(200);

                for (int i = 0; i < 4; i++)
                {
                    UInt16[] rcoef = new UInt16[4];
                   
                    rcoef = IOPort.ReadMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, 4);
                    if ((bcoefmdio[0] != rcoef[0]) || (bcoefmdio[1] != rcoef[1]) || (bcoefmdio[2] != rcoef[2]) || (bcoefmdio[3] != rcoef[3]))
                    {
                        if (i>2)
                        {
                            return false;
                        }
                        IOPort.WriteMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, bcoefmdio);
                        System.Threading.Thread.Sleep(100);
                    }
                    else
                    {
                        flag = true;
                        break;
                    }

                }
            }
            else
            {
                IOPort.WrtieReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, bcoef);
                System.Threading.Thread.Sleep(200);

                for (int i = 0; i < 4; i++)
                {
                    byte[] rcoef = new byte[4];
                    rcoef = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 4);
                    if ((bcoef[0] != rcoef[0]) || (bcoef[1] != rcoef[1]) || (bcoef[2] != rcoef[2]) || (bcoef[3] != rcoef[3]))
                    {
                        IOPort.WrtieReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, bcoef);
                        System.Threading.Thread.Sleep(200);
                    }
                    else
                    {
                        flag = true;
                        break;
                    }

                }
            }
            return flag;
        }
        public static float ieeetofloat(int deviceIndex, int deviceAddress, int regAddress, int phycialAdress = 0, int mdiomode = 0)
        {
            float fcoef;
            byte[] bcoef = new byte[4];

            UInt16[] bcoefmdio = new UInt16[4];
            if (mdiomode == 1)
            {
                bcoefmdio = IOPort.ReadMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, 4);
                for (int i = 0; i < 4; i++)
                {
                    bcoef[i] = (byte)(bcoefmdio[i]);
                }
            }
            else
            {
                bcoef = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 4);
                
            }
            System.Threading.Thread.Sleep(200);
            bcoef.Reverse();
            fcoef = BitConverter.ToSingle(bcoef, 0);
            return fcoef;
        }
        public static bool u16tobyte(int deviceIndex, int deviceAddress, int regAddress, UInt16 coef, int phycialAdress = 0, int mdiomode = 0)
        {
            bool flag = false;
            byte[] buff = new byte[2];
            UInt16[] bcoefmdio = new UInt16[2];
            buff[0] = (byte)((coef >> 8) & 0xff);
            buff[1] = (byte)(coef & 0xff);
            bcoefmdio[0] = (UInt16)(buff[0]);
            bcoefmdio[1] = (UInt16)(buff[1]);
            if (mdiomode == 1)
            {
                IOPort.WriteMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, bcoefmdio);
                System.Threading.Thread.Sleep(200);

                for (int i = 0; i < 4; i++)
                {
                    UInt16[] rcoef = new UInt16[2];
                    rcoef = IOPort.ReadMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, 2);
                    if ((bcoefmdio[0] != rcoef[0]) || (bcoefmdio[1] != rcoef[1]))
                    {
                        IOPort.WriteMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, bcoefmdio);
                        System.Threading.Thread.Sleep(200);
                    }
                    else
                    {
                        flag = true;
                        break;
                    }

                }

            }
            else
            {
                IOPort.WrtieReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                System.Threading.Thread.Sleep(200);

                for (int i = 0; i < 4; i++)
                {
                    byte[] rcoef = new byte[2];
                    rcoef = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 2);
                    if ((buff[0] != rcoef[0]) || (buff[1] != rcoef[1]))
                    {
                        IOPort.WrtieReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                        System.Threading.Thread.Sleep(200);
                    }
                    else
                    {
                        flag = true;
                        break;
                    }

                }
            }
            return flag;
        }
        public static bool u8tobyte(int deviceIndex, int deviceAddress, int regAddress, UInt16 coef, int phycialAdress = 0, int mdiomode = 0)
        {
            bool flag = false;
            byte[] buff = new byte[1];
            UInt16[] bcoefmdio = new UInt16[1];
            buff[0] = (byte)((coef >> 8) & 0xff);

            bcoefmdio[0] = coef;
          
            if (mdiomode == 1)
            {
                IOPort.WriteMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, bcoefmdio);
                System.Threading.Thread.Sleep(200);

                for (int i = 0; i < 4; i++)
                {
                    UInt16[] rcoef = new UInt16[1];
                    rcoef = IOPort.ReadMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, 1);
                    if (bcoefmdio[0] != rcoef[0])
                    {
                        IOPort.WriteMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, bcoefmdio);
                        System.Threading.Thread.Sleep(200);
                    }
                    else
                    {
                        flag = true;
                        break;
                    }

                }

            }
            else
            {
                IOPort.WrtieReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                System.Threading.Thread.Sleep(200);

                for (int i = 0; i < 4; i++)
                {
                    byte[] rcoef = new byte[2];
                    rcoef = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 2);
                    if ((buff[0] != rcoef[0]) || (buff[1] != rcoef[1]))
                    {
                        IOPort.WrtieReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                        System.Threading.Thread.Sleep(200);
                    }
                    else
                    {
                        flag = true;
                        break;
                    }

                }
            }
            return flag;
        }
       
        public static UInt16 bytetou16(int deviceIndex, int deviceAddress, int regAddress, int phycialAdress = 0, int mdiomode = 0)
        {
            byte[] bcoef = new byte[2];
            UInt16[] bcoefmdio = new UInt16[2];
            if (mdiomode == 1)
            {
                bcoefmdio = IOPort.ReadMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, 2);
                for (int i = 0; i < 2; i++)
                {
                    bcoef[i] = (byte)(bcoefmdio[i]);
                }
            }
            else
            {
                bcoef = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 2);
                
            }

            System.Threading.Thread.Sleep(200);

            UInt16 U16coef = (UInt16)((bcoef[0] << 8) + bcoef[1]);
            return U16coef;

        }
        public static bool u32tobyte(int deviceIndex, int deviceAddress, int regAddress, UInt32 coef, int phycialAdress = 0, int mdiomode = 0)
        {
            bool flag = false;
            byte[] buff = new byte[4];
            UInt16[] bcoefmdio = new UInt16[4];
            buff[0] = (byte)((coef >> 24) & 0xff);
            buff[1] = (byte)((coef >> 16) & 0xff);
            buff[2] = (byte)((coef >> 8) & 0xff);
            buff[3] = (byte)(coef & 0xff);
            for (int i = 0; i < 4; i++)
            {
                bcoefmdio[i] = (UInt16)(buff[i]);
            }

            if (mdiomode == 1)
            {
                IOPort.WriteMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, bcoefmdio);
                System.Threading.Thread.Sleep(200);

                for (int i = 0; i < 4; i++)
                {
                    UInt16[] rcoef = new UInt16[4];
                    rcoef = IOPort.ReadMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, 4);
                    if ((bcoefmdio[0] != rcoef[0]) || (bcoefmdio[1] != rcoef[1]) || (bcoefmdio[2] != rcoef[2]) || (bcoefmdio[3] != rcoef[3]))
                    {
                        IOPort.WriteMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, bcoefmdio);
                        System.Threading.Thread.Sleep(200);
                    }
                    else
                    {
                        flag = true;
                        break;
                    }

                }
            }
            else
            {
                IOPort.WrtieReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                System.Threading.Thread.Sleep(200);

                for (int i = 0; i < 4; i++)
                {
                    byte[] rcoef = new byte[4];
                    rcoef = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 4);
                    if ((buff[0] != rcoef[0]) || (buff[1] != rcoef[1]) || (buff[2] != rcoef[2]) || (buff[3] != rcoef[3]))
                    {
                        IOPort.WrtieReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                        System.Threading.Thread.Sleep(200);
                    }
                    else
                    {
                        flag = true;
                        break;
                    }

                }
            }
            
            return flag;
        }
        public static UInt32 bytetou32(int deviceIndex, int deviceAddress, int regAddress, int phycialAdress = 0, int mdiomode = 0)
        {
            byte[] bcoef = new byte[4];
            UInt16[] bcoefmdio = new UInt16[4];
            if (mdiomode == 1)
            {
                bcoefmdio = IOPort.ReadMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, 4);
                for (int i = 0; i < 4; i++)
                {
                    bcoef[i] = (byte)(bcoefmdio[i]);
                }
            }
            else
            {
                bcoef = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 4);
            }
            UInt32 U32coef = (UInt32)((bcoef[0] << 24) + (bcoef[1] << 16) + (bcoef[2] << 8) + bcoef[3]);
            System.Threading.Thread.Sleep(200);
            return U32coef;
        }

        public static bool SetCoef(int deviceIndex, int deviceAddress, int regAddress, string coef, byte format, int phycialAdress = 0, int mdiomode = 0)
        {//1 ieee754;2 UInt16;3 UInt32
            bool flag = false;
            try
            {

                switch (format)
                {
                    case 1:
                        float fcoef = Convert.ToSingle(coef);
                        flag = floattoieee(deviceIndex, deviceAddress, regAddress, fcoef, phycialAdress,mdiomode);
                        break;
                    case 2:
                        //UInt16 u16coef = Convert.ToUInt16(Convert.ToDouble(coef));
                        UInt16 u16coef = (UInt16)(Convert.ToDouble(coef));
                        flag = u16tobyte(deviceIndex, deviceAddress, regAddress, u16coef, phycialAdress, mdiomode);
                        break;
                    case 3://u8
                        byte Bcoef = Convert.ToByte(coef);
                       // float fcoef = Convert.ToSingle(coef);
                        flag = u8tobyte(deviceIndex, deviceAddress, regAddress, Bcoef, phycialAdress, mdiomode);
                        break;
                    case 4:
                        UInt32 u32coef = Convert.ToUInt32(Convert.ToDouble(coef));
                       // float fcoef = Convert.ToSingle(coef);
                        flag = u32tobyte(deviceIndex, deviceAddress, regAddress, u32coef, phycialAdress, mdiomode);
                        break;
                    default:
                        break;
                }
                return flag;
            }
            catch (Exception ex)
            {

                Log.SaveLogToTxt(ex.Message);
                return false;
            }
           
        }
     
        public static string ReadCoef(int deviceIndex, int deviceAddress, int regAddress, byte format, int phycialAdress = 0, int mdiomode = 0)
        {//1 ieee754;2 UInt16;3 UInt32
            string strcoef="";
            try
            {
               
                switch (format)
                {
                    case 1:
                        float fcoef = ieeetofloat(deviceIndex, deviceAddress, regAddress, phycialAdress,mdiomode);
                        strcoef = fcoef.ToString();
                        break;
                    case 2:
                        UInt16 u16coef = bytetou16(deviceIndex, deviceAddress, regAddress, phycialAdress, mdiomode);
                        strcoef = u16coef.ToString();
                        break;
                    case 3:
                        UInt32 u32coef = bytetou32(deviceIndex, deviceAddress, regAddress, phycialAdress, mdiomode);
                        strcoef = u32coef.ToString();
                        break;
                    default:
                        strcoef = "";
                        break;
                }

                return strcoef;
            }
            catch (Exception ex)
            {

                Log.SaveLogToTxt(ex.Message);
                return strcoef;
            }
        }
        public static string ReadALLCoef(int deviceIndex, int deviceAddress, int regAddress, byte format, int phycialAdress = 0, int mdiomode = 0)
        {//1 ieee754;2 UInt16;3 UInt32
            string strcoef = "";
            try
            {
                switch (format)
                {
                    case 1:
                         byte[] bcoef = new byte[4];
                         string[] bcoefstr=new string[4];
                         UInt16[] bcoefmdio = new UInt16[4];
                         if (mdiomode == 1)
                         {
                             bcoefmdio = IOPort.ReadMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, 4);
                             for (int i = 0; i < 4; i++)
                             {
                                 if (bcoefmdio[i] < 16)
                                 {
                                     bcoefstr[i] = "000" + Convert.ToString(bcoefmdio[i], 16);
                                 }
                                 else
                                 {
                                     bcoefstr[i] ="00"+ Convert.ToString(bcoefmdio[i], 16);
                                 }
                             }
                         }
                         else
                         {
                             bcoef = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 4);
                             for (int i = 0; i < 4; i++)
                             {
                                 if (bcoef[i] < 16)
                                 {
                                     bcoefstr[i] = "0" + Convert.ToString(bcoef[i], 16);
                                 }
                                 else
                                 {
                                     bcoefstr[i] = Convert.ToString(bcoef[i], 16);
                                 }


                             }
                         }
                         strcoef = bcoefstr[0].ToString() + bcoefstr[1].ToString() + bcoefstr[2].ToString() + bcoefstr[3].ToString();
                        break;
                    case 2:
                        byte[] rcoef = new byte[2];
                        string[] rcoefstr = new string[2];
                        rcoef = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 2);
                        for (int i = 0; i < 2; i++)
                         {
                             if (rcoef[i] < 16)
                             {
                                 rcoefstr[i] = "0" + Convert.ToString(rcoef[i], 16);
                             }
                             else
                             {
                                 rcoefstr[i] = Convert.ToString(rcoef[i], 16);
                             }

                         
                         }
                        strcoef = rcoefstr[0].ToString() + rcoefstr[1].ToString();
                        break;
                    case 3:
                         byte[] bcoef1 = new byte[4];
                         string[] bcoefstr1=new string[4];
                         bcoef1 = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 4);
                         for (int i = 0; i < 4; i++)
                         {
                             if (bcoef1[i]<16)
                             {
                                 bcoefstr1[i] = "0" + Convert.ToString(bcoef1[i], 16);
                             }
                             else
                             {
                                 bcoefstr1[i] = Convert.ToString(bcoef1[i], 16);
                             }

                         
                         }
                         strcoef = bcoefstr1[0].ToString() + bcoefstr1[1].ToString() + bcoefstr1[2].ToString() + bcoefstr1[3].ToString();
                        break;
                    default:
                        strcoef = "";
                        break;
                }
                return strcoef;
            }
            catch (Exception ex)
            {

                Log.SaveLogToTxt(ex.Message);
                return strcoef;
            }
        
        }
        public static string ReadALLEEprom(int deviceIndex, int deviceAddress, int regAddress, int phycialAdress = 0, int mdiomode = 0)
        {//1 ieee754;2 UInt16;3 UInt32
            string strcoef = "";
            try
            {
                
                        byte[] bcoef = new byte[1];
                        string[] bcoefstr = new string[1];
                        UInt16[] bcoefmdio = new UInt16[1];
                        if (mdiomode == 1)
                        {
                            bcoefmdio = IOPort.ReadMDIO(deviceIndex, deviceAddress, phycialAdress, regAddress, IOPort.MDIOSoftHard.SOFTWARE, 1);
                            if (bcoefmdio[0] < 16)
                            {
                                bcoefstr[0] = "000" + Convert.ToString(bcoefmdio[0], 16);
                            }
                            else
                            {
                                bcoefstr[0] = "00" + Convert.ToString(bcoefmdio[0], 16);
                            }
                        }
                        else
                        {
                            bcoef = IOPort.ReadReg(deviceIndex, deviceAddress, regAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);

                            if (bcoef[0] < 16)
                            {
                                bcoefstr[0] = "0" + Convert.ToString(bcoef[0], 16);
                            }
                            else
                            {
                                bcoefstr[0] = Convert.ToString(bcoef[0], 16);
                            }

                        }
                        strcoef = bcoefstr[0].ToString();
                       
                return strcoef;
            }
            catch (Exception ex)
            {

                Log.SaveLogToTxt(ex.Message);
                return strcoef;
            }

        }

        public static byte[] ReadWriteDriverQSFP(int deviceIndex, int deviceAddress, int StartAddress, int regAddress, byte channel, byte operation, byte chipset, byte[] buffer,bool Switch)
        {
            //database 0: LDD 1: AMP 2: DAC 3: CDR

            ////chipset 1tx,2rx,4dac
            ////operation 1 READ,2 WRITE,4 store
            byte chipoperation;
            byte[] tempData=new byte[0];
            byte[] tempData1 = new byte[1];
            byte[] arrRead = new byte[buffer.Length];
            int datastartadd = 0;
            try
            {
            if (Switch == false)
            {
                datastartadd = 5;
                tempData = new byte[buffer.Length + datastartadd-1];
                chipoperation = (byte)((chipset << 4) + operation);
                tempData1[0] = chipoperation;
                tempData[0] = (byte)(channel - 1);
                tempData[1] = (byte)((regAddress >> 8) & 0xff);
                tempData[2] = (byte)(regAddress & 0xff);
                tempData[3] = (byte)(buffer.Length);
            }
            else 
            {
                datastartadd = 4;
                tempData = new byte[buffer.Length + datastartadd - 1];
                chipoperation = (byte)((chipset << 4) + operation);
                tempData1[0] = chipoperation;
                tempData[0] = (byte)(channel - 1);
                tempData[1] = (byte)(regAddress & 0xff);
                tempData[2] = (byte)(buffer.Length);
            }
            buffer.CopyTo(tempData, datastartadd-1);
            IOPort.WrtieReg(deviceIndex, deviceAddress, StartAddress+1, IOPort.SoftHard.HARDWARE_SEQUENT, tempData);
            
            System.Threading. Thread.Sleep(50);

            IOPort.WrtieReg(deviceIndex, deviceAddress, StartAddress, IOPort.SoftHard.HARDWARE_SEQUENT, tempData1);
            for (int i = 0; i < 10; i++)
            {
                System.Threading.Thread.Sleep(50);
                byte[] temp = IOPort.ReadReg(deviceIndex, deviceAddress, StartAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if (temp[0] == 0)
                {
                    if (operation == 1)
                    { arrRead = IOPort.ReadReg(deviceIndex, deviceAddress, StartAddress + datastartadd, IOPort.SoftHard.HARDWARE_SEQUENT, buffer.Length); }
                    break;
                }
            }
            //System.Threading.Thread.Sleep(200);
            return arrRead;
            }
            catch (Exception ex)
            {

                Log.SaveLogToTxt(ex.Message);
                return arrRead;
            }

        }
        public static byte[] ReadWriteDriverSFP(int deviceIndex, int deviceAddress, int StartAddress, int regAddress, byte operation, byte chipset, byte[] buffer)
        {
            //database 0: LDD 1: AMP 2: DAC 3: CDR

            ////chipset 1tx,2rx,4dac,3,CDR
            ////operation 1 READ,2 WRITE,4 store
            byte chipoperation;
            byte[] arrRead = new byte[buffer.Length];
            byte[] tempread = new byte[buffer.Length];
            int RegistWide = 1;

            if (chipset == 4)
            {
                RegistWide = 2;
            }


            try
            {
                for (int i = 0; i < buffer.Length / RegistWide; i++)
                {
                    chipoperation = (byte)((chipset << 4) + operation);
                    byte[] tempData = new byte[4];
                    byte[] tempData1 = new byte[1];
                    tempData[0] = 0;
                    tempData[1] = buffer[i];
                    tempData[2] = (byte)((regAddress + i >> 8) & 0xff);
                    tempData[3] = (byte)(regAddress + i & 0xff);
                    tempData1[0] = chipoperation;
                    IOPort.WrtieReg(deviceIndex, deviceAddress, StartAddress, IOPort.SoftHard.HARDWARE_SEQUENT, tempData);
                    IOPort.WrtieReg(deviceIndex, deviceAddress, StartAddress + 8, IOPort.SoftHard.HARDWARE_SEQUENT, tempData1);
                    for (int j = 0; j < 10; j++)
                    {
                        System.Threading.Thread.Sleep(50);
                        byte[] temp = IOPort.ReadReg(deviceIndex, deviceAddress, StartAddress + 8, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                        if (temp[0] == 0)
                        {
                            if (operation == 1)
                            {
                                tempread = IOPort.ReadReg(deviceIndex, deviceAddress, StartAddress, IOPort.SoftHard.HARDWARE_SEQUENT, 2);
                                if (RegistWide == 1)
                                {

                                    arrRead[i] = tempread[1];
                                }
                                else
                                {
                                    arrRead[i*RegistWide] = tempread[0];
                                    arrRead[i * RegistWide+1] = tempread[1];

                                }
                          
                            }
                            break;
                        }
                    }
                }
               
                return arrRead;
            }
            catch (Exception ex)
            {

                Log.SaveLogToTxt(ex.Message);
                return arrRead;
            }
        }
       
        public static UInt16 ReadWriteDriverCFP(int deviceIndex, int deviceAddress, int StartAddress, int regAddress, byte channel, byte operation, byte chipset, UInt16 buffer,int phycialAdress=0)
        {
            //database 0: LDD 1: AMP 2: DAC 3: CDR
            ////chipset 1ldd,2amp,3cdr,0dac
            ////operation 1 READ,2 WRITE,4 store
            byte chipoperation;
            UInt16 returndata = 0;
            try
            {
                chipoperation = (byte)((chipset << 4) + operation);
                byte[] buff = new byte[2];
                buff[0] = (byte)(buffer / 256);
                buff[1] = (byte)(buffer & 0xFF);
                UInt16[] tempData = new UInt16[6];
                tempData[2] = (UInt16)(buff[0]);
                tempData[3] = (UInt16)(buff[1]);
                tempData[0] = (UInt16)((regAddress >> 8) & 0xff);
                tempData[1] = (UInt16)(regAddress & 0xff);
                tempData[4] = (UInt16)(channel);
                tempData[5] = (UInt16)(chipoperation);
                IOPort.WriteMDIO(deviceIndex, deviceAddress, phycialAdress, StartAddress, IOPort.MDIOSoftHard.SOFTWARE, tempData);
                for (int i = 0; i < 10; i++)
                {
                    System.Threading.Thread.Sleep(50);
                    UInt16[] temp = IOPort.ReadMDIO(deviceIndex, deviceAddress, phycialAdress, StartAddress + 5, IOPort.MDIOSoftHard.SOFTWARE, 1);
                    if (temp[0] == 0)
                    {
                        if (operation == 1)
                        {
                            UInt16[] arrRead = IOPort.ReadMDIO(deviceIndex, deviceAddress, phycialAdress, StartAddress, IOPort.MDIOSoftHard.SOFTWARE, 6);
                            System.Threading.Thread.Sleep(200);
                            buff[0] = (byte)(arrRead[2]);
                            buff[1] = (byte)(arrRead[3]);
                            returndata = (UInt16)(buff[0] * 256 + buff[1]);
                        }
                        break;
                    }
                }
                return returndata;
            }
            catch (Exception ex)
            {

                Log.SaveLogToTxt(ex.Message);
                return returndata;
            }
        }
        public static byte[] ReadWriteDriverXFP(int deviceIndex, int deviceAddress, int StartAddress, int regAddress, byte operation, byte[] buffer)
        {
            //operation 80 write ldd,40 read ldd,20 store ldd
            //08 store dac,04 read dac,02 write dac
            byte[] tempData = new byte[buffer.Length + 4];
            byte[] arrRead = new byte[buffer.Length];
            try
            {
                tempData[0] = buffer[0];
                tempData[1] = buffer[1];
                tempData[2] = (byte)regAddress;
                tempData[3] = (operation);
                buffer.CopyTo(tempData, 4);
                IOPort.WrtieReg(deviceIndex, deviceAddress, StartAddress, IOPort.SoftHard.HARDWARE_SEQUENT, tempData);
                arrRead = IOPort.ReadReg(deviceIndex, deviceAddress, StartAddress, IOPort.SoftHard.HARDWARE_SEQUENT, buffer.Length);
                System.Threading.Thread.Sleep(200);
                return arrRead;
            }
            catch (Exception ex)
            {

                Log.SaveLogToTxt(ex.Message);
                return arrRead;
            }


        }
    }
}
