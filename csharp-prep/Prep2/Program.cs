using System;

class Program
{
    static void Main(string[] args)
    {
        string letter;
        Console.WriteLine("What is your grade percentage?");
        string gradePercentage = Console.ReadLine();
        if (double.Parse(gradePercentage) >= 90)
        {
            letter = "A";
        }
        else if (double.Parse(gradePercentage) >= 80)
        {
            letter = "B";
        }
        else if (double.Parse(gradePercentage) >= 70)
        {
            letter = "C";
        }
        else if (double.Parse(gradePercentage) >= 60)
        {
            letter = "D";
        }
        else
        {
            letter = "F";
        }
        if (letter == "A")
        {
            Console.WriteLine($"You got an {letter}!");
        }
        else
        {
            Console.WriteLine($"You got a {letter}!");            
        }
        if (double.Parse(gradePercentage) >= 70)
        {
            Console.WriteLine("That means you passed! Great job!");
        }
        else
        {
            Console.WriteLine("That means you failed this time,but BYU-I has resources to help you out if you need them, you've got this!");
        }
    }
}