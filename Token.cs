using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Ensamblador
{
    public class Token
    {
        public enum Tipos
        {
            Identificador, Numero, FinSentencia, OpTermino, OpFactor,
            OpLogico, OpRelacional, OpTernario, Asignacion, IncTermino,
            IncFactor, Cadena, Inicio, Fin, Caracter, TipoDato, Ciclo, 
            Condicion
        };
        private string contenido;
        private Tipos clasificacion;
        public Token()
        {
            contenido = "";
            Contenido = "";
        }
        
   
       public string Contenido{
        get;
        set;
        }
        public Tipos Clasificacion{
        get;
        set;
        }
}}