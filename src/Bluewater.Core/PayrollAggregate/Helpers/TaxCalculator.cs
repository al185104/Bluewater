namespace Bluewater.Core.PayrollAggregate.Helpers;

public static class TaxCalculator
{
    public static (decimal taxDeduction, decimal TaxPercentage) CalculateTax(decimal salary)
    {
        decimal taxDeduction = 0;
        decimal taxPercentage = 0;

        if (salary <= 10417)
        {
            taxDeduction = 0;
            taxPercentage = 0;
        }
        else if (salary <= 16666)
        {
            taxDeduction = (salary - 10417) * 0.15m;
            taxPercentage = 15;
        }
        else if (salary <= 33332)
        {
            taxDeduction = 937.50m + (salary - 16667) * 0.20m;
            taxPercentage = 20;
        }
        else if (salary <= 83332)
        {
            taxDeduction = 4270.70m + (salary - 33333) * 0.25m;
            taxPercentage = 25;
        }
        else if (salary <= 333332)
        {
            taxDeduction = 16770.70m + (salary - 83333) * 0.30m;
            taxPercentage = 30;
        }
        else
        {
            taxDeduction = 91770.70m + (salary - 333333) * 0.35m;
            taxPercentage = 35;
        }

        return (taxDeduction, taxPercentage);
    }
}