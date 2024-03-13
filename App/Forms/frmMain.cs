using System;
using System.Linq;
using System.Windows.Forms;
using Bot.Helpers;
using Bot.Workers;
using Core.Bot;
using Core.Configs.Kariyer;
using FormHelper = App.Helpers.FormHelper;

namespace App.Forms
{
    public partial class FrmMain : Form
    {
        private readonly IWorker<KariyerConfig,KariyerApplyJobConfig> _worker;
        public FrmMain()
        {
            InitializeComponent();
            GetSavedConfigs();
            _worker = new KariyerBot();
            _worker.ApplicationStartPath = Application.StartupPath;
            _worker.StartBrowser();
        }

        private void countryRemove_Click(object sender, EventArgs e)
        {
            FormHelper.RemoveListItems(listCountry);
        }

        private void countryAdd_Click(object sender, EventArgs e)
        {
            var value = string.Empty;
            if (FormHelper.InputBox("Ülke Ekle", "Eklemek İstediğiniz Ülkeyi Girin", ref value) is DialogResult.OK &&
                FormHelper.IsInsertingValueUnique(listCountry, value))
            {
                listCountry.Items.Add(value);
            }
        }

        private void cityRemove_Click(object sender, EventArgs e)
        {
            FormHelper.RemoveListItems(listCity);
        }

        private void cityAdd_Click(object sender, EventArgs e)
        {
            var value = string.Empty;
            if (FormHelper.InputBox("Şehir Ekle", "Eklemek İstediğiniz Şehri Girin", ref value) is DialogResult.OK &&
                FormHelper.IsInsertingValueUnique(listCity, value))
            {
                listCity.Items.Add(value);
            }
        }

        private void countyAdd_Click(object sender, EventArgs e)
        {
            var value = string.Empty;
            if (FormHelper.InputBox("İlçe Ekle", "Eklemek İstediğiniz İlçeyi Girin", ref value) is DialogResult.OK &&
                FormHelper.IsInsertingValueUnique(listCounty, value))
            {
                listCounty.Items.Add(value);
            }
        }

        private void countyRemove_Click(object sender, EventArgs e)
        {
            FormHelper.RemoveListItems(listCounty);
        }

        private void sectorAdd_Click(object sender, EventArgs e)
        {
            var value = string.Empty;
            if (FormHelper.InputBox("Sektör Ekle", "Eklemek İstediğiniz Şirket Sektörünü Girin", ref value) is DialogResult.OK &&
                FormHelper.IsInsertingValueUnique(listSector, value))
            {
                listSector.Items.Add(value);
            }
        }

        private void addDepartment_Click(object sender, EventArgs e)
        {
            var value = string.Empty;
            if (FormHelper.InputBox("Departman Ekle", "Eklemek İstediğiniz Departmanı Girin", ref value) is DialogResult.OK &&
                FormHelper.IsInsertingValueUnique(listDepartment, value))
            {
                listDepartment.Items.Add(value);
            }
        }

