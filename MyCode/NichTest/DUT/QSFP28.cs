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
        EEPROM QSFP;

        public byte[] WriteDriver40g(int deviceIndex, int deviceAddress, int StartAddress, int regAddress, byte channel, byte chipset, byte[] dataToWrite, bool Switch)
        {
            return QSFP.ReadWriteDriverQSFP(deviceIndex, deviceAddress, StartAddress, regAddress, channel, 0x02, chipset, dataToWrite, Switch);
        }

        public byte[] ReadDriver40g(int deviceIndex, int deviceAddress, int StartAddress, int regAddress, byte channel, byte chipset, int readLength, bool Switch)
        {
            return QSFP.ReadWriteDriverQSFP(deviceIndex, deviceAddress, StartAddress, regAddress, channel, 0x01, chipset, new byte[readLength], Switch);
        }
        
        public byte[] StoreDriver40g(int deviceIndex, int deviceAddress, int StartAddress, int regAddress, byte channel, byte chipset, byte[] dataToWrite, bool Switch)
        {
            return QSFP.ReadWriteDriverQSFP(deviceIndex, deviceAddress, StartAddress, regAddress, channel, 0x06, chipset, dataToWrite, Switch);
        }
        
        public override bool Initial(ChipControlByPN tableA, ChipDefaultValueByPN tableB, EEPROMDefaultValueByTestPlan tableC, DUTCoeffControlByPN tableD)
        {
            try
            {
                this.dataTable_ChipControlByPN = tableA;
                this.dataTable_ChipDefaultValueByPN = tableB;
                this.dataTable_EEPROMDefaultValueByTestPlan = tableC;
                this.dataTable_DUTCoeffControlByPN = tableD;

                QSFP = new EEPROM(TestPlanParaByPN.DUT_USB_Port);
                USBIO = new IOPort("USB", TestPlanParaByPN.DUT_USB_Port.ToString());
                USBIO.IOConnect();

                string filter = "ItemName = " + "'" + "DEBUGINTERFACE" + "'";
                DataRow[] foundRows = this.dataTable_DUTCoeffControlByPN .Select(filter);

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

        private bool CDR_Enable()
        {
            byte[] dataToWrite = { 0xFF };
            byte[] dataReadArray;
            for (int i = 0; i < 3; i++)
            {
                USBIO.WrtieReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 98, IOPort.SoftHard.HARDWARE_SEQUENT, dataToWrite);
                Thread.Sleep(100);
                dataReadArray = USBIO.ReadReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 98, IOPort.SoftHard.HARDWARE_SEQUENT, 1);

                if (dataReadArray[0] == 0xff)
                {
                    return true;
                }
            }
            return false;
        }

        private bool HightPowerClass_Enable()
        {
            byte[] dataToWrite = { 0x04 };
            byte[] dataReadArray;
            for (int i = 0; i < 3; i++)
            {
                USBIO.WrtieReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 93, IOPort.SoftHard.HARDWARE_SEQUENT, dataToWrite);
                Thread.Sleep(100);
                dataReadArray = USBIO.ReadReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 93, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if (dataReadArray[0] == 0x04)
                {
                    return true;
                }
            }
            return false;
        }

        public override bool FullFunctionEnable()
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

        public override string ReadSN()
        {
            string SN = "";
            this.EnterEngMode(0);
            SN = QSFP.ReadSn(TestPlanParaByPN.DUT_USB_Port, 0xA0, 196);
            return SN.Trim();
        }

        public void EnterEngMode(byte page)
        {
            byte[] buff = new byte[5];
            buff[0] = 0xca;
            buff[1] = 0x2d;
            buff[2] = 0x81;
            buff[3] = 0x5f;
            buff[4] = page;
            USBIO.WrtieReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 123, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
        }

        public override string ReadFW()
        {
            string fwrev = "";
            this.EnterEngMode(4);
            fwrev = QSFP.ReadFWRev(TestPlanParaByPN.DUT_USB_Port, 0xA0, 128);
            return fwrev.ToUpper();
        }

        public override bool InitialChip(DUTCoeffControlByPN coeffControl, ChipDefaultValueByPN chipDefaultValue)
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
                this.EnterEngMode(engpage);

                int registerAddress = Convert.ToInt32(chipDefaultValue.Rows[row]["RegisterAddress"]);
                byte chipLine = Convert.ToByte(chipDefaultValue.Rows[row]["ChipLine"]);
                int k = 0;
                for (k = 0; k < 3; k++)
                {
                    this.WriteDriver40g(TestPlanParaByPN.DUT_USB_Port, 0xA0, startaddr, registerAddress, chipLine, chipset, writeData, GlobalParaByPN.isOldDriver);
                    // Thread.Sleep(200);  
                    this.StoreDriver40g(TestPlanParaByPN.DUT_USB_Port, 0xA0, startaddr, registerAddress, chipLine, chipset, writeData, GlobalParaByPN.isOldDriver);
                    // Thread.Sleep(200);  
                    byte[] temp = new byte[length];
                    temp = this.ReadDriver40g(TestPlanParaByPN.DUT_USB_Port, 0xA0, startaddr, registerAddress, chipLine, chipset, length, GlobalParaByPN.isOldDriver);

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

        public override bool SetSoftTxDis(int channel)
        {
            byte[] buff = new byte[1];
            try
            {
                switch (channel)
                {
                    case 1:
                        buff = USBIO.ReadReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 86, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                        buff[0] = (byte)(buff[0] | 0x01);
                        USBIO.WrtieReg(0, 0xA0, 86, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                        break;
                    case 2:
                        buff = USBIO.ReadReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 86, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                        buff[0] = (byte)(buff[0] | 0x02);
                        USBIO.WrtieReg(0, 0xA0, 86, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                        break;
                    case 3:
                        buff = USBIO.ReadReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 86, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                        buff[0] = (byte)(buff[0] | 0x04);
                        USBIO.WrtieReg(0, 0xA0, 86, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
                        break;
                    case 4:
                        buff = USBIO.ReadReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 86, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                        buff[0] = (byte)(buff[0] | 0x08);
                        USBIO.WrtieReg(0, 0xA0, 86, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
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

        public override bool SetSoftTxDis()
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

        public override bool TxAllChannelEnable()
        {
            byte[] dataToWrite = { 0x00 };
            byte[] dataReadArray;
            for (int i = 0; i < 3; i++)
            {
                USBIO.WrtieReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 86, IOPort.SoftHard.HARDWARE_SEQUENT, dataToWrite);
                dataReadArray = USBIO.ReadReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, 86, IOPort.SoftHard.HARDWARE_SEQUENT, 1);
                if (dataReadArray[0] == 0x00)
                {
                    return true;
                }
            }
            return false;
        }

        public override double ReadDmiTxP(int channel)
        {
            double dmitxp =0.0;
            try
            {
                switch (channel)
                {
                    case 1:
                        dmitxp = QSFP.readdmitxp(TestPlanParaByPN.DUT_USB_Port, 0xA0, 50);
                        break;
                    case 2:
                        dmitxp = QSFP.readdmitxp(TestPlanParaByPN.DUT_USB_Port, 0xA0, 52);
                        break;
                    case 3:
                        dmitxp = QSFP.readdmitxp(TestPlanParaByPN.DUT_USB_Port, 0xA0, 54);
                        break;
                    case 4:
                        dmitxp = QSFP.readdmitxp(TestPlanParaByPN.DUT_USB_Port, 0xA0, 56);
                        break;
                    default:
                        break;
                }
                return dmitxp;
            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.ToString());
                throw new Exception("ReadDmiTxP failed");
            }
        }        

        public override ushort ReadADC(NameOfADC enumName, int channel)
        {
            try
            {
                string filter = "ItemName = " + "'" + enumName.ToString() + "'";
                DataRow[] foundRows = this.dataTable_DUTCoeffControlByPN.Select(filter);

                for (int row = 0; row < foundRows.Length; row++)
                {
                    if (Convert.ToInt32(foundRows[row]["Channel"]) == channel)
                    {
                        byte page = Convert.ToByte(foundRows[row]["Page"]);
                        int startAddress = Convert.ToInt32(foundRows[row]["StartAddress"]);
                        int length = Convert.ToInt32(foundRows[row]["Length"]);
                        string format = foundRows[row]["Format"].ToString();

                        this.EnterEngMode(page);
                        UInt16 valueADC = QSFP.readadc(TestPlanParaByPN.DUT_USB_Port, 0xA0, startAddress);
                        Log.SaveLogToTxt("Current TXPOWERADC is " + valueADC);
                        return valueADC;
                    }
                }
                throw new Exception("ReadADC failed, please check module table config");
            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.ToString());
                throw new Exception("ReadADC failed");
            }
        }        

        public override bool WriteChipDAC(NameOfChipDAC enumName, int channel, object writeDAC)
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

        public override bool ReadChipDAC(NameOfChipDAC enumName, int channel, out int readDAC)
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

        private object Write_Store_Read_ChipReg(object writeData, byte length, bool isLittleendian, int regAddress,  byte chipLine, byte chipset, ChipOperation operate)
        {
            byte[] writeDataArray = Algorithm.ObjectToByteArray(writeData, length, isLittleendian);
            this.EnterEngMode(DebugInterface.EngPage);

            if ((int)operate == 0)//写
            {
                this.WriteDriver40g(TestPlanParaByPN.DUT_USB_Port, 0xA0, DebugInterface.StartAddress, regAddress, chipLine, chipset, writeDataArray, GlobalParaByPN.isOldDriver);
            }
            else if((int)operate == 1)//存
            {
                this.StoreDriver40g(TestPlanParaByPN.DUT_USB_Port, 0xA0, DebugInterface.StartAddress, regAddress, chipLine, chipset, writeDataArray, GlobalParaByPN.isOldDriver);                
            }
            else if ((int)operate == 2)
            {
                byte[] readData = this.ReadDriver40g(TestPlanParaByPN.DUT_USB_Port, 0xA0, DebugInterface.StartAddress, regAddress, chipLine, chipset, length, GlobalParaByPN.isOldDriver);
                
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

        public override double CalRxRes(double inputPower_dBm, int channel, double ratio, double U_Ref, double resolution, double R_rssi)
        {
            ushort RxPowerADC = this.ReadADC(NameOfADC.RXPOWERADC, channel);
            double k = ratio * U_Ref / (Math.Pow(2, resolution) - 1);
            double U_rssi = RxPowerADC * k;
            double I_rssi = U_rssi / R_rssi;
            double responsivity = I_rssi / Math.Pow(10, inputPower_dBm * 0.1);

            return responsivity;
        }

        public override bool CloseAndOpenAPC(byte mode)
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
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private bool APCON(byte apcswitch)
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
            this.EnterEngMode(page);
            USBIO.WrtieReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, startAddress, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
            return true;
        }

        private bool APCOFF(byte apcswitch)
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
            this.EnterEngMode(page);
            USBIO.WrtieReg(TestPlanParaByPN.DUT_USB_Port, 0xA0, startAddress, IOPort.SoftHard.HARDWARE_SEQUENT, buff);
            return true;
        }
    }
}
