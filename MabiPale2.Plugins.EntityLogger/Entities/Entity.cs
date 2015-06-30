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
    public class Entity : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long EntityId { get; set; }
        public string Name { get; set; }
        public abstract string EntityType { get; }

        // Use lazies to defer building of the script till it's actually needed
        // This also then caches the results
        public Lazy<string> Info { get; protected set; }
        public Lazy<string> Script { get; protected set; }

        private abstract string GenerateInfo();
        private abstract string GenerateScript();

        public Entity()
        {
            // Subscribe to our own property change
            PropertyChanged += Entity_PropertyChanged;
        }

        void Entity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Info" || e.PropertyName == "Script")
                return;

            // Invalidate scripts when a property changes.
            Info = new Lazy<string>(GenerateInfo);
            Script = new Lazy<string>(GenerateScript);
        }
    }
}
