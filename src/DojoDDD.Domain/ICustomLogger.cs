using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;

namespace DojoDDD.Domain
{
    public interface ICustomLogger
    {
        void Log(string mensagem, EventLevel nivel = EventLevel.Informational);
    }
}
