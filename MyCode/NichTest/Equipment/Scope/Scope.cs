using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NichTest
{
    public class Scope : Equipment
    {
        protected string configFilePath;
        protected byte FilterSwitch;
        protected byte triggerSource;
        protected byte opticalSlot;
        protected byte elecSlot;
        protected byte opticalAttSwitch;
        protected byte erFactorSwitch;
        protected double erFactor;
        protected string FlexOptChannel;
        protected string FlexElecChannel;
        protected byte precisionTimebaseModuleSlot;
        protected byte precisionTimebaseSynchMethod;
        protected byte rapidEyeSwitch;
        protected byte Threshold;
        protected byte reference;
        protected byte marginType;
        protected byte marginHitType;
        protected byte acqLimitType;
        protected double precisionTimebaseRefClk;
        protected double marginHitRatio;
        protected string opticalMaskName;
        protected string opticalMaskName2;
        protected string elecMaskName;
        protected string opticalEyeSavePath;
        protected string elecEyeSavePath;
        protected int marginHitCount;
        protected int acqLimitNumber;
        protected byte FlexTriggerBwlimit;
        protected byte FlexDcaWavelength;
        protected double FlexDcaDataRate;
        protected double FlexFilterFreq;
        protected double FlexDcaAtt;
        protected double FlexScale;
        protected double FlexOffset;
        protected int flexsetscaledelay;
        protected byte DiffSwitch; //0 off,1 on
        protected byte BandWidth;//1,2,3,4

        public virtual bool SetMaskAlignMethod(byte method, int syn = 0) { return true; }
        public virtual bool SetMode(byte Mode, int syn = 0) { return true; }
        public virtual bool MaskONOFF(bool MaskOn, int syn = 0) { return true; }
        public virtual bool SetRunTilOff(int syn = 0) { return true; }
        public virtual bool AutoScale(int syn = 0) { return true; }
        public virtual bool RunStop(bool run) { return true; }
        public virtual bool ClearDisplay() { return true; }
        public virtual bool OpenOpticalChannel(bool Switch, int syn = 0) { return true; }
        public virtual bool OpticalEyeTest(ref Dictionary<string, double> result, int syn = 0) { return false; }
        /// <summary>
        /// 模板余量测试方法设置
        /// </summary>
        /// <param name="marginOnOff"> 是否打开Margin,1=ON,0=OFF </param>
        /// <param name="marginAutoManul">手动测试Margin还是自动测试，1=自动，0=手动 </param>
        /// <param name="manualMarginPercent">手动测试Margin时，输入Margin值，自动测试时忽略该参数 </param>
        /// <param name="autoMarginType">自动测试Margin时，Margin类型选择，1=HitRatio,0=HitCount </param>
        /// <param name="hitRatio"> HitRatio的数值，例如1E-6 </param>
        /// <param name="hitCount">HitCount的数值，例如0 </param>
        /// <returns>结果</returns>
        public virtual bool MaskTestMarginSetup(byte marginOnOff, byte marginAutoManul, int manualMarginPercent, byte autoMarginType, double hitRatio, int hitCount, int syn = 0) { return true; }
        public virtual bool LoadMask() { return true; }
        public virtual bool EyeTuningDisplay(byte Switch, int syn = 0) { return true; }
        public virtual bool DisplayThreeEyes(int syn = 0) { return true; }
        public virtual bool DisplayEyeHeight() { return false; }
        public virtual bool DisplayEyeWidth() { return false; }
        public virtual bool DisplayCrossing() { return false; }
        public virtual bool DisplayJitter(byte jitterFormat) { return false; }//[20160419]Nate: add
        public virtual bool DisplayER() { return true; }
        public virtual bool DisplayPowerWatt() { return false; }
        public virtual bool DisplayPowerdBm() { return false; }
        public virtual double GetCrossing() { return 0.0; }
        public virtual double GetAveragePowerdBm() { return 0; }
    }
}
