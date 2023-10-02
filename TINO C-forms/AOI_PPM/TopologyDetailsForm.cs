using DevExpress.XtraCharts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AOI_PPM
{
    public partial class TopologyDetailsForm : Form
    {
        private List<ComponentWithValor> _data = null;
        private string _topology = "";
        private Color _color = Color.Black;

        public TopologyDetailsForm(Color color, List<ComponentWithValor> data, string topology)
        {
            _data = data;
            _topology = topology;
            _color = color;
                
            InitializeComponent();
        }

        private void prepareTopologyHistory()
        {
            List<History> groupedByDate = null;

            var tmp = _data.Where(x => x.Topology == _topology).ToList();
            groupedByDate = tmp.GroupBy(x => new
            {
                StartTime = x.StartTime,
            }).Select(x => new History(

                x.Key.StartTime,
                x.Count()
            )).ToList();
            

            chartControl1.Series.Clear();
            chartControl1.Titles.Clear();
            chartControl1.Legends.Clear();
            chartControl1.DataSource = null;

            Series UserSeries = new Series();
            LineSeriesView view = new LineSeriesView();
            view.MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;
            view.Color = _color;
            //view.Label.TextColor = Color.Black;
            UserSeries.View = view;

            UserSeries.DataSource = groupedByDate;
            UserSeries.ToolTipEnabled = DevExpress.Utils.DefaultBoolean.True;
            UserSeries.ToolTipPointPattern = "{A:dd-MM-yyyy} : {V}";
            UserSeries.ArgumentDataMember = "StartTime";
            UserSeries.ValueDataMembers[0] = "Qty";
            UserSeries.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.False;
            //UserSeries.Name = argument;
            chartControl1.CrosshairOptions.ShowArgumentLine = false;
            chartControl1.Series.Add(UserSeries);
            chartControl1.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;

            ChartTitle title = new ChartTitle();
            title.Font = new Font("Arial", 8);
            title.Text = "History of : " + _topology;
            chartControl1.Titles.Add(title);
        }

        private void TopologyDetailsForm_Shown(object sender, EventArgs e)
        {
            prepareTopologyHistory();
        }
    }
}
