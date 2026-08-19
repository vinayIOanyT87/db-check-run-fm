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

namespace DataTables.Mvc
{
    using System.Collections;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FMBusinessObjects.DataObjects;

    /// <summary>
    /// Represents a read-only DataTables column collection.
    /// </summary>
    public class DataTablesColumnCollection : IEnumerable<DataTablesColumn>
    {
        /// <summary>
        /// For internal use only.
        /// Stores data.
        /// </summary>
        private readonly IReadOnlyList<DataTablesColumn> data;

        /// <summary>
        /// Created a new ReadOnlyColumnCollection with predefined data.
        /// </summary>
        /// <param name="columns">The column collection from DataTables.</param>
        public DataTablesColumnCollection(IEnumerable<DataTablesColumn> columns)
        {
            if (columns == null)
            {
                throw new ArgumentNullException("The provided column collection cannot be null", "columns");
            }

            this.data = columns.ToList().AsReadOnly();
        }

        /// <summary>
        /// Get sorted columns on client-side already on the same order as the client requested.
        /// The method checks if the column is bound and if it's ordered on client-side.
        /// </summary>
        /// <returns>The ordered enumeration of sorted columns.</returns>
        public IOrderedEnumerable<DataTablesColumn> GetSortedColumns()
        {
            return this.data
                .Where(column => !String.IsNullOrWhiteSpace(column.Data) && column.IsOrdered)
                .OrderBy(c => c.OrderNumber);
        }

        /// <summary>
        /// Get filtered columns on client-side.
        /// The method checks if the column is bound and if the search has a value.
        /// </summary>
        /// <returns>The enumeration of filtered columns.</returns>
        public IEnumerable<DataTablesColumn> GetFilteredColumns()
        {
            return this.data.Where(column => !String.IsNullOrWhiteSpace(column.Data) && column.Searchable && !String.IsNullOrWhiteSpace(column.Search.Value));
        }

        /// <summary>
        /// Get sorted columns on client-side already on the same order as the client requested.
        /// The method checks if the column is bound and if it's ordered on client-side.
        /// The returned expression can be used with the OrderBy(string sortExpression) extension mehod
        /// found here : http://extensionmethod.net/csharp/ienumerable-t/orderby-string-sortexpression
        /// </summary>
        /// <remarks>Added by phayman www.kwiboo.com</remarks>
        /// <returns>The ordered enumeration of sorted columns as an expression. e.g. "columnname asc, othercolumn desc"</returns>
        public string GetSortedColumnsExpression()
        {
            var sortExpression = new List<string>();
            foreach (var column in this.GetSortedColumns())
            {
                sortExpression.Add(column.Data + " " + (column.SortDirection == DataTablesColumn.OrderDirection.Descendant ? "desc" : "asc"));
            }

            return String.Join(",", sortExpression.ToArray());
        }

        /// <summary>
        /// Returns the enumerable element as defined on IEnumerable.
        /// </summary>
        /// <returns>The enumerable elemento to iterate through data.</returns>
        public IEnumerator<DataTablesColumn> GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        /// <summary>
        /// Returns the enumerable element as defined on IEnumerable.
        /// </summary>
        /// <returns>The enumerable element to iterate through data.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.data).GetEnumerator();
        }
    }
}
