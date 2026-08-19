/// <summary>
/// File name:	BOLProductBase.cs
/// Purpose:	
///	Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 
///				2008.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Varec, Inc.
///	Author(s):	Ivan Orndorff
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:		By:				Reason:
///		----------	-------------	-------------------------------------------
///		10-Jul-08	I.Orndorff		1.0.1 - Remove "Math.Abs()" from "GrossDigit()". This
///											will make the assignment of gross and net values
///											consistent.
///		
/// </summary>
/// 

using System;
using System.Collections;
using System.Text;

namespace PIDXTransactions
{
    public abstract class BOLProductBase
    {
        #region Private attributes
        private string productCode;
        private int blendID;
        private double gross;
        private double netTemperature;
        private int netTemperatureFlag;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor for the BOL Product class.
        /// </summary>
        public BOLProductBase()
        {
            this.Initialize();
        }
        #endregion
        
        #region Properties
        public string ProductCode
        {
            get { return this.productCode; }
            set { this.productCode = value; }
        }

        public int BlendIDDigit
        {
            get { return this.blendID; }
            set { this.blendID = Math.Abs(value); }
        }

        public string BlendID
        {
            get { return this.blendID.ToString(); }
        }

        public double GrossDigit
        {
            get { return this.gross; }
            set { this.gross = value; }
        }

        public string Gross
        {
            get
            {
                string outStr = this.gross.ToString();
                int length = 8 - outStr.Length;

                for (int count = length; count > 0; count--)
                {
                    outStr = "0" + outStr;
                }

                return outStr;
            }
        }
 
        public double NetTemperatureDigit
        {
            get { return this.netTemperature; }
            set { this.netTemperature = value; }
        }

        public string NetTemperature
        {
            get
            {
                string outStr = this.netTemperature.ToString();
                int length = 8 - outStr.Length;

                for (int count = length; count > 0; count--)
                {
                    outStr = "0" + outStr;
                }

                return outStr;
            }
        }

        public int NetTemperatureFlagDigit
        {
            get { return this.netTemperatureFlag; }
            set { this.netTemperatureFlag = Math.Abs(value); }
        }

        public string NetTemperatureFlag
        {
            get { return this.netTemperatureFlag.ToString(); }
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// This method validates for the BOL Product common fields. It throws
        /// an exception if the validation fails.
        /// </summary>
        protected void Validate()
        {
            if (this.blendID == -99)
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_012);
            }

            if (this.gross == -9999.0)
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_013);
            }

            if (this.netTemperature == -9999.0)
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_014);
            }

            if (this.netTemperatureFlag == -99)
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_015);
            }

            if ((this.productCode == null) ||
                (this.productCode.Length != PIDXConstants.PRODUCT_CODE_LENGTH))
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_016);
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        private void Initialize()
        {
            this.productCode = null;
            this.blendID = -99;
            this.gross = -9999.0;
            this.netTemperature = -9999.0;
            this.netTemperatureFlag = -99;
        }
        #endregion

        #region Abstract methods
        /// <summary>
        /// This is an abstract method that forces implementation of the validation at the derived class.
        /// </summary>
        /// <returns></returns>
        public abstract void ValidateProduct();
        #endregion

    }
}
