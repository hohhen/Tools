﻿Imports System.ComponentModel, Tools.CollectionsT.GenericT
Imports System.ComponentModel.Design
Imports System.Drawing.Design

#If Config <= Nightly Then 'Stage: Nightly
Imports System.Windows.Forms
Namespace WindowsT.NativeT
    'ASAP: Mark, Wiki, Forum
    ''' <summary>Represents native window used in Microsoft Windows</summary>
    ''' <remarks>This class can be used to manipulate windows and controls that originates from non-.NET applications as well as .NET ones. It can be used in 64b environment as well.</remarks>
    <DebuggerDisplay("{ToString}")> _
    Public Class Win32Window
        Implements IWin32Window, IDisposable, ICloneable(Of IWin32Window), ICloneable(Of Win32Window)
        Implements IEquatable(Of IWin32Window), IEquatable(Of Win32Window), IEquatable(Of Control)
#Region "Basic"
        ''' <summary>Contains value of the <see cref="Handle"/> property</summary>
        Private _Handle As System.IntPtr
#Region "CTors & CTypes"
        ''' <summary>CTor from <see cref="Integer"/> handle</summary>
        ''' <param name="hWnd">Handle to window</param>
        Public Sub New(ByVal hWnd As Integer)
            _Handle = hWnd
        End Sub
        ''' <summary>CTor from <see cref="IntPtr"/> handle</summary>
        ''' <param name="hWnd">Handle to window</param>
        Public Sub New(ByVal hWnd As IntPtr)
            Me.New(CInt(hWnd))
        End Sub
        ''' <summary>CTor from <see cref="Control"/> (including <see cref="Form"/>)</summary>
        ''' <param name="Control">Control to create new instance from</param>
        Public Sub New(ByVal Control As Control)
            Me.New(Control.Handle)
        End Sub
        ''' <summary>CTor from <see cref="IWin32Window"/></summary>
        ''' <param name="Window">Window to create new instance from</param>
        Public Sub New(ByVal Window As IWin32Window)
            Me.New(Window.Handle)
        End Sub
        ''' <summary>Converts <see cref="Control"/> to <see cref="Win32Window"/></summary>
        ''' <param name="a">A <see cref="Control"/></param>
        ''' <returns>A <see cref="Win32Window"/> with same handle as <paramref name="a"/></returns>
        Public Shared Widening Operator CType(ByVal a As Control) As Win32Window
            Return New Win32Window(a)
        End Operator
#End Region

        ''' <summary>Gets the handle to the window represented by the implementer.</summary>
        ''' <returns>A handle to the window represented by the implementer.</returns>
        <Category("Handle")> _
        Public ReadOnly Property Handle() As System.IntPtr Implements System.Windows.Forms.IWin32Window.Handle 'Localize: Category
            Get
                Return _Handle
            End Get
        End Property
        ''' <summary>Same as <see cref="Handle"/> but <see cref="Integer"/></summary>
        <Category("Handle")> _
        Public ReadOnly Property hWnd() As Integer 'Localize:Category
            Get
                Return Handle
            End Get
        End Property

#Region " IDisposable Support "
        ''' <summary>To detect redundant calls</summary>
        Private disposedValue As Boolean = False

        ''' <summary>Sets <see cref="Handle"/> to zero</summary>
        ''' <remarks>This code added by Visual Basic to correctly implement the disposable pattern.</remarks>
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                End If
            End If
            _Handle = IntPtr.Zero
            Me.disposedValue = True
        End Sub
        ''' <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        ''' <remarks>Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.</remarks>
        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
        ''' <summary>Creates a new object that is a copy of the current instance.</summary>
        ''' <returns>A new object that is a copy of this instance.</returns>
        ''' <remarks>Use type-safe <see cref="Clone"/> instead</remarks>
        <EditorBrowsable(EditorBrowsableState.Never)> _
        <Obsolete("Use type-safe Clone instead")> _
        Private Function Clone__() As Object Implements System.ICloneable.Clone
            Return Clone()
        End Function
        ''' <summary>Implements <see cref="ICloneable(Of System.Windows.Forms.IWin32Window).Clone"/></summary>
        ''' <returns><see cref="Clone"/></returns>
        <EditorBrowsable(EditorBrowsableState.Never)> _
        Private Function Clone_() As System.Windows.Forms.IWin32Window Implements ICloneable(Of System.Windows.Forms.IWin32Window).Clone
            Return Clone()
        End Function
        ''' <summary>Creates new instance of <see cref="Win32Window"/> pointing to same window as curent instance</summary>
        ''' <returns>New instance pointing to same window as current instance</returns>
        ''' <remarks>In fact there is no need to clone <see cref="Win32Window"/> object, because it has no internal state other than <see cref="Handle"/></remarks>
        Public Function Clone() As Win32Window Implements ICloneable(Of Win32Window).Clone
            Return New Win32Window(Handle)
        End Function
