using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Ghoul.Native;
using PeanutButter.Utils;

namespace Ghoul.Ui
{
    public interface ICheckListDialogFactory
    {
        CheckListDialog<T> Create<T>(T[] items, string[] interestingProperties);
    }

    public class CheckListDialogFactory
        : ICheckListDialogFactory
    {
        private readonly IIconProvider _iconProvider;

        public CheckListDialogFactory(IIconProvider iconProvider)
        {
            _iconProvider = iconProvider;
        }

        public CheckListDialog<T> Create<T>(T[] items, string[] interestingProperties)
        {
            return new CheckListDialog<T>(items, interestingProperties)
            {
                Icon = _iconProvider.MainIcon()
            };
        }
    }

    public partial class CheckListDialog<T> : Form
    {
        private readonly string[] _interestingProperties;
        private CheckedItem<T>[] _items;
        private ColumnHeader[] _headers;

        // ReSharper disable once UnusedMember.Global
        public CheckListDialog(T[] items) 
            : this(items, ListAllPropertiesOf<T>())
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

            listView1.Items.AddRange(
                _items.Select(
                    o => MakeListViewItemFor(itemProperties, o)
                ).ToArray());

            var dialogResult = ShowDialog();
            return new CheckListDialogResult<T>(
                dialogResult,
                dialogResult == DialogResult.OK
                    ? _items.Where(o => o.Checked).Select(o => o.ItemData).ToArray()
                    : new T[0]
            );
        }

        private static ListViewItem MakeListViewItemFor(
            PropertyInfo[] itemProperties,
            CheckedItem<T> data)
        {
            return new ListViewItem(
                new[] {""}.Concat(
                    itemProperties.Select(
                        pi => pi.GetValue(data.ItemData, null)?.ToString() ?? ""
                    )
                ).ToArray()
            )
            {
                Tag = data
            };
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
            {
                var data = GetDataFor(item);
                data.Checked = item.Checked = !value;
            }

            listView1.Invalidate();
        }

        private CheckedItem<T> GetDataFor(ListViewItem item)
        {
            return item.Tag is CheckedItem<T> data
                ? data
                : throw new InvalidOperationException("Checklist should only contain Checkeditems");
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            var col = GetColumnAt(e.X);
            if (col != 0)
                return;
            var item = listView1.GetItemAt(e.X, e.Y);
            if (!(item.Tag is CheckedItem<T> data))
                return;
            data.Checked = !data.Checked;
        }

        private int GetColumnAt(int xpos)
        {
            xpos += Win32Api.GetScrollPos(listView1.Handle, Win32Api.ScrollbarOrientation.Horizontal);
            for (var i = 0; i < _headers.Length; i++)
            {
                xpos -= _headers[i].Width;
                if (xpos < 0)
                    return i;
            }

            return -1;
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Space)
                return;

            foreach (ListViewItem item in listView1.SelectedItems)
            {
                var newValue = !item.Checked;
                var data = GetDataFor(item);
                item.Checked = data.Checked = newValue;
            }

            e.SuppressKeyPress = true;
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