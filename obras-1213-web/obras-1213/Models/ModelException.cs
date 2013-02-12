using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace obras_1213.Models
{
    public class ModelException : Exception
    {
        public ModelException(string message)
            : base(message)
        {
        }

        public ModelException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}