#End Region
#Region "Basic properties and operations"
        ''' <summary>Gets or sets parent of current Window</summary>
        ''' <value>A <see cref="Win32Window"/> to reparent current window into. Can be null to un-parent current window completely.</value>
        ''' <returns>Current parent of current window. Can return null if current window has no parent or there was error when obtaining parent (ie. <see cref="Handle"/> is invalid)</returns>
        ''' <exception cref="API.Win32APIException">Setting failed. It may indicate that <see cref="hWnd"/> does not point to existing window or attempt to set parent to the same window or to one of children.</exception>
        ''' <remarks>Setting value to <see cref="Win32Window"/> with <see cref="Handle"/> of zero has the same effect as setting it to null.
        ''' Non-top-level windows (such as button) cannot be unparented. Setting null for shuch window will cause window to be parented into desktop - not by this implementation but by the OS.</remarks>
        <Category("Relationship")> _
        Public Property Parent() As Win32Window 'Localize:Category
            Get
                Dim ret As Integer = API.GetParent(hWnd)
                Return If(ret <> 0, New Win32Window(ret), Nothing)
            End Get
            Set(ByVal value As Win32Window)
                If Not API.SetParent(hWnd, If(value Is Nothing, 0, value.hWnd)) Then _
                    Throw New API.Win32APIException()
            End Set
        End Property
        ''' <summary>Adds <paramref name="item"/> to <paramref name="List"/> and returns true</summary>
        ''' <param name="List"><see cref="List(Of T)"/> to add item to</param>
        ''' <param name="item">Item to be added</param>
        ''' <typeparam name="T">Type of <paramref name="item"/></typeparam>
        ''' <returns>True</returns>
        Private Shared Function AddToList(Of T)(ByVal List As List(Of T), ByVal item As T) As Boolean
            List.Add(item)
            Return True
        End Function
        ''' <summary>Gets all childrens of current windows</summary>
        ''' <returns>Childrens of current window</returns>
        ''' <exception cref="API.Win32APIException">Error while enumerating windows. Ie. <see cref="Handle"/> is invalid</exception>
        <Category("Relationship")> _
        <TypeConverter(GetType(CollectionConverter))> _
        Public ReadOnly Property Children() As IReadOnlyList(Of Win32Window) 'Localize: description
            Get
                Dim List As New List(Of Win32Window)
                If API.EnumChildWindows(hWnd, New API.EnumWindowsProc(Function(hWnd As Integer, lParam As Integer) AddToList(List, New Win32Window(hWnd))), 0) Then
                    Return New ReadOnlyListAdapter(Of Win32Window)(List)
                Else
                    Dim ex As New API.Win32APIException
                    If ex.NativeErrorCode <> 0 Then
                        Throw ex
                    Else
                        Return New ReadOnlyListAdapter(Of Win32Window)(List)
                    End If
                End If
            End Get
        End Property
        ''' <summary>Gets or sets handle of current window's parent</summary>
        ''' <value>Handle to window to parent current window into. Set to 0 if window should be parented into desktop.</value>
        ''' <returns>Handle of current window's parent. Zero if current window has no parent.</returns>
        ''' <exception cref="API.Win32APIException">Error when setting parent. It may be caused by invalid <see cref="Handle"/> or invalid <see cref="ParentHandle"/> being set</exception>
        ''' <remarks>It's recomended to use <see cref="Parent"/> instead.
        ''' Non-top-level windows (such as button) cannot be unparented. Setting zero for shuch window will cause window to be parented into desktop - not by this implementation but by the OS.</remarks>
        <EditorBrowsable(EditorBrowsableState.Advanced)> _
        <Category("Relationship")> _
        Public Property ParentHandle() As IntPtr 'Localize:Category
            Get
                Return API.GetParent(hWnd)
            End Get
            Set(ByVal value As IntPtr)
                If Not API.SetParent(hWnd, value) Then _
                    Throw New API.Win32APIException
            End Set
        End Property
        ''' <summary>Gets or sets specified window long of current window</summary>
        ''' <param name="Long">Long to get or set. Can be one of <see cref="API.[Public].WindowLongs"/> values or can be any user-defined integer</param>
        ''' <value>New value of window long</value>
        ''' <returns>Current value of window long</returns>
        ''' <exception cref="API.Win32APIException">Getting or setting of value failed (i.e. <see cref="Handle"/> is invalid or <paramref name="Long"/> is invalid)</exception>
        <Category("Low-level")> _
        Public Property WindowLong(ByVal [Long] As API.Public.WindowLongs) As Integer 'Localize:Category
            Get
                Try
                    Return API.GetWindowLong(hWnd, CType([Long], API.WindowLongs))
                Finally
                    Dim ex As New API.Win32APIException
                    If ex.NativeErrorCode <> 0 Then Throw ex
                End Try
            End Get
            Set(ByVal value As Integer)
                If API.SetWindowLong(hWnd, [Long], value) = 0 Then
                    Dim ex As New API.Win32APIException
                    If ex.NativeErrorCode <> 0 Then Throw ex
                End If
            End Set
        End Property
#Region "Size & location"
        'ASAP: Comment, do not forget exceptions
        ''' <summary>Changes window position and size</summary>
        ''' <param name="Height">New height of window in px</param>
        ''' <param name="Left">New x coordinate of left edge of the window in px</param>
        ''' <param name="Repaint">Forces window to repaint its content after moving - default is true</param>
        ''' <param name="Top">New y coordinate of top edge of the window in px</param>
        ''' <param name="Width">New width of window in px</param>
        ''' <exception cref="API.Win32APIException">Moving failed, ie. <see cref="Handle"/> is invalid</exception>
        ''' <remarks>
        ''' In some multi-monitor configurations the <paramref name="Top"/> and <see cref="Left"/> can be negative and it does not necesarilly mean that window is positioned outside the desktop.
        ''' For top-level windows screen coordinates are used. For windows with <see cref="Parent"/> parent's coordinates are used.
        ''' </remarks>
        Public Sub Move(ByVal Left As Integer, ByVal Top As Integer, ByVal Width As Integer, ByVal Height As Integer, Optional ByVal Repaint As Boolean = True)
            If Not API.MoveWindow(hWnd, Left, Top, Width, Height, Repaint) Then _
                Throw New API.Win32APIException
        End Sub
        ''' <summary>Changes window position and size to specified <see cref="Rectangle"/></summary>
        ''' <param name="Rectangle">Defines new window size and position</param>
        ''' <remarks><paramref name="Rectangle"/>.<see cref="Rectangle.Location">Location</see> should be in screen coordibates for top-level windows and in parent's coordinates for windows with <see cref="Parent"/></remarks>
        ''' <exception cref="API.Win32APIException">Moving failed, ie. <see cref="Handle"/> is invalid</exception>
        Public Sub Move(ByVal Rectangle As Rectangle)
            Move(Rectangle.Left, Rectangle.Top, Rectangle.Width, Rectangle.Height)
        End Sub
        ''' <summary>Gets or sets rectangle covered by the window</summary>
        ''' <returns>Current rectangle covered by the window</returns>
        ''' <value>New rectangle covered by the window</value>
        ''' <remarks>For top-level windows screen coordinates are used. For windows with <see cref="Parent"/> coordinates of parent are used.</remarks>
        ''' <exception cref="API.Win32APIException">Setting or obtaining window's rectangle failed, ie. <see cref="Handle"/> is invalid</exception>
        <Category("Size and position")> _
        Public Property Area() As Rectangle 'Localize: Category
            Get
                Dim ret As API.RECT
                If API.GetWindowRect(hWnd, ret) Then
                    If Parent IsNot Nothing Then
                        Dim pos As API.POINTAPI = CType(ret, Rectangle).Location
                        If API.ScreenToClient(Parent.hWnd, pos) Then
                            Return New Rectangle(pos, CType(ret, Rectangle).Size)
                        Else
                            Throw New API.Win32APIException
                        End If
                    Else
                        Return ret
                    End If
                Else
                    Throw New API.Win32APIException
                End If
            End Get
            Set(ByVal value As Rectangle)
                Move(value)
            End Set
        End Property
        ''' <summary>Gets or sets the size of the window</summary>
        ''' <value>New size of the window. Position will be unchanged</value>
        ''' <returns>Current size of the window</returns>
        ''' <exception cref="API.Win32APIException">Setting or obtaining of window's rectangle failed, ie. <see cref="Handle"/> is invalid</exception>
        <Category("Size and position")> _
        Public Property Size() As Size 'Localize: Category
            Get
                Return Area.Size
            End Get
            Set(ByVal value As Size)
                Area = New Rectangle(Location, value)
            End Set
        End Property
        ''' <summary>Gets or sets location of the window</summary>
        ''' <value>New position of top left corner of window. Size will ne unchanged.</value>
        ''' <returns>Current position of window top left corner</returns>
        ''' <remarks>For top-level windows the location is in screen coordinates, for windows with <see cref="Parent"/> in parent' coordinates.</remarks>
        ''' <exception cref="API.Win32APIException">Setting or obtaining of window's rectangle failed, ie. <see cref="Handle"/> is invalid</exception>
        <Category("Size and position")> _
        Public Property Location() As Point 'Localize:Category
            Get
                Return Area.Location
            End Get
            Set(ByVal value As Point)
                Area = New Rectangle(value, Size)
            End Set
        End Property
        ''' <summary>Gets or sets x coordinale of left edge of the window.</summary>
        ''' <value>New x coordinate of left edge of the window</value>
        ''' <returns>Current x coordinate of left edge of the window</returns>
        ''' <remarks>In some multi-monitor configurations the left edge of desktop can be negative number. In such case <see cref="Left"/> can be also negative and it does not necesarilly mean that the window is outside of the desktop.
        ''' For top-level windows the location is in screen coordinates, for windows with <see cref="Parent"/> in parent' coordinates.</remarks>
        ''' <exception cref="API.Win32APIException">Setting or obtaining of window's rectangle failed, ie. <see cref="Handle"/> is invalid</exception>
        <Category("Size and position")> _
        Public Property Left() As Integer 'Localize:Category
            Get
                Return Location.X
            End Get
            Set(ByVal value As Integer)
                Location = New Point(value, Top)
            End Set
        End Property
        ''' <summary>Gets or sets y coordinate of top edge of the window.</summary>
        ''' <value>New y coordinate of top edge of the window</value>
        ''' <returns>Current y coordinate of top edge of the window</returns>
        ''' <remarks>In some multi-monitor configurations the top edge of desktop can be negative number. In such case <see cref="Top"/> can be also negative and it does not necesarilly mean thet the window is outside of the desktop.
        ''' For top-level windows the location is in screen coordinates, for windows with <see cref="Parent"/> in parent' coordinates.</remarks>
        ''' <exception cref="API.Win32APIException">Setting or obtaining of window's rectangle failed, ie. <see cref="Handle"/> is invalid</exception>
        <Category("Size and position")> _
        Public Property Top() As Integer 'Localize:Category
            Get
                Return Location.Y
            End Get
            Set(ByVal value As Integer)
                Location = New Point(Left, value)
            End Set
        End Property
        ''' <summary>Gets or sets width of the window</summary>
        ''' <value>New width of the window</value>
        ''' <returns>Current width of the window</returns>
        ''' <exception cref="API.Win32APIException">Setting or obtaining of window's rectangle failed, ie. <see cref="Handle"/> is invalid</exception>
        <Category("Size and position")> _
        Public Property Width() As Int32 'Localize:Category
            Get
                Return Size.Width
            End Get
            Set(ByVal value As Integer)
                Size = New Size(value, Height)
            End Set
        End Property
        ''' <summary>Gets or sets height of the window</summary>
        ''' <value>New height of the window</value>
        ''' <returns>Current height of the window</returns>
        ''' <exception cref="API.Win32APIException">Setting or obtaining of window's rectangle failed, ie. <see cref="Handle"/> is invalid</exception>
        <Category("Size and position")> _
        Public Property Height() As Integer 'Localize:Category
            Get
                Return Size.Height
            End Get
            Set(ByVal value As Integer)
                Size = New Size(Width, value)
            End Set
        End Property
        ''' <summary>Gets x coordinate of right edge of the window</summary>
        ''' <returns>Current x-coordinate of right edge of the window</returns>
        ''' <remarks>For top-level windows the location is in screen coordinates, for windows with <see cref="Parent"/> in parent' coordinates.</remarks>
        ''' <exception cref="API.Win32APIException">Obtaining of window's rectangle failed, ie. <see cref="Handle"/> is invalid</exception>
        <Category("Size and position")> _
        Public ReadOnly Property Right() As Integer 'Localize:Category
            Get
                Return Area.Right
            End Get
        End Property
        ''' <summary>Gets y coordinate of bottom edge of the window</summary>
        ''' <returns>Current y-coordinate of bottom edge of the window</returns>
        ''' <remarks>For top-level windows the location is in screen coordinates, for windows with <see cref="Parent"/> in parent' coordinates.</remarks>
        ''' <exception cref="API.Win32APIException">Obtaining of window's rectangle failed, ie. <see cref="Handle"/> is invalid</exception>
        <Category("Size and position")> _
        Public ReadOnly Property Bottom() As Integer 'Localize:Category
            Get
                Return Area.Bottom
            End Get
        End Property
        ''' <summary>Gets or sets window area in screen coordinates (even for non-top-level windows)</summary>
        ''' <returns>Current area that windows covers on screen</returns>
        ''' <value>New area to cover</value>
        ''' <exception cref="API.Win32APIException">Error while setting or obtaining the area (ie. <see cref="Handle"/> is invalid)</exception>
        <Category("Size and position")> _
        Public Property ScreenArea() As Rectangle 'Localize:Category
            Get
                Dim ret As API.RECT
                If API.GetWindowRect(hWnd, ret) Then
                    Return ret
                Else
                    Throw New API.Win32APIException
                End If
            End Get
            Set(ByVal value As Rectangle)
                If Parent IsNot Nothing Then
                    Dim pos As API.POINTAPI = value.Location
                    If API.ScreenToClient(Parent.hWnd, pos) Then
                        value.Location = pos
                    Else
                        Throw New API.Win32APIException
                    End If
                End If
                Move(value)
            End Set
        End Property
