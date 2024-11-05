using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Net.Http.Headers;

/*
    1. Completar asignación               --> Listo
    2. Console.Write & Console.WriteLine  --> Listo
    3. Console.Read & Console.ReadLine    --> Listo
    4. Considerar else en el If           --> Listo
    5. Programar el while                 --> Listo
    6. Programar el for                   --> Listo
*/

namespace Ensamblador
{
    public class Lenguaje : Sintaxis
    {
        private List<Variable> listaVariables;
        private Variable.TipoDato tipoDatoExpresion;

        private List<Msg> listaMsg = new List<Msg>();

        private int cIFs, cDoes, cWhiles, cElses, cFor, cMsg;
        private bool esDoe = false;
        public Lenguaje()
        {
            log.WriteLine("Analizador Sintactico");
            asm.WriteLine("Analizador Sintactico");
            asm.WriteLine("Analizador Semántico");
            listaVariables = new List<Variable>();
            listaMsg = new List<Msg>();

            cIFs = cElses = cDoes = cWhiles = cFor = cMsg = 1;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            log.WriteLine("Analizador Sintactico");
            asm.WriteLine("; Analizador Sintactico");
            asm.WriteLine("; Analizador Semantico");
            listaVariables = new List<Variable>();
            listaMsg = new List<Msg>();
            cIFs = cElses = cDoes = cWhiles = cFor = cMsg = 1;
            //Por cada if, debe generar una etiqueta
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
            asm.WriteLine("\ntipo db " + '"' + "%d" + '"' + ", 0");
            foreach (Variable v in listaVariables)
            {
                log.WriteLine(v.Nombre + " (" + v.Tipo + ") = " + v.Valor);
                if (v.Tipo == Variable.TipoDato.Char)
                {
                    asm.WriteLine("\t" + v.Nombre + " db 0");
                }
                else if (v.Tipo == Variable.TipoDato.Int)
                {
                    asm.WriteLine("\t" + v.Nombre + " dd 0");
                }
                else
                {
                    asm.WriteLine("\t" + v.Nombre + " dw 0 ");
                }
            }
            foreach (Msg m in listaMsg)
            {            
                    //asm.WriteLine("\t" + m.Nombre + " db \"" + m.Texto + "\" ,13, 0");
                    asm.WriteLine("\t" + m.Nombre + " db \"" + m.Texto + "\" ,0");
            }
        }


