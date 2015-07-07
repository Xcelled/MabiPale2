using MabiLib.Structs;
using MabiPale2.Plugins.EntityLogger.Entities;
using MabiPale2.Plugins.EntityLogger.Properties;
using MabiPale2.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MabiPale2.Plugins.EntityLogger
{
	public class Main : Plugin
	{
		private readonly ObservableCollection<Entity> _entities;
		private readonly Lazy<MainWindow> _window;

		public override string Name
		{
			get { return "Entity Logger"; }
		}

		public Main(IPluginManager p)
			: base(p)
		{
			_entities = new ObservableCollection<Entity>();
			_window = new Lazy<MainWindow>(() => new MainWindow(_entities), false);
		}

		public override void Initialize()
		{
			manager.AddToMenu(Name, OnClick);
			manager.AddToToolbar(Resources.bug, Name, OnClick);

			manager.Recv += OnRecv;
			manager.Clear += OnClear;
		}

		private void OnClick(object sender, EventArgs e)
		{
			if (_window.Value.IsVisible)
				_window.Value.Focus();
			else
				_window.Value.Show();
		}

		private void OnClear()
		{
			lock (_entities)
				_entities.Clear();
		}

		private void OnRecv(PalePacket palePacket)
		{
			// EntityAppears
			if (palePacket.Op == 0x520C)
			{
				AddCreatureInfo(palePacket.Packet);
			}
			// PropAppears
			if (palePacket.Op == 0x52D0)
			{
				AddProp(palePacket.Packet);
			}
			// ItemNew
			if (palePacket.Op == 0x59E0)
			{
				AddItem(palePacket.Packet);
			}
			// EntitiesAppear
			else if (palePacket.Op == 0x5334)
			{
				var entityCount = palePacket.Packet.GetShort();
				for (int i = 0; i < entityCount; ++i)
				{
					var type = palePacket.Packet.GetShort();
					palePacket.Packet.GetInt();
					var entityData = palePacket.Packet.GetBin();
					var entityPacket = new Packet(entityData, 0);

					// Creature
					if (type == 16)
					{
						AddCreatureInfo(entityPacket);
					}
					// Item
					else if (type == 80)
					{
						AddItem(entityPacket);
					}
					// Prop
					else if (type == 160)
					{
						AddProp(entityPacket);
					}
				}
			}
		}

		private void AddCreatureInfo(Packet packet)
		{
			var id = packet.GetLong();
			var type = packet.GetByte();

			// Public
			if (type != 5)
				return;

			var creature = new Creature();

			packet.Rewind();

			try
			{
				Creature.Parse(packet, creature);
			}
			catch (Exception ex)
			{
				creature.Error = new ParseException(ex, packet);
			}

			AddEntity(creature);
		}

		private void AddProp(Packet packet)
		{
			var prop = new Prop();

			try
			{
				Prop.Parse(packet, prop);
			}
			catch (Exception ex)
			{
				prop.Error = new ParseException(ex, packet);
			}

			AddEntity(prop);
		}

		private void AddItem(Packet packet)
		{
			var item = new Item();

			try
			{
				Item.Parse(packet, item);
			}
			catch (Exception ex)
			{
				item.Error = new ParseException(ex, packet);
			}

			AddEntity(item);
		}

		private void AddEntity(Entity entity)
		{
			if (CheckDuplicate(entity))
				return;

			lock (_entities)
				_entities.Add(entity);
		}

		private bool CheckDuplicate(Entity newEntity)
		{
			lock (_entities)
				return _entities.Any(entity => entity.Equals(newEntity));
		}
	}
}
