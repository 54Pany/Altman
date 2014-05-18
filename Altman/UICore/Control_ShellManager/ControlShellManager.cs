﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Altman.LogicCore;
using Altman.ModelCore;
using PluginFramework;

namespace Altman.UICore.Control_ShellManager
{
    public partial class ControlShellManager : UserControl
    {
        private ShellManager _shellManager = null;

        public ControlShellManager(IEnumerable<IPlugin> plugins)
        {
            InitializeComponent();
            this.Dock = System.Windows.Forms.DockStyle.Fill;

            //创建listview
            //CreateListView();

            _shellManager = new ShellManager();
            _shellManager.GetDataTableCompletedToDo += _shellManager_GetDataTableCompletedToDo;
            _shellManager.DeleteCompletedToDo += _shellManager_DeleteCompletedToDo;
            _shellManager.InsertCompletedToDo += _shellManager_InsertCompletedToDo;
            _shellManager.UpdateCompletedToDo += _shellManager_UpdateCompletedToDo;

            //载入shell数据
            LoadWebshellData();

            //添加插件到右键菜单
            foreach (var plugin in plugins)
            {
                string title = plugin.PluginAttribute.Title;

                //添加到Tsmi_Plugins中
                ToolStripMenuItem pluginItem = new ToolStripMenuItem();
                pluginItem.Name = title;
                pluginItem.Text = title;
                pluginItem.Click += pluginItem_Click;
                pluginItem.Tag = plugin;
                rightMenu_Webshell.Items.Add(pluginItem);
            }
        }

        private void pluginItem_Click(object sender, EventArgs e)
        {
            if (lv_shell.SelectedItems.Count > 0)
            {
                ToolStripMenuItem item = sender as ToolStripMenuItem;
                if (item != null)
                {
                    IPlugin plugin = item.Tag as IPlugin;

                    ShellStruct shellStruct = (ShellStruct) lv_shell.SelectedItems[0].Tag;
                    shellStruct.TimeOut = 8000;

                    UserControl view = plugin.GetUi(shellStruct);
                    //创建新的tab标签
                    TabCore.CreateNewTabPage(plugin.PluginAttribute.Title, view);
                }
            }
        }

        /// <summary>
        /// 创建listview
        /// </summary>
        private void CreateListView()
        {
            lv_shell = new ListView
            {
                ContextMenuStrip = this.rightMenu_Webshell,
                Dock = System.Windows.Forms.DockStyle.Fill,
                FullRowSelect = true,
                GridLines = true,
                Location = new System.Drawing.Point(0, 0),
                MultiSelect = false,
                Name = "lv_shell",
                Size = new System.Drawing.Size(648, 315),
                TabIndex = 2,
                UseCompatibleStateImageBehavior = false,
                View = System.Windows.Forms.View.Details
            };

            //添加webshell列
            lv_shell.Columns.Add("Id", 0);
            //1-3
            lv_shell.Columns.Add("序号", 30, HorizontalAlignment.Left);
            lv_shell.Columns[1].TextAlign = HorizontalAlignment.Left;
            lv_shell.Columns.Add("项目编号", 80);
            lv_shell.Columns[2].TextAlign = HorizontalAlignment.Left;
            lv_shell.Columns.Add("级别", 40);
            lv_shell.Columns[3].TextAlign = HorizontalAlignment.Center;
            //4-7
            lv_shell.Columns.Add("状态", 40);
            lv_shell.Columns[4].TextAlign = HorizontalAlignment.Center;
            lv_shell.Columns.Add("Shell地址", 250);
            lv_shell.Columns[5].TextAlign = HorizontalAlignment.Left;
            lv_shell.Columns.Add("类型", 50);
            lv_shell.Columns[6].TextAlign = HorizontalAlignment.Center;
            lv_shell.Columns.Add("服务器编码", 0);
            lv_shell.Columns[7].TextAlign = HorizontalAlignment.Left;
            //8-10
            lv_shell.Columns.Add("国家", 60);
            lv_shell.Columns[8].TextAlign = HorizontalAlignment.Left;
            lv_shell.Columns.Add("备注", 120);
            lv_shell.Columns[9].TextAlign = HorizontalAlignment.Left;            
            lv_shell.Columns.Add("添加时间", 100);
            lv_shell.Columns[10].TextAlign = HorizontalAlignment.Center;

            this.Controls.Add(lv_shell);
        }

        /// <summary>
        /// 载入webshell数据
        /// </summary>
        public  void LoadWebshellData()
        {
            int num = 1;
            lv_shell.Items.Clear();
            DataTable dataTable = _shellManager.GetDataTable();
            if (dataTable == null)
            {
                return;
            }
            foreach (DataRow row in dataTable.Rows)
            {
                ShellStruct shellStruct = new ShellStruct();

                shellStruct.Id = row["id"].ToString();
                shellStruct.TargetId = row["target_id"].ToString();
                shellStruct.TargetLevel = row["target_level"].ToString();

                shellStruct.ShellUrl = row["shell_url"].ToString();
                shellStruct.ShellPwd = row["shell_pwd"].ToString();
                shellStruct.ShellType = row["shell_type"].ToString();
                shellStruct.ShellExtraSetting = row["shell_extra_setting"].ToString();
                shellStruct.ServerCoding = row["server_coding"].ToString();
                shellStruct.WebCoding = row["web_coding"].ToString();

                shellStruct.Area = row["area"].ToString();
                shellStruct.Remark = row["remark"].ToString();
                shellStruct.AddTime = row["add_time"].ToString();

                string[] items = new string[] { 
                    shellStruct.Id, 
                    num++.ToString(), 
                    shellStruct.TargetId, 
                    shellStruct.TargetLevel,
                    "-1", 
                    shellStruct.ShellUrl, 
                    shellStruct.ShellType,
                    shellStruct.ServerCoding, 
                    shellStruct.Area,
                    shellStruct.Remark,
                    shellStruct.AddTime
                    };

                ListViewItem viewItem = new ListViewItem(items);
                viewItem.Tag = shellStruct;
                lv_shell.Items.Add(viewItem);
            }
        }

        #region 数据获取/插入/删除/更新事件
        private void _shellManager_UpdateCompletedToDo(object sender, ShellManager.CompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
        }
        private void _shellManager_InsertCompletedToDo(object sender, ShellManager.CompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
        }
        private void _shellManager_DeleteCompletedToDo(object sender, ShellManager.CompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
        }
        private void _shellManager_GetDataTableCompletedToDo(object sender, ShellManager.CompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        #endregion

        #region 右键菜单事件
        private void OnWebshellChange(object sender, EventArgs e)
        {
            LoadWebshellData();
        }

        private void item_add_Click(object sender, EventArgs e)
        {
            FormEditWebshell editwebshell = new FormEditWebshell();
            editwebshell.WebshellWatchEvent += OnWebshellChange;
            editwebshell.Show();
        }

        private void item_alter_Click(object sender, EventArgs e)
        {
            if (lv_shell.SelectedItems.Count > 0)
            {
                ShellStruct shellStruct = (ShellStruct)lv_shell.SelectedItems[0].Tag;
                //ShellStruct shellStruct = (ShellStruct)lv_shell.SelectedItems[0].Tag;

                FormEditWebshell editwebshell = new FormEditWebshell(shellStruct);
                editwebshell.WebshellWatchEvent += OnWebshellChange;
                editwebshell.Show();
            }
        }

        private void item_del_Click(object sender, EventArgs e)
        {
            if (lv_shell.SelectedItems.Count > 0)
            {
                int id = int.Parse(((ShellStruct)lv_shell.SelectedItems[0].Tag).Id);
                _shellManager.Delete(id);
                LoadWebshellData();
            }
        }

        #endregion
    }
}
