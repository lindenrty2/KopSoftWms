using System;
using System.Collections.Generic;
using System.Text;
using YL.Utils.Pub;

namespace YL.Utils
{
    public class WMSException : Exception
    {
        private MessageItem _errorMessage = null;
        public MessageItem ErrorMessage { get { return _errorMessage; } }
        public WMSException(MessageItem errorMessage)
                : base($"{errorMessage.ToString()}")
        {
            _errorMessage = errorMessage;
        }
        public WMSException(MessageItem errorMessage,string additionMessage )
            : base ( $"{errorMessage.ToString()}[{additionMessage}]" )
        {
            _errorMessage = errorMessage;
        }

    }
}
