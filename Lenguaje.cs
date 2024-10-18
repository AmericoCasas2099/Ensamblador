using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Net.Http.Headers;

using Ensamblador;
/*
    1. Completar asignación               --> 
    2. Console.Write & Console.WriteLine  --> 
    3. Console.Read & Console.ReadLine    --> 
    4. Considerar else en el If           -->
    5. Programar el while                 -->
    6. Programar el for                   -->
*/

namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        private List<Variable> listaVariables;
        private Stack<float> S;
        private Variable.TipoDato tipoDatoExpresion;

        private int cIFs,cDoes;
        public Lenguaje()
        {
            log.WriteLine("Analizador Sintactico");
            asm.WriteLine("Analizador Sintactico");
            asm.WriteLine("Analizador Semántico");
            listaVariables = new List<Variable>();
            S = new Stack<float>();
            cIFs=cDoes=1;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            log.WriteLine("Analizador Sintactico");
            listaVariables = new List<Variable>();
            S = new Stack<float>();
            cIFs=cDoes=1;
            //Por cada if, debe genera una etiqueta
        }
        public void Programa()
        {
            if (Contenido == "using")
            {
                Librerias();
            }
            Main();
            imprimeVariables();
        }
        private void Librerias()
        {
            match("using");
            listaLibrerias();
            match(";");
            if (Contenido == "using")
            {
                Librerias();
            }
        }
        // ListaLibrerias -> identificador (.ListaLibrerias)?
        private void listaLibrerias()
        {
            match(Tipos.Identificador);
            if (Contenido == ".")
            {
                match(".");
                listaLibrerias();
            }
        }
        Variable.TipoDato getTipo(string TipoDato)
        {
            Variable.TipoDato tipo = Variable.TipoDato.Char;
            switch (TipoDato)
            {
                case "int": tipo = Variable.TipoDato.Int; break;
                case "float": tipo = Variable.TipoDato.Float; break;
            }
            return tipo;
        }
        // Variables -> tipo_dato Lista_identificadores;
        private void Variables()
        {
            Variable.TipoDato tipo = getTipo(Contenido);
            match(Tipos.TipoDato);
            listaIdentificadores(tipo);
            match(";");
        }
        private void imprimeVariables()
        {
            log.WriteLine("Lista de variables");
            asm.WriteLine("\nsegment .data");
            foreach (Variable v in listaVariables)
            {
               // log.WriteLine(v.getNombre() + " (" + v.getTipo() + ") = " + v.getValor());
                if (v.Tipo == Variable.TipoDato.Char)
                {
					asm.WriteLine("\t" + v.Nombre+ " db 0");
				}
                else if (v.Tipo == Variable.TipoDato.Int)
                {
					asm.WriteLine("\t" + v.Nombre + " dd 0");
				}
				else
                {
					asm.WriteLine("\t" + v.Nombre + " dq 0 ");
				}
            }
        }
        // ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void listaIdentificadores(Variable.TipoDato t)
        {
            //Requerimiento 2
            if (listaVariables.Exists(v => v.Nombre == Contenido))
            {
                throw new Error("Semantico: la variable \"" + Contenido + "\" ya ha sido declarada; linea: " + linea, log);
            }
            listaVariables.Add(new Variable(Contenido, t));
            var variable = listaVariables.Find(delegate (Variable x) { return x.Nombre == Contenido; });
            /*if(variable == null){
                throw new Error("La variable \""+ Contenido + "\" no ha sido declarada; linea:"+linea,log);
            }   */
            match(Tipos.Identificador);
            if (Contenido == "=")
            {
                match("=");
                Expresion();
                float x = S.Pop();
                if (analisisSemantico(variable, x))
                {
                    variable.Valor = x;
                    S.Push(x);
                    log.WriteLine(variable.Nombre + " = " + x);
                }
                else
                {
                    throw new Error("Semántico: No se puede asignar un " + tipoDatoExpresion + "a un" + variable.Tipo + "; linea: ", log);
                }

            }
            if (Contenido == ",")
            {
                match(",");
                listaIdentificadores(t);
            }
        }
        // BloqueInstrucciones -> { listaIntrucciones? }
        private void bloqueInstrucciones(bool ejecutar)
        {
            match("{");
            if (Contenido != "}")
            {
                listaIntrucciones(ejecutar);
            }
            match("}");
        }
        // ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void listaIntrucciones(bool ejecutar)
        {
            Instruccion(ejecutar);
            if (Contenido != "}")
            {
                listaIntrucciones(ejecutar);
            }
        }
        // Instruccion -> Console | If | While | do | For | Variables | Asignacion
        private void Instruccion(bool ejecutar)
        {
            if (Contenido == "Console")
            {
                console(ejecutar);
            }
            else if (Contenido == "if")
            {
                If(ejecutar);
            }
            else if (Contenido == "while")
            {
                While(ejecutar);
            }
            else if (Contenido == "do")
            {
                Do(ejecutar);
            }
            else if (Contenido == "for")
            {
                For(ejecutar);
            }
            else if (Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion(ejecutar);
                match(";");
            }
        }
        // Asignacion -> Identificador = Expresion;
        private void Asignacion(bool ejecutar)
        {
            string variable = Contenido;
            if (!listaVariables.Exists(v => v.Nombre == variable))
            {
                throw new Error("Semantico: la variable \"" + variable + "\" no ha sido declarada; línea: " + linea, log);
            }
            match(Tipos.Identificador);
            asm.WriteLine("; Asignacion a " + variable);
            var v = listaVariables.Find(delegate (Variable x) { return x.Nombre == variable; });
            float nuevoValor = v.Valor;

            tipoDatoExpresion = Variable.TipoDato.Char;

            if (Contenido == "=")
            {
                match("=");
                if (Contenido == "Console")
                {
                    match("Console");
                    match(".");
                    if (Contenido == "Read")
                    {
                        match("Read");
                        match("(");
                        if (ejecutar)
                        {
                            float valor = Console.Read();
                        }
                        // 8
                    }
                    else
                    {
                        match("ReadLine");
                        match("(");
                        string entrada = ("" + Console.ReadLine());
                        if (!float.TryParse(entrada, out nuevoValor))
                        {
                            throw new Exception("El valor introducido debe ser un número");
                        }
                        // 8
                    }

                    match(")");
                    //match(";");
                }
                else
                {
                    Expresion();
                    nuevoValor = S.Pop();
                    asm.WriteLine("\tpop ax");
                    asm.WriteLine("\tmov "+variable+", ax");
                
                }
            }
            else if (Contenido == "++")
            {
                match("++");
                asm.WriteLine("\tinc "+variable);
                nuevoValor++;
            }
            else if (Contenido == "--")
            {
                match("--");
                asm.WriteLine("\tdec "+variable);
                nuevoValor--;
            }
            else if (Contenido == "+=")
            {
                match("+=");
                Expresion();
                nuevoValor += S.Pop();
                asm.WriteLine("\tpop ax");
            }
            else if (Contenido == "-=")
            {
                match("-=");
                Expresion();
                nuevoValor -= S.Pop();
                asm.WriteLine("\tpop ax");
            }
            else if (Contenido == "*=")
            {
                match("*=");
                Expresion();
                nuevoValor *= S.Pop();
                asm.WriteLine("\tpop ax");
            }
            else if (Contenido == "/=")
            {
                match("/=");
                Expresion();
                nuevoValor /= S.Pop();
                asm.WriteLine("\tpop ax");
            }
            else
            {
                match("%=");
                Expresion();
                nuevoValor %= S.Pop();
                asm.WriteLine("\tpop ax");
            }
            // match(";");
            if (analisisSemantico(v, nuevoValor))
            {
                if (ejecutar)
                    v.Valor = nuevoValor;
            }
            else
            {
                // tipoDatoExpresion = 
                throw new Error("Semantico, no puedo asignar un " + tipoDatoExpresion +
                                " a un " + v.Tipo + "; linea: " + linea, log);
            }
            log.WriteLine(variable + " = " + nuevoValor);
        }
        private Variable.TipoDato valorToTipo(float valor)
        {
            if (valor % 1 != 0)
            {
                return Variable.TipoDato.Float;
            }
            else if (valor <= 255)
            {
                return Variable.TipoDato.Char;
            }
            else if (valor <= 65535)
            {
                return Variable.TipoDato.Int;
            }
            return Variable.TipoDato.Float;
        }
        bool analisisSemantico(Variable v, float valor)
        {
            if (tipoDatoExpresion > v.Tipo)
            {
                return false;
            }
            else if (valor % 1 == 0)
            {
                if (v.Tipo == Variable.TipoDato.Char)
                {
                    if (valor <= 255)
                        return true;
                }
                else if (v.Tipo == Variable.TipoDato.Int)
                {
                    if (valor <= 65535)
                        return true;
                }
                return false;
            }
            else
            {
                if (v.Tipo == Variable.TipoDato.Char ||
                    v.Tipo == Variable.TipoDato.Int)
                    return false;
            }
            return true;
        }
        private void If(bool ejecutar)
        {
            asm.WriteLine("; if"+cIFs);
            string etiqueta = "; _if"+ cIFs++;
            match("if");
            match("(");
            bool resultado = Condicion();
            match(")");
            if (Contenido == "{")
            {
                bloqueInstrucciones(resultado && ejecutar);
            }
            else
            {
                Instruccion(resultado && ejecutar);
            }
            if (Contenido == "else")
            {
                match("else");
                if (Contenido == "{")
                {
                    bloqueInstrucciones(!resultado && ejecutar);
                }
                else
                {
                    Instruccion(!resultado && ejecutar);
                }
            }
            asm.WriteLine(etiqueta + ":");
            //Generar etiqueta para que la condición diga a que etiqueta saltar
        }
        private bool Condicion()
        {
            Expresion(); // E1
            string etiqueta="";
            string operador = Contenido;
            match(Tipos.OpRelacional);
            Expresion(); // E2
            float R2 = S.Pop();
            asm.WriteLine("\tpop ax");
            float R1 = S.Pop();
            asm.WriteLine("\tpop bx");
            asm.WriteLine("\tcmp ax, bx");
            switch (operador)
            {
                case ">": return R1 > R2;
                case ">=": return R1 >= R2;
                case "<": return R1 < R2;
                case "<=": return R1 <= R2;
                case "==": asm.WriteLine("\tjne "+etiqueta);
                 return R1 == R2;
                default:  asm.WriteLine("\tje"+etiqueta);
                return R1 != R2;
            }
        }
        // While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While(bool ejecutar)
        {
            int cTemp = caracter - 3;
            int lTemp = linea;
            bool resultado = false;

            match("while");
            match("(");
            resultado = Condicion() && ejecutar;
            match(")");
            while (resultado)
            {
                if (Contenido == "{")
                {
                    bloqueInstrucciones(ejecutar);
                }
                else
                {
                    Instruccion(ejecutar);
                }

                resultado = Condicion() && ejecutar;
                if (resultado)
                {
                    caracter = cTemp;
                    linea = lTemp;
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(cTemp, SeekOrigin.Begin);
                    nextToken();
                }
            }//

        }

        private void Do(bool ejecutar)
        {

            string etiqueta = "Do"+cDoes++;

            int cTemp = caracter - 3;
            int lTemp = linea;
            bool resultado = false;
            
            do
            {
                match("do");
                if (Contenido == "{")
                {
                    bloqueInstrucciones(ejecutar);
                }
                else
                {
                    Instruccion(ejecutar);
                }
                match("while");
                match("(");
                resultado = Condicion() && ejecutar;
                match(")");
                match(";");
                if (resultado)
                {
                    caracter = cTemp;
                    linea = lTemp;
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(cTemp, SeekOrigin.Begin);
                    nextToken();
                }
            } while (resultado);
        }
        // For -> for(Asignacion Condicion; Incremento) 
        //          BloqueInstrucciones | Intruccion
        private void For(bool ejecutar)
        {
            int cTemp = caracter - 3;
            int lTemp = linea;
            bool resultado = false;

            match("for");
            match("(");
            var varF1 = listaVariables.Find(v => v.Nombre == Contenido);
            if (varF1 == null)
            {
                throw new Exception("La variable dentro del for no ha sido declarada");
            }
            Asignacion(ejecutar);
            match(";");
            resultado = Condicion() && ejecutar;
            match(";");
            var varF2 = listaVariables.Find(v => v.Nombre == Contenido);
            if (varF2 == null)
            {
                throw new Exception("La variable dentro del for no ha sido declarada");
            }
            /*if(varF2.Nombre != varF1.Nombre){
                throw new Exception("No puede haber más de una variable dentro del For");
            }*/
            Asignacion(ejecutar);
            match(")");
            match("{");
            while (resultado)
            {
                if (Contenido == "{")
                {
                    bloqueInstrucciones(ejecutar);
                }
                else
                {
                    Instruccion(ejecutar);
                }
                resultado = Condicion() && ejecutar;
                if (resultado)
                {
                    caracter = cTemp;
                    linea = lTemp;
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(cTemp, SeekOrigin.Begin);
                    nextToken();
                }

            }

        }

        private void console(bool ejecutar)
        {
            match("Console");
            match(".");
            bool b = false;
            string texto;
            if (Contenido == "WriteLine")
            {
                b = true;
                match("WriteLine");
            }
            else if (Contenido == "Write")
            {
                match("Write");
            }
            match("(");
            if (Clasificacion == Tipos.Cadena)
            {
                if (ejecutar)
                {
                    texto = Contenido;
                    texto = texto.Replace("\"", "");
                    match(Tipos.Cadena);
                    if (b)
                    {
                        if (Contenido == "+")
                        {
                            string textoT = listaConcatenacion();
                            if (ejecutar)
                            {
                                texto = texto + textoT;
                                Console.WriteLine(texto.Replace('"', ' ').TrimEnd());
                            }
                        }
                        else
                        {
                            if (ejecutar)
                            {
                                Console.WriteLine(texto.Replace('"', ' ').TrimEnd());
                            }
                        }
                    }
                    else
                    {
                        if (Contenido == "+")
                        {
                            string textoT = listaConcatenacion();
                            if (ejecutar)
                            {
                                texto = texto + textoT;
                                Console.Write(texto.Replace('"', ' ').TrimEnd());
                            }
                        }
                        else
                        {
                            if (ejecutar)
                            {
                                Console.Write(texto.Replace('"', ' ').TrimEnd());
                            }
                        }
                    }


                }

            }
            else{
                float resultado;
                var v = listaVariables.Find(variable => variable.Nombre == Contenido);
                resultado = v.Valor;
                match(Tipos.Identificador);
                if (b)
                {
                    if (Contenido == "+")
                    {
                        String temp = listaConcatenacion();
                        if (ejecutar)
                            Console.WriteLine(resultado + temp);
                    }
                    else
                    {
                        if (ejecutar)
                            Console.WriteLine(resultado);
                    }
                }
                else
                {
                    if (Contenido == "+")
                    {
                        String temp = listaConcatenacion();
                        if (ejecutar)
                            Console.Write(resultado + temp);
                    }
                    else
                    {
                        if (ejecutar)
                            Console.Write(resultado);
                    }
                }

            }
            match(")");
            match(";");
        }
        string listaConcatenacion()
        {
            String texto = "";
            string resultado = "";
            match("+");
            if (Clasificacion == Tipos.Identificador)
            {
                if (!listaVariables.Exists(v => v.Nombre == Contenido))
                {
                throw new Error("Semantico: la variable \""+Contenido+ "\" no ha sido declarada; linea: "+linea,log);
                }
                var v = listaVariables.Find(variable => variable.Nombre == Contenido);
                resultado = v.Valor.ToString();
                match(Tipos.Identificador);
            }
            if (Clasificacion == Tipos.Cadena)
            {
                texto = Contenido;
                texto = texto.Replace("\"","");
                resultado += texto;
                match(Tipos.Cadena);
            }
            if (Contenido == "+")
            {
                resultado += listaConcatenacion();
            }
            return resultado;
        }

        // Main      -> static void Main(string[] args) BloqueInstrucciones 
        private void Main()
        {
            
            match("static");
            match("void");
            match("Main");
            match("(");
            match("string");
            match("[");
            match("]");
            match("args");
            match(")");
            bloqueInstrucciones(true);
        }
        // Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        // MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (Clasificacion == Tipos.OpTermino)
            {
                string operador = Contenido;
                match(Tipos.OpTermino);
                Termino();
                float R1 = S.Pop();
                asm.WriteLine("\tpop bx");
                float R2 = S.Pop();
                asm.WriteLine("\tpop ax");
                switch (operador)
                {
                    case "+":
                        S.Push(R2 + R1);
                        asm.WriteLine("\tadd ax, bx");
                        asm.WriteLine("\tpush ax");
                        tipoDatoExpresion = valorToTipo(R2 + R1);
                        break;
                    case "-":
                        S.Push(R2 - R1);
                        asm.WriteLine("\tsub ax, bx");
                        asm.WriteLine("\tpush ax");
                        tipoDatoExpresion = valorToTipo(R2 - R1);
                        break;
                }
            }
        }
        // Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        // PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (Clasificacion == Tipos.OpFactor)
            {
                string operador = Contenido;
                match(Tipos.OpFactor);
                Factor();
                float R1 = S.Pop();
                asm.WriteLine("\tpop bx");
                float R2 = S.Pop();
                asm.WriteLine("\tpop ax");
                switch (operador)
                {
                    case "*": S.Push(R2 * R1);
                    asm.WriteLine("\tmul bx");
                    asm.WriteLine("\tpush ax"); break;
                    case "/": S.Push(R2 / R1);
                    asm.WriteLine("\tdiv bx");
                    asm.WriteLine("\tpush ax"); break;
                    case "%": S.Push(R2 % R1);
                    asm.WriteLine("\tdiv bx");
                    asm.WriteLine("\tpush dx"); break;
                }
            }
        }
        /* private void imprimeStack()
         {
             log.WriteLine("Stack:");

             foreach (float e in S.Reverse())
             {
                 log.Write(e + " ");
             }
             log.WriteLine();
         }*/
        private void Factor()
        {
            if (Clasificacion == Tipos.Numero)
            {
                S.Push(float.Parse(Contenido));
                asm.WriteLine("\tmov ax, "+Contenido);
                asm.WriteLine("\tpush");
                if (tipoDatoExpresion < valorToTipo(float.Parse(Contenido)))
                {
                    tipoDatoExpresion = valorToTipo(float.Parse(Contenido));
                }
                match(Tipos.Numero);
            }
            else if (Clasificacion == Tipos.Identificador)
            {
                var v = listaVariables.Find(delegate (Variable x) { return x.Nombre == Contenido; });
                S.Push(v.Valor);
                asm.WriteLine("\tmov ax, "+Contenido);
                asm.WriteLine("\tpush");
                if (tipoDatoExpresion < v.Tipo)
                {
                    tipoDatoExpresion = v.Tipo;
                }
                match(Tipos.Identificador);
            }
            else
            {
                bool huboCast = false;
                Variable.TipoDato aCastear = Variable.TipoDato.Char;
                match("(");
                if (Clasificacion == Tipos.TipoDato)
                {
                    huboCast = true;
                    aCastear = getTipo(Contenido);
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion();
                match(")");
                if (huboCast && aCastear != Variable.TipoDato.Float)
                {
                    tipoDatoExpresion = aCastear;
                    float valor = S.Pop();
                    asm.WriteLine("\tpop ax");
                    if (aCastear == Variable.TipoDato.Char)
                    {
                        valor %= 256;
                    }
                    else
                    {
                        valor %= 65536;
                    }
                    S.Push(valor);
                    asm.WriteLine("\tmov ax, "+valor);
                    asm.WriteLine("\tpush dx");
                }
            }
        }
    }
}