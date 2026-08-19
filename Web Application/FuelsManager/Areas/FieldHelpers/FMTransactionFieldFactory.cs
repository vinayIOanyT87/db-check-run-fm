namespace FuelsManager.Areas.FieldHelpers
{
	using System;
	using System.Reflection;
	using System.Web.Mvc;

	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.AccountingArea.ViewModels;

	public static class FMTransactionFieldFactory
	{
		public static MvcHtmlString FMGenerateField(this HtmlHelper<TransactionEditorViewModel> html, TransactionAliasFieldClass field, TransactionEditorViewModel model)
		{
			try
			{
				var helper = GetFieldHelper(field);
				return helper.Generate( html, field, model );
			}
			catch (Exception exception)
			{
				return MvcHtmlString.Create(exception.Message);
			}
		}

		public static FMFieldHelperBase GetFieldHelper(TransactionAliasFieldClass field)
		{
			var typeNamespace = new AliasNameHelper().GetType().Namespace;
			var fieldId = field.ID;

			if ( field.Type == TransactionFieldType.LineItem )
			{
				fieldId = "LineItem" + fieldId;
			}

			var helperName = typeNamespace + "." + fieldId + "Helper";
			var helperType = Assembly.GetExecutingAssembly().GetType( helperName );
			if ( helperType == null )
			{
				throw new Exception("Helper type not found: " + helperName);
			}

			return (FMFieldHelperBase) Activator.CreateInstance( helperType );
		}
	}
}
