using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SacaDev.Util.Encryption;
using SacaDev.Util.String;
using SacaDev.Util.String.Url;
namespace SacaDev.Configuration
{
	//TODO: this class goes right if the file is correct format, but there is a need of checks (rebuild file partially, possible without losing noncorrupt data)
	/// <summary>
	/// when there are exceptions when loading data bcause invalid format. now it only does this when the file doesnt exist at all in the constructor.
	/// </summary>
	/// <typeparam name="T">The Type of the stored elements</typeparam>
	/// <typeparam name="TPropertyEnum">Enum of the possible properties.</typeparam>
	/// <typeparam name="TGroupEnum">All subgroups below rootlayer in the document</typeparam>
	public abstract class ConfigFileBase<T, TPropertyEnum, TGroupEnum>
		where TGroupEnum : struct, IConvertible, IComparable, IFormattable
		where TPropertyEnum : struct, IConvertible, IComparable, IFormattable
		where T : IConfigItem<TPropertyEnum>
	{
		protected string EncryptionKey = null;
		private bool FileIsEncrypted => (EncryptionKey != null);

		protected abstract string RootElement { get; }
		protected string FilePath { get; private set; }
		public void Save()
		{
			using(var fs = new FileStream(FilePath, FileMode.Truncate, FileAccess.Write)) {
				using(var sw = "".ToStream()) {
					ConfigFile.Save(sw);
					sw.Position = 0;
					StreamReader sr = new StreamReader(sw);
					var content = sr.ReadToEnd();
					var contentEncrypted = (FileIsEncrypted) ? content.EncryptWithKey(EncryptionKey) : content;

					var contentEncryptedByteArray = Encoding.UTF8.GetBytes(contentEncrypted);
					fs.Write(contentEncryptedByteArray, 0, contentEncrypted.Length);
				}
			}
		}

		private Stream LoadDocument()
		{
			using(var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read)) {
				using(var sr = new StreamReader(fs)) {
					var content = sr.ReadToEnd();

					var contentDecrypted = (FileIsEncrypted) ? content.DecryptWithKey(EncryptionKey) : content;
					return contentDecrypted.ToStream();
				}
			}
		}

		protected XDocument _configFile;
		protected XDocument ConfigFile {
			get {
				if(_configFile == null) {
					try {

						_configFile = XDocument.Load(LoadDocument(), LoadOptions.PreserveWhitespace);
					}
					catch(Exception) {
						File.Delete(FilePath);
						CreateEmptyFile();
						_configFile = XDocument.Load(LoadDocument(), LoadOptions.PreserveWhitespace);
					}
				}
				return _configFile;
			}
		}
		private XElement Root => ConfigFile.Elements(RootElement).Single();
		private XElement Child(TGroupEnum child) => Root.Elements(child.ToString()).Single();

		//private string GetEnumName(IConvertible inputEnum,Type type)=> Enum.Parse(type,inputEnum).ToString()

		public string this[TGroupEnum child, TPropertyEnum key] {
			get {
				var wrap = Root.Elements(child.ToString()).Single();
				var el = wrap.Elements(key.ToString()).First();
				return el?.Value;
			}
			set {
				UpdateSetting(key, value, child);
			}
		}

		public ICollection<T> CurrentItems {
			get {
				Dictionary<string, string> itemsFromFile = new Dictionary<string, string>();

				foreach(XElement child in Root.Elements()) {
					foreach(XElement el in child.Elements()) {
						itemsFromFile.Add(el.Name.LocalName, el.Value);
					}
				}

				var items = new ObservableCollection<T>();
				foreach(var child in DefaultItems) {
					foreach(var item in child.Value) {
						var fromFile = itemsFromFile.SingleOrDefault(a => a.Key == item.Name.ToString()).Value;
						if(fromFile != null) {
							item.Value = fromFile;
						}
						items.Add(item);
					}

				}

				return items;
			}
		}

		public abstract IDictionary<TGroupEnum, ICollection<T>> DefaultItems { get; }

		#region constructors
		public ConfigFileBase(string filePath, string encryptionKey)
		{
			EncryptionKey = encryptionKey;
			Initialize(filePath);
		}
		public ConfigFileBase(string filePath)
		{
			Initialize(filePath);
		}

		private void Initialize(string filePath)
		{
			this.FilePath = filePath;
			if(!File.Exists(FilePath))
				CreateEmptyFile();
			CreateDefaultFileLayout();
		}
		#endregion

		private void CreateEmptyFile()
		{
			Directory.CreateDirectory(FilePath.GetDirectory());

			using(var file = File.CreateText(FilePath)) {
				var children = DefaultItems.Select(i => new XElement(i.Key.ToString(), null));
				var content = new XElement(RootElement, children)
					.ToString();
				var encryptedContent = (FileIsEncrypted) ? content.EncryptWithKey(EncryptionKey) : content;
				file.WriteLine(encryptedContent);
			}
		}

		private void CreateDefaultFileLayout()
		{
			foreach(KeyValuePair<TGroupEnum, ICollection<T>> child in DefaultItems) {
				var childEl = Child(child.Key);
				foreach(var prop in child.Value) {

					if(childEl.Elements(prop.Name.ToString()).Count() == 0) {
						var el = new XElement(prop.Name.ToString(), prop.Value);
						childEl.Add(el);
					}
				}
			}
			Save();
		}

		public void UpdateSetting(TPropertyEnum key, string newValue, TGroupEnum childEl)
		{
			var wrap = Root.Elements(childEl.ToString()).Single();
			Save();
			if(wrap.Elements(key.ToString()).Count() != 0) {
				var el = wrap.Elements(key.ToString()).First();
				el.SetValue(newValue);
				Save();
			}
		}
	}
}
