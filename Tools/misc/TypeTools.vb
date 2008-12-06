Imports System.Runtime.CompilerServices, System.Linq
Imports Tools.ReflectionT
Imports System.Reflection

#If Config <= Nightly Then 'Stage:Nightly
''' <author www="http://dzonny.cz">�onny</author>
''' <version version="1.5.2" stage="Nightly"><see cref="VersionAttribute"/> and <see cref="AuthorAttribute"/> removed</version>
Public Module TypeTools
    ''' <summary>Checks if specified value is member of an enumeration</summary>
    ''' <param name="value">Value to be chcked</param>
    ''' <returns>True if <paramref name="value"/> is member of <paramref name="T"/></returns>
    ''' <typeparam name="T">Enumeration to be tested</typeparam>
    ''' <exception cref="ArgumentException"><paramref name="T"/> is not <see cref="[Enum]"/></exception>
    ''' <seelaso cref="IsDefined"/>
    <CLSCompliant(False)> _
    Public Function InEnum(Of T As {IConvertible, Structure})(ByVal value As T) As Boolean
        Return Array.IndexOf([Enum].GetValues(GetType(T)), value) >= 0
    End Function
    ''' <summary>Gets <see cref="Reflection.FieldInfo"/> that represent given constant within an enum</summary>
    ''' <param name="value">Constant to be found</param>
    ''' <returns><see cref="Reflection.FieldInfo"/> of given <paramref name="value"/> if <paramref name="value"/> is member of <paramref name="T"/></returns>
    ''' <typeparam name="T"><see cref="[Enum]"/> to found constant within</typeparam>
    ''' <exception cref="ArgumentException"><paramref name="T"/> is not <see cref="[Enum]"/></exception>
    ''' <exception cref="ArgumentNullException"><paramref name="value"/> is not member of <paramref name="T"/></exception>
    <CLSCompliant(False)> _
    Public Function GetConstant(Of T As {IConvertible, Structure})(ByVal value As T) As Reflection.FieldInfo
        Return GetType(T).GetField([Enum].GetName(GetType(T), value))
    End Function
    ''' <summary>Gets first <see cref="Attribute"/> of specified type from specified <see cref="Reflection.ICustomAttributeProvider"/></summary>
    ''' <param name="From"><see cref="Reflection.ICustomAttributeProvider"/> to get <see cref="Attribute"/> from</param>
    ''' <param name="Inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
    ''' <returns>First attribute returned by <see cref="Reflection.ICustomAttributeProvider.GetCustomAttributes"/> or null if no attribute is returned</returns>
    ''' <typeparam name="T">Type of <see cref="Attribute"/> to get</typeparam>
    <Extension()> _
     Public Function GetAttribute(Of T As Attribute)(ByVal From As Reflection.ICustomAttributeProvider, Optional ByVal Inherit As Boolean = True) As T
        Dim attrs As Object() = From.GetCustomAttributes(GetType(T), Inherit)
        If attrs Is Nothing OrElse attrs.Length = 0 Then Return Nothing Else Return attrs(0)
    End Function
    ''' <summary>Gets all <see cref="Attribute"/>s of specified type from specified <see cref="Reflection.ICustomAttributeProvider"/></summary>
    ''' <param name="From"><see cref="Reflection.ICustomAttributeProvider"/> to get <see cref="Attribute"/> from</param>
    ''' <param name="Inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
    ''' <returns>All attributes returned by <see cref="Reflection.ICustomAttributeProvider.GetCustomAttributes"/> or null if no attribute is returned</returns>
    ''' <typeparam name="T">Type of <see cref="Attribute"/> to get</typeparam>
    <Extension()> _
    Public Function GetAttributes(Of T As Attribute)(ByVal From As Reflection.ICustomAttributeProvider, Optional ByVal Inherit As Boolean = True) As T()

        Dim attrs As Object() = From.GetCustomAttributes(GetType(T), Inherit)
        If attrs Is Nothing Then Return Nothing
        Return (From Attr In attrs Select DirectCast(Attr, T)).ToArray
    End Function
    ''' <summary>Converts specified value to underlying type of specified enumeration (type-safe)</summary>
    ''' <param name="value"><see cref="IConvertible"/> to be converted using invariant culture</param>
    ''' <typeparam name="T">Type of enumeration (must derive from <see cref="System.Enum"/>)</typeparam>
    ''' <exception cref="System.ArgumentException"><paramref name="T"/> is not an <see cref="System.Enum"/> -or- Underlying type of <paramref name="Type"/> is neither <see cref="Byte"/>, <see cref="SByte"/>, <see cref="Short"/>, <see cref="UShort"/>, <see cref="Integer"/>, <see cref="UInteger"/>, <see cref="Long"/> nor <see cref="ULong"/></exception>
    <CLSCompliant(False)> _
    Public Function GetValueInEnumBaseType(Of T As {IConvertible, Structure})(ByVal value As IConvertible) As IConvertible
        Return GetValueInEnumBaseType(GetType(T), value)
    End Function
    ''' <summary>Converts specified value to underlying type of specified enumeration (type-unsafe)</summary>
    ''' <param name="value"><see cref="IConvertible"/> to be converted using invariant culture</param>
    ''' <param name="Type">Type of enumeration (must derive from <see cref="System.Enum"/>)</param>
    ''' <exception cref="System.ArgumentNullException"><paramref name="Type"/> is null.</exception>
    ''' <exception cref="System.ArgumentException"><paramref name="Type"/> is not an <see cref="System.Enum"/> -or- Underlying type of <paramref name="Type"/> is neither <see cref="Byte"/>, <see cref="SByte"/>, <see cref="Short"/>, <see cref="UShort"/>, <see cref="Integer"/>, <see cref="UInteger"/>, <see cref="Long"/> nor <see cref="ULong"/></exception>
    <CLSCompliant(False)> _
    Public Function GetValueInEnumBaseType(ByVal Type As Type, ByVal Value As IConvertible) As IConvertible
        Dim EType As Type = [Enum].GetUnderlyingType(Type)
        If GetType(Byte).Equals(EType) Then : Return Value.ToByte(System.Globalization.CultureInfo.InvariantCulture)
        ElseIf GetType(SByte).Equals(EType) Then : Return Value.ToSByte(System.Globalization.CultureInfo.InvariantCulture)
        ElseIf GetType(Short).Equals(EType) Then : Return Value.ToInt16(System.Globalization.CultureInfo.InvariantCulture)
        ElseIf GetType(UShort).Equals(EType) Then : Return Value.ToUInt16(System.Globalization.CultureInfo.InvariantCulture)
        ElseIf GetType(Integer).Equals(EType) Then : Return Value.ToInt32(System.Globalization.CultureInfo.InvariantCulture)
        ElseIf GetType(UInteger).Equals(EType) Then : Return Value.ToUInt32(System.Globalization.CultureInfo.InvariantCulture)
        ElseIf GetType(Long).Equals(EType) Then : Return Value.ToInt64(System.Globalization.CultureInfo.InvariantCulture)
        ElseIf GetType(ULong).Equals(EType) Then : Return Value.ToUInt64(System.Globalization.CultureInfo.InvariantCulture)
        Else : Throw New ArgumentException(String.Format(ResourcesT.Exceptions.UnknownUnderlyingType0, EType.Name))
        End If
    End Function
    ''' <summary>Gets name of given enumeration value</summary>
    ''' <param name="value">Value to get name of</param>
    ''' <returns>Name of value in enumeration or null if there is no constant with given value</returns>
    <Extension()> Public Function GetName(ByVal value As [Enum]) As String
        Return [Enum].GetName(value.GetType, value)
    End Function
    ''' <summary>Gets constant field that represents given enum value</summary>
    ''' <param name="value">Value to get constant of</param>
    ''' <returns><see cref="Reflection.FieldInfo"/> with constant value equal to <paramref name="value"/> or null if such field does not exist</returns>
    <Extension()> Public Function GetConstant(ByVal value As [Enum]) As System.Reflection.FieldInfo
        Dim name = [Enum].GetName(value.GetType, value)
        If name Is Nothing Then Return Nothing
        Return value.GetType.GetField(name)
    End Function
    ''' <summary>Gets <see cref="Reflection.FieldInfo"/> representing constant of given name from an enumeration</summary>
    ''' <param name="name">Name of constant to get</param>
    ''' <param name="EnumType">Type of enumeration</param>
    ''' <returns><see cref="Reflection.FieldInfo"/> that represents constant enum member of type <paramref name="EnumType"/> with name <paramref name="name"/>. Null if such constant doesnot exists.</returns>
    ''' <exception cref="ArgumentException"><paramref name="EnumType"/> is not enumeration</exception>
    Public Function GetConstant(ByVal name As String, ByVal EnumType As Type) As Reflection.FieldInfo
        If Not EnumType.IsEnum Then Throw New ArgumentException(String.Format(ResourcesT.Exceptions.MustBeEnumeration, "Type"), "Type")
        Dim field = EnumType.GetField(name, Reflection.BindingFlags.Static Or Reflection.BindingFlags.Public)
        If field Is Nothing Then Return Nothing
        If Not field.IsLiteral Then Return Nothing
        Return field
    End Function
    ''' <summary>Gets <see cref="Reflection.FieldInfo"/> representing constant of given name from an enumeration</summary>
    ''' <param name="name">Name of constant to get</param>
    ''' <typeparam name="T">Type of enumeration</typeparam>
    ''' <returns><see cref="Reflection.FieldInfo"/> that represents constant enum member of type <paramref name="EnumType"/> with name <paramref name="name"/>. Null if such constant doesnot exists.</returns>
    ''' <exception cref="ArgumentException"><typeparamref name="T"/> is not enumeration</exception>
    <CLSCompliant(False)> _
    Public Function GetConstant(Of T As {Structure, IConvertible})(ByVal name$) As Reflection.FieldInfo
        Return GetConstant(name, GetType(T))
    End Function
    ''' <summary>Gets value of enum in its unedlying type</summary>
    ''' <param name="value">Enumeration value</param>
    ''' <returns>Value of enum in its underlying type (so it no longer derives from <see cref="System.[Enum]"/>)</returns>
    <Extension(), CLSCompliant(False)> Public Function GetValue(ByVal value As [Enum]) As IConvertible
        Return GetValueInEnumBaseType(value.GetType, value)
    End Function
    ''' <summary>Gets value of enum member</summary>
    ''' <param name="name">Name of enumeration memebr</param>
    ''' <param name="EnumType">Type of enumeration</param>
    ''' <exception cref="ArgumentException"><paramref name="EnumType"/> is not enumeration =or= Constant with name <paramref name="name"/> does not exist in enumeration <paramref name="EnumType"/>.</exception>
    ''' <returns>Value of constant with name <paramref name="name"/> in type <paramref name="EnumType"/></returns>
    Public Function GetValue(ByVal name$, ByVal EnumType As Type) As [Enum]
        Dim cns = GetConstant(name, EnumType)
        If cns Is Nothing Then Throw New ArgumentException(String.Format(ResourcesT.Exceptions.Constant0DoesNotExistInType1, name, EnumType.Name))
        Return cns.GetValue(Nothing)
    End Function
    ''' <summary>Gets value of enum member</summary>
    ''' <param name="name">Name of enumeration memebr</param>
    ''' <typeparam name="T">Type of enumeration</typeparam>
    ''' <exception cref="ArgumentException"><typeparamref name="T"/> is not enumeration =or= Constant with name <paramref name="name"/> does not exist in enumeration <typeparamref name="T"/>.</exception>
    ''' <returns>Value of constant with name <paramref name="name"/> in type <typeparamref name="T"/></returns>
    <CLSCompliant(False)> _
    Public Function GetValue(Of T As {IConvertible, Structure})(ByVal name As String) As T
        Return GetValue(name, GetType(T)).GetValue
    End Function
    ''' <summary>Converts specified <see cref="IConvertible"/> to specified <see cref="[Enum]"/> (type-safe)</summary>
    ''' <param name="value"><see cref="IConvertible"/> to be converted using invariant culture</param>
    ''' <typeparam name="T">Type of enumeration (must derive from <see cref="System.Enum"/>)</typeparam>
    ''' <exception cref="System.ArgumentException"><paramref name="T"/> is not an <see cref="System.Enum"/> -or- Underlying type of <paramref name="Type"/> is neither <see cref="Byte"/>, <see cref="SByte"/>, <see cref="Short"/>, <see cref="UShort"/>, <see cref="Integer"/>, <see cref="UInteger"/>, <see cref="Long"/> nor <see cref="ULong"/></exception>
    <CLSCompliant(False)> _
    Public Function GetEnumValue(Of T As {IConvertible, Structure})(ByVal Value As IConvertible) As T
        Return CObj(GetEnumValue(GetType(T), Value))
    End Function
    ''' <summary>Converts specified <see cref="IConvertible"/> to specified <see cref="[Enum]"/> (type-unsafe)</summary>
    ''' <param name="value"><see cref="IConvertible"/> to be converted using invariant culture</param>
    ''' <param name="Type">Type of enumeration (must derive from <see cref="System.Enum"/>)</param>
    ''' <exception cref="System.ArgumentNullException"><paramref name="Type"/> is null.</exception>
    ''' <exception cref="System.ArgumentException"><paramref name="Type"/> is not an <see cref="System.Enum"/> -or- Underlying type of <paramref name="Type"/> is neither <see cref="Byte"/>, <see cref="SByte"/>, <see cref="Short"/>, <see cref="UShort"/>, <see cref="Integer"/>, <see cref="UInteger"/>, <see cref="Long"/> nor <see cref="ULong"/></exception>
    <CLSCompliant(False)> _
    Public Function GetEnumValue(ByVal Type As Type, ByVal Value As IConvertible) As [Enum]
        Return [Enum].ToObject(Type, GetValueInEnumBaseType(Type, Value))
    End Function
    ''' <summary>Gets value idicating if given value is defined as constant in enumeration</summary>
    ''' <param name="value">Value to be checked. Value must be to type of enumeration to be checked in</param>
    ''' <returns>True if <paramref name="value"/> is defined as constant in enumeration of type of <paramref name="value"/></returns>
    ''' <remarks>Assembly Tools IL contains more type-safe generic extension function IsDefined. This is comanion function to <see cref="InEnum"/>.</remarks>
    <Extension()> Public Function IsDefined(ByVal value As [Enum]) As Boolean
        Return Array.IndexOf([Enum].GetValues(value.GetType), value) >= 0
    End Function

    ''' <summary>Converts set of flags separated by separator to value of given enumeration</summary>
    ''' <param name="Flags">Flags to parse. Each flag can be name or number</param>
    ''' <param name="EnumType">Type fo parse flags into</param>
    ''' <param name="Separator">Separator of flags</param>
    ''' <returns>Returns value of type <paramref name="EnumType"/></returns>
    ''' <exception cref="ArgumentException"><paramref name="EnumType"/> is not enumeration =or= any flag cannot be found as member of <paramref name="EnumType"/></exception>
    Public Function FlagsFromString(ByVal Flags As String, ByVal EnumType As Type, ByVal Separator As String) As [Enum]
        If Not EnumType.IsEnum Then Throw New ArgumentException(String.Format(ResourcesT.Exceptions.MustBeEnumeration, "EnumType"), "EnumType")
        If [Enum].GetUnderlyingType(EnumType).Equals(GetType(SByte)) OrElse [Enum].GetUnderlyingType(EnumType).Equals(GetType(Short)) OrElse [Enum].GetUnderlyingType(EnumType).Equals(GetType(Integer)) OrElse [Enum].GetUnderlyingType(EnumType).Equals(GetType(Long)) Then
            Dim ret As Long = 0
            For Each item In Flags.Split(Separator)
                If IsNumeric(item) Then ret = ret Or CLng(item) Else _
                ret = ret Or CType(GetValue(item, EnumType).GetValue, Long)
            Next
            Return [Enum].ToObject(EnumType, GetValueInEnumBaseType(EnumType, ret))
        Else
            Dim ret As ULong = 0
            For Each item In Flags.Split(Separator)
                If IsNumeric(item) AndAlso CDec(item) >= 0 Then : ret = ret Or CULng(item)
                ElseIf IsNumeric(item) Then : Throw New ArgumentException(ResourcesT.Exceptions.EnumerationDoesNotAllowNegativeValues)
                Else : ret = ret Or CType(GetValue(item, EnumType).GetValue, ULong) : End If
            Next
            Return [Enum].ToObject(EnumType, GetValueInEnumBaseType(EnumType, ret))
        End If
    End Function
    ''' <summary>Converts set of flags separated by separator to value of given enumeration</summary>
    ''' <param name="Flags">Flags to parse. Each flag can be name or number</param>
    ''' <param name="EnumType">Type fo parse flags into</param>
    ''' <param name="Culture">Culture to obtain separator from</param>
    ''' <returns>Returns value of type <paramref name="EnumType"/></returns>
    ''' <exception cref="ArgumentException"><paramref name="EnumType"/> is not enumeration =or= any flag cannot be found as member of <paramref name="EnumType"/></exception>
    Public Function FlagsFromString(ByVal Flags As String, ByVal EnumType As Type, ByVal Culture As Globalization.CultureInfo) As [Enum]
        Return FlagsFromString(Flags, EnumType, Culture.TextInfo)
    End Function
    ''' <summary>Converts set of flags separated by separator to value of given enumeration</summary>
    ''' <param name="Flags">Flags to parse. Each flag can be name or number</param>
    ''' <param name="EnumType">Type fo parse flags into</param>
    ''' <param name="TextInfo"><see cref="Globalization.TextInfo"/> to obtain separator from</param>
    ''' <returns>Returns value of type <paramref name="EnumType"/></returns>
    ''' <exception cref="ArgumentException"><paramref name="EnumType"/> is not enumeration =or= any flag cannot be found as member of <paramref name="EnumType"/></exception>
    Public Function FlagsFromString(ByVal Flags As String, ByVal EnumType As Type, ByVal TextInfo As Globalization.TextInfo) As [Enum]
        Return FlagsFromString(Flags, EnumType, TextInfo.ListSeparator)
    End Function
    ''' <summary>Converts set of flags separated by separator to value of given enumeration</summary>
    ''' <param name="Flags">Flags to parse. Each flag can be name or number</param>
    ''' <param name="EnumType">Type fo parse flags into</param>
    ''' <returns>Returns value of type <paramref name="EnumType"/></returns>
    ''' <exception cref="ArgumentException"><paramref name="EnumType"/> is not enumeration =or= any flag cannot be found as member of <paramref name="EnumType"/></exception>
    ''' <remarks>Obtains separator from current culture</remarks>
    Public Function FlagsFromString(ByVal Flags As String, ByVal EnumType As Type) As [Enum]
        Return FlagsFromString(Flags, EnumType, Globalization.CultureInfo.CurrentCulture)
    End Function

    ''' <summary>Converts set of flags separated by separator to value of given enumeration</summary>
    ''' <param name="Flags">Flags to parse. Each flag can be name or number</param>
    ''' <typeparam name="T">Type fo parse flags into</typeparam>
    ''' <param name="Separator">Separator of flags</param>
    ''' <returns>Returns value of type <paramref name="EnumType"/></returns>
    ''' <exception cref="ArgumentException"><typeparamref name="T"/> is not enumeration =or= any flag cannot be found as member of <paramref name="EnumType"/></exception>
    <CLSCompliant(False)> _
    Public Function FlagsFromString(Of T As {Structure, IConvertible})(ByVal Flags As String, ByVal Separator As String) As T
        Return CObj(FlagsFromString(Flags, GetType(T), Separator))
    End Function
    ''' <summary>Converts set of flags separated by separator to value of given enumeration</summary>
    ''' <param name="Flags">Flags to parse. Each flag can be name or number</param>
    ''' <typeparam name="T">Type fo parse flags into</typeparam>
    ''' <param name="Culture">Culture to obtain separator from</param>
    ''' <returns>Returns value of type <paramref name="EnumType"/></returns>
    ''' <exception cref="ArgumentException"><typeparamref name="T"/> is not enumeration =or= any flag cannot be found as member of <paramref name="EnumType"/></exception>
    <CLSCompliant(False)> _
    Public Function FlagsFromString(Of T As {Structure, IConvertible})(ByVal Flags As String, ByVal Culture As Globalization.CultureInfo) As T
        Return CObj(FlagsFromString(Flags, GetType(T), Culture.TextInfo))
    End Function
    ''' <summary>Converts set of flags separated by separator to value of given enumeration</summary>
    ''' <param name="Flags">Flags to parse. Each flag can be name or number</param>
    ''' <typeparam name="T">Type fo parse flags into</typeparam>
    ''' <param name="TextInfo"><see cref="Globalization.TextInfo"/> to obtain separator from</param>
    ''' <returns>Returns value of type <paramref name="EnumType"/></returns>
    ''' <exception cref="ArgumentException"><typeparamref name="T"/> is not enumeration =or= any flag cannot be found as member of <paramref name="EnumType"/></exception>
    <CLSCompliant(False)> _
    Public Function FlagsFromString(Of T As {Structure, IConvertible})(ByVal Flags As String, ByVal TextInfo As Globalization.TextInfo) As T
        Return CObj(FlagsFromString(Flags, GetType(T), TextInfo.ListSeparator))
    End Function
    ''' <summary>Converts set of flags separated by separator to value of given enumeration</summary>
    ''' <param name="Flags">Flags to parse. Each flag can be name or number</param>
    ''' <typeparam name="T">Type fo parse flags into</typeparam>
    ''' <returns>Returns value of type <paramref name="EnumType"/></returns>
    ''' <exception cref="ArgumentException"><typeparamref name="T"/> is not enumeration =or= any flag cannot be found as member of <paramref name="EnumType"/></exception>
    ''' <remarks>Obtains separator from current culture</remarks>
    <CLSCompliant(False)> _
    Public Function FlagsFromString(Of T As {Structure, IConvertible})(ByVal Flags As String) As T
        Return CObj(FlagsFromString(Flags, GetType(T), Globalization.CultureInfo.CurrentCulture))
    End Function
    ''' <summary>Gets toolbox bitmap assciated with given <see cref="Type"/></summary>
    ''' <param name="Type">Type to get toolbox bitmap for</param>
    ''' <param name="Large">True to obtain large bitmap (32�32), false to obtain small one (16�16))</param>
    ''' <param name="Inherit">True to allow inheriting of toolbox bitmap from base type of <paramref name="Type"/></param>
    ''' <returns>Bitmap assciated with <see cref="Type"/> if any</returns>
    ''' <remarks>If <paramref name="Type"/> is decorated with <see cref="Drawing.ToolboxBitmapAttribute"/> then it is used. If not static method <see cref="Drawing.ToolboxBitmapAttribute.GetImageFromResource"/> is used with <see cref="Type.Name"/> of <paramref name="Type"/>.</remarks>
    ''' <exception cref="ArgumentNullException"><paramref name="Type"/> is null</exception>
    <Extension()> _
    Public Function GetToolBoxBitmap(ByVal Type As Type, Optional ByVal Large As Boolean = False, Optional ByVal Inherit As Boolean = False) As Drawing.Image
        If Type Is Nothing Then Throw New ArgumentException("Type")
        Dim attr = Type.GetAttribute(Of Drawing.ToolboxBitmapAttribute)(False)
        If attr Is Nothing AndAlso Inherit Then attr = Type.GetAttribute(Of Drawing.ToolboxBitmapAttribute)(True)
        If attr IsNot Nothing Then
            Return attr.GetImage(Type, Large)
        Else
            Dim bmp = Drawing.ToolboxBitmapAttribute.GetImageFromResource(Type, Type.Name, Large)
            If bmp IsNot Nothing Then Return bmp
        End If
        If Inherit AndAlso Not Type.Equals(GetType(Object)) AndAlso Type.BaseType IsNot Nothing Then
            Return Type.BaseType.GetToolBoxBitmap(Large, Inherit)
        Else
            Return Nothing
        End If
    End Function
    ''' <summary>Gets default CTor for given type</summary>
    ''' <param name="Type"><see cref="Type"/> to get default CTor for</param>
    ''' <param name="Attributes">Optionaly specifies aaccesibility attributes for default constructor. Default is <see cref="Reflection.BindingFlags.[Public]"/>.</param>
    ''' <returns><see cref="Reflection.ConstructorInfo"/> refresenting default CTor of type <paramref name="Type"/>. Null if there is no default (parameter-less) CTor</returns>
    ''' <exception cref="ArgumentNullException"><paramref name="Type"/> is null</exception>
    ''' <seealso cref="HasDefaultCTor"/>
    ''' <version stage="Nightly" version="1.5.2">Fixed: Always returns null due to <paramref name="Attributes"/> being and-ed with <see cref="Reflection.BindingFlags.Instance"/> instead of or-ed</version>
    <Extension()> _
    Public Function GetDefaltCTor(ByVal Type As Type, Optional ByVal Attributes As Reflection.BindingFlags = Reflection.BindingFlags.Public) As Reflection.ConstructorInfo
        If Type Is Nothing Then Throw New ArgumentException("Type")
        Return Type.GetConstructor(Attributes Or Reflection.BindingFlags.Instance, Nothing, Type.EmptyTypes, Nothing)
    End Function
    ''' <summary>Gets value indicationg if given <see cref="Type"/> has default constructor</summary>
    ''' <param name="Type"><see cref="Type"/> to check</param>
    ''' <param name="Attributes">Optionaly specifies aaccesibility attributes for default constructor. Default is <see cref="Reflection.BindingFlags.[Public]"/>.</param>
    ''' <remarks>True if type has default (parameterless) CTor, fale otherwise.</remarks>
    ''' <seealso cref="GetDefaltCTor"/>
    ''' <exception cref="ArgumentNullException"><paramref name="Type"/> is null</exception>
    <Extension()> _
    Public Function HasDefaultCTor(ByVal Type As Type, Optional ByVal Attributes As Reflection.BindingFlags = Reflection.BindingFlags.Public) As Boolean
        If Type Is Nothing Then Throw New ArgumentException("Type")
        Return Type.GetDefaltCTor(Attributes) IsNot Nothing
    End Function
    ''' <summary>Gets value indicating if instance of geven type can be easily created using default CTor</summary>
    ''' <param name="Type"><see cref="Type"/> to check</param>
    ''' <returns>False if type is either interface, abstract or open; true if type has default contructor or is value type</returns>
    ''' <seealso cref="HasDefaultCTor"/>
    ''' <exception cref="ArgumentNullException"><paramref name="Type"/> is null</exception>
    <Extension()> _
    Public Function CanAutomaticallyCreateInstance(ByVal Type As Type) As Boolean
        If Type Is Nothing Then Throw New ArgumentException("Type")
        If Type.IsInterface Then Return False
        If Type.IsAbstract Then Return False
        If Type.IsGenericTypeDefinition Then Return False
        If Type.IsValueType Then Return True
        If Type.HasDefaultCTor Then Return True
    End Function
    ''' <summary>Creates an instance of the specified type using that type's default constructor.</summary>
    ''' <param name="type">The type of object to create.</param>
    ''' <returns>A reference to the newly created object.</returns>
    ''' <exception cref="System.ArgumentNullException"><paramref name="type"/> is null.</exception>
    ''' <exception cref="System.ArgumentException"><paramref name="type"/> is not a RuntimeType. -or- <paramref name="type"/> is an open generic type (that is, the <see cref="System.Type.ContainsGenericParameters" /> property returns true).</exception>
    ''' <exception cref="System.NotSupportedException"><paramref name="type"/> cannot be a <see cref="System.Reflection.Emit.TypeBuilder" />.  -or- Creation of <see cref="System.TypedReference" />, <see cref="System.ArgIterator" />, <see cref="System.Void" />, and <see cref="System.RuntimeArgumentHandle" /> types, or arrays of those types, is not supported.</exception>
    ''' <exception cref="System.Reflection.TargetInvocationException">The constructor being called throws an exception.</exception>
    ''' <exception cref="System.MethodAccessException">The caller does not have permission to call this constructor.</exception>
    ''' <exception cref="System.MemberAccessException">Cannot create an instance of an abstract class, or this member was invoked with a late-binding mechanism.</exception>
    ''' <exception cref="System.Runtime.InteropServices.InvalidComObjectException">The COM type was not obtained through Overload:<see cref="System.Type.GetTypeFromProgID" /> or Overload:<see cref="System.Type.GetTypeFromCLSID" />.</exception>
    ''' <exception cref="System.MissingMethodException">No matching public constructor was found.</exception>
    ''' <exception cref="System.Runtime.InteropServices.COMException"><paramref name="type"/> is a COM object but the class identifier used to obtain the type is invalid, or the identified class is not registered.</exception>
    ''' <exception cref="System.TypeLoadException"><paramref name="type"/> is not a valid type.</exception>
    ''' <seelaso cref="Activator.CreateInstance"/>
    <Extension()> Function CreateInstance(ByVal type As Type) As Object
        Return Activator.CreateInstance(type)
    End Function
    'TODO: Uncomment and implement dynamic cast
    '<Extension()> _
    'Public Function DynamicCast(Of T)(ByVal obj As Object) As T
    '    Return DynamicCast(obj, GetType(T))
    'End Function
    '<Extension()> _
    'Public Function DynamicCast(ByVal obj As Object, ByVal Type As Type) As Object
    '    If obj Is Nothing Then Return Nothing
    '    If Type Is Nothing Then Throw New ArgumentNullException("Type")
    '    Dim ObjType = obj.GetType
    '    If Type.IsAssignableFrom(ObjType) Then Return obj
    '    Dim SourceImplicit = ObjType.GetOperators(Operators.Implicit)
    '    Dim SourceExplicit = ObjType.GetOperators(Operators.Explicit)
    '    Dim TargetImplicit = Type.GetOperators(Operators.Implicit)
    '    Dim TargetExplicit = Type.GetOperators(Operators.Explicit)
    '    Dim PossibleOperators = (From op In SourceImplicit.Union(SourceImplicit).Union(TargetImplicit).Union(TargetExplicit) _
    '                            Where op.GetParameters()(0).ParameterType.IsAssignableFrom(ObjType) _
    '                                  AndAlso Type.IsAssignableFrom(op.ReturnType) _
    '                            Select op).ToArray
    '    If PossibleOperators.Length = 1 Then
    '        Return PossibleOperators(0).Invoke(Nothing, New Object() {obj})
    '    ElseIf PossibleOperators.Count > 1 Then
    '        Dim ArgType = ObjType
    '        While ArgType IsNot Nothing AndAlso Not ArgType.Equals(GetType(Object))
    '            Dim LevelOperators = (From op In PossibleOperators Where op.GetParameters()(0).ParameterType.Equals(ObjType) Select op).ToArray
    '            If LevelOperators.Length = 1 Then
    '                Return LevelOperators(0).Invoke(Nothing, New Object() {obj})
    '            ElseIf LevelOperators.Length > 1 Then

    '            End If
    '            ArgType = ArgType.BaseType
    '        End While
    '    End If
    '    'Enum
    '    If TypeOf obj Is [Enum] Then
    '        If Type.Equals(GetType(String)) Then Return obj.ToString
    '        Return DynamicCast(DirectCast(obj, [Enum]).GetValue, Type)
    '    ElseIf Type.IsEnum Then
    '        If TypeOf obj Is String AndAlso [Enum].GetNames(Type).Contains(obj) Then Return [Enum].Parse(Type, obj)
    '        Dim value As IConvertible = DynamicCast(obj, [Enum].GetUnderlyingType(Type))
    '        Return GetEnumValue(Type, value)
    '    End If
    '    'Built-in types
    '    If TypeOf obj Is Byte Then
    '        With DirectCast(obj, Byte)
    '            If Type.IsAssignableFrom(GetType(Byte)) Then : Return CByte(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(SByte)) Then : Return CSByte(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Short)) Then : Return CShort(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(UShort)) Then : Return CUShort(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Integer)) Then : Return CInt(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(UInteger)) Then : Return CUInt(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Long)) Then : Return CLng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(ULong)) Then : Return CULng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Single)) Then : Return CSng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Double)) Then : Return CDbl(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Char)) Then : Return ChrW(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Boolean)) Then : Return CBool(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(String)) Then : Return CStr(.self)
    '            End If
    '        End With
    '    ElseIf TypeOf obj Is SByte Then
    '        With DirectCast(obj, SByte)
    '            If Type.IsAssignableFrom(GetType(Byte)) Then : Return CByte(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(SByte)) Then : Return CSByte(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Short)) Then : Return CShort(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(UShort)) Then : Return CUShort(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Integer)) Then : Return CInt(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(UInteger)) Then : Return CUInt(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Long)) Then : Return CLng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(ULong)) Then : Return CULng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Single)) Then : Return CSng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Double)) Then : Return CDbl(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Char)) Then : Return ChrW(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Boolean)) Then : Return CBool(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(String)) Then : Return CStr(.self)
    '            End If
    '        End With
    '    ElseIf TypeOf obj Is Short Then
    '        With DirectCast(obj, Short)
    '            If Type.IsAssignableFrom(GetType(Byte)) Then : Return CByte(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(SByte)) Then : Return CSByte(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Short)) Then : Return CShort(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(UShort)) Then : Return CUShort(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Integer)) Then : Return CInt(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(UInteger)) Then : Return CUInt(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Long)) Then : Return CLng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(ULong)) Then : Return CULng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Single)) Then : Return CSng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Double)) Then : Return CDbl(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Char)) Then : Return ChrW(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Boolean)) Then : Return CBool(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(String)) Then : Return CStr(.self)
    '            End If
    '        End With
    '    ElseIf TypeOf obj Is UShort Then
    '        With DirectCast(obj, UShort)
    '            If Type.IsAssignableFrom(GetType(Byte)) Then : Return CByte(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(SByte)) Then : Return CSByte(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Short)) Then : Return CShort(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(UShort)) Then : Return CUShort(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Integer)) Then : Return CInt(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(UInteger)) Then : Return CUInt(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Long)) Then : Return CLng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(ULong)) Then : Return CULng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Single)) Then : Return CSng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Double)) Then : Return CDbl(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Char)) Then : Return ChrW(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Boolean)) Then : Return CBool(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(String)) Then : Return CStr(.self)
    '            End If
    '        End With
    '    ElseIf TypeOf obj Is Integer Then
    '        With DirectCast(obj, Integer)
    '            If Type.IsAssignableFrom(GetType(Byte)) Then : Return CByte(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(SByte)) Then : Return CSByte(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Short)) Then : Return CShort(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(UShort)) Then : Return CUShort(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Integer)) Then : Return CInt(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(UInteger)) Then : Return CUInt(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Long)) Then : Return CLng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(ULong)) Then : Return CULng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Single)) Then : Return CSng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Double)) Then : Return CDbl(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Char)) Then : Return ChrW(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Boolean)) Then : Return CBool(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(String)) Then : Return CStr(.self)
    '            End If
    '        End With
    '    ElseIf TypeOf obj Is UInteger Then
    '        With DirectCast(DirectCast(obj, UInteger), IConvertible)
    '            If Type.IsAssignableFrom(GetType(Byte)) Then : Return CByte(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(SByte)) Then : Return CSByte(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Short)) Then : Return CShort(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(UShort)) Then : Return CUShort(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Integer)) Then : Return CInt(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(UInteger)) Then : Return CUInt(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Long)) Then : Return CLng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(ULong)) Then : Return CULng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Single)) Then : Return CSng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Double)) Then : Return CDbl(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Char)) Then : Return ChrW(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Boolean)) Then : Return CBool(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(String)) Then : Return CStr(.self)
    '            End If
    '        End With
    '    ElseIf TypeOf obj Is Long Then
    '        With DirectCast(DirectCast(obj, Long), IConvertible)
    '            If Type.IsAssignableFrom(GetType(Byte)) Then : Return CByte(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(SByte)) Then : Return CSByte(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Short)) Then : Return CShort(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(UShort)) Then : Return CUShort(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Integer)) Then : Return CInt(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(UInteger)) Then : Return CUInt(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Long)) Then : Return CLng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(ULong)) Then : Return CULng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Single)) Then : Return CSng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Double)) Then : Return CDbl(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Char)) Then : Return ChrW(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Boolean)) Then : Return CBool(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(String)) Then : Return CStr(.self)
    '            End If
    '        End With
    '    ElseIf TypeOf obj Is ULong Then
    '        With DirectCast(DirectCast(obj, ULong), IConvertible)
    '            If Type.IsAssignableFrom(GetType(Byte)) Then : Return CByte(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(SByte)) Then : Return CSByte(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Short)) Then : Return CShort(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(UShort)) Then : Return CUShort(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Integer)) Then : Return CInt(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(UInteger)) Then : Return CUInt(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Long)) Then : Return CLng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(ULong)) Then : Return CULng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Single)) Then : Return CSng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Double)) Then : Return CDbl(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Char)) Then : Return ChrW(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Boolean)) Then : Return CBool(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(String)) Then : Return CStr(.self)
    '            End If
    '        End With
    '    ElseIf TypeOf obj Is Double Then
    '        With DirectCast(obj, Double)
    '            If Type.IsAssignableFrom(GetType(Byte)) Then : Return CByte(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(SByte)) Then : Return CSByte(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Short)) Then : Return CShort(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(UShort)) Then : Return CUShort(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Integer)) Then : Return CInt(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(UInteger)) Then : Return CUInt(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Long)) Then : Return CLng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(ULong)) Then : Return CULng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Single)) Then : Return CSng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Double)) Then : Return CDbl(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Char)) Then : Return ChrW(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Boolean)) Then : Return CBool(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(String)) Then : Return CStr(.self)
    '            End If
    '        End With
    '    ElseIf TypeOf obj Is Char Then
    '        With DirectCast(obj, Char)
    '            If Type.IsAssignableFrom(GetType(Byte)) Then : Return CByte(AscW(.self))
    '            ElseIf Type.IsAssignableFrom(GetType(SByte)) Then : Return CSByte(AscW(.self))
    '            ElseIf Type.IsAssignableFrom(GetType(Short)) Then : Return CShort(AscW(.self))
    '            ElseIf Type.IsAssignableFrom(GetType(UShort)) Then : Return CUShort(AscW(.self))
    '            ElseIf Type.IsAssignableFrom(GetType(Integer)) Then : Return CInt(AscW(.self))
    '            ElseIf Type.IsAssignableFrom(GetType(UInteger)) Then : Return CUInt(AscW(.self))
    '            ElseIf Type.IsAssignableFrom(GetType(Long)) Then : Return CLng(AscW(.self))
    '            ElseIf Type.IsAssignableFrom(GetType(ULong)) Then : Return CULng(AscW(.self))
    '            ElseIf Type.IsAssignableFrom(GetType(Single)) Then : Return CSng(AscW(.self))
    '            ElseIf Type.IsAssignableFrom(GetType(Double)) Then : Return CDbl(AscW(.self))
    '            ElseIf Type.IsAssignableFrom(GetType(Char)) Then : Return .self
    '            ElseIf Type.IsAssignableFrom(GetType(Boolean)) Then : Return CBool(AscW(.self))
    '            ElseIf Type.IsAssignableFrom(GetType(String)) Then : Return CStr(.self)
    '            End If
    '        End With
    '    ElseIf TypeOf obj Is Boolean Then
    '        With DirectCast(obj, Boolean)
    '            If Type.IsAssignableFrom(GetType(Byte)) Then : Return CByte(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(SByte)) Then : Return CSByte(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Short)) Then : Return CShort(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(UShort)) Then : Return CUShort(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Integer)) Then : Return CInt(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(UInteger)) Then : Return CUInt(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Long)) Then : Return CLng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(ULong)) Then : Return CULng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Single)) Then : Return CSng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Double)) Then : Return CDbl(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Char)) Then : Return ChrW(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Boolean)) Then : Return CBool(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(String)) Then : Return CStr(.self)
    '            End If
    '        End With
    '    ElseIf TypeOf obj Is String Then
    '        With DirectCast(obj, String)
    '            If Type.IsAssignableFrom(GetType(Byte)) Then : Return CByte(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(SByte)) Then : Return CSByte(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Short)) Then : Return CShort(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(UShort)) Then : Return CUShort(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Integer)) Then : Return CInt(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(UInteger)) Then : Return CUInt(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Long)) Then : Return CLng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(ULong)) Then : Return CULng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Single)) Then : Return CSng(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Double)) Then : Return CDbl(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Char)) Then : Return CChar(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(Boolean)) Then : Return CBool(.self)
    '            ElseIf Type.IsAssignableFrom(GetType(String)) Then : Return .self
    '            End If
    '        End With
    '    End If
    'End Function
    
End Module


#End If