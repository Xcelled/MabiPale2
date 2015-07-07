using Aura.Mabi;
using MabiLib.Structs;
using MabiPale2.Shared;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MabiPale2.Plugins.EntityLogger.Entities
{
	[ImplementPropertyChanged]
	public class Item : Entity
	{
		private ItemInfo _itemInfo;

		public ItemPacketType KnowledgeLevel { get; set; }
		public ItemInfo ItemInfo
		{
			get { return _itemInfo; }
			set { _itemInfo = value; Name = "Class: " + value.Id; }
		}

		public override string EntityType
		{
			get { return "Item"; }
		}

		// Public only
		public float SizeMultiplier { get; set; }
		public ItemBounceStyle BounceStyle { get; set; }

		// Private only
		public ItemOptionInfo OptionInfo { get; set; }
		public EgoInfo EgoInfo { get; set; }

		public MabiDictionary MetaData1 { get; set; }
		public MabiDictionary MetaData2 { get; set; }

		public long QuestId { get; set; }

		protected override string GenerateInfo()
		{
			var sb = new StringBuilder();

			sb.AppendLine("Entity id: {0:X16}", EntityId);
			sb.AppendLine("Knowledge Level: {0}", KnowledgeLevel);
			sb.AppendLine();

			sb.AppendLine("--- Item Info ---");
			foreach (var ifi in typeof(ItemInfo).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).OrderBy(f => f.MetadataToken))
			{
				sb.AppendFormat("\t{0}: {1}", ifi.Name, ifi.GetValue(ItemInfo));
				if (ifi.Name.IndexOf("color", StringComparison.OrdinalIgnoreCase) > -1)
					sb.AppendFormat(" (0x{0:X})", ifi.GetValue(ItemInfo));
				sb.AppendLine();
			}
			sb.AppendLine();

			if (KnowledgeLevel == ItemPacketType.Public)
			{
				sb.AppendLine("Size Multiplier: {0}", SizeMultiplier);
				sb.AppendLine("Bounce Style: {0}", BounceStyle);
			}
			else
			{
				sb.AppendLine("--- Item Option Info ---");
				foreach (var ifi in typeof(ItemOptionInfo).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).OrderBy(f => f.MetadataToken))
				{
					sb.AppendLine("\t{0}: {1}", ifi.Name, ifi.GetValue(OptionInfo));
				}
				sb.AppendLine();

				if (EgoInfo != null)
				{
					sb.AppendLine("--- Ego Info ---");
					sb.AppendLine("\tName: {0}", EgoInfo.Name);
					sb.AppendLine("\tRace: {0}", EgoInfo.Race);
					sb.AppendLine("\tFullness: {0}", EgoInfo.Fullness);

					sb.AppendLine("\tSocial Level: {0}", EgoInfo.SocialLevel);
					sb.AppendLine("\tSocial Exp: {0}", EgoInfo.SocialExp);
					sb.AppendLine("\tStr Level: {0}", EgoInfo.StrLevel);
					sb.AppendLine("\tStr Exp: {0}", EgoInfo.StrExp);
					sb.AppendLine("\tInt Level: {0}", EgoInfo.IntLevel);
					sb.AppendLine("\tInt Exp: {0}", EgoInfo.IntExp);
					sb.AppendLine("\tDex Level: {0}", EgoInfo.DexLevel);
					sb.AppendLine("\tDex Exp: {0}", EgoInfo.DexExp);
					sb.AppendLine("\tWill Level: {0}", EgoInfo.WillLevel);
					sb.AppendLine("\tWill Exp: {0}", EgoInfo.WillExp);
					sb.AppendLine("\tLuck Level: {0}", EgoInfo.LuckLevel);
					sb.AppendLine("\tLuck Exp: {0}", EgoInfo.LuckExp);
					sb.AppendLine("\tAwakening Energy: {0}", EgoInfo.AwakeningEnergy);
					sb.AppendLine("\tAwakening Exp: {0}", EgoInfo.AwakeningExp);

					sb.AppendLine("\tLast Feeding: {0}", EgoInfo.LastFeeding);

					sb.AppendLine();
				}

				sb.AppendLine("--- MetaData 1 ---");
				sb.AppendLine("\t(Raw): {0}", MetaData1.ToString());
				sb.AppendLine();

				sb.AppendLine("--- MetaData 2 ---");
				sb.AppendLine("\t(Raw): {0}", MetaData2.ToString());
				sb.AppendLine();

			}

			return sb.ToString();
		}

		protected override string GenerateScript()
		{
			var sb = new StringBuilder();

			sb.AppendLine("var i = new Item({0});", ItemInfo.Id);
			sb.AppendLine();

			foreach (var ifi in typeof(ItemInfo).GetFields(BindingFlags.Public | BindingFlags.Instance).OrderBy(f => f.MetadataToken))
			{
				sb.AppendFormat("i.Info.{0} = ", ifi.Name);
				if (ifi.Name.IndexOf("pocket", StringComparison.OrdinalIgnoreCase) > -1)
					sb.AppendFormat("Pocket.{0}", ifi.GetValue(ItemInfo));
				else if (ifi.Name.IndexOf("color", StringComparison.OrdinalIgnoreCase) > -1)
					sb.AppendFormat("0x{0:X}", ifi.GetValue(ItemInfo));
				else
					sb.AppendFormat("{0}", ifi.GetValue(ItemInfo));

				sb.AppendLine(";");
			}
			sb.AppendLine();

			if (KnowledgeLevel == ItemPacketType.Public)
			{
				
			}
			else
			{
				foreach (var ifi in typeof(ItemOptionInfo).GetFields(BindingFlags.Public | BindingFlags.Instance).OrderBy(f => f.MetadataToken))
				{
					sb.AppendLine("i.OptionInfo.{0} = {1};", ifi.Name, ifi.GetValue(OptionInfo));
				}
				sb.AppendLine();

				if (EgoInfo != null)
				{
					sb.AppendLine("i.EgoInfo.Name = {0};", EgoInfo.Name);
					sb.AppendLine("i.EgoInfo.Race = {0};", EgoInfo.Race);

					sb.AppendLine("i.EgoInfo.Social Level = {0};", EgoInfo.SocialLevel);
					sb.AppendLine("i.EgoInfo.Social Exp = {0};", EgoInfo.SocialExp);
					sb.AppendLine("i.EgoInfo.Str Level = {0};", EgoInfo.StrLevel);
					sb.AppendLine("i.EgoInfo.Str Exp = {0};", EgoInfo.StrExp);
					sb.AppendLine("i.EgoInfo.Int Level = {0};", EgoInfo.IntLevel);
					sb.AppendLine("i.EgoInfo.Int Exp = {0};", EgoInfo.IntExp);
					sb.AppendLine("i.EgoInfo.Dex Level = {0};", EgoInfo.DexLevel);
					sb.AppendLine("i.EgoInfo.Dex Exp = {0};", EgoInfo.DexExp);
					sb.AppendLine("i.EgoInfo.Will Level = {0};", EgoInfo.WillLevel);
					sb.AppendLine("i.EgoInfo.Will Exp = {0};", EgoInfo.WillExp);
					sb.AppendLine("i.EgoInfo.Luck Level = {0};", EgoInfo.LuckLevel);
					sb.AppendLine("i.EgoInfo.Luck Exp = {0};", EgoInfo.LuckExp);
					sb.AppendLine("i.EgoInfo.Awakening Energy = {0};", EgoInfo.AwakeningEnergy);
					sb.AppendLine("i.EgoInfo.Awakening Exp = {0};", EgoInfo.AwakeningExp);

					sb.AppendLine();
				}

				sb.AppendLine("i.MetaData1 = new MabiDictionary(\"{0}\");", MetaData1.ToString());
				sb.AppendLine("i.MetaData2 = new MabiDictionary(\"{0}\");", MetaData2.ToString());

			}

			return sb.ToString();
		}

		public static void Parse(Packet packet, Item item)
		{
			item.EntityId = packet.GetLong();
			item.KnowledgeLevel = (ItemPacketType)packet.GetByte();
			item.ItemInfo = packet.GetObj<ItemInfo>();

			if (item.KnowledgeLevel == ItemPacketType.Public)
			{
				packet.GetByte();
				packet.GetByte();

				if ((packet.GetByte() & 1) != 0)
					item.SizeMultiplier = packet.GetFloat();

				item.BounceStyle = (ItemBounceStyle)packet.GetByte();
			}
			else
			{
				item.OptionInfo = packet.GetObj<ItemOptionInfo>();

				var possibleEgo = packet.GetString();

				if (packet.NextIs(PacketElementType.Byte)) // Yup. It's an ego
				{
					var ego = new EgoInfo();

					ego.Name = possibleEgo;
					ego.Race = (EgoRace)packet.GetByte();
					ego.Fullness = packet.GetByte();

					ego.SocialLevel = packet.GetByte();
					ego.SocialExp = packet.GetInt();
					ego.StrLevel = packet.GetByte();
					ego.StrExp = packet.GetInt();
					ego.IntLevel = packet.GetByte();
					ego.IntExp = packet.GetInt();
					ego.DexLevel = packet.GetByte();
					ego.DexExp = packet.GetInt();
					ego.WillLevel = packet.GetByte();
					ego.WillExp = packet.GetInt();
					ego.LuckLevel = packet.GetByte();
					ego.LuckExp = packet.GetInt();
					ego.AwakeningEnergy = packet.GetByte();
					ego.AwakeningExp = packet.GetInt();

					packet.GetLong();
					ego.LastFeeding = packet.GetDate();
					packet.GetInt();

					item.EgoInfo = ego;

					item.MetaData1 = new MabiDictionary(packet.GetString());
				}
				else
				{
					item.MetaData1 = new MabiDictionary(possibleEgo);
				}

				item.MetaData2 = new MabiDictionary(packet.GetString());
			}
		}
	}

	public enum ItemBounceStyle : byte
	{
		NoBounce = 0,
		Bounce = 1,
		DelayedBounce = 2
	}

	public enum ItemPacketType : byte
	{
		/// <summary>
		/// Used in public appears, doesn't include option info.
		/// </summary>
		Public = 1,

		/// <summary>
		/// Used in private item packets, includes option info.
		/// </summary>
		Private = 2
	}

	public class EgoInfo
	{
		/// <summary>
		/// Ego's race, displayed in stat window.
		/// </summary>
		public EgoRace Race { get; set; }

		/// <summary>
		/// Ego's name, displayed in stat window.
		/// </summary>
		public string Name { get; set; }

		public byte Fullness { get; set; }

		/// <summary>
		/// Ego's strength level
		/// </summary>
		public byte StrLevel { get; set; }

		/// <summary>
		/// Ego's strength exp
		/// </summary>
		public int StrExp { get; set; }

		/// <summary>
		/// Ego's intelligence level
		/// </summary>
		public byte IntLevel { get; set; }

		/// <summary>
		/// Ego's intelligence exp
		/// </summary>
		public int IntExp { get; set; }

		/// <summary>
		/// Ego's dexterity level
		/// </summary>
		public byte DexLevel { get; set; }

		/// <summary>
		/// Ego's dexterity exp
		/// </summary>
		public int DexExp { get; set; }

		/// <summary>
		/// Ego's will level
		/// </summary>
		public byte WillLevel { get; set; }

		/// <summary>
		/// Ego's will exp
		/// </summary>
		public int WillExp { get; set; }

		/// <summary>
		/// Ego's luck level
		/// </summary>
		public byte LuckLevel { get; set; }

		/// <summary>
		/// Ego's luck exp
		/// </summary>
		public int LuckExp { get; set; }

		/// <summary>
		/// Ego's social level
		/// </summary>
		public byte SocialLevel { get; set; }

		/// <summary>
		/// Ego's social exp
		/// </summary>
		public int SocialExp { get; set; }

		/// <summary>
		/// Awakening energy counter
		/// </summary>
		public byte AwakeningEnergy { get; set; }

		/// <summary>
		/// Awakening exp
		/// </summary>
		public int AwakeningExp { get; set; }

		/// <summary>
		/// Time the ego was fed last
		/// </summary>
		public DateTime LastFeeding { get; set; }
	}

	public enum EgoRace : byte
	{
		None = 0,
		MaleSword = 1,
		FemaleSword = 2,
		MaleBlunt = 3,
		FemaleBlunt = 4,
		MaleWand = 5,
		FemaleWand = 6,
		MaleBow = 7,
		FemaleBow = 8,
		EirySword = 9,
		EiryBow = 10,
		EiryAxe = 11,
		EiryLute = 12,
		EiryCylinder = 13,
		EiryWand = 14, // ?
		MaleCylinder = 15,
		FemaleCylinder = 16,
	}
}
