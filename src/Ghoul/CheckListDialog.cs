using System;
using System.Collections.Generic;
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
        private ColumnHeader[] _headers;

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
            StoreItems(items);
            listView1.OwnerDraw = true;
            listView1.View = View.Details;
            listView1.CheckBoxes = true;
            Icon = Resources.Ghoul;
        }

        private ColumnHeader[] GetColumnHeadersInOrder()
        {
            var cols = new List<ColumnHeader>();
            foreach (var header in listView1.Columns)
            {
                cols.Add(header as ColumnHeader);
            }

            return cols.OrderBy(c => c.DisplayIndex).ToArray();
        }

        private void StoreItems(T[] items)
        {
            _items = items.Select(o => new CheckedItem<T>(o)).ToArray();
        }

        public CheckListDialogResult<T> Prompt()
        {
            listView1.Clear();
            var itemType = typeof(T);
            var itemProperties = itemType.GetProperties().Where(
                pi => _interestingProperties.Contains(pi.Name)
            ).ToArray();
            new[] {""}.Concat(itemProperties.Select(pi => pi.Name))
                .ForEach(text => listView1.Columns.Add(text, -2, HorizontalAlignment.Left));

            _headers = GetColumnHeadersInOrder();

            listView1.Items.AddRange(_items.Select(o =>
            {
                var listItem = new ListViewItem(
                    new[] { "" }.Concat(
                        itemProperties.Select(pi => pi.GetValue(o.ItemData, null)?.ToString() ?? "")
                        ).ToArray()
                        );
                listItem.Tag = o;
                return listItem;
            }).ToArray());

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

                CheckBoxRenderer.DrawCheckBox(
                    e.Graphics,
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

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            var col = GetColumnAt(e.X);
            if (col != 0)
                return;
            var item = listView1.GetItemAt(e.X, e.Y);
            var data = item.Tag as CheckedItem<T>;
            if (data == null)
                return;
            data.Checked = !data.Checked;
        }

        private int GetColumnAt(int xpos)
        {
            xpos += Win32Api.GetScrollPos(listView1.Handle, Win32Api.ScrollbarOrientation.SB_HORZ);
            for (var i = 0; i < _headers.Length; i++)
            {
                xpos -= _headers[i].Width;
                if (xpos < 0)
                    return i;
            }

            return -1;
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