        private void listaIdentificadores(Variable.TipoDato t)
        {
            if (listaVariables.Exists(v => v.Nombre == Contenido))
            {
                throw new Error("Semantico: la variable \"" + Contenido + "\" ya ha sido declarada; linea: " + linea, log);
            }
            listaVariables.Add(new Variable(Contenido, t));
            var variable = listaVariables.Find(delegate (Variable x) { return x.Nombre == Contenido; });

            match(Tipos.Identificador);
            if (Contenido == "=")
            {
                match("=");
                Expresion();
            }
            if (Contenido == ",")
            {
                match(",");
                listaIdentificadores(t);
            }
        }
        private void bloqueInstrucciones()
        {
            match("{");
            if (Contenido != "}")
            {
                listaIntrucciones();
            }
            match("}");
        }
        // ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void listaIntrucciones()
        {
            Instruccion();
            if (Contenido != "}")
            {
                listaIntrucciones();
            }
        }
        // Instruccion -> Console | If | While | do | For | Variables | Asignacion
        private void Instruccion()
        {
            if (Contenido == "Console")
            {
                console();
            }
            else if (Contenido == "if")
            {
                If();
            }
            else if (Contenido == "while")
            {
                While();
            }
            else if (Contenido == "do")
            {
                Do();
            }
            else if (Contenido == "for")
            {
                For();
            }
            else if (Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion();
                match(";");
            }
        }
        // Asignacion -> Identificador = Expresion;
        private void Asignacion()
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
                        asm.WriteLine("\tpush input");
                        asm.WriteLine("\tpush tipo");
                        asm.WriteLine("\tcall scanf");
                        asm.WriteLine("\tadd esp, 8");
                        asm.WriteLine("\tmov eax, [input]");
                        asm.WriteLine("\tmov dword[" + v.Nombre + "], eax");

                    }
                    else
                    {
                        match("ReadLine");
                        asm.WriteLine("\tpush input");
                        asm.WriteLine("\tpush tipo");
                        asm.WriteLine("\tcall scanf");
                        asm.WriteLine("\tadd esp, 8");
                        asm.WriteLine("\tmov eax, [input]");
                        asm.WriteLine("\tmov dword[" + v.Nombre + "], eax");

                    }
                    match("(");
                    match(")");
                    //match(";");
                }
                else
                {
                    Expresion();

                    asm.WriteLine("\tpop eax");
                    asm.WriteLine("\tmov dword [" + variable + "], eax");
                }
            }
            else if (Contenido == "++")
            {
                match("++");
                asm.WriteLine("\tinc dword [" + variable + "]");
                nuevoValor++;
            }
            else if (Contenido == "--")
            {
                match("--");
                asm.WriteLine("\tdec dword [" + variable + "]");
                nuevoValor--;
            }
            else if (Contenido == "+=")
            {
                match("+=");
                Expresion();
                asm.WriteLine("\tpop eax");
                asm.WriteLine("\tadd [" + variable + "], eax");

            }
            else if (Contenido == "-=")
            {
                match("-=");
                Expresion();
                asm.WriteLine("\tpop eax");
                asm.WriteLine("\tsub [" + variable + "], eax");

            }
            else if (Contenido == "*=")
            {
                match("*=");
                Expresion();
                asm.WriteLine("\tpop eax");
                asm.WriteLine("\tmov ebx, dword [" + variable + "]");
                asm.WriteLine("\timul eax, ebx");
                asm.WriteLine("\tmov dword [" + variable + "], eax");
            }
            else if (Contenido == "/=")
            {
                match("/=");
                Expresion();
                asm.WriteLine("\tpop eax");
                asm.WriteLine("\tmov ecx, eax");
                asm.WriteLine("\tmov eax, " + "[" + variable + "]");
                asm.WriteLine("\txor edx,edx");
                asm.WriteLine("\tdiv ecx");
                asm.WriteLine("\tmov dword [" + variable + "], eax");
            }
            else if (Contenido == "%=")
            {
                match("%=");
                Expresion();
                asm.WriteLine("\txor edx, edx");
                asm.WriteLine("\tpop eax");
                asm.WriteLine("\tmov ebx, dword [" + variable + "]");
                asm.WriteLine("\tidiv ebx");
                asm.WriteLine("\tmov dword [" + variable + "], edx");
            }
            // match(";");
            v.Valor = nuevoValor;
            log.WriteLine(variable + " = " + nuevoValor);
        }

        private void If()
        {
            bool elseExiste = false;
            asm.WriteLine("; if " + cIFs);
            string etqElse = "_else" + cIFs;
            string EndIf = "_endIf" + cIFs++;
            match("if");
            match("(");
            Condicion(etqElse);
            match(")");

            if (Contenido == "{")
            {
                bloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            asm.WriteLine("\tjmp " + EndIf);
            asm.WriteLine(etqElse + ":");

            if (Contenido == "else")
            {
                elseExiste = true;
                match("else");
                if (Contenido == "{")
                {
                    bloqueInstrucciones();
                }
                else
                {
                    Instruccion();
                }
            }
            asm.WriteLine(EndIf + ":");
        }

        private void Condicion(string etiqueta)
        {
            Expresion(); // E1
            string operador = Contenido;
            match(Tipos.OpRelacional);
            Expresion(); // E2
            asm.WriteLine("\tpop eax");
            asm.WriteLine("\tpop ebx");
            asm.WriteLine("\tcmp eax, ebx");
            if (esDoe)
            {

                switch (operador)
                {
                    case ">":
                        asm.WriteLine("\tjle " + etiqueta);
                        break;
                    case ">=":
                        asm.WriteLine("\tjl " + etiqueta);
                        break;
                    case "<":
                        asm.WriteLine("\tjge " + etiqueta);
                        break;
                    case "<=":
                        asm.WriteLine("\tjg " + etiqueta);
                        break;
                    case "==":
                        asm.WriteLine("\tjne " + etiqueta);
                        break;
                    default:
                        asm.WriteLine("\tje " + etiqueta);
                        break;
                }
            }
            else
            {
                switch (operador)
                {
                    case ">":
                        asm.WriteLine("\tjge " + etiqueta);
                        break;
                    case ">=":
                        asm.WriteLine("\tjg " + etiqueta);
                        break;
                    case "<":
                        asm.WriteLine("\tjle " + etiqueta);
                        break;
                    case "<=":
                        asm.WriteLine("\tjl " + etiqueta);
                        break;
                    case "==":
                        asm.WriteLine("\tjne " + etiqueta);
                        break;
                    default:
                        asm.WriteLine("\tje " + etiqueta);
                        break;
                }
            }
        }
        // While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While()
        {
            asm.WriteLine("; while " + cWhiles);
            string etiquetaIni = "_whileIni" + cWhiles;
            string etiquetaFin = "_whileFin" + cWhiles++;
            match("while");
            match("(");
            asm.WriteLine(etiquetaIni + ":");
            Condicion(etiquetaFin);
            match(")");
            if (Contenido == "{")
            {
                bloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            asm.WriteLine("jmp " + etiquetaIni);
            asm.WriteLine(etiquetaFin + ":");
        }

        private void Do()
        {
            asm.WriteLine("; do " + cDoes);
            string etiqueta = "_do" + cDoes++;
            asm.WriteLine(etiqueta + ":");
            esDoe = true;
            match("do");
            if (Contenido == "{")
            {
                bloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            match("while");
            match("(");
            Condicion(etiqueta);
            match(")");
            match(";");
            esDoe = false;
        }
        private string operacionFor(string nombreV)
        {
            string resultado;
            match(Tipos.Identificador);
            if (Contenido == "++")
            {
                match("++");
                resultado = "\tinc dword [" + nombreV + "]";
            }
            else if (Contenido == "--")
            {
                match("--");
                resultado = "\tdec dword [" + nombreV + "]";
            }
            else if (Contenido == "+=")
            {
                match("+=");
                Expresion();
                resultado = "\tpop eax\n\tadd [" + nombreV + "], eax";
            }
            else if (Contenido == "-=")
            {
                match("-=");
                Expresion();
                resultado = "\tpop eax\n\tsub [" + nombreV + "], eax";
            }
            else if (Contenido == "*=")
            {
                match("*=");
                Expresion();
                resultado = "\tpop eax\n\tmov ebx, dword [" + nombreV + "]\n\timul eax, ebx\n\tmov dword [" + nombreV + "], eax";
            }
            else if (Contenido == "/=")
            {
                match("/=");
                Expresion();
                resultado = "\tpop eax\n\tmov ebx, dword [" + nombreV + "]\n\tidiv ebx\n\tmov dword [" + nombreV + "], eax";
            }
            else
            {
                match("%=");
                Expresion();
                resultado = "\txor edx, edx\n\tpop eax\n\tmov ebx, dword [" + nombreV + "]\n\tidiv ebx\n\tmov dword [" + nombreV + "], edx";
            }
            return resultado;
        }
        private void For()
        {
            asm.WriteLine("; for" + cFor);
            string etiquetaIni = "_ForIni" + cFor;
            string etiquetaFin = "_ForFin" + cFor;
            string etiquetaC = "_CondicionFor" + cFor;
            string etiquetaA = "_AsignacionFor" + cFor;
            string etiquetaO = "_OperacionFor" + cFor++;
            string resultado;

            match("for");
            match("(");
            asm.WriteLine(";" + etiquetaA + ":");
            Asignacion();
            asm.WriteLine(etiquetaIni + ":");
            match(";");
            asm.WriteLine(";" + etiquetaC + ":");
            Condicion(etiquetaFin);
            match(";");
            var v = listaVariables.Find(delegate (Variable x) { return x.Nombre == Contenido; });

            resultado = operacionFor(v.Nombre);
            match(")");
            if (Contenido == "{")
            {
                bloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            asm.WriteLine(";" + etiquetaO + ":");
            asm.WriteLine(resultado);
            asm.WriteLine("jmp " + etiquetaIni);
            asm.WriteLine(etiquetaFin + ":");
        }


        private void console()
        {
            bool sLn = false;
            match("Console");
            match(".");
            if (Contenido == "WriteLine")
            {
                match("WriteLine");
                sLn = true;

            }
            else if (Contenido == "write")
            {
                match("Write");
            }
            else if (Contenido == "ReadLine")
            {
                match("ReadLine");
                asm.WriteLine("push [" + Contenido + "]");
                asm.WriteLine("push tipo");
                asm.WriteLine("call scanf");


            }
            else if (Contenido == "Read")
            {
                match("Read");
                asm.WriteLine("push [" + Contenido + "]");
                asm.WriteLine("push tipo");
                asm.WriteLine("call scanf");


            }

            string texto;
            match("(");
            if (Clasificacion == Tipos.Cadena)
            {

                texto = Contenido.Replace("\"", "");
                listaMsg.Add(new Msg(texto, "msg" + cMsg));
                match(Tipos.Cadena);
                asm.Write("\tPRINT_STRING ");
                asm.WriteLine("msg" + cMsg);
                cMsg++;
                /*if(sLn == true){
                    asm.WriteLine("\tNEWLINE");
                }*/
            }
            else
            {
                asm.WriteLine("\tmov eax, [" + Contenido + "]");
                asm.WriteLine("\tpush eax");
                asm.WriteLine("\tpush tipo");
                asm.WriteLine("\tcall printf");
                match(Tipos.Identificador);

            }
            if (Contenido == "+")
            {
                listaConcatenacion();
            }
            match(")");
            //listaMsg[cMsg - 2].Salto = sLn;
            match(";");
            if (sLn == true)
             {
                asm.WriteLine("\tNEWLINE");
             }
        }






        string listaConcatenacion()
        {
            String texto;
            string resultado = "";
            match("+");
            if (Clasificacion == Tipos.Identificador)
            {
                if (!listaVariables.Exists(v => v.Nombre == Contenido))
                {
                    throw new Error("Semantico: la variable \"" + Contenido + "\" no ha sido declarada; linea: " + linea, log);
                }
                var v = listaVariables.Find(variable => variable.Nombre == Contenido);
                // resultado = v.Valor.ToString();
                asm.WriteLine("\tmov eax, [" + v.Nombre + "]");
                asm.WriteLine("\tpush eax");
                asm.WriteLine("\tpush tipo");
                asm.WriteLine("\tcall printf");
                match(Tipos.Identificador);
            }
            else if (Clasificacion == Tipos.Cadena)
            {

                texto = Contenido.Replace("\"", "");
                listaMsg.Add(new Msg(texto, "msg" + cMsg));
                match(Tipos.Cadena);
                asm.Write("\tPRINT_STRING ");
                asm.WriteLine("msg" + cMsg);
                cMsg++;
            }
            if (Contenido == "+")
            {
                listaConcatenacion();
            }
            return resultado;
        }
        private void asm_Main()
        {
            asm.WriteLine();
            asm.WriteLine("%include 'io.inc'");
            asm.WriteLine("extern fflush");
            asm.WriteLine("extern printf");
            asm.WriteLine("extern scanf");
            asm.WriteLine("extern stdout");
            asm.WriteLine("\nsection .bss");
            asm.WriteLine("\tinput resd 1");
            asm.WriteLine("\nsegment .text");
            asm.WriteLine("\tglobal main");
            asm.WriteLine("\nmain:");
        }
        private void asm_endMain()
        {
            asm.WriteLine("\tadd esp, 4\n");
            asm.WriteLine("\tmov eax, 1");
            asm.WriteLine("\txor ebx, ebx");
            asm.WriteLine("\tint 0x80");
        }
        // Main      -> static void Main(string[] args) BloqueInstrucciones 
        private void Main()
        {
            asm_Main();
            match("static");
            match("void");
            match("Main");
            match("(");
            match("string");
            match("[");
            match("]");
            match("args");
            match(")");
            bloqueInstrucciones();
            asm_endMain();

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
                asm.WriteLine("\tpop ebx");
                asm.WriteLine("\tpop eax");
                switch (operador)
                {
                    case "+":
                        asm.WriteLine("\tadd eax, ebx");
                        asm.WriteLine("\tpush eax");
                        break;
                    case "-":
                        asm.WriteLine("\tsub eax, ebx");
                        asm.WriteLine("\tpush eax");
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
                asm.WriteLine("\tpop ebx");
                asm.WriteLine("\tpop eax");
                switch (operador)
                {
                    case "*":
                        asm.WriteLine("\tmul ebx");
                        asm.WriteLine("\tpush eax");
                        break;
                    case "/":
                        asm.WriteLine("\tdiv ebx");
                        asm.WriteLine("\tpush eax");
                        break;
                    case "%":
                        asm.WriteLine("\tdiv ebx");
                        asm.WriteLine("\tpush edx");
                        break;
                }
            }
        }

        private void Factor()
        {
            if (Clasificacion == Tipos.Numero)
            {
                asm.WriteLine("\tmov eax, " + Contenido);
                asm.WriteLine("\tpush eax");
                match(Tipos.Numero);
            }
            else if (Clasificacion == Tipos.Identificador)
            {
                var v = listaVariables.Find(delegate (Variable x) { return x.Nombre == Contenido; });
                asm.WriteLine("\tmov eax, [" + Contenido + "]");
                asm.WriteLine("\tpush eax");
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                Expresion();
                match(")");
            }
        }
    }
}