#End Region
        ''' <summary>Gets or sets text associated with the window</summary>
        ''' <value>New text of window</value>
        ''' <returns>Current text of the window</returns>
        ''' <remarks>For windows that represents form it is text from title bar, for other controls like labels it is text of the control. See also <seealso cref="Control.Text"/>.
        ''' This property can can get/set text for all windows in the same process as it is called from and text of windows that has title bar (forms) from any process.</remarks>
        ''' <exception cref="API.Win32APIException">Setting or obtaining of text failed. ie. <see cref="Handle"/> is invalid</exception>
        <Category("Window properties")> _
        Public Property Text$() 'Localize: category
            Get
                Dim len As Integer = API.GetWindowTextLength(hWnd)
                If len > 0 Then
                    Dim b As New System.Text.StringBuilder(ChrW(0), len + 1)
                    Dim ret = API.GetWindowText(hWnd, b, b.Capacity)
                    If ret <> 0 Then
                        Return b.ToString
                    Else
                        Throw New API.Win32APIException
                    End If
                Else
                    Dim ex As New API.Win32APIException
                    If ex.NativeErrorCode <> 0 Then
                        Throw New API.Win32APIException
                    Else
                        Return ""
                    End If
                End If
            End Get
            Set(ByVal value$)
                If Not API.SetWindowText(hWnd, value) Then _
                    Throw New Win32Exception
            End Set
        End Property
        ''' <summary>Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.</summary>
        ''' <returns>A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.</returns>
        Public Overrides Function ToString() As String
            If hWnd = 0 Then Return "<no window>"
            Try : Return String.Format("{0} hWnd = {1}", Text, hWnd) : Catch ex As Win32Exception : Return String.Format("hWnd = {0}", hWnd) : End Try
        End Function
        ''' <summary>Gets or sets pointer to wnd proc of current window. Used for so-called sub-classing.</summary>
        ''' <returns>Pointer to current wnd proc of current window</returns>
        ''' <value>Pointer to new wnd proc. Note: Old wnd proc is lost when setting this property. You should consider backing old value up.</value>
        ''' <remarks>
        ''' wnd proc (window procedure) is procedure with signature of th <see cref="API.Messages.WndProc"/> delegate that processes all the messages. You should consider using <see cref="WndProc"/> property rather then this one.
        ''' You can do this also with <see cref="WindowLong"/> with <see cref="API.[Public].WindowLongs.WndProc"/> as argument.
        ''' </remarks>
        ''' <exception cref="API.Win32APIException">Getting or setting of value failed (i.e. <see cref="Handle"/> is invalid)</exception>
        <Category("Low-level")> _
        <Browsable(False), EditorBrowsable(EditorBrowsableState.Advanced)> _
        Public Overridable Property WndProcPointer() As IntPtr  'Localize: category
            Get
                Return WindowLong(API.Public.WindowLongs.WndProc)
            End Get
            Set(ByVal value As IntPtr)
                WindowLong(API.Public.WindowLongs.WndProc) = value
            End Set
        End Property
        ''' <summary>Gets or sets wnd proc of current window. Used for so-called window sub-classing.</summary>
        ''' <value>New window proc. Note: Old window proc is lost by setting this property. You should consider backing it up.
        ''' <para>Warning: By setting value of this property youar passing delegate to unmanaged code! You must keep that delegate alive as long as it is in use - that means while the window exists or until <see cref="WndProc"/> property is changed again. For example following VB code is completely invalid!</para>
        ''' <example><code>instance.WndProc = AddressOf MyReplacementProc</code></example>
        ''' <para>This example creates new delegate, passes it to unmanaged code, and forgets it. The is no reference to that delegate keeping it alive (protecting it from being garbage collected), so you can get unexpected error when the runtime garbage collector collects the delegate and the there is an attempt to call it. The proper way of setting this property is create an instance of <see cref="API.Messages.WndProc"/>, store it somewhere, pass it here and keep that 'somewhere' alive as long as window uses that replaced wnd proc.</para>
        ''' <para>The need to keep delegate alive may be problem when creating backup of previos window procedure in order to revert change of window procedure in the future. This property returns a managed delegate (to possibly onmanaged code). So, this delegate must be kept alive as long as it is used by window. That is not always the think you want to (or can) do. In such case you should considering backing up pointer to the old wnd proc. Pointer can be used to restore the procedure with no need to keep it alive. To do so use the <see cref="WndProcPointer"/> property. It is common parctise to backup old wnd proc in order to call it from new one. You cannot call a pointer. So, if you need to back up old wnd proc in order to restore it as well as in order to call it, the best think you can do is back it up as pointer as well as as delegate.</para>
        ''' </value>
        ''' <returns>Delegate to old window proc</returns>
        ''' <exception cref="API.Win32APIException">Getting or setting value failed (i.e. <see cref="Handle"/> is invalid). This is also usually thrown when window comes from another process than property is being got.</exception>
        ''' <remarks>
        ''' Window procedure is used to handle messages of current window.
        ''' <para>If current window represents .NET <see cref="Form"/> or other <see cref="Control"/> and you have chance to derive from it, you'd better to do so and the override <see cref="Control.WndProc"/>.
        ''' You are the proctedted from problems with keeping delegate alive. You can also derive from <see cref="Windows.Forms.NativeWindow"/> and override it's <see cref="Windows.Forms.NativeWindow.WndProc"/>.</para>
        ''' </remarks>
        <Category("Low-level")> _
        Public Overridable Property WndProc() As API.Messages.WndProc 'Localize: Category
            Get
                Dim ret As API.Messages.WndProc = API.GetWindowLong(hWnd, API.WindowProcs.GWL_WNDPROC)
                If ret Is Nothing Then
                    Dim ex As New API.Win32APIException
                    If ex.NativeErrorCode <> 0 Then Throw ex
                End If
                Return ret
            End Get
            Set(ByVal value As API.Messages.WndProc)
                If value Is Nothing Then
                    WndProcPointer = 0
                Else
                    If Not API.SetWindowLong(hWnd, API.WindowProcs.GWL_WNDPROC, value) Then
                        Throw New API.Win32APIException
                    End If
                End If
            End Set
        End Property
        ''' <summary>Gets default window procedure implementation that responds to all messages in defaut way. This implementation is provided by the OS.</summary>
        ''' <returns>Delegate to <see cref="API.DefWindowProc"/> (internal, PInvoke function)</returns>
        Public Shared ReadOnly Property DefWndProc() As API.Messages.WndProc
            Get
                Static ret As API.Messages.WndProc
                If ret Is Nothing Then ret = AddressOf API.DefWindowProc
                Return ret
            End Get
        End Property
