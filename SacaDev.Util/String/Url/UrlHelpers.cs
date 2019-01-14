using System;
using System.Collections.Generic;
using System.Text;

namespace SacaDev.Util.String.Url
{
	public static class UrlHelpers
	{
		/// <summary>
		/// When a directory without trailing slash is given, will return previous directory
		/// </summary>
		/// <param name="path"></param>
		/// <returns>directorypart</returns>
		public static string GetDirectory(this string path) {
			int lastSlashIndex = path.LastIndexOfAny(new char[] { '\\', '/' })+1;
			return path.Substring(0, lastSlashIndex);
		}
		public static string GetFileNameFromString(this string path) {
			int lastSlashIndex = path.LastIndexOfAny(new char[] { '\\', '/' });

			int ind = lastSlashIndex + 1;
			return path.Substring(ind);
		}
	}
}
