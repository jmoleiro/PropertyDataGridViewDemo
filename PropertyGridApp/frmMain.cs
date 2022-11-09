using System.Data;
using System;
using System.Drawing.Drawing2D;
using System.Reflection;
using Newtonsoft.Json.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PropertyGridApp
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            typeof(DataGridView).InvokeMember(
               "DoubleBuffered",
               BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
               null,
               dgvProperties,
               new object[] { true });

        }

        private void dgvProperties_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                e.Graphics.FillRectangle(Brushes.White, e.CellBounds);
                Color c1 = Color.FromArgb(128, 185, 219, 253);
                Color c2 = Color.FromArgb(128, 93, 115, 142);
                Color c3 = Color.FromArgb(128, 185, 219, 253);

                LinearGradientBrush br = new LinearGradientBrush(e.CellBounds, c1, c3, 90, true);
                ColorBlend cb = new ColorBlend();
                cb.Positions = new[] { 0, (float)0.5, 1 };
                cb.Colors = new[] { c1, c2, c3 };
                br.InterpolationColors = cb;

                e.Graphics.FillRectangle(br, e.CellBounds);
                e.PaintContent(e.ClipBounds);
                e.Handled = true;
            }
            if ((e.ColumnIndex == 0) && (e.RowIndex >= 0))
            {
                int alpha = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex].Selected ? 128 : 64;
                e.Graphics.FillRectangle(Brushes.White, e.CellBounds);
                Color c1 = Color.FromArgb(alpha, 54, 54, 54);
                Color c2 = Color.FromArgb(alpha, 62, 62, 62);
                Color c3 = Color.FromArgb(alpha, 98, 98, 98);

                LinearGradientBrush br = new LinearGradientBrush(e.CellBounds, c1, c3, 90, true);
                ColorBlend cb = new ColorBlend();
                cb.Positions = new[] { 0, (float)0.5, 1 };
                cb.Colors = new[] { c1, c2, c3 };
                br.InterpolationColors = cb;


                e.Graphics.FillRectangle(br, e.CellBounds);
                e.PaintContent(e.ClipBounds);
                e.Handled = true;
            }
            if (e.RowIndex >= 0)
            {
                var row = ((DataGridView)sender).Rows[e.RowIndex];
                if (row != null)
                {
                    if (row.Cells["type"].Value.ToString() == "section")
                    {
                        int alpha = 255;
                        e.Graphics.FillRectangle(Brushes.White, e.CellBounds);
                        Color c1 = Color.FromArgb(alpha, 54, 54, 54);
                        Color c2 = Color.FromArgb(alpha, 62, 62, 62);
                        Color c3 = Color.FromArgb(alpha, 98, 98, 98);

                        LinearGradientBrush br = new LinearGradientBrush(e.CellBounds, c1, c3, 90, true);
                        ColorBlend cb = new ColorBlend();
                        cb.Positions = new[] { 0, (float)0.5, 1 };
                        cb.Colors = new[] { c1, c2, c3 };
                        br.InterpolationColors = cb;


                        e.Graphics.FillRectangle(br, e.CellBounds);
                        e.PaintContent(e.ClipBounds);
                        e.Handled = true;
                    }
                }

            }
        }

        private DataColumn newDataColumn(string caption, string field)
        {
            DataColumn dc = new DataColumn();
            dc.Caption = caption;
            dc.ColumnName = field;
            dc.DataType = System.Type.GetType("System.String");
            return dc;
        }

        private void populatePropertyInspector(string data, string key, DataGridView target, bool allow_default)
        {
            JObject json = JObject.Parse(data);
            foreach (var x in json)
            {
                string name = x.Key;
                if (x.Key == key)
                {
                    JArray value = (JArray)x.Value;

                    DataTable dt = new DataTable();
                    dt.Columns.Add(newDataColumn("Field", "display"));
                    dt.Columns.Add(newDataColumn("name", "name"));
                    dt.Columns.Add(newDataColumn("type", "type"));
                    dt.Columns.Add(newDataColumn("default", "default"));
                    dt.Columns.Add(newDataColumn("Value", "value"));

                    foreach (var pp in value)
                    {
                        DataRow dr = dt.NewRow();
                        dr["display"] = pp["display"].ToString();
                        dr["type"] = pp["type"].ToString();
                        dr["name"] = pp["name"].ToString();
                        dr["default"] = "";
                        string _value = "";
                        if (allow_default)
                        {
                            if (pp["default"] != null)
                            {
                                _value = pp["default"].ToString();
                            }
                        }
                        if (pp["default"] != null)
                        {
                            dr["default"] = pp["default"].ToString();
                        }
                        if (pp["value"] != null)
                        {
                            _value = pp["value"].ToString();
                        }
                        dr["value"] = _value;

                        dt.Rows.Add(dr);
                    }

                    target.Columns.Clear();

                    target.DataSource = dt;

                    target.Columns["name"].Visible = false;
                    target.Columns["type"].Visible = false;
                    target.Columns["default"].Visible = false;
                    target.Columns["display"].ReadOnly = true;
                    target.Columns["display"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    target.Columns["display"].HeaderText = "Field";
                    target.Columns["value"].HeaderText = "Value";
                    target.Columns["value"].ReadOnly = true;
                    target.Columns["value"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    target.Columns["display"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    target.Columns["value"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    DataGridViewButtonColumn column1 =
                                new DataGridViewButtonColumn();
                    column1.Name = "action";
                    column1.UseColumnTextForButtonValue = true;
                    column1.HeaderText = "";
                    column1.Text = "...";
                    column1.Width = 30;
                    target.Columns.Add(column1);

                    DataGridViewCellStyle dgvNoButtonStyle = new DataGridViewCellStyle();
                    dgvNoButtonStyle.Padding = new System.Windows.Forms.Padding(75, 0, 0, 0);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        target[0, i].Style.Font = new Font(target.DefaultCellStyle.Font.Name, target.DefaultCellStyle.Font.Size, FontStyle.Bold);
                        if (dt.Rows[i]["type"].ToString() == "section")
                        {
                            target[0, i].Style.ForeColor = Color.White;
                            target[0, i].Style.SelectionForeColor = Color.White;
                            target[5, i].Style = dgvNoButtonStyle;
                        }
                    }

                }
            }
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            populatePropertyInspector(txtJson.Text, "parameters", dgvProperties, true);
        }

        private void cellEdit(int rowIndex)
        {
            var row = dgvProperties.Rows[rowIndex];
            frmEditor edt = new frmEditor();
            edt.value = row.Cells["value"].Value.ToString();
            if (edt.ShowDialog() == DialogResult.OK)
            {
                row.Cells["value"].Value = edt.value;
            }
        }

        private void dgvProperties_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5)
            {
                cellEdit(e.RowIndex);
            }
        }

        private void dgvProperties_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            cellEdit(e.RowIndex);
        }

    }
}