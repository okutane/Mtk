namespace Matveev.Mtk.Core
{
    public interface IPointFunctionWithGradient
    {
        double Eval(Point p);
        Vector Grad(Point p);
    }
}