#Region "Equals"
        ''' <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        ''' <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
        ''' <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />. </param>

        Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
            If TypeOf obj Is IWin32Window Then Return DirectCast(obj, IWin32Window).Handle = Me.Handle
            Return MyBase.Equals(obj)
        End Function
        ''' <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        ''' <param name="other">An object to compare with this object.</param>
        ''' <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        Public Overloads Function Equals(ByVal other As System.Windows.Forms.Control) As Boolean Implements System.IEquatable(Of System.Windows.Forms.Control).Equals
            Return Equals(CObj(other))
        End Function
        ''' <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        ''' <param name="other">An object to compare with this object.</param>
        ''' <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        Public Overloads Function Equals(ByVal other As System.Windows.Forms.IWin32Window) As Boolean Implements System.IEquatable(Of System.Windows.Forms.IWin32Window).Equals
            Return Equals(CObj(other))
        End Function
        ''' <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        ''' <param name="other">An object to compare with this object.</param>
        ''' <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        Public Overloads Function Equals(ByVal other As Win32Window) As Boolean Implements System.IEquatable(Of Win32Window).Equals
            Return Equals(CObj(other))
        End Function
        ''' <summary>Compares <see cref="IWin32Window"/> and <see cref="Win32Window"/></summary>
        ''' <param name="a">A <see cref="IWin32Window"/></param>
        ''' <param name="b">A <see cref="Win32Window"/></param>
        ''' <returns>True if <paramref name="a"/>.<see cref="IWin32Window.Handle">Handle</see> equals to <paramref name="b"/>.<see cref="Win32Window.Handle">Handle</see></returns>
        Public Shared Operator =(ByVal a As IWin32Window, ByVal b As Win32Window) As Boolean
            Return b.Equals(a)
        End Operator
        ''' <summary>Compares <see cref="Win32Window"/> and <see cref="IWin32Window"/></summary>
        ''' <param name="a">A <see cref="Win32Window"/></param>
        ''' <param name="b">A <see cref="IWin32Window"/></param>
        ''' <returns>True if <paramref name="a"/>.<see cref="Win32Window.Handle">Handle</see> equals to <paramref name="b"/>.<see cref="IWin32Window.Handle">Handle</see></returns>
        Public Shared Operator =(ByVal a As Win32Window, ByVal b As IWin32Window) As Boolean
            Return a.Equals(b)
        End Operator
        ''' <summary>Compares <see cref="Win32Window"/> and <see cref="Win32Window"/></summary>
        ''' <param name="a">A <see cref="Win32Window"/></param>
        ''' <param name="b">A <see cref="Win32Window"/></param>
        ''' <returns>True if <paramref name="a"/>.<see cref="Win32Window.Handle">Handle</see> equals to <paramref name="b"/>.<see cref="Win32Window.Handle">Handle</see></returns>
        Public Shared Operator =(ByVal a As Win32Window, ByVal b As Win32Window) As Boolean
            Return a.Equals(b)
        End Operator
        ''' <summary>Compares <see cref="IWin32Window"/> and <see cref="Win32Window"/></summary>
        ''' <param name="a">A <see cref="IWin32Window"/></param>
        ''' <param name="b">A <see cref="Win32Window"/></param>
        ''' <returns>False if <paramref name="a"/>.<see cref="IWin32Window.Handle">Handle</see> equals to <paramref name="b"/>.<see cref="Win32Window.Handle">Handle</see></returns>
        Public Shared Operator <>(ByVal a As IWin32Window, ByVal b As Win32Window) As Boolean
            Return Not a = b
        End Operator
        ''' <summary>Compares <see cref="Win32Window"/> and <see cref="IWin32Window"/></summary>
        ''' <param name="a">A <see cref="Win32Window"/></param>
        ''' <param name="b">A <see cref="IWin32Window"/></param>
        ''' <returns>False if <paramref name="a"/>.<see cref="Win32Window.Handle">Handle</see> equals to <paramref name="b"/>.<see cref="IWin32Window.Handle">Handle</see></returns>
        Public Shared Operator <>(ByVal a As Win32Window, ByVal b As IWin32Window) As Boolean
            Return Not a = b
        End Operator
        ''' <summary>Compares <see cref="Win32Window"/> and <see cref="Win32Window"/></summary>
        ''' <param name="a">A <see cref="Win32Window"/></param>
        ''' <param name="b">A <see cref="Win32Window"/></param>
        ''' <returns>False if <paramref name="a"/>.<see cref="Win32Window.Handle">Handle</see> equals to <paramref name="b"/>.<see cref="Win32Window.Handle">Handle</see></returns>
        Public Shared Operator <>(ByVal a As Win32Window, ByVal b As Win32Window) As Boolean
            Return Not a = b
        End Operator
