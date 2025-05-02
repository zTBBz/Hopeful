using System;
using System.Xml;
using System.Xml.Serialization;

namespace Hopeful;

[Serializable]
public struct GameSetting
{
    [XmlElement("dev-mode")]
    public bool DevMode { get; internal set; }

    [XmlElement("resolution")]
    public Resolution Resolution { get; internal set; }

    public GameSetting(Resolution? resolution = null, bool dev = false)
    {
        Resolution = resolution ?? Resolution.FullHD;
        DevMode = dev;
    }
}
