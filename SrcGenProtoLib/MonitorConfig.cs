using System.Collections.Generic;
using System.Text;

namespace SrcGenProtoLib
{
    public class MonitorConfig
    {
        public string MonitorName;
        public string RoutingId;
        public List<StateCheckConfig> DimensionNSSVerification;
        public List<StateCheckConfig> DimensionLSSVerification;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(MonitorName);
            sb.AppendLine("================================");
            sb.AppendLine($"RoutingId: {RoutingId}");

            sb.AppendLine("NSSChecks:");
            AppendList(sb, DimensionNSSVerification);

            sb.AppendLine("LSSChecks:");
            AppendList(sb, DimensionLSSVerification);

            return sb.ToString();
        }

        public static void AppendList(StringBuilder sb, List<StateCheckConfig> list)
        {
            if (list is null || list.Count == 0)
            {
                return;
            }

            foreach (var nssCheck in list)
            {
                sb.Append(nssCheck.ToString());
            }
        }
    }

    public class StateCheckConfig
    {
        public string Dimension;
        public string RequiredState;
        public string Delimiter;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append('\t');
            sb.AppendLine(Dimension);
            sb.Append('\t');
            sb.Append('\t');
            sb.AppendLine($"Required State: {RequiredState}");
            sb.Append('\t');
            sb.Append('\t');
            sb.AppendLine($"Delimiter: {Delimiter}");

            return sb.ToString();
        }
    }
}