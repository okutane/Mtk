namespace Matveev.Common
{
    public interface IAdditable<OtherType, ResultType>
    {
        ResultType Add(OtherType other, double weight);
    }

    public interface ISubtractable<OtherType, ResultType>
    {
        ResultType Subtract(OtherType other);
    }

    public interface ISizeable
    {
        double Size();
    }
}
