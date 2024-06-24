using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;

namespace Damath.EOS.Lobbies
{
    /// <summary>
    /// Class represents all Lobby Attribute properties
    /// </summary>
    public class LobbyAttribute
    {
        public LobbyAttributeVisibility Visibility = LobbyAttributeVisibility.Public;

        public AttributeType ValueType = AttributeType.String;

        //Key is uppercased when transmitted, so this should be uppercase
        public string Key;

        //Only one of the following properties will have valid data (depending on 'ValueType')
        public long? AsInt64 = 0;
        public double? AsDouble = 0.0;
        public bool? AsBool = false;
        public string AsString;

        public AttributeData AsAttribute
        {
            get
            {
                AttributeData attrData = new AttributeData();
                attrData.Key = Key;
                attrData.Value = new AttributeDataValue();

                switch (ValueType)
                {
                    case AttributeType.String:
                        attrData.Value = AsString;
                        break;
                    case AttributeType.Int64:
                        attrData.Value = (AttributeDataValue)AsInt64;
                        break;
                    case AttributeType.Double:
                        attrData.Value = (AttributeDataValue)AsDouble;
                        break;
                    case AttributeType.Boolean:
                        attrData.Value = (AttributeDataValue)AsBool;
                        break;
                }

                return attrData;
            }
        }

        public override bool Equals(object other)
        {
            LobbyAttribute lobbyAttr = (LobbyAttribute)other;

            return ValueType == lobbyAttr.ValueType &&
                AsInt64 == lobbyAttr.AsInt64 &&
                AsDouble == lobbyAttr.AsDouble &&
                AsBool == lobbyAttr.AsBool &&
                AsString == lobbyAttr.AsString &&
                Key == lobbyAttr.Key &&
                Visibility == lobbyAttr.Visibility;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void InitFromAttribute(Epic.OnlineServices.Lobby.Attribute? attributeParam)
        {
            AttributeData attributeData = (AttributeData)(attributeParam?.Data);

            Key = attributeData.Key;
            ValueType = attributeData.Value.ValueType;

            switch (attributeData.Value.ValueType)
            {
                case AttributeType.Boolean:
                    AsBool = attributeData.Value.AsBool;
                    break;
                case AttributeType.Int64:
                    AsInt64 = attributeData.Value.AsInt64;
                    break;
                case AttributeType.Double:
                    AsDouble = attributeData.Value.AsDouble;
                    break;
                case AttributeType.String:
                    AsString = attributeData.Value.AsUtf8;
                    break;
            }
        }
    }

}