using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NichTest
{
    public class PPG: Equipment
    {
        protected string rate;
        protected byte totalChannels;
        protected byte prbsLength;//PRBS码型长度:31,7
        protected byte patternType;//PPG码型选择（0=PRBS,1=Zero Subsitution,2=Data,3=Alternate,4=Mixed Data,5=Sequense）

        public virtual bool ConfigurePatternType(byte patternType, int syn = 0) { return true; }//0=PRBS,1=ZSUBstitution,2=DATA,3=ALT,4=MIXData,5=MIXalt,6=SEQuence
        public virtual bool ConfigurePrbsLength(byte prbsLength, int syn = 0) { return true; }
        public virtual bool DataPattern_DataLength(string length) { return true; }
        public virtual bool DataPattern_SetPatternData(string start, string end, string data) { return true; }
        public virtual bool RecallPrbsLength() { return true; }
        public virtual bool ConfigureOTxPolarity(bool polarity) { return true; }
    }
}
