using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
	public class ExStarsDataDictionary
	{
		public string[] Keys(SecurityClass security)
		{
#if false
			 
From ExStarsSegment:
			 
"AppendToElement() failed for{0} Segment \"{1}\"({2} -- {3})[{4}] {5}\n{6}"
"Element \"{0}{2:00}\" name=\"{1}\" is already defined"
"Element {0}{1:00} is missing parameters"
"ExStarsSegment.ElementValue({0}) has bad argument for segment ID {1}"
"For Segment \"{0} ({1})\": \"{2}\", {3}\n  Errort=\"{4}\"\n{5}"
"For Segment {0}, element {1} ({2}) has an invalid format of {3}\n{4}"
"Invalid segment data:\"{0}\""
"Invalid segment ID:\"{0}\""
"missing the {0} segment or it has an invalid format"
"Name Unknown"
"Segment {0} ({1}) does not contain an element for index {2}"
"Segment {0} does not have an element with index {1}"
"There is no defined segment type to match the start of {0}"

From ExStarsReportsBase:
			 
" \nBEGIN "
"REF", "Reference Identification"
"Reference Identification Qualifier", "55 = Sequence Number, Use of this code is required."
"Reference Identification", "unique filer assigned sequence number."
"Description", "Error Response Code, Use only when responding to errors, 00001  = Record corrected"
" \nEND "

#endif

			return new string[] { "" };
		}
	}
}
