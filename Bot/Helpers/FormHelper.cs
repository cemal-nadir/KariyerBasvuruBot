
namespace Bot.Helpers;

public static class FormHelper
{
    public static DialogResult InputBox(string title, string promptText, ref string value)
    {
        if (title == null) throw new ArgumentNullException(nameof(title));
        if (promptText == null) throw new ArgumentNullException(nameof(promptText));
        if (value == null) throw new ArgumentNullException(nameof(value));
        var form = new Form();
        var label = new Label();
        var textBox = new TextBox();
        var buttonOk = new Button();
        var buttonCancel = new Button();
        form.Text = title;
        label.Text = promptText;
        buttonOk.Text = "Tamam";
        buttonCancel.Text = "İptal";
        buttonOk.DialogResult = DialogResult.OK;
        buttonCancel.DialogResult = DialogResult.Cancel;
        label.SetBounds(36, 36, 372, 13);
        textBox.SetBounds(36, 86, 700, 20);
        buttonOk.SetBounds(228, 160, 160, 60);
        buttonCancel.SetBounds(400, 160, 160, 60);
        label.AutoSize = true;
        form.ClientSize = new Size(796, 307);
        form.FormBorderStyle = FormBorderStyle.FixedDialog;
        form.StartPosition = FormStartPosition.CenterScreen;
        form.MinimizeBox = false;
        form.MaximizeBox = false;
        form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
        form.AcceptButton = buttonOk;
        form.CancelButton = buttonCancel;
        var dialogResult = form.ShowDialog();
        value = textBox.Text;
        return dialogResult;
    }

    public static bool IsInsertingValueUnique(this ListBox listBox, string value)
    {
        return listBox.Items.Cast<string>().All(x => x != value);
    }
    public static bool IsInsertingValueUnique(this CheckedListBox listBox, string value)
    {
        return listBox.Items.Cast<string>().All(x => x != value);
    }
    public static void RemoveListItems(this ListBox list)
    {
        if (!list.SelectedItems.Cast<string>().Any())
            list.Items.Clear();

        var selectedItems = list.SelectedItems;


        for (var i = selectedItems.Count - 1; i >= 0; i--)
            list.Items.Remove(selectedItems[i]);
    }

}


