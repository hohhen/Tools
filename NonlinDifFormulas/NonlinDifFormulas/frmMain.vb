Imports System.Globalization, System.Threading
Imports Tools.VisualBasic
Imports System.ComponentModel
Imports System.Drawing.Drawing2D
''' <summary>Hlavní okno aplikace</summary>
Public Class frmMain
    ''' <summary>Hodnota názvu pro soubor bez názvu</summary>
    Const BezNázvuFileName As String = "(bez názvu)"
    ''' <summary>Název XML tagu equas</summary>
    Const XMLEquas As String = "equas"
    ''' <summary>Udržuje hodnotu vlastnosti <see cref="Changed"/></summary>
    <EditorBrowsable(EditorBrowsableState.Never)> Private _Changed As Boolean = False
    ''' <summary>Sezma aktuálně načtených rovnic</summary>
    Private Equas As New List(Of Equation)
    ''' <summary>Udržuje hodnotu vlastnosti <see cref="CurrentEqua"/></summary>
    <EditorBrowsable(EditorBrowsableState.Never)> Private _CurrentFile As String = ""
    Private CurrentEqua As ToolStripButton

    ''' <summary>Nastavuje/zjišˇuje cestu k aktuálnímu souboru</summary>
    ''' <remarks>Může být "" pro soubor bez názvu</remarks>
    Public Property CurrentFile() As String
        <DebuggerStepThrough()> Get
            Return _CurrentFile
        End Get
        <DebuggerStepThrough()> Set(ByVal value As String)
            _CurrentFile = value
            _Changed = False
            ShowFileName()
        End Set
    End Property
    ''' <summary>Zjišťuje / nastavuje jestli byl aktuální soubor změněn</summary>
    Public Property Changed() As Boolean
        <DebuggerStepThrough()> Get
            Return _Changed
        End Get
        <DebuggerStepThrough()> Set(ByVal value As Boolean)
            _Changed = value
            ShowFileName()
        End Set
    End Property
    ''' <summary>CTor</summary>
    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        tosRovnice.Items.Clear()
        ShowFileName()
        Me.Text = My.Application.Info.Title & " " & My.Application.Info.Version.ToString
    End Sub
    ''' <summary>Zobrazí název souboru v titulku okna</summary>
    Private Sub ShowFileName()
        If CurrentFile = "" Then
            Me.Text = my.Application.Info.Title & " "& BezNázvuFileName
        Else
            Me.Text = My.Application.Info.Title & " " & IO.Path.GetFileName(CurrentFile)
        End If
        If Changed Then Me.Text &= "*"
    End Sub

    Private Sub tmiKonec_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmiKonec.Click
        Me.Close()
    End Sub

    Private Sub equa_Click(ByVal sender As Object, ByVal e As [EventArgs])
        CurrentEqua = sender
        For Each itm As ToolStripButton In tosRovnice.Items
            If itm Is sender Then
                itm.Checked = True
            Else
                itm.Checked = False
            End If
        Next itm
        ShowEqua()
    End Sub
    ''' <summary>Zobrazí informace o vybrané rovnici</summary>
    Private Sub ShowEqua()
        RCEEnabled = True
        With CType(CurrentEqua.Tag, Equation)
            tstNázev.Text = .Name
            tstDx.Text = .Dx
            tstDy.Text = .Dy
            tstDz.Text = .Dz
            tstDu.Text = .Du
            tstPPx.Text = .StartX
            tstPPy.Text = .StartY
            tstPPz.Text = .StartZ
            tstPPu.Text = .StartU
            tstXmax.Text = .MaxX
            tstYmax.Text = .MaxY
            tstYmin.Text = .MinY
            tstXmin.Text = .MinX
            tstTmin.Text = .Tmin
            tstTmax.Text = .Tmax
            tstΔt.Text = .Δt
            tscDifSch.SelectedIndex = .DifSch
            For Each itm As ToolStripItem In tosOsy.Items
                If TypeOf itm Is ToolStripButton Then
                    CType(itm, ToolStripButton).Checked = False
                End If
            Next itm
            Select Case .AxeX
                Case Equation.enmAxes.t
                    tsbXt.Checked = True
                Case Equation.enmAxes.u
                    tsbXU.Checked = True
                Case Equation.enmAxes.x
                    tsbXX.Checked = True
                Case Equation.enmAxes.y
                    tsbXY.Checked = True
                Case Equation.enmAxes.z
                    tsbXZ.Checked = True
            End Select
            Select Case .AxeY
                Case Equation.enmAxes.t
                    tsbYt.Checked = True
                Case Equation.enmAxes.u
                    tsbYU.Checked = True
                Case Equation.enmAxes.x
                    tsbYX.Checked = True
                Case Equation.enmAxes.y
                    tsbYY.Checked = True
                Case Equation.enmAxes.z
                    tsbYZ.Checked = True
            End Select
        End With
        DrawAxes()
    End Sub
    ''' <summary>Zobrazí seznam rovnic</summary>
    Private Sub ShowAllEquas()
        tosRovnice.Items.Clear()
        For Each rce As Equation In Equas
            tosRovnice.Items(tosRovnice.Items.Add( _
                    New ToolStripButton(rce.ToString, Nothing, AddressOf equa_Click) _
                    )).Tag = rce
        Next rce
    End Sub

    Private Sub tsbAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbAdd.Click
        Dim equa As New Equation
        equa.Name = "Equa " & (tosRovnice.Items.Count + 1)
        Equas.Add(equa)
        Changed = True
        Dim itm As New ToolStripButton(equa.ToString, Nothing, AddressOf equa_Click)
        itm.Tag = equa
        tosRovnice.Items.Add(itm)
        equa_Click(itm, New EventArgs())
    End Sub
    ''' <summary>Nastavuje vlastnost Enabled prvků u nichž se tato vlastnost mění pokud je/není vybrána rovnice</summary>
    Private WriteOnly Property RCEEnabled() As Boolean
        Set(ByVal value As Boolean)
            tsbDel.Enabled = value
            tmiVykreslit.Enabled = value
            tosRight.Enabled = value
            picMain.Enabled = value
            tosOsy.Enabled = value
            tosWait.Enabled = value
            nudWait.Enabled = value
        End Set
    End Property
    ''' <summary>Uloží soubor</summary>
    ''' <returns>True pokud uživatel operaci nestornoval</returns>
    Private Function Save() As Boolean
        If CurrentFile = "" Then
            Return SaveAs()
        Else
            If Save(CurrentFile) Then
                Changed = False
                Return True
            Else
                Return SaveAs()
            End If
        End If
    End Function
    ''' <summary>Uloží sobor pod zvoleným názvem</summary>
    ''' <returns>True pokud uživatel operaci nestornoval</returns>
    Private Function SaveAs() As Boolean
        If sfdSave.ShowDialog = Windows.Forms.DialogResult.OK Then
            If Save(sfdSave.FileName) Then
                CurrentFile = sfdSave.FileName
                Return True
            Else
                Return SaveAs
            End If
        Else
            Return False
        End If
    End Function
    ''' <summary>Uloží soubor pod daným názvel</summary>
    ''' <param name="FileName">Cesta k souboru</param>
    ''' <returns>True pokud se uložení podařilo</returns>
    Private Function Save(ByVal FileName As String) As Boolean
        Dim doc As New Xml.XmlDocument
        doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", ""))
        Dim EquasNode As Xml.XmlElement = doc.AppendChild(doc.CreateElement(XMLEquas))
        For Each equa As Equation In Equas
            EquasNode.AppendChild(equa.ToXml(doc))
        Next equa
        Try
            doc.Save(FileName)
        Catch ex As Exception
            MsgBox("Soubor " & FileName & "se nepodařilo uložit:" & vbCrLf & ex.Message, MsgBoxStyle.Critical, "Chyba")
            Return False
        End Try
        Changed = False
        Return True
    End Function
    ''' <summary>Zeptá se jestli chce uživatel uložit změny (pokud byly nějaké provedeny)</summary>
    ''' <returns>True, pokud uživatel operaci nestornoval</returns>
    Private Function AskChanged() As Boolean
        If Not Changed Then
            Return True
        Else
            Select Case MsgBox("Obsah souboru " & iif(CurrentFile = "", BezNázvuFileName, IO.Path.GetFileName(CurrentFile)) & " byl změněn. Chcete jej nyní uložit?", MsgBoxStyle.YesNoCancel Or MsgBoxStyle.Question, "Uložit změny")
                Case MsgBoxResult.Yes
                    Return Save()
                Case MsgBoxResult.No
                    Return True
                Case MsgBoxResult.Cancel
                    Return False
                Case Else
                    Return False
            End Select
        End If
    End Function

    Private Sub mniOtevřít_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mniOtevřít.Click
        If AskChanged() Then
            If ofdOpen.ShowDialog = Windows.Forms.DialogResult.OK Then
                Dim lst As New List(Of Equation)
                Try
                    Dim doc As New Xml.XmlDocument
                    doc.Load(ofdOpen.FileName)
                    For Each node As Xml.XmlNode In CType(doc.GetElementsByTagName(XMLEquas)(0), Xml.XmlElement).GetElementsByTagName(Equation.XMLEqua)
                        lst.Add(New Equation(node))
                    Next node
                Catch ex As Exception
                    MsgBox("Chyba při otevírání souboru " & ofdOpen.FileName & ":" & ex.Message, MsgBoxStyle.Critical, "Chyba")
                    Return
                End Try
                Equas = lst
                CurrentFile = ofdOpen.FileName
                RCEEnabled = False
                ShowAllEquas()
                RCEEnabled = False
            End If
        End If
    End Sub

    Private Sub mniUložit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mniUložit.Click
        Save()
    End Sub

    Private Sub mniUložitJako_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mniUložitJako.Click
        SaveAs()
    End Sub

    Private Sub mniNový_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mniNový.Click
        If AskChanged() Then
            Equas = New List(Of Equation)
            CurrentFile = ""
            ShowAllEquas()
            RCEEnabled = False
        End If
    End Sub

    Private Sub tsbDel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbDel.Click
        For Each itm As ToolStripButton In tosRovnice.Items
            If itm.Checked Then
                tosRovnice.Items.Remove(itm)
                Equas.Remove(itm.Tag)
                Changed = True
                RCEEnabled = False
                Exit For
            End If
        Next itm
    End Sub

    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = Not AskChanged()
    End Sub
#Region "Validators"
    Private Sub tst_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles tstXmin.TextChanged, tstPPy.TextChanged, tstXmax.TextChanged, tstYmin.TextChanged, tstYmax.TextChanged, tstPPx.TextChanged, tstPPz.TextChanged, tstPPu.TextChanged, tstTmax.TextChanged, tstTmin.TextChanged, tstΔt.TextChanged, _
             tstNázev.TextChanged, tstDx.TextChanged, tstDy.TextChanged, tstDu.TextChanged, tstDz.TextChanged
        With CType(sender, ToolStripTextBox)
            .Tag = enmChState.Changed
        End With
    End Sub

    Private Sub tstNumber_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles tstXmin.Validating, tstPPy.Validating, tstXmax.Validating, tstYmin.Validating, tstYmax.Validating, tstPPx.Validating, tstPPz.Validating, tstPPu.Validating, tstTmin.Validating, tstTmax.Validating, tstΔt.Validating
        With CType(sender, ToolStripTextBox)
            Try
                Dim x As Single = CDbl(.Text)
            Catch ex As Exception
                e.Cancel = True
                MsgBox("Zadaná hodnota musí být číslo", MsgBoxStyle.Critical, "Chyba")
                .Tag = enmChState.NotValidated
                Return
            End Try
        End With
    End Sub

    Private Sub tst_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles tstXmin.GotFocus, tstPPy.GotFocus, tstXmax.GotFocus, tstYmin.GotFocus, tstYmax.GotFocus, tstPPz.GotFocus, tstPPx.GotFocus, tstPPu.GotFocus, tstTmin.GotFocus, tstTmax.GotFocus, tstΔt.GotFocus, _
             tstNázev.GotFocus, tstDx.GotFocus, tstDy.GotFocus, tstDz.GotFocus, tstDu.GotFocus
        With CType(sender, ToolStripTextBox)
            If .Tag Is Nothing OrElse (TypeOf .Tag Is enmChState AndAlso CType(.Tag, enmChState) <> enmChState.NotValidated) Then
                .Tag = enmChState.NotChanged
            ElseIf TypeOf .Tag Is enmChState AndAlso CType(.Tag, enmChState) = enmChState.NotValidated Then
                .Tag = enmChState.Changed
            Else
                .Tag = enmChState.Changed
            End If
        End With
    End Sub

    Private Sub tst_Validated(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles tstXmin.Validated, tstPPy.Validated, tstXmax.Validated, tstYmin.Validated, tstYmax.Validated, tstPPx.Validated, tstPPx.Validated, tstPPy.Validated, tstTmin.Validated, tstTmax.Validated, tstΔt.Validated, _
             tstNázev.Validated, tstDx.Validated, tstDy.Validated, tstDz.Validated, tstDu.Validated
        With CType(sender, ToolStripTextBox)
            If (TypeOf .Tag Is enmChState AndAlso CType(.Tag, enmChState) <> enmChState.NotChanged) OrElse Not TypeOf .Tag Is enmChState Then
                StoreTextBoxes()
            End If
            If sender Is tstXmin OrElse sender Is tstXmax OrElse sender Is tstYmin OrElse sender Is tstYmax Then
                DrawAxes()
            ElseIf sender Is tstNázev Then
                CurrentEqua.Text = .Text
            End If
        End With
    End Sub
    ''' <summary>Vykreslí osy</summary>
    Private Sub DrawAxes()
        Try
            picMain.BackgroundImage = Drawer.DrawAxes( _
                    tstXmin.Text, tstYmin.Text, tstXmax.Text, tstYmax.Text, picMain.ClientSize.Width, picMain.ClientSize.Height)
        Catch
            picMain.BackgroundImage = Nothing
        End Try
    End Sub
    ''' <summary>Možné stavy textového pole</summary>
    Private Enum enmChState
        ''' <summary>Nedošlo ke změně</summary>
        NotChanged
        ''' <summary>Hodnota v pole nebyla validována</summary>
        NotValidated
        ''' <summary>Došlo ke změně</summary>
        Changed
    End Enum

    Private Sub tstString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles tstNázev.Validating, tstDx.Validating, tstDy.Validating, tstDz.Validating, tstDy.Validating
        With CType(sender, ToolStripTextBox)
            If .Text = "" Then
                e.Cancel = True
                MsgBox("Text musí být zadán", MsgBoxStyle.Critical, "Chyba")
                .Tag = enmChState.NotValidated
            End If
        End With
    End Sub
