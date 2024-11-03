using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Ensamblador{

     public class Msg
    {
        private string texto;
        private string nombre;
        private bool salto = false;

        public Msg(string texto, string nombre)
        {
            this.texto = texto;
            this.nombre = nombre;
        }
        public Msg(string texto, string nombre, bool salto)
        {
            this.texto = texto;
            this.nombre = nombre;
            this.salto = salto;
        }
        public string Texto{
            set  => texto=value;
            get => texto;
        }
        public string Nombre{
            set  => nombre=value;
            get => nombre;
        }
        public bool Salto{
            set => salto=value;
            get => salto;
        }
    }
}