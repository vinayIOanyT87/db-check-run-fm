namespace RateModules.RateCalculationModules
{
	using System;
	using System.Collections;

	public class LeastSquaredQuadRegression
	{
		public double LastValueTime { get; set; }
		public double LastValueValue { get; set; }
		public int NumOfEntries => this.numOfEntries;

		public ArrayList pointArray = new ArrayList();
		private int numOfEntries;
		private readonly int numOfAllowedEntries;
		private double[] pointpair;

		#region Constructors
		/// <summary>
		/// This is the default constructor
		/// </summary>
		public LeastSquaredQuadRegression()
		{
			this.numOfEntries = 0;
			this.numOfAllowedEntries = 6;
			this.pointpair = new double[2];
			this.LastValueTime = 0.0;
			this.LastValueValue = 0.0;
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will add point pairs to be used in the calculations.
		/// x = difference in time
		/// y = instantaneous calculated rate
		/// </summary>
		/// <param name="x">x value</param>
		/// <param name="y">y value</param>
		public void AddPoints(double x, double y)
		{
			double timeToStore	= x;
			double rateToStore	= y;
			double offsetValue	= 0.95;
			bool duplicates		= true;

			this.pointpair = new double[2];

			if (this.numOfEntries >= this.numOfAllowedEntries)
			{
				// always remove the first index
				this.pointArray.RemoveAt(0);
			}

			// Since we cannot have a duplicate time/rate in the collection, then we need
			// to offset the time.  In addition, the rate has to be offset too so that
			// it is portional.
			while (duplicates)
			{
				duplicates = this.DuplicatesExists(timeToStore, rateToStore);

				if (duplicates)
				{
					timeToStore = timeToStore * offsetValue;
					rateToStore = rateToStore * offsetValue;
					offsetValue = offsetValue - 0.05;
				}
			}

			this.pointpair[0] = timeToStore;
			this.pointpair[1] = rateToStore;
			this.pointArray.Add(this.pointpair);
			this.numOfEntries = this.pointArray.Count;
		}

		/// <summary>
		/// This method will return the a term of the equation ax^2 + bx + c
		/// </summary>
		/// <returns>a term</returns>
		public double? ATerm()
		{
			if (this.numOfEntries < 3)
			{
				// need to change this throw
				//throw new InvalidOperationException("Insufficient pairs of co-ordinates");
				return null;
			}

			//notation sjk to mean the sum of x_i^j*y_i^k. 
			double s40 = this.GetSx4(); //sum of x^4
			double s30 = this.GetSx3(); //sum of x^3
			double s20 = this.GetSx2(); //sum of x^2
			double s10 = this.GetSx();  //sum of x
			double s00 = this.numOfEntries;  //sum of x^0 * y^0  ie 1 * number of entries

			double s21 = this.GetSx2Y(); //sum of x^2*y
			double s11 = this.GetSxy();  //sum of x*y
			double s01 = this.GetSy();   //sum of y

			// a = Da/D
			return (s21 * (s20 * s00 - s10 * s10) - s11 * (s30 * s00 - s10 * s20) + s01 * (s30 * s10 - s20 * s20))
					/
					(s40 * (s20 * s00 - s10 * s10) - s30 * (s30 * s00 - s10 * s20) + s20 * (s30 * s10 - s20 * s20));
		}

		/// <summary>
		/// This method will return the b term of the equation ax^2 + bx + c.
		/// </summary>
		/// <returns>b term</returns>
		public double? BTerm()
		{
			if (this.numOfEntries < 3)
			{
				// need to change this throw
				//throw new InvalidOperationException("Insufficient pairs of co-ordinates");
				return null;
			}

			// Notation sjk to mean the sum of x_i^j*y_i^k.  
			double s40 = this.GetSx4(); //sum of x^4
			double s30 = this.GetSx3(); //sum of x^3
			double s20 = this.GetSx2(); //sum of x^2
			double s10 = this.GetSx();  //sum of x
			double s00 = this.numOfEntries;  //sum of x^0 * y^0  ie 1 * number of entries

			double s21 = this.GetSx2Y(); //sum of x^2*y
			double s11 = this.GetSxy();  //sum of x*y
			double s01 = this.GetSy();   //sum of y

			// b = Db/D
			return (s40 * (s11 * s00 - s01 * s10) - s30 * (s21 * s00 - s01 * s20) + s20 * (s21 * s10 - s11 * s20))
					/
					(s40 * (s20 * s00 - s10 * s10) - s30 * (s30 * s00 - s10 * s20) + s20 * (s30 * s10 - s20 * s20));
		}

		/// <summary>
		/// returns the c term of the equation ax^2 + bx + c
		/// </summary>
		/// <returns>c term</returns>
		public double? CTerm()
		{
			if (this.numOfEntries < 3)
			{
				// need to change this throw
				//throw new InvalidOperationException("Insufficient pairs of co-ordinates");
				return null;
			}

			//notation sjk to mean the sum of x_i^j*y_i^k.  
			double s40 = this.GetSx4(); //sum of x^4
			double s30 = this.GetSx3(); //sum of x^3
			double s20 = this.GetSx2(); //sum of x^2
			double s10 = this.GetSx();  //sum of x
			double s00 = this.numOfEntries;  //sum of x^0 * y^0  ie 1 * number of entries

			double s21 = this.GetSx2Y(); //sum of x^2*y
			double s11 = this.GetSxy();  //sum of x*y
			double s01 = this.GetSy();   //sum of y

			//c = Dc/D
			return (s40 * (s20 * s01 - s10 * s11) - s30 * (s30 * s01 - s10 * s21) + s20 * (s30 * s11 - s20 * s21))
					/
					(s40 * (s20 * s00 - s10 * s10) - s30 * (s30 * s00 - s10 * s20) + s20 * (s30 * s10 - s20 * s20));
		}

		/// <summary>
		/// This method will return true if the time has changed from the last time stored.
		/// </summary>
		/// <param name="currentTimeInTicks">The current time in ticks.</param>
		/// <returns>Returns true if the time has changed. Otherwise, it returns false.</returns>
		public bool HasTimeChanged(double currentTimeInTicks)
		{
			if ((long)this.LastValueTime == (long)currentTimeInTicks)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// This method will calculate the predicted Y value.
		/// </summary>
		/// <param name="x">The x value (time).</param>
		/// <returns>Returns the predicted Y value.</returns>
		public double? GetPredictedY(double x,ref double ATerm, ref double BTerm, ref double CTerm)
		{
			double? aterm = this.ATerm();
			double? bterm = this.BTerm();
			double? cterm = this.CTerm();

			if (aterm == null || bterm == null || cterm == null)
			{
				return null;
			}

			ATerm = (double)aterm;
			BTerm = (double)bterm;
			CTerm = (double)cterm;

			//returns value of y predicted by the equation for a given value of x
			return aterm * Math.Pow(x, 2) + bterm * x + cterm;
		}

		/// <summary>
		/// This method will calculate the instantaneous rate based on the difference
		/// of time and value.
		/// </summary>
		/// <param name="x">X is the new time.</param>
		/// <param name="y">Y is the new value.</param>
		/// <returns></returns>
		public double CalculateInstantaneousRate(double x, double y,ref double changeinTime,ref double changeinValue)
		{
			double changeInTime = Math.Abs(x - this.LastValueTime);
			double changeInValue = y - this.LastValueValue;

			changeinTime = changeInTime;
			changeinValue = changeInValue;

			if ((long)changeInTime == 0)
			{
				return 0;
			}

			double returnValue = changeInValue / changeInTime;
			return returnValue;
		}
		#endregion

		#region Private methods
		/*helper methods*/
		/// <summary>
		/// This method will get the sum of x values.
		/// </summary>
		/// <returns>Returns the sum of X values.</returns>
		private double GetSx() 
		{
			double sx = 0;

			foreach (double[] ppair in this.pointArray)
			{
				sx += ppair[0];
			}

			return sx;
		}


		/// <summary>
		/// This method will get the sum of Y values.
		/// </summary>
		/// <returns>Returns the sum of Y values.</returns>
		private double GetSy()
		{
			double sy = 0;

			foreach (double[] ppair in this.pointArray)
			{
				sy += ppair[1];
			}

			return sy;
		}

		/// <summary>
		/// This method will get the sum of x squares.
		/// </summary>
		/// <returns>Returns the sum of x squares.</returns>
		private double GetSx2() 
		{
			double sx2 = 0;

			foreach (double[] ppair in this.pointArray)
			{
				// sum of x^2
				sx2 += Math.Pow(ppair[0], 2); 
			}

			return sx2;
		}

		/// <summary>
		/// This method will get the sum of x cube values.
		/// </summary>
		/// <returns>Returns X cube values</returns>
		private double GetSx3()
		{
			double sx3 = 0;

			foreach (double[] ppair in this.pointArray)
			{
				// sum of x^3
				sx3 += Math.Pow(ppair[0], 3); 
			}

			return sx3;
		}

		/// <summary>
		/// This method will get the sum of x to the 4th value.
		/// </summary>
		/// <returns>Returns x to the 4th value.</returns>
		private double GetSx4()
		{
			double sx4 = 0;

			foreach (double[] ppair in this.pointArray)
			{
				// sum of x^4
				sx4 += Math.Pow(ppair[0], 4); 
			}

			return sx4;
		}

		/// <summary>
		/// This method will get the sum of x times y.
		/// </summary>
		/// <returns>Returns the sum of x times y.</returns>
		private double GetSxy() 
		{
			double sxy = 0;

			foreach (double[] ppair in this.pointArray)
			{
				// sum of x*y
				sxy += ppair[0] * ppair[1]; 
			}

			return sxy;
		}

		/// <summary>
		/// This method will get the sum of X square times Y (x^2*y).
		/// </summary>
		/// <returns>Returns the sum of x square times y value.</returns>
		private double GetSx2Y() 
		{
			double sx2Y = 0;

			foreach (double[] ppair in this.pointArray)
			{
				// sum of x^2*y
				sx2Y += Math.Pow(ppair[0], 2) * ppair[1]; 
			}

			return sx2Y;
		}

		/// <summary>
		/// This method will check to see if the time span and quantity already exists in the collection.
		/// </summary>
		/// <param name="timeValue">The new time to compare.</param>
		/// <param name="rateValue"></param>
		/// <returns>Returns true if the time span exists, otherwise it returns false.</returns>
		private bool DuplicatesExists(double timeValue, double rateValue)
		{
			foreach (double[] pointData in this.pointArray)
			{
				double timeDifferential = pointData[0];
				double rate = pointData[1];

				if ((long) timeDifferential == (long) timeValue 
					&& (long) timeDifferential != 0)
				{
					return true;
				}
			}

			return false;
		}
		#endregion
	}
}
