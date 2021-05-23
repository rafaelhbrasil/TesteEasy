using DojoDDD.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;

namespace DojoDDD.Api.Helpers
{
    /// <summary>
    /// Classe de log com saída para o console
    /// </summary>
    public class ConsoleLogger : ICustomLogger
    {
        /// <summary>
        /// Registra no Log a mensagem com o nível especificado
        /// </summary>
        /// <param name="mensagem">A mensagem do log</param>
        /// <param name="nivel">O nível da severidade do log</param>
        public void Log(string mensagem, EventLevel nivel = EventLevel.Informational)
        {
            var mensagemFormatada = $"[{DateTime.Now:s}] [{nivel}] {mensagem}";
            Console.WriteLine(mensagemFormatada);
#if DEBUG 
            Debug.WriteLine(mensagemFormatada);
#endif
        }
    }
}