        private void addPosition_Click(object sender, EventArgs e)
        {
            var value = string.Empty;
            if (FormHelper.InputBox("Pozisyon Ekle", "Eklemek İstediğiniz Pozisyonu Girin", ref value) is DialogResult.OK &&
                FormHelper.IsInsertingValueUnique(listPosition, value))
            {
                listPosition.Items.Add(value);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            _worker.Config = new KariyerConfig()
            {
                Countries = listCountry.Items.Count > 0 ? listCountry.Items.Cast<string>() : null,
                Words = listWordList.Items.Count > 0 ? listWordList.Items.Cast<string>() : null,
                Cities = listCity.Items.Count > 0 ? listCity.Items.Cast<string>() : null,
                Counties = listCounty.Items.Count > 0 ? listCounty.Items.Cast<string>() : null,
                Date = comboDate.SelectedItem?.ToString(),
                ExpeirenceTime = comboExpeirenceTime.SelectedItem?.ToString(),
                DisabilityJobs = comboHandicappedPerson.SelectedItem?.ToString(),
                RemoteJob = checkSingle.GetItemCheckState(0) == CheckState.Checked,
                FirstTimePublished = checkSingle.GetItemCheckState(1) == CheckState.Checked,
                HybridJob = checkSingle.GetItemCheckState(2) == CheckState.Checked,
                OnSite = checkSingle.GetItemCheckState(3) == CheckState.Checked,
                JobsForYou = checkAdvertProperties.GetItemCheckState(0) == CheckState.Checked,
                SavedJobs = checkAdvertProperties.GetItemCheckState(1) == CheckState.Checked,
                FollowedCompaniesJobs = checkAdvertProperties.GetItemCheckState(2) == CheckState.Checked,
                ViewedJobs = checkAdvertProperties.GetItemCheckState(3) == CheckState.Checked,
                Sectors = listSector.Items.Count > 0 ? listSector.Items.Cast<string>() : null,
                PositionLevels = checkPositionLevel.CheckedItems.Count > 0 ? checkPositionLevel.CheckedItems.Cast<string>() : null,
                Departments = listDepartment.Items.Count > 0 ? listDepartment.Items.Cast<string>() : null,
                WorkingTypes = checkWorkType.CheckedItems.Count > 0 ? checkWorkType.CheckedItems.Cast<string>() : null,
                EducationLevels = checkEducationLevel.CheckedItems.Count > 0 ? checkEducationLevel.CheckedItems.Cast<string>() : null,
                Positions = listPosition.Items.Count > 0 ? listPosition.Items.Cast<string>() : null,
                CompanyProperties = checkCompanyProperties.CheckedItems.Count > 0 ? checkCompanyProperties.CheckedItems.Cast<string>() : null,
                JobLanguages = checkLanguage.CheckedItems.Count > 0 ? checkLanguage.CheckedItems.Cast<string>() : null
            };
            _worker.ApplyJobConfigs = MainHelper.GetSavedKariyerApplyConfig();
            var isCancelled = false;
            
            while (!_worker.CheckLogin()&&isCancelled == false)
            {
              
                if (MessageBox.Show("Lütfen Kariyer.net\'e Giriş Yaptıktan Sonra Tamam\'a Basın", "Giriş Yapın", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
                {
                    isCancelled = true;
                }

                
            }

            if (isCancelled)
            {
                await _worker.StopBot();
                Stop();
                return;
            }

           
            try
            {
               
                Start();
                await _worker.StartBot();
                await _worker.StopBot();
                Stop();
                MessageBox.Show("İş Başvurusu İşlemi Tamamlandı", "Bot Durdu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            }
            catch (Exception ex)
            {
               
                await _worker.StopBot();
                button1_Click(sender,e);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            var config = new KariyerConfig()
            {
                Countries = listCountry.Items.Count > 0 ? listCountry.Items.Cast<string>() : null,
                Words = listWordList.Items.Count > 0 ? listWordList.Items.Cast<string>() : null,
                Cities = listCity.Items.Count > 0 ? listCity.Items.Cast<string>() : null,
                Counties = listCounty.Items.Count > 0 ? listCounty.Items.Cast<string>() : null,
                Date = comboDate.SelectedItem?.ToString(),
                ExpeirenceTime = comboExpeirenceTime.SelectedItem?.ToString(),
                DisabilityJobs = comboHandicappedPerson.SelectedItem?.ToString(),
                RemoteJob = checkSingle.GetItemCheckState(0) == CheckState.Checked,
                FirstTimePublished = checkSingle.GetItemCheckState(1) == CheckState.Checked,
                HybridJob = checkSingle.GetItemCheckState(2) == CheckState.Checked,
                OnSite = checkSingle.GetItemCheckState(3) == CheckState.Checked,
                JobsForYou = checkAdvertProperties.GetItemCheckState(0) == CheckState.Checked,
                SavedJobs = checkAdvertProperties.GetItemCheckState(1) == CheckState.Checked,
                FollowedCompaniesJobs = checkAdvertProperties.GetItemCheckState(2) == CheckState.Checked,
                ViewedJobs = checkAdvertProperties.GetItemCheckState(3) == CheckState.Checked,
                Sectors = listSector.Items.Count > 0 ? listSector.Items.Cast<string>() : null,
                PositionLevels = checkPositionLevel.CheckedItems.Count > 0 ? checkPositionLevel.CheckedItems.Cast<string>() : null,
                Departments = listDepartment.Items.Count > 0 ? listDepartment.Items.Cast<string>() : null,
                WorkingTypes = checkWorkType.CheckedItems.Count > 0 ? checkWorkType.CheckedItems.Cast<string>() : null,
                EducationLevels = checkEducationLevel.CheckedItems.Count > 0 ? checkEducationLevel.CheckedItems.Cast<string>() : null,
                Positions = listPosition.Items.Count > 0 ? listPosition.Items.Cast<string>() : null,
                CompanyProperties = checkCompanyProperties.CheckedItems.Count > 0 ? checkCompanyProperties.CheckedItems.Cast<string>() : null,
                JobLanguages = checkLanguage.CheckedItems.Count > 0 ? checkLanguage.CheckedItems.Cast<string>() : null
            };
            config.SaveKariyerConfigs();
        }
        private void GetSavedConfigs()
        {
            var config = MainHelper.GetSavedKariyerConfig();
            if (config.Countries != null)
                foreach (var country in config.Countries)
                {
                    listCountry.Items.Add(country);
                }
            if (config.Cities != null)
                foreach (var city in config.Cities)
                {
                    listCity.Items.Add(city);
                }
            if (config.Counties != null)
                foreach (var county in config.Counties)
                {
                    listCounty.Items.Add(county);
                }
            if (config.Words != null)
                foreach (var word in config.Words)
                {
                    listWordList.Items.Add(word);
                }
            comboDate.SelectedItem = config.Date;
            comboExpeirenceTime.SelectedItem = config.ExpeirenceTime;
            comboHandicappedPerson.SelectedItem = config.DisabilityJobs;

            if (config.RemoteJob)
                checkSingle.SetItemChecked(0, true);

            if (config.FirstTimePublished)
                checkSingle.SetItemChecked(1, true);
            if (config.HybridJob)
                checkSingle.SetItemChecked(2, true);
            if (config.OnSite)
                checkSingle.SetItemChecked(3, true);
            if (config.JobsForYou)
                checkAdvertProperties.SetItemChecked(0, true);
            if (config.SavedJobs)
                checkAdvertProperties.SetItemChecked(1, true);
            if (config.FollowedCompaniesJobs)
                checkAdvertProperties.SetItemChecked(2, true);
            if (config.ViewedJobs)
                checkAdvertProperties.SetItemChecked(3, true);
            if (config.Sectors != null)
                foreach (var sector in config.Sectors)
                {
                    listSector.Items.Add(sector);
                }
            if (config.PositionLevels != null)
                for (var i = 0; i < checkPositionLevel.Items.Count; i++)
                {
                    if (config.PositionLevels.Any(x => x == checkPositionLevel.Items[i].ToString()))
                        checkPositionLevel.SetItemChecked(i, true);
                }
            if (config.Departments != null)
                foreach (var department in config.Departments)
                {
                    listDepartment.Items.Add(department);
                }
            if (config.WorkingTypes != null)
                for (var i = 0; i < checkWorkType.Items.Count; i++)
                {
                    if (config.WorkingTypes.Any(x => x == checkWorkType.Items[i].ToString()))
                        checkWorkType.SetItemChecked(i, true);
                }
            if (config.EducationLevels != null)
                for (var i = 0; i < checkEducationLevel.Items.Count; i++)
                {
                    if (config.EducationLevels.Any(x => x == checkEducationLevel.Items[i].ToString()))
                        checkEducationLevel.SetItemChecked(i, true);
                }
            if (config.Positions != null)
                foreach (var position in config.Positions)
                {
                    listPosition.Items.Add(position);
                }
            if (config.CompanyProperties != null)
                for (var i = 0; i < checkCompanyProperties.Items.Count; i++)
                {
                    if (config.CompanyProperties.Any(x => x == checkCompanyProperties.Items[i].ToString()))
                        checkCompanyProperties.SetItemChecked(i, true);
                }
            if (config.JobLanguages != null)
                for (var i = 0; i < checkLanguage.Items.Count; i++)
                {
                    if (config.JobLanguages.Any(x => x == checkLanguage.Items[i].ToString()))
                        checkLanguage.SetItemChecked(i, true);
                }
        }

        private void sectorRemove_Click(object sender, EventArgs e)
        {
            FormHelper.RemoveListItems(listSector);
        }

        private void departmentRemove_Click(object sender, EventArgs e)
        {
            FormHelper.RemoveListItems(listDepartment);
        }

        private void removePosition_Click(object sender, EventArgs e)
        {
            FormHelper.RemoveListItems(listPosition);
        }

        private void addWordList_Click(object sender, EventArgs e)
        {
            var value = string.Empty;
            if (FormHelper.InputBox("Kelime Ekle", "İlanda geçmesi gereken kelimeyi girin", ref value) is DialogResult.OK &&
                FormHelper.IsInsertingValueUnique(listWordList, value))
            {
                listWordList.Items.Add(value);
            }
        }

        private void removeWordList_Click(object sender, EventArgs e)
        {
            FormHelper.RemoveListItems(listWordList);
        }

        private void setApplySettings_Click(object sender, EventArgs e)
        {
            var frmApplyConfigs = new FrmApplyConfigs();
            frmApplyConfigs.ShowDialog();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await _worker.StopBot();
            Stop();
        }
        private void Start()
        {
            groupBox2.Enabled = false;
            groupBox5.Enabled = false;
            groupBox13.Enabled = false;
            groupBox18.Enabled = false;
            groupBox25.Enabled = false;
            groupBox24.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = true;
        }
        private void Stop()
        {
            groupBox2.Enabled = true;
            groupBox5.Enabled = true;
            groupBox13.Enabled = true;
            groupBox18.Enabled = true;
            groupBox25.Enabled = true;
            groupBox24.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = false;
        }


    }
}
