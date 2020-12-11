<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.StatusBox_lbl = New System.Windows.Forms.Label
        Me.StatusBox_txtbx = New System.Windows.Forms.TextBox
        Me.FormUpdateTimer = New System.Windows.Forms.Timer(Me.components)
        Me.ANxVoltageToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.ToggleLEDToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.PushbuttonStateTooltip = New System.Windows.Forms.ToolTip(Me.components)
        Me.ReadWriteThread = New System.ComponentModel.BackgroundWorker
        Me.txtBoxResponse = New System.Windows.Forms.TextBox
        Me.TextBox1 = New System.Windows.Forms.TextBox
        Me.TextBox2 = New System.Windows.Forms.TextBox
        Me.Button2 = New System.Windows.Forms.Button
        Me.DateTimePicker1 = New System.Windows.Forms.DateTimePicker
        Me.DateTimePicker2 = New System.Windows.Forms.DateTimePicker
        Me.NumericUpDown1 = New System.Windows.Forms.NumericUpDown
        Me.Label3 = New System.Windows.Forms.Label
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.Label11 = New System.Windows.Forms.Label
        Me.idObjekta = New System.Windows.Forms.NumericUpDown
        Me.RadioButton1 = New System.Windows.Forms.RadioButton
        Me.RadioButton2 = New System.Windows.Forms.RadioButton
        Me.RadioButton3 = New System.Windows.Forms.RadioButton
        Me.Button3 = New System.Windows.Forms.Button
        Me.ComboBox1 = New System.Windows.Forms.ComboBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.ComboBox2 = New System.Windows.Forms.ComboBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.ComboBox3 = New System.Windows.Forms.ComboBox
        Me.Label8 = New System.Windows.Forms.Label
        Me.ComboBox4 = New System.Windows.Forms.ComboBox
        Me.TextBox3 = New System.Windows.Forms.TextBox
        Me.TextBox4 = New System.Windows.Forms.TextBox
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label10 = New System.Windows.Forms.Label
        Me.RadioButton4 = New System.Windows.Forms.RadioButton
        Me.RadioButton5 = New System.Windows.Forms.RadioButton
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.Label14 = New System.Windows.Forms.Label
        Me.NumericUpDown4 = New System.Windows.Forms.NumericUpDown
        Me.CheckBox12 = New System.Windows.Forms.CheckBox
        Me.CheckBox13 = New System.Windows.Forms.CheckBox
        Me.TextBox5 = New System.Windows.Forms.TextBox
        Me.Label13 = New System.Windows.Forms.Label
        Me.Label12 = New System.Windows.Forms.Label
        Me.NumericUpDown3 = New System.Windows.Forms.NumericUpDown
        Me.DateTimePicker4 = New System.Windows.Forms.DateTimePicker
        Me.DateTimePicker3 = New System.Windows.Forms.DateTimePicker
        Me.CheckBox11 = New System.Windows.Forms.CheckBox
        Me.CheckBox10 = New System.Windows.Forms.CheckBox
        Me.CheckBox9 = New System.Windows.Forms.CheckBox
        Me.CheckBox8 = New System.Windows.Forms.CheckBox
        Me.CheckBox7 = New System.Windows.Forms.CheckBox
        Me.CheckBox6 = New System.Windows.Forms.CheckBox
        Me.CheckBox5 = New System.Windows.Forms.CheckBox
        Me.Button1 = New System.Windows.Forms.Button
        Me.Button4 = New System.Windows.Forms.Button
        Me.CheckBox4 = New System.Windows.Forms.CheckBox
        Me.CheckBox3 = New System.Windows.Forms.CheckBox
        Me.CheckBox2 = New System.Windows.Forms.CheckBox
        Me.CheckBox1 = New System.Windows.Forms.CheckBox
        Me.Label4 = New System.Windows.Forms.Label
        Me.NumericUpDown2 = New System.Windows.Forms.NumericUpDown
        Me.Label2 = New System.Windows.Forms.Label
        Me.ComboBox6 = New System.Windows.Forms.ComboBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.ComboBox5 = New System.Windows.Forms.ComboBox
        Me.Button5 = New System.Windows.Forms.Button
        Me.GroupBox3 = New System.Windows.Forms.GroupBox
        Me.Button6 = New System.Windows.Forms.Button
        Me.Button7 = New System.Windows.Forms.Button
        Me.Button8 = New System.Windows.Forms.Button
        Me.CheckBox14 = New System.Windows.Forms.CheckBox
        Me.TextBox6 = New System.Windows.Forms.TextBox
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        CType(Me.idObjekta, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox2.SuspendLayout()
        CType(Me.NumericUpDown4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericUpDown3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericUpDown2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'StatusBox_lbl
        '
        Me.StatusBox_lbl.AutoSize = True
        Me.StatusBox_lbl.Location = New System.Drawing.Point(227, 9)
        Me.StatusBox_lbl.Name = "StatusBox_lbl"
        Me.StatusBox_lbl.Size = New System.Drawing.Size(86, 13)
        Me.StatusBox_lbl.TabIndex = 22
        Me.StatusBox_lbl.Text = "Status konekcije"
        '
        'StatusBox_txtbx
        '
        Me.StatusBox_txtbx.BackColor = System.Drawing.SystemColors.Window
        Me.StatusBox_txtbx.Location = New System.Drawing.Point(22, 6)
        Me.StatusBox_txtbx.Name = "StatusBox_txtbx"
        Me.StatusBox_txtbx.ReadOnly = True
        Me.StatusBox_txtbx.Size = New System.Drawing.Size(202, 20)
        Me.StatusBox_txtbx.TabIndex = 21
        '
        'FormUpdateTimer
        '
        Me.FormUpdateTimer.Enabled = True
        Me.FormUpdateTimer.Interval = 6
        '
        'ANxVoltageToolTip
        '
        Me.ANxVoltageToolTip.AutomaticDelay = 20
        Me.ANxVoltageToolTip.AutoPopDelay = 20000
        Me.ANxVoltageToolTip.InitialDelay = 15
        Me.ANxVoltageToolTip.ReshowDelay = 15
        '
        'ToggleLEDToolTip
        '
        Me.ToggleLEDToolTip.AutomaticDelay = 2000
        Me.ToggleLEDToolTip.AutoPopDelay = 20000
        Me.ToggleLEDToolTip.InitialDelay = 15
        Me.ToggleLEDToolTip.ReshowDelay = 15
        '
        'PushbuttonStateTooltip
        '
        Me.PushbuttonStateTooltip.AutomaticDelay = 20
        Me.PushbuttonStateTooltip.AutoPopDelay = 20000
        Me.PushbuttonStateTooltip.InitialDelay = 15
        Me.PushbuttonStateTooltip.ReshowDelay = 15
        '
        'ReadWriteThread
        '
        Me.ReadWriteThread.WorkerReportsProgress = True
        '
        'txtBoxResponse
        '
        Me.txtBoxResponse.Location = New System.Drawing.Point(13, 535)
        Me.txtBoxResponse.Multiline = True
        Me.txtBoxResponse.Name = "txtBoxResponse"
        Me.txtBoxResponse.Size = New System.Drawing.Size(399, 83)
        Me.txtBoxResponse.TabIndex = 30
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(12, 417)
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(399, 54)
        Me.TextBox1.TabIndex = 33
        '
        'TextBox2
        '
        Me.TextBox2.Location = New System.Drawing.Point(12, 477)
        Me.TextBox2.Multiline = True
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(399, 54)
        Me.TextBox2.TabIndex = 34
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(311, 144)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 38
        Me.Button2.Text = "Piši"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'DateTimePicker1
        '
        Me.DateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DateTimePicker1.Location = New System.Drawing.Point(11, 30)
        Me.DateTimePicker1.Name = "DateTimePicker1"
        Me.DateTimePicker1.Size = New System.Drawing.Size(100, 20)
        Me.DateTimePicker1.TabIndex = 39
        '
        'DateTimePicker2
        '
        Me.DateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.DateTimePicker2.Location = New System.Drawing.Point(124, 30)
        Me.DateTimePicker2.Name = "DateTimePicker2"
        Me.DateTimePicker2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.DateTimePicker2.ShowUpDown = True
        Me.DateTimePicker2.Size = New System.Drawing.Size(64, 20)
        Me.DateTimePicker2.TabIndex = 40
        '
        'NumericUpDown1
        '
        Me.NumericUpDown1.Location = New System.Drawing.Point(205, 30)
        Me.NumericUpDown1.Maximum = New Decimal(New Integer() {20000, 0, 0, 0})
        Me.NumericUpDown1.Minimum = New Decimal(New Integer() {100, 0, 0, 0})
        Me.NumericUpDown1.Name = "NumericUpDown1"
        Me.NumericUpDown1.Size = New System.Drawing.Size(64, 20)
        Me.NumericUpDown1.TabIndex = 41
        Me.NumericUpDown1.Value = New Decimal(New Integer() {100, 0, 0, 0})
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(190, 16)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(90, 13)
        Me.Label3.TabIndex = 42
        Me.Label3.Text = "Adresa kontrolera"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.NumericUpDown1)
        Me.GroupBox1.Controls.Add(Me.DateTimePicker2)
        Me.GroupBox1.Controls.Add(Me.DateTimePicker1)
        Me.GroupBox1.Location = New System.Drawing.Point(10, 13)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(301, 60)
        Me.GroupBox1.TabIndex = 43
        Me.GroupBox1.TabStop = False
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(339, 8)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(56, 13)
        Me.Label11.TabIndex = 44
        Me.Label11.Text = "ID objekta"
        '
        'idObjekta
        '
        Me.idObjekta.Location = New System.Drawing.Point(417, 6)
        Me.idObjekta.Maximum = New Decimal(New Integer() {65000, 0, 0, 0})
        Me.idObjekta.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.idObjekta.Name = "idObjekta"
        Me.idObjekta.Size = New System.Drawing.Size(75, 20)
        Me.idObjekta.TabIndex = 43
        Me.idObjekta.Value = New Decimal(New Integer() {100, 0, 0, 0})
        '
        'RadioButton1
        '
        Me.RadioButton1.AutoSize = True
        Me.RadioButton1.Checked = True
        Me.RadioButton1.Location = New System.Drawing.Point(320, 43)
        Me.RadioButton1.Name = "RadioButton1"
        Me.RadioButton1.Size = New System.Drawing.Size(47, 17)
        Me.RadioButton1.TabIndex = 44
        Me.RadioButton1.TabStop = True
        Me.RadioButton1.Text = "Gost"
        Me.RadioButton1.UseVisualStyleBackColor = True
        '
        'RadioButton2
        '
        Me.RadioButton2.AutoSize = True
        Me.RadioButton2.Location = New System.Drawing.Point(320, 67)
        Me.RadioButton2.Name = "RadioButton2"
        Me.RadioButton2.Size = New System.Drawing.Size(67, 17)
        Me.RadioButton2.TabIndex = 45
        Me.RadioButton2.Text = "Sobarica"
        Me.RadioButton2.UseVisualStyleBackColor = True
        '
        'RadioButton3
        '
        Me.RadioButton3.AutoSize = True
        Me.RadioButton3.Location = New System.Drawing.Point(320, 19)
        Me.RadioButton3.Name = "RadioButton3"
        Me.RadioButton3.Size = New System.Drawing.Size(56, 17)
        Me.RadioButton3.TabIndex = 46
        Me.RadioButton3.Text = "Ponisti"
        Me.RadioButton3.UseVisualStyleBackColor = True
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(311, 175)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(75, 23)
        Me.Button3.TabIndex = 48
        Me.Button3.Text = "Čitaj"
        Me.ToolTip1.SetToolTip(Me.Button3, """R""")
        Me.Button3.UseVisualStyleBackColor = True
        '
        'ComboBox1
        '
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Items.AddRange(New Object() {" ", "K", "N", "7"})
        Me.ComboBox1.Location = New System.Drawing.Point(20, 135)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(103, 21)
        Me.ComboBox1.TabIndex = 51
        Me.ComboBox1.Text = " "
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(20, 119)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(56, 13)
        Me.Label5.TabIndex = 52
        Me.Label5.Text = "Logo firme"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(132, 119)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(52, 13)
        Me.Label6.TabIndex = 54
        Me.Label6.Text = "Invaliditet"
        '
        'ComboBox2
        '
        Me.ComboBox2.FormattingEnabled = True
        Me.ComboBox2.Items.AddRange(New Object() {"N", "I"})
        Me.ComboBox2.Location = New System.Drawing.Point(132, 135)
        Me.ComboBox2.Name = "ComboBox2"
        Me.ComboBox2.Size = New System.Drawing.Size(100, 21)
        Me.ComboBox2.TabIndex = 53
        Me.ComboBox2.Text = "N"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(131, 159)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(28, 13)
        Me.Label7.TabIndex = 58
        Me.Label7.Text = "Spol"
        '
        'ComboBox3
        '
        Me.ComboBox3.FormattingEnabled = True
        Me.ComboBox3.Items.AddRange(New Object() {"M", "Z"})
        Me.ComboBox3.Location = New System.Drawing.Point(132, 175)
        Me.ComboBox3.Name = "ComboBox3"
        Me.ComboBox3.Size = New System.Drawing.Size(100, 21)
        Me.ComboBox3.TabIndex = 57
        Me.ComboBox3.Text = "M"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(20, 159)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(31, 13)
        Me.Label8.TabIndex = 56
        Me.Label8.Text = "Jezik"
        '
        'ComboBox4
        '
        Me.ComboBox4.FormattingEnabled = True
        Me.ComboBox4.Items.AddRange(New Object() {"E", "B", "A", "F"})
        Me.ComboBox4.Location = New System.Drawing.Point(20, 175)
        Me.ComboBox4.Name = "ComboBox4"
        Me.ComboBox4.Size = New System.Drawing.Size(103, 21)
        Me.ComboBox4.TabIndex = 55
        Me.ComboBox4.Text = "E"
        '
        'TextBox3
        '
        Me.TextBox3.Location = New System.Drawing.Point(20, 96)
        Me.TextBox3.Name = "TextBox3"
        Me.TextBox3.Size = New System.Drawing.Size(100, 20)
        Me.TextBox3.TabIndex = 59
        '
        'TextBox4
        '
        Me.TextBox4.Location = New System.Drawing.Point(132, 96)
        Me.TextBox4.Name = "TextBox4"
        Me.TextBox4.Size = New System.Drawing.Size(100, 20)
        Me.TextBox4.TabIndex = 60
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(132, 81)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(53, 13)
        Me.Label9.TabIndex = 62
        Me.Label9.Text = "Ime gosta"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(20, 81)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(73, 13)
        Me.Label10.TabIndex = 61
        Me.Label10.Text = "Prezime gosta"
        '
        'RadioButton4
        '
        Me.RadioButton4.AutoSize = True
        Me.RadioButton4.Location = New System.Drawing.Point(320, 115)
        Me.RadioButton4.Name = "RadioButton4"
        Me.RadioButton4.Size = New System.Drawing.Size(63, 17)
        Me.RadioButton4.TabIndex = 64
        Me.RadioButton4.Text = "Serviser"
        Me.RadioButton4.UseVisualStyleBackColor = True
        '
        'RadioButton5
        '
        Me.RadioButton5.AutoSize = True
        Me.RadioButton5.Location = New System.Drawing.Point(320, 91)
        Me.RadioButton5.Name = "RadioButton5"
        Me.RadioButton5.Size = New System.Drawing.Size(67, 17)
        Me.RadioButton5.TabIndex = 63
        Me.RadioButton5.Text = "Manager"
        Me.RadioButton5.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label14)
        Me.GroupBox2.Controls.Add(Me.NumericUpDown4)
        Me.GroupBox2.Controls.Add(Me.CheckBox12)
        Me.GroupBox2.Controls.Add(Me.CheckBox13)
        Me.GroupBox2.Controls.Add(Me.TextBox5)
        Me.GroupBox2.Controls.Add(Me.Label13)
        Me.GroupBox2.Controls.Add(Me.Label12)
        Me.GroupBox2.Controls.Add(Me.NumericUpDown3)
        Me.GroupBox2.Controls.Add(Me.DateTimePicker4)
        Me.GroupBox2.Controls.Add(Me.DateTimePicker3)
        Me.GroupBox2.Controls.Add(Me.CheckBox11)
        Me.GroupBox2.Controls.Add(Me.CheckBox10)
        Me.GroupBox2.Controls.Add(Me.CheckBox9)
        Me.GroupBox2.Controls.Add(Me.CheckBox8)
        Me.GroupBox2.Controls.Add(Me.CheckBox7)
        Me.GroupBox2.Controls.Add(Me.CheckBox6)
        Me.GroupBox2.Controls.Add(Me.CheckBox5)
        Me.GroupBox2.Controls.Add(Me.Button1)
        Me.GroupBox2.Controls.Add(Me.Button4)
        Me.GroupBox2.Controls.Add(Me.CheckBox4)
        Me.GroupBox2.Controls.Add(Me.CheckBox3)
        Me.GroupBox2.Controls.Add(Me.CheckBox2)
        Me.GroupBox2.Controls.Add(Me.CheckBox1)
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.NumericUpDown2)
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Controls.Add(Me.ComboBox6)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Controls.Add(Me.ComboBox5)
        Me.GroupBox2.Location = New System.Drawing.Point(3, 254)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(408, 157)
        Me.GroupBox2.TabIndex = 66
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Sektor 2"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(323, 17)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(81, 13)
        Me.Label14.TabIndex = 100
        Me.Label14.Text = "Grupa korisnika"
        '
        'NumericUpDown4
        '
        Me.NumericUpDown4.Location = New System.Drawing.Point(326, 34)
        Me.NumericUpDown4.Maximum = New Decimal(New Integer() {99, 0, 0, 0})
        Me.NumericUpDown4.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.NumericUpDown4.Name = "NumericUpDown4"
        Me.NumericUpDown4.Size = New System.Drawing.Size(56, 20)
        Me.NumericUpDown4.TabIndex = 99
        Me.NumericUpDown4.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'CheckBox12
        '
        Me.CheckBox12.AutoSize = True
        Me.CheckBox12.Location = New System.Drawing.Point(158, 108)
        Me.CheckBox12.Name = "CheckBox12"
        Me.CheckBox12.Size = New System.Drawing.Size(40, 17)
        Me.CheckBox12.TabIndex = 98
        Me.CheckBox12.Text = "R1"
        Me.CheckBox12.UseVisualStyleBackColor = True
        '
        'CheckBox13
        '
        Me.CheckBox13.AutoSize = True
        Me.CheckBox13.Location = New System.Drawing.Point(238, 107)
        Me.CheckBox13.Name = "CheckBox13"
        Me.CheckBox13.Size = New System.Drawing.Size(40, 17)
        Me.CheckBox13.TabIndex = 97
        Me.CheckBox13.Text = "R2"
        Me.CheckBox13.UseVisualStyleBackColor = True
        '
        'TextBox5
        '
        Me.TextBox5.Location = New System.Drawing.Point(10, 77)
        Me.TextBox5.MaxLength = 4
        Me.TextBox5.Name = "TextBox5"
        Me.TextBox5.Size = New System.Drawing.Size(50, 20)
        Me.TextBox5.TabIndex = 96
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(8, 61)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(39, 13)
        Me.Label13.TabIndex = 95
        Me.Label13.Text = "id slike"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(72, 60)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(82, 13)
        Me.Label12.TabIndex = 93
        Me.Label12.Text = "Duzina boravka"
        '
        'NumericUpDown3
        '
        Me.NumericUpDown3.Location = New System.Drawing.Point(75, 76)
        Me.NumericUpDown3.Maximum = New Decimal(New Integer() {99, 0, 0, 0})
        Me.NumericUpDown3.Name = "NumericUpDown3"
        Me.NumericUpDown3.Size = New System.Drawing.Size(56, 20)
        Me.NumericUpDown3.TabIndex = 92
        Me.NumericUpDown3.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'DateTimePicker4
        '
        Me.DateTimePicker4.CustomFormat = "HH:mm"
        Me.DateTimePicker4.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DateTimePicker4.Location = New System.Drawing.Point(75, 102)
        Me.DateTimePicker4.Name = "DateTimePicker4"
        Me.DateTimePicker4.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.DateTimePicker4.ShowUpDown = True
        Me.DateTimePicker4.Size = New System.Drawing.Size(50, 20)
        Me.DateTimePicker4.TabIndex = 91
        Me.DateTimePicker4.Value = New Date(2017, 6, 9, 23, 0, 0, 0)
        '
        'DateTimePicker3
        '
        Me.DateTimePicker3.CustomFormat = "HH:mm"
        Me.DateTimePicker3.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DateTimePicker3.Location = New System.Drawing.Point(10, 102)
        Me.DateTimePicker3.Name = "DateTimePicker3"
        Me.DateTimePicker3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.DateTimePicker3.ShowUpDown = True
        Me.DateTimePicker3.Size = New System.Drawing.Size(50, 20)
        Me.DateTimePicker3.TabIndex = 90
        Me.DateTimePicker3.Value = New Date(2017, 6, 9, 20, 0, 0, 0)
        '
        'CheckBox11
        '
        Me.CheckBox11.AutoSize = True
        Me.CheckBox11.Location = New System.Drawing.Point(84, 133)
        Me.CheckBox11.Name = "CheckBox11"
        Me.CheckBox11.Size = New System.Drawing.Size(36, 17)
        Me.CheckBox11.TabIndex = 89
        Me.CheckBox11.Text = "Sr"
        Me.CheckBox11.UseVisualStyleBackColor = True
        '
        'CheckBox10
        '
        Me.CheckBox10.AutoSize = True
        Me.CheckBox10.BackColor = System.Drawing.Color.Transparent
        Me.CheckBox10.ForeColor = System.Drawing.Color.Red
        Me.CheckBox10.Location = New System.Drawing.Point(230, 133)
        Me.CheckBox10.Name = "CheckBox10"
        Me.CheckBox10.Size = New System.Drawing.Size(40, 17)
        Me.CheckBox10.TabIndex = 88
        Me.CheckBox10.Text = "Ne"
        Me.CheckBox10.UseVisualStyleBackColor = False
        '
        'CheckBox9
        '
        Me.CheckBox9.AutoSize = True
        Me.CheckBox9.BackColor = System.Drawing.Color.Transparent
        Me.CheckBox9.ForeColor = System.Drawing.Color.Red
        Me.CheckBox9.Location = New System.Drawing.Point(193, 133)
        Me.CheckBox9.Name = "CheckBox9"
        Me.CheckBox9.Size = New System.Drawing.Size(39, 17)
        Me.CheckBox9.TabIndex = 87
        Me.CheckBox9.Text = "Su"
        Me.CheckBox9.UseVisualStyleBackColor = False
        '
        'CheckBox8
        '
        Me.CheckBox8.AutoSize = True
        Me.CheckBox8.Location = New System.Drawing.Point(158, 133)
        Me.CheckBox8.Name = "CheckBox8"
        Me.CheckBox8.Size = New System.Drawing.Size(39, 17)
        Me.CheckBox8.TabIndex = 86
        Me.CheckBox8.Text = "Pe"
        Me.CheckBox8.UseVisualStyleBackColor = True
        '
        'CheckBox7
        '
        Me.CheckBox7.AutoSize = True
        Me.CheckBox7.Location = New System.Drawing.Point(121, 133)
        Me.CheckBox7.Name = "CheckBox7"
        Me.CheckBox7.Size = New System.Drawing.Size(39, 17)
        Me.CheckBox7.TabIndex = 85
        Me.CheckBox7.Text = "Ce"
        Me.CheckBox7.UseVisualStyleBackColor = True
        '
        'CheckBox6
        '
        Me.CheckBox6.AutoSize = True
        Me.CheckBox6.Location = New System.Drawing.Point(52, 133)
        Me.CheckBox6.Name = "CheckBox6"
        Me.CheckBox6.Size = New System.Drawing.Size(37, 17)
        Me.CheckBox6.TabIndex = 84
        Me.CheckBox6.Text = "Ut"
        Me.CheckBox6.UseVisualStyleBackColor = True
        '
        'CheckBox5
        '
        Me.CheckBox5.AutoSize = True
        Me.CheckBox5.Location = New System.Drawing.Point(15, 133)
        Me.CheckBox5.Name = "CheckBox5"
        Me.CheckBox5.Size = New System.Drawing.Size(39, 17)
        Me.CheckBox5.TabIndex = 83
        Me.CheckBox5.Text = "Po"
        Me.CheckBox5.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(319, 128)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 82
        Me.Button1.Text = "Čitaj"
        Me.ToolTip1.SetToolTip(Me.Button1, """C""")
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button4
        '
        Me.Button4.Location = New System.Drawing.Point(319, 98)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(75, 23)
        Me.Button4.TabIndex = 81
        Me.Button4.Text = "Piši"
        Me.Button4.UseVisualStyleBackColor = True
        '
        'CheckBox4
        '
        Me.CheckBox4.AutoSize = True
        Me.CheckBox4.Location = New System.Drawing.Point(158, 60)
        Me.CheckBox4.Name = "CheckBox4"
        Me.CheckBox4.Size = New System.Drawing.Size(63, 17)
        Me.CheckBox4.TabIndex = 80
        Me.CheckBox4.Text = "SAUNA"
        Me.CheckBox4.UseVisualStyleBackColor = True
        '
        'CheckBox3
        '
        Me.CheckBox3.AutoSize = True
        Me.CheckBox3.Location = New System.Drawing.Point(158, 85)
        Me.CheckBox3.Name = "CheckBox3"
        Me.CheckBox3.Size = New System.Drawing.Size(74, 17)
        Me.CheckBox3.TabIndex = 79
        Me.CheckBox3.Text = "PARKING"
        Me.CheckBox3.UseVisualStyleBackColor = True
        '
        'CheckBox2
        '
        Me.CheckBox2.AutoSize = True
        Me.CheckBox2.Location = New System.Drawing.Point(238, 84)
        Me.CheckBox2.Name = "CheckBox2"
        Me.CheckBox2.Size = New System.Drawing.Size(75, 17)
        Me.CheckBox2.TabIndex = 78
        Me.CheckBox2.Text = "IGRAONA"
        Me.CheckBox2.UseVisualStyleBackColor = True
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.Checked = True
        Me.CheckBox1.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckBox1.Location = New System.Drawing.Point(238, 60)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(62, 17)
        Me.CheckBox1.TabIndex = 77
        Me.CheckBox1.Text = "BAZEN"
        Me.CheckBox1.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(230, 17)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(70, 13)
        Me.Label4.TabIndex = 76
        Me.Label4.Text = "Broj korisnika"
        '
        'NumericUpDown2
        '
        Me.NumericUpDown2.Location = New System.Drawing.Point(233, 34)
        Me.NumericUpDown2.Maximum = New Decimal(New Integer() {99, 0, 0, 0})
        Me.NumericUpDown2.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.NumericUpDown2.Name = "NumericUpDown2"
        Me.NumericUpDown2.Size = New System.Drawing.Size(56, 20)
        Me.NumericUpDown2.TabIndex = 75
        Me.NumericUpDown2.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(122, 17)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(66, 13)
        Me.Label2.TabIndex = 74
        Me.Label2.Text = "Tip pretplate"
        '
        'ComboBox6
        '
        Me.ComboBox6.FormattingEnabled = True
        Me.ComboBox6.Items.AddRange(New Object() {"Jednokratna", "vremenska"})
        Me.ComboBox6.Location = New System.Drawing.Point(122, 33)
        Me.ComboBox6.Name = "ComboBox6"
        Me.ComboBox6.Size = New System.Drawing.Size(102, 21)
        Me.ComboBox6.TabIndex = 73
        Me.ComboBox6.Text = "Jednokratna"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(10, 17)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(74, 13)
        Me.Label1.TabIndex = 72
        Me.Label1.Text = "Tip RFID taga"
        '
        'ComboBox5
        '
        Me.ComboBox5.FormattingEnabled = True
        Me.ComboBox5.Items.AddRange(New Object() {"Kartica", "Narukvica", "Privjesak"})
        Me.ComboBox5.Location = New System.Drawing.Point(11, 32)
        Me.ComboBox5.Name = "ComboBox5"
        Me.ComboBox5.Size = New System.Drawing.Size(87, 21)
        Me.ComboBox5.TabIndex = 71
        Me.ComboBox5.Text = "Kartica"
        '
        'Button5
        '
        Me.Button5.Location = New System.Drawing.Point(417, 224)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(75, 23)
        Me.Button5.TabIndex = 82
        Me.Button5.Text = "Piši 1+2"
        Me.Button5.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.RadioButton4)
        Me.GroupBox3.Controls.Add(Me.RadioButton5)
        Me.GroupBox3.Controls.Add(Me.Label9)
        Me.GroupBox3.Controls.Add(Me.Label10)
        Me.GroupBox3.Controls.Add(Me.TextBox4)
        Me.GroupBox3.Controls.Add(Me.TextBox3)
        Me.GroupBox3.Controls.Add(Me.Label7)
        Me.GroupBox3.Controls.Add(Me.ComboBox3)
        Me.GroupBox3.Controls.Add(Me.Label8)
        Me.GroupBox3.Controls.Add(Me.Button3)
        Me.GroupBox3.Controls.Add(Me.ComboBox4)
        Me.GroupBox3.Controls.Add(Me.Label6)
        Me.GroupBox3.Controls.Add(Me.ComboBox2)
        Me.GroupBox3.Controls.Add(Me.Label5)
        Me.GroupBox3.Controls.Add(Me.ComboBox1)
        Me.GroupBox3.Controls.Add(Me.RadioButton3)
        Me.GroupBox3.Controls.Add(Me.RadioButton2)
        Me.GroupBox3.Controls.Add(Me.RadioButton1)
        Me.GroupBox3.Controls.Add(Me.GroupBox1)
        Me.GroupBox3.Controls.Add(Me.Button2)
        Me.GroupBox3.Location = New System.Drawing.Point(2, 35)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(410, 212)
        Me.GroupBox3.TabIndex = 83
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Sektor 1"
        '
        'Button6
        '
        Me.Button6.Location = New System.Drawing.Point(417, 266)
        Me.Button6.Name = "Button6"
        Me.Button6.Size = New System.Drawing.Size(75, 23)
        Me.Button6.TabIndex = 84
        Me.Button6.Text = "Čitaj 1+2"
        Me.Button6.UseVisualStyleBackColor = True
        '
        'Button7
        '
        Me.Button7.Location = New System.Drawing.Point(417, 434)
        Me.Button7.Name = "Button7"
        Me.Button7.Size = New System.Drawing.Size(75, 37)
        Me.Button7.TabIndex = 85
        Me.Button7.Text = "Samo Čitanje"
        Me.ToolTip1.SetToolTip(Me.Button7, """K""")
        Me.Button7.UseVisualStyleBackColor = True
        '
        'Button8
        '
        Me.Button8.Location = New System.Drawing.Point(417, 477)
        Me.Button8.Name = "Button8"
        Me.Button8.Size = New System.Drawing.Size(75, 37)
        Me.Button8.TabIndex = 86
        Me.Button8.Text = "Stop"
        Me.ToolTip1.SetToolTip(Me.Button8, """S""")
        Me.Button8.UseVisualStyleBackColor = True
        '
        'CheckBox14
        '
        Me.CheckBox14.AutoSize = True
        Me.CheckBox14.Location = New System.Drawing.Point(430, 48)
        Me.CheckBox14.Name = "CheckBox14"
        Me.CheckBox14.Size = New System.Drawing.Size(61, 17)
        Me.CheckBox14.TabIndex = 87
        Me.CheckBox14.Text = "Update"
        Me.ToolTip1.SetToolTip(Me.CheckBox14, "Checked ""U""  Unchecked ""S""")
        Me.CheckBox14.UseVisualStyleBackColor = True
        '
        'TextBox6
        '
        Me.TextBox6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox6.Location = New System.Drawing.Point(586, 6)
        Me.TextBox6.Multiline = True
        Me.TextBox6.Name = "TextBox6"
        Me.TextBox6.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TextBox6.Size = New System.Drawing.Size(231, 612)
        Me.TextBox6.TabIndex = 88
        Me.TextBox6.Text = resources.GetString("TextBox6.Text")
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(505, 627)
        Me.Controls.Add(Me.TextBox6)
        Me.Controls.Add(Me.CheckBox14)
        Me.Controls.Add(Me.Button8)
        Me.Controls.Add(Me.Button7)
        Me.Controls.Add(Me.Button6)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.idObjekta)
        Me.Controls.Add(Me.Button5)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.TextBox2)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.txtBoxResponse)
        Me.Controls.Add(Me.StatusBox_lbl)
        Me.Controls.Add(Me.StatusBox_txtbx)
        Me.Name = "Form1"
        Me.Text = "MIFARE RFID Programator"
        CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.idObjekta, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        CType(Me.NumericUpDown4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericUpDown3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericUpDown2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents StatusBox_lbl As System.Windows.Forms.Label
    Private WithEvents StatusBox_txtbx As System.Windows.Forms.TextBox
    Private WithEvents FormUpdateTimer As System.Windows.Forms.Timer
    Private WithEvents ToggleLEDToolTip As System.Windows.Forms.ToolTip
    Private WithEvents PushbuttonStateTooltip As System.Windows.Forms.ToolTip
    Private WithEvents ReadWriteThread As System.ComponentModel.BackgroundWorker
    Private WithEvents ANxVoltageToolTip As System.Windows.Forms.ToolTip
    Private WithEvents txtBoxResponse As System.Windows.Forms.TextBox
    Private WithEvents TextBox1 As System.Windows.Forms.TextBox
    Private WithEvents TextBox2 As System.Windows.Forms.TextBox
    Private WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents DateTimePicker1 As System.Windows.Forms.DateTimePicker
    Private WithEvents DateTimePicker2 As System.Windows.Forms.DateTimePicker
    Friend WithEvents NumericUpDown1 As System.Windows.Forms.NumericUpDown
    Private WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents RadioButton1 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton2 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton3 As System.Windows.Forms.RadioButton
    Private WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents ComboBox1 As System.Windows.Forms.ComboBox
    Private WithEvents Label5 As System.Windows.Forms.Label
    Private WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents ComboBox2 As System.Windows.Forms.ComboBox
    Private WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents ComboBox3 As System.Windows.Forms.ComboBox
    Private WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents ComboBox4 As System.Windows.Forms.ComboBox
    Friend WithEvents TextBox3 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox4 As System.Windows.Forms.TextBox
    Private WithEvents Label9 As System.Windows.Forms.Label
    Private WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents RadioButton4 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton5 As System.Windows.Forms.RadioButton
    Private WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents idObjekta As System.Windows.Forms.NumericUpDown
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents CheckBox3 As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox2 As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents NumericUpDown2 As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents ComboBox6 As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ComboBox5 As System.Windows.Forms.ComboBox
    Friend WithEvents CheckBox4 As System.Windows.Forms.CheckBox
    Private WithEvents Button1 As System.Windows.Forms.Button
    Private WithEvents Button4 As System.Windows.Forms.Button
    Private WithEvents Button5 As System.Windows.Forms.Button
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Private WithEvents Button6 As System.Windows.Forms.Button
    Private WithEvents Button7 As System.Windows.Forms.Button
    Friend WithEvents CheckBox11 As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox10 As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox9 As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox8 As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox7 As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox6 As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox5 As System.Windows.Forms.CheckBox
    Private WithEvents DateTimePicker3 As System.Windows.Forms.DateTimePicker
    Private WithEvents Button8 As System.Windows.Forms.Button
    Private WithEvents DateTimePicker4 As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents NumericUpDown3 As System.Windows.Forms.NumericUpDown
    Friend WithEvents TextBox5 As System.Windows.Forms.TextBox
    Friend WithEvents CheckBox12 As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox13 As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox14 As System.Windows.Forms.CheckBox
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents NumericUpDown4 As System.Windows.Forms.NumericUpDown
    Private WithEvents TextBox6 As System.Windows.Forms.TextBox
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip

End Class
