using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MabiPale2.Plugins.EntityLogger.Entities
{
	[ImplementPropertyChanged]
	public abstract class Entity : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public long EntityId { get; set; }
		public string Name { get; set; }
		public DateTime Seen { get; set; }

		public Exception Error { get; set; }

		public abstract string EntityType { get; }

		// Use lazies to defer building of the script till it's actually needed
		// This also then caches the results
		public Lazy<string> Info { get; protected set; }
		public Lazy<string> Script { get; protected set; }

		protected abstract string GenerateInfo();
		protected abstract string GenerateScript();

		protected Entity()
		{
			Seen = DateTime.Now;

			// Subscribe to our own property change
			PropertyChanged += Entity_PropertyChanged;
		}

		void Entity_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Info" || e.PropertyName == "Script")
				return;

			// Invalidate scripts when a property changes.
			Info = new Lazy<string>(GenerateInfoInternal);
			Script = new Lazy<string>(GenerateScriptInternal);
		}

		public override bool Equals(object obj)
		{
			var other = obj as Entity;
			if (other == null)
				return false;

			return EntityId == other.EntityId;
		}

		public override int GetHashCode()
		{
			return EntityId.GetHashCode();
		}

		private string GenerateInfoInternal()
		{
			return Error != null ? Error.ToString() : GenerateInfo();
		}

		private string GenerateScriptInternal()
		{
			return Error != null ? "" : GenerateScript();
		}
	}
}
