using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOI_PPM
{
    public class Component
    {
        public string AOIName { get; set; }
        public string VALORName { get; set; }
        public long Card_ID { get; set; }
        public string Card_Bar_Code { get; set; }
        public long Panel_ID { get; set; }
        public long Tested_ID { get; set; }
        public DateTime StartTime { get; set; }
        public string Topology { get; set; }
        public int ErrorTableAR { get; set; }
        public int RepairStateResult { get; set; }
        public string Reference { get; set; }
        public string Serie { get; set; }
        public string ProductName { get; set; }
        public string ErrorName { get; set; }

        public Component(Component component)
        {
            AOIName = component.AOIName;
            VALORName = component.VALORName;
            Card_ID = component.Card_ID;
            Card_Bar_Code = component.Card_Bar_Code;
            Panel_ID = component.Panel_ID;
            Tested_ID = component.Tested_ID;
            StartTime = component.StartTime;
            Topology = component.Topology;
            ErrorTableAR = component.ErrorTableAR;
            RepairStateResult = component.RepairStateResult;
            Reference = component.Reference;
            Serie = component.Serie;
            ProductName = component.ProductName;
            ErrorName = component.ErrorName;
        }

        public Component(string aoiname, string valorname, long cardid, string cardbarcode, long panelid, long testedid, DateTime starttime, string topology, int errortablear, int oldtableear, int repairstateresult, string reference, string productname, int type)
        {
            AOIName = aoiname;
            VALORName = valorname;
            Card_ID = cardid;
            Card_Bar_Code = cardbarcode;
            Panel_ID = panelid;
            Tested_ID = testedid;
            StartTime = new DateTime(starttime.Year, starttime.Month, starttime.Day);
            Topology = topology;
            ErrorTableAR = errortablear > 0? errortablear : (oldtableear > 0? oldtableear : 0);
            RepairStateResult = repairstateresult;
            Reference = reference;
            ProductName = productname;
            Serie = "serie1";

            ErrorName = "Unknown";

            if ((type == 0) || (type == 1))
            {
                if ((ErrorTableAR / 1) % 2 == 1) ErrorName = "Missing";
                if ((ErrorTableAR / 2) % 2 == 1) ErrorName = "Polarity";
                if ((ErrorTableAR / 16) % 2 == 1) ErrorName = "OCV";
                if ((ErrorTableAR / 64) % 2 == 1) ErrorName = "Delta X";
                if ((ErrorTableAR / 128) % 2 == 1) ErrorName = "Delta Y";
                if ((ErrorTableAR / 256) % 2 == 1) ErrorName = "Delta Theta";
                if ((ErrorTableAR / 512) % 2 == 1) ErrorName = "Thickness";
                if ((ErrorTableAR / 2048) % 2 == 1) ErrorName = "element skipped";
                if ((ErrorTableAR / 524288) % 2 == 1) ErrorName = "Tilt";
                if ((ErrorTableAR / 1048576) % 2 == 1) ErrorName = "Side Overhang";
                if ((ErrorTableAR / 2097152) % 2 == 1) ErrorName = "Length Overhang";
                if ((ErrorTableAR / 4194304) % 2 == 1) ErrorName = "Foreign Material";
                if ((ErrorTableAR / 8388608) % 2 == 1) ErrorName = "Component Present";
                if ((ErrorTableAR / 16777216) % 2 == 1) ErrorName = "Lifted lead(refer to PIN Table)";
            }
            if ((type == 0) || (type == 2))
            {
                if ((ErrorTableAR / 4) % 2 == 1) ErrorName = "Solder Joint";
                if ((ErrorTableAR / 8) % 2 == 1) ErrorName = "Solder Bridge";
                if ((ErrorTableAR / 1024) % 2 == 1) ErrorName = "Paste surface area out of range";
            }
        }
    }

    class MyComponentComparer : IEqualityComparer<Component>
    {
        public bool Equals(Component x, Component y)
        {
            return x.AOIName == y.AOIName && x.Reference == y.Reference && x.Topology == y.Topology && x.Card_ID == y.Card_ID;
        }

        public int GetHashCode(Component myModel)
        {
            int hashAOIName = myModel.AOIName.GetHashCode();
            int hashReference = myModel.Reference.GetHashCode();
            int hashTopology = myModel.Topology.GetHashCode();
            int hashCard_ID = myModel.Card_ID.GetHashCode();

            return hashAOIName ^ hashReference ^ hashTopology ^ hashCard_ID;
        }
    }

    public class ComponentWithValor : Component
    {
        public string MC { get; set; }
        public string Station { get; set; }
        public string Slot { get; set; }
        public int Subslot { get; set; }
        public int TypeFeeder { get; set; }
        public ComponentWithValor(string aoiname, string valorname, long cardid, string cardbarcode, long panelid, long testedid, DateTime starttime, string topology, 
            int errortablear, int olderrortablear, int repairstateresult, string reference, string productname,
            string mc, string station, string slot, int subslot, int typefeeder, int type)
            : base(aoiname, valorname, cardid, cardbarcode, panelid, testedid, starttime, topology, errortablear, olderrortablear, repairstateresult, reference, productname, type)
        {
            MC = mc;
            Station = station;
            Slot = slot;
            Subslot = subslot;
            TypeFeeder = typefeeder;
        }

        public ComponentWithValor(ComponentWithValor component):base(component)
        {
            MC = component.MC;
            Station = component.Station;
            Slot = component.Slot;
            Subslot = component.Subslot;
            TypeFeeder = component.TypeFeeder;
        }
    }

    class MyComponentWithValorComparer : IEqualityComparer<ComponentWithValor>
    {
        public bool Equals(ComponentWithValor x, ComponentWithValor y)
        {
            return x.AOIName == y.AOIName && x.Reference == y.Reference && x.Topology == y.Topology && x.Card_ID == y.Card_ID;
        }

        public int GetHashCode(ComponentWithValor myModel)
        {
            int hashAOIName = myModel.AOIName.GetHashCode();
            int hashReference = myModel.Reference.GetHashCode();
            int hashTopology = myModel.Topology.GetHashCode();
            int hashCard_ID = myModel.Card_ID.GetHashCode();

            return hashAOIName ^ hashReference ^ hashTopology ^ hashCard_ID;
        }
    }

    public class AOIQty
    {
        public string AOIName { get; set; }
        public int PCBQty { get; set; }
        public int AllComponents { get; set; }
        public int AllPads { get; set; }

        public AOIQty(string aoiname, int pcbqty, int allcomponents, int allpads)
        {
            AOIName = aoiname;
            PCBQty = pcbqty;
            AllComponents = allcomponents;
            AllPads = allpads;
        }
    }

    class MyModelTheUniqueIDComparer : IEqualityComparer<AllAOIInfoPlacement>
    {
        public bool Equals(AllAOIInfoPlacement x, AllAOIInfoPlacement y)
        {
            return x.AOIName == y.AOIName;
        }

        public int GetHashCode(AllAOIInfoPlacement myModel)
        {
            return myModel.AOIName.GetHashCode();
        }
    }


    [MetadataType(typeof(AllAOIInfoPlacement))]
    public class AllAOIInfoPlacement
    {
        [Display(Order = 0)]
        public string AOIName { get; set; }
        [Display(Order = 1)]
        public int AllComponents { get; set; }
        [Display(Order = 2)]
        public int PCBQty { get; set; }
        [Display(Order = 3)]

        public int AllPads { get; set; }
        [Display(Order = 4)]


        public int QtyAllPlacement { get; set; }
        [Display(Order = 5)]
        public int QtyValorNotFoundPlacement { get; set; }
        [Display(Order = 6)]
        public int QtyPlacement { get; set; }
        [Display(Order = 7)]
        public double PPMPlacement { get; set; }

        public AllAOIInfoPlacement(string aoiname, int qtyall, int qtyvalornotfound, int pcbqty, int allcomponents, int allpads, double ppm)
        {
            AOIName = aoiname;
            PCBQty = pcbqty;
            AllComponents = allcomponents;
            AllPads = allpads;
            QtyAllPlacement = qtyall;
            QtyValorNotFoundPlacement = qtyvalornotfound;
            QtyPlacement = qtyall - qtyvalornotfound;
            PPMPlacement = ppm;
        }
    }

    [MetadataType(typeof(AllAOIInfoPlacementSoldering))]
    public class AllAOIInfoPlacementSoldering : AllAOIInfoPlacement
    {
        [Display(Order = 8)]
        public int QtyAllReflow { get; set; }
        [Display(Order = 9)]
        public int QtyValorNotFoundReflow { get; set; }
        [Display(Order = 10)]
        public int QtyReflow { get; set; }
        [Display(Order = 11)]
        public double PPMReflow { get; set; }


        public AllAOIInfoPlacementSoldering(string aoiname, int qtyall, int qtyvalornotfound, int pcbqty, int allcomponents, int allpads, double ppm, int qtyallS, int qtyvalornotfoundS, double ppmS)
            : base(aoiname, qtyall, qtyvalornotfound, pcbqty, allcomponents, allpads, ppm)
        {
            QtyAllReflow = qtyallS;
            QtyValorNotFoundReflow = qtyvalornotfoundS;
            QtyReflow = qtyallS - qtyvalornotfoundS;
            PPMReflow = ppmS;
        }
    }

    public class ValorVITLine
    {
        public string ValorLine { get; set; }
        public string VITLine { get; set; }

        public ValorVITLine(string valor, string vit)
        {
            ValorLine = valor;
            VITLine = vit;
        }
    }

    public class MCStationSlotFeeder
    {
        public string Reference { get; set; }
        public string VALORName { get; set; }
        public string Topology { get; set; }
        public string MC { get; set; }
        public string Station { get; set; }
        public string Slot { get; set; }
        public int Subslot { get; set; }
        public int TypeFeeder { get; set; }
        public MCStationSlotFeeder(string reference, string line, string topology, string mc, string station, string slot, int subslot, int typefeeder)
        {
            Reference = reference;
            VALORName = line;
            Topology = topology;
            MC = mc;
            Station = station;
            Slot = slot;
            Subslot = subslot;
            TypeFeeder = typefeeder;
        }
    }

    public class History
    {
        public DateTime StartTime { get; set; }
        public int Qty { get; set; }

        public History(DateTime starttime, int qty)
        {
            StartTime = starttime;
            Qty = qty;
        }
    }

    public class AOIPads
    {
        public string AOIName { get; set; }
        public int Pads { get; set; }

        public AOIPads(string aoiname, int pads)
        {
            AOIName = aoiname;
            Pads = pads;
        }
    }

    public class GroupedData
    {
        public string AOIName { get; set; }
        public long Card_ID { get; set; } 
        public int Qty { get; set; }
    }
}
