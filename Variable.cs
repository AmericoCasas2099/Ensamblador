using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Ensamblador
{
    public class Variable
    {
        public enum TipoDato
        {
            Char,Int,Float
        }
        private string nombre="";
        private TipoDato tipo;
        private float valor;
        private string texto ="";
        private bool salto = false;

        public Variable(string nombre, TipoDato tipo)
        {
            this.nombre = nombre;
            this.tipo = tipo;
            this.valor = 0;
        }
        
        
        public float Valor{
            get;
            set;
        }

        public string Nombre{
            get => nombre;
        }
        public TipoDato Tipo{
            get=>tipo;
        }
        
        public string Texto{
            set  => texto=value;
            get => texto;
        }
        public bool Salto{
            set => salto=value;
            get => salto;
        }
    }
}