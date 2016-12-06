using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NichTest
{
    public interface IEquipment
    {
        /// <summary>
        /// Initial epuipment
        /// </summary>
        /// <param name="inPara">input parameters for eupipment</param>
        /// <param name="syn">synchronization or asynchronization</param>
        /// <returns>is successful</returns>
        bool Initial(Dictionary<string, string> inPara, int syn = 0);

        /// <summary>
        /// config equipment
        /// </summary>
        /// <param name="syn">synchronization or asynchronization</param>
        /// <returns></returns>
        bool Configure(int syn = 0);

        /// <summary>
        /// power on or shut down equipment
        /// </summary>
        /// <param name="isON">power on?</param>
        /// <param name="syn">synchronization or asynchronization</param>
        /// <returns>is successful</returns>
        bool OutPutSwitch(bool isON, int syn = 0);

        /// <summary>
        /// config the offset
        /// </summary>
        /// <param name="channel">the channel of equipment</param>
        /// <param name="offset">offset value</param>
        /// <param name="syn">synchronization or asynchronization</param>
        /// <returns>is successful</returns>
        bool ConfigOffset(int channel, double offset, int syn = 0);

        /// <summary>
        /// config the offset for powersupply
        /// </summary>
        /// <param name="channel">the channel of equipment</param>
        /// <param name="offset_VCC">offset of VCC</param>
        /// <param name="offset_ICC">offset of ICC</param>
        /// <param name="syn">synchronization or asynchronization</param>
        /// <returns>is successful</returns>
        bool ConfigOffset(int channel, double offset_VCC, double offset_ICC, int syn = 0);

        /// <summary>
        /// change the output channel of equipment
        /// </summary>
        /// <param name="channel">the channel of equipment</param>
        /// <param name="syn">synchronization or asynchronization</param>
        /// <returns>is successful</returns>
        bool ChangeChannel(int channel, int syn = 0);
    }
}
