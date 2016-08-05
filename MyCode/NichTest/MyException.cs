using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NichTest
{
    class MyException: ApplicationException, ISerializable
    {
        private int id;

        public MyException()
        {

        }

        public MyException(string message)
            : base(message)
        {

        }

        public MyException(int exceptionID, string message)
            : base("error code #"+ exceptionID + ": "+ message)
        {
            id = exceptionID;
        }

        public MyException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public MyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        public int ID
        {
            set
            {
                this.id = value;
            }
            get
            {
                return this.id;
            }
        }
    }
}
