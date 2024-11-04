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
    3. Console.Read & Console.ReadLine    --> 
    4. Considerar else en el If           --> Listo
    5. Programar el while                 -->
    6. Programar el for                   -->
*/

namespace Ensamblador
{
    public class Lenguaje : Sintaxis
    {
        private List<Variable> listaVariables;
        private List<Msg> listaMsg = new List<Msg>();

        private int cIFs, cDoes, cWhiles, cElses, cFor, cMsg;
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
            asm.WriteLine("\nformat db " + '"' + "%d" + '"' + ", 0");
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
                if (m.Salto)
                {
                    asm.WriteLine("\t" + m.Nombre + " db \"" + m.Texto + "\" ,13, 0");
                }
                else
                {
                    asm.WriteLine("\t" + m.Nombre + " db \"" + m.Texto + "\" ,0");
                }

            }
            //asm.WriteLine("\tsLinea db " + "''" + ", 10, 0");
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

                    }
                    else
                    {
                        match("ReadLine");

                        asm.WriteLine("mov eax, input_buffer");
                        asm.WriteLine("push eax");
                        asm.WriteLine("call scanf");
                        asm.WriteLine("mov dword [" + variable + "], input_buffer");

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
                asm.WriteLine("\tdec " + variable);
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
                asm.WriteLine("\tmov ebx, dword [" + variable + "]");
                asm.WriteLine("\tidiv ebx");
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
        // While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While()
        {
            asm.WriteLine("; while " + ++cWhiles);
            string etiquetaIni = "_whileIni" + cWhiles;
            string etiquetaFin = "_whileFin" + cWhiles;
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
        }

        private void For()
        {
            asm.WriteLine("; for" + ++cFor);
            string etiquetaIni = "_ForIni" + cFor;
            string etiquetaFin = "_ForFin" + cFor;
            match("for");
            match("(");
            asm.WriteLine(etiquetaIni + ":");
            Asignacion();
            match(";");
            Condicion(etiquetaFin);
            match(";");
            Asignacion();
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
            else
            {
                match("Write");
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
            }
            else
            {
                asm.WriteLine("\tmov eax, [" + Contenido + "]");
                asm.WriteLine("\tpush eax");
                asm.WriteLine("\tpush format");
                asm.WriteLine("\tcall printf");
                match(Tipos.Identificador);

            }
            if (Contenido == "+")
            {
                listaConcatenacion();
            }
            match(")");
            listaMsg[cMsg - 2].Salto = sLn;
            match(";");
           /* if (sLn == true)
            {
                asm.WriteLine("\tPRINT_STRING salto");
            }*/
        }



        /*
        string texto = "";
        bool salto = false;
        string etiquetaMsg = "msg" + cMsg++;

        match("Console");
        match(".");
        if (Contenido == "WriteLine")
        {
            match("WriteLine");

        }
        else
        {
            match("WriteLine");
            salto = true;
        }
        match("(");
        if (Clasificacion == Tipos.Cadena)
        {
            texto = Contenido;
            match(Tipos.Cadena);
            if (Contenido == "+")
            {
                Console.Write(texto); //borrar

                listaVariables.Add(new Variable(etiquetaMsg, Variable.TipoDato.Char));
                var v = listaVariables.Find(delegate (Variable x) { return x.Nombre == etiquetaMsg; });
                v.Texto = texto;
                v.Wrt = true;

                asm.WriteLine("\tpush ebp");
                asm.WriteLine("\tmov ebp, esp");
                asm.WriteLine("\tpush " + etiquetaMsg);

                asm.WriteLine("\tcall printf");
                asm.WriteLine("\tmov esp, ebp");
                asm.WriteLine("\tpop ebp");

                listaConcatenacion();
            }
            else
            {
                if (salto)
                {
                    Console.Write(texto); //borrar
                    texto = Contenido;
                    listaVariables.Add(new Variable(etiquetaMsg, Variable.TipoDato.Char));
                    var v = listaVariables.Find(delegate (Variable x) { return x.Nombre == etiquetaMsg; });
                    v.Texto = texto;
                    v.Wrt = true;
                    //v.Salto;
                    asm.WriteLine("\tpush ebp");
                    asm.WriteLine("\tmov ebp, esp");
                    asm.WriteLine("\tpush " + etiquetaMsg);

                    asm.WriteLine("\tcall printf");
                    asm.WriteLine("\tmov esp, ebp");
                    asm.WriteLine("\tpop ebp");
                    /*
                                           asm.WriteLine("mov eax, 4"); //Salto de linea
                                           asm.WriteLine("mov ebx, 1");
                                           asm.WriteLine("mov ecx, esp");
                                           asm.WriteLine(" mov byte [esp], 10 ");
                                           asm.WriteLine("mov edx, 1");
                                           asm.WriteLine("int 0x80");


                }
                else
                {
                    Console.Write(texto); //borrar
                    texto = Contenido;
                    listaVariables.Add(new Variable(etiquetaMsg, Variable.TipoDato.Char));
                    var v = listaVariables.Find(delegate (Variable x) { return x.Nombre == etiquetaMsg; });
                    v.Texto = texto;
                    v.Wrt = true;
                    //v.Salto;
                    asm.WriteLine("\tpush ebp");
                    asm.WriteLine("\tmov ebp, esp");
                    asm.WriteLine("\tpush " + etiquetaMsg);

                    asm.WriteLine("\tcall printf");
                    asm.WriteLine("\tmov esp, ebp");
                    asm.WriteLine("\tpop ebp");
                }

            }
            match(")");
            match(";");
        }
        else if (Clasificacion == Tipos.Identificador)
        {

        }*/


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
                asm.WriteLine("\tpush format"); //borrar
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
                asm.WriteLine("\tmov eax, [" + Contenido+"]");
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