#End Region
#End Region
#Region "Shared"
        ''' <summary>Gets window that represents the desktop</summary>
        ''' <exception cref="API.Win32APIException">An error occured</exception>
        Public Shared ReadOnly Property Desktop() As Win32Window
            Get
                Dim dhWnd As Integer = API.GetDesktopWindow
                If dhWnd <> 0 Then
                    Return New Win32Window(dhWnd)
                Else
                    Throw New API.Win32APIException
                End If
            End Get
        End Property
        ''' <summary>Gets all the top-level windows</summary>
        ''' <returns>List of all top-level windows</returns>
        ''' <exception cref="API.Win32APIException">An error occured</exception>
        Public Shared ReadOnly Property TopLevelWindows() As IReadOnlyList(Of Win32Window)
            Get
                Dim List As New List(Of Win32Window)
                If API.EnumWindows(New API.EnumWindowsProc(Function(hWnd As Integer, lParam As Integer) AddToList(List, New Win32Window(hWnd))), 0) Then
                    Return New ReadOnlyListAdapter(Of Win32Window)(List)
                Else
                    Dim ex As New API.Win32APIException
                    If ex.NativeErrorCode <> 0 Then
                        Throw ex
                    Else
                        Return New ReadOnlyListAdapter(Of Win32Window)(List)
                    End If
                End If
            End Get
        End Property
#End Region
    End Class
End Namespace
#End If
