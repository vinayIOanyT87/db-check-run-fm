#region Copyright
/* The MIT License (MIT)

Copyright (c) 2014 Anderson Luiz Mendes Matos

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion Copyright

/* This file is part of Datatables.Mvc, which is used to bind datatables request parameters.
 * See https://github.com/ALMMa/datatables.mvc */

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Runtime.Serialization;

    using DataTables.Mvc;

    /// <summary>
    /// Represents a DataTables column for server-side parsing and work.
    /// </summary>
    [DataContract]
    public class DataTablesColumn
    {
        /// <summary>
        /// Gets the data component (bind property name).
        /// </summary>
        [DataMember]
        public string Data { get; private set; }

        /// <summary>
        /// Get's the name component (if any provided on client-side script).
        /// </summary>
        [DataMember]
        public string Name { get; private set; }

        /// <summary>
        /// Indicates if the column is searchable or not.
        /// </summary>
        [DataMember]
        public bool Searchable { get; private set; }

        /// <summary>
        /// Indicates if the column is orderable or not.
        /// </summary>
        [DataMember]
        public bool Orderable { get; private set; }

        /// <summary>
        /// Gets the search component for the column.
        /// </summary>
        [DataMember]
        public Search Search { get; private set; }

        /// <summary>
        /// Indicates if the current column should be ordered on server-side or not.
        /// </summary>
        public bool IsOrdered { get { return this.OrderNumber != -1; } }

        /// <summary>
        /// Indicates the column' position on the ordering (multi-column ordering).
        /// </summary>
        [DataMember]
        public int OrderNumber { get; private set; }

        /// <summary>
        /// Indicates the column's sort direction.
        /// </summary>
        [DataMember]
        public OrderDirection SortDirection { get; private set; }

        /// <summary>
        /// Sets the columns ordering.
        /// </summary>
        /// <param name="orderNumber">The column's position on the ordering (multi-column ordering).</param>
        /// <param name="orderDirection">The column's sort direction.</param>
        /// <exception cref="System.ArgumentException">Thrown if the provided orderDirection is not valid.</exception>
        public void SetColumnOrdering(int orderNumber, string orderDirection)
        {
            this.OrderNumber = orderNumber;

            if (orderDirection.ToLower().Equals("asc"))
            {
                this.SortDirection = DataTablesColumn.OrderDirection.Ascendant;
            }
            else if (orderDirection.ToLower().Equals("desc"))
            {
                this.SortDirection = DataTablesColumn.OrderDirection.Descendant;
            }
            else
            {
                throw new ArgumentException("The provided ordering direction was not valid. Valid values must be 'asc' or 'desc' only.");
            }
        }

        /// <summary>
        /// Creates a new DataTables column.
        /// </summary>
        /// <param name="data">The data component (bind property name).</param>
        /// <param name="name">The name of the column (if provided).</param>
        /// <param name="searchable">True if the column allows searching, false otherwise.</param>
        /// <param name="orderable">True if the column allows ordering, false otherwise.</param>
        /// <param name="searchValue">The searched value for the column, or an empty string.</param>
        /// <param name="isRegexValue">True if the search value is a regex value, false otherwise.</param>
        public DataTablesColumn(string data, string name, bool searchable, bool orderable, string searchValue, bool isRegexValue)
        {
            this.Data = data;
            this.Name = name;
            this.Searchable = searchable;
            this.Orderable = orderable;
            this.Search = new Search(searchValue, isRegexValue);

            // Default - indicates that the column should not be ordered on server-side.
            this.OrderNumber = -1;
        }

        /// <summary>
        /// Defines order directions for proper use.
        /// </summary>
        public enum OrderDirection
        {
            /// <summary>
            /// Represents an ascendant (A-Z) ordering.
            /// </summary>
            Ascendant = 0,
            /// <summary>
            /// Represents a descendant (Z-A) ordering.
            /// </summary>
            Descendant = 1
        }
    }
}
