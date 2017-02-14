using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace NichTest
{
    interface ITest
    {
        /// <summary>
        /// test method of items.
        /// </summary>
        /// <param name="dut">the module for test</param>
        /// <param name="equipments">used equipmentes by test item</param>
        /// <param name="inPara">input parameters for test item</param>
        /// <returns>the result of test, is true or false</returns>
        Dictionary<string, double> BeginTest(DUT dut, Dictionary<string, IEquipment> equipments, Dictionary<string, string> inPara);

        /// <summary>
        /// Upload test data to server, or save them to lochost.
        /// </summary>
        /// <returns>the result is true or false</returns>
        bool SaveTestData();
    }
}
