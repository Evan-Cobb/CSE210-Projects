using System;

public class Fraction
{
    private int _numerator;
    private int _denominator;

    public Fraction()
    {
        // Default to 1/1
        _numerator = 1;
        _denominator = 1;
    }

    public Fraction(int wholeNumber)
    {
        _numerator = wholeNumber;
        _denominator = 1;
    }

    public Fraction(int numerator, int denominator)
    {
        _numerator = numerator;
        Setdenominator(denominator);
    }

    public void Setdenominator(int denominator)
    {
        if (denominator != 0)
        {
            _denominator = denominator;
        }
        else
        {
            _denominator = 1;
        }
    }

    public void Setnumerator(int numerator)
    {
        _numerator = numerator;
    }

    public int Getnumerator()
    {
        return _numerator;
    }
    public int Getdenominator()
    {
        return _denominator;
    }

    public string GetFractionString()
    {
        string text = $"{_numerator}/{_denominator}";
        return text;
    }

    public double GetDecimalValue()
    {
        return (double)_numerator / (double)_denominator;
    }
}