namespace FMBusinessObjects.PIDXTransactions
{
	using System;

	using FMBusinessObjects.Constants;
	using FMBusinessObjects.Exceptions;

	// ReSharper disable once InconsistentNaming
	public class AuthorizationGrantedLA : AuthorizationGrantedBase
	{
		#region Private attributes
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the authorization granted LA class.
		/// </summary>
		public AuthorizationGrantedLA()
		{
			this.Initialize();
		}
		#endregion


		#region Properties
		public string PidxrVersion { get; set; }
		#endregion


		#region Public override methods
		/// <summary>
		/// This method parses the LA response.
		/// </summary>
		/// <param name="response"></param>
		public override void Parse(string response)
		{
			int headerLength = this.ParseHeader(response);

			// response end finding must be adjusted to deal with extraneous
			// NUL bytes in the stream :(
			int respEnd = response.IndexOf("R?", StringComparison.Ordinal);
			while (respEnd > 0 && response[respEnd - 1] == '\0')
			{
				respEnd--;
			}
			// Now move back 5 more characters, past the checkDigit16 and checkDigit37
			respEnd -= 5;

			int productLength = respEnd - headerLength;

			if (productLength > 0
			&& this.ProductAllocationMethod != "0")
			{
				string productAuthorizations = response.Substring(headerLength, productLength);
				this.ParseProductAuthorizations(productAuthorizations);
			}
			// raw response needed to check validity against check digit
			this.ResponseNoCheckDigit = response.Substring(0, respEnd);
		}

		/// <summary>
		/// This method will parse out the product authorizations
		/// </summary>
		/// <param name="productStr"></param>
		protected override void ParseProductAuthorizations(string productStr)
		{
			int totalPIDXProductAuthorizations;

			try
			{
				totalPIDXProductAuthorizations = Convert.ToInt32(this.TotalPidxProducts);
				if (totalPIDXProductAuthorizations == 0)
				{
					return;
				}

				// Note that while it appears the spec does not permit it,
				// in practice a PIDX provider may include extra whitespace after
				// the product ('B') records; we do not reject the response unless there 
				// is insufficient characters to complete the specified number
				// of product records.
				if (string.IsNullOrEmpty(productStr)
				|| productStr.Length < totalPIDXProductAuthorizations * 18)
				{
					throw new PIDXException(PIDXConstants.ERR_MSG_034);
				}
			}
			catch
			{
				throw new PIDXException(PIDXConstants.ERR_MSG_034);
			}

			for (int index = 0; index < totalPIDXProductAuthorizations; index++)
			{
				PIDXProductAuthorization productAuthorization = new PIDXProductAuthorization
				{
					ProductTypeIndicator = productStr.Substring(index * 18, 1),
					PidxProductOrFamily = productStr.Substring(index * 18 + 1, 4).TrimEnd(' '),
					AuthorizedVolume = productStr.Substring(index * 18 + 5, 10),
					UnitOfMeasure = productStr.Substring(index * 18 + 15, 3)
				};
				this.PIDXProductAuthorizations.Add(productAuthorization);
			}
		}

		/// <summary>
		/// This method parse the header portion of the response along with check
		/// digit value.
		/// </summary>
		/// <param name="response"></param>
		protected override int ParseHeader(string response)
		{
			int headerLength;

			if (!string.IsNullOrEmpty(response))
			{
				if (response.IndexOf("E!", StringComparison.Ordinal) >= 0)
				{
					throw new PIDXException(PIDXConstants.ERR_MSG_025, PIDXException.ErrorTypes.WARNING);
				}

				int respLength = response.Length;
				// response end finding must be adjusted to deal with extraneous
				// NUL bytes in the stream :(
				int respEnd = response.IndexOf("R?", StringComparison.Ordinal);
				while (respEnd > 0 && response[respEnd - 1] == '\0')
				{
					respEnd--;
				}
				// Now move back 1 more character, to the end of the checkDigit16
				respEnd -= 1;

				headerLength = 37;

				if (respLength < headerLength
				|| respEnd < 4)
				{
					throw new PIDXException(PIDXConstants.ERR_MSG_021);
				}

				this.ResponseType = response.Substring(0, 4);
				this.PidxrVersion = response.Substring(4, 4);
				this.AuthorizationNumber = response.Substring(8, 8);
				this.ConsigneeNumber = response.Substring(16, 14);
				this.CarrierID = response.Substring(30, 4);
				this.TotalPidxProducts = response.Substring(34, 2);
				this.ProductAllocationMethod = response.Substring(36, 1);
				this.CheckDigit37 = response.Substring(respEnd - 4, 1);
				this.CheckDigit16 = response.Substring(respEnd - 3, 4);
				this.TerminatingString = response.Substring(respEnd + 1);
			}
			else
			{
				throw new PIDXException(PIDXConstants.ERR_MSG_022);
			}

			return headerLength;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Initialize()
		{
			this.PidxrVersion = "";
		}
		#endregion
	}
}
