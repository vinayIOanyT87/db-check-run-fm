namespace FMBusinessObjects.DataObjects
{
    using Varec.CommonComponents.EngineeringUnitsLibrary;

    public class IntoPlaneImportTempGravVcfParams
    {
        public IntoPlaneImportTempGravVcfParams()
        {
            this.Gravity = null;
            //this.GravityUnit = EngineeringUnit.FmdDegApi;
            this.Temperature = null;
            //this.TemperatureUnit = EngineeringUnit.FmtDegF;
            this.VCF = null;
        }

        public double? Gravity { get; set; }

        //public EngineeringUnit GravityUnit { get; set; }

        public double? Temperature { get; set; }

        //public EngineeringUnit TemperatureUnit { get; set; }

        public double? VCF { get; set; }
    }
}
