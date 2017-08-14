using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryLibrary
{
    public class MemoryException : Exception
    {
        const string innerMessage = "MemoryException";

        public MemoryException() : base(innerMessage)
        {
        }

        public MemoryException(string exMessage)
            :
            base( String.Format( "{0} - {1}",
                exMessage, innerMessage))
        { }

    }

}
