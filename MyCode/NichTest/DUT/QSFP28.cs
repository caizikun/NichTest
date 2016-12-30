using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data;

namespace NichTest
{
    public class QSFP28 : DUT
    {
        private static object syncRoot = new Object();//used for thread synchronization

        private static byte[] WriteDriver40g(int deviceIndex, int deviceAddress, int StartAddress, int regAddress, byte channel, byte chipset, byte[] dataToWrite, bool Switch)
        {
            return EEPROM.ReadWriteDriverQSFP(deviceIndex, deviceAddress, StartAddress, regAddress, channel, 0x02, chipset, dataToWrite, Switch);
        }

        private static byte[] ReadDriver40g(int deviceIndex, int deviceAddress, int StartAddress, int regAddress, byte channel, byte chipset, int readLength, bool Switch)
        {
            return EEPROM.ReadWriteDriverQSFP(deviceIndex, deviceAddress, StartAddress, regAddress, channel, 0x01, chipset, new byte[readLength], Switch);
        }

        private static byte[] StoreDriver40g(int deviceIndex, int deviceAddress, int StartAddress, int regAddress, byte channel, byte chipset, byte[] dataToWrite, bool Switch)
        {
            return EEPROM.ReadWriteDriverQSFP(deviceIndex, deviceAddress, StartAddress, regAddress, channel, 0x06, chipset, dataToWrite, Switch);
        }
        
