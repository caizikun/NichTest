using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NichTest
{
    public class MP1800PPG : PPG
    {
        private double dataLevelGuardAmpMax;//输出保护幅度最大值（单位mV): 1
        private double dataLevelGuardOffsetMax;//输出保护最大偏移量（单位mV）: 0
        private double dataLevelGuardOffsetMin;//输出保护最小偏移量（单位mV）: 0
        private double dataAmplitude;//输出单端幅度（单位mV）: 0
        private double dataCrossPoint;//输出数据信号交叉点: 50
        private string configFilePath;//仪器中配置文件的地址: ""
        private byte slot;//PPG所处槽位: 1
        private byte clockSource;//时钟源, PPG码型选择(0=INTernal1;1=EXTernal）: 0
        private byte auxOutputClkDiv;//	辅助输出是时钟信号的几分频: 4
        private byte dataSwitch;//数据输出开关: 1
        private byte dataTrackingSwitch;//DATA /DATA跟踪开关: 1
        private byte dataLevelGuardSwitch;//输出保护开关: 1
        private byte dataAcModeSwitch;//输出模式选择(0=DC，1=AC）: 1
        private byte dataLevelMode;//输出电平模式选择（0=VARiable,1=NECL,2=PCML,3=NCML,4=SCFL,5=LVPecl,6=LVDS200,7=LVDS400）: 0
        private byte clockSwitch;//时钟输出开关: 1
        private byte outputSwitch;//总输出开关: 1
        private string patternfile;//@"C:\02"
        private byte OperationBitrate;//外部时钟速率0=12.89.1;1=25.78: 0
        private static object syncRoot = SyncRoot_PPG_ED.Get_SyncRoot_PPG_ED();//used for thread synchronization

        public override bool Initial(Dictionary<string, string> inPara, int syn = 0)
        {
            try
            {
                this.IOType = inPara["IOTYPE"];
                this.address = inPara["ADDR"];
                this.name = inPara["NAME"];
                this.reset = Convert.ToBoolean(inPara["RESET"]);
                this.role = Convert.ToInt32(inPara["ROLE"]);

                this.rate = inPara["DATARATE"];
                this.dataLevelGuardAmpMax = Convert.ToDouble(inPara["DATALEVELGUARDAMPMAX"]);
                this.dataLevelGuardOffsetMax = Convert.ToDouble(inPara["DATALEVELGUARDOFFSETMAX"]);
                this.dataLevelGuardOffsetMin = Convert.ToDouble(inPara["DATALEVELGUARDOFFSETMIN"]);
                this.dataAmplitude = Convert.ToDouble(inPara["DATAAMPLITUDE"]);
                this.dataCrossPoint = Convert.ToDouble(inPara["DATACROSSPOINT"]);
                this.configFilePath = inPara["CONFIGFILEPATH"];
                this.slot = Convert.ToByte(inPara["SLOT"]);
                this.clockSource = Convert.ToByte(inPara["CLOCKSOURCE"]);
                this.auxOutputClkDiv = Convert.ToByte(inPara["AUXOUTPUTCLKDIV"]);
                this.totalChannels = Convert.ToByte(inPara["TOTALCHANNEL"]);
                this.prbsLength = Convert.ToByte(inPara["PRBSLENGTH"]);
                this.patternType = Convert.ToByte(inPara["PATTERNTYPE"]);
                this.dataSwitch = Convert.ToByte(inPara["DATASWITCH"]);
                this.dataTrackingSwitch = Convert.ToByte(inPara["DATATRACKINGSWITCH"]);
                this.dataLevelGuardSwitch = Convert.ToByte(inPara["DATALEVELGUARDSWITCH"]);
                this.dataAcModeSwitch = Convert.ToByte(inPara["DATAACMODESWITCH"]);
                this.dataLevelMode = Convert.ToByte(inPara["DATALEVELMODE"]);
                this.clockSwitch = Convert.ToByte(inPara["CLOCKSWITCH"]);
                this.outputSwitch = Convert.ToByte(inPara["OUTPUTSWITCH"]);
                this.patternfile = inPara["PATTERNFILE"];
                this.OperationBitrate = Convert.ToByte(inPara["OPERATIONBITRATE"]);

                this.isConnected = false;
                switch (IOType)
                {
                    case "GPIB":
                        lock (syncRoot)
                        {
                            this.WriteString("*IDN?");
                            string content = this.ReadString();
                            this.isConnected = content.Contains("1800");
                        }
                        break;

                    default:
                        Log.SaveLogToTxt("GPIB port error.");
                        break;
                }
                return this.isConnected;
            }
            catch
            {
                Log.SaveLogToTxt("Failed to initial MP1800.");
                return false;
            }
        }

        public override bool Configure(int syn = 0)
        {
            lock (syncRoot)
            {
                if (this.isConfigured)//曾经设定过
                {
                    return true;
                }

                if (this.reset == true)
                {
                    this.Reset();
                }
                ConfigureSlot(slot, syn);
                ConfigureClockSource(clockSource, syn);
                ConfigureAuxOutputClkDiv(auxOutputClkDiv, syn);
                if (clockSource == 0)//内部时钟
                {

                    ConfigureDataRate(syn);
                }
                else
                {
                    ConfigureOperationBitrate(OperationBitrate);
                }
                for (byte i = 1; i <= totalChannels; i++)
                {
                    ConfigureChannel(i, syn);
                    ConfigurePatternType(patternType, syn);
                    ConfigurePolarity(true);
                    if (patternType == 0)
                    {
                        ConfigurePrbsLength(prbsLength, syn);

                    }
                    if (patternType == 2)
                    {
                        configureusertype(patternfile);

                    }
                    ConfigureDataSwitch(dataSwitch, syn);
                    ConfigureDataTracking(dataTrackingSwitch, syn);
                    ConfigureDataLevelGuardAmpMax(dataLevelGuardAmpMax, syn);
                    ConfigureDataLevelGuardOffset(dataLevelGuardOffsetMax, dataLevelGuardOffsetMin, syn);
                    ConfigureDataLevelGuardSwitch(dataLevelGuardSwitch, syn);
                    ConfigureDataAcModeSwitch(dataAcModeSwitch, syn);
                    ConfigureDataLevelMode(dataLevelMode, syn);
                    ConfigureDataAmplitude(dataAmplitude, syn);
                    ConfigureDataCrossPoint(dataCrossPoint, syn);
                }
                ConfigureChannel(1, syn);
                ConfigureClockSwitch(clockSwitch, syn);
                ConfigureOutputSwitch(outputSwitch, syn);
                this.isConfigured = true;
                return this.isConfigured;
            }
        }

        public bool Reset()
        {
            lock (syncRoot)
            {
                if (this.WriteString("*RST"))
                {
                    Thread.Sleep(3000);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override bool ChangeChannel(int channel, int syn = 0)
        {
            return ConfigureChannel((byte)channel);
        }

        public override bool ConfigOffset(int channel, double offset, int syn = 0)
        {
            return true;
        }

        public override bool DataPattern_DataLength(string length)
        {
            lock (syncRoot)
            {
                return this.WriteString(":SOURce:PATTern:DATA:LENGth " + length);
            }
        }

        public override bool DataPattern_SetPatternData(string start, string end, string data)
        {
            lock (syncRoot)
            {
                return this.WriteString(":SOURce:PATTern:DATA:WHOLe " + start + "," + end + ",\"" + data + "\"");
            }
        }

        public override bool ConfigureOTxPolarity(bool polarity)
        {
            lock (syncRoot)
            {
                // :SOURce:PATTern:LOGic NEGative
                ConfigureSlot(slot, 1);
                ConfigureChannel(1, 1);// 先进入通道,后才能设置极性

                string StrPolarity;

                if (polarity)
                {
                    StrPolarity = "POS";
                }
                else
                {
                    StrPolarity = "NEG";
                }
                this.WriteString(":SOURce:PATTern:LOGic " + StrPolarity);
                Thread.Sleep(200);
                this.WriteString(":SOURce:PATTern:LOGic?");
                string Str = this.ReadString();
                if (Str.Contains(StrPolarity))
                {
                    return true;
                }
                else
                {
                    return false;

                }
            }
        }

        public override bool ConfigurePatternType(byte patternType, int syn = 0)//0=PRBS,1=ZSUBstitution,2=DATA,3=ALT,4=MIXData,5=MIXalt,6=SEQuence
        {//其他选项datasheet里也没有包含，实测返回值也没有，所以删掉，直接是mixd
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                string strPatternType;
                switch (patternType)
                {
                    case 0:
                        strPatternType = "PRBS";
                        break;
                    case 1:
                        strPatternType = "ZSUB";
                        break;
                    case 2:
                        strPatternType = "DATA";
                        break;
                    case 3:
                        strPatternType = "PRBS";//"ALT";
                        break;
                    case 4:
                        strPatternType = "MIXD";
                        break;
                    case 5:
                        strPatternType = "PRBS";// "MIXalt";
                        break;
                    case 6:
                        strPatternType = "PRBS";// "SEQuence";
                        break;
                    default:
                        strPatternType = "PRBS";
                        break;
                }
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("PatternType is" + strPatternType);
                        return this.WriteString(":SOURce:PATTern:TYPE " + strPatternType);
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":SOURce:PATTern:TYPE " + strPatternType);
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":SOURce:PATTern:TYPE?");
                                readtemp = this.ReadString();
                                if (readtemp == strPatternType + "\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("PatternType is" + strPatternType);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set PatternType wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }
        // override public 
        public override bool ConfigurePrbsLength(byte prbsLength, int syn = 0)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("PrbsLength is" + prbsLength);
                        return this.WriteString(":SOURce:PATTern:PRBS:LENGth " + prbsLength.ToString());
                    }
                    else
                    {

                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":SOURce:PATTern:PRBS:LENGth " + prbsLength.ToString());
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":SOURce:PATTern:PRBS:LENGth?");
                                readtemp = this.ReadString();
                                if (readtemp == prbsLength.ToString() + "\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("PrbsLength is" + prbsLength);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set PrbsLength wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        public override bool RecallPrbsLength()
        {
            lock (syncRoot)
            {
                ConfigurePatternType(3);//[20160428]nate: add
                return ConfigurePrbsLength(prbsLength);
            }
        }

        private bool ConfigureSlot(byte slot, int syn = 0)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("MP1800ppg slot is" + slot);
                        return this.WriteString(":MODule:ID " + slot.ToString() + "\n");
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":MODule:ID " + slot.ToString() + "\n");
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":MODule:ID?");
                                readtemp = this.ReadString();
                                if (readtemp == slot.ToString() + "\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                flag = true;
                                Log.SaveLogToTxt("MP1800ppg slot is" + slot);

                            }
                            else
                            {
                                Log.SaveLogToTxt("set MP1800ppg slot wrong");
                            }
                        }

                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private bool ConfigureClockSource(byte clkSource, int syn = 0)//0=InternalClock
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                string strClkSource;
                switch (clkSource)
                {
                    case 0:
                        strClkSource = "INTernal1";
                        break;
                    case 1:
                        strClkSource = "EXTernal";
                        break;
                    default:
                        strClkSource = "INTernal1";
                        break;
                }
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("MP1800ppg ClockSource is" + strClkSource);
                        return this.WriteString(":SYSTem:INPut:CSELect " + strClkSource + "\n");
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":SYSTem:INPut:CSELect " + strClkSource + "\n");
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            this.WriteString(":SYSTem:INPut:CSELect?");
                            readtemp = this.ReadString();

                            if (clkSource == 0)
                            {


                                for (k = 0; k < 3; k++)
                                {

                                    this.WriteString(":SYSTem:INPut:CSELect?");
                                    readtemp = this.ReadString();
                                    // if (readtemp == strClkSource)
                                    if (readtemp.Contains("INT1"))
                                        break;
                                }
                            }
                            if (k <= 3)
                            {
                                flag = true;
                                Log.SaveLogToTxt("MP1800ppg ClockSource is" + strClkSource);

                            }
                            else
                            {
                                Log.SaveLogToTxt("set MP1800ppg ClockSource wrong");
                            }
                        }

                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private bool ConfigureAuxOutputClkDiv(byte clkDiv, int syn = 0)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("AuxOutputClkDiv is" + clkDiv);
                        return this.WriteString(":OUTPut:SYNC:SOURce NCLock," + clkDiv.ToString() + "\n");
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":OUTPut:SYNC:SOURce NCLock," + clkDiv.ToString() + "\n");//:OUTPut:SYNC:SOURce NCLock,4
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":OUTPut:SYNC:SOURce?");
                                readtemp = this.ReadString();
                                if (readtemp == "CLOC" + clkDiv.ToString() + "\n")//"NCL"+ clkDiv.ToString() + "\n"
                                    break;//need modify
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("AuxOutputClkDiv is" + clkDiv);

                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set AuxOutputClkDiv wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }//PPG AuxOutput,形参为分频数

        private bool ConfigureDataRate(int syn = 0)//PPG比特率，单位为Gbps.
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                try
                {

                    string Str = ":OUTPut:DATA:STANdard \"Variable\"";
                    this.WriteString(Str);

                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("MP1800ppg dataRate is" + rate);
                        return this.WriteString(":OUTPut:DATA:BITRate " + rate);

                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":OUTPut:DATA:BITRate " + rate);
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":OUTPut:DATA:BITRate?");
                                readtemp = this.ReadString();
                                if (Convert.ToDouble(readtemp).ToString("0.00") == Convert.ToDouble(rate).ToString("0.00"))
                                    break;
                            }
                            if (k < 3)
                            {
                                Log.SaveLogToTxt("MP1800ppg dataRate is" + rate);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set MP1800ppg dataRate wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private bool ConfigureOperationBitrate(byte OperationRate, int syn = 0)//PPG比特率，单位为Gbps.
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                string StrRate;
                try
                {
                    switch (OperationRate)
                    {
                        case 0:
                            StrRate = "HALF";
                            break;
                        case 1:
                            StrRate = "HIGH1";
                            break;
                        default:
                            StrRate = "HIGH1";
                            break;
                    }
                    Log.SaveLogToTxt("MP1800ppg OperationRate is" + StrRate);

                    this.WriteString(":SYSTem:OUTPut:BITRate " + StrRate);

                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("MP1800ppg OperationRate is" + StrRate);
                        // string STR = ":SYSTem:OUTPut:BITRate " + StrRate;
                        return this.WriteString(":SYSTem:OUTPut:BITRate " + StrRate);
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":SYSTem:OUTPut:BITRate?");
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":OUTPut:DATA:BITRate?");
                                readtemp = this.ReadString();
                                if (readtemp == StrRate)
                                    break;
                            }
                            if (k < 3)
                            {
                                Log.SaveLogToTxt("MP1800ppg OperationRate is" + rate);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set MP1800ppg OperationRate wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private bool ConfigureChannel(byte channel, int syn = 0)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("MP1800ppg channel is" + channel);
                        return this.WriteString(":INTerface:ID " + channel.ToString());
                    }
                    else
                    {

                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":INTerface:ID " + channel.ToString());
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":INTerface:ID?");
                                readtemp = this.ReadString();
                                if (readtemp == channel.ToString() + "\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("MP1800ppg channel is" + channel);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set MP1800ppg channel wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private bool configureusertype(string filename)//patternType=2,选择DATA,user pattern
        {
            lock (syncRoot)
            {
                bool flag = false;
                string AA = @filename;
                flag = this.WriteString(":SYSTem:MMEMory:PATTern:RECall " + "\"" + AA + "\"" + ",BIN");
                // Thread.Sleep(1000);
                return flag;
            }
        }

        private bool ConfigurePolarity(bool polarity)// 它没有通道好的选择,必须依赖别人修改通道号
        {
            // :SOURce:PATTern:LOGic NEGative
            lock (syncRoot)
            {
                string StrPolarity;

                if (polarity)
                {
                    StrPolarity = "POS";
                }
                else
                {
                    StrPolarity = "NEG";
                }
                this.WriteString(":SOURce:PATTern:LOGic " + StrPolarity);
                Thread.Sleep(200);
                this.WriteString(":SOURce:PATTern:LOGic?");
                string Str = this.ReadString();
                if (Str.Contains(StrPolarity))
                {
                    return true;
                }
                else
                {
                    return false;

                }
            }
        }

        private bool ConfigureDataSwitch(byte dataSwitch, int syn = 0)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                string strDataSwitch;
                switch (dataSwitch)
                {
                    case 0:
                        strDataSwitch = "OFF";
                        break;
                    case 1:
                        strDataSwitch = "ON";
                        break;
                    default:
                        strDataSwitch = "ON";
                        break;
                }
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("DataSwitch is" + strDataSwitch);
                        return this.WriteString(":OUTPut:DATA:OUTPut " + strDataSwitch);

                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":OUTPut:DATA:OUTPut " + strDataSwitch);
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":OUTPut:DATA:OUTPut?");
                                readtemp = this.ReadString();
                                if (readtemp == dataSwitch.ToString() + "\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("DataSwitch is" + strDataSwitch);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("DataSwitch wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private bool ConfigureDataTracking(byte dataTrackingSwitch, int syn = 0)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                string strDataTrackingSwitch;
                switch (dataTrackingSwitch)
                {
                    case 0:
                        strDataTrackingSwitch = "OFF";
                        break;
                    case 1:
                        strDataTrackingSwitch = "ON";
                        break;
                    default:
                        strDataTrackingSwitch = "ON";
                        break;
                }
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("DataTracking is" + strDataTrackingSwitch);
                        return this.WriteString(":OUTPut:DATA:TRACking " + strDataTrackingSwitch);

                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":OUTPut:DATA:TRACking " + strDataTrackingSwitch);
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":OUTPut:DATA:TRACking?");
                                readtemp = this.ReadString();
                                if (readtemp == dataTrackingSwitch.ToString() + "\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("DataTracking is" + strDataTrackingSwitch);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("DataTracking wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }//0=TrcakingOff,1=TrackingON

        private bool ConfigureDataLevelGuardAmpMax(double ampMax, int syn = 0)//ampMax单位为mV
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("DataLevelGuardAmpMax is" + (ampMax / 1000).ToString());

                        return this.WriteString(":OUTPut:DATA:LIMitter:AMPLitude " + (ampMax / 1000).ToString());
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":OUTPut:DATA:LIMitter:AMPLitude " + (ampMax / 1000).ToString());
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":OUTPut:DATA:LIMitter:AMPLitude?");
                                readtemp = this.ReadString();
                                if (Convert.ToDouble(readtemp) == Convert.ToDouble((ampMax / 1000).ToString()))
                                    break;//need modify
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("DataLevelGuardAmpMax is" + (ampMax / 1000).ToString());
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set DataLevelGuardAmpMax wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private bool ConfigureDataLevelGuardOffset(double offsetMax, double offsetMin, int syn = 0)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("DataLevelGuardoffsetMax is" + (offsetMax / 1000).ToString() + "DataLevelGuardoffsetMin is" + (offsetMin / 1000).ToString());
                        return this.WriteString(":OUTPut:DATA:LIMitter:OFFSet " + (offsetMax / 1000).ToString() + "," + (offsetMin / 1000).ToString());


                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":OUTPut:DATA:LIMitter:OFFSet " + (offsetMax / 1000).ToString() + "," + (offsetMin / 1000).ToString());
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":OUTPut:DATA:LIMitter:OFFSet?");
                                readtemp = this.ReadString();
                                double temp1 = Convert.ToDouble((offsetMax / 1000));
                                double temp2 = Convert.ToDouble((offsetMin / 1000));
                                if (readtemp == temp1.ToString("0.000") + "," + temp2.ToString("0.000") + "\n")
                                    break;//need modify
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("DataLevelGuardoffsetMax is" + (offsetMax / 1000).ToString() + "DataLevelGuardoffsetMin is" + (offsetMin / 1000).ToString());
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set DataLevelGuardoffset wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }//形参单位全部为mV

        private bool ConfigureDataLevelGuardSwitch(byte lvGuardSwitch, int syn = 0)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                string strLvGuardSwitch;
                switch (lvGuardSwitch)
                {
                    case 0:
                        strLvGuardSwitch = "OFF";
                        break;
                    case 1:
                        strLvGuardSwitch = "ON";
                        break;
                    default:
                        strLvGuardSwitch = "ON";
                        break;
                }
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("DataLevelGuardSwitch is" + strLvGuardSwitch);
                        return this.WriteString(":OUTPut:DATA:LEVGuard " + strLvGuardSwitch);

                    }
                    else
                    {

                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":OUTPut:DATA:LEVGuard " + strLvGuardSwitch);
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":OUTPut:DATA:LEVGuard?");
                                readtemp = this.ReadString();
                                if (readtemp == lvGuardSwitch.ToString() + "\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("DataLevelGuardSwitch is" + strLvGuardSwitch);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set DataLevelGuardSwitch wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private bool ConfigureDataAcModeSwitch(byte acSwitch, int syn = 0)//0=DC Mode，1=AC Mode
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                string strAcSwitch;
                switch (acSwitch)
                {
                    case 0:
                        strAcSwitch = "OFF";
                        break;
                    case 1:
                        strAcSwitch = "ON";
                        break;
                    default:
                        strAcSwitch = "ON";
                        break;
                }
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("DataAcModeSwitch is" + strAcSwitch);
                        return this.WriteString(":OUTPut:DATA:AOFFset " + strAcSwitch);
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":OUTPut:DATA:AOFFset " + strAcSwitch);
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":OUTPut:DATA:AOFFset?");
                                readtemp = this.ReadString();
                                if (readtemp == acSwitch.ToString() + "\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("DataAcModeSwitch is" + strAcSwitch);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set DataAcModeSwitch wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private bool ConfigureDataLevelMode(byte elecLevelMode, int syn = 0)//0=VARiable,1=NECL,2=PCML,3=NCML,4=SCFL,5=LVPecl,6=LVDS200,7=LVDS400
        {//无6 7选项,mode选择与levelguard范围有关
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                string strElecLevelMode;
                switch (elecLevelMode)
                {
                    case 0:
                        strElecLevelMode = "VAR";
                        break;
                    case 1:
                        strElecLevelMode = "NECL";
                        break;
                    case 2:
                        strElecLevelMode = "PCML";
                        break;
                    case 3:
                        strElecLevelMode = "NCML";
                        break;
                    case 4:
                        strElecLevelMode = "SCFL";
                        break;
                    case 5:
                        strElecLevelMode = "LVP";
                        break;
                    //case 6:
                    //    strElecLevelMode = "LVDS200";
                    //    break;
                    //case 7:
                    //    strElecLevelMode = "LVDS400";
                    //    break;
                    default:
                        strElecLevelMode = "VAR";
                        break;
                }
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("DataLevelMode is" + strElecLevelMode);
                        return this.WriteString(":OUTPut:DATA:LEVel DATA," + strElecLevelMode);
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":OUTPut:DATA:LEVel DATA," + strElecLevelMode);
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":OUTPut:DATA:LEVel? DATA");
                                readtemp = this.ReadString();
                                if (readtemp == strElecLevelMode + "\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("DataLevelMode is" + strElecLevelMode);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set DataLevelMode wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private bool ConfigureDataAmplitude(double amplitude, int syn = 0)//单位为mV
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("DataAmplitude is" + (amplitude / 1000).ToString());
                        return this.WriteString(":OUTPut:DATA:AMPLitude DATA," + (amplitude / 1000).ToString());
                    }
                    else
                    {

                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":OUTPut:DATA:AMPLitude DATA," + (amplitude / 1000).ToString());
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":OUTPut:DATA:AMPLitude? DATA");
                                readtemp = this.ReadString();
                                if (Convert.ToDouble(readtemp) == Convert.ToDouble((amplitude / 1000).ToString() + "\n"))
                                //if (Convert.ToDouble(readtemp) == Convert.ToDouble((amplitude).ToString() + "\n"))
                                        break;//need modify
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("DataAmplitude is" + (amplitude / 1000).ToString());
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set DataAmplitude wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private bool ConfigureDataCrossPoint(double crossPoint, int syn = 0)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("DataCrossPoint is" + crossPoint.ToString());
                        return this.WriteString(":OUTPut:DATA:CPOint DATA," + crossPoint.ToString());
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":OUTPut:DATA:CPOint DATA," + crossPoint.ToString());
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":OUTPut:DATA:CPOint? DATA");
                                readtemp = this.ReadString();
                                if (Convert.ToDouble(readtemp) == Convert.ToDouble(crossPoint.ToString() + "\n"))
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("DataCrossPoint is" + crossPoint.ToString());
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set DataCrossPoint wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private bool ConfigureClockSwitch(byte clkSwitch, int syn = 0)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                string strClkSwitch;
                switch (clkSwitch)
                {
                    case 0:
                        strClkSwitch = "OFF";
                        break;
                    case 1:
                        strClkSwitch = "ON";
                        break;
                    default:
                        strClkSwitch = "ON";
                        break;
                }
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("ClockSwitch is" + strClkSwitch);
                        return this.WriteString(":OUTPut:CLOCk:OUTPut " + strClkSwitch);

                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":OUTPut:CLOCk:OUTPut " + strClkSwitch);
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":OUTPut:CLOCk:OUTPut?");
                                readtemp = this.ReadString();
                                if (readtemp == clkSwitch.ToString() + "\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("ClockSwitch is" + strClkSwitch);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set ClockSwitch wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private bool ConfigureOutputSwitch(byte outSwitch, int syn = 0)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                string strOutSwitch;
                switch (outSwitch)
                {
                    case 0:
                        strOutSwitch = "OFF";
                        break;
                    case 1:
                        strOutSwitch = "ON";
                        break;
                    default:
                        strOutSwitch = "ON";
                        break;
                }
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("OutputSwitch is" + strOutSwitch);
                        return this.WriteString(":SOURce:OUTPut:ASET " + strOutSwitch);
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":SOURce:OUTPut:ASET " + strOutSwitch);
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":SOURce:OUTPut:ASET?");
                                readtemp = this.ReadString();
                                if (readtemp == outSwitch.ToString() + "\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("OutputSwitch is" + strOutSwitch);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set OutputSwitch wrong");

                            }

                        }
                        return flag;
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

    public class MP1800ED : ErrorDetector
    {
        private byte currentChannel;//当前通道号: 1
        private byte slot;//ED所处槽位: 3
        private byte dataInputInterface;//数据输入接口类型: 2
        private byte errorResultZoom;//误码显示类型0=ZoomIn(显示详细误码信息),1=ZoomOut(只显示误码率和误码数): 1
        private byte edGatingMode;//累积模式（0=REPeat,1=SINGle,2=UNTimed）: 1
        private byte edGatingUnit;//累积单位（0=TIME,1=CLOCk,2=ERRor,3=BLOCk）: 0
        private int edGatingTime;//误码累计时间: 5s
        private string patternfile;//@"C:\02"
        private static object syncRoot = SyncRoot_PPG_ED.Get_SyncRoot_PPG_ED();//used for thread synchronization

        public override bool Initial(Dictionary<string, string> inPara, int syn = 0)
        {
            try
            { 
                this.IOType = inPara["IOTYPE"];
                this.address = inPara["ADDR"];
                this.name = inPara["NAME"];
                this.reset = Convert.ToBoolean(inPara["RESET"]);
                this.role = Convert.ToInt32(inPara["ROLE"]);

                this.slot = Convert.ToByte(inPara["SLOT"]);
                this.totalChannels = Convert.ToByte(inPara["TOTALCHANNEL"]);
                this.currentChannel = Convert.ToByte(inPara["CURRENTCHANNEL"]);
                this.dataInputInterface = Convert.ToByte(inPara["DATAINPUTINTERFACE"]);
                this.errorResultZoom = Convert.ToByte(inPara["ERRORRESULTZOOM"]);
                this.edGatingMode = Convert.ToByte(inPara["EDGATINGMODE"]);
                this.edGatingUnit = Convert.ToByte(inPara["EDGATINGUNIT"]);
                this.edGatingTime = Convert.ToInt32(inPara["EDGATINGTIME"]);
                this.patternfile = inPara["PATTERNFILE"];
                this.prbsLength = Convert.ToByte(inPara["PRBSLENGTH"]);

                this.isConnected = false;
                switch (IOType)
                {
                    case "GPIB":
                        lock (syncRoot)
                        {
                            this.WriteString("*IDN?");
                            string content = this.ReadString();
                            this.isConnected = content.Contains("1800");
                        }
                        break;

                    default:
                        Log.SaveLogToTxt("GPIB port error.");
                        break;
                }
                return this.isConnected;
            }
            catch
            {
                Log.SaveLogToTxt("Failed to initial MP1800.");
                return false;
            }
        }

        public override bool Configure(int syn = 0)
        {
            lock (syncRoot)
            {
                if (this.isConfigured)//曾经设定过
                {
                    return true;
                }

                if (this.reset == true)
                {
                    this.Reset();
                }
                ConfigureSlot(slot, syn);
                for (byte i = 1; i <= totalChannels; i++)
                {
                    ConfigureChannel(i, syn);
                    ConfigureDataInputInterface(dataInputInterface, syn);
                    ConfigurePatternType(patternType, syn);
                    if (patternType == 0)
                    {
                        ConfigurePrbsLength(prbsLength, syn);

                    }
                    if (patternType == 2)
                    {
                        configureusertype(patternfile);

                    }
                    ConfigureErrorResultZoomSwitch(errorResultZoom, syn);
                    //ConfigurePolarity(true);
                }
                AutoAdjust();
                this.isConfigured = true;
                return this.isConfigured;
            }
        }

        public bool Reset()
        {
            lock (syncRoot)
            {
                if (this.WriteString("*RST"))
                {
                    Thread.Sleep(3000);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override bool ChangeChannel(int channel, int syn = 0)
        {
            lock (syncRoot)
            {
                ConfigureSlot(slot, syn);
                byte channelbyte = Convert.ToByte(channel);
                Log.SaveLogToTxt("channel is" + channel);
                currentChannel = channelbyte;
                currentChannel = channelbyte;

                ConfigureChannel(channelbyte, syn);

                ConfigureEdGatingMode(edGatingMode, syn);
                ConfigureEdGatingUnit(edGatingUnit, syn);

                return true;
            }
        }

        public override bool ConfigOffset(int channel, double offset, int syn = 0)
        {
            return true;
        }

        public override bool AutoAlign(bool becenter)
        {
            lock (syncRoot)
            {
                this.WriteString(":SENSe:MEASure:AADJust32:STATe?");
                string Str = this.ReadString();
                if (Str != "1\n")
                {
                    AutoAdjust();
                    Thread.Sleep(200);
                    this.WriteString(":SENSe:MEASure:AADJust32:STATe?");
                    Str = this.ReadString();
                    if (Str != "1\n") return false;

                }

                return true;
            }
        }

        public override double GetErrorRate(int syn = 0)
        {
            lock (syncRoot)
            {
                int readerr = 0;
                ConfigureSlot(slot, syn);
                ConfigureChannel(currentChannel, syn);
                ConfigureEdGatingMode(edGatingMode, syn);
                ConfigureEdGatingUnit(edGatingUnit, syn);
                ConfigureEdGatingPeriod(edGatingTime, syn);
                EdGatingStart();
                double errcount = 0;
                double errratio = 0;

                for (int i = 0; i < 12; i++)
                {

                    Thread.Sleep(Convert.ToInt32(edGatingTime * 1000));
                    errcount = QureyEdErrorCount();
                    errratio = QureyEdErrorRatio();
                    if (errratio == 1)
                        readerr++;
                    if (readerr > 3)
                        break;
                    if (errcount > 3)
                        break;
                }
                return errratio;
            }
        }
        //快速误码测试
        public override double RapidErrorRate(int syn = 0)
        {
            lock (syncRoot)
            {
                //ConfigureSlot(slot, syn);
                //ConfigureChannel(currentChannel, syn);
                //ConfigureEdGatingMode(edGatingMode, syn);
                //ConfigureEdGatingUnit(edGatingUnit, syn);
                // ConfigureEdGatingPeriod(edGatingTime, syn);
                EdGatingStart();
                //double errcount = 0;
                double errratio = 0;
                Thread.Sleep(Convert.ToInt32(edGatingTime * 1000));
                //errcount = QureyEdErrorCount();
                errratio = QureyEdErrorRatio();
                return errratio;
            }
        }

        public override double[] RapidErrorRate_AllCH(int syn = 0)
        {
            lock (syncRoot)
            {
                double[] ErrRata = new double[4];
                for (int i = 0; i < 4; i++)
                {
                    ConfigureChannel(currentChannel, syn);
                    ConfigureEdGatingMode(edGatingMode, syn);
                    ConfigureEdGatingUnit(edGatingUnit, syn);
                    ConfigureEdGatingPeriod(edGatingTime, syn);
                    EdGatingStart();
                }

                Thread.Sleep(Convert.ToInt32(edGatingTime * 1000));

                for (int i = 0; i < 4; i++)
                {
                    ConfigureChannel(i + 1, syn);
                    ErrRata[i] = QureyEdErrorRatio();
                }
                return ErrRata;
            }
        }

        public override double[] RapidErrorCount_AllCH(int syn = 0, bool IsClear = false)
        {
            lock (syncRoot)
            {
                double[] ErrCount = new double[4];
                ConfigureSlot(slot, syn);
                if (IsClear)
                {
                    EdGatingStart();
                }

                for (int i = 0; i < 4; i++)
                {
                    // ConfigureChannel(channelbyte, syn);
                    ConfigureChannel(i + 1, syn);
                    ErrCount[i] = QureyEdErrorCount();

                }

                return ErrCount;
            }
        }

        public override bool EdGatingStart()
        {
            lock (syncRoot)
            {
                try
                {
                    return this.WriteString(":SENSe:MEASure:STARt");
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        public override double QureyEdErrorRatio()
        {
            lock (syncRoot)
            {
                string StrBer = "1";
                double ber = 0;
                try
                {
                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {
                            this.WriteString(":CALCulate:DATA:EALarm? \"CURRent:ER:TOTal\"");
                            Thread.Sleep(100);
                            StrBer = this.ReadString(100).Replace("\"", "").Replace("\n", "");
                            Thread.Sleep(100);

                            if (StrBer.Contains("---") || StrBer == "" || StrBer == null)
                            {
                                ber = 9.999E+17;

                            }
                            else
                            {
                                break;
                            }


                        }
                        catch (Exception ex)
                        {

                            Log.SaveLogToTxt(ex.Message);
                        }

                    }
                    if (StrBer.Contains("---") || StrBer == "" || StrBer == null)
                    {
                        ber = 9.999E+17;
                        Log.SaveLogToTxt("ErrorCount is" + ber);
                    }
                    else
                    {
                        ber = Convert.ToDouble(StrBer);
                        // Log.SaveLogToTxt("ErrorCount is" + ber);
                    }
                }
                catch (Exception ex)
                {
                    ber = 9.999E+17;
                    Log.SaveLogToTxt(ex.ToString());
                    return ber;
                }
                return ber;
            }
        }

        public override bool ConfigureERxPolarity(bool polarity)
        {
            lock (syncRoot)
            {
                // :SOURce:PATTern:LOGic NEGative
                ConfigureSlot(slot, 0);
                ConfigureChannel(1, 1);// 先进入通道,后才能设置极性

                string StrPolarity;

                if (polarity)
                {
                    StrPolarity = "POS";
                }
                else
                {
                    StrPolarity = "NEG";
                }
                this.WriteString(":SENSe:PATTern:LOGic " + StrPolarity);
                Thread.Sleep(200);
                this.WriteString(":SENSe:PATTern:LOGic?");
                string Str = this.ReadString();
                if (Str.Contains(StrPolarity))
                {
                    return true;
                }
                else
                {
                    return false;

                }
            }
        }

        private bool ConfigureSlot(byte slot, int syn = 0)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("MP1800 ED slot is" + slot);
                        return this.WriteString(":MODule:ID " + slot.ToString() + "\n");
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":MODule:ID " + slot.ToString() + "\n");
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":MODule:ID?");
                                readtemp = this.ReadString();
                                if (readtemp == slot.ToString() + "\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                flag = true;
                                Log.SaveLogToTxt("MP1800 ED slot is" + slot);

                            }
                            else
                            {
                                Log.SaveLogToTxt("MP1800 ED set slot wrong");
                            }
                        }

                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private bool ConfigureChannel(int channel, int syn = 0)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("MP1800 ED channel is" + channel);
                        return this.WriteString(":INTerface:ID " + channel.ToString());

                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":INTerface:ID " + channel.ToString());
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":INTerface:ID?");
                                readtemp = this.ReadString();
                                if (readtemp == channel.ToString() + "\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("MP1800 ED channel is" + channel);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set MP1800 ED channel wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataInterface"></param>
        /// <returns></returns>
        private bool ConfigureDataInputInterface(byte dataInterface, int syn = 0)//0=SingleEnd,1=Diff 50ohm,2=Diff 100ohm;
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                string DataInterface;
                string returninterface = "";
                switch (dataInterface)
                {
                    case 0:
                        DataInterface = "SINGle";
                        returninterface = "SING\n";
                        break;
                    case 1:
                        DataInterface = "DIF50ohm";
                        returninterface = "DIF50\n";
                        break;
                    case 2:
                        DataInterface = "DIF100ohm";
                        returninterface = "DIF100\n";
                        break;
                    default:
                        DataInterface = "SINGle";
                        returninterface = "SING\n";
                        break;
                }

                try
                {

                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("DataInputInterface is" + DataInterface);
                        return this.WriteString(":INPut:DATA:INTerface " + DataInterface);
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":INPut:DATA:INTerface " + DataInterface);
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":INPut:DATA:INTerface?");
                                readtemp = this.ReadString();
                                if (readtemp == returninterface)
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("DataInputInterface is" + DataInterface);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set DataInputInterface wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }
        
        private bool ConfigurePatternType(byte patternType, int syn = 0)//0=PRBS,1=Zero Subsitution,2=Data,3=Alternate,4=Mixed Data,5=Sequense
        {//3 5 not used
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                string PatternType;
                switch (patternType)
                {
                    case 0:
                        PatternType = "PRBS";
                        break;
                    case 1:
                        PatternType = "ZSUB";
                        break;
                    case 2:
                        PatternType = "DATA";
                        break;
                    case 3:
                        PatternType = "PRBS";//"ALT";
                        break;
                    case 4:
                        PatternType = "MIXD";
                        break;
                    case 5:
                        PatternType = "PRBS";//"SEQuence";
                        break;
                    default:
                        PatternType = "PRBS";
                        break;
                }
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("PatternType is" + PatternType);
                        return this.WriteString(":SENSe:PATTern:TYPE " + PatternType);
                    }
                    else
                    {

                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":SENSe:PATTern:TYPE " + PatternType);
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":SENSe:PATTern:TYPE?");
                                readtemp = this.ReadString();
                                if (readtemp == PatternType + "\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("PatternType is" + PatternType);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set PatternType wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private bool ConfigurePrbsLength(byte prbsLength, int syn = 0)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("prbsLength is" + prbsLength);
                        return this.WriteString(":SENSe:PATTern:PRBS:LENGth " + prbsLength.ToString());

                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":SENSe:PATTern:PRBS:LENGth " + prbsLength.ToString());
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":SENSe:PATTern:PRBS:LENGth?");
                                readtemp = this.ReadString();
                                if (readtemp == prbsLength.ToString() + "\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("prbsLength is" + prbsLength);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set prbsLength wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private bool configureusertype(string filename)//patternType=2,选择DATA,user pattern
        {
            lock (syncRoot)
            {
                bool flag = false;
                string AA = @filename;
                flag = this.WriteString(":SYSTem:MMEMory:PATTern:RECall " + "\"" + AA + "\"" + ",BIN");
                // Thread.Sleep(1000);
                return flag;
            }
        }

        private bool ConfigureErrorResultZoomSwitch(byte ZoomSwitch, int syn = 0)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                string strSwitch;
                switch (ZoomSwitch)
                {
                    case 0:
                        strSwitch = "OFF";
                        break;
                    case 1:
                        strSwitch = "ON";
                        break;
                    default:
                        strSwitch = "OFF";
                        break;
                }
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("ErrorResultZoomSwitch is" + strSwitch);
                        return this.WriteString(":DISPlay:RESult:ZOOM " + strSwitch);
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":DISPlay:RESult:ZOOM " + strSwitch);
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":DISPlay:RESult:ZOOM?");
                                readtemp = this.ReadString();
                                if (readtemp == ZoomSwitch.ToString() + "\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("ErrorResultZoomSwitch is" + strSwitch);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set ErrorResultZoomSwitch wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private bool ConfigurePolarity(bool polarity)// 它没有通道好的选择,必须依赖别人修改通道号
        {
            // :SOURce:PATTern:LOGic NEGative
            lock (syncRoot)
            {
                string StrPolarity;

                if (polarity)
                {
                    StrPolarity = "POS";
                }
                else
                {
                    StrPolarity = "NEG";
                }
                this.WriteString(":SOURce:PATTern:LOGic " + StrPolarity);
                Thread.Sleep(200);
                this.WriteString(":SOURce:PATTern:LOGic?");
                Log.SaveLogToTxt("debug");
                string Str = this.ReadString();//it returns "failed", //need modify
                if (Str.Contains(StrPolarity))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool AutoAdjust()
        {
            lock (syncRoot)
            {
                this.WriteString(":SENSe:MEASure:AADJust32:STATe?");
                string Str = this.ReadString();
                if (Str != "1\n")
                {
                    for (int i = 0; i < 3; i++)
                    {
                        this.WriteString(":SYSTem:CFUNction AADJ32");  //open autosearch interface
                        this.WriteString(":SENSe:MEASure:AADJust32:SLASet");//set all
                        Thread.Sleep(200);
                        this.WriteString(":SENSe:MEASure:AADJust32:STARt");//start
                        this.WriteString(":SYSTem:CFUNction OFF");  //close autosearch interface
                        Thread.Sleep(200);
                        this.WriteString(":SENSe:MEASure:AADJust32:STATe?");
                        Str = this.ReadString();
                        if (Str.Contains("1")) return true;
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private bool ConfigureEdGatingMode(byte gatingMode, int syn = 0)//0=REPeat,1=UNTimed,2=SINGle
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                string strGatingMode;
                switch (gatingMode)
                {
                    case 0:
                        strGatingMode = "REP";
                        break;
                    case 1:
                        strGatingMode = "UNT";
                        break;
                    case 2:
                        strGatingMode = "SING";
                        break;
                    default:
                        strGatingMode = "REP";
                        break;
                }
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("EdGatingMode is" + strGatingMode);
                        return this.WriteString(":SENSe:MEASure:EALarm:MODE " + strGatingMode);
                    }
                    else
                    {

                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":SENSe:MEASure:EALarm:MODE " + strGatingMode);
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":SENSe:MEASure:EALarm:MODE?");
                                readtemp = this.ReadString();
                                if (readtemp == strGatingMode + "\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("EdGatingMode is" + strGatingMode);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set EdGatingMode wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private bool ConfigureEdGatingUnit(byte gatingUnit, int syn = 0)//0=TIME,1=BLOCk,2=CLOCk,3=ERRor
        {//无block回读值？
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                string strGatingUnit;
                switch (gatingUnit)
                {
                    case 0:
                        strGatingUnit = "TIME";
                        break;
                    case 1:
                        strGatingUnit = "BLOCk";
                        break;
                    case 2:
                        strGatingUnit = "CLOC";
                        break;
                    case 3:
                        strGatingUnit = "ERR";
                        break;
                    default:
                        strGatingUnit = "TIME";
                        break;
                }
                try
                {
                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("EdGatingUnit is" + strGatingUnit);
                        return this.WriteString(":SENSe:MEASure:EALarm:UNIT " + strGatingUnit);
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":SENSe:MEASure:EALarm:UNIT " + strGatingUnit);
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":SENSe:MEASure:EALarm:UNIT?");
                                readtemp = this.ReadString();
                                if (readtemp == strGatingUnit + "\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("EdGatingUnit is" + strGatingUnit);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set EdGatingUnit wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private bool ConfigureEdGatingPeriod(int gatingTime, int syn = 0)
        {
            lock (syncRoot)
            {
                bool flag = false;
                bool flag1 = false;
                int k = 0;
                string readtemp = "";
                string time = "0,0,0,0";
                int day, hour, min, sec;
                day = gatingTime / 86400;
                hour = (gatingTime - day * 86400) / 3600;
                min = (gatingTime - day * 86400 - hour * 3600) / 60;
                sec = gatingTime - day * 86400 - hour * 3600 - min * 60;
                time = day.ToString() + "," + hour.ToString() + "," + min.ToString() + "," + sec.ToString();
                try
                {

                    if (syn == 0)
                    {
                        Log.SaveLogToTxt("EdGatingPeriod is" + time);
                        return this.WriteString(":SENSe:MEASure:EALarm:PERiod " + time);
                    }
                    else
                    {

                        for (int i = 0; i < 3; i++)
                        {
                            flag1 = this.WriteString(":SENSe:MEASure:EALarm:PERiod " + time);
                            if (flag1 == true)
                                break;
                        }
                        if (flag1 == true)
                        {
                            for (k = 0; k < 3; k++)
                            {

                                this.WriteString(":SENSe:MEASure:EALarm:PERiod?");
                                readtemp = this.ReadString();
                                if (readtemp == time + "\n")
                                    break;
                            }
                            if (k <= 3)
                            {
                                Log.SaveLogToTxt("EdGatingPeriod is" + time);
                                flag = true;
                            }
                            else
                            {
                                Log.SaveLogToTxt("set EdGatingPeriod wrong");

                            }

                        }
                        return flag;
                    }
                }
                catch (Exception ex)
                {
                    Log.SaveLogToTxt(ex.ToString());
                    return false;
                }
            }
        }

        private double QureyEdErrorCount()
        {
            lock (syncRoot)
            {
                string Ber = "-1";
                double ber = 0;
                try
                {
                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {

                            this.WriteString(":CALCulate:DATA:EALarm? \"CURRent:EC:TOTal\"");
                            Thread.Sleep(500);
                            Ber = this.ReadString(100).Replace("\"", "").Replace("\n", "");
                        }
                        catch (Exception ex)
                        {

                            Log.SaveLogToTxt(ex.Message);
                            this.WriteString("*CLS");
                            Thread.Sleep(2000);

                        }
                        if (Ber.Contains("---") || Ber == "" || Ber == null)
                        {
                            ber = 9.999E+17;

                        }
                        else
                        {
                            if (Ber == "-1")
                            {
                                this.WriteString("*CLS");
                                Thread.Sleep(2000);

                            }
                            else
                            {


                                break;
                            }
                        }
                        Thread.Sleep(500);
                    }
                    ////MyIO.WriteString(":CALCulate:DATA:EALarm? \"CURRent:EC:TOTal\"");
                    //////Thread.Sleep(50);
                    ////Ber = MyIO.ReadString(100).Replace("\"", "").Replace("\n", "");



                    if (Ber.Contains("---") || Ber == "" || Ber == null)
                    {
                        ber = 9.999E+17;
                        Log.SaveLogToTxt("ErrorCount is" + ber);
                    }
                    else
                    {
                        ber = Convert.ToDouble(Ber);
                        Log.SaveLogToTxt("ErrorCount is" + ber);
                    }
                }
                catch (Exception error)
                {
                    ber = 9.999E+17;
                    Log.SaveLogToTxt(error.ToString());
                    // return ber;
                }
                finally
                {


                }
                return ber;
            }
        }
    }
}
