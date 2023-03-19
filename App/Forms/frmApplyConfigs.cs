using Core.Configs.Kariyer;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using App.Helpers;

namespace App.Forms
{
    public partial class FrmApplyConfigs : Form
    {
        public FrmApplyConfigs()
        {
            InitializeComponent();
            LoadDataSource();
        }
        private void LoadDataSource()
        {
            var ds = MainHelper.GetSavedKariyerApplyConfig();
            bindingSource1.DataSource = ds;
            dataGridView1.DataSource = bindingSource1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var newDs = bindingSource1.DataSource as IEnumerable<KariyerApplyJobConfig>;
            newDs.SaveKariyerApplyConfigs();
            MessageBox.Show("Başarılı","Ayarlar Başarıyla Kaydedildi",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }
    }
}