        public override bool Initial(ChipControlByPN tableA, ChipDefaultValueByPN tableB, EEPROMDefaultValueByTestPlan tableC, DUTCoeffControlByPN tableD)
        {
            lock (syncRoot)
            {
                try
                {
                    this.dataTable_ChipControlByPN = tableA;
                    this.dataTable_ChipDefaultValueByPN = tableB;
                    this.dataTable_EEPROMDefaultValueByTestPlan = tableC;
                    this.dataTable_DUTCoeffControlByPN = tableD;

                    //QSFP = new EEPROM(TestPlanParaByPN.DUT_USB_Port);
                    //USBIO = new IOPort("USB", TestPlanParaByPN.DUT_USB_Port.ToString());
                    //IOPort.IOConnect();
                    //USBIO = IOPort.GetIOPort();

                    string filter = "ItemName = " + "'" + "DEBUGINTERFACE" + "'";
                    DataRow[] foundRows = this.dataTable_DUTCoeffControlByPN.Select(filter);

                    if (foundRows.Length == 0)
                    {
                        Log.SaveLogToTxt("there is no DEBUGINTERFACE");
                    }
                    else if (foundRows.Length > 1)
                    {
                        Log.SaveLogToTxt("count of DEBUGINTERFACE is more than 1");
                    }
                    else
                    {
                        DebugInterface.EngPage = Convert.ToByte(foundRows[0]["Page"]);
                        DebugInterface.StartAddress = Convert.ToInt32(foundRows[0]["StartAddress"]);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private bool CDR_Enable()
        {
            lock (syncRoot)
            {
                byte[] dataToWrite = { 0xFF };
                byte[] dataReadArray;
                for (int i = 0; i < 3; i++)
                {
                    IOPort.WrtieReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 98, IOPort.SoftHard.HARDWARE_SEQUENT, dataToWrite);
                    Thread.Sleep(100);
                    dataReadArray = IOPort.ReadReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 98, IOPort.SoftHard.HARDWARE_SEQUENT, 1);

                    if (dataReadArray[0] == 0xff)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private bool HightPowerClass_Enable()
        {
            lock (syncRoot)
            {
                byte[] dataToWrite = { 0x04 };
                byte[] dataReadArray;
                for (int i = 0; i < 3; i++)
                {
                    IOPort.WrtieReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 93, IOPort.SoftHard.HARDWARE_SEQUENT, dataToWrite);
                    Thread.Sleep(100);
                    dataReadArray = IOPort.ReadReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 93, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                    if (dataReadArray[0] == 0x04)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public override bool FullFunctionEnable()
        {
            lock (syncRoot)
            {
                int i = 0;
                for (i = 0; i < 3; i++)
                {
                    if (CDR_Enable() && HightPowerClass_Enable() && TxAllChannelEnable())
                    {
                        return true;
                    }
                }
                if (i == 3)
                {
                    throw new Exception(" CDR ByPass Error");
                }
                return true;
            }
        }

        public override string ReadSN()
        {
            lock (syncRoot)
            {
                string SN = "";
                EnterEngMode(0);
                SN = EEPROM.ReadSn(TestPlanParaByPN.DUT_USB_Port, 0xA0, 196);
                return SN.Trim();
            }
        }

        private static void EnterEngMode(byte page)
        {
            byte[] buff = new byte[5];
            buff[0] = 0xca;
            buff[1] = 0x2d;
            buff[2] = 0x81;
            buff[3] = 0x5f;
            buff[4] = page;
            IOPort.WrtieReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 123, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
        }

        public override string ReadFW()
        {
            lock (syncRoot)
            {
                string fwrev = "";
                EnterEngMode(4);
                fwrev = EEPROM.ReadFWRev(TestPlanParaByPN.DUT_USB_Port, 0xA0, 128);
                return fwrev.ToUpper();
            }
        }

        public override bool InitialChip(DUTCoeffControlByPN coeffControl, ChipDefaultValueByPN chipDefaultValue)
        {
            lock (syncRoot)
            {
                byte engpage = 0;
                int startaddr = 0;
                string filter = "ItemName = " + "'" + "DEBUGINTERFACE" + "'";
                DataRow[] foundRows = coeffControl.Select(filter);

                if (foundRows == null)
                {
                    Log.SaveLogToTxt("There is no debug interface.");
                }
                else if (foundRows.Length > 1)
                {
                    Log.SaveLogToTxt("Count of debug interface is more than 1");
                }
                else
                {
                    engpage = Convert.ToByte(foundRows[0]["Page"]);
                    startaddr = Convert.ToInt32(foundRows[0]["StartAddress"]);
                }

                if (chipDefaultValue == null)
                {
                    return true;
                }

                for (int row = 0; row < chipDefaultValue.Rows.Count; row++)
                {
                    if (Convert.ToInt32(chipDefaultValue.Rows[row]["Length"]) == 0)
                    {
                        continue;
                    }
                    byte length = Convert.ToByte(chipDefaultValue.Rows[row]["Length"]);
                    bool isLittleendian = Convert.ToBoolean(chipDefaultValue.Rows[row]["Endianness"]);
                    var inputdata = chipDefaultValue.Rows[row]["ItemValue"];
                    byte[] writeData = Algorithm.ObjectToByteArray(inputdata, length, isLittleendian);
                    byte driveTpye = Convert.ToByte(chipDefaultValue.Rows[row]["DriverType"]);
                    byte chipset = 0x01;
                    switch (driveTpye)
                    {
                        case 0:
                            chipset = 0x01;
                            break;
                        case 1:
                            chipset = 0x02;
                            break;
                        case 2:
                            chipset = 0x04;
                            break;
                        case 3:
                            chipset = 0x08;
                            break;
                        default:
                            chipset = 0x01;
                            break;

                    }
                    EnterEngMode(engpage);

                    int registerAddress = Convert.ToInt32(chipDefaultValue.Rows[row]["RegisterAddress"]);
                    byte chipLine = Convert.ToByte(chipDefaultValue.Rows[row]["ChipLine"]);
                    int k = 0;
                    for (k = 0; k < 3; k++)
                    {
                        WriteDriver40g(TestPlanParaByPN.DUT_USB_Port, 0xA0, startaddr, registerAddress, chipLine, chipset, writeData, GlobalParaByPN.isOldDriver);
                        // Thread.Sleep(200);  
                        StoreDriver40g(TestPlanParaByPN.DUT_USB_Port, 0xA0, startaddr, registerAddress, chipLine, chipset, writeData, GlobalParaByPN.isOldDriver);
                        // Thread.Sleep(200);  
                        byte[] temp = new byte[length];
                        temp = ReadDriver40g(TestPlanParaByPN.DUT_USB_Port, 0xA0, startaddr, registerAddress, chipLine, chipset, length, GlobalParaByPN.isOldDriver);

                        if (BitConverter.ToString(temp) == BitConverter.ToString(writeData))
                        {
                            break;
                        }
                    }

                    if (k >= 3)
                    {
                        return false;
                    }
                }
                return false;
            }         
        }

        public override bool SetSoftTxDis(int channel)
        {
            lock (syncRoot)
            {
                byte[] buff = new byte[1];
                try
                {
                    switch (channel)
                    {
                        case 1:
                            buff = IOPort.ReadReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 86, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                            buff[0] = (byte)(buff[0] | 0x01);
                            IOPort.WrtieReg(0, 0xA0, 86, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                            break;
                        case 2:
                            buff = IOPort.ReadReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 86, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                            buff[0] = (byte)(buff[0] | 0x02);
                            IOPort.WrtieReg(0, 0xA0, 86, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                            break;
                        case 3:
                            buff = IOPort.ReadReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 86, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                            buff[0] = (byte)(buff[0] | 0x04);
                            IOPort.WrtieReg(0, 0xA0, 86, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                            break;
                        case 4:
                            buff = IOPort.ReadReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 86, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                            buff[0] = (byte)(buff[0] | 0x08);
                            IOPort.WrtieReg(0, 0xA0, 86, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                            break;
                        default:
                            break;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        public override bool SetSoftTxDis()
        {
            lock (syncRoot)
            {
                for (int i = 0; i < GlobalParaByPN.TotalChCount; i++)
                {
                    if (!this.SetSoftTxDis(i + 1))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public override bool TxAllChannelEnable()
        {
            lock (syncRoot)
            {
                byte[] dataToWrite = { 0x00 };
                byte[] dataReadArray;
                for (int i = 0; i < 3; i++)
                {
                    IOPort.WrtieReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 86, IOPort.SoftHard.HARDWARE_SEQUENT, dataToWrite);
                    dataReadArray = IOPort.ReadReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 86, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                    if (dataReadArray[0] == 0x00)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public override double ReadDmiTemp()
        {
            lock (syncRoot)
            {
                return EEPROM.readdmitemp(TestPlanParaByPN.DUT_USB_Port, 0xA0, 22);
            }
        }

        public override double ReadDmiVcc()
        {
            lock (syncRoot)
            {
                return EEPROM.readdmivcc(TestPlanParaByPN.DUT_USB_Port, 0xA0, 26);
            }
        }

        public override double ReadDmiBias(int channel)
        {
            lock (syncRoot)
            {
                try
                {
                    double dmibias = 0.0;
                    switch (channel)
                    {
                        case 1:
                            dmibias = EEPROM.readdmibias(TestPlanParaByPN.DUT_USB_Port, 0xA0, 42);
                            break;
                        case 2:
                            dmibias = EEPROM.readdmibias(TestPlanParaByPN.DUT_USB_Port, 0xA0, 44);
                            break;
                        case 3:
                            dmibias = EEPROM.readdmibias(TestPlanParaByPN.DUT_USB_Port, 0xA0, 46);
                            break;
                        case 4:
                            dmibias = EEPROM.readdmibias(TestPlanParaByPN.DUT_USB_Port, 0xA0, 48);
                            break;
                        default:
                            break;
                    }
                    return dmibias;
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return Algorithm.MyNaN;
                }
            }
        }

        public override double ReadDmiTxP(int channel)
        {
            lock (syncRoot)
            {
                try
                {
                    double dmitxp = 0.0;
                    switch (channel)
                    {
                        case 1:
                            dmitxp = EEPROM.readdmitxp(TestPlanParaByPN.DUT_USB_Port, 0xA0, 50);
                            break;
                        case 2:
                            dmitxp = EEPROM.readdmitxp(TestPlanParaByPN.DUT_USB_Port, 0xA0, 52);
                            break;
                        case 3:
                            dmitxp = EEPROM.readdmitxp(TestPlanParaByPN.DUT_USB_Port, 0xA0, 54);
                            break;
                        case 4:
                            dmitxp = EEPROM.readdmitxp(TestPlanParaByPN.DUT_USB_Port, 0xA0, 56);
                            break;
                        default:
                            break;
                    }
                    return dmitxp;
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return Algorithm.MyNaN;
                }
            }
        }

        public override double ReadDmiRxP(int channel)
        {
            lock (syncRoot)
            {
                try
                {
                    double dmirxp = 0.0;
                    switch (channel)
                    {
                        case 1:
                            dmirxp = EEPROM.readdmirxp(TestPlanParaByPN.DUT_USB_Port, 0xA0, 34);
                            break;
                        case 2:
                            dmirxp = EEPROM.readdmirxp(TestPlanParaByPN.DUT_USB_Port, 0xA0, 36);
                            break;
                        case 3:
                            dmirxp = EEPROM.readdmirxp(TestPlanParaByPN.DUT_USB_Port, 0xA0, 38);
                            break;
                        case 4:
                            dmirxp = EEPROM.readdmirxp(TestPlanParaByPN.DUT_USB_Port, 0xA0, 40);
                            break;
                        default:
                            break;
                    }
                    return dmirxp;
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return Algorithm.MyNaN;
                }
            }
        }

        public override ushort ReadADC(NameOfADC enumName, int channel)
        {
            lock (syncRoot)
            {
                try
                {
                    string name = enumName.ToString();
                    DUTCoeffControlByPN.CoeffInfo coeffInfo = dataTable_DUTCoeffControlByPN.GetOneInfoFromTable(name, channel);

                    EnterEngMode(coeffInfo.Page);
                    UInt16 valueADC = EEPROM.readadc(TestPlanParaByPN.DUT_USB_Port, 0xA0, coeffInfo.StartAddress);
                    Log.SaveLogToTxt("Current TXPOWERADC is " + valueADC);
                    return valueADC;

                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return Algorithm.MyNaN;
                }
            }
        }        

        public override bool WriteChipDAC(NameOfChipDAC enumName, int channel, object writeDAC)
        {
            lock (syncRoot)
            {
                string filter = "ItemName = " + "'" + enumName.ToString() + "'";
                DataRow[] foundRows = this.dataTable_ChipControlByPN.Select(filter);
                for (int row = 0; row < foundRows.Length; row++)
                {
                    if (Convert.ToInt32(foundRows[row]["ModuleLine"]) == channel)
                    {
                        byte chipLine = Convert.ToByte(foundRows[row]["ChipLine"]);
                        byte driveType = Convert.ToByte(foundRows[row]["DriveType"]);
                        int regAddress = Convert.ToInt32(foundRows[row]["RegisterAddress"]);
                        byte length = Convert.ToByte(foundRows[row]["Length"]);
                        bool endianness = Convert.ToBoolean(foundRows[row]["Endianness"]);
                        byte startBit = Convert.ToByte(foundRows[row]["StartBit"]);
                        byte endBit = Convert.ToByte(foundRows[row]["EndBit"]);
                        byte chipset = 0x01;
                        switch (driveType)
                        {
                            case 0:
                                chipset = 0x01;
                                break;
                            case 1:
                                chipset = 0x02;
                                break;
                            case 2:
                                chipset = 0x04;
                                break;
                            case 3:
                                chipset = 0x08;
                                break;
                            default:
                                chipset = 0x01;
                                break;
                        }
                        bool isFull = Algorithm.BitNeedManage(length, startBit, endBit);
                        if (!isFull)//寄存器全位,不需要做任何处理
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                if (!(bool)Write_Store_Read_ChipReg(writeDAC, length, endianness, regAddress, chipLine, chipset, ChipOperation.Write))
                                {
                                    return false;//写DAC值
                                }
                                int readDAC = (int)Write_Store_Read_ChipReg(writeDAC, length, endianness, regAddress, chipLine, chipset, ChipOperation.Read);
                                if (readDAC == Convert.ToInt16(writeDAC))
                                {
                                    return true;
                                }
                            }
                        }
                        else//寄存器位缺,需要做任何处理
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                int readDAC = (int)Write_Store_Read_ChipReg(writeDAC, length, endianness, regAddress, chipLine, chipset, ChipOperation.Read);
                                int joinValue = Algorithm.WriteJointBitValue((int)writeDAC, readDAC, length, startBit, endBit);
                                if (!(bool)Write_Store_Read_ChipReg(joinValue, length, endianness, regAddress, chipLine, chipset, ChipOperation.Write))
                                {
                                    return false;//写入寄存器的全位DAC值
                                }
                                readDAC = (int)Write_Store_Read_ChipReg(writeDAC, length, endianness, regAddress, chipLine, chipset, ChipOperation.Read);
                                int readJoinValue = Algorithm.ReadJointBitValue(readDAC, length, startBit, endBit);
                                if (readJoinValue == Convert.ToInt16(writeDAC))
                                {
                                    return true;
                                }
                            }
                        }
                        Log.SaveLogToTxt("Writer " + enumName.ToString() + " failed");
                        return false;
                    }
                }
                return false;
            }
        }

        public override bool ReadChipDAC(NameOfChipDAC enumName, int channel, out int readDAC)
        {
            lock (syncRoot)
            {
                readDAC = 0;
                string filter = "ItemName = " + "'" + enumName.ToString() + "'";
                DataRow[] foundRows = this.dataTable_ChipControlByPN.Select(filter);
                for (int row = 0; row < foundRows.Length; row++)
                {
                    if (Convert.ToInt32(foundRows[row]["ModuleLine"]) == channel)
                    {
                        byte chipLine = Convert.ToByte(foundRows[row]["ChipLine"]);
                        byte driveType = Convert.ToByte(foundRows[row]["DriveType"]);
                        int regAddress = Convert.ToInt32(foundRows[row]["RegisterAddress"]);
                        byte length = Convert.ToByte(foundRows[row]["Length"]);
                        bool endianness = Convert.ToBoolean(foundRows[row]["Endianness"]);
                        byte startBit = Convert.ToByte(foundRows[row]["StartBit"]);
                        byte endBit = Convert.ToByte(foundRows[row]["EndBit"]);
                        byte chipset = 0x01;
                        switch (driveType)
                        {
                            case 0:
                                chipset = 0x01;
                                break;
                            case 1:
                                chipset = 0x02;
                                break;
                            case 2:
                                chipset = 0x04;
                                break;
                            case 3:
                                chipset = 0x08;
                                break;
                            default:
                                chipset = 0x01;
                                break;
                        }
                        readDAC = (int)Write_Store_Read_ChipReg(null, length, endianness, regAddress, chipLine, chipset, ChipOperation.Read);
                        Log.SaveLogToTxt("Read " + enumName.ToString() + " failed");
                        return true;
                    }
                }
                return false;
            }
        }

        private object Write_Store_Read_ChipReg(object writeData, byte length, bool isLittleendian, int regAddress,  byte chipLine, byte chipset, ChipOperation operate)
        {
            lock (syncRoot)
            {
                byte[] writeDataArray = Algorithm.ObjectToByteArray(writeData, length, isLittleendian);
                EnterEngMode(DebugInterface.EngPage);

                if ((int)operate == 0)//写
                {
                    WriteDriver40g(TestPlanParaByPN.DUT_USB_Port, 0xA0, DebugInterface.StartAddress, regAddress, chipLine, chipset, writeDataArray, GlobalParaByPN.isOldDriver);
                }
                else if ((int)operate == 1)//存
                {
                    StoreDriver40g(TestPlanParaByPN.DUT_USB_Port, 0xA0, DebugInterface.StartAddress, regAddress, chipLine, chipset, writeDataArray, GlobalParaByPN.isOldDriver);
                }
                else if ((int)operate == 2)
                {
                    byte[] readData = ReadDriver40g(TestPlanParaByPN.DUT_USB_Port, 0xA0, DebugInterface.StartAddress, regAddress, chipLine, chipset, length, GlobalParaByPN.isOldDriver);

                    int returnData = 0;
                    for (int i = readData.Length; i > 0; i--)
                    {
                        returnData += Convert.ToUInt16(readData[i - 1] * Math.Pow(256, length - i));
                    }
                    return returnData;
                }
                else
                {
                    return false;
                }
                return true;
            }
        }

        public override double CalRxRes(double inputPower_dBm, int channel, double ratio, double U_Ref, double resolution, double R_rssi)
        {
            lock (syncRoot)
            {
                ushort RxPowerADC = this.ReadADC(NameOfADC.RXPOWERADC, channel);
                double k = ratio * U_Ref / (Math.Pow(2, resolution) - 1);
                double U_rssi = RxPowerADC * k;
                double I_rssi = U_rssi / R_rssi;
                double responsivity = I_rssi / Math.Pow(10, inputPower_dBm * 0.1);

                return responsivity;
            }
        }

        public override bool CloseAndOpenAPC(byte mode)
        {
            lock (syncRoot)
            {
                bool isOK = false;
                if (GlobalParaByPN.APCType == Convert.ToByte(apctype.none))
                {
                    //logoStr += logger.AdapterLogString(0, "no apc");
                    return true;
                }
                try
                {
                    switch (mode)
                    {
                        case (byte)APCMODE.IBAISandIMODON:
                            {
                                //logoStr += logger.AdapterLogString(0, "Open apc");
                                isOK = this.APCON(0x11);
                                // isOK = dut.APCON(0x00);
                                //logoStr += logger.AdapterLogString(0, "Open apc" + isOK.ToString());
                                break;
                            }
                        case (byte)APCMODE.IBAISandIMODOFF:
                            {
                                //logoStr += logger.AdapterLogString(0, " Close apc");
                                isOK = this.APCOFF(0x11);
                                //logoStr += logger.AdapterLogString(0, "Close apc" + isOK.ToString());
                            }
                            break;
                        case (byte)APCMODE.IBIASONandIMODOFF:
                            {
                                //logoStr += logger.AdapterLogString(0, " Close IModAPCand Open IBiasAPC");
                                isOK = this.APCON(0x01);
                                //logoStr += logger.AdapterLogString(0, "Close IModAPCand Open IBiasAPC" + isOK.ToString());
                                break;
                            }
                        case (byte)APCMODE.IBIASOFFandIMODON:
                            {
                                //logoStr += logger.AdapterLogString(0, " Close IBiasAPCand Open IModAPC");
                                isOK = this.APCON(0x10);
                                //logoStr += logger.AdapterLogString(0, "Close IBiasAPCand Open IModAPC" + isOK.ToString());
                                break;
                            }
                        default:
                            {
                                break;
                            }

                    }
                    return isOK;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private bool APCON(byte apcswitch)
        {
            lock (syncRoot)
            {
                byte[] buff = new byte[1];
                buff[0] = 0x00;
                if ((apcswitch & 0x01) == 0x01)
                {
                    buff[0] |= 0x05;
                }
                if ((apcswitch & 0x10) == 0x10)
                {
                    buff[0] |= 0x50;
                }

                string filter = "ItemName = 'APCCONTROLL'";
                DataRow[] foundRows = this.dataTable_DUTCoeffControlByPN.Select(filter);

                byte page = Convert.ToByte(foundRows[0]["Page"]);
                int startAddress = Convert.ToInt32(foundRows[0]["StartAddress"]);
                EnterEngMode(page);
                IOPort.WrtieReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, startAddress, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                return true;
            }
        }

        private bool APCOFF(byte apcswitch)
        {
            lock (syncRoot)
            {
                byte[] buff = new byte[1];
                buff[0] = 0xff;
                if ((apcswitch & 0x01) == 0x01)
                {
                    buff[0] &= 0xf0;

                }
                if ((apcswitch & 0x10) == 0x10)
                {
                    buff[0] &= 0x0f;

                }

                string filter = "ItemName = 'APCCONTROLL'";
                DataRow[] foundRows = this.dataTable_DUTCoeffControlByPN.Select(filter);

                byte page = Convert.ToByte(foundRows[0]["Page"]);
                int startAddress = Convert.ToInt32(foundRows[0]["StartAddress"]);
                EnterEngMode(page);
                IOPort.WrtieReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, startAddress, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                return true;
            }
        }

        public override bool SetCoeff(Coeff coeff, int channel, string value)
        {
            lock (syncRoot)
            {
                try
                {
                    string coeffName = coeff.ToString();
                    DUTCoeffControlByPN.CoeffInfo coeffInfo = dataTable_DUTCoeffControlByPN.GetOneInfoFromTable(coeffName, channel);

                    EnterEngMode(coeffInfo.Page);
                    bool result = EEPROM.SetCoef(TestPlanParaByPN.DUT_USB_Port, 0xA0, coeffInfo.StartAddress, value, coeffInfo.Format);

                    Log.SaveLogToTxt("Set " + coeffName + " to " + value);
                    return result;

                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        public override string GetCoeff(Coeff coeff, int channel)
        {
            lock (syncRoot)
            {
                string coeffName = coeff.ToString();
                try
                {
                    DUTCoeffControlByPN.CoeffInfo coeffInfo = dataTable_DUTCoeffControlByPN.GetOneInfoFromTable(coeffName, channel);

                    EnterEngMode(coeffInfo.Page);
                    string value = EEPROM.ReadCoef(TestPlanParaByPN.DUT_USB_Port, 0xA0, coeffInfo.StartAddress, coeffInfo.Format);

                    Log.SaveLogToTxt("Get " + coeffName + " is " + value);
                    return value;

                }
                catch
                {
                    Log.SaveLogToTxt("Failed to get value of " + coeffName);
                    return Algorithm.MyNaN.ToString();
                }
            }
        }

        public override bool ChkRxLos(int channel)
        {
            lock (syncRoot)
            {
                byte[] buff = new byte[1];
                try
                {
                    buff = IOPort.ReadReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 3, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                    switch (channel)
                    {
                        case 1:
                            return (buff[0] & 0x01) != 0;
                        case 2:
                            return (buff[0] & 0x02) != 0;
                        case 3:
                            return (buff[0] & 0x04) != 0;
                        case 4:
                            return (buff[0] & 0x08) != 0;
                        default:
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }
    }
}
