using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace VpMobilPlusPlus.VpAPI;

// using System.Xml.Serialization;
// XmlSerializer serializer = new XmlSerializer(typeof(VpMobil));
// using (StringReader reader = new StringReader(xml))
// {
//    var test = (VpMobil)serializer.Deserialize(reader);
// }

[XmlRoot(ElementName="Kopf")]
public class Kopf { 

	[XmlElement(ElementName="planart")] 
	public string Planart { get; set; } 

	[XmlElement(ElementName="zeitstempel")] 
	public string Zeitstempel { get; set; } 

	[XmlElement(ElementName="DatumPlan")] 
	public string DatumPlan { get; set; } 

	[XmlElement(ElementName="datei")] 
	public string Datei { get; set; } 

	[XmlElement(ElementName="nativ")] 
	public int Nativ { get; set; } 

	[XmlElement(ElementName="woche")] 
	public int Woche { get; set; } 

	[XmlElement(ElementName="tageprowoche")] 
	public int Tageprowoche { get; set; } 

	[XmlElement(ElementName="schulnummer")] 
	public object Schulnummer { get; set; } 
}

[XmlRoot(ElementName="FreieTage")]
public class FreieTage { 

	[XmlElement(ElementName="ft")] 
	public List<int> Ft { get; set; } 
}

[XmlRoot(ElementName="KlSt")]
public class KlSt { 

	[XmlAttribute(AttributeName="ZeitVon")] 
	public string ZeitVon { get; set; } 

	[XmlAttribute(AttributeName="ZeitBis")] 
	public string ZeitBis { get; set; } 

	[XmlText] 
	public int Text { get; set; } 
}

[XmlRoot(ElementName="KlStunden")]
public class KlStunden { 

	[XmlElement(ElementName="KlSt")] 
	public List<KlSt> KlSt { get; set; } 
}

[XmlRoot(ElementName="KKz")]
public class KKz { 

	[XmlAttribute(AttributeName="KLe")] 
	public string KLe { get; set; } 

	[XmlText] 
	public string Text { get; set; } 
}

[XmlRoot(ElementName="Ku")]
public class Ku { 

	[XmlElement(ElementName="KKz")] 
	public KKz KKz { get; set; } 
}

[XmlRoot(ElementName="Kurse")]
public class Kurse { 

	[XmlElement(ElementName="Ku")] 
	public List<Ku> Ku { get; set; } 
}

[XmlRoot(ElementName="UeNr")]
public class UeNr { 

	[XmlAttribute(AttributeName="UeLe")] 
	public string UeLe { get; set; } 

	[XmlAttribute(AttributeName="UeFa")] 
	public string UeFa { get; set; } 

	[XmlText] 
	public int Text { get; set; } 

	[XmlAttribute(AttributeName="UeGr")] 
	public string UeGr { get; set; } 
}

[XmlRoot(ElementName="Ue")]
public class Ue { 

	[XmlElement(ElementName="UeNr")] 
	public UeNr UeNr { get; set; } 
}

[XmlRoot(ElementName="Unterricht")]
public class Unterricht { 

	[XmlElement(ElementName="Ue")] 
	public List<Ue> Ue { get; set; } 
}

[XmlRoot(ElementName="Std")]
public class Std { 

	[XmlElement(ElementName="Fa")] 
	public Fa Fa { get; set; } 

	[XmlElement(ElementName="Le")] 
	public Le Le { get; set; } 

	[XmlElement(ElementName="Ra")] 
	public Ra Ra { get; set; } 

	[XmlElement(ElementName="Nr")] 
	public int Nr { get; set; } 

	[XmlElement(ElementName="If")] 
	public string If { get; set; } 

	[XmlElement(ElementName="St")] 
	public int St { get; set; } 

	[XmlElement(ElementName="Beginn")] 
	public string Beginn { get; set; } 

	[XmlElement(ElementName="Ende")] 
	public string Ende { get; set; } 

	[XmlElement(ElementName="Ku2")] 
	public string Ku2 { get; set; } 
}

[XmlRoot(ElementName="Fa")]
public class Fa { 

	[XmlAttribute(AttributeName="FaAe")] 
	public string FaAe { get; set; } 

	[XmlText] 
	public string Text { get; set; } 
}

[XmlRoot(ElementName="Le")]
public class Le { 

	[XmlAttribute(AttributeName="LeAe")] 
	public string LeAe { get; set; } 

	[XmlText] 
	public string Text { get; set; } 
}

[XmlRoot(ElementName="Ra")]
public class Ra { 

	[XmlAttribute(AttributeName="RaAe")] 
	public string RaAe { get; set; } 

	[XmlText] 
	public string Text { get; set; } 
}

[XmlRoot(ElementName="Pl")]
public class Pl { 

	[XmlElement(ElementName="Std")] 
	public List<Std> Std { get; set; } 
}

[XmlRoot(ElementName="Kl")]
public class Kl { 

	[XmlElement(ElementName="Kurz")] 
	public string Kurz { get; set; } 

	[XmlElement(ElementName="Hash")] 
	public object Hash { get; set; } 

	[XmlElement(ElementName="KlStunden")] 
	public KlStunden KlStunden { get; set; } 

	[XmlElement(ElementName="Kurse")] 
	public Kurse Kurse { get; set; } 

	[XmlElement(ElementName="Unterricht")] 
	public Unterricht Unterricht { get; set; } 

	[XmlElement(ElementName="Pl")] 
	public Pl Pl { get; set; } 
}

[XmlRoot(ElementName="Klassen")]
public class Klassen { 

	[XmlElement(ElementName="Kl")] 
	public List<Kl> Kl { get; set; } 
}

[XmlRoot(ElementName="ZusatzInfo")]
public class ZusatzInfo { 

	[XmlElement(ElementName="ZiZeile")] 
	public List<string> ZiZeile { get; set; } 
}

[XmlRoot(ElementName="VpMobil")]
public class VpMobil { 

	[XmlElement(ElementName="Kopf")] 
	public Kopf Kopf { get; set; } 

	[XmlElement(ElementName="FreieTage")] 
	public FreieTage FreieTage { get; set; } 

	[XmlElement(ElementName="Klassen")] 
	public Klassen Klassen { get; set; } 

	[XmlElement(ElementName="ZusatzInfo")] 
	public ZusatzInfo ZusatzInfo { get; set; } 
}

