using DevExpress.Spreadsheet;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AOI_PPM
{
    public partial class MainForm : Form
    {
        private enum GroupType : int
        {
            ALL,
            GROUP_MISSING_DELTA,
            GROUP_MISSING_DELTA_TILT,
            GROUP_MISSING_DELTA_TILT_THICKNESS,
            GROUP_ALL
        }

        private SqlConnection _DBConnectionAOI = null;
        private SqlConnection _DBConnectionProduction = null;
        private SqlConnection _DBConnectionValor = null;

        private List<Component> _listComponentsPlacement = null;
        private List<Component> _listComponentsSoldering = null;
        private List<AOIQty> _listAOIQty = null;
        private List<ValorVITLine> _listValorVITLines = null;
        private List<ComponentWithValor> _listComponentsForSelectedAOIPlacement = null;
        private List<ComponentWithValor> _listComponentsForSelectedAOISoldering = null;
        
        private List<AllAOIInfoPlacementSoldering> _mainSumary = null;

        private string _selectedAOIName = "";

        private string _selectedMCPlacement = "";
        private string _selectedStationPlacement = "";
        private string _selectedSlotPlacement = "";
        private string _selectedTopologyPlacement = "";

        private string _selectedMCSoldering = "";
        private string _selectedStationSoldering = "";
        private string _selectedSlotSoldering = "";
        private string _selectedTopologySoldering = "";

        private Color _placementColor = Color.FromArgb(255, 160, 140);
        private Color _solderingColor = Color.FromArgb(230, 230, 0);

        private GroupType _allComponentsGroupType = GroupType.ALL;

        private Dictionary<string, List<ComponentWithValor>> _valorCache = new Dictionary<string, List<ComponentWithValor>>();
        List<ComponentData> _updatedComponentsForSelectedAOIPlacement;
        List<ComponentData> _updatedComponentsForSelectedAOISoldering;

        //Remove placement related
        //Soldering premenovat na Reflow PPM
        /*

        3.1.2022 06:00 -> 9.2.2022 06:00
        Card ID = 12572312
        Pato je nema v zozname
        
        select* from PanelViewWithProgramUTC where Card_Id = 12572312
        select* from TESTED_OBJECT where Card_Id = 12572312
        select* from TESTED_OBJECT_HISTO where Tested_Object_Id = 2965611
        */



        public MainForm()
        {
            InitializeComponent();

        }

        public bool init(SqlConnection sqlConnectionAOI, SqlConnection sqlConnectionProduction, SqlConnection sqlConnectionValor)
        {
            _DBConnectionAOI = sqlConnectionAOI;
            _DBConnectionProduction = sqlConnectionProduction;
            _DBConnectionValor = sqlConnectionValor;
            return true;
        }      

        private string convertAOIName(string aoiname)
        {
            if (aoiname == "28E1AOI1") return "AOI-SMT08";
            if (aoiname == "28E2AOI1") return "AOI-SMT07";
            if (aoiname == "28E3AOI1") return "AOI-SMT06";
            if (aoiname == "AOI-SMT1") return "AOI-SMT01";
            if (aoiname == "AOI-SMT2") return "AOI-SMT02";
            if (aoiname == "AOI-SMT3") return "AOI-SMT03";
            if (aoiname == "AOI-SMT4") return "AOI-SMT04";
            if (aoiname == "AOI-SMT5") return "AOI-SMT05";
            if (aoiname == "AOI-SMT9") return "AOI-SMT09";

            if (aoiname == "SMT14-AOI") return "AOI-SMT14";
            if (aoiname == "SMT5_AOI")  return "AOI-SMT05";
            if (aoiname == "SMT5-AOI")  return "AOI-SMT05";
            return aoiname;
        }

        private string unconvertAOIName(string aoiname)
        {
            if (aoiname == "AOI-SMT08") return "28E1AOI1";
            if (aoiname == "AOI-SMT07") return "28E2AOI1";
            if (aoiname == "AOI-SMT06") return "28E3AOI1";
            if (aoiname == "AOI-SMT01") return "AOI-SMT1";
            if (aoiname == "AOI-SMT02") return "AOI-SMT2";
            if (aoiname == "AOI-SMT03") return "AOI-SMT3";
            if (aoiname == "AOI-SMT04") return "AOI-SMT4";
            if (aoiname == "AOI-SMT05") return "AOI-SMT05";
            if (aoiname == "AOI-SMT09") return "AOI-SMT09";

            return aoiname;
        }

        private string getBookmark()
        {
            if (gridView1.FocusedRowHandle != GridControl.InvalidRowHandle)
                return gridView1.GetRowCellDisplayText(gridView1.FocusedRowHandle, gridView1.Columns["AOIName"]);
            return "";
        }

        private void gotoBookmark(string bookmark)
        {
            try
            {
                int FocusedRowHandle = gridView1.LocateByDisplayText(0, gridView1.Columns["AOIName"], bookmark);
                if (FocusedRowHandle != GridControl.InvalidRowHandle)
                {
                    gridView1.OptionsSelection.MultiSelect = false;
                    gridView1.ClearSelection();
                    gridView1.FocusedRowHandle = FocusedRowHandle;
                    gridView1.OptionsSelection.MultiSelect = true;
                    gridView1.SelectRow(FocusedRowHandle);
                }
            }
            catch (Exception) {; }
        }

        private void enableControls(bool enable)
        {
            checkBox1.Enabled = enable;
            checkBox7.Enabled = enable;
            panel2.Enabled = enable;
            panel3.Enabled = enable;
            panel4.Enabled = enable;
            panel6.Enabled = enable;
        }

        /// <summary>
        /// //////////////
        /// </summary>
        /// <param name="reloadFromDB"></param>
        /// <param name=""></param>
        private void refreshMainData(bool reloadFromDB)
        {
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            if (reloadFromDB)
            {
                prepareValorVITLines();
                prepareAllComponentsFromDB();
                prepareAllAOIQty();
            }

            string bookmark = getBookmark();

            gridControl1.DataSource = _mainSumary = getMainSummaryGroupByAOIName();

            gotoBookmark(bookmark);


            //Add Sumary
            GridColumnSummaryItem item1 = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, "QtyPlacement", "SUM = {0}");
            item1.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            gridView1.Columns["QtyPlacement"].Summary.Clear();
            gridView1.Columns["QtyPlacement"].Summary.Add(item1);

            GridColumnSummaryItem item2 = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, "QtyReflow", "SUM = {0}");
            item2.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            gridView1.Columns["QtyReflow"].Summary.Clear();
            gridView1.Columns["QtyReflow"].Summary.Add(item2);


            drawGridViewSymbols("DeltaPlacement", "PPMPlacement");
            drawGridViewSymbols("DeltaReflow", "PPMReflow");


            gridView1_Click_1(null, null);
            this.Cursor = Cursors.Arrow;
        }

        private void drawGridViewSymbols(string name1, string name2)
        {
            //Draw symbols
            GridFormatRule gridFormatRule = new GridFormatRule();
            FormatConditionRuleIconSet formatConditionRuleIconSet1 = new FormatConditionRuleIconSet();
            formatConditionRuleIconSet1.IconSet = new FormatConditionIconSet();
            FormatConditionIconSet iconSet = formatConditionRuleIconSet1.IconSet;
            var icon1 = new FormatConditionIconSetIcon();
            var icon2 = new FormatConditionIconSetIcon();
            var icon3 = new FormatConditionIconSetIcon();

            icon1.PredefinedName = "Triangles3_1.png";
            icon2.PredefinedName = "Triangles3_2.png";
            icon3.PredefinedName = "Triangles3_3.png";

            iconSet.ValueType = FormatConditionValueType.Number;
            icon1.Value = -200;
            icon1.ValueComparison = FormatConditionComparisonType.GreaterOrEqual;
            icon2.Value = 0;
            icon2.ValueComparison = FormatConditionComparisonType.GreaterOrEqual;
            icon3.Value = 0;
            icon3.ValueComparison = FormatConditionComparisonType.Greater;

            iconSet.Icons.Add(icon1);
            iconSet.Icons.Add(icon2);
            iconSet.Icons.Add(icon3);

            gridFormatRule.Rule = formatConditionRuleIconSet1;
            gridFormatRule.Column = gridView1.Columns[name1];
            gridView1.FormatRules.Add(gridFormatRule);


            //Draw %
            gridFormatRule = new GridFormatRule();
            FormatConditionRuleDataBar formatConditionRuleDataBar = new FormatConditionRuleDataBar();
            gridFormatRule.Column = gridView1.Columns[name2];
            formatConditionRuleDataBar.PredefinedName = "Mint Gradient";
            gridFormatRule.Rule = formatConditionRuleDataBar;
            gridView1.FormatRules.Add(gridFormatRule);
        }

        private void prepareAllAOIQty()
        {
            DateTime dtFrom = Local2UTC(dateTimePickerFrom.Value);
            DateTime dtTo = Local2UTC(dateTimePickerTo.Value);

            //Get AOI PCB qty and Components Qty
            string sql = @"select Machine_Name,
                    count(Card_ID) as [Pcb qty], sum([Number_Of_Component]) as [ALL component] from PanelViewWithProgramUTC tPanelCard
                    where (tPanelCard.StartTime between '{0}' AND '{1}') and Card_Status <> 0
                    group by Machine_Name";
            sql = string.Format(sql, dtFrom.ToString("yyyy-MM-dd HH:mm"), dtTo.ToString("yyyy-MM-dd HH:mm"));
            DataTable table = HelperClass.callQuery(_DBConnectionAOI, sql);


            //Get AOI PCB Pads Qty
            sql = @"select Machine_Name, Product_Name_Product, count(*) as Pocet from PanelViewWithProgramUTC
                where (StartTime between '{0}' AND '{1}') and Card_Status <> 0
                group by Machine_Name, Product_Name_Product";
            sql = string.Format(sql, dtFrom.ToString("yyyy-MM-dd HH:mm"), dtTo.ToString("yyyy-MM-dd HH:mm"));
            DataTable tableAllProducts = HelperClass.callQuery(_DBConnectionAOI, sql);

            DataTable tableProductPads = HelperClass.callQuery(_DBConnectionProduction, @"select Product, Pads from VIT_PCB_Pads with(NOLOCK)");

            //Check relationships: if some missing in VIT_PCB_Pads -> add new with value 1
            int missing = handleProducts(tableAllProducts, tableProductPads);
            if (missing > 0)
            {
                MessageBox.Show(string.Format("Found new {0} product(s) and was added to table with value 1\nPress button 'Product/Pads' to edit number of pads.", missing));
                //Get again with new item(s)
                tableProductPads = HelperClass.callQuery(_DBConnectionProduction, @"select Product, Pads from VIT_PCB_Pads with(NOLOCK)");
            }

            try
            {
                //Join with Product->Pads table
                List<AOIPads> tmpList = (from a in tableAllProducts.AsEnumerable()
                                     join b in tableProductPads.AsEnumerable() on a["Product_Name_Product"].ToString() equals b["Product"].ToString() into c
                                     from x in c.DefaultIfEmpty()
                                     select new AOIPads(convertAOIName(a["Machine_Name"].ToString()), Convert.ToInt32(a["Pocet"]) * repairNumberOfPads(convertAOIName(a["Machine_Name"].ToString()), Convert.ToInt32(x["Pads"])))).ToList();
                                    //select new AOIPads(convertAOIName(a["Machine_Name"].ToString()), Convert.ToInt32(a["Pocet"]) * Convert.ToInt32(x["Pads"]))).ToList();

                //Group by AOINAme
                var tmpList2 = tmpList.GroupBy(x => new { AOIName = x.AOIName }).Select(x => new { AOIName = x.Key.AOIName, Pads = x.Sum(y => y.Pads) }).ToList();

                //Join with main table : PCBComponents
                _listAOIQty = (from a in table.AsEnumerable()
                               join b in tmpList2 on convertAOIName(a["Machine_Name"].ToString()) equals b.AOIName into c
                               from x in c.DefaultIfEmpty()
                               select new AOIQty(convertAOIName(a["Machine_Name"].ToString()), Convert.ToInt32(a["Pcb qty"]), Convert.ToInt32(a["ALL component"]), x.Pads)).ToList();


                //Toto je potrrebne zgrupit, pretoze sa zmenilo na chvilu MacchineName na niektoruych AOI a potom bol0 viac zaznamov s rovnakym menom a roznymi hodnotami
                _listAOIQty = _listAOIQty.GroupBy(x => x.AOIName).Select(x => new AOIQty(x.First().AOIName, x.Sum(c => c.PCBQty), x.Sum(c => c.AllComponents), x.Sum(c => c.AllPads))).ToList();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Exception on PrepareAllAOIQty: " + ex.Message);
            }
        }

        private int repairNumberOfPads(string AOIName, int pads)
        {
            if (pads > 1) return pads;

            if (AOIName == "AOI-SMT01") return 500;
            if (AOIName == "AOI-SMT02") return 500;
            if (AOIName == "AOI-SMT03") return 2550;
            if (AOIName == "AOI-SMT04") return 960;
            if (AOIName == "AOI-SMT05") return 2000;
            if (AOIName == "AOI-SMT06") return 2140;
            if (AOIName == "AOI-SMT08") return 2110;
            if (AOIName == "AOI-SMT09") return 1850;
            if (AOIName == "AOI-SMT10") return 980;
            if (AOIName == "AOI-SMT11") return 1870;
            if (AOIName == "AOI-SMT12") return 1340;
            if (AOIName == "AOI-SMT13") return 2400;
            if (AOIName == "AOI-SMT14") return 2083;
            if (AOIName == "AOI-SMT15") return 1840;
            if (AOIName == "AOI-SMT17") return 2000;

            return 1;

        }

        private int handleProducts(DataTable tableProducts, DataTable tablePads)
        {
            List<string> productsFound = (from a in tableProducts.AsEnumerable() select a["Product_Name_Product"].ToString()).ToList();
            List<string> productsExists = (from a in tablePads.AsEnumerable() select a["Product"].ToString()).ToList();


            List<string> notFound = productsFound.Except(productsExists).ToList();
            if (notFound.Count == 0) return 0;

            foreach(string item in notFound)
            {
                string sql = "insert into VIT_PCB_Pads(Product, Pads) values ('{0}', {1})";
                sql = string.Format(sql, item, 1);
                HelperClass.callCmd(_DBConnectionProduction, sql);
            }
            return notFound.Count;
        }

        private void prepareValorVITLines()
        {
            string sql = @"select Valor_Line, VIT_Line from Line with (nolock) where Valor_Line is not null";
            DataTable table = HelperClass.callQuery(_DBConnectionProduction, sql);

            _listValorVITLines = (from rw in table.AsEnumerable()
                                   select new ValorVITLine( rw["Valor_Line"].ToString(),
                                                            convertAOIName(rw["VIT_Line"].ToString())
                                                          )).ToList();
        }

        private DateTime Local2UTC(DateTime dt)
        {
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneInfo.Local.Id);
            return TimeZoneInfo.ConvertTimeToUtc(dt, tzi);
        }

        private void getTopologyAndRefByAOIName(List<Component> components,  ref List<string> references, ref List<string> topologs)
        {
            references = components.Select(x => x.Reference).Distinct().ToList();
            topologs = components.Select(x => x.Topology).Distinct().ToList();
        }
        /// <summary>
        /// /////////////////////////////////////////////
        /// </summary>
        private void prepareAllComponentsFromDB()
        {
            DateTime dtFrom = Local2UTC(DateTime.SpecifyKind(dateTimePickerFrom.Value, DateTimeKind.Unspecified));
            DateTime dtTo = Local2UTC(DateTime.SpecifyKind(dateTimePickerTo.Value, DateTimeKind.Unspecified));

            
            string sql = @"select
                    tPanelCard.Machine_Name,
                    tPanelCard.Card_ID, tPanelCard.Card_Bar_Code, Panel_ID, tTestedObj.Tested_Object_Id, tProduct.Product_Name,
                    StartTime, Topology, Error_table_AR, IsNull(old_Error_table_AR, 0) as old_Error_table_AR, Repair_State_Result,  
		            case 
			            when Len(Card_Bar_Code) >= 12 then Left(Card_Bar_Code, 4)
			            when Len(Card_Bar_Code) >= 9 then Left(Card_Bar_Code, 3)
			            else Left(Card_Bar_Code, 2)
		            end as Reference		
		            from PanelViewWithProgramUTC tPanelCard
		            left join Product tProduct WITH(NOLOCK) on tProduct.Product_Id = tPanelCard.Product_Id
		            left join Tested_Object tTestedObj with(NOLOCK) on tTestedObj.Card_id = tPanelCard.Card_id
	                left join
	                (
		                SELECT th.Tested_Object_Id, th.old_Error_Table_AR  FROM TESTED_OBJECT_HISTO th with(NOLOCK) 
		                inner join 
		                (
			                SELECT th1.Tested_Object_Id, max(DateTime) AS [LastDate] FROM TESTED_OBJECT_HISTO th1 with(NOLOCK) 
			                WHERE old_Error_Table_AR != 0  
			                GROUP BY Tested_Object_Id
		                ) th1 ON th.Tested_Object_Id = th1.Tested_Object_Id and th.DateTime = th1.LastDate 
	                ) th ON th.Tested_Object_Id = tTestedObj.Tested_Object_Id
		            where                    
                    --tPanelCard.Machine_Name = 'AOI-SMT05' AND
		            (tPanelCard.StartTime between '{0}' AND '{1}') AND tPanelCard.Card_Status <> 0 AND (tPanelCard.panel_status <> 0 or (tPanelCard.anomaly_BR / 256) % 2 = 1)
                    AND tTestedObj.Topology is not null
                    {2}AND left(tTestedObj.Topology,1) != 'P'";

            sql = string.Format(sql, dtFrom.ToString("yyyy-MM-dd HH:mm"), dtTo.ToString("yyyy-MM-dd HH:mm"), "--");

            DataTable table = HelperClass.callQuery(_DBConnectionAOI, sql);

            List<Component> tmp = (from rw in table.AsEnumerable()
                                             select new Component(convertAOIName(rw["Machine_Name"].ToString()),
                                                                    "", 
                                                                    Convert.ToInt64(rw["Card_ID"]),
                                                                    rw["Card_Bar_Code"].ToString(),
                                                                    Convert.ToInt64(rw["Panel_ID"]),
                                                                    Convert.ToInt64(rw["Tested_Object_Id"]),
                                                                    Convert.ToDateTime(rw["StartTime"]),
                                                                    rw["Topology"].ToString(),
                                                                    Convert.ToInt32(rw["Error_table_ar"]),
                                                                    Convert.ToInt32(rw["old_Error_table_AR"]),
                                                                    Convert.ToInt32(rw["Repair_State_Result"]),
                                                                    rw["Reference"].ToString(),
                                                                    rw["Product_Name"].ToString(),
                                                                    0
                                                                    )).ToList();


            List<Component> tmpListComponentsPlacement = tmp.Where(x => ((x.ErrorTableAR / 1) % 2 == 1) || 
                                                                        ((x.ErrorTableAR / 2) % 2 == 1) || 
                                                                        ((x.ErrorTableAR / 16) % 2 == 1) ||
                                                                        ((x.ErrorTableAR / 64) % 2 == 1) ||
                                                                        ((x.ErrorTableAR / 128) % 2 == 1) ||
                                                                        ((x.ErrorTableAR / 256) % 2 == 1) ||
                                                                        ((x.ErrorTableAR / 512) % 2 == 1) ||
                                                                        ((x.ErrorTableAR / 2048) % 2 == 1) ||
                                                                        ((x.ErrorTableAR / 524288) % 2 == 1) ||
                                                                        ((x.ErrorTableAR / 1048576) % 2 == 1) ||
                                                                        ((x.ErrorTableAR / 2097152) % 2 == 1) ||
                                                                        ((x.ErrorTableAR / 4194304) % 2 == 1) ||
                                                                        ((x.ErrorTableAR / 8388608) % 2 == 1) ||
                                                                        ((x.ErrorTableAR / 16777216) % 2 == 1)
                                                                        ).ToList();

            List<Component> tmpListComponentsSoldering = tmp.Where(x => ((x.ErrorTableAR / 4) % 2 == 1) ||
                                                                        ((x.ErrorTableAR / 8) % 2 == 1) ||
                                                                        ((x.ErrorTableAR / 1024) % 2 == 1)
                                                                        ).ToList();




            //Add ValorName           
            _listComponentsPlacement = (from g in tmpListComponentsPlacement
                                        join a in _listValorVITLines on g.AOIName equals a.VITLine
                                        select new Component(g.AOIName, a.ValorLine, g.Card_ID, g.Card_Bar_Code, g.Panel_ID, g.Tested_ID, g.StartTime, g.Topology, g.ErrorTableAR, g.ErrorTableAR, g.RepairStateResult, g.Reference, g.ProductName, 1)).ToList();



            _listComponentsSoldering =  (from g in tmpListComponentsSoldering
                                        join a in _listValorVITLines on g.AOIName equals a.VITLine
                                        select new Component(g.AOIName, a.ValorLine, g.Card_ID, g.Card_Bar_Code, g.Panel_ID, g.Tested_ID, g.StartTime, g.Topology, g.ErrorTableAR, g.ErrorTableAR, g.RepairStateResult, g.Reference, g.ProductName, 2)).ToList();
        }

        private List<int> getResultConditionPlacement()
        {
            List<int> ret = new List<int>();
            if (checkBox4.Checked) ret.Add(0);
            if (checkBox5.Checked) ret.Add(1);
            if (checkBox6.Checked) ret.Add(3);
            return ret;
        }

        private List<int> getResultConditionVisibility()
        {
            List<int> ret = new List<int>();
            if (checkBox3.Checked) ret.Add(0);
            if (checkBox9.Checked) ret.Add(1);
            return ret;
        }
        /// <summary>
        /// ////////////////
        /// </summary>
        /// <returns></returns>
        private List<AllAOIInfoPlacementSoldering> getMainSummaryGroupByAOIName()
        {
            int errorFrom = Convert.ToInt32(numericUpDown1.Value);
            int errorTo = Convert.ToInt32(numericUpDown2.Value);
            if (!checkBox2.Checked) errorTo = int.MaxValue;

            if (_listComponentsPlacement == null) return new List<AllAOIInfoPlacementSoldering>();

            //PLACEMENT
            //Filter by Result
            var filteredP = _listComponentsPlacement.Where(x => getResultConditionPlacement().Contains(x.RepairStateResult));

            //Filter by ProductName
            var filteredP2 = (!checkBox1.Checked) ? filteredP.Where(x => !x.ProductName.Contains("SPARE") && !x.ProductName.Contains("OBSOLETE") && !x.ProductName.Contains("PROTOTYPE")) : filteredP;

            //Group by AOIName and Card_ID and set grouped qty to selected value if there are more errors like selected value
            var groupedP = filteredP2.GroupBy(x => new
            {
                AOIName = x.AOIName,
                Card_ID = x.Card_ID
            }).Select(x => new
            {
                AOIName = x.Key.AOIName,
                Card_ID = x.Key.Card_ID,
                Qty = (x.Count() > errorFrom) && (x.Count() <= errorTo) ? errorFrom : x.Count()
            }).ToList();

            //Remove cards with too much errors
            groupedP.RemoveAll(x => x.Qty > errorTo);

            //Group by AOIName
            var groupedPlacement = groupedP.GroupBy(x => new { AOIName = x.AOIName }).Select(x => new { AOIName = x.Key.AOIName, Qty = x.Sum(y => y.Qty) }).ToList();



            //SOLDERING

            //Filter by Result and move soldering errors to placement if needed
            List<Component> filteredS = null;
            if (checkBox7.Checked)
                filteredS = _listComponentsSoldering.Where(x => getResultConditionPlacement().Contains(x.RepairStateResult)).Except(filteredP, new MyComponentComparer()).ToList();
            else
                filteredS = _listComponentsSoldering.Where(x => getResultConditionPlacement().Contains(x.RepairStateResult)).ToList();


            //Filter by ProductName
            var filteredS2 = (!checkBox1.Checked) ? filteredS.Where(x => !x.ProductName.Contains("SPARE") && !x.ProductName.Contains("OBSOLETE") && !x.ProductName.Contains("PROTOTYPE")) : filteredS;

            //Group by AOIName and Card_ID and set grouped qty to selected value if there are more errors like selected value
            var groupedS = filteredS2.GroupBy(x => new
            {
                AOIName = x.AOIName,
                Card_ID = x.Card_ID
            }).Select(x => new
            {
                AOIName = x.Key.AOIName,
                Card_ID = x.Key.Card_ID,
                Qty = (x.Count() > errorFrom) && (x.Count() <= errorTo) ? errorFrom : x.Count()
            }).ToList();

            //Remove cards with too much errors
            groupedS.RemoveAll(x => x.Qty > errorTo);

            //Group by AOIName
            var groupedSoldering = groupedS.GroupBy(x => new { AOIName = x.AOIName }).Select(x => new { AOIName = x.Key.AOIName, Qty = x.Sum(y => y.Qty) });


            try
            {
                //Join with AllLineQty
                var finalPlacement =
                                    (from a in _listAOIQty
                                     join g in groupedPlacement on a.AOIName equals g.AOIName into ot
                                     from otnew in ot.DefaultIfEmpty()
                                     select new AllAOIInfoPlacement(otnew == null ? a.AOIName : otnew.AOIName,
                                                                     otnew == null ? 0 : otnew.Qty,
                                                                     prepareDataForAOILine(0, otnew == null ? "" : otnew.AOIName).Where(x => x.MC.Contains("Not found")).ToList().Count,
                                                                     a.PCBQty,
                                                                     a.AllComponents,
                                                                     a.AllPads,
                                                                     0)).ToList();
                                                                    //10.2.2023 : zmena: PPM sa rata z QTY ocistenej od ValorNotFound, takze tu sa setne 0 a vyratava sa neskor (nasledujucom joine)
                                                                    //Math.Round(((double)(otnew == null ? 0 : otnew.Qty) / (double)a.AllComponents) * 1000000, 2))).ToList();


                //Join Soldering
                List<AllAOIInfoPlacementSoldering> final = (from a in finalPlacement
                                                            join g in groupedSoldering on a.AOIName equals g.AOIName
                                                            into ot
                                                            from otnew in ot.DefaultIfEmpty()
                                                            select new AllAOIInfoPlacementSoldering(
                                                                a.AOIName,
                                                                a.QtyAllPlacement,
                                                                a.QtyValorNotFoundPlacement,
                                                                a.PCBQty,
                                                                a.AllComponents,
                                                                a.AllPads,
                                                                Math.Round((double)(a.QtyPlacement/(double)a.AllComponents)*1000000, 2), //a.PPMPlacement,
                                                                otnew == null ? 0 : otnew.Qty,
                                                                prepareDataForAOILine(1, otnew == null ? "" : otnew.AOIName).Where(x => x.MC.Contains("Not found")).ToList().Count,
                                                                Math.Round(((double)(otnew == null ? 0 : otnew.Qty) / (double)a.AllPads) * 1000000, 2))).ToList();
                return final;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Exception on getMainSummaryGroupByAOIName: " + ex.Message);
            }
            return new List<AllAOIInfoPlacementSoldering>();
        }                  

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            refreshMainData(true);
            enableControls(true);
            simpleButton1.Enabled = false;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            enableControls(false);

            DateTime dt = DateTime.Now.AddDays(-1);
            dateTimePickerFrom.Value = new DateTime(dt.Year, dt.Month, dt.Day, 6, 0, 0);
            dt = DateTime.Now;//.AddDays(-1);
            dateTimePickerTo.Value = new DateTime(dt.Year, dt.Month, dt.Day, 6, 0, 0);
            gridBand2.AppearanceHeader.BackColor = _placementColor;
            gridBand3.AppearanceHeader.BackColor = _solderingColor;

            gridView1.BestFitColumns();

        }

        private AllAOIInfoPlacementSoldering getSelectedRow()
        {
            int[] rows = gridView1.GetSelectedRows();

            if (rows.Count() == 0) return null;

            return gridView1.GetRow(rows[0]) as AllAOIInfoPlacementSoldering;
        }

        private string getTextFromList(List<string> values)
        {
            string returnString = "";
            for(int i = 0; i < values.Count(); i++)
            {
                if (i > 0) returnString += ",";
                returnString += "'" + values[i] + "'";
            }
            return returnString;
        }
        /// <summary>
        /// ////////////
        /// </summary>
        private void refreshAOIDetails()
        {
            AllAOIInfoPlacement row = getSelectedRow();
            if (row == null) return;

            _selectedAOIName = row.AOIName;

            prepareAOIDetails(_selectedAOIName);

            drawAOIValorChartPlacement("MC");
            drawAOIValorChartSoldering("MC");
            drawComponentsErrorsPlacement();
            drawComponentsErrorsSoldering();
        }

        private void prepareAOIDetails(string AOIName)
        {
            _listComponentsForSelectedAOIPlacement = prepareDataForAOILine(0, AOIName);
            _listComponentsForSelectedAOISoldering = prepareDataForAOILine(1, AOIName);
        }

        private List<ComponentWithValor> getValorInfoForComponentsByAOI(int type, string AOILineName)
        {
            List<string> references = null;
            List<string> topologs = null;

            //Work only with components from selected AOI
            List<Component> filteredComponentsByAOIName = null;

            if (type == 0)
                filteredComponentsByAOIName = _listComponentsPlacement.Where(x => x.AOIName == AOILineName).ToList();
            else
            {
                if (checkBox7.Checked)
                {
                    List<Component> placementTmp = _listComponentsPlacement.Where(x => x.AOIName == AOILineName).ToList();
                    filteredComponentsByAOIName = _listComponentsSoldering.Where(x => x.AOIName == AOILineName).ToList();
                    filteredComponentsByAOIName = filteredComponentsByAOIName.Except(placementTmp, new MyComponentComparer()).ToList();
                }
                else
                    filteredComponentsByAOIName = _listComponentsSoldering.Where(x => x.AOIName == AOILineName).ToList();
            }



            if (filteredComponentsByAOIName.Count == 0) return new List<ComponentWithValor>();

            //Get Topology and References from filteredList
            getTopologyAndRefByAOIName(filteredComponentsByAOIName, ref references, ref topologs);

            //Get ValorName
            string ValorLineName = filteredComponentsByAOIName.FirstOrDefault().VALORName;


            //Get Valor informations : slot, station, ...
            string topologsString = getTextFromList(topologs);
            string referencesString = getTextFromList(references);
            string key = topologsString + referencesString;
            List<ComponentWithValor> returnValue = null;
            if (!_valorCache.TryGetValue(key, out returnValue))
            {
                //Prepare sql to get components description from Valor
                string sql = @"select NIP, Linka, Reference, nickname as  MC, ('Station_' + cast(Station as varchar)) as Station,('Slot_' +  cast(Slot as varchar)) as Slot, IsNull(Subslot, -1) Subslot, IsNull(TypFeedra, -1) as TypFeedra 
                                from Automaticky_feederlist WITH (NOLOCK)
                                INNER JOIN Linedescription1 with(nolock) ON Linedescription1.mcid = Automaticky_feederlist.McID    
                                where Reference in (" + topologsString + ") and NIP in (" + referencesString + ") and Linka = '" + ValorLineName + "'";

                DataTable table = HelperClass.callQuery(_DBConnectionValor, sql);
                List<MCStationSlotFeeder> tmpValor = (from rw in table.AsEnumerable()
                                                      select new MCStationSlotFeeder(rw["NIP"].ToString().Trim(),
                                                                                       rw["Linka"].ToString().Trim(),
                                                                                       rw["Reference"].ToString().Trim(),
                                                                                       rw["MC"].ToString(),
                                                                                       rw["Station"].ToString(),
                                                                                       rw["Slot"].ToString(),
                                                                                       Convert.ToInt32(rw["Subslot"]),
                                                                                       Convert.ToInt32(rw["TypFeedra"])
                                                            )).ToList();

                //Join AllComponents with Valor info
                returnValue = (from g in filteredComponentsByAOIName
                                               join a in tmpValor
                                                  on new { g.VALORName, g.Reference, g.Topology } equals new { a.VALORName, a.Reference, a.Topology } into tmp
                                               from x in tmp.DefaultIfEmpty()
                                               select new ComponentWithValor(g.AOIName, g.VALORName, g.Card_ID, g.Card_Bar_Code, g.Panel_ID, g.Tested_ID, g.StartTime, g.Topology, g.ErrorTableAR,
                                                   g.ErrorTableAR, g.RepairStateResult, g.Reference, g.ProductName,
                                                   x == null ? "Not found in Valor" : x.MC,
                                                   x == null ? "Not found in Valor" : x.Station,
                                                   x == null ? "Not found in Valor" : x.Slot,
                                                   x == null ? -1 : x.Subslot,
                                                   x == null ? -1 : x.TypeFeeder, (type == 0) ? 1 : 2)
                                        ).ToList();
                _valorCache[key] = returnValue;
            }
            return returnValue;
        }
        // !!
        // here i can just write an if that will determine whether it's filtered product or not and if yes - only use data that connects to this product

        private List<ComponentWithValor> prepareDataForAOILine(int type, string AOILineName)
        {
            int errorFrom = Convert.ToInt32(numericUpDown1.Value);
            int errorTo = Convert.ToInt32(numericUpDown2.Value);
            if (!checkBox2.Checked) errorTo = int.MaxValue;

            List<ComponentWithValor> joined = getValorInfoForComponentsByAOI(type, AOILineName);

            List<ComponentWithValor> tmp = new List<ComponentWithValor>();

            var filtered1 = joined.Where(x => getResultConditionPlacement().Contains(x.RepairStateResult));

            var filtered2 = (!checkBox1.Checked) ? filtered1.Where(x => !x.ProductName.Contains("SPARE") && !x.ProductName.Contains("OBSOLETE") && !x.ProductName.Contains("PROTOTYPE")) : filtered1;

            //If there is card with too much errors -> take selected qty (ussually 1)
            List<ComponentWithValor> dataOrdered = filtered2.OrderBy(x => x.Card_ID).ToList();


            //if (type == 0) gridControl4.DataSource = dataOrdered;
            //else gridControl6.DataSource = dataOrdered;

            if (dataOrdered.Count == 0) return new List<ComponentWithValor>();

            ComponentWithValor oldComponent = dataOrdered.First();            
            int iCount = 0;
            for (int i = 0; i < dataOrdered.Count(); i++)
            {
                ComponentWithValor component = dataOrdered[i];
                //Always add first item
                if (i == 0)
                {
                    iCount++;
                    tmp.Add(component);
                }
                else
                {
                    if (component.Card_ID == oldComponent.Card_ID)
                    {
                        if (iCount < errorFrom) tmp.Add(component);
                        iCount++;                        
                    }
                    else
                    {
                        //Remove last item if there are more like top level errors count
                        if (iCount > errorTo)
                        {
                            tmp.RemoveAll(x => x.Card_ID == oldComponent.Card_ID);
                        }
                        iCount = 0;
                        tmp.Add(component);
                        iCount++;
                    }
                }
                oldComponent = component;
            }

            return tmp;
        }

        private void drawAOIValorChartPlacement(string param)
        {
            if (_listComponentsForSelectedAOIPlacement == null) return;

            try
            {
                chartControl1.Series.Clear();
                chartControl1.Titles.Clear();
                chartControl1.Legends.Clear();
                chartControl1.DataSource = null;
                chartControl1.SeriesTemplate.ArgumentDrillTemplate = null;


                chartControl1.DataSource = _listComponentsForSelectedAOIPlacement;

                chartControl1.SeriesTemplate.SeriesDataMember = "Serie";
                chartControl1.SeriesTemplate.ArgumentDataMember = "MC";
                chartControl1.SeriesTemplate.QualitativeSummaryOptions.SummaryFunction = "COUNT()";
                chartControl1.SeriesTemplate.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
                chartControl1.SeriesTemplate.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.False;
                chartControl1.SeriesTemplate.View = new StackedBarSeriesView();
                chartControl1.SeriesTemplate.View.Color = _placementColor;
                chartControl1.SeriesTemplate.Label.TextColor = Color.Black;
                chartControl1.SeriesTemplate.SeriesPointsSortingKey = SeriesPointKey.Value_1;
                chartControl1.SeriesTemplate.SeriesPointsSorting = SortingMode.Descending;
                
                chartControl1.SeriesTemplate.TopNOptions.Enabled = true;
                chartControl1.SeriesTemplate.TopNOptions.ShowOthers =false;
                chartControl1.SeriesTemplate.TopNOptions.Mode = TopNMode.Count;
                chartControl1.SeriesTemplate.TopNOptions.Count = Convert.ToInt32(numericUpDown3.Value);
                
                
                SeriesTemplate argumentDrillTemplate = new SeriesTemplate();
                argumentDrillTemplate.SeriesDataMember = "Serie";
                argumentDrillTemplate.ArgumentDataMember = "Station";
                argumentDrillTemplate.View = new StackedBarSeriesView();
                argumentDrillTemplate.View.Color = _placementColor;
                argumentDrillTemplate.Label.TextColor = Color.Black;
                argumentDrillTemplate.QualitativeSummaryOptions.SummaryFunction = "COUNT()";
                argumentDrillTemplate.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
                argumentDrillTemplate.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.False;
                argumentDrillTemplate.SeriesPointsSortingKey = SeriesPointKey.Value_1;
                argumentDrillTemplate.SeriesPointsSorting = SortingMode.Descending;
                
                argumentDrillTemplate.TopNOptions.Enabled = true;
                argumentDrillTemplate.TopNOptions.ShowOthers = false;
                argumentDrillTemplate.TopNOptions.Mode = TopNMode.Count;
                argumentDrillTemplate.TopNOptions.Count = Convert.ToInt32(numericUpDown3.Value);
                chartControl1.SeriesTemplate.ArgumentDrillTemplate = argumentDrillTemplate;
                
                
                SeriesTemplate argumentDrillTemplate2 = new SeriesTemplate();
                argumentDrillTemplate2.SeriesDataMember = "Serie";
                argumentDrillTemplate2.ArgumentDataMember = "Slot";
                argumentDrillTemplate2.View = new StackedBarSeriesView();
                argumentDrillTemplate2.View.Color = _placementColor;
                argumentDrillTemplate2.Label.TextColor = Color.Black;
                argumentDrillTemplate2.QualitativeSummaryOptions.SummaryFunction = "COUNT()";
                argumentDrillTemplate2.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
                argumentDrillTemplate2.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.False;
                argumentDrillTemplate2.SeriesPointsSortingKey = SeriesPointKey.Value_1;
                argumentDrillTemplate2.SeriesPointsSorting = SortingMode.Descending;
                
                argumentDrillTemplate2.TopNOptions.Enabled = true;
                argumentDrillTemplate2.TopNOptions.ShowOthers = false;
                argumentDrillTemplate2.TopNOptions.Mode = TopNMode.Count;
                argumentDrillTemplate2.TopNOptions.Count = Convert.ToInt32(numericUpDown3.Value);   
                           
                argumentDrillTemplate.ArgumentDrillTemplate = argumentDrillTemplate2;

                SeriesTemplate argumentDrillTemplate3 = new SeriesTemplate();
                argumentDrillTemplate3.SeriesDataMember = "Serie";
                argumentDrillTemplate3.ArgumentDataMember = "Topology";
                argumentDrillTemplate3.View = new StackedBarSeriesView();
                argumentDrillTemplate3.View.Color = _placementColor;
                argumentDrillTemplate3.Label.TextColor = Color.Black;
                argumentDrillTemplate3.QualitativeSummaryOptions.SummaryFunction = "COUNT()";
                argumentDrillTemplate3.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
                argumentDrillTemplate3.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.False;
                argumentDrillTemplate3.SeriesPointsSortingKey = SeriesPointKey.Value_1;
                argumentDrillTemplate3.SeriesPointsSorting = SortingMode.Descending;
                
                argumentDrillTemplate3.TopNOptions.Enabled = true;
                argumentDrillTemplate3.TopNOptions.ShowOthers = false;
                argumentDrillTemplate3.TopNOptions.Mode = TopNMode.Count;
                argumentDrillTemplate3.TopNOptions.Count = Convert.ToInt32(numericUpDown3.Value);                
                
                argumentDrillTemplate2.ArgumentDrillTemplate = argumentDrillTemplate3;

                chartControl1.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;

                ChartTitle title = new ChartTitle();
                title.Font = new Font("Arial", 8);
                title.Text = "VALOR Trend (PLACEMENT) for : " + _selectedAOIName;
                chartControl1.Titles.Add(title);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void drawAOIValorChartSoldering(string param)
        {
            
            if (_listComponentsForSelectedAOISoldering == null) return;

            chartControl4.Series.Clear();
            chartControl4.Titles.Clear();
            chartControl4.Legends.Clear();
            chartControl4.DataSource = null;
            chartControl4.SeriesTemplate.ArgumentDataMember = "";

            chartControl4.DataSource = _listComponentsForSelectedAOISoldering;


            chartControl4.SeriesTemplate.SeriesDataMember = "Serie";
            chartControl4.SeriesTemplate.ArgumentDataMember = "MC";
            chartControl4.SeriesTemplate.QualitativeSummaryOptions.SummaryFunction = "COUNT()";
            chartControl4.SeriesTemplate.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
            chartControl4.SeriesTemplate.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.False;
            chartControl4.SeriesTemplate.View = new StackedBarSeriesView();
            chartControl4.SeriesTemplate.View.Color = _solderingColor;
            chartControl4.SeriesTemplate.Label.TextColor = Color.Black;
            chartControl4.SeriesTemplate.TopNOptions.Enabled = true;
            chartControl4.SeriesTemplate.TopNOptions.ShowOthers = false;
            chartControl4.SeriesTemplate.TopNOptions.Mode = TopNMode.Count;
            chartControl4.SeriesTemplate.TopNOptions.Count = Convert.ToInt32(numericUpDown3.Value);


            SeriesTemplate argumentDrillTemplate = new SeriesTemplate();
            argumentDrillTemplate.SeriesDataMember = "Serie";
            argumentDrillTemplate.ArgumentDataMember = "Station";
            argumentDrillTemplate.View = new StackedBarSeriesView();
            argumentDrillTemplate.View.Color = _solderingColor;
            argumentDrillTemplate.Label.TextColor = Color.Black;
            argumentDrillTemplate.QualitativeSummaryOptions.SummaryFunction = "COUNT()";
            argumentDrillTemplate.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
            argumentDrillTemplate.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.False;
            argumentDrillTemplate.TopNOptions.Enabled = true;
            argumentDrillTemplate.TopNOptions.ShowOthers = false;
            argumentDrillTemplate.TopNOptions.Mode = TopNMode.Count;
            argumentDrillTemplate.TopNOptions.Count = Convert.ToInt32(numericUpDown3.Value);
            chartControl4.SeriesTemplate.ArgumentDrillTemplate = argumentDrillTemplate;

            SeriesTemplate argumentDrillTemplate2 = new SeriesTemplate();
            argumentDrillTemplate2.SeriesDataMember = "Serie";
            argumentDrillTemplate2.ArgumentDataMember = "Slot";
            argumentDrillTemplate2.View = new StackedBarSeriesView();
            argumentDrillTemplate2.View.Color = _solderingColor;
            argumentDrillTemplate2.Label.TextColor = Color.Black;
            argumentDrillTemplate2.QualitativeSummaryOptions.SummaryFunction = "COUNT()";
            argumentDrillTemplate2.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
            argumentDrillTemplate2.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.False;
            argumentDrillTemplate2.TopNOptions.Enabled = true;
            argumentDrillTemplate2.TopNOptions.ShowOthers = false;
            argumentDrillTemplate2.TopNOptions.Mode = TopNMode.Count;
            argumentDrillTemplate2.TopNOptions.Count = Convert.ToInt32(numericUpDown3.Value);
            //(chartControl4.SeriesTemplate.ArgumentDrillTemplate as SeriesTemplate).ArgumentDrillTemplate = argumentDrillTemplate2;
            argumentDrillTemplate.ArgumentDrillTemplate = argumentDrillTemplate2;

            SeriesTemplate argumentDrillTemplate3 = new SeriesTemplate();
            argumentDrillTemplate3.SeriesDataMember = "Serie";
            argumentDrillTemplate3.ArgumentDataMember = "Topology";
            argumentDrillTemplate3.View = new StackedBarSeriesView();
            argumentDrillTemplate3.View.Color = _solderingColor;
            argumentDrillTemplate3.Label.TextColor = Color.Black;
            argumentDrillTemplate3.QualitativeSummaryOptions.SummaryFunction = "COUNT()";
            argumentDrillTemplate3.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
            argumentDrillTemplate3.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.False;
            argumentDrillTemplate3.TopNOptions.Enabled = true;
            argumentDrillTemplate3.TopNOptions.ShowOthers = false;
            argumentDrillTemplate3.TopNOptions.Mode = TopNMode.Count;
            argumentDrillTemplate3.TopNOptions.Count = Convert.ToInt32(numericUpDown3.Value);
            //((chartControl4.SeriesTemplate.ArgumentDrillTemplate as SeriesTemplate).ArgumentDrillTemplate as SeriesTemplate).ArgumentDrillTemplate = argumentDrillTemplate3;
            argumentDrillTemplate2.ArgumentDrillTemplate = argumentDrillTemplate3;


            chartControl4.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;
            ChartTitle title = new ChartTitle();
            title.Font = new Font("Arial", 8);
            title.Text = "VALOR Trend (REFLOW) for : " + _selectedAOIName;
            chartControl4.Titles.Add(title);

        }

        private void chartControl1_MouseUp(object sender, MouseEventArgs e)
        {
            ChartHitInfo hitInfoP = chartControl1.CalcHitInfo(e.Location);
            if (hitInfoP.InSeriesPoint)
            {
                try
                {
                    string argument = hitInfoP.SeriesPoint.Argument;
                    var series = hitInfoP.HitObject as Series;                    
                    if (series == null) series = hitInfoP.Series as Series;
                    if (series != null)
                        drawValorHistoryByDrillArgumentPlacement(series.ArgumentDataMember, argument);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void drawValorHistoryByDrillArgumentPlacement(string argument, string value)
        {
            List<History> groupedByDate = null;
            if (argument == "MC")
            {
                _selectedMCPlacement = value;
                var tmp = _listComponentsForSelectedAOIPlacement.Where(x => x.MC == _selectedMCPlacement).ToList();
                groupedByDate = tmp.GroupBy(x => new
                {
                    StartTime = x.StartTime,
                }).Select(x => new History(
                
                   x.Key.StartTime,
                   x.Count()
                )).ToList();
            }
            if (argument == "Station")
            {
                _selectedStationPlacement = value;
                var tmp = _listComponentsForSelectedAOIPlacement.Where(x => x.MC == _selectedMCPlacement && x.Station == _selectedStationPlacement).ToList();
                groupedByDate = tmp.GroupBy(x => new
                {
                    StartTime = x.StartTime,
                }).Select(x => new History(

                   x.Key.StartTime,
                   x.Count()
                )).ToList();
            }
            if (argument == "Slot")
            {
                _selectedSlotPlacement = value;
                var tmp = _listComponentsForSelectedAOIPlacement.Where(x => x.MC == _selectedMCPlacement && x.Station == _selectedStationPlacement && x.Slot == _selectedSlotPlacement).ToList();
                groupedByDate = tmp.GroupBy(x => new
                {
                    StartTime = x.StartTime,
                }).Select(x => new History(

                   x.Key.StartTime,
                   x.Count()
                )).ToList();
            }
            if (argument == "Topology")
            {
                _selectedTopologyPlacement = value;
                var tmp = _listComponentsForSelectedAOIPlacement.Where(x => x.MC == _selectedMCPlacement && x.Station == _selectedStationPlacement && x.Slot == _selectedSlotPlacement && x.Topology == _selectedTopologyPlacement).ToList();
                groupedByDate = tmp.GroupBy(x => new
                {
                    StartTime = x.StartTime,
                }).Select(x => new History(

                   x.Key.StartTime,
                   x.Count()
                )).ToList();
            }


            //Draw chart
            try
            {
                chartControl2.Series.Clear();
                chartControl2.Titles.Clear();
                chartControl2.Legends.Clear();
                chartControl2.DataSource = null;

                Series UserSeries = new Series();
                LineSeriesView view = new LineSeriesView();
                view.MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;
                view.Color = _placementColor;
                //view.Label.TextColor = Color.Black;
                UserSeries.View = view;

                UserSeries.DataSource = groupedByDate;
                UserSeries.ToolTipEnabled = DevExpress.Utils.DefaultBoolean.True;
                UserSeries.ToolTipPointPattern = "{A:dd-MM-yyyy} : {V}";
                UserSeries.ArgumentDataMember = "StartTime";
                UserSeries.ValueDataMembers[0] = "Qty";
                UserSeries.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.False;
                UserSeries.Name = argument;
                chartControl2.CrosshairOptions.ShowArgumentLine = false;
                chartControl2.Series.Add(UserSeries);
                chartControl2.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;
               
                ChartTitle title = new ChartTitle();
                title.Font = new Font("Arial", 8);
                title.Text = "History of : " + _selectedMCPlacement + (_selectedStationPlacement != ""?(" > " + _selectedStationPlacement) :"") + (_selectedSlotPlacement != ""?(" > " + _selectedSlotPlacement) :"") + (_selectedTopologyPlacement != ""?(" > " + _selectedTopologyPlacement) :"");
                chartControl2.Titles.Add(title);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void drawValorHistoryByDrillArgumentSoldering(string argument, string value)
        {
            List<History> groupedByDate = null;
            if (argument == "MC")
            {
                _selectedMCSoldering = value;
                var tmp = _listComponentsForSelectedAOISoldering.Where(x => x.MC == _selectedMCSoldering).ToList();
                groupedByDate = tmp.GroupBy(x => new
                {
                    StartTime = x.StartTime,
                }).Select(x => new History(

                   x.Key.StartTime,
                   x.Count()
                )).ToList();
            }
            if (argument == "Station")
            {
                _selectedStationSoldering = value;
                var tmp = _listComponentsForSelectedAOISoldering.Where(x => x.MC == _selectedMCSoldering && x.Station == _selectedStationSoldering).ToList();
                groupedByDate = tmp.GroupBy(x => new
                {
                    StartTime = x.StartTime,
                }).Select(x => new History(

                   x.Key.StartTime,
                   x.Count()
                )).ToList();
            }
            if (argument == "Slot")
            {
                _selectedSlotSoldering = value;
                var tmp = _listComponentsForSelectedAOISoldering.Where(x => x.MC == _selectedMCSoldering && x.Station == _selectedStationSoldering && x.Slot == _selectedSlotSoldering).ToList();
                groupedByDate = tmp.GroupBy(x => new
                {
                    StartTime = x.StartTime,
                }).Select(x => new History(

                   x.Key.StartTime,
                   x.Count()
                )).ToList();
            }
            if (argument == "Topology")
            {
                _selectedTopologySoldering = value;
                var tmp = _listComponentsForSelectedAOISoldering.Where(x => x.MC == _selectedMCSoldering && x.Station == _selectedStationSoldering && x.Slot == _selectedSlotSoldering && x.Topology == _selectedTopologySoldering).ToList();
                groupedByDate = tmp.GroupBy(x => new
                {
                    StartTime = x.StartTime,
                }).Select(x => new History(

                   x.Key.StartTime,
                   x.Count()
                )).ToList();
            }


            //Draw chart
            chartControl5.Series.Clear();
            chartControl5.Titles.Clear();
            chartControl5.Legends.Clear();
            chartControl5.DataSource = null;

            Series UserSeries = new Series();
            LineSeriesView view = new LineSeriesView();
            view.MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;
            view.Color = _solderingColor;
            UserSeries.View = view;

            UserSeries.DataSource = groupedByDate;
            UserSeries.ToolTipEnabled = DevExpress.Utils.DefaultBoolean.True;
            UserSeries.ToolTipPointPattern = "{A:dd-MM-yyyy} : {V}";
            UserSeries.ArgumentDataMember = "StartTime";
            UserSeries.ValueDataMembers[0] = "Qty";
            UserSeries.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.False;
            UserSeries.Name = argument;
            chartControl5.CrosshairOptions.ShowArgumentLine = false;
            chartControl5.Series.Add(UserSeries);
            chartControl5.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;

            ChartTitle title = new ChartTitle();
            title.Font = new Font("Arial", 8);
            title.Text = "History of : " + _selectedMCSoldering + (_selectedStationSoldering != "" ? (" > " + _selectedStationSoldering) : "") + (_selectedSlotSoldering != "" ? (" > " + _selectedSlotSoldering) : "") + (_selectedTopologySoldering != "" ? (" > " + _selectedTopologySoldering) : "");
            chartControl5.Titles.Add(title);
        }

        private void chartControl1_DrillDownStateChanged(object sender, DrillDownStateChangedEventArgs e)
        {
            try
            {
                if (e.Series[0].View is StackedBarSeriesView)
                {
                    _selectedMCPlacement = e.States.Count() > 0 ? e.States[0].BreadcrumbItemText : "";
                    _selectedStationPlacement = e.States.Count() > 1 ? e.States[1].BreadcrumbItemText : "";
                    _selectedSlotPlacement = e.States.Count() > 2 ? e.States[2].BreadcrumbItemText : "";
                    _selectedTopologyPlacement = "";

                    Console.WriteLine("selected MC : " + _selectedMCPlacement);
                    Console.WriteLine("selected Station : " + _selectedStationPlacement);
                    Console.WriteLine("selected Slot : " + _selectedSlotPlacement);

                    ComponentWithValor tmp = null;

                    if (e.States.Count() == 0) tmp = _listComponentsForSelectedAOIPlacement.First();
                    if (e.States.Count() == 1) tmp = _listComponentsForSelectedAOIPlacement.Where(x => x.MC == _selectedMCPlacement).First();
                    if (e.States.Count() == 2) tmp = _listComponentsForSelectedAOIPlacement.Where(x => x.MC == _selectedMCPlacement && x.Station == _selectedStationPlacement).First();
                    if (e.States.Count() == 3) tmp = _listComponentsForSelectedAOIPlacement.Where(x => x.MC == _selectedMCPlacement && x.Station == _selectedStationPlacement && x.Slot == _selectedSlotPlacement).First();


                    if (tmp != null)
                    {
                        if (e.Series[0].ArgumentDataMember == "MC") drawValorHistoryByDrillArgumentPlacement("MC", tmp.MC);
                        if (e.Series[0].ArgumentDataMember == "Station") drawValorHistoryByDrillArgumentPlacement("Station", tmp.Station);
                        if (e.Series[0].ArgumentDataMember == "Slot") drawValorHistoryByDrillArgumentPlacement("Slot", tmp.Slot);
                        if (e.Series[0].ArgumentDataMember == "Topology") drawValorHistoryByDrillArgumentPlacement("Topology", tmp.Topology);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void chartControl4_DrillDownStateChanged(object sender, DrillDownStateChangedEventArgs e)
        {
            if (e.Series[0].View is StackedBarSeriesView)
            {
                _selectedMCSoldering = e.States.Count() > 0 ? e.States[0].BreadcrumbItemText : "";
                _selectedStationSoldering = e.States.Count() > 1 ? e.States[1].BreadcrumbItemText : "";
                _selectedSlotSoldering = e.States.Count() > 2 ? e.States[2].BreadcrumbItemText : "";
                _selectedTopologySoldering = "";

                ComponentWithValor tmp = null;

                if (e.States.Count() == 0) tmp = _listComponentsForSelectedAOISoldering.First();
                if (e.States.Count() == 1) tmp = _listComponentsForSelectedAOISoldering.Where(x => x.MC == _selectedMCSoldering).First();
                if (e.States.Count() == 2) tmp = _listComponentsForSelectedAOISoldering.Where(x => x.MC == _selectedMCSoldering && x.Station == _selectedStationSoldering).First();
                if (e.States.Count() == 3) tmp = _listComponentsForSelectedAOISoldering.Where(x => x.MC == _selectedMCSoldering && x.Station == _selectedStationSoldering && x.Slot == _selectedSlotSoldering).First();


                if (tmp != null)
                {
                    if (e.Series[0].ArgumentDataMember == "MC") drawValorHistoryByDrillArgumentSoldering("MC", tmp.MC);
                    if (e.Series[0].ArgumentDataMember == "Station") drawValorHistoryByDrillArgumentSoldering("Station", tmp.Station);
                    if (e.Series[0].ArgumentDataMember == "Slot") drawValorHistoryByDrillArgumentSoldering("Slot", tmp.Slot);
                    if (e.Series[0].ArgumentDataMember == "Topology") drawValorHistoryByDrillArgumentSoldering("Topology", tmp.Topology);
                }
            }
        }

        List<ComponentWithValor> prepareComponentsForDrawAll()
        {
            if (_allComponentsGroupType == GroupType.ALL) return _listComponentsForSelectedAOIPlacement.Where(x => !x.Station.Contains("Not found")).ToList();

            List<ComponentWithValor> copyList = _listComponentsForSelectedAOIPlacement.Where(x => !x.Station.Contains("Not found")).Select(s => new ComponentWithValor(s)).ToList();
            if (_allComponentsGroupType == GroupType.GROUP_MISSING_DELTA)
            {                
                foreach (ComponentWithValor component in copyList)
                {
                    if ((component.ErrorName == "Missing") || (component.ErrorName == "Delta X") || (component.ErrorName == "Delta Y") || (component.ErrorName == "Delta Theta"))
                    {
                        component.ErrorName = "Missing, Delta";
                    }
                }
                return copyList;
            }
            if (_allComponentsGroupType == GroupType.GROUP_MISSING_DELTA_TILT)
            {
                foreach (ComponentWithValor component in copyList)
                {
                    if ((component.ErrorName == "Missing") || (component.ErrorName == "Delta X") || (component.ErrorName == "Delta Y") || (component.ErrorName == "Delta Theta") || 
                        (component.ErrorName == "Tilt"))
                    {
                        component.ErrorName = "Missing, Delta, Tilt";
                    }
                }
                return copyList;
            }
            if (_allComponentsGroupType == GroupType.GROUP_MISSING_DELTA_TILT_THICKNESS)
            {
                foreach (ComponentWithValor component in copyList)
                {
                    if ((component.ErrorName == "Missing") || (component.ErrorName == "Delta X") || (component.ErrorName == "Delta Y") || (component.ErrorName == "Delta Theta") || 
                        (component.ErrorName == "Tilt") || (component.ErrorName == "Thickness"))
                    {
                        component.ErrorName = "Missing, Delta, Tilt, Thickness";
                    }
                }
                return copyList;
            }
            if (_allComponentsGroupType == GroupType.GROUP_ALL)
            {
                foreach (ComponentWithValor component in copyList)
                {
                    if ((component.ErrorName == "Missing") || (component.ErrorName == "Delta X") || (component.ErrorName == "Delta Y") || (component.ErrorName == "Delta Theta") ||
                        (component.ErrorName == "Tilt") || (component.ErrorName == "Thickness") || (component.ErrorName == "Polarity") || (component.ErrorName == "OCV") ||
                        (component.ErrorName == "element skipped") || (component.ErrorName == "Side Overhang") || (component.ErrorName == "Length Overhang") || (component.ErrorName == "Foreign Material") ||
                        (component.ErrorName == "Component Present") || (component.ErrorName == "Lifted lead(refer to PIN Table)")
                        )
                    {
                        component.ErrorName = "All together";
                    }
                }
                return copyList;
            }
            return copyList;            
        }
        
        private void drawComponentsErrorsPlacement()
        {
            if (_listComponentsForSelectedAOIPlacement == null) return;

            try
            {
                chartControl3.Series.Clear();
                chartControl3.Titles.Clear();
                chartControl3.Legends.Clear();
                chartControl3.DataSource = null;
                chartControl3.SeriesTemplate.ArgumentDataMember = "";

                chartControl3.DataSource = prepareComponentsForDrawAll();// _listComponentsForSelectedAOIPlacement;           


                //LEVEL0
                chartControl3.SeriesTemplate.SeriesDataMember = "Serie";
                chartControl3.SeriesTemplate.ArgumentDataMember = "ErrorName";
                chartControl3.SeriesTemplate.QualitativeSummaryOptions.SummaryFunction = "COUNT()";
                chartControl3.SeriesTemplate.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
                chartControl3.SeriesTemplate.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.False;
                chartControl3.SeriesTemplate.View = new StackedBarSeriesView();
                chartControl3.SeriesTemplate.View.Color = _placementColor;
                chartControl3.SeriesTemplate.Label.TextColor = Color.Black;

                chartControl3.SeriesTemplate.TopNOptions.Enabled = true;
                chartControl3.SeriesTemplate.TopNOptions.ShowOthers = false;
                chartControl3.SeriesTemplate.TopNOptions.Mode = TopNMode.Count;
                chartControl3.SeriesTemplate.TopNOptions.Count = Convert.ToInt32(numericUpDown3.Value);
                chartControl3.SeriesTemplate.SeriesPointsSorting = SortingMode.Descending;
                chartControl3.SeriesTemplate.SeriesPointsSortingKey = SeriesPointKey.Value_1;


                //LEVEL1
                SeriesTemplate argumentDrillTemplate = new SeriesTemplate();
                argumentDrillTemplate.SeriesDataMember = "Serie";
                argumentDrillTemplate.ArgumentDataMember = "Topology";
                argumentDrillTemplate.View = new StackedBarSeriesView();
                argumentDrillTemplate.View.Color = _placementColor;
                argumentDrillTemplate.Label.TextColor = Color.Black;
                argumentDrillTemplate.QualitativeSummaryOptions.SummaryFunction = "COUNT()";
                argumentDrillTemplate.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
                argumentDrillTemplate.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.False;

                argumentDrillTemplate.TopNOptions.Enabled = true;
                argumentDrillTemplate.TopNOptions.ShowOthers = false;
                argumentDrillTemplate.TopNOptions.Mode = TopNMode.Count;
                argumentDrillTemplate.TopNOptions.Count = Convert.ToInt32(numericUpDown3.Value);
                argumentDrillTemplate.SeriesPointsSorting = SortingMode.Descending;
                argumentDrillTemplate.SeriesPointsSortingKey = SeriesPointKey.Value_1;

                chartControl3.SeriesTemplate.ArgumentDrillTemplate = argumentDrillTemplate;
                chartControl3.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;

                ChartTitle title = new ChartTitle();
                title.Font = new Font("Arial", 8);
                title.Text = "Placement Trend for : " + _selectedAOIName;
                chartControl3.Titles.Add(title);

                ((DevExpress.XtraCharts.XYDiagram)chartControl3.Diagram).AxisX.QualitativeScaleOptions.AutoGrid = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void drawComponentsErrorsSoldering()
        {
            
            if (_listComponentsForSelectedAOISoldering == null) return;

            try
            {
                chartControl6.Series.Clear();
                chartControl6.Titles.Clear();
                chartControl6.Legends.Clear();
                chartControl6.DataSource = null;
                chartControl6.SeriesTemplate.ArgumentDataMember = "";

                chartControl6.DataSource = _listComponentsForSelectedAOISoldering;

                //LEVEL0
                chartControl6.SeriesTemplate.SeriesDataMember = "Serie";
                chartControl6.SeriesTemplate.ArgumentDataMember = "ErrorName";
                chartControl6.SeriesTemplate.QualitativeSummaryOptions.SummaryFunction = "COUNT()";
                chartControl6.SeriesTemplate.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
                chartControl6.SeriesTemplate.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.False;
                chartControl6.SeriesTemplate.View = new StackedBarSeriesView();
                chartControl6.SeriesTemplate.View.Color = _solderingColor;
                chartControl6.SeriesTemplate.Label.TextColor = Color.Black;
                //chartControl6.SeriesTemplate.View.Tag = "ErrorName";

                chartControl6.SeriesTemplate.TopNOptions.Enabled = true;
                chartControl6.SeriesTemplate.TopNOptions.ShowOthers = false;
                chartControl6.SeriesTemplate.TopNOptions.Mode = TopNMode.Count;
                chartControl6.SeriesTemplate.TopNOptions.Count = Convert.ToInt32(numericUpDown3.Value);
                chartControl6.SeriesTemplate.SeriesPointsSorting = SortingMode.Descending;
                chartControl6.SeriesTemplate.SeriesPointsSortingKey = SeriesPointKey.Value_1;


                //LEVEL1
                SeriesTemplate argumentDrillTemplate = new SeriesTemplate();
                argumentDrillTemplate.SeriesDataMember = "Serie";
                argumentDrillTemplate.ArgumentDataMember = "Topology";
                argumentDrillTemplate.View = new StackedBarSeriesView();
                argumentDrillTemplate.View.Color = _solderingColor;
                argumentDrillTemplate.Label.TextColor = Color.Black;
                argumentDrillTemplate.QualitativeSummaryOptions.SummaryFunction = "COUNT()";
                argumentDrillTemplate.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
                argumentDrillTemplate.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.False;
                //argumentDrillTemplate.Tag = "Topology";

                argumentDrillTemplate.TopNOptions.Enabled = true;
                argumentDrillTemplate.TopNOptions.ShowOthers = false;
                argumentDrillTemplate.TopNOptions.Mode = TopNMode.Count;
                argumentDrillTemplate.TopNOptions.Count = Convert.ToInt32(numericUpDown3.Value);
                argumentDrillTemplate.SeriesPointsSorting = SortingMode.Descending;
                argumentDrillTemplate.SeriesPointsSortingKey = SeriesPointKey.Value_1;

                chartControl6.SeriesTemplate.ArgumentDrillTemplate = argumentDrillTemplate;
                chartControl6.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;

                ChartTitle title = new ChartTitle();
                title.Font = new Font("Arial", 8);
                title.Text = "Reflow Trend for : " + _selectedAOIName;
                chartControl6.Titles.Add(title);

                ((DevExpress.XtraCharts.XYDiagram)chartControl6.Diagram).AxisX.QualitativeScaleOptions.AutoGrid = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            
            try
            {
                if (chartControl1.Series.Count > 0) chartControl1.Series[0].TopNOptions.Count = Convert.ToInt32(numericUpDown3.Value);
                if (chartControl3.Series.Count > 0) chartControl3.Series[0].TopNOptions.Count = Convert.ToInt32(numericUpDown3.Value);

                if (chartControl4.Series.Count > 0) chartControl4.Series[0].TopNOptions.Count = Convert.ToInt32(numericUpDown3.Value);
                if (chartControl6.Series.Count > 0) chartControl6.Series[0].TopNOptions.Count = Convert.ToInt32(numericUpDown3.Value);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void chartControl3_DrillDownStateChanged(object sender, DrillDownStateChangedEventArgs e)
        {
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            refreshMainData(false);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            refreshMainData(false);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            refreshMainData(false);
        }

        private void checkBox4_CheckedChanged_1(object sender, EventArgs e)
        {
            if (getResultConditionPlacement().Count == 0)
            {
                MessageBox.Show("One option must be selected!");
                checkBox6.Checked = true;
            }
            refreshMainData(false);
        }

        private void chartControl4_MouseUp(object sender, MouseEventArgs e)
        {
            ChartHitInfo hitInfoS = chartControl4.CalcHitInfo(e.Location);
            if (hitInfoS.InSeriesPoint)
            {
                string argument = hitInfoS.SeriesPoint.Argument;
                var series = hitInfoS.HitObject as Series;
                if (series == null) series = hitInfoS.Series as Series;
                if (series != null)
                    drawValorHistoryByDrillArgumentSoldering(series.ArgumentDataMember, argument);
            }
        }

        private void dateTimePickerFrom_ValueChanged(object sender, EventArgs e)
        {
            simpleButton1.Enabled = true;
            enableControls(false);
        }

        private void chartControl3_MouseUp(object sender, MouseEventArgs e)
        {
            ChartHitInfo hitInfoP = chartControl3.CalcHitInfo(e.Location);
            if (hitInfoP.InSeriesPoint)
            {
                string argument = hitInfoP.SeriesPoint.Argument;
                var series = hitInfoP.HitObject as Series;
                if (series == null) series = hitInfoP.Series as Series;
                if (series != null)
                {
                    if (series.ArgumentDataMember == "Topology")
                    {
                        TopologyDetailsForm form = new TopologyDetailsForm(_placementColor, _listComponentsForSelectedAOIPlacement, argument);
                        form.ShowDialog();
                    }
                }
            }
        }

        private void chartControl6_MouseUp(object sender, MouseEventArgs e)
        {
            ChartHitInfo hitInfoP = chartControl6.CalcHitInfo(e.Location);
            if (hitInfoP.InSeriesPoint)
            {
                string argument = hitInfoP.SeriesPoint.Argument;
                var series = hitInfoP.HitObject as Series;
                if (series == null) series = hitInfoP.Series as Series;
                if (series != null)
                {
                    if (series.ArgumentDataMember == "Topology")
                    {
                        TopologyDetailsForm form = new TopologyDetailsForm(_solderingColor, _listComponentsForSelectedAOISoldering, argument);
                        form.ShowDialog();
                    }
                }
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (_mainSumary == null) return;

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            saveDialog.FilterIndex = 0;

            if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    DevExpress.XtraSpreadsheet.SpreadsheetControl spreadsheetControl1 = new DevExpress.XtraSpreadsheet.SpreadsheetControl();
                    DevExpress.Spreadsheet.Worksheet worksheet = spreadsheetControl1.Document.Worksheets[0];
                    worksheet.Name = "Preview";

                    DataTable table = LinqHelper.toDataTable<AllAOIInfoPlacementSoldering>(_mainSumary.AsQueryable());
                    //Need to add to references: DevExpress.Docs
                    worksheet.Import(table, true, 0, 0);
                    worksheet.Columns.AutoFit(0, 100);
                    

                    foreach (AllAOIInfoPlacementSoldering item in _mainSumary)
                    {
                        List<ComponentWithValor> tmpListPlacement = prepareDataForAOILine(0, item.AOIName);
                        List<ComponentWithValor> tmpListReflow = prepareDataForAOILine(1, item.AOIName);

                        worksheet = spreadsheetControl1.Document.Worksheets.Add("Plac_" + item.AOIName);
                        table = LinqHelper.toDataTable<ComponentWithValor>(tmpListPlacement.AsQueryable());
                        worksheet.Import(table, true, 0, 0);
                        worksheet.Columns.AutoFit(0, 100);

                        worksheet = spreadsheetControl1.Document.Worksheets.Add("Refl_" + item.AOIName);
                        table = LinqHelper.toDataTable<ComponentWithValor>(tmpListReflow.AsQueryable());
                        worksheet.Import(table, true, 0, 0);
                        worksheet.Columns.AutoFit(0, 100);
                    }

                    spreadsheetControl1.Document.Worksheets.ActiveWorksheet = spreadsheetControl1.Document.Worksheets[0];

                    spreadsheetControl1.SaveDocument(saveDialog.FileName);
                    MessageBox.Show("Export finished successfully!");
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Export failed: " + ex.Message);
                }
            }
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
        }

        private void gridView1_Click_1(object sender, EventArgs e)
        {
            refreshAOIDetails();

            if (!((_listComponentsForSelectedAOIPlacement == null) || (_listComponentsForSelectedAOIPlacement.Count == 0)))
            {
                //Draw history of first value in chart
                ComponentWithValor tmp = _listComponentsForSelectedAOIPlacement.First();
                if (tmp != null) drawValorHistoryByDrillArgumentPlacement("MC", tmp.MC);
            }
            if (!((_listComponentsForSelectedAOISoldering == null) || (_listComponentsForSelectedAOISoldering.Count == 0)))
            {
                //Draw history of first value in chart
                ComponentWithValor tmp = _listComponentsForSelectedAOISoldering.First();
                if (tmp != null) drawValorHistoryByDrillArgumentSoldering("MC", tmp.MC);
            }
        }

        private void gridView1_KeyUp(object sender, KeyEventArgs e)
        {
            gridView1_Click_1(sender, null);
        }

        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender == allToolStripMenuItem) _allComponentsGroupType = GroupType.ALL;
            if (sender == groupMissingDeltaToolStripMenuItem) _allComponentsGroupType = GroupType.GROUP_MISSING_DELTA;
            if (sender == groupMissingDeltaTiltToolStripMenuItem) _allComponentsGroupType = GroupType.GROUP_MISSING_DELTA_TILT;
            if (sender == groupMissingDeltaTiltThicknessToolStripMenuItem) _allComponentsGroupType = GroupType.GROUP_MISSING_DELTA_TILT_THICKNESS;
            if (sender == groupALLToolStripMenuItem) _allComponentsGroupType = GroupType.GROUP_ALL;

            allToolStripMenuItem.Checked = false;
            groupMissingDeltaToolStripMenuItem.Checked = false;
            groupMissingDeltaTiltToolStripMenuItem.Checked = false;
            groupMissingDeltaTiltThicknessToolStripMenuItem.Checked = false;
            groupALLToolStripMenuItem.Checked = false;

            (sender as ToolStripMenuItem).Checked = true;

            drawComponentsErrorsPlacement();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            tableLayoutPanel8.ColumnStyles[0].SizeType = SizeType.Percent;
            tableLayoutPanel8.ColumnStyles[0].Width = 50;
            tableLayoutPanel8.ColumnStyles[1].SizeType = SizeType.Percent;
            tableLayoutPanel8.ColumnStyles[1].Width = 50;

            gridBand2.Visible = true;
            gridBand3.Visible = true;

            if (radioButton1.Checked)
            {
                //Hide reflow grids
                tableLayoutPanel8.ColumnStyles[1].SizeType = SizeType.Absolute;
                tableLayoutPanel8.ColumnStyles[1].Width = 0;
                //Hide reflow column in main grid
                gridBand3.Visible = false;
            }
            if (radioButton2.Checked)
            {
                //Hide placement grids
                tableLayoutPanel8.ColumnStyles[0].SizeType = SizeType.Absolute;
                tableLayoutPanel8.ColumnStyles[0].Width = 0;
                //Hide placement column in main grid
                gridBand2.Visible = false;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            
            if (getResultConditionVisibility().Count == 0)
            {
                MessageBox.Show("One option must be selected!");
                sender = checkBox3;
                checkBox3.Checked = true;
            }            

           

            tableLayoutPanel8.RowStyles[0].SizeType = SizeType.Percent;
            tableLayoutPanel8.RowStyles[0].Height = 33;
            tableLayoutPanel8.RowStyles[1].SizeType = SizeType.Percent;
            tableLayoutPanel8.RowStyles[1].Height = 33;
            tableLayoutPanel8.RowStyles[2].SizeType = SizeType.Percent;
            tableLayoutPanel8.RowStyles[2].Height = 33;

            if (!checkBox3.Checked)
            {
                tableLayoutPanel8.RowStyles[0].SizeType = SizeType.Absolute;
                tableLayoutPanel8.RowStyles[0].Height = 0;
                tableLayoutPanel8.RowStyles[1].SizeType = SizeType.Absolute;
                tableLayoutPanel8.RowStyles[1].Height = 0;
            }
            if (!checkBox9.Checked)
            {
                tableLayoutPanel8.RowStyles[2].SizeType = SizeType.Absolute;
                tableLayoutPanel8.RowStyles[2].Height = 0;
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            saveDialog.FilterIndex = 0;

            if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                chartControl3.ExportToXlsx(saveDialog.FileName);
                MessageBox.Show("Export finished successfully!");
            }                
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            RelationsForm form = new RelationsForm(_DBConnectionProduction);
            form.ShowDialog();
        }

        private string getNameOfRepairState(int value)
        {
            switch (value)
            {
                case 0: return "KO";
                case 1: return "REPAIRED";
                case 3: return "KO_OPERATOR";
                default: return "";
            }
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            if (_mainSumary == null) return;

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            saveDialog.FilterIndex = 0;

            if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    List<ComponentWithValor> listPlacement = new List<ComponentWithValor>();
                    List<ComponentWithValor> listReflow = new List<ComponentWithValor>();

                    foreach (AllAOIInfoPlacementSoldering item in _mainSumary)
                    {
                        List<ComponentWithValor> tmpListPlacement = prepareDataForAOILine(0, item.AOIName);
                        listPlacement.AddRange(tmpListPlacement);
                        List<ComponentWithValor> tmpListReflow = prepareDataForAOILine(1, item.AOIName);
                        listReflow.AddRange(tmpListReflow);
                    }
                    DevExpress.XtraSpreadsheet.SpreadsheetControl spreadsheetControl1 = new DevExpress.XtraSpreadsheet.SpreadsheetControl();
                    DevExpress.Spreadsheet.Worksheet worksheet = spreadsheetControl1.Document.Worksheets[0];
                    worksheet.Name = "Placement";
                    DataTable table = LinqHelper.toDataTable<ComponentWithValor>(listPlacement.AsQueryable());
                    worksheet.Import(reorganizeTableForQualityExport(0, table), true, 0, 0);
                    worksheet.Columns.AutoFit(0, 100);

                    worksheet = spreadsheetControl1.Document.Worksheets.Add("Reflow");
                    table = LinqHelper.toDataTable<ComponentWithValor>(listReflow.AsQueryable());
                    worksheet.Import(reorganizeTableForQualityExport(1, table), true, 0, 0);
                    worksheet.Columns.AutoFit(0, 100);


                    spreadsheetControl1.Document.Worksheets.ActiveWorksheet = spreadsheetControl1.Document.Worksheets[0];
                    spreadsheetControl1.SaveDocument(saveDialog.FileName);
                    MessageBox.Show("Export finished successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Export failed: " + ex.Message);
                }
            }
        }

        private DataTable reorganizeTableForQualityExport(int type, DataTable table)
        {
            DataTable newTable = new DataTable();

            newTable.Columns.Add("Machine", typeof(string));
            newTable.Columns.Add("Card_ID", typeof(int));
            newTable.Columns.Add("typ", typeof(string));
            newTable.Columns.Add("Typ_Qty", typeof(int));
            newTable.Columns.Add("Placement", typeof(int));
            newTable.Columns.Add("serie", typeof(string));
            newTable.Columns.Add("Topology", typeof(string));
            newTable.Columns.Add("Chyba", typeof(string));
            newTable.Columns.Add("qty", typeof(int));
            newTable.Columns.Add("Card_Bar_Code", typeof(string));
            newTable.Columns.Add("Serie2", typeof(string));
            newTable.Columns.Add("Datum", typeof(string));
            newTable.Columns.Add("NIP", typeof(string));
            newTable.Columns.Add("MC", typeof(string));
            newTable.Columns.Add("Station", typeof(string));
            newTable.Columns.Add("Slot", typeof(string));
            newTable.Columns.Add("Subslot", typeof(int));
            newTable.Columns.Add("typ Feedra", typeof(int));
            newTable.Columns.Add("valor Linka", typeof(string));

            foreach (DataRow row in table.Rows)
            {
                DataRow newRow = newTable.Rows.Add();
                newRow["Machine"] = row["AOIName"].ToString();
                newRow["Card_ID"] = row["Card_ID"] != DBNull.Value ? Convert.ToInt32(row["Card_ID"]) : 0;
                newRow["typ"] = type==0?"PLACEMENT":"REFLOW";
                newRow["Typ_Qty"] = 1;
                newRow["Placement"] = 1;
                newRow["serie"] = getNameOfRepairState(Convert.ToInt32(row["RepairStateResult"]));
                newRow["Topology"] = row["Topology"].ToString();
                newRow["Chyba"] = row["ErrorName"].ToString();
                newRow["qty"] = 1;
                newRow["Card_Bar_Code"] = row["Card_Bar_Code"].ToString();
                newRow["Serie2"] = getNameOfRepairState(Convert.ToInt32(row["RepairStateResult"]));
                newRow["Datum"] = Convert.ToDateTime(row["StartTime"]).ToString("yyyy-MM-dd");
                newRow["NIP"] = row["Reference"].ToString();
                newRow["MC"] = row["MC"].ToString();
                newRow["Station"] = row["Station"].ToString();
                newRow["Slot"] = row["Slot"].ToString();
                newRow["Subslot"] = Convert.ToInt32(row["Subslot"]);
                newRow["typ Feedra"] = Convert.ToInt32(row["TypeFeeder"]);
                newRow["valor Linka"] = row["VALORName"].ToString();
            }
            return newTable;
        }

        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                // Get the handle of the focused row
                int focusedRowHandle = gridView1.FocusedRowHandle;

                if (focusedRowHandle < 0 || gridView1.IsGroupRow(focusedRowHandle))
                {
                    // Invalid row handle or it's a group row, so just return
                    return;
                }

                // Fetch the "AOIName" value from the focused row
                string aoiName = gridView1.GetRowCellValue(focusedRowHandle, "AOIName").ToString();

                string dtFrom = Local2UTC(DateTime.SpecifyKind(dateTimePickerFrom.Value, DateTimeKind.Unspecified)).ToString("yyyy-MM-dd HH:mm");
                string dtTo = Local2UTC(DateTime.SpecifyKind(dateTimePickerTo.Value, DateTimeKind.Unspecified)).ToString("yyyy-MM-dd HH:mm");

                // Use the unconvertAOIName function to get the unconverted AOIName
                string unconvertedAOIName = unconvertAOIName(aoiName);

                // Open the ProductsDetail form and pass the unconverted AOIName as a parameter
                ProductsDetail detailForm = new ProductsDetail(aoiName, unconvertedAOIName, dtFrom, dtTo);
                detailForm.ShowDialog();

                string selectedProduct = detailForm.SelectedProductName;
                if (!string.IsNullOrEmpty(selectedProduct))
                {
                    // Update the focused row with data specific to the selected product
                    UpdateSelectedRowForProduct(focusedRowHandle, selectedProduct, aoiName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening ProductsDetail: " + ex.Message);
            }
        }

        private void UpdateSelectedRowForProduct(int selectedRowHandle, string selectedProduct, string AOIname)
        {

            int errorFrom = Convert.ToInt32(numericUpDown1.Value);
            int errorTo = Convert.ToInt32(numericUpDown2.Value);
            if (!checkBox2.Checked) errorTo = int.MaxValue;


            // PLACEMENT
            var filteredPlacement = _listComponentsPlacement.Where(x => x.ProductName == selectedProduct && x.AOIName == AOIname);

            var filteredP = filteredPlacement.Where(x => getResultConditionPlacement().Contains(x.RepairStateResult));

            var groupedPlacement = PlacementDataFilter(filteredPlacement, errorFrom, errorTo, filteredP);
            // SOLDERING
            var filteredSoldering = _listComponentsSoldering.Where(x => x.ProductName == selectedProduct && x.AOIName == AOIname);
            var groupedSoldering = SolderingDataFilter(filteredSoldering, errorFrom, errorTo, filteredP);

            // Prepare finalPlacement list
            var finalPlacement = PrepareFinalPlacement(_listAOIQty, groupedPlacement);

            // Prepare final list
            var final = PrepareFinalList(finalPlacement, groupedSoldering);

            UpdateGridView(final, AOIname, selectedRowHandle);
            ShowUpdateProductLabel(selectedProduct, AOIname);
        }

        private List<GroupedData> PlacementDataFilter(IEnumerable<Component> filteredPlacement, int errorFrom, int errorTo, IEnumerable<Component> filteredP)
        {
            //Filter by ProductName
            var filteredP2 = (!checkBox1.Checked) ? filteredP.Where(x => !x.ProductName.Contains("SPARE") && !x.ProductName.Contains("OBSOLETE") && !x.ProductName.Contains("PROTOTYPE")) : filteredP;

            //Group by AOIName and Card_ID and set grouped qty to selected value if there are more errors like selected value
            var groupedP = filteredP2.GroupBy(x => new GroupedData
            {
                AOIName = x.AOIName,
                Card_ID = x.Card_ID
            }).Select(x => new GroupedData
            {
                AOIName = x.Key.AOIName,
                Card_ID = x.Key.Card_ID,
                Qty = (x.Count() > errorFrom) && (x.Count() <= errorTo) ? errorFrom : x.Count()
            }).ToList();

            //Remove cards with too much errors
            groupedP.RemoveAll(x => x.Qty > errorTo);
            var groupedPlacement = groupedP.GroupBy(x => new GroupedData { AOIName = x.AOIName }).Select(x => new GroupedData { AOIName = x.Key.AOIName, Qty = x.Sum(y => y.Qty) }).ToList();
            return groupedPlacement;
        }

        private List<GroupedData> SolderingDataFilter(IEnumerable<Component> filteredData, int errorFrom, int errorTo, IEnumerable<Component> filteredP)
        {
            List<Component> filteredS = null;
            if (checkBox7.Checked)
                filteredS = _listComponentsSoldering.Where(x => getResultConditionPlacement().Contains(x.RepairStateResult)).Except(filteredP, new MyComponentComparer()).ToList();
            else
                filteredS = _listComponentsSoldering.Where(x => getResultConditionPlacement().Contains(x.RepairStateResult)).ToList();


            //Filter by ProductName
            var filteredS2 = (!checkBox1.Checked) ? filteredS.Where(x => !x.ProductName.Contains("SPARE") && !x.ProductName.Contains("OBSOLETE") && !x.ProductName.Contains("PROTOTYPE")) : filteredS;
            //Group by AOIName and Card_ID and set grouped qty to selected value if there are more errors like selected value
            var groupedS = filteredS2.GroupBy(x => new GroupedData
            {
                AOIName = x.AOIName,
                Card_ID = x.Card_ID
            }).Select(x => new GroupedData
            {
                AOIName = x.Key.AOIName,
                Card_ID = x.Key.Card_ID,
                Qty = (x.Count() > errorFrom) && (x.Count() <= errorTo) ? errorFrom : x.Count()
            }).ToList();

            //Remove cards with too much errors
            groupedS.RemoveAll(x => x.Qty > errorTo);

            //Group by AOIName
            var groupedSoldering = groupedS.GroupBy(x => new GroupedData { AOIName = x.AOIName }).Select(x => new GroupedData { AOIName = x.Key.AOIName, Qty = x.Sum(y => y.Qty) });

            return groupedSoldering.ToList();
        }

        private List<AllAOIInfoPlacement> PrepareFinalPlacement(IEnumerable<AOIQty> listAOIQty, List<GroupedData> groupedPlacement)
        {
            return (from a in listAOIQty
                    join g in groupedPlacement on a.AOIName equals g.AOIName into ot
                    from otnew in ot.DefaultIfEmpty()
                    select new AllAOIInfoPlacement(
                        otnew == null ? a.AOIName : otnew.AOIName,
                        otnew == null ? 0 : otnew.Qty,
                        prepareDataForAOILine(0, otnew == null ? "" : otnew.AOIName).Where(x => x.MC.Contains("Not found")).ToList().Count,
                        a.PCBQty,
                        a.AllComponents,
                        a.AllPads,
                        0.0)) // Assuming ppm is 0.0 for now
                .ToList();
        }

        private List<AllAOIInfoPlacementSoldering> PrepareFinalList(List<AllAOIInfoPlacement> finalPlacement, List<GroupedData> groupedSoldering)
        {
            return (from a in finalPlacement
                    join g in groupedSoldering on a.AOIName equals g.AOIName into ot
                    from otnew in ot.DefaultIfEmpty()
                    select new AllAOIInfoPlacementSoldering(
                        a.AOIName,
                        a.QtyAllPlacement,
                        a.QtyValorNotFoundPlacement,
                        a.PCBQty,
                        a.AllComponents,
                        a.AllPads,
                        Math.Round((double)(a.QtyPlacement / (double)a.AllComponents) * 1000000, 2), //a.PPMPlacement,
                        otnew == null ? 0 : otnew.Qty,
                        prepareDataForAOILine(1, otnew == null ? "" : otnew.AOIName).Where(x => x.MC.Contains("Not found")).ToList().Count,
                        Math.Round(((double)(otnew == null ? 0 : otnew.Qty) / (double)a.AllPads) * 1000000, 2))) // Assuming ppm is 0.0 for soldering as well
                .ToList();
        }

        private void UpdateGridView(List<AllAOIInfoPlacementSoldering> final, string AOIname, int selectedRowHandle)
        {
            PrintAllRowsWithIndexes();
            var resultForAOI = final.FirstOrDefault(item => item.AOIName == AOIname);
            if (resultForAOI == null) return;

            // Attempt to directly access the data source using AOIName
            if (gridControl1.DataSource is List<AllAOIInfoPlacementSoldering> dataSourceList)
            {
                var directRowData = dataSourceList.FirstOrDefault(row => row.AOIName == AOIname);

                if (directRowData != null)
                {
                    bool hasChanged = false; // Flag to check if any property value has been changed

                    foreach (GridColumn column in gridView1.Columns)
                    {
                        var propertyName = column.FieldName;
                        var propertyInfo = resultForAOI.GetType().GetProperty(propertyName);

                        if (propertyInfo != null)
                        {
                            var currentVal = propertyInfo.GetValue(directRowData);
                            var newVal = propertyInfo.GetValue(resultForAOI);

                            if (currentVal != null && !currentVal.Equals(newVal) || currentVal == null && newVal != null)
                            {
                                propertyInfo.SetValue(directRowData, newVal);
                                hasChanged = true;
                            }
                        }
                    }

                    if (hasChanged)
                    {
                        gridView1.RefreshRow(selectedRowHandle);
                    }
                }
            }
        }

        private void PrintAllRowsWithIndexes()
        {
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                if (!gridView1.IsGroupRow(i)) // Ensure it's not a group row
                {
                    object cellValue = gridView1.GetRowCellValue(i, "AOIName"); // Replace "AOIName" with the desired column name
                    Console.WriteLine($"Index: {i}, Value: {cellValue}");
                }
            }
        }

        private void ShowUpdateProductLabel(string product, string AOIname)
        {
            productLabel.Name = $"{AOIname}";
            productLabel.Text = $"Selected product: {product} for the {AOIname}";
            productLabel.Visible = true;
            returnToDefaultButton.Visible = true;
        }

        private void HideProductLabel() 
        {
            returnToDefaultButton.Visible = false;
            productLabel.Visible = false;
        }

    }
}
