
namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class TagViewerUserViewStateSettings
    {
		public List<List<int>> SortOrder = new List<List<int>>();
		public List<Guid> PointTagGuidList = new List<Guid>();
    }
}
