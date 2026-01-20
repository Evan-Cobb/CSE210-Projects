using System;

class Program
{
    static void Main(string[] args)
    {
        Fraction f1 = new Fraction();
        Console.WriteLine(f1.GetFractionString());
        Console.WriteLine(f1.GetDecimalValue());

        Fraction f2 = new Fraction(5);
        Console.WriteLine(f2.GetFractionString());
        Console.WriteLine(f2.GetDecimalValue());

        Fraction f3 = new Fraction(3, 4);
        Console.WriteLine(f3.GetFractionString());
        Console.WriteLine(f3.GetDecimalValue());

        Fraction f4 = new Fraction(1, 3);
        Console.WriteLine(f4.GetFractionString());
        Console.WriteLine(f4.GetDecimalValue());

        Random random= new Random();
        Fraction f5 = new Fraction();
        for (int i = 0; i < 20; i++)
        {
            int numeratorValue = random.Next(1, 11);
            int denominatorValue = random.Next(1, 11);
            f5.Setnumerator(numeratorValue);
            f5.Setdenominator(denominatorValue);
            Console.Write($"Fraction {i + 1}: ");
            Console.Write($"string: {f5.GetFractionString()}");
            Console.WriteLine($" Number: {f5.GetDecimalValue()}");
        }
    }
}