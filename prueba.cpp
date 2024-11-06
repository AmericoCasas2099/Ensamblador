using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

static void Main(string[] args)
{
   int altura,i,j;
    float y,z;
    y=10;
    z=2;
    char c;
    c = 100;

    Console.Write("Valor de altura = ");
    altura = Console.ReadLine();
    Console.WriteLine("");
    float x = (3 + altura) * 8 - (10 - 4) / 2; // = 61
    Console.WriteLine("Valor de x: "+x);
    x--;
    x+=(altura*8);
    x*=2;
    Console.WriteLine("Valor de x: "+x);
    /*
    // x/=(y-6);
    int k=1;
    for (i = 1; k<=altura; k++) // mandar false y pedir de retorno el valor de la asignacion
    {
        for (j = 1; j<=k; j++)
        {
            if (j%2==0)
                {Console.Write("*");}
            else
                {Console.Write("-");}
        }
        Console.WriteLine("");
    }
    i = 0;
    do
    {
        Console.Write("-");
        i++;
    }
    while (i<altura*2);
    Console.WriteLine("");
    for (i = 1; i<=altura; i++)
    {
        j = 1;
        while (j<=i)
        { 
            Console.Write(""+j);
            j++;
        }
        Console.WriteLine("");
    }
    i = 0;
    do
    {
        Console.Write("-");
        i++;
    }
    while (i<altura*2);
    */
}

