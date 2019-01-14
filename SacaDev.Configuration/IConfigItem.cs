using System;
using System.Collections.Generic;
using System.Text;

namespace SacaDev.Configuration
{
	/// <summary>
	/// Base class required for each object stored in the ConfigFileBase
	/// </summary>
	/// <typeparam name="NameType">The enumtype used as name</typeparam>
	public interface IConfigItem<NameType> where NameType : struct, IConvertible, IComparable, IFormattable
	{
		NameType Name { get; set; }
		string Value { get; set; }
	}
}
