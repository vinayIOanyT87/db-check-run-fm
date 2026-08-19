namespace FuelsManager.Areas.FieldHelpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Web.Mvc;
	using System.Web.Mvc.Html;

	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.AccountingArea.ViewModels;

	public abstract class FMFieldHelperBase
	{
		public abstract MvcHtmlString Generate(
			HtmlHelper<TransactionEditorViewModel> html,
			TransactionAliasFieldClass fieldInfo,
			TransactionEditorViewModel model);

		/// <summary>
		/// The id of the field being edited by this control.
		/// </summary>
		public abstract string FieldId { get; }
	}

	public static class FMHtmlHelper
	{
		public static MvcHtmlString Test<TModel, TValue>( this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression )
		{
			return MvcHtmlString.Create( "This is the string!" );
		}
	}

	public abstract class FMFieldHelper<TFieldType> : FMFieldHelperBase
	{
		protected Dictionary<string, Object> Attributes { get; private set; }

		protected Dictionary<string, string> Style { get; private set; }

		public virtual bool Editable { get { return true; } }

		protected virtual Type FieldType { get { return typeof(object); } }

		protected FMFieldHelper()
		{
			this.Attributes = new Dictionary<string, Object>
			                  {
				                  { "class", "formfield" }
			                  };

			this.Style = new Dictionary<string, string>
			             {
				             { "min-width", "175px" }
			             };
		}

		protected virtual void SpecializeControl( TransactionAliasFieldClass fieldInfo, TransactionDO transaction )
		{
		}
		
		public override MvcHtmlString Generate(HtmlHelper<TransactionEditorViewModel> html, TransactionAliasFieldClass fieldInfo, TransactionEditorViewModel model)
		{
			if (this.Editable)
			{
				this.Attributes["autofocus"] = "autofocus";
			}
			else
			{
				this.Style.Add("background-color", "#F0F0F0");
				this.Attributes.Add( "readonly", "readonly" );
			}

			this.SpecializeControl(fieldInfo, model.Transaction);

			var parameter = Expression.Parameter(typeof(TransactionEditorViewModel), "m");

			Expression transExpression;
			PropertyInfo propertyInfo;

			if (fieldInfo.Type == TransactionFieldType.Transaction)
			{
				transExpression = Expression.PropertyOrField(Expression.Constant(model), "Transaction");

				propertyInfo = model.Transaction.GetType().GetProperty( this.FieldId );
			}
			else if ( fieldInfo.Type == TransactionFieldType.LineItem )
			{
				transExpression = Expression.PropertyOrField( Expression.Constant( model ), "LineItem" );
				//transExpression = Expression.Property( transExpression, "Item", Expression.Constant( 0 ) );

				propertyInfo = model.LineItem.GetType().GetProperty( this.FieldId );
			}
			else
			{
				return MvcHtmlString.Create("<b><i>Unsupported field type: " + this.FieldId + "</i></b>");
			}

			if (propertyInfo == null)
			{
				return MvcHtmlString.Create("<b><i>Could not find field: " + this.FieldId + "</i></b>");
			}

			var fieldExpression = Expression.Property( transExpression, propertyInfo );

			var expression = Expression.Lambda<Func<TransactionEditorViewModel, TFieldType>>(
				fieldExpression,
				parameter);

			this.UpdateStyleFromDictionary();

			return this.Editor(html, expression);
		}

		protected virtual MvcHtmlString Editor(HtmlHelper<TransactionEditorViewModel> html, Expression<Func<TransactionEditorViewModel, TFieldType>> expression)
		{
			return html.TextBoxFor(expression, this.Attributes);
		}

		private void UpdateStyleFromDictionary()
		{
			// Convert dictionary to semicolon delimited string
			this.Attributes["style"] = string.Join(";", this.Style.Select(x => x.Key + ":" + x.Value).ToArray());
		}
	}
}
