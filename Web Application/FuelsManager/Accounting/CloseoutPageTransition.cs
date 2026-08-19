namespace FuelsManager.Accounting
{
	using System;
	using System.Collections;

	using FMBusinessObjects.UtilityObjects;

	[Serializable]
	public class CloseoutPageTransition
	{
		public static string SessionKey = "CloseoutPageTransition";

		#region Private Attributes
		private DateTimeOffset inventoryDate;
		private DateTimeOffset fromDate;
		private DateTimeOffset toDate;
		private DateTimeOffset lastcloseoutDate;
		private double bookInventoryGross;
		private double bookInventoryNet;
		private double physicalInventoryGross;
		private double physicalInventoryNet;
		private double varianceGross;
		private double varianceNet;
		private Hashtable validationHshTbl;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the closeout page transition object.
		/// </summary>
		public CloseoutPageTransition()
		{
			this.Init();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will get and set the inventory date attribute.
		/// </summary>
		public DateTimeOffset InventoryDate
		{
			get { return this.inventoryDate; }
			set { this.inventoryDate = value; }
		}

		/// <summary>
		/// This property will get and set the FROM date attribute.
		/// </summary>
		public DateTimeOffset FromDate
		{
			get { return this.fromDate; }
			set { this.fromDate = value; }
		}

		public DateTimeOffset LastcloseoutDate
		{
			get { return this.lastcloseoutDate; }
			set { this.lastcloseoutDate = value; }
		}
		/// <summary>
		/// This property will get and set the TO date attribute.
		/// </summary>
		public DateTimeOffset ToDate
		{
			get { return this.toDate; }
			set { this.toDate = value; }
		}

		/// <summary>
		/// This property will get and set the book inventory gross attribute.
		/// </summary>
		public double BookInventoryGross
		{
			get { return this.bookInventoryGross; }
			set
			{
				this.bookInventoryGross = value;
				this.validationHshTbl["bookInventoryGross"] = true;
			}
		}

		/// <summary>
		/// This property will get and set the book inventory gross attribute.
		/// </summary>
		public string BookInventoryGrossStr
		{
			get { return this.bookInventoryGross.ToString(); }
			set
			{
				try
				{
					this.bookInventoryGross = System.Convert.ToDouble(value);
					this.validationHshTbl["bookInventoryGross"] = true;
				}
				catch (Exception)
				{
					this.validationHshTbl["bookInventoryGross"] = false;
				}
			}
		}

		/// <summary>
		/// This property will get and set the book inventory net attribute.
		/// </summary>
		public double BookInventoryNet
		{
			get { return this.bookInventoryNet; }
			set
			{
				this.bookInventoryNet = value;
				this.validationHshTbl["bookInventoryNet"] = true;
			}
		}

		/// <summary>
		/// This property will get and set the book inventory net attribute.
		/// </summary>
		public string BookInventoryNetStr
		{
			get { return this.bookInventoryNet.ToString(); }
			set
			{
				try
				{
					this.bookInventoryNet = System.Convert.ToDouble(value);
					this.validationHshTbl["bookInventoryNet"] = true;
				}
				catch (Exception)
				{
					this.validationHshTbl["bookInventoryNet"] = false;
				}
			}
		}

		/// <summary>
		/// This property will get and set the physical inventory gross attribute.
		/// </summary>
		public double PhysicalInventoryGross
		{
			get { return this.physicalInventoryGross; }
			set
			{
				this.physicalInventoryGross = value;
				this.validationHshTbl["physicalInventoryGross"] = true;
			}
		}

		/// <summary>
		/// This property will get and set the physical inventory gross attribute.
		/// </summary>
		public string PhysicalInventoryGrossStr
		{
			get { return this.physicalInventoryGross.ToString(); }
			set
			{
				try
				{
					this.physicalInventoryGross = System.Convert.ToDouble(value);
					this.validationHshTbl["physicalInventoryGross"] = true;
				}
				catch (Exception)
				{
					this.validationHshTbl["physicalInventoryGross"] = false;
				}
			}
		}

		/// <summary>
		/// This property will get and set the physical inventory net attribute.
		/// </summary>
		public double PhysicalInventoryNet
		{
			get { return this.physicalInventoryNet; }
			set
			{
				this.physicalInventoryNet = value;
				this.validationHshTbl["physicalInventoryNet"] = true;
			}
		}

		/// <summary>
		/// This property will get and set the physical inventory net attribute.
		/// </summary>
		public string PhysicalInventoryNetStr
		{
			get { return this.physicalInventoryNet.ToString(); }
			set
			{
				try
				{
					this.physicalInventoryNet = System.Convert.ToDouble(value);
					this.validationHshTbl["physicalInventoryNet"] = true;
				}
				catch (Exception)
				{
					this.validationHshTbl["physicalInventoryNet"] = false;
				}
			}
		}

		/// <summary>
		/// This property will get and set the variance gross attribute.
		/// </summary>
		public double VarianceGross
		{
			get { return this.varianceGross; }
			set
			{
				this.varianceGross = value;
				this.validationHshTbl["varianceGross"] = true;
			}
		}

		/// <summary>
		/// This property will get and set the variance gross attribute.
		/// </summary>
		public string VarianceGrossStr
		{
			get { return this.varianceGross.ToString(); }
			set
			{
				try
				{
					this.varianceGross = System.Convert.ToDouble(value);
					this.validationHshTbl["varianceGross"] = true;
				}
				catch (Exception)
				{
					this.validationHshTbl["varianceGross"] = false;
				}
			}
		}

		/// <summary>
		/// This property will get and set the variance net attribute.
		/// </summary>
		public double VarianceNet
		{
			get { return this.varianceNet; }
			set
			{
				this.varianceNet = value;
				this.validationHshTbl["varianceNet"] = true;
			}
		}

		/// <summary>
		/// This property will get and set the variance net attribute.
		/// </summary>
		public string VarianceNetStr
		{
			get { return this.varianceNet.ToString(); }
			set
			{
				try
				{
					this.varianceNet = System.Convert.ToDouble(value);
					this.validationHshTbl["varianceNet"] = true;
				}
				catch (Exception)
				{
					this.validationHshTbl["varianceNet"] = true;
				}
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method will true if there is data for all the fields. It will return
		/// false if there is not data for all the fields.  Closeout requires all the
		/// fields to have data.
		/// </summary>
		/// <returns></returns>
		public bool IsValid()
		{
			bool valid = true;

			System.Collections.IDictionaryEnumerator enumerator = this.validationHshTbl.GetEnumerator();

			while (enumerator.MoveNext() == true)
			{
				valid = (bool)enumerator.Value;

				if (valid == false)
					break;
			}

			return valid;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method initializes the closeout page transition object to its
		/// initial state.
		/// </summary>
		private void Init()
		{
			this.inventoryDate = TimeConverter.Today();
			this.fromDate = DateTimeOffset.Now;
			this.toDate = DateTimeOffset.Now;
			this.bookInventoryGross = 0.0;
			this.bookInventoryNet = 0.0;
			this.physicalInventoryGross = 0.0;
			this.physicalInventoryNet = 0.0;
			this.varianceGross = 0.0;
			this.varianceNet = 0.0;
			this.validationHshTbl = new Hashtable();

			this.validationHshTbl.Add("bookInventoryGross", false);
			this.validationHshTbl.Add("bookInventoryNet", false);
			this.validationHshTbl.Add("physicalInventoryGross", false);
			this.validationHshTbl.Add("physicalInventoryNet", false);
			this.validationHshTbl.Add("varianceGross", false);
			this.validationHshTbl.Add("varianceNet", false);
		}
		#endregion
	}
}
