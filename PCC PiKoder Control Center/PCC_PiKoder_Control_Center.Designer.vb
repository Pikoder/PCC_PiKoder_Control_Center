<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PCC_PiKoder_Control_Center
    Inherits System.Windows.Forms.Form
    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PCC_PiKoder_Control_Center))
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.AvailableCOMPorts = New System.Windows.Forms.ListBox()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Led2 = New LED()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.GroupBox12 = New System.Windows.Forms.GroupBox()
        Me.NumericUpDown1 = New System.Windows.Forms.NumericUpDown()
        Me.TimeOut = New System.Windows.Forms.NumericUpDown()
        Me.GroupBox8 = New System.Windows.Forms.GroupBox()
        Me.miniSSCOffset = New System.Windows.Forms.NumericUpDown()
        Me.GroupBox7 = New System.Windows.Forms.GroupBox()
        Me.saveButton = New System.Windows.Forms.Button()
        Me.strSSC_Firmware = New System.Windows.Forms.TextBox()
        Me.frmOuter = New System.Windows.Forms.GroupBox()
        Me.GroupBox9 = New System.Windows.Forms.GroupBox()
        Me.strCH_8_Min = New System.Windows.Forms.NumericUpDown()
        Me.strCH_7_Min = New System.Windows.Forms.NumericUpDown()
        Me.strCH_6_Min = New System.Windows.Forms.NumericUpDown()
        Me.strCH_5_Min = New System.Windows.Forms.NumericUpDown()
        Me.strCH_4_Min = New System.Windows.Forms.NumericUpDown()
        Me.strCH_3_Min = New System.Windows.Forms.NumericUpDown()
        Me.strCH_2_Min = New System.Windows.Forms.NumericUpDown()
        Me.strCH_1_Min = New System.Windows.Forms.NumericUpDown()
        Me.GroupBox10 = New System.Windows.Forms.GroupBox()
        Me.strCH_8_Max = New System.Windows.Forms.NumericUpDown()
        Me.strCH_7_Max = New System.Windows.Forms.NumericUpDown()
        Me.strCH_6_Max = New System.Windows.Forms.NumericUpDown()
        Me.strCH_5_Max = New System.Windows.Forms.NumericUpDown()
        Me.strCH_4_Max = New System.Windows.Forms.NumericUpDown()
        Me.strCH_3_Max = New System.Windows.Forms.NumericUpDown()
        Me.strCH_2_Max = New System.Windows.Forms.NumericUpDown()
        Me.strCH_1_Max = New System.Windows.Forms.NumericUpDown()
        Me.GroupBox11 = New System.Windows.Forms.GroupBox()
        Me.strCH_8_Neutral = New System.Windows.Forms.NumericUpDown()
        Me.strCH_7_Neutral = New System.Windows.Forms.NumericUpDown()
        Me.strCH_6_Neutral = New System.Windows.Forms.NumericUpDown()
        Me.strCH_5_Neutral = New System.Windows.Forms.NumericUpDown()
        Me.strCH_4_Neutral = New System.Windows.Forms.NumericUpDown()
        Me.strCH_3_Neutral = New System.Windows.Forms.NumericUpDown()
        Me.strCH_2_Neutral = New System.Windows.Forms.NumericUpDown()
        Me.strCH_1_Neutral = New System.Windows.Forms.NumericUpDown()
        Me._Frame2_0 = New System.Windows.Forms.GroupBox()
        Me.ch8_HScrollBar = New System.Windows.Forms.HScrollBar()
        Me.ch7_HScrollBar = New System.Windows.Forms.HScrollBar()
        Me.ch6_HScrollBar = New System.Windows.Forms.HScrollBar()
        Me.ch5_HScrollBar = New System.Windows.Forms.HScrollBar()
        Me.ch4_HScrollBar = New System.Windows.Forms.HScrollBar()
        Me.ch3_HScrollBar = New System.Windows.Forms.HScrollBar()
        Me.ch2_HScrollBar = New System.Windows.Forms.HScrollBar()
        Me.ch1_HScrollBar = New System.Windows.Forms.HScrollBar()
        Me.strCH_8_Current = New System.Windows.Forms.TextBox()
        Me.strCH_7_Current = New System.Windows.Forms.TextBox()
        Me.strCH_6_Current = New System.Windows.Forms.TextBox()
        Me.strCH_5_Current = New System.Windows.Forms.TextBox()
        Me.strCH_4_Current = New System.Windows.Forms.TextBox()
        Me.strCH_3_Current = New System.Windows.Forms.TextBox()
        Me.strCH_2_Current = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.strCH_1_Current = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TypeId = New System.Windows.Forms.TextBox()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.GroupBox17 = New System.Windows.Forms.GroupBox()
        Me.PPM_Mode = New System.Windows.Forms.ListBox()
        Me.GroupBox13 = New System.Windows.Forms.GroupBox()
        Me.GroupBox15 = New System.Windows.Forms.GroupBox()
        Me.GroupBox16 = New System.Windows.Forms.GroupBox()
        Me.NumericUpDown4 = New System.Windows.Forms.NumericUpDown()
        Me.NumericUpDown5 = New System.Windows.Forms.NumericUpDown()
        Me.GroupBox14 = New System.Windows.Forms.GroupBox()
        Me.NumericUpDown2 = New System.Windows.Forms.NumericUpDown()
        Me.PPM_Channels = New System.Windows.Forms.NumericUpDown()
        Me.tHeartBeat = New System.Windows.Forms.Timer(Me.components)
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox12.SuspendLayout()
        CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TimeOut, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox8.SuspendLayout()
        CType(Me.miniSSCOffset, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox7.SuspendLayout()
        Me.frmOuter.SuspendLayout()
        Me.GroupBox9.SuspendLayout()
        CType(Me.strCH_8_Min, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.strCH_7_Min, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.strCH_6_Min, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.strCH_5_Min, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.strCH_4_Min, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.strCH_3_Min, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.strCH_2_Min, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.strCH_1_Min, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox10.SuspendLayout()
        CType(Me.strCH_8_Max, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.strCH_7_Max, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.strCH_6_Max, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.strCH_5_Max, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.strCH_4_Max, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.strCH_3_Max, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.strCH_2_Max, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.strCH_1_Max, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox11.SuspendLayout()
        CType(Me.strCH_8_Neutral, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.strCH_7_Neutral, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.strCH_6_Neutral, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.strCH_5_Neutral, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.strCH_4_Neutral, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.strCH_3_Neutral, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.strCH_2_Neutral, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.strCH_1_Neutral, System.ComponentModel.ISupportInitialize).BeginInit()
        Me._Frame2_0.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.GroupBox17.SuspendLayout()
        Me.GroupBox13.SuspendLayout()
        Me.GroupBox15.SuspendLayout()
        Me.GroupBox16.SuspendLayout()
        CType(Me.NumericUpDown4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericUpDown5, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox14.SuspendLayout()
        CType(Me.NumericUpDown2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PPM_Channels, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.AutoSize = True
        Me.GroupBox1.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.AvailableCOMPorts)
        Me.GroupBox1.Font = New System.Drawing.Font("Arial", 11.0!, System.Drawing.FontStyle.Bold)
        Me.GroupBox1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBox1.Location = New System.Drawing.Point(10, 20)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBox1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.GroupBox1.Size = New System.Drawing.Size(259, 74)
        Me.GroupBox1.TabIndex = 21
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "COM"
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(111, 21)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(136, 32)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Select COM port and double click to connect"
        '
        'AvailableCOMPorts
        '
        Me.AvailableCOMPorts.FormattingEnabled = True
        Me.AvailableCOMPorts.ItemHeight = 18
        Me.AvailableCOMPorts.Location = New System.Drawing.Point(11, 29)
        Me.AvailableCOMPorts.Name = "AvailableCOMPorts"
        Me.AvailableCOMPorts.Size = New System.Drawing.Size(87, 22)
        Me.AvailableCOMPorts.TabIndex = 0
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(12, 22)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(279, 24)
        Me.TextBox1.TabIndex = 22
        '
        'GroupBox2
        '
        Me.GroupBox2.AutoSize = True
        Me.GroupBox2.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox2.Controls.Add(Me.Led2)
        Me.GroupBox2.Controls.Add(Me.TextBox1)
        Me.GroupBox2.Font = New System.Drawing.Font("Arial", 11.0!, System.Drawing.FontStyle.Bold)
        Me.GroupBox2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBox2.Location = New System.Drawing.Point(278, 20)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBox2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.GroupBox2.Size = New System.Drawing.Size(360, 75)
        Me.GroupBox2.TabIndex = 22
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Status"
        '
        'Led2
        '
        Me.Led2.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.Led2.Blink = False
        Me.Led2.Color = LED.LEDColorSelection.LED_Red
        Me.Led2.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Led2.Interval = CType(0, Short)
        Me.Led2.Location = New System.Drawing.Point(311, 18)
        Me.Led2.Name = "Led2"
        Me.Led2.Size = New System.Drawing.Size(33, 35)
        Me.Led2.State = False
        Me.Led2.TabIndex = 23
        '
        'GroupBox3
        '
        Me.GroupBox3.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox3.Controls.Add(Me.GroupBox1)
        Me.GroupBox3.Controls.Add(Me.GroupBox2)
        Me.GroupBox3.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox3.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBox3.Location = New System.Drawing.Point(14, 10)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBox3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.GroupBox3.Size = New System.Drawing.Size(655, 107)
        Me.GroupBox3.TabIndex = 22
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Connection to PiKoder"
        '
        'GroupBox4
        '
        Me.GroupBox4.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox4.Controls.Add(Me.GroupBox12)
        Me.GroupBox4.Controls.Add(Me.TimeOut)
        Me.GroupBox4.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox4.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBox4.Location = New System.Drawing.Point(11, 91)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBox4.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.GroupBox4.Size = New System.Drawing.Size(145, 63)
        Me.GroupBox4.TabIndex = 26
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "TimeOut [0.1s]"
        '
        'GroupBox12
        '
        Me.GroupBox12.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox12.Controls.Add(Me.NumericUpDown1)
        Me.GroupBox12.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox12.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBox12.Location = New System.Drawing.Point(1, 71)
        Me.GroupBox12.Name = "GroupBox12"
        Me.GroupBox12.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBox12.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.GroupBox12.Size = New System.Drawing.Size(145, 63)
        Me.GroupBox12.TabIndex = 27
        Me.GroupBox12.TabStop = False
        Me.GroupBox12.Text = "Zero Offset"
        '
        'NumericUpDown1
        '
        Me.NumericUpDown1.ForeColor = System.Drawing.Color.White
        Me.NumericUpDown1.Location = New System.Drawing.Point(28, 25)
        Me.NumericUpDown1.Maximum = New Decimal(New Integer() {99, 0, 0, 0})
        Me.NumericUpDown1.Name = "NumericUpDown1"
        Me.NumericUpDown1.Size = New System.Drawing.Size(73, 26)
        Me.NumericUpDown1.TabIndex = 26
        Me.NumericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'TimeOut
        '
        Me.TimeOut.ForeColor = System.Drawing.Color.White
        Me.TimeOut.Location = New System.Drawing.Point(13, 25)
        Me.TimeOut.Maximum = New Decimal(New Integer() {999, 0, 0, 0})
        Me.TimeOut.Name = "TimeOut"
        Me.TimeOut.Size = New System.Drawing.Size(73, 26)
        Me.TimeOut.TabIndex = 26
        Me.TimeOut.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'GroupBox8
        '
        Me.GroupBox8.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox8.Controls.Add(Me.miniSSCOffset)
        Me.GroupBox8.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox8.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBox8.Location = New System.Drawing.Point(11, 27)
        Me.GroupBox8.Name = "GroupBox8"
        Me.GroupBox8.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBox8.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.GroupBox8.Size = New System.Drawing.Size(145, 58)
        Me.GroupBox8.TabIndex = 28
        Me.GroupBox8.TabStop = False
        Me.GroupBox8.Text = "miniSSC Offset"
        '
        'miniSSCOffset
        '
        Me.miniSSCOffset.ForeColor = System.Drawing.Color.White
        Me.miniSSCOffset.Location = New System.Drawing.Point(12, 23)
        Me.miniSSCOffset.Maximum = New Decimal(New Integer() {248, 0, 0, 0})
        Me.miniSSCOffset.Name = "miniSSCOffset"
        Me.miniSSCOffset.Size = New System.Drawing.Size(72, 26)
        Me.miniSSCOffset.TabIndex = 26
        Me.miniSSCOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'GroupBox7
        '
        Me.GroupBox7.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox7.Controls.Add(Me.saveButton)
        Me.GroupBox7.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox7.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBox7.Location = New System.Drawing.Point(675, 440)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBox7.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.GroupBox7.Size = New System.Drawing.Size(206, 81)
        Me.GroupBox7.TabIndex = 24
        Me.GroupBox7.TabStop = False
        Me.GroupBox7.Text = "Parameter"
        '
        'saveButton
        '
        Me.saveButton.AutoSize = True
        Me.saveButton.BackColor = System.Drawing.SystemColors.Control
        Me.saveButton.Cursor = System.Windows.Forms.Cursors.Default
        Me.saveButton.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.saveButton.ForeColor = System.Drawing.SystemColors.ControlText
        Me.saveButton.Location = New System.Drawing.Point(12, 22)
        Me.saveButton.Name = "saveButton"
        Me.saveButton.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.saveButton.Size = New System.Drawing.Size(177, 49)
        Me.saveButton.TabIndex = 0
        Me.saveButton.Text = "Save Parameters"
        Me.saveButton.UseVisualStyleBackColor = False
        '
        'strSSC_Firmware
        '
        Me.strSSC_Firmware.AcceptsReturn = True
        Me.strSSC_Firmware.BackColor = System.Drawing.SystemColors.Window
        Me.strSSC_Firmware.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.strSSC_Firmware.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.strSSC_Firmware.ForeColor = System.Drawing.SystemColors.WindowText
        Me.strSSC_Firmware.Location = New System.Drawing.Point(107, 66)
        Me.strSSC_Firmware.Margin = New System.Windows.Forms.Padding(2)
        Me.strSSC_Firmware.MaxLength = 4
        Me.strSSC_Firmware.Name = "strSSC_Firmware"
        Me.strSSC_Firmware.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.strSSC_Firmware.Size = New System.Drawing.Size(80, 26)
        Me.strSSC_Firmware.TabIndex = 18
        Me.strSSC_Firmware.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'frmOuter
        '
        Me.frmOuter.BackColor = System.Drawing.SystemColors.Control
        Me.frmOuter.Controls.Add(Me.GroupBox9)
        Me.frmOuter.Controls.Add(Me.GroupBox10)
        Me.frmOuter.Controls.Add(Me.GroupBox11)
        Me.frmOuter.Controls.Add(Me._Frame2_0)
        Me.frmOuter.Font = New System.Drawing.Font("Arial", 13.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.frmOuter.ForeColor = System.Drawing.SystemColors.ControlText
        Me.frmOuter.Location = New System.Drawing.Point(14, 124)
        Me.frmOuter.Name = "frmOuter"
        Me.frmOuter.Padding = New System.Windows.Forms.Padding(0)
        Me.frmOuter.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.frmOuter.Size = New System.Drawing.Size(655, 397)
        Me.frmOuter.TabIndex = 23
        Me.frmOuter.TabStop = False
        Me.frmOuter.Text = "Channels [µs]"
        '
        'GroupBox9
        '
        Me.GroupBox9.AutoSize = True
        Me.GroupBox9.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox9.Controls.Add(Me.strCH_8_Min)
        Me.GroupBox9.Controls.Add(Me.strCH_7_Min)
        Me.GroupBox9.Controls.Add(Me.strCH_6_Min)
        Me.GroupBox9.Controls.Add(Me.strCH_5_Min)
        Me.GroupBox9.Controls.Add(Me.strCH_4_Min)
        Me.GroupBox9.Controls.Add(Me.strCH_3_Min)
        Me.GroupBox9.Controls.Add(Me.strCH_2_Min)
        Me.GroupBox9.Controls.Add(Me.strCH_1_Min)
        Me.GroupBox9.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox9.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBox9.Location = New System.Drawing.Point(380, 24)
        Me.GroupBox9.Name = "GroupBox9"
        Me.GroupBox9.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBox9.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.GroupBox9.Size = New System.Drawing.Size(80, 365)
        Me.GroupBox9.TabIndex = 27
        Me.GroupBox9.TabStop = False
        Me.GroupBox9.Text = "min."
        '
        'strCH_8_Min
        '
        Me.strCH_8_Min.AutoSize = True
        Me.strCH_8_Min.ForeColor = System.Drawing.Color.White
        Me.strCH_8_Min.Location = New System.Drawing.Point(9, 317)
        Me.strCH_8_Min.Maximum = New Decimal(New Integer() {2500, 0, 0, 0})
        Me.strCH_8_Min.Name = "strCH_8_Min"
        Me.strCH_8_Min.Size = New System.Drawing.Size(61, 26)
        Me.strCH_8_Min.TabIndex = 25
        Me.strCH_8_Min.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.strCH_8_Min.Value = New Decimal(New Integer() {500, 0, 0, 0})
        '
        'strCH_7_Min
        '
        Me.strCH_7_Min.AutoSize = True
        Me.strCH_7_Min.ForeColor = System.Drawing.Color.White
        Me.strCH_7_Min.Location = New System.Drawing.Point(9, 276)
        Me.strCH_7_Min.Maximum = New Decimal(New Integer() {2500, 0, 0, 0})
        Me.strCH_7_Min.Name = "strCH_7_Min"
        Me.strCH_7_Min.Size = New System.Drawing.Size(61, 26)
        Me.strCH_7_Min.TabIndex = 24
        Me.strCH_7_Min.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.strCH_7_Min.Value = New Decimal(New Integer() {500, 0, 0, 0})
        '
        'strCH_6_Min
        '
        Me.strCH_6_Min.AutoSize = True
        Me.strCH_6_Min.ForeColor = System.Drawing.Color.White
        Me.strCH_6_Min.Location = New System.Drawing.Point(9, 238)
        Me.strCH_6_Min.Maximum = New Decimal(New Integer() {2500, 0, 0, 0})
        Me.strCH_6_Min.Name = "strCH_6_Min"
        Me.strCH_6_Min.Size = New System.Drawing.Size(61, 26)
        Me.strCH_6_Min.TabIndex = 23
        Me.strCH_6_Min.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.strCH_6_Min.Value = New Decimal(New Integer() {500, 0, 0, 0})
        '
        'strCH_5_Min
        '
        Me.strCH_5_Min.AutoSize = True
        Me.strCH_5_Min.ForeColor = System.Drawing.Color.White
        Me.strCH_5_Min.Location = New System.Drawing.Point(9, 195)
        Me.strCH_5_Min.Maximum = New Decimal(New Integer() {2500, 0, 0, 0})
        Me.strCH_5_Min.Name = "strCH_5_Min"
        Me.strCH_5_Min.Size = New System.Drawing.Size(61, 26)
        Me.strCH_5_Min.TabIndex = 22
        Me.strCH_5_Min.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.strCH_5_Min.Value = New Decimal(New Integer() {500, 0, 0, 0})
        '
        'strCH_4_Min
        '
        Me.strCH_4_Min.AutoSize = True
        Me.strCH_4_Min.ForeColor = System.Drawing.Color.White
        Me.strCH_4_Min.Location = New System.Drawing.Point(9, 155)
        Me.strCH_4_Min.Maximum = New Decimal(New Integer() {2500, 0, 0, 0})
        Me.strCH_4_Min.Name = "strCH_4_Min"
        Me.strCH_4_Min.Size = New System.Drawing.Size(61, 26)
        Me.strCH_4_Min.TabIndex = 21
        Me.strCH_4_Min.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.strCH_4_Min.Value = New Decimal(New Integer() {500, 0, 0, 0})
        '
        'strCH_3_Min
        '
        Me.strCH_3_Min.AutoSize = True
        Me.strCH_3_Min.ForeColor = System.Drawing.Color.White
        Me.strCH_3_Min.Location = New System.Drawing.Point(9, 111)
        Me.strCH_3_Min.Maximum = New Decimal(New Integer() {2500, 0, 0, 0})
        Me.strCH_3_Min.Name = "strCH_3_Min"
        Me.strCH_3_Min.Size = New System.Drawing.Size(61, 26)
        Me.strCH_3_Min.TabIndex = 20
        Me.strCH_3_Min.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.strCH_3_Min.Value = New Decimal(New Integer() {500, 0, 0, 0})
        '
        'strCH_2_Min
        '
        Me.strCH_2_Min.AutoSize = True
        Me.strCH_2_Min.ForeColor = System.Drawing.Color.White
        Me.strCH_2_Min.Location = New System.Drawing.Point(9, 70)
        Me.strCH_2_Min.Maximum = New Decimal(New Integer() {2500, 0, 0, 0})
        Me.strCH_2_Min.Name = "strCH_2_Min"
        Me.strCH_2_Min.Size = New System.Drawing.Size(61, 26)
        Me.strCH_2_Min.TabIndex = 19
        Me.strCH_2_Min.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.strCH_2_Min.Value = New Decimal(New Integer() {500, 0, 0, 0})
        '
        'strCH_1_Min
        '
        Me.strCH_1_Min.AutoSize = True
        Me.strCH_1_Min.ForeColor = System.Drawing.Color.White
        Me.strCH_1_Min.Location = New System.Drawing.Point(9, 29)
        Me.strCH_1_Min.Maximum = New Decimal(New Integer() {2500, 0, 0, 0})
        Me.strCH_1_Min.Name = "strCH_1_Min"
        Me.strCH_1_Min.Size = New System.Drawing.Size(61, 26)
        Me.strCH_1_Min.TabIndex = 18
        Me.strCH_1_Min.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.strCH_1_Min.Value = New Decimal(New Integer() {500, 0, 0, 0})
        '
        'GroupBox10
        '
        Me.GroupBox10.AutoSize = True
        Me.GroupBox10.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox10.Controls.Add(Me.strCH_8_Max)
        Me.GroupBox10.Controls.Add(Me.strCH_7_Max)
        Me.GroupBox10.Controls.Add(Me.strCH_6_Max)
        Me.GroupBox10.Controls.Add(Me.strCH_5_Max)
        Me.GroupBox10.Controls.Add(Me.strCH_4_Max)
        Me.GroupBox10.Controls.Add(Me.strCH_3_Max)
        Me.GroupBox10.Controls.Add(Me.strCH_2_Max)
        Me.GroupBox10.Controls.Add(Me.strCH_1_Max)
        Me.GroupBox10.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox10.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBox10.Location = New System.Drawing.Point(469, 24)
        Me.GroupBox10.Name = "GroupBox10"
        Me.GroupBox10.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBox10.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.GroupBox10.Size = New System.Drawing.Size(80, 365)
        Me.GroupBox10.TabIndex = 26
        Me.GroupBox10.TabStop = False
        Me.GroupBox10.Text = "max."
        '
        'strCH_8_Max
        '
        Me.strCH_8_Max.ForeColor = System.Drawing.Color.White
        Me.strCH_8_Max.Location = New System.Drawing.Point(9, 317)
        Me.strCH_8_Max.Maximum = New Decimal(New Integer() {2500, 0, 0, 0})
        Me.strCH_8_Max.Minimum = New Decimal(New Integer() {500, 0, 0, 0})
        Me.strCH_8_Max.Name = "strCH_8_Max"
        Me.strCH_8_Max.Size = New System.Drawing.Size(64, 26)
        Me.strCH_8_Max.TabIndex = 25
        Me.strCH_8_Max.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.strCH_8_Max.Value = New Decimal(New Integer() {500, 0, 0, 0})
        '
        'strCH_7_Max
        '
        Me.strCH_7_Max.ForeColor = System.Drawing.Color.White
        Me.strCH_7_Max.Location = New System.Drawing.Point(9, 276)
        Me.strCH_7_Max.Maximum = New Decimal(New Integer() {2500, 0, 0, 0})
        Me.strCH_7_Max.Minimum = New Decimal(New Integer() {500, 0, 0, 0})
        Me.strCH_7_Max.Name = "strCH_7_Max"
        Me.strCH_7_Max.Size = New System.Drawing.Size(64, 26)
        Me.strCH_7_Max.TabIndex = 24
        Me.strCH_7_Max.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.strCH_7_Max.Value = New Decimal(New Integer() {500, 0, 0, 0})
        '
        'strCH_6_Max
        '
        Me.strCH_6_Max.ForeColor = System.Drawing.Color.White
        Me.strCH_6_Max.Location = New System.Drawing.Point(9, 238)
        Me.strCH_6_Max.Maximum = New Decimal(New Integer() {2500, 0, 0, 0})
        Me.strCH_6_Max.Minimum = New Decimal(New Integer() {500, 0, 0, 0})
        Me.strCH_6_Max.Name = "strCH_6_Max"
        Me.strCH_6_Max.Size = New System.Drawing.Size(64, 26)
        Me.strCH_6_Max.TabIndex = 23
        Me.strCH_6_Max.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.strCH_6_Max.Value = New Decimal(New Integer() {500, 0, 0, 0})
        '
        'strCH_5_Max
        '
        Me.strCH_5_Max.ForeColor = System.Drawing.Color.White
        Me.strCH_5_Max.Location = New System.Drawing.Point(9, 195)
        Me.strCH_5_Max.Maximum = New Decimal(New Integer() {2500, 0, 0, 0})
        Me.strCH_5_Max.Minimum = New Decimal(New Integer() {500, 0, 0, 0})
        Me.strCH_5_Max.Name = "strCH_5_Max"
        Me.strCH_5_Max.Size = New System.Drawing.Size(64, 26)
        Me.strCH_5_Max.TabIndex = 22
        Me.strCH_5_Max.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.strCH_5_Max.Value = New Decimal(New Integer() {500, 0, 0, 0})
        '
        'strCH_4_Max
        '
        Me.strCH_4_Max.ForeColor = System.Drawing.Color.White
        Me.strCH_4_Max.Location = New System.Drawing.Point(9, 155)
        Me.strCH_4_Max.Maximum = New Decimal(New Integer() {2500, 0, 0, 0})
        Me.strCH_4_Max.Minimum = New Decimal(New Integer() {500, 0, 0, 0})
        Me.strCH_4_Max.Name = "strCH_4_Max"
        Me.strCH_4_Max.Size = New System.Drawing.Size(64, 26)
        Me.strCH_4_Max.TabIndex = 21
        Me.strCH_4_Max.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.strCH_4_Max.Value = New Decimal(New Integer() {500, 0, 0, 0})
        '
        'strCH_3_Max
        '
        Me.strCH_3_Max.ForeColor = System.Drawing.Color.White
        Me.strCH_3_Max.Location = New System.Drawing.Point(9, 111)
        Me.strCH_3_Max.Maximum = New Decimal(New Integer() {2500, 0, 0, 0})
        Me.strCH_3_Max.Minimum = New Decimal(New Integer() {500, 0, 0, 0})
        Me.strCH_3_Max.Name = "strCH_3_Max"
        Me.strCH_3_Max.Size = New System.Drawing.Size(64, 26)
        Me.strCH_3_Max.TabIndex = 20
        Me.strCH_3_Max.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.strCH_3_Max.Value = New Decimal(New Integer() {500, 0, 0, 0})
        '
        'strCH_2_Max
        '
        Me.strCH_2_Max.AutoSize = True
        Me.strCH_2_Max.ForeColor = System.Drawing.Color.White
        Me.strCH_2_Max.Location = New System.Drawing.Point(9, 70)
        Me.strCH_2_Max.Maximum = New Decimal(New Integer() {2500, 0, 0, 0})
        Me.strCH_2_Max.Minimum = New Decimal(New Integer() {500, 0, 0, 0})
        Me.strCH_2_Max.Name = "strCH_2_Max"
        Me.strCH_2_Max.Size = New System.Drawing.Size(64, 26)
        Me.strCH_2_Max.TabIndex = 19
        Me.strCH_2_Max.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.strCH_2_Max.Value = New Decimal(New Integer() {500, 0, 0, 0})
        '
        'strCH_1_Max
        '
        Me.strCH_1_Max.AutoSize = True
        Me.strCH_1_Max.ForeColor = System.Drawing.Color.White
        Me.strCH_1_Max.Location = New System.Drawing.Point(9, 29)
        Me.strCH_1_Max.Maximum = New Decimal(New Integer() {2500, 0, 0, 0})
        Me.strCH_1_Max.Minimum = New Decimal(New Integer() {500, 0, 0, 0})
        Me.strCH_1_Max.Name = "strCH_1_Max"
        Me.strCH_1_Max.Size = New System.Drawing.Size(64, 26)
        Me.strCH_1_Max.TabIndex = 18
        Me.strCH_1_Max.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.strCH_1_Max.Value = New Decimal(New Integer() {500, 0, 0, 0})
        '
        'GroupBox11
        '
        Me.GroupBox11.AutoSize = True
        Me.GroupBox11.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox11.Controls.Add(Me.strCH_8_Neutral)
        Me.GroupBox11.Controls.Add(Me.strCH_7_Neutral)
        Me.GroupBox11.Controls.Add(Me.strCH_6_Neutral)
        Me.GroupBox11.Controls.Add(Me.strCH_5_Neutral)
        Me.GroupBox11.Controls.Add(Me.strCH_4_Neutral)
        Me.GroupBox11.Controls.Add(Me.strCH_3_Neutral)
        Me.GroupBox11.Controls.Add(Me.strCH_2_Neutral)
        Me.GroupBox11.Controls.Add(Me.strCH_1_Neutral)
        Me.GroupBox11.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox11.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBox11.Location = New System.Drawing.Point(557, 24)
        Me.GroupBox11.Name = "GroupBox11"
        Me.GroupBox11.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBox11.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.GroupBox11.Size = New System.Drawing.Size(81, 365)
        Me.GroupBox11.TabIndex = 18
        Me.GroupBox11.TabStop = False
        Me.GroupBox11.Text = "neutral "
        '
        'strCH_8_Neutral
        '
        Me.strCH_8_Neutral.ForeColor = System.Drawing.Color.White
        Me.strCH_8_Neutral.Location = New System.Drawing.Point(9, 317)
        Me.strCH_8_Neutral.Maximum = New Decimal(New Integer() {254, 0, 0, 0})
        Me.strCH_8_Neutral.Name = "strCH_8_Neutral"
        Me.strCH_8_Neutral.Size = New System.Drawing.Size(64, 26)
        Me.strCH_8_Neutral.TabIndex = 25
        Me.strCH_8_Neutral.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'strCH_7_Neutral
        '
        Me.strCH_7_Neutral.ForeColor = System.Drawing.Color.White
        Me.strCH_7_Neutral.Location = New System.Drawing.Point(9, 276)
        Me.strCH_7_Neutral.Maximum = New Decimal(New Integer() {254, 0, 0, 0})
        Me.strCH_7_Neutral.Name = "strCH_7_Neutral"
        Me.strCH_7_Neutral.Size = New System.Drawing.Size(64, 26)
        Me.strCH_7_Neutral.TabIndex = 24
        Me.strCH_7_Neutral.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'strCH_6_Neutral
        '
        Me.strCH_6_Neutral.ForeColor = System.Drawing.Color.White
        Me.strCH_6_Neutral.Location = New System.Drawing.Point(9, 238)
        Me.strCH_6_Neutral.Maximum = New Decimal(New Integer() {254, 0, 0, 0})
        Me.strCH_6_Neutral.Name = "strCH_6_Neutral"
        Me.strCH_6_Neutral.Size = New System.Drawing.Size(64, 26)
        Me.strCH_6_Neutral.TabIndex = 23
        Me.strCH_6_Neutral.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'strCH_5_Neutral
        '
        Me.strCH_5_Neutral.ForeColor = System.Drawing.Color.White
        Me.strCH_5_Neutral.Location = New System.Drawing.Point(9, 195)
        Me.strCH_5_Neutral.Maximum = New Decimal(New Integer() {254, 0, 0, 0})
        Me.strCH_5_Neutral.Name = "strCH_5_Neutral"
        Me.strCH_5_Neutral.Size = New System.Drawing.Size(64, 26)
        Me.strCH_5_Neutral.TabIndex = 22
        Me.strCH_5_Neutral.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'strCH_4_Neutral
        '
        Me.strCH_4_Neutral.ForeColor = System.Drawing.Color.White
        Me.strCH_4_Neutral.Location = New System.Drawing.Point(9, 155)
        Me.strCH_4_Neutral.Maximum = New Decimal(New Integer() {254, 0, 0, 0})
        Me.strCH_4_Neutral.Name = "strCH_4_Neutral"
        Me.strCH_4_Neutral.Size = New System.Drawing.Size(64, 26)
        Me.strCH_4_Neutral.TabIndex = 21
        Me.strCH_4_Neutral.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'strCH_3_Neutral
        '
        Me.strCH_3_Neutral.ForeColor = System.Drawing.Color.White
        Me.strCH_3_Neutral.Location = New System.Drawing.Point(9, 111)
        Me.strCH_3_Neutral.Maximum = New Decimal(New Integer() {254, 0, 0, 0})
        Me.strCH_3_Neutral.Name = "strCH_3_Neutral"
        Me.strCH_3_Neutral.Size = New System.Drawing.Size(64, 26)
        Me.strCH_3_Neutral.TabIndex = 20
        Me.strCH_3_Neutral.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'strCH_2_Neutral
        '
        Me.strCH_2_Neutral.ForeColor = System.Drawing.Color.White
        Me.strCH_2_Neutral.Location = New System.Drawing.Point(9, 70)
        Me.strCH_2_Neutral.Maximum = New Decimal(New Integer() {254, 0, 0, 0})
        Me.strCH_2_Neutral.Name = "strCH_2_Neutral"
        Me.strCH_2_Neutral.Size = New System.Drawing.Size(64, 26)
        Me.strCH_2_Neutral.TabIndex = 19
        Me.strCH_2_Neutral.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'strCH_1_Neutral
        '
        Me.strCH_1_Neutral.ForeColor = System.Drawing.Color.White
        Me.strCH_1_Neutral.Location = New System.Drawing.Point(9, 29)
        Me.strCH_1_Neutral.Maximum = New Decimal(New Integer() {254, 0, 0, 0})
        Me.strCH_1_Neutral.Name = "strCH_1_Neutral"
        Me.strCH_1_Neutral.Size = New System.Drawing.Size(64, 26)
        Me.strCH_1_Neutral.TabIndex = 18
        Me.strCH_1_Neutral.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        '_Frame2_0
        '
        Me._Frame2_0.AutoSize = True
        Me._Frame2_0.BackColor = System.Drawing.SystemColors.Control
        Me._Frame2_0.Controls.Add(Me.ch8_HScrollBar)
        Me._Frame2_0.Controls.Add(Me.ch7_HScrollBar)
        Me._Frame2_0.Controls.Add(Me.ch6_HScrollBar)
        Me._Frame2_0.Controls.Add(Me.ch5_HScrollBar)
        Me._Frame2_0.Controls.Add(Me.ch4_HScrollBar)
        Me._Frame2_0.Controls.Add(Me.ch3_HScrollBar)
        Me._Frame2_0.Controls.Add(Me.ch2_HScrollBar)
        Me._Frame2_0.Controls.Add(Me.ch1_HScrollBar)
        Me._Frame2_0.Controls.Add(Me.strCH_8_Current)
        Me._Frame2_0.Controls.Add(Me.strCH_7_Current)
        Me._Frame2_0.Controls.Add(Me.strCH_6_Current)
        Me._Frame2_0.Controls.Add(Me.strCH_5_Current)
        Me._Frame2_0.Controls.Add(Me.strCH_4_Current)
        Me._Frame2_0.Controls.Add(Me.strCH_3_Current)
        Me._Frame2_0.Controls.Add(Me.strCH_2_Current)
        Me._Frame2_0.Controls.Add(Me.Label14)
        Me._Frame2_0.Controls.Add(Me.Label13)
        Me._Frame2_0.Controls.Add(Me.strCH_1_Current)
        Me._Frame2_0.Controls.Add(Me.Label2)
        Me._Frame2_0.Controls.Add(Me.Label9)
        Me._Frame2_0.Controls.Add(Me.Label8)
        Me._Frame2_0.Controls.Add(Me.Label7)
        Me._Frame2_0.Controls.Add(Me.Label6)
        Me._Frame2_0.Controls.Add(Me.Label5)
        Me._Frame2_0.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._Frame2_0.ForeColor = System.Drawing.SystemColors.ControlText
        Me._Frame2_0.Location = New System.Drawing.Point(14, 24)
        Me._Frame2_0.Name = "_Frame2_0"
        Me._Frame2_0.Padding = New System.Windows.Forms.Padding(0)
        Me._Frame2_0.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me._Frame2_0.Size = New System.Drawing.Size(360, 370)
        Me._Frame2_0.TabIndex = 3
        Me._Frame2_0.TabStop = False
        Me._Frame2_0.Text = "Position"
        '
        'ch8_HScrollBar
        '
        Me.ch8_HScrollBar.LargeChange = 1
        Me.ch8_HScrollBar.Location = New System.Drawing.Point(122, 315)
        Me.ch8_HScrollBar.Margin = New System.Windows.Forms.Padding(2)
        Me.ch8_HScrollBar.Maximum = 2500
        Me.ch8_HScrollBar.Name = "ch8_HScrollBar"
        Me.ch8_HScrollBar.Size = New System.Drawing.Size(228, 32)
        Me.ch8_HScrollBar.TabIndex = 36
        Me.ch8_HScrollBar.Value = 1250
        '
        'ch7_HScrollBar
        '
        Me.ch7_HScrollBar.LargeChange = 1
        Me.ch7_HScrollBar.Location = New System.Drawing.Point(122, 274)
        Me.ch7_HScrollBar.Margin = New System.Windows.Forms.Padding(2)
        Me.ch7_HScrollBar.Maximum = 2500
        Me.ch7_HScrollBar.Name = "ch7_HScrollBar"
        Me.ch7_HScrollBar.Size = New System.Drawing.Size(228, 32)
        Me.ch7_HScrollBar.TabIndex = 35
        Me.ch7_HScrollBar.Value = 1250
        '
        'ch6_HScrollBar
        '
        Me.ch6_HScrollBar.LargeChange = 1
        Me.ch6_HScrollBar.Location = New System.Drawing.Point(122, 233)
        Me.ch6_HScrollBar.Margin = New System.Windows.Forms.Padding(2)
        Me.ch6_HScrollBar.Maximum = 2500
        Me.ch6_HScrollBar.Name = "ch6_HScrollBar"
        Me.ch6_HScrollBar.Size = New System.Drawing.Size(228, 32)
        Me.ch6_HScrollBar.TabIndex = 34
        Me.ch6_HScrollBar.Value = 1250
        '
        'ch5_HScrollBar
        '
        Me.ch5_HScrollBar.LargeChange = 1
        Me.ch5_HScrollBar.Location = New System.Drawing.Point(122, 194)
        Me.ch5_HScrollBar.Margin = New System.Windows.Forms.Padding(2)
        Me.ch5_HScrollBar.Maximum = 2500
        Me.ch5_HScrollBar.Name = "ch5_HScrollBar"
        Me.ch5_HScrollBar.Size = New System.Drawing.Size(228, 32)
        Me.ch5_HScrollBar.TabIndex = 33
        Me.ch5_HScrollBar.Value = 1250
        '
        'ch4_HScrollBar
        '
        Me.ch4_HScrollBar.LargeChange = 1
        Me.ch4_HScrollBar.Location = New System.Drawing.Point(122, 150)
        Me.ch4_HScrollBar.Margin = New System.Windows.Forms.Padding(2)
        Me.ch4_HScrollBar.Maximum = 2500
        Me.ch4_HScrollBar.Name = "ch4_HScrollBar"
        Me.ch4_HScrollBar.Size = New System.Drawing.Size(228, 32)
        Me.ch4_HScrollBar.TabIndex = 32
        Me.ch4_HScrollBar.Value = 1250
        '
        'ch3_HScrollBar
        '
        Me.ch3_HScrollBar.LargeChange = 1
        Me.ch3_HScrollBar.Location = New System.Drawing.Point(122, 107)
        Me.ch3_HScrollBar.Margin = New System.Windows.Forms.Padding(2)
        Me.ch3_HScrollBar.Maximum = 2500
        Me.ch3_HScrollBar.Name = "ch3_HScrollBar"
        Me.ch3_HScrollBar.Size = New System.Drawing.Size(228, 32)
        Me.ch3_HScrollBar.TabIndex = 31
        Me.ch3_HScrollBar.Value = 1250
        '
        'ch2_HScrollBar
        '
        Me.ch2_HScrollBar.LargeChange = 1
        Me.ch2_HScrollBar.Location = New System.Drawing.Point(122, 66)
        Me.ch2_HScrollBar.Margin = New System.Windows.Forms.Padding(2)
        Me.ch2_HScrollBar.Maximum = 2500
        Me.ch2_HScrollBar.Name = "ch2_HScrollBar"
        Me.ch2_HScrollBar.Size = New System.Drawing.Size(228, 32)
        Me.ch2_HScrollBar.TabIndex = 30
        Me.ch2_HScrollBar.Value = 1250
        '
        'ch1_HScrollBar
        '
        Me.ch1_HScrollBar.LargeChange = 1
        Me.ch1_HScrollBar.Location = New System.Drawing.Point(122, 26)
        Me.ch1_HScrollBar.Margin = New System.Windows.Forms.Padding(2)
        Me.ch1_HScrollBar.Maximum = 2500
        Me.ch1_HScrollBar.Name = "ch1_HScrollBar"
        Me.ch1_HScrollBar.Size = New System.Drawing.Size(228, 32)
        Me.ch1_HScrollBar.TabIndex = 29
        Me.ch1_HScrollBar.Value = 1250
        '
        'strCH_8_Current
        '
        Me.strCH_8_Current.AcceptsReturn = True
        Me.strCH_8_Current.BackColor = System.Drawing.SystemColors.Window
        Me.strCH_8_Current.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.strCH_8_Current.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.strCH_8_Current.ForeColor = System.Drawing.SystemColors.WindowText
        Me.strCH_8_Current.Location = New System.Drawing.Point(42, 313)
        Me.strCH_8_Current.MaxLength = 4
        Me.strCH_8_Current.Name = "strCH_8_Current"
        Me.strCH_8_Current.ReadOnly = True
        Me.strCH_8_Current.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.strCH_8_Current.Size = New System.Drawing.Size(73, 35)
        Me.strCH_8_Current.TabIndex = 25
        Me.strCH_8_Current.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'strCH_7_Current
        '
        Me.strCH_7_Current.AcceptsReturn = True
        Me.strCH_7_Current.BackColor = System.Drawing.SystemColors.Window
        Me.strCH_7_Current.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.strCH_7_Current.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.strCH_7_Current.ForeColor = System.Drawing.SystemColors.WindowText
        Me.strCH_7_Current.Location = New System.Drawing.Point(42, 273)
        Me.strCH_7_Current.MaxLength = 4
        Me.strCH_7_Current.Name = "strCH_7_Current"
        Me.strCH_7_Current.ReadOnly = True
        Me.strCH_7_Current.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.strCH_7_Current.Size = New System.Drawing.Size(73, 35)
        Me.strCH_7_Current.TabIndex = 24
        Me.strCH_7_Current.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'strCH_6_Current
        '
        Me.strCH_6_Current.AcceptsReturn = True
        Me.strCH_6_Current.BackColor = System.Drawing.SystemColors.Window
        Me.strCH_6_Current.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.strCH_6_Current.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.strCH_6_Current.ForeColor = System.Drawing.SystemColors.WindowText
        Me.strCH_6_Current.Location = New System.Drawing.Point(41, 231)
        Me.strCH_6_Current.MaxLength = 4
        Me.strCH_6_Current.Name = "strCH_6_Current"
        Me.strCH_6_Current.ReadOnly = True
        Me.strCH_6_Current.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.strCH_6_Current.Size = New System.Drawing.Size(73, 35)
        Me.strCH_6_Current.TabIndex = 23
        Me.strCH_6_Current.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'strCH_5_Current
        '
        Me.strCH_5_Current.AcceptsReturn = True
        Me.strCH_5_Current.BackColor = System.Drawing.SystemColors.Window
        Me.strCH_5_Current.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.strCH_5_Current.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.strCH_5_Current.ForeColor = System.Drawing.SystemColors.WindowText
        Me.strCH_5_Current.Location = New System.Drawing.Point(40, 191)
        Me.strCH_5_Current.MaxLength = 4
        Me.strCH_5_Current.Name = "strCH_5_Current"
        Me.strCH_5_Current.ReadOnly = True
        Me.strCH_5_Current.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.strCH_5_Current.Size = New System.Drawing.Size(73, 35)
        Me.strCH_5_Current.TabIndex = 14
        Me.strCH_5_Current.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'strCH_4_Current
        '
        Me.strCH_4_Current.AcceptsReturn = True
        Me.strCH_4_Current.BackColor = System.Drawing.SystemColors.Window
        Me.strCH_4_Current.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.strCH_4_Current.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.strCH_4_Current.ForeColor = System.Drawing.SystemColors.WindowText
        Me.strCH_4_Current.Location = New System.Drawing.Point(39, 148)
        Me.strCH_4_Current.MaxLength = 4
        Me.strCH_4_Current.Name = "strCH_4_Current"
        Me.strCH_4_Current.ReadOnly = True
        Me.strCH_4_Current.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.strCH_4_Current.Size = New System.Drawing.Size(73, 35)
        Me.strCH_4_Current.TabIndex = 13
        Me.strCH_4_Current.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'strCH_3_Current
        '
        Me.strCH_3_Current.AcceptsReturn = True
        Me.strCH_3_Current.BackColor = System.Drawing.SystemColors.Window
        Me.strCH_3_Current.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.strCH_3_Current.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.strCH_3_Current.ForeColor = System.Drawing.SystemColors.WindowText
        Me.strCH_3_Current.Location = New System.Drawing.Point(39, 107)
        Me.strCH_3_Current.MaxLength = 4
        Me.strCH_3_Current.Name = "strCH_3_Current"
        Me.strCH_3_Current.ReadOnly = True
        Me.strCH_3_Current.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.strCH_3_Current.Size = New System.Drawing.Size(73, 35)
        Me.strCH_3_Current.TabIndex = 12
        Me.strCH_3_Current.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'strCH_2_Current
        '
        Me.strCH_2_Current.AcceptsReturn = True
        Me.strCH_2_Current.BackColor = System.Drawing.SystemColors.Window
        Me.strCH_2_Current.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.strCH_2_Current.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.strCH_2_Current.ForeColor = System.Drawing.SystemColors.WindowText
        Me.strCH_2_Current.Location = New System.Drawing.Point(38, 65)
        Me.strCH_2_Current.MaxLength = 4
        Me.strCH_2_Current.Name = "strCH_2_Current"
        Me.strCH_2_Current.ReadOnly = True
        Me.strCH_2_Current.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.strCH_2_Current.Size = New System.Drawing.Size(73, 35)
        Me.strCH_2_Current.TabIndex = 6
        Me.strCH_2_Current.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.BackColor = System.Drawing.SystemColors.Control
        Me.Label14.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label14.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label14.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label14.Location = New System.Drawing.Point(13, 317)
        Me.Label14.Name = "Label14"
        Me.Label14.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label14.Size = New System.Drawing.Size(32, 27)
        Me.Label14.TabIndex = 28
        Me.Label14.Text = "8:"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.BackColor = System.Drawing.SystemColors.Control
        Me.Label13.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label13.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label13.Location = New System.Drawing.Point(13, 276)
        Me.Label13.Name = "Label13"
        Me.Label13.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label13.Size = New System.Drawing.Size(32, 27)
        Me.Label13.TabIndex = 27
        Me.Label13.Text = "7:"
        '
        'strCH_1_Current
        '
        Me.strCH_1_Current.AcceptsReturn = True
        Me.strCH_1_Current.BackColor = System.Drawing.SystemColors.Window
        Me.strCH_1_Current.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.strCH_1_Current.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.strCH_1_Current.ForeColor = System.Drawing.SystemColors.WindowText
        Me.strCH_1_Current.Location = New System.Drawing.Point(38, 24)
        Me.strCH_1_Current.MaxLength = 4
        Me.strCH_1_Current.Name = "strCH_1_Current"
        Me.strCH_1_Current.ReadOnly = True
        Me.strCH_1_Current.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.strCH_1_Current.Size = New System.Drawing.Size(73, 35)
        Me.strCH_1_Current.TabIndex = 1
        Me.strCH_1_Current.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.SystemColors.Control
        Me.Label2.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label2.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label2.Location = New System.Drawing.Point(13, 235)
        Me.Label2.Name = "Label2"
        Me.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label2.Size = New System.Drawing.Size(32, 27)
        Me.Label2.TabIndex = 26
        Me.Label2.Text = "6:"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.BackColor = System.Drawing.SystemColors.Control
        Me.Label9.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label9.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label9.Location = New System.Drawing.Point(12, 195)
        Me.Label9.Name = "Label9"
        Me.Label9.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label9.Size = New System.Drawing.Size(32, 27)
        Me.Label9.TabIndex = 22
        Me.Label9.Text = "5:"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.BackColor = System.Drawing.SystemColors.Control
        Me.Label8.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label8.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label8.Location = New System.Drawing.Point(13, 151)
        Me.Label8.Name = "Label8"
        Me.Label8.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label8.Size = New System.Drawing.Size(32, 27)
        Me.Label8.TabIndex = 21
        Me.Label8.Text = "4:"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.BackColor = System.Drawing.SystemColors.Control
        Me.Label7.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label7.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label7.Location = New System.Drawing.Point(10, 109)
        Me.Label7.Name = "Label7"
        Me.Label7.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label7.Size = New System.Drawing.Size(32, 27)
        Me.Label7.TabIndex = 20
        Me.Label7.Text = "3:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.SystemColors.Control
        Me.Label6.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label6.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label6.Location = New System.Drawing.Point(9, 68)
        Me.Label6.Name = "Label6"
        Me.Label6.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label6.Size = New System.Drawing.Size(32, 27)
        Me.Label6.TabIndex = 19
        Me.Label6.Text = "2:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.SystemColors.Control
        Me.Label5.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label5.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label5.Location = New System.Drawing.Point(7, 27)
        Me.Label5.Name = "Label5"
        Me.Label5.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label5.Size = New System.Drawing.Size(32, 27)
        Me.Label5.TabIndex = 18
        Me.Label5.Text = "1:"
        '
        'GroupBox5
        '
        Me.GroupBox5.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox5.Controls.Add(Me.Label4)
        Me.GroupBox5.Controls.Add(Me.strSSC_Firmware)
        Me.GroupBox5.Controls.Add(Me.Label3)
        Me.GroupBox5.Controls.Add(Me.TypeId)
        Me.GroupBox5.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox5.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBox5.Location = New System.Drawing.Point(677, 10)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBox5.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.GroupBox5.Size = New System.Drawing.Size(204, 107)
        Me.GroupBox5.TabIndex = 23
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "PiKoder Info"
        '
        'Label4
        '
        Me.Label4.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label4.Location = New System.Drawing.Point(10, 68)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(101, 22)
        Me.Label4.TabIndex = 20
        Me.Label4.Text = "Firmware"
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label3.Location = New System.Drawing.Point(7, 35)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(75, 32)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "Type Id"
        '
        'TypeId
        '
        Me.TypeId.AcceptsReturn = True
        Me.TypeId.BackColor = System.Drawing.SystemColors.Window
        Me.TypeId.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.TypeId.Font = New System.Drawing.Font("Arial", 12.0!)
        Me.TypeId.ForeColor = System.Drawing.SystemColors.WindowText
        Me.TypeId.Location = New System.Drawing.Point(87, 32)
        Me.TypeId.Margin = New System.Windows.Forms.Padding(2)
        Me.TypeId.MaxLength = 4
        Me.TypeId.Name = "TypeId"
        Me.TypeId.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.TypeId.Size = New System.Drawing.Size(100, 26)
        Me.TypeId.TabIndex = 19
        Me.TypeId.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'GroupBox6
        '
        Me.GroupBox6.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox6.Controls.Add(Me.GroupBox17)
        Me.GroupBox6.Controls.Add(Me.GroupBox13)
        Me.GroupBox6.Controls.Add(Me.GroupBox8)
        Me.GroupBox6.Controls.Add(Me.GroupBox4)
        Me.GroupBox6.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox6.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBox6.Location = New System.Drawing.Point(677, 126)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBox6.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.GroupBox6.Size = New System.Drawing.Size(204, 305)
        Me.GroupBox6.TabIndex = 24
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "PiKoder Parameters"
        '
        'GroupBox17
        '
        Me.GroupBox17.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox17.Controls.Add(Me.PPM_Mode)
        Me.GroupBox17.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox17.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBox17.Location = New System.Drawing.Point(9, 234)
        Me.GroupBox17.Name = "GroupBox17"
        Me.GroupBox17.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBox17.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.GroupBox17.Size = New System.Drawing.Size(178, 59)
        Me.GroupBox17.TabIndex = 30
        Me.GroupBox17.TabStop = False
        Me.GroupBox17.Text = "PPM-Mode"
        '
        'PPM_Mode
        '
        Me.PPM_Mode.Font = New System.Drawing.Font("Arial", 10.0!, System.Drawing.FontStyle.Bold)
        Me.PPM_Mode.FormattingEnabled = True
        Me.PPM_Mode.ItemHeight = 16
        Me.PPM_Mode.Location = New System.Drawing.Point(10, 25)
        Me.PPM_Mode.Name = "PPM_Mode"
        Me.PPM_Mode.Size = New System.Drawing.Size(154, 20)
        Me.PPM_Mode.TabIndex = 1
        '
        'GroupBox13
        '
        Me.GroupBox13.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox13.Controls.Add(Me.GroupBox15)
        Me.GroupBox13.Controls.Add(Me.GroupBox14)
        Me.GroupBox13.Controls.Add(Me.PPM_Channels)
        Me.GroupBox13.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox13.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBox13.Location = New System.Drawing.Point(9, 162)
        Me.GroupBox13.Name = "GroupBox13"
        Me.GroupBox13.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBox13.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.GroupBox13.Size = New System.Drawing.Size(145, 63)
        Me.GroupBox13.TabIndex = 28
        Me.GroupBox13.TabStop = False
        Me.GroupBox13.Text = "PPM-Channels"
        '
        'GroupBox15
        '
        Me.GroupBox15.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox15.Controls.Add(Me.GroupBox16)
        Me.GroupBox15.Controls.Add(Me.NumericUpDown5)
        Me.GroupBox15.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox15.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBox15.Location = New System.Drawing.Point(2, 71)
        Me.GroupBox15.Name = "GroupBox15"
        Me.GroupBox15.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBox15.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.GroupBox15.Size = New System.Drawing.Size(145, 63)
        Me.GroupBox15.TabIndex = 29
        Me.GroupBox15.TabStop = False
        Me.GroupBox15.Text = "PPM-Channels"
        '
        'GroupBox16
        '
        Me.GroupBox16.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox16.Controls.Add(Me.NumericUpDown4)
        Me.GroupBox16.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox16.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBox16.Location = New System.Drawing.Point(1, 71)
        Me.GroupBox16.Name = "GroupBox16"
        Me.GroupBox16.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBox16.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.GroupBox16.Size = New System.Drawing.Size(145, 63)
        Me.GroupBox16.TabIndex = 27
        Me.GroupBox16.TabStop = False
        Me.GroupBox16.Text = "Zero Offset"
        '
        'NumericUpDown4
        '
        Me.NumericUpDown4.ForeColor = System.Drawing.Color.White
        Me.NumericUpDown4.Location = New System.Drawing.Point(28, 25)
        Me.NumericUpDown4.Maximum = New Decimal(New Integer() {99, 0, 0, 0})
        Me.NumericUpDown4.Name = "NumericUpDown4"
        Me.NumericUpDown4.Size = New System.Drawing.Size(73, 26)
        Me.NumericUpDown4.TabIndex = 26
        Me.NumericUpDown4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'NumericUpDown5
        '
        Me.NumericUpDown5.ForeColor = System.Drawing.Color.White
        Me.NumericUpDown5.Location = New System.Drawing.Point(28, 25)
        Me.NumericUpDown5.Maximum = New Decimal(New Integer() {99, 0, 0, 0})
        Me.NumericUpDown5.Name = "NumericUpDown5"
        Me.NumericUpDown5.Size = New System.Drawing.Size(73, 26)
        Me.NumericUpDown5.TabIndex = 26
        Me.NumericUpDown5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'GroupBox14
        '
        Me.GroupBox14.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox14.Controls.Add(Me.NumericUpDown2)
        Me.GroupBox14.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox14.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBox14.Location = New System.Drawing.Point(1, 71)
        Me.GroupBox14.Name = "GroupBox14"
        Me.GroupBox14.Padding = New System.Windows.Forms.Padding(0)
        Me.GroupBox14.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.GroupBox14.Size = New System.Drawing.Size(145, 63)
        Me.GroupBox14.TabIndex = 27
        Me.GroupBox14.TabStop = False
        Me.GroupBox14.Text = "Zero Offset"
        '
        'NumericUpDown2
        '
        Me.NumericUpDown2.ForeColor = System.Drawing.Color.White
        Me.NumericUpDown2.Location = New System.Drawing.Point(28, 25)
        Me.NumericUpDown2.Maximum = New Decimal(New Integer() {99, 0, 0, 0})
        Me.NumericUpDown2.Name = "NumericUpDown2"
        Me.NumericUpDown2.Size = New System.Drawing.Size(73, 26)
        Me.NumericUpDown2.TabIndex = 26
        Me.NumericUpDown2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'PPM_Channels
        '
        Me.PPM_Channels.ForeColor = System.Drawing.Color.White
        Me.PPM_Channels.Location = New System.Drawing.Point(13, 25)
        Me.PPM_Channels.Maximum = New Decimal(New Integer() {8, 0, 0, 0})
        Me.PPM_Channels.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.PPM_Channels.Name = "PPM_Channels"
        Me.PPM_Channels.Size = New System.Drawing.Size(73, 26)
        Me.PPM_Channels.TabIndex = 26
        Me.PPM_Channels.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.PPM_Channels.Value = New Decimal(New Integer() {8, 0, 0, 0})
        '
        'tHeartBeat
        '
        Me.tHeartBeat.Interval = 5000
        '
        'PCC_PiKoder_Control_Center
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(890, 529)
        Me.Controls.Add(Me.GroupBox5)
        Me.Controls.Add(Me.GroupBox7)
        Me.Controls.Add(Me.frmOuter)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox6)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "PCC_PiKoder_Control_Center"
        Me.Text = "PCC_PiKoder_Control_Center"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox12.ResumeLayout(False)
        CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TimeOut, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox8.ResumeLayout(False)
        CType(Me.miniSSCOffset, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox7.ResumeLayout(False)
        Me.GroupBox7.PerformLayout()
        Me.frmOuter.ResumeLayout(False)
        Me.frmOuter.PerformLayout()
        Me.GroupBox9.ResumeLayout(False)
        Me.GroupBox9.PerformLayout()
        CType(Me.strCH_8_Min, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.strCH_7_Min, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.strCH_6_Min, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.strCH_5_Min, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.strCH_4_Min, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.strCH_3_Min, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.strCH_2_Min, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.strCH_1_Min, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox10.ResumeLayout(False)
        Me.GroupBox10.PerformLayout()
        CType(Me.strCH_8_Max, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.strCH_7_Max, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.strCH_6_Max, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.strCH_5_Max, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.strCH_4_Max, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.strCH_3_Max, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.strCH_2_Max, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.strCH_1_Max, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox11.ResumeLayout(False)
        CType(Me.strCH_8_Neutral, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.strCH_7_Neutral, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.strCH_6_Neutral, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.strCH_5_Neutral, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.strCH_4_Neutral, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.strCH_3_Neutral, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.strCH_2_Neutral, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.strCH_1_Neutral, System.ComponentModel.ISupportInitialize).EndInit()
        Me._Frame2_0.ResumeLayout(False)
        Me._Frame2_0.PerformLayout()
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox17.ResumeLayout(False)
        Me.GroupBox13.ResumeLayout(False)
        Me.GroupBox15.ResumeLayout(False)
        Me.GroupBox16.ResumeLayout(False)
        CType(Me.NumericUpDown4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericUpDown5, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox14.ResumeLayout(False)
        CType(Me.NumericUpDown2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PPM_Channels, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents AvailableCOMPorts As System.Windows.Forms.ListBox
    Public WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents Led2 As LED
    Public WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Public WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents TimeOut As System.Windows.Forms.NumericUpDown
    Public WithEvents GroupBox8 As System.Windows.Forms.GroupBox
    Friend WithEvents miniSSCOffset As System.Windows.Forms.NumericUpDown
    Public WithEvents GroupBox7 As System.Windows.Forms.GroupBox
    Public WithEvents saveButton As System.Windows.Forms.Button
    Public WithEvents strSSC_Firmware As System.Windows.Forms.TextBox
    Public WithEvents frmOuter As System.Windows.Forms.GroupBox
    Public WithEvents GroupBox9 As System.Windows.Forms.GroupBox
    Friend WithEvents strCH_8_Min As System.Windows.Forms.NumericUpDown
    Friend WithEvents strCH_7_Min As System.Windows.Forms.NumericUpDown
    Friend WithEvents strCH_6_Min As System.Windows.Forms.NumericUpDown
    Friend WithEvents strCH_5_Min As System.Windows.Forms.NumericUpDown
    Friend WithEvents strCH_4_Min As System.Windows.Forms.NumericUpDown
    Friend WithEvents strCH_3_Min As System.Windows.Forms.NumericUpDown
    Friend WithEvents strCH_2_Min As System.Windows.Forms.NumericUpDown
    Friend WithEvents strCH_1_Min As System.Windows.Forms.NumericUpDown
    Public WithEvents GroupBox10 As System.Windows.Forms.GroupBox
    Friend WithEvents strCH_8_Max As System.Windows.Forms.NumericUpDown
    Friend WithEvents strCH_7_Max As System.Windows.Forms.NumericUpDown
    Friend WithEvents strCH_6_Max As System.Windows.Forms.NumericUpDown
    Friend WithEvents strCH_5_Max As System.Windows.Forms.NumericUpDown
    Friend WithEvents strCH_4_Max As System.Windows.Forms.NumericUpDown
    Friend WithEvents strCH_3_Max As System.Windows.Forms.NumericUpDown
    Friend WithEvents strCH_2_Max As System.Windows.Forms.NumericUpDown
    Friend WithEvents strCH_1_Max As System.Windows.Forms.NumericUpDown
    Public WithEvents GroupBox11 As System.Windows.Forms.GroupBox
    Friend WithEvents strCH_8_Neutral As System.Windows.Forms.NumericUpDown
    Friend WithEvents strCH_7_Neutral As System.Windows.Forms.NumericUpDown
    Friend WithEvents strCH_6_Neutral As System.Windows.Forms.NumericUpDown
    Friend WithEvents strCH_5_Neutral As System.Windows.Forms.NumericUpDown
    Friend WithEvents strCH_4_Neutral As System.Windows.Forms.NumericUpDown
    Friend WithEvents strCH_3_Neutral As System.Windows.Forms.NumericUpDown
    Friend WithEvents strCH_2_Neutral As System.Windows.Forms.NumericUpDown
    Friend WithEvents strCH_1_Neutral As System.Windows.Forms.NumericUpDown
    Public WithEvents _Frame2_0 As System.Windows.Forms.GroupBox
    Friend WithEvents ch8_HScrollBar As System.Windows.Forms.HScrollBar
    Friend WithEvents ch7_HScrollBar As System.Windows.Forms.HScrollBar
    Friend WithEvents ch6_HScrollBar As System.Windows.Forms.HScrollBar
    Friend WithEvents ch5_HScrollBar As System.Windows.Forms.HScrollBar
    Friend WithEvents ch4_HScrollBar As System.Windows.Forms.HScrollBar
    Friend WithEvents ch3_HScrollBar As System.Windows.Forms.HScrollBar
    Friend WithEvents ch2_HScrollBar As System.Windows.Forms.HScrollBar
    Friend WithEvents ch1_HScrollBar As System.Windows.Forms.HScrollBar
    Public WithEvents strCH_8_Current As System.Windows.Forms.TextBox
    Public WithEvents strCH_7_Current As System.Windows.Forms.TextBox
    Public WithEvents strCH_6_Current As System.Windows.Forms.TextBox
    Public WithEvents strCH_5_Current As System.Windows.Forms.TextBox
    Public WithEvents strCH_4_Current As System.Windows.Forms.TextBox
    Public WithEvents strCH_3_Current As System.Windows.Forms.TextBox
    Public WithEvents strCH_2_Current As System.Windows.Forms.TextBox
    Public WithEvents Label14 As System.Windows.Forms.Label
    Public WithEvents Label13 As System.Windows.Forms.Label
    Public WithEvents strCH_1_Current As System.Windows.Forms.TextBox
    Public WithEvents Label2 As System.Windows.Forms.Label
    Public WithEvents Label9 As System.Windows.Forms.Label
    Public WithEvents Label8 As System.Windows.Forms.Label
    Public WithEvents Label7 As System.Windows.Forms.Label
    Public WithEvents Label6 As System.Windows.Forms.Label
    Public WithEvents Label5 As System.Windows.Forms.Label
    Public WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Public WithEvents TypeId As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Public WithEvents GroupBox6 As System.Windows.Forms.GroupBox
    Public WithEvents tHeartBeat As System.Windows.Forms.Timer
    Public WithEvents GroupBox12 As System.Windows.Forms.GroupBox
    Friend WithEvents NumericUpDown1 As System.Windows.Forms.NumericUpDown
    Public WithEvents GroupBox17 As System.Windows.Forms.GroupBox
    Friend WithEvents PPM_Mode As System.Windows.Forms.ListBox
    Public WithEvents GroupBox13 As System.Windows.Forms.GroupBox
    Public WithEvents GroupBox15 As System.Windows.Forms.GroupBox
    Public WithEvents GroupBox16 As System.Windows.Forms.GroupBox
    Friend WithEvents NumericUpDown4 As System.Windows.Forms.NumericUpDown
    Friend WithEvents NumericUpDown5 As System.Windows.Forms.NumericUpDown
    Public WithEvents GroupBox14 As System.Windows.Forms.GroupBox
    Friend WithEvents NumericUpDown2 As System.Windows.Forms.NumericUpDown
    Friend WithEvents PPM_Channels As System.Windows.Forms.NumericUpDown
End Class
