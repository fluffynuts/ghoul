using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PeanutButter.Utils;

namespace Ghoul
{
    public partial class CheckListDialog<T> : Form
    {
        private readonly string[] _interestingProperties;
        private CheckedItem<T>[] _items;

        public CheckListDialog(T[] items) : this(items, ListAllPropertiesOf<T>())
        {
        }

        private static string[] ListAllPropertiesOf<T1>()
        {
            return typeof(T1).GetProperties()
                .Select(pi => pi.Name)
                .ToArray();
        }

        public CheckListDialog(T[] items, string[] interestingProperties)
        {
            _interestingProperties = interestingProperties;
            InitializeComponent();
            Render(items);
            listView1.OwnerDraw = true;
            listView1.View = View.Details;
            listView1.CheckBoxes = true;
            Icon = Resources.Ghoul;
        }

        private void Render(T[] items)
        {
            _items = items.Select(o => new CheckedItem<T>(o)).ToArray();
        }

        public CheckListDialogResult<T> Prompt()
        {
            listView1.Clear();
            var itemType = typeof(T);
            var itemProperties = itemType.GetProperties().Where(pi => _interestingProperties.Contains(pi.Name)
                                                                ).ToArray();
            new[] {""}.Concat(itemProperties.Select(pi => pi.Name))
                .ForEach(text => listView1.Columns.Add(text, -2, HorizontalAlignment.Left));

            listView1.Items.AddRange(
                _items.Select(o =>
                    new ListViewItem(
                        new[] {""}.Concat(itemProperties.Select(
                            pi => pi.GetValue(o.ItemData, null)?.ToString() ?? ""
                        )).ToArray()
                    )
                ).ToArray()
            );

            var dialogResult = ShowDialog();
            return new CheckListDialogResult<T>(
                dialogResult,
                dialogResult == DialogResult.OK
                    ? _items.Where(o => o.Checked).Select(o => o.ItemData).ToArray()
                    : new T[0]
            );
        }

        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.DrawBackground();
                var value = false;
                try
                {
                    value = Convert.ToBoolean(e.Header.Tag);
                }
                catch (Exception)
                {
                    /* ignore */
                }

                CheckBoxRenderer.DrawCheckBox(e.Graphics,
                    new Point(e.Bounds.Left + 4, e.Bounds.Top + 4),
                    value
                        ? System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal
                        : System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal);
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listView1_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != 0)
                return;
            var value = false;
            try
            {
                value = Convert.ToBoolean(listView1.Columns[e.Column].Tag);
            }
            catch (Exception)
            {
                /* ignore */
            }

            listView1.Columns[e.Column].Tag = !value;
            foreach (ListViewItem item in listView1.Items)
                item.Checked = !value;

            listView1.Invalidate();
        }
    }

    public class CheckedItem<T>
    {
        public T ItemData { get; }
        public bool Checked { get; set; }

        public CheckedItem(T itemData)
        {
            ItemData = itemData;
        }
    }

    public class CheckListDialogResult<T>
    {
        public T[] SelectedItems { get; }
        public DialogResult Result { get; }

        public CheckListDialogResult(
            DialogResult result,
            T[] selectedItems
        )
        {
            Result = result;
            SelectedItems = selectedItems;
        }
    }
}