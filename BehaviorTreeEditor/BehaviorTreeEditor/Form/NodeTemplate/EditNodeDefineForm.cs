﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BehaviorTreeEditor
{
    public partial class EditNodeDefineForm : Form
    {
        private NodeDefine m_NodeDefine;
        private NodeDefine m_EditNodeDefine;
        private NodeTemplateForm m_ClassForm = null;
        private string m_Content;
        private Action m_EditCallback;

        public EditNodeDefineForm(NodeTemplateForm classForm, NodeDefine nodeDefine, Action editCallback)
        {
            m_ClassForm = classForm;
            m_NodeDefine = nodeDefine;
            m_Content = XmlUtility.ObjectToString(m_NodeDefine);
            m_EditNodeDefine = XmlUtility.StringToObject<NodeDefine>(m_Content);
            m_EditCallback = editCallback;
            InitializeComponent();
        }

        private void EditNodeDefineForm_Load(object sender, EventArgs e)
        {
            classTypeTB.Text = m_EditNodeDefine.ClassType;

            nodeTypeCBB.Items.Clear();
            string[] enumNames = Enum.GetNames(typeof(NodeType));
            for (int i = 1; i < enumNames.Length; i++)
            {
                nodeTypeCBB.Items.Add(enumNames[i]);
            }
            nodeTypeCBB.SelectedIndex = (int)m_EditNodeDefine.NodeType - 1;

            CBB_CheckField.Checked = m_EditNodeDefine.CheckField;
            labelTB.Text = m_EditNodeDefine.Label;
            categoryTB.Text = m_EditNodeDefine.Category;
            describeTB.Text = m_EditNodeDefine.Describe;

            BindField(m_EditNodeDefine);
        }

        private void cancelBTN_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void enterBTN_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(classTypeTB.Text.Trim()))
            {
                MainForm.Instance.ShowMessage("请填写类名", "提示");
                return;
            }

            if (categoryTB.Text.Trim().Contains(" "))
            {
                MainForm.Instance.ShowMessage("类别不能有空格", "提示");
                return;
            }

            m_EditNodeDefine.ClassType = classTypeTB.Text.Trim();
            m_EditNodeDefine.NodeType = (NodeType)(nodeTypeCBB.SelectedIndex + 1);
            m_EditNodeDefine.Label = labelTB.Text.Trim();
            m_EditNodeDefine.Describe = describeTB.Text.Trim();
            m_EditNodeDefine.Category = categoryTB.Text.Trim();
            m_EditNodeDefine.CheckField = CBB_CheckField.Checked;

            //校验节点类是否合法
            VerifyInfo verifyNodeDefine = m_EditNodeDefine.VerifyNodeDefine();
            if (verifyNodeDefine.HasError)
            {
                MainForm.Instance.ShowMessage(verifyNodeDefine.Msg);
                return;
            }

            string content = XmlUtility.ObjectToString(m_EditNodeDefine);

            if (m_Content != content)
            {
                m_NodeDefine.UpdateNodeDefine(m_EditNodeDefine);
                MainForm.Instance.ShowInfo("修改成功 时间:" + DateTime.Now);
            }

            this.Close();

            m_EditCallback?.Invoke();
        }

        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exec("CopyField");
        }

        private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exec("PasteField");
        }

        private void 选项ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exec("EditField");
        }

        private void 新建ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exec("NewField");
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exec("DeleteField");
        }

        private void listViewFields_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.C:
                        Exec("CopyField");
                        break;
                    case Keys.V:
                        Exec("PasteField");
                        break;
                    case Keys.Up:
                        Exec("SwapField", true);
                        break;
                    case Keys.Down:
                        Exec("SwapField", false);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.F12:
                        Exec("EditField");
                        break;
                    case Keys.N:
                        Exec("NewField");
                        break;
                    case Keys.Delete:
                        Exec("DeleteField");
                        break;
                }
            }
        }

        private void listViewFields_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks == 1)
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Tag = listViewFields;
                    contextMenuStrip1.Show(listViewFields, e.Location);
                }
            }
            else if (e.Clicks == 2)
            {
                if (listViewFields.SelectedItems.Count == 1)
                {
                    InputValueDialogForm dlg = new InputValueDialogForm("编辑字段", listViewFields.SelectedItems[0].Tag);
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        Exec("Refresh");
                    }
                }
                else
                {
                    Exec("NewField");
                }
                Exec("Refresh");
            }
        }

        private bool Exec(string cmd, params object[] agrs)
        {
            if (string.IsNullOrEmpty(cmd))
                return false;

            switch (cmd)
            {
                case "Refresh":
                    BindField(m_EditNodeDefine);
                    break;
                case "NewField":
                    NewField();
                    break;
                case "EditField":
                    EditField();
                    break;
                case "DeleteField":
                    DeleteField();
                    break;
                case "SwapField":
                    SwapField((bool)agrs[0]);
                    break;
                case "CopyField":
                    CopyField();
                    break;
                case "PasteField":
                    PasteField();
                    break;
            }

            return true;
        }

        private void NewField()
        {
            NodeField field = new NodeField();
            InputValueDialogForm form = new InputValueDialogForm("添加字段", field);
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (m_EditNodeDefine.AddField(field))
                {
                    Exec("Refresh");
                }
            }
        }

        //刷新ListView
        private void BindField(NodeDefine nodeDefine)
        {
            if (nodeDefine == null)
                return;

            listViewFields.Items.Clear();
            foreach (NodeField field in nodeDefine.Fields)
            {
                ListViewItem listViewItem = listViewFields.Items.Add(field.FieldName);
                listViewItem.Tag = field;
                listViewItem.SubItems.Add(field.Label);
                listViewItem.SubItems.Add(EditorUtility.GetFieldTypeName(field.FieldType));
                listViewItem.SubItems.Add(field.DefaultValue != null ? field.DefaultValue.ToString() : string.Empty);
                listViewItem.SubItems.Add(field.Describe);
            }
        }

        //编辑字段
        private void EditField()
        {
            CollectionEditor.EditValue(this, m_EditNodeDefine, "Fields");
        }

        public class FieldListContent
        {
            private List<NodeField> m_DataList = new List<NodeField>();
            public List<NodeField> DataList { get { return m_DataList; } }
        }

        //复制字段
        private void CopyField()
        {
            if (listViewFields.SelectedItems.Count > 0)
            {
                FieldListContent content = new FieldListContent();
                foreach (ListViewItem lvItem in listViewFields.SelectedItems)
                {
                    if (lvItem.Tag is NodeField)
                        content.DataList.Add((NodeField)lvItem.Tag);
                }

                if (content.DataList.Count > 0)
                    Clipboard.SetText(XmlUtility.ObjectToString(content));

                MainForm.Instance.ShowInfo("您复制了" + content.DataList.Count.ToString() + "个字段！！！");
            }
            else
            {
                MainForm.Instance.ShowInfo("您必须选择至少一个进行复制！！！");
                MainForm.Instance.ShowMessage("您必须选择至少一个进行复制！！！", "警告");
            }
        }

        //粘贴字段
        private void PasteField()
        {
            try
            {
                FieldListContent content = (FieldListContent)XmlUtility.StringToObject<FieldListContent>(Clipboard.GetText());

                for (int i = 0; i < content.DataList.Count; i++)
                {
                    NodeField field = content.DataList[i];
                    string fieldName = field.FieldName;
                    while (m_EditNodeDefine.ExistFieldName(fieldName))
                    {
                        fieldName += "_New";
                    }

                    field.FieldName = fieldName;
                    m_EditNodeDefine.AddField(field);
                }
                Exec("Refresh");
                MainForm.Instance.ShowInfo("您粘贴了" + content.DataList.Count + "个字段！！！");
            }
            catch (Exception ex)
            {
                MainForm.Instance.ShowInfo("无法进行粘贴，错误信息：" + ex.Message);
                MainForm.Instance.ShowMessage("无法进行粘贴，错误信息：" + ex.Message, "警告");
            }
        }

        //删除字段
        private void DeleteField()
        {
            if (listViewFields.SelectedIndices.Count == 1)
            {
                int selectIdx = listViewFields.SelectedIndices[0];
                m_EditNodeDefine.Fields.RemoveAt(selectIdx);
                MainForm.Instance.ShowInfo("删除成功");
                Exec("Refresh");
                if (listViewFields.Items.Count > selectIdx)
                    listViewFields.Items[selectIdx].Selected = true;
            }
            else if (listViewFields.SelectedIndices.Count == 0)
            {
                MainForm.Instance.ShowInfo("请选择一条记录进行删除");
            }
            else
            {
                MainForm.Instance.ShowInfo("无法一次删除多个记录");
            }
        }

        //交换
        private void SwapField(bool up)
        {
            if (listViewFields.SelectedIndices.Count > 1)
            {
                MainForm.Instance.ShowInfo("请选择一条记录进行交换");
                MainForm.Instance.ShowMessage("请选择一条记录进行交换", "警告");
                return;
            }

            int selectIdx = listViewFields.SelectedIndices[0];
            if (up)
            {
                //第一个不能往上交换
                if (selectIdx == 0)
                    return;

                int preIdx = selectIdx - 1;

                NodeField preField = m_EditNodeDefine.Fields[preIdx];
                NodeField selectedField = m_EditNodeDefine.Fields[selectIdx];

                m_EditNodeDefine.Fields[preIdx] = selectedField;
                m_EditNodeDefine.Fields[selectIdx] = preField;

                selectIdx = preIdx;
            }
            else
            {
                //最后一个不能往下交换
                if (selectIdx == listViewFields.Items.Count - 1)
                    return;

                int nextIdx = selectIdx + 1;

                NodeField preField = m_EditNodeDefine.Fields[nextIdx];
                NodeField selectedField = m_EditNodeDefine.Fields[selectIdx];

                m_EditNodeDefine.Fields[nextIdx] = selectedField;
                m_EditNodeDefine.Fields[selectIdx] = preField;

                selectIdx = nextIdx;
            }

            Exec("Refresh");
            MainForm.Instance.ShowInfo("交换成功 时间:" + DateTime.Now);
            listViewFields.Items[selectIdx].Selected = true;
        }
    }
}
