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
		private readonly ObservableCollection<Entities.Entity> _entities;
		private readonly Lazy<MainWindow> _window;

		public override string Name
		{
			get { return "Entity Logger"; }
		}

		public Main(IPluginManager p)
			: base(p)
		{
			_entities = new ObservableCollection<Entities.Entity>();
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
			lock (entities)
				entities.Clear();

			if (form != null && !form.IsDisposed)
				form.ClearEntities();
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
			// EntitiesAppear
			else if (palePacket.Op == 0x5334)
			{
				var entityCount = palePacket.Packet.GetShort();
				for (int i = 0; i < entityCount; ++i)
				{
					var type = palePacket.Packet.GetShort();
					var len = palePacket.Packet.GetInt();
					var entityData = palePacket.Packet.GetBin();

					// Creature
					if (type == 16)
					{
						var entityPacket = new Packet(entityData, 0);
						AddCreatureInfo(entityPacket);
					}
					// Prop
					else if (type == 160)
					{
						var entityPacket = new Packet(entityData, 0);
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

			creature.EntityId = id;
			creature.Name = packet.GetString();
			packet.GetString();
			packet.GetString();
			creature.Race = packet.GetInt();
			creature.SkinColor = packet.GetByte();

			// [180600, NA187 (25.06.2014)] Changed from byte to short
			if (packet.NextIs(PacketElementType.Byte))
				creature.EyeType = packet.GetByte();
			else if (packet.NextIs(PacketElementType.Short))
				creature.EyeType = packet.GetShort();

			creature.EyeColor = packet.GetByte();
			creature.MouthType = packet.GetByte();
			creature.State = packet.GetUInt();
			creature.StateEx = packet.GetUInt();
			// Public only
			if (packet.NextIs(PacketElementType.Int))
			{
				creature.StateEx2 = packet.GetUInt();

				// [180300, NA166 (18.09.2013)]
				if (packet.NextIs(PacketElementType.Int))
					packet.GetUInt();
			}
			creature.Height = packet.GetFloat();
			creature.Weight = packet.GetFloat();
			creature.Upper = packet.GetFloat();
			creature.Lower = packet.GetFloat();
			creature.Region = packet.GetInt();
			creature.X = packet.GetInt();
			creature.Y = packet.GetInt();
			creature.Direction = packet.GetByte();
			creature.BattleState = packet.GetInt();
			creature.WeaponSet = packet.GetByte();
			creature.Color1 = packet.GetUInt();
			creature.Color2 = packet.GetUInt();
			creature.Color3 = packet.GetUInt();
			creature.CombatPower = packet.GetFloat();
			creature.StandStyle = packet.GetString();

			creature.LifeRaw = packet.GetFloat();
			creature.LifeMaxBase = packet.GetFloat();
			creature.LifeMaxMod = packet.GetFloat();
			creature.LifeInjured = packet.GetFloat();

			// [180800, NA196 (14.10.2014)] ?
			if (packet.NextIs(PacketElementType.Short))
				packet.GetShort(); // ?

			var regenCount = packet.GetInt();
			for (int i = 0; i < regenCount; ++i)
				packet.Skip(6);
			var unkCount = packet.GetInt();
			for (int i = 0; i < unkCount; ++i)
				packet.Skip(6);

			creature.Title = packet.GetUShort();
			creature.TitleApplied = packet.GetDate();
			creature.OptionTitle = packet.GetUShort();

			creature.MateName = packet.GetString();
			creature.Destiny = packet.GetByte();

			var itemCount = packet.GetInt();
			for (int i = 0; i < itemCount; ++i)
			{
				var itemOId = packet.GetLong();
				var itemInfo = packet.GetObj<ItemInfo>();
				if (packet.NextIs(PacketElementType.String))
					packet.GetString(); // Extra Item Info
				creature.Items.Add(itemOId, itemInfo);
			}

			AddEntity(creature);
		}

		private void AddProp(Packet packet)
		{
			var prop = new Prop();

			prop.EntityId = packet.GetLong();
			prop.Id = packet.GetInt();

			if (prop.IsServerProp)
			{
				prop.Name = packet.GetString();
				prop.Title = packet.GetString();
				prop.PropInfo = packet.GetObj<PropInfo>();
			}

			prop.State = packet.GetString();
			packet.GetLong();

			if (packet.GetBool())
				prop.Xml = packet.GetString();

			if (!prop.IsServerProp)
				prop.Direction = packet.GetFloat();

			AddEntity(prop);
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
