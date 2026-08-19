namespace FMBusinessServices.DataAccessLayer
{
	using System.Data;

	using FMBusinessObjects.DataObjects;

	internal static class GeneralConfigDAO
	{
		/// <summary>
		/// This method will load the associated assigned adjustment aliases into the object.
		/// </summary>
		internal static void LoadGeneralConfigurationAlias( this GeneralConfigDO config, DataSet dataSet )
		{
			config.AdjustmentAliasList.Clear();

			if ( dataSet != null )
			{
				DataTable table = dataSet.Tables[0];

				foreach ( DataRow row in table.Rows )
				{
					var generalConfigAliasDO = new GeneralConfigAlias();
					generalConfigAliasDO.LoadGeneralConfigAliasSQL( row );
					config.AdjustmentAliasList.Add( generalConfigAliasDO );
				}
			}
		}
	}
}
