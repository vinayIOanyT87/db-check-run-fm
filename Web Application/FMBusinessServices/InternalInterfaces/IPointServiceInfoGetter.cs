namespace FMBusinessServices.InternalInterfaces
{
    using InternalClasses;

    internal interface IPointServiceInfoGetter
    {
        PointServiceInfo Info { get; }

        void Refresh();
    }
}