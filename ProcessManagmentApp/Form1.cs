using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProcessManagmentApp
{
    public partial class ProcessManager : Form
    {
        private Dictionary<string, string> processes = new Dictionary<string, string>();// the key is the process id and the value is it's name.
        private int toSort;

        /*
         ListViewColumnSorter is a private class which can only be called from ProcessManager, and it's implementing the IComparer
         iterface which overrides the compare method for the Sorting of the items, by the process name or by the process id 
         */
        private class ListViewColumnSorter : IComparer
        {
            /// Specifies the column to be sorted
            public int ColumnToSort { get; set; }

            /// Specifies the order in which to sort (i.e. 'Ascending').
            public SortOrder OrderOfSort { get; set; }

            public ListViewColumnSorter(int columnIndex)
            {
                ColumnToSort = columnIndex;
                OrderOfSort = SortOrder.None;
            }

            
            public int Compare(object x, object y)
            {
                int compareResult=0;
                int left, right;
                ListViewItem listviewX, listviewY;

                // Cast the objects to be compared to ListViewItem objects
                listviewX = (ListViewItem)x;
                listviewY = (ListViewItem)y;

                // compare the 2 items depending on the selected column
                switch (ColumnToSort)
                {
                    case 0:
                        compareResult = string.Compare(listviewX.Name, listviewY.Name);
                        break;
                    case 1:
                        left = Int32.Parse(listviewX.SubItems[ColumnToSort].Text);
                        right = Int32.Parse(listviewY.SubItems[ColumnToSort].Text);
                        compareResult = left > right ? 1 : -1;
                        break;
                }

                if (OrderOfSort == SortOrder.Ascending)
                {
                    return compareResult;
                }
                else if (OrderOfSort == SortOrder.Descending)
                {
                    return (-compareResult);
                }
                return 0;
                
            }


        }
        public ProcessManager()
        {;
            InitializeComponent();
            showProcessess();
            toSort = 0;
        }
        /*
         showProcesses - refreshes the listview items whenever we kill some of the processess and adds the current running 
                         processess to the list.
         */
        private void showProcessess()
        {
            listView1.Items.Clear();
            Process[] s = Process.GetProcesses();
            
            foreach (Process process in s)
            {
                ListViewItem item = new ListViewItem(process.ProcessName);
                item.Name = process.ProcessName;
                item.SubItems.Add(process.Id.ToString());
                item.Tag = process;
                listView1.Items.Add(item);
                if(!processes.ContainsKey(process.Id.ToString()))
                    processes.Add(process.Id.ToString(), process.ProcessName);


            }
            label1.Text = listView1.Items.Count.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //check if text was entered to the textbox and see if it's in the processess' list or in the dictionary of the processess and mark the
            //relevant item as selected
            string processName = textBox1.Text;
            if (listView1.Items.ContainsKey(processName))
                listView1.Items[listView1.Items.IndexOfKey(processName)].Selected = true;

            else if (processes.ContainsKey(processName))
                listView1.Items[listView1.Items.IndexOfKey(processes[processName])].Selected = true;

            listView1.Select();
            //pop up message box if the process doesn't exist
            if(listView1.SelectedItems.Count==0)
                MessageBox.Show("Process "+processName+ " Not Running", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            foreach (ListViewItem process in listView1.SelectedItems)
            {
                try
                {
                    //try killing the processess from the selected items. exception is thrown if we don't have permission for a specific process
                    ((Process)(process.Tag)).Kill();
                    processes.Remove(((Process)(process.Tag)).Id.ToString());
                    MessageBox.Show("Process " + ((Process)(process.Tag)).ProcessName+" Is Terminated", "Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Can't Kill Process " + ((Process)(process.Tag)).ProcessName, exception.Message, MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
               
               
            showProcessess();


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewColumnSorter listViewColumnSorter = new ListViewColumnSorter(e.Column);
            listView1.ListViewItemSorter = listViewColumnSorter;
            switch (toSort)
            {
                case 0:
                    listViewColumnSorter.OrderOfSort = SortOrder.Ascending;
                    listView1.Sort();
                    toSort++;
                    break;
                case 1:
                    listViewColumnSorter.OrderOfSort = SortOrder.Descending;
                    listView1.Sort();
                    toSort--;
                    break;
            }
            
        }
        private void label1_Click(object sender, EventArgs e)
        {
            showProcessess();
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void ProcessManager_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {
            showProcessess();
        }
    }
}
