using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Service.Logic;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Helpers
{
    public static class LanguageSelectorHelper
    {
        public static void SetupComboBox(ComboBox cmbLanguage, LanguageService languageService, Action updateLanguageCallback)
        {
            cmbLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLanguage.DrawMode = DrawMode.OwnerDrawFixed;
            cmbLanguage.ItemHeight = 20;

            cmbLanguage.DrawItem += (sender, e) => CmbLanguage_DrawItem(sender, e, (ComboBox)sender);
            cmbLanguage.SelectedIndexChanged += (sender, e) => CmbLanguage_SelectedIndexChanged(sender, e, (ComboBox)sender, languageService, updateLanguageCallback);

            LoadLanguages(cmbLanguage, languageService);
        }

        public static void SetupToolStripComboBox(ToolStripComboBox cmbLanguage, LanguageService languageService, Action updateLanguageCallback)
        {
            cmbLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLanguage.ComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            cmbLanguage.ComboBox.ItemHeight = 20;

            cmbLanguage.ComboBox.DrawItem += (sender, e) => CmbLanguage_DrawItem(sender, e, (ComboBox)sender);
            cmbLanguage.SelectedIndexChanged += (sender, e) => CmbLanguage_SelectedIndexChanged(sender, e, cmbLanguage.ComboBox, languageService, updateLanguageCallback);

            LoadLanguages(cmbLanguage.ComboBox, languageService);
        }

        private static void LoadLanguages(ComboBox cmbLanguage, LanguageService languageService)
        {
            var languages = languageService.GetLanguages();
            cmbLanguage.DataSource = new BindingSource(languages, null);
            cmbLanguage.DisplayMember = "Value";
            cmbLanguage.ValueMember = "Key";

            string currentCulture = Thread.CurrentThread.CurrentUICulture.Name;
            if (languages.ContainsKey(currentCulture))
            {
                cmbLanguage.SelectedValue = currentCulture;
            }
            else
            {
                if (cmbLanguage.Items.Count > 0) cmbLanguage.SelectedIndex = 0;
            }
        }

        private static void CmbLanguage_DrawItem(object sender, DrawItemEventArgs e, ComboBox cmbLanguage)
        {
            if (e.Index < 0) return;
            e.DrawBackground();

            var item = (KeyValuePair<string, string>)cmbLanguage.Items[e.Index];
            string code = item.Key;
            string name = item.Value;

            Image flag = FlagHelper.DrawFlag(code, 25, 15);
            e.Graphics.DrawImage(flag, e.Bounds.Left + 2, e.Bounds.Top + 2);
            flag.Dispose(); // Ensure the image is disposed after drawing if it's not cached

            using (Brush textBrush = new SolidBrush(e.ForeColor))
            {
                e.Graphics.DrawString(name, e.Font, textBrush, e.Bounds.Left + 30, e.Bounds.Top + 2);
            }

            e.DrawFocusRectangle();
        }

        private static void CmbLanguage_SelectedIndexChanged(object sender, EventArgs e, ComboBox cmbLanguage, LanguageService languageService, Action updateLanguageCallback)
        {
            if (cmbLanguage.SelectedValue is string code)
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(code);
                languageService.SaveUserLanguage(code);
                updateLanguageCallback?.Invoke();
            }
        }
    }
}