using MabiLib.Const;
using MabiLib.Structs;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MabiPale2.Shared;

namespace MabiPale2.Plugins.EntityLogger.Entities
{
	[ImplementPropertyChanged]
	public class Prop : Entity
	{
		public int Id { get; set; }
		public string State { get; set; }
		public string Xml { get; set; }

		// Server only
		public string Title { get; set; }
		public PropInfo PropInfo { get; set; }

		// Client only
		public float Direction { get; set; }

		public override string EntityType
		{
			get { return "Prop"; }
		}

		public bool IsServerProp
		{
			get { return EntityId >= MabiId.ServerProps; }
		}

		protected override string GenerateInfo()
		{
			var sb = new StringBuilder();

			sb.AppendLine((IsServerProp ? "Server" : "Client") + " sided prop");
			sb.AppendLine();

			sb.AppendLine("Entity id: {0:X8}", EntityId);
			sb.AppendLine("Prop id: {0}", Id);
			sb.AppendLine("State: {0}", State);
			sb.AppendLine("XML: {0}", Xml);

			if (IsServerProp)
			{
				sb.AppendLine("Name: {0}", Name);
				sb.AppendLine("Title: {0}", Title);
				sb.AppendLine("Info: ");
				sb.AppendLine("   Altitude: {0:0.########}", PropInfo.Altitude);
				sb.AppendLine("   Color1: 0x{0:X8}", PropInfo.Color1);
				sb.AppendLine("   Color2: 0x{0:X8}", PropInfo.Color2);
				sb.AppendLine("   Color3: 0x{0:X8}", PropInfo.Color3);
				sb.AppendLine("   Color4: 0x{0:X8}", PropInfo.Color4);
				sb.AppendLine("   Color5: 0x{0:X8}", PropInfo.Color5);
				sb.AppendLine("   Color6: 0x{0:X8}", PropInfo.Color6);
				sb.AppendLine("   Color7: 0x{0:X8}", PropInfo.Color7);
				sb.AppendLine("   Color8: 0x{0:X8}", PropInfo.Color8);
				sb.AppendLine("   Color9: 0x{0:X8}", PropInfo.Color9);
				sb.AppendLine("   Direction: {0:0.########}", PropInfo.Direction);
				sb.AppendLine("   FixedAltitude: {0}", PropInfo.FixedAltitude);
				sb.AppendLine("   Id: {0}", PropInfo.Id);
				sb.AppendLine("   Region: {0}", PropInfo.Region);
				sb.AppendLine("   Scale: {0:0.########}", PropInfo.Scale);
				sb.AppendLine("   X: {0:0.########}", PropInfo.X);
				sb.AppendLine("   Y: {0:0.########}", PropInfo.Y);
			}
			else
				sb.AppendLine("Direction: {0}", Direction);

			return sb.ToString();
		}

		protected override string GenerateScript()
		{
			var sb = new StringBuilder();

			if (IsServerProp)
			{
				sb.AppendLine("var prop = SpawnProp({0}, {1}, {2}, {3}, {4:0.########f}, {5:0.########f});", Id, PropInfo.Region, PropInfo.X, PropInfo.Y, PropInfo.Direction, PropInfo.Scale);

				if (!string.IsNullOrWhiteSpace(State)) sb.AppendLine("prop.State = \"{0}\"", State);
				if (!string.IsNullOrWhiteSpace(Xml)) sb.AppendLine("prop.Xml = XElement.Parse(\"{0}\");", Xml.Replace("\"", "\\\""));
				if (!string.IsNullOrWhiteSpace(Name)) sb.AppendLine("prop.Name = \"{0}\";", Name);
				if (!string.IsNullOrWhiteSpace(Title)) sb.AppendLine("prop.Title = \"{0}\";", Title);

				if (PropInfo.Altitude != 0) sb.AppendLine("prop.Info.Altitude = {0:0.########f};", PropInfo.Altitude);
				if (PropInfo.FixedAltitude != 0) sb.AppendLine("prop.Info.FixedAltitude = {0};", PropInfo.FixedAltitude);

				if (PropInfo.Color1 != 0xFF808080) sb.AppendFormat("prop.Info.Color1 = 0x{0:X8};", PropInfo.Color1);
				if (PropInfo.Color2 != 0xFF808080) sb.AppendFormat("prop.Info.Color2 = 0x{0:X8};", PropInfo.Color2);
				if (PropInfo.Color3 != 0xFF808080) sb.AppendFormat("prop.Info.Color3 = 0x{0:X8};", PropInfo.Color3);
				if (PropInfo.Color4 != 0xFF808080) sb.AppendFormat("prop.Info.Color4 = 0x{0:X8};", PropInfo.Color4);
				if (PropInfo.Color5 != 0xFF808080) sb.AppendFormat("prop.Info.Color5 = 0x{0:X8};", PropInfo.Color5);
				if (PropInfo.Color6 != 0xFF808080) sb.AppendFormat("prop.Info.Color6 = 0x{0:X8};", PropInfo.Color6);
				if (PropInfo.Color7 != 0xFF808080) sb.AppendFormat("prop.Info.Color7 = 0x{0:X8};", PropInfo.Color7);
				if (PropInfo.Color8 != 0xFF808080) sb.AppendFormat("prop.Info.Color8 = 0x{0:X8};", PropInfo.Color8);
				if (PropInfo.Color9 != 0xFF808080) sb.AppendFormat("prop.Info.Color9 = 0x{0:X8};", PropInfo.Color9);
			}
			else
			{
				var format =
@"SetPropBehavior(0x{0:X8}, (creature, prop) =>
{{
	// On interaction...
}});
";
				sb.AppendFormat(format, EntityId);
			}

			return sb.ToString();
		}
	}
}