#End Region
    ''' <summary>Uloží hodnoty s textboxů k rovnici</summary>
    Private Sub StoreTextBoxes()
        Changed = True
        With CType(CurrentEqua.Tag, Equation)
            .Name = tstNázev.Text
            .Dx = tstDx.Text
            .Dy = tstDy.Text
            .Dz = tstDz.Text
            .Du = tstDu.Text
            .StartX = tstPPx.Text
            .StartY = tstPPy.Text
            .StartU = tstPPu.Text
            .StartZ = tstPPz.Text
            .MaxX = tstXmax.Text
            .MaxY = tstYmax.Text
            .MinY = tstYmin.Text
            .MinX = tstXmin.Text
            .Tmax = tstTmax.Text
            .Tmin = tstTmin.Text
            .Δt = tstΔt.Text
            .DifSch = tscDifSch.SelectedIndex
            .AxeX = AxeX
            .AxeY = AxeY
        End With
    End Sub
    ''' <summary>POdle stavu tlačítek zjistzí vybrané přiřazení osy x</summary>
    Private ReadOnly Property AxeX() As Equation.enmAxes
        Get
            Select Case True
                Case tsbXX.Checked
                    Return Equation.enmAxes.x
                Case tsbXY.Checked
                    Return Equation.enmAxes.y
                Case tsbXZ.Checked
                    Return Equation.enmAxes.z
                Case tsbXU.Checked
                    Return Equation.enmAxes.u
                Case tsbXt.Checked
                    Return Equation.enmAxes.t
            End Select
        End Get
    End Property
    ''' <summary>POdle stavu tlačítek zjistzí vybrané přiřazení osy y</summary>
    Private ReadOnly Property AxeY() As Equation.enmAxes
        Get
            Select Case True
                Case tsbYX.Checked
                    Return Equation.enmAxes.x
                Case tsbYY.Checked
                    Return Equation.enmAxes.y
                Case tsbYZ.Checked
                    Return Equation.enmAxes.z
                Case tsbYU.Checked
                    Return Equation.enmAxes.u
                Case tsbYt.Checked
                    Return Equation.enmAxes.t
            End Select
        End Get
    End Property

    Private Sub tmiAbout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmiAbout.Click
        frmAbout.ShowDialog(Me)
    End Sub

    Private Sub tmiVykreslit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmiVykreslit.Click
        Draw()
    End Sub
    ''' <summary>Vykreslí požadovanou rovnici</summary>
    ''' <param name="X">Počáteční hodnota pro osu mapovanou na osu x v souřadnicích pictureboxu</param>
    ''' <param name="Y">Počáteční hodnota pro osu mapovanou na osu y v souřadnicích pictureboxu</param>
    ''' <param name="Target">Cílový <see cref="PictureBox"/>, pokud je null použije se <see cref="picMain"/></param>
    ''' <param name="Owner">Vlastnický formulář, pokud je nothing použije se aktuální instance</param>
    ''' <param name="Axes">Vykreslit osy?</param>
    ''' <param name="Quick">Pokud True nebude pauza a nepoužije duplicitní grafiku</param>
    ''' <returns>True pokud vykreslení proběhlo bez chyb</returns>
    Private Function Draw(Optional ByVal X As Integer = -1, Optional ByVal Y As Integer = -1, _
            Optional ByVal Target As PictureBox = Nothing, Optional ByVal Owner As Form = Nothing, Optional ByVal Axes As Boolean = True, Optional ByVal Quick As Boolean = False) As Boolean
        If Owner Is Nothing Then Owner = Me
        Owner.Cursor = Cursors.WaitCursor
        If Target Is Nothing Then Target = picMain
        Draw = False
        Try
            Dim minX As Single = tstXmin.Text
            Dim miny As Single = tstYmin.Text
            Dim maxX As Single = tstXmax.Text
            Dim maxY As Single = tstYmax.Text
            Dim maxT As Single = tstTmax.Text
            Dim minT As Single = tstTmin.Text
            Dim Δt As Single = tstΔt.Text
            Dim startX As Single, startY As Single, startZ As Single, startU As Single, startT As Single

            Try
                startX = tstPPx.Text
                startY = tstPPy.Text
                startZ = tstPPz.Text
                startU = tstPPu.Text
                startT = tstTmin.Text
            Catch ex As Exception
                MsgBox("Chyba při převodu počáteční podmínky nebo času:" & vbCrLf & ex.Message, MsgBoxStyle.Critical, "Chyba")
                Exit Function
            End Try

            If X >= 0 AndAlso Y >= 0 Then 'Počáteční hodnoty z kliknutí nikoliv z textboxů
                Dim pt As PointF = CalcXY(New Point(X, Y))
                Select Case AxeX
                    Case Equation.enmAxes.t
                        startT = pt.X
                    Case Equation.enmAxes.u
                        startU = pt.X
                    Case Equation.enmAxes.x
                        startX = pt.X
                    Case Equation.enmAxes.y
                        startY = pt.X
                    Case Equation.enmAxes.z
                        startZ = pt.X
                End Select
                Select Case AxeY
                    Case Equation.enmAxes.t
                        startT = pt.Y
                    Case Equation.enmAxes.u
                        startU = pt.Y
                    Case Equation.enmAxes.x
                        startX = pt.Y
                    Case Equation.enmAxes.y
                        startY = pt.Y
                    Case Equation.enmAxes.z
                        startZ = pt.Y
                End Select
            End If
            'Kontrola
            If minX >= maxX Then
                MsgBox("Minimum osy X musí být menší než maximum osy X", MsgBoxStyle.Critical, "Chyba")
                Exit Function
            ElseIf miny >= maxY Then
                MsgBox("Minimum osy Y musí být menší než maximum osy Y", MsgBoxStyle.Critical, "Chyba")
                Exit Function
            ElseIf maxT < startT AndAlso Δt > 0 Then
                MsgBox("Když je Δt kladná, musí být maximum t větší nebo rovno minimu t", MsgBoxStyle.Critical, "Chyba")
                Exit Function
            ElseIf maxT > startT AndAlso Δt < 0 Then
                MsgBox("Když je Δt záporná, musí být maximum t menší nebo rovno minimu t", MsgBoxStyle.Critical, "Chyba")
                Exit Function
            ElseIf Δt = 0 Then
                MsgBox("Δt nemůže být 0", MsgBoxStyle.Critical, "Chyba")
                Exit Function
            End If
            'Inicializace vykreslování
            Dim r As RectangleF
            r.X = minX
            r.Y = maxY
            r.Width = maxX - minX
            r.Height = -(maxY - miny)
            Dim VarArr() As String = {"x", "y", "z", "u", "t"}
            Dim dr As Drawer
            Try
                dr = New Drawer( _
                        r, _
                        New PointF(startX, startY), New PointF(startZ, startU), _
                        New SyntaktickyAnalyzator.Analyzer(tstDx.Text, VarArr), _
                        New SyntaktickyAnalyzator.Analyzer(tstDy.Text, VarArr), _
                        New SyntaktickyAnalyzator.Analyzer(tstDz.Text, VarArr), _
                        New SyntaktickyAnalyzator.Analyzer(tstDu.Text, VarArr), _
                        Target.ClientSize, tstTmax.Text, tstΔt.Text, _
                        tscDifSch.SelectedIndex, AxeX, AxeY, _
                        startT)
            Catch ex As SyntaktickyAnalyzator.SynAnalyzer.WrongSyntaxException
                MsgBox("Syntaxe rovnice je nesprávná:" & vbCrLf & ex.Message, MsgBoxStyle.Critical, "Chyba")
                Exit Function
            Catch ex As Exception
                MsgBox("Během inicializace vykreslování došlo k chybě:" & vbCrLf & ex.Message, MsgBoxStyle.Critical, "Chyba")
                Exit Function
            End Try
            If Not Quick Then
                If X >= 0 AndAlso Y >= 0 Then dr.AdoptImage(Target.BackgroundImage)
                dr.SecondaryGraphics = Target.CreateGraphics
                dr.DrawStepPause = nudWait.Value
            End If
            'Vykreslení
            Try
                Target.BackgroundImage = dr.Draw(Axes)
                Draw = True
            Catch ex As Exception
                MsgBox("Chyba při vykreslování:" & vbCrLf & ex.Message, MsgBoxStyle.Critical, "Chyba")
            End Try
        Finally
            Owner.Cursor = Cursors.Default
        End Try
    End Function

    Private Sub picMain_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles picMain.MouseMove
        Dim pt As PointF = CalcXY(e.Location)
        tslX.Text = pt.X
        tslY.Text = pt.Y
    End Sub
    ''' <summary>Přepočítá souřadnice PictureBoxu na souřadnice světa</summary>
    Private Function CalcXY(ByVal pt As Point) As PointF
        Try
            Dim TrnsMx As New Matrix( _
                    New RectangleF(tstXmin.Text, tstYmax.Text, CSng(tstXmax.Text) - tstXmin.Text, -(CSng(tstYmax.Text) - tstYmin.Text)), _
                    New PointF() { _
                    New PointF(0, 0), New PointF(picMain.ClientSize.Width, 0), New PointF(0, picMain.ClientSize.Height)})
            TrnsMx.Invert()
            Dim pts As PointF() = {pt}
            TrnsMx.TransformPoints(pts)
            Return pts(0)
        Catch
        End Try
    End Function

    Private Sub picMain_EnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles picMain.EnabledChanged
        If picMain.Enabled Then
            picMain.BackColor = Color.Black
        Else
            picMain.BackColor = Color.Gray
        End If
    End Sub

    Private Sub picMain_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles picMain.MouseClick
        Draw(e.X, e.Y)
    End Sub

    Private Sub tosOsy_ItemClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tosOsy.ItemClicked
        If TypeOf e.ClickedItem Is ToolStripButton Then
            If e.ClickedItem Is tsbXX OrElse e.ClickedItem Is tsbXY OrElse e.ClickedItem Is tsbXZ OrElse e.ClickedItem Is tsbXU OrElse e.ClickedItem Is tsbXt Then
                tsbXX.Checked = False
                tsbXY.Checked = False
                tsbXZ.Checked = False
                tsbXU.Checked = False
                tsbXt.Checked = False
            Else
                tsbYX.Checked = False
                tsbYY.Checked = False
                tsbYZ.Checked = False
                tsbYU.Checked = False
                tsbYt.Checked = False
            End If
            CType(e.ClickedItem, ToolStripButton).Checked = True
            StoreTextBoxes()
        End If
    End Sub

    Private Sub tmiVyčistit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmiVyčistit.Click
        DrawAxes()
    End Sub

    Private Sub tmiUložitObrázek_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmiUložitObrázek.Click
        SaveImage()
    End Sub
    ''' <summary>Uloží obrázek do souboru</summary>
    ''' <param name="image">Obrázek k uložení. Pokud je null použijese <see cref="PictureBox.BackgroundImage"/> z <see cref="picMain"/></param>
    Private Function SaveImage(Optional ByVal image As Image = Nothing) As Boolean
        If image Is Nothing Then image = picMain.BackgroundImage
        SaveImage = False
        If sfdImage.ShowDialog = Windows.Forms.DialogResult.OK Then
            Try
                Dim format As Imaging.ImageFormat
                Select Case IO.Path.GetExtension(sfdImage.FileName).ToLower
                    Case ".png" : format = Imaging.ImageFormat.Png
                    Case ".gif" : format = Imaging.ImageFormat.Gif
                    Case ".jpg", ".jpeg", ".jpe" : format = Imaging.ImageFormat.Jpeg
                    Case ".bmp", ".dib" : format = Imaging.ImageFormat.Bmp
                    Case ".tif", ".tiff" : format = Imaging.ImageFormat.Tiff
                    Case Else : format = Imaging.ImageFormat.Bmp
                End Select
                image.Save(sfdImage.FileName, format)
                SaveImage = True
            Catch ex As Exception
                MsgBox("Obrázek se nepodařilo uložit:" & vbCrLf & ex.Message, MsgBoxStyle.Critical, "Chyba")
            End Try
        End If
    End Function
