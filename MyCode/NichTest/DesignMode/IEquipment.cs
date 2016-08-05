using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NichTest
{
    public interface IEquipment
    {
        bool Initial(Dictionary<string, string> inPara, int syn = 0);

        bool Configure(int syn = 0);

        bool OutPutSwitch(bool isON, int syn = 0);

        bool ConfigOffset(int channel, double offset, int syn = 0);

        bool ChangeChannel(int channel, int syn = 0);
    }
}
