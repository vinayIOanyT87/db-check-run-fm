namespace FuelsManager.Areas.FieldHelpers
{
	public class SiteHelper : FMFieldHelper<string>
	{
		public override bool Editable { get { return false; } }

		public override string FieldId { get { return "Site"; } }
	}
}