#Region "Vykreslit do obrázku"
    ''' <summary>Dialog pro vykreslení do obrázku</summary>
    Private frmVyk As frmImage
    Private Sub tmiVykreslitDoSouboru_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmiVykreslitDoSouboru.Click
        frmVyk = New frmImage
        AddHandler frmVyk.cmdOK.Click, AddressOf frmVyk_cmdOk_Click
        AddHandler frmVyk.cmdNáhled.Click, AddressOf frmVyk_cmdNáhled_Click
        frmVyk.ShowDialog(Me)
        RemoveHandler frmVyk.cmdOK.Click, AddressOf frmVyk_cmdOk_Click
        RemoveHandler frmVyk.cmdNáhled.Click, AddressOf frmVyk_cmdNáhled_Click
        frmVyk = Nothing
    End Sub
    Private Sub frmVyk_cmdOk_Click(ByVal sender As Object, ByVal e As [EventArgs])
        If SaveImage(frmVyk.picMain.BackgroundImage) Then
            frmVyk.Close()
        End If
    End Sub
    Private Sub frmVyk_cmdNáhled_Click(ByVal sender As Object, ByVal e As [EventArgs])
        frmVyk.picMain.ClientSize = New Size(frmVyk.nudŠířka.Value, frmVyk.nudVýška.Value)
        frmVyk.cmdOK.Enabled = Draw(, , frmVyk.picMain, frmVyk, frmVyk.chkOsy.Checked, True)
    End Sub
#End Region

    Private Sub KopírovatToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmiCopy.Click
        My.Computer.Clipboard.SetImage(picMain.BackgroundImage)
    End Sub
End Class


