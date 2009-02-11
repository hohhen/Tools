﻿Imports System.Runtime.CompilerServices, Tools.ExtensionsT
Imports System.Reflection
Imports System.Linq, Tools.LinqT
Imports System.Runtime.InteropServices

#If Config <= Nightly Then 'Stage: Nightly
Namespace ReflectionT
    'TODO: UnitTests
    ''' <summary>Various reflection tools</summary>
    ''' <author www="http://dzonny.cz">Đonny</author>
    ''' <version stage="Nightly" version="1.5.2">Added overloaded functions <see cref="ReflectionTools.GetOperators"/>.</version>
    ''' <version version="1.5.2">Added <see cref="ReflectionTools.IsMemberOf"/> overloaded methods.</version>
    Public Module ReflectionTools
        ''' <summary>Gets namespaces in given module</summary>
        ''' <param name="Module">Module to get namespaces in</param>
        ''' <returns>Array of namespaces in <paramref name="Module"/></returns>
        ''' <param name="IncludeGlobal">True to include global namespace (with empty name)</param>
        ''' <param name="Flat">True to list all namespaces even if their name contains dot (.), False to list only top-level namespaces</param>
        ''' <exception cref="ArgumentNullException"><paramref name="Module"/> is null</exception>
        ''' <exception cref="System.Reflection.ReflectionTypeLoadException">One or more classes in a module could not be loaded.</exception>
        ''' <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        <Extension()> Public Function GetNamespaces(ByVal [Module] As Reflection.Module, Optional ByVal IncludeGlobal As Boolean = False, Optional ByVal Flat As Boolean = True) As [NamespaceInfo]()
            Return [Module].GetNamespaces(Function(t As Type) True, IncludeGlobal, Flat)
            'If [Module] Is Nothing Then Throw New ArgumentNullException("Module")
            'Dim NamespaceNames As New List(Of String)
            'Dim Namespaces As New List(Of NamespaceInfo)
            'For Each Type In [Module].GetTypes
            '    Dim NamespaceName As String
            '    If Not Flat OrElse Type.Namespace = "" OrElse Not Type.Namespace.Contains("."c) Then
            '        NamespaceName = Type.Namespace
            '    Else
            '        NamespaceName = Type.Namespace.Substring(0, Type.Namespace.IndexOf("."c))
            '    End If
            '    If (NamespaceName <> "" OrElse IncludeGlobal) AndAlso Not NamespaceNames.Contains(NamespaceName) Then
            '        NamespaceNames.Add(Type.Namespace)
            '        Namespaces.Add(New NamespaceInfo([Module], Type.Namespace))
            '    End If
            'Next Type
            'Return Namespaces.ToArray()
        End Function
        ''' <summary>Gets namespaces in given module</summary>
        ''' <param name="Module">Module to get namespaces in</param>
        ''' <returns>Array of namespaces in <paramref name="Module"/></returns>
        ''' <param name="TypeFilter">Predicate. Onyl those types for which the predicate returns true will be observed for namespaces.</param>
        ''' <param name="IncludeGlobal">True to include global namespace (with empty name)</param>
        ''' <param name="Flat">True to list all namespaces even if their name contains dot (.), False to list only top-level namespaces</param>
        ''' <exception cref="ArgumentNullException"><paramref name="Module"/> or <paramref name="TypeFilter"/> is null</exception>
        ''' <exception cref="System.Reflection.ReflectionTypeLoadException">One or more classes in a module could not be loaded.</exception>
        ''' <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        <Extension()> Public Function GetNamespaces(ByVal [Module] As Reflection.Module, ByVal TypeFilter As Predicate(Of Type), Optional ByVal IncludeGlobal As Boolean = False, Optional ByVal Flat As Boolean = True) As [NamespaceInfo]()
            If [Module] Is Nothing Then Throw New ArgumentNullException("Module")
            If TypeFilter Is Nothing Then Throw New ArgumentNullException("TypeFilter")
            Dim NamespaceNames As New List(Of String)
            Dim Namespaces As New List(Of NamespaceInfo)
            For Each Type In [Module].GetTypes
                Dim NamespaceName As String
                If Flat OrElse Type.Namespace = "" OrElse Not Type.Namespace.Contains("."c) Then
                    NamespaceName = Type.Namespace
                Else
                    NamespaceName = Type.Namespace.Substring(0, Type.Namespace.IndexOf("."c))
                End If
                If ((NamespaceName <> "" OrElse IncludeGlobal) AndAlso Not NamespaceNames.Contains(NamespaceName)) AndAlso TypeFilter(Type) Then
                    NamespaceNames.Add(NamespaceName)
                    Namespaces.Add(New NamespaceInfo([Module], NamespaceName))
                End If
            Next Type
            Return Namespaces.ToArray()
        End Function
        ''' <summary> defined in given module</summary>
        ''' <param name="Module">Module to get types from</param>
        ''' <param name="FromNamespaces">True to get only types from global namespace. False to get all types (same as <see cref="[Module].GetTypes"/>)</param>
        ''' <returns>Array of types from module <paramref name="Module"/>.</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="Module"/> is null</exception>
        ''' <exception cref="System.Reflection.ReflectionTypeLoadException">One or more classes in a module could not be loaded.</exception>
        ''' <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        <Extension()> Public Function GetTypes(ByVal [Module] As [Module], ByVal FromNamespaces As Boolean) As Type()
            If [Module] Is Nothing Then Throw New ArgumentNullException("Module")
            If FromNamespaces Then Return [Module].GetTypes
            Return (From Type In [Module].GetTypes Where Type.Namespace = "" Select Type).ToArray
        End Function
#Region "Is..."
        ''' <summary>For <see cref="Type"/>, <see cref="MethodBase"/> (<see cref="MethodInfo"/> or <see cref="ConstructorInfo"/>), <see cref="PropertyInfo"/>, <see cref="EventInfo"/> or <see cref="FieldInfo"/> indicates its accessibility</summary>
        ''' <param name="Member">Member to indicate accesibility of</param>
        ''' <returns>True if accessibility of member is public</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="Member"/> is null</exception>
        ''' <exception cref="System.MethodAccessException">The caller does not have permission to reflect on non-public methods and <paramref name="Member"/> is either <see cref="EventInfo"/> or <see cref="PropertyInfo"/>.</exception>
        <Extension()> Public Function IsPublic(ByVal Member As MemberInfo) As Boolean
            If Member Is Nothing Then Throw New ArgumentNullException("Member")
            Select Case Member.MemberType
                Case MemberTypes.TypeInfo, MemberTypes.NestedType
                    With DirectCast(Member, Type)
                        Return (Not .IsNested AndAlso .IsPublic) OrElse (.IsNested AndAlso .IsNestedPublic)
                    End With
                Case MemberTypes.Constructor, MemberTypes.Method
                    Return DirectCast(Member, MethodBase).IsPublic
                Case MemberTypes.Event
                    Return DirectCast(Member, EventInfo).GetAccessibility = MethodAttributes.Public
                Case MemberTypes.Field : Return DirectCast(Member, FieldInfo).IsPublic
                Case MemberTypes.Property
                    Return DirectCast(Member, PropertyInfo).GetAccessibility = MethodAttributes.Public
                Case Else : Return False
            End Select
        End Function
        ''' <summary>For <see cref="Type"/>, <see cref="MethodBase"/> (<see cref="MethodInfo"/> or <see cref="ConstructorInfo"/>), <see cref="PropertyInfo"/>, <see cref="EventInfo"/> or <see cref="FieldInfo"/> indicates its accessibility</summary>
        ''' <param name="Member">Member to indicate accesibility of</param>
        ''' <returns>True if accessibility of member is private</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="Member"/> is null</exception>
        ''' <exception cref="System.MethodAccessException">The caller does not have permission to reflect on non-public methods and <paramref name="Member"/> is either <see cref="EventInfo"/> or <see cref="PropertyInfo"/>.</exception>
        <Extension()> Public Function IsPrivate(ByVal Member As MemberInfo) As Boolean
            If Member Is Nothing Then Throw New ArgumentNullException("Member")
            Select Case Member.MemberType
                Case MemberTypes.TypeInfo, MemberTypes.NestedType
                    With DirectCast(Member, Type)
                        Return .IsNested AndAlso .IsNestedPrivate
                    End With
                Case MemberTypes.Constructor, MemberTypes.Method
                    Return DirectCast(Member, MethodBase).IsPrivate
                Case MemberTypes.Event
                    Return DirectCast(Member, EventInfo).GetAccessibility = MethodAttributes.Private
                Case MemberTypes.Field : Return DirectCast(Member, FieldInfo).IsPrivate
                Case MemberTypes.Property
                    Return DirectCast(Member, PropertyInfo).GetAccessibility = MethodAttributes.Private
                Case Else : Return False
            End Select
        End Function
        ''' <summary>For <see cref="Type"/>, <see cref="MethodBase"/> (<see cref="MethodInfo"/> or <see cref="ConstructorInfo"/>), <see cref="PropertyInfo"/>, <see cref="EventInfo"/> or <see cref="FieldInfo"/> indicates its accessibility</summary>
        ''' <param name="Member">Member to indicate accesibility of</param>
        ''' <returns>True if accessibility of member is assembly (friend, internal)</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="Member"/> is null</exception>
        ''' <exception cref="System.MethodAccessException">The caller does not have permission to reflect on non-public methods and <paramref name="Member"/> is either <see cref="EventInfo"/> or <see cref="PropertyInfo"/>.</exception>
        <Extension()> Public Function IsAssembly(ByVal Member As MemberInfo) As Boolean
            If Member Is Nothing Then Throw New ArgumentNullException("Member")
            Select Case Member.MemberType
                Case MemberTypes.TypeInfo, MemberTypes.NestedType
                    With DirectCast(Member, Type)
                        Return (.IsNested AndAlso .IsNestedAssembly) OrElse (Not .IsNested AndAlso .IsNotPublic)
                    End With
                Case MemberTypes.Constructor, MemberTypes.Method
                    Return DirectCast(Member, MethodBase).IsAssembly
                Case MemberTypes.Event
                    Return DirectCast(Member, EventInfo).GetAccessibility = MethodAttributes.Assembly
                Case MemberTypes.Field : Return DirectCast(Member, FieldInfo).IsAssembly
                Case MemberTypes.Property
                    Return DirectCast(Member, PropertyInfo).GetAccessibility = MethodAttributes.Assembly
                Case Else : Return False
            End Select
        End Function
        ''' <summary>For <see cref="Type"/>, <see cref="MethodBase"/> (<see cref="MethodInfo"/> or <see cref="ConstructorInfo"/>), <see cref="PropertyInfo"/>, <see cref="EventInfo"/> or <see cref="FieldInfo"/> indicates its accessibility</summary>
        ''' <param name="Member">Member to indicate accesibility of</param>
        ''' <returns>True if accessibility of member is family-and-assembly</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="Member"/> is null</exception>
        ''' <exception cref="System.MethodAccessException">The caller does not have permission to reflect on non-public methods and <paramref name="Member"/> is either <see cref="EventInfo"/> or <see cref="PropertyInfo"/>.</exception>
        <Extension()> Public Function IsFamilyAndAssembly(ByVal Member As MemberInfo) As Boolean
            If Member Is Nothing Then Throw New ArgumentNullException("Member")
            Select Case Member.MemberType
                Case MemberTypes.TypeInfo, MemberTypes.NestedType
                    With DirectCast(Member, Type)
                        Return (.IsNested AndAlso .IsNestedFamANDAssem)
                    End With
                Case MemberTypes.Constructor, MemberTypes.Method
                    Return DirectCast(Member, MethodBase).IsFamilyAndAssembly
                Case MemberTypes.Event
                    Return DirectCast(Member, EventInfo).GetAccessibility = MethodAttributes.FamANDAssem
                Case MemberTypes.Field : Return DirectCast(Member, FieldInfo).IsFamilyAndAssembly
                Case MemberTypes.Property
                    Return DirectCast(Member, PropertyInfo).GetAccessibility = MethodAttributes.FamANDAssem
                Case Else : Return False
            End Select
        End Function
        ''' <summary>For <see cref="Type"/>, <see cref="MethodBase"/> (<see cref="MethodInfo"/> or <see cref="ConstructorInfo"/>), <see cref="PropertyInfo"/>, <see cref="EventInfo"/> or <see cref="FieldInfo"/> indicates its accessibility</summary>
        ''' <param name="Member">Member to indicate accesibility of</param>
        ''' <returns>True if accessibility of member is family-or-assembly (protected friend)</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="Member"/> is null</exception>
        ''' <exception cref="System.MethodAccessException">The caller does not have permission to reflect on non-public methods and <paramref name="Member"/> is either <see cref="EventInfo"/> or <see cref="PropertyInfo"/>.</exception>
        <Extension()> Public Function IsFamilyOrAssembly(ByVal Member As MemberInfo) As Boolean
            If Member Is Nothing Then Throw New ArgumentNullException("Member")
            Select Case Member.MemberType
                Case MemberTypes.TypeInfo, MemberTypes.NestedType
                    With DirectCast(Member, Type)
                        Return (.IsNested AndAlso .IsNestedFamORAssem)
                    End With
                Case MemberTypes.Constructor, MemberTypes.Method
                    Return DirectCast(Member, MethodBase).IsFamilyAndAssembly
                Case MemberTypes.Event
                    Return DirectCast(Member, EventInfo).GetAccessibility = MethodAttributes.FamORAssem
                Case MemberTypes.Field : Return DirectCast(Member, FieldInfo).IsFamilyOrAssembly
                Case MemberTypes.Property
                    Return DirectCast(Member, PropertyInfo).GetAccessibility = MethodAttributes.FamORAssem
                Case Else : Return False
            End Select
        End Function
        ''' <summary>For <see cref="Type"/>, <see cref="MethodBase"/> (<see cref="MethodInfo"/> or <see cref="ConstructorInfo"/>), <see cref="PropertyInfo"/>, <see cref="EventInfo"/> or <see cref="FieldInfo"/> indicates its accessibility</summary>
        ''' <param name="Member">Member to indicate accesibility of</param>
        ''' <returns>True if accessibility of member is family (protected)</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="Member"/> is null</exception>
        ''' <exception cref="System.MethodAccessException">The caller does not have permission to reflect on non-public methods and <paramref name="Member"/> is either <see cref="EventInfo"/> or <see cref="PropertyInfo"/>.</exception>
        <Extension()> Public Function IsFamily(ByVal Member As MemberInfo) As Boolean
            If Member Is Nothing Then Throw New ArgumentNullException("Member")
            Select Case Member.MemberType
                Case MemberTypes.TypeInfo, MemberTypes.NestedType
                    With DirectCast(Member, Type)
                        Return (.IsNested AndAlso .IsNestedFamily)
                    End With
                Case MemberTypes.Constructor, MemberTypes.Method
                    Return DirectCast(Member, MethodBase).IsFamily
                Case MemberTypes.Event
                    Return DirectCast(Member, EventInfo).GetAccessibility = MethodAttributes.Family
                Case MemberTypes.Field : Return DirectCast(Member, FieldInfo).IsFamily
                Case MemberTypes.Property
                    Return DirectCast(Member, PropertyInfo).GetAccessibility = MethodAttributes.Family
                Case Else : Return False
            End Select
        End Function
        ''' <summary>Gets maximum visibility of getter and setter of property</summary>
        ''' <param name="prp">Property to check accessibility of</param>
        ''' <returns>Accessibility that is union of accessibilities of getter and setter</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="prp"/> is null</exception>
        ''' <exception cref="System.MethodAccessException">The caller does not have permission to reflect on non-public methods.</exception>
        <Extension()> Public Function GetAccessibility(ByVal prp As PropertyInfo) As MethodAttributes
            If prp Is Nothing Then Throw New ArgumentNullException("prp")
            Return MaxVisibility(If(prp.GetGetMethod(True) Is Nothing, 0, prp.GetGetMethod(True).Attributes), If(prp.GetSetMethod(True) Is Nothing, 0, prp.GetSetMethod(True).Attributes))
        End Function
        ''' <summary>Gets maximum visibility of adder, remover and raiser of event</summary>
        ''' <param name="ev">Event to check accessibility of</param>
        ''' <returns>Accessibility that is union of accessibilities of adder, remover and raiser</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="ev"/> is null</exception>
        ''' <exception cref="System.MethodAccessException">The caller does not have permission to reflect on non-public methods.</exception>
        <Extension()> Public Function GetAccessibility(ByVal ev As EventInfo) As MethodAttributes
            If ev Is Nothing Then Throw New ArgumentNullException("ev")
            Return MaxVisibility(If(ev.GetAddMethod(True) Is Nothing, 0, ev.GetAddMethod(True).Attributes), If(ev.GetRemoveMethod(True) Is Nothing, 0, ev.GetRemoveMethod(True).Attributes), If(ev.GetRaiseMethod(True) Is Nothing, 0, ev.GetRaiseMethod(True).Attributes))
        End Function

        ''' <summary>Gets maximum visibility from visibilities of methods</summary>
        ''' <param name="Visibility">Array of visibilities to test (it can contain any valid value of <see cref="MethodAttributes"/>, non-visibity part will be ignored)</param>
        ''' <returns>Maximum visibility as union of all visibilities in <paramref name="Visibility"/></returns>
        Private Function MaxVisibility(ByVal ParamArray Visibility As MethodAttributes()) As MethodAttributes
            Dim ret As MethodAttributes = 0
            For Each vis In Visibility
                Select Case ret
                    Case MethodAttributes.Private
                        Select Case vis And MethodAttributes.MemberAccessMask
                            Case MethodAttributes.Family, MethodAttributes.FamORAssem, MethodAttributes.FamANDAssem, MethodAttributes.Assembly, MethodAttributes.Public
                                ret = vis And MethodAttributes.MemberAccessMask
                        End Select
                    Case MethodAttributes.Family
                        Select Case vis And MethodAttributes.MemberAccessMask
                            Case MethodAttributes.FamORAssem, MethodAttributes.Assembly
                                ret = MethodAttributes.FamORAssem
                            Case MethodAttributes.Public : ret = MethodAttributes.Public
                        End Select
                    Case MethodAttributes.FamORAssem
                        Select Case vis And MethodAttributes.MemberAccessMask
                            Case MethodAttributes.Public : ret = MethodAttributes.Public
                        End Select
                    Case MethodAttributes.FamANDAssem
                        Select Case vis And MethodAttributes.MemberAccessMask
                            Case MethodAttributes.Family : ret = MethodAttributes.Family
                            Case MethodAttributes.FamORAssem : ret = MethodAttributes.FamORAssem
                            Case MethodAttributes.Assembly : ret = MethodAttributes.Assembly
                            Case MethodAttributes.Public : ret = MethodAttributes.Public
                        End Select
                    Case MethodAttributes.Assembly
                        Select Case vis And MethodAttributes.MemberAccessMask
                            Case MethodAttributes.FamORAssem, MethodAttributes.Assembly
                                ret = MethodAttributes.FamORAssem
                            Case MethodAttributes.Public : ret = MethodAttributes.Public
                        End Select
                    Case Else
                        ret = vis And MethodAttributes.MemberAccessMask
                End Select
                If ret = MethodAttributes.Public Then Return ret 'It cannot be better
            Next vis
            Return ret
        End Function

        ''' <summary>Gets value indicating if member should be considered static</summary>
        ''' <param name="Member">Member to check</param>
        ''' <returns>True if member should or can be considered static</returns>
        ''' <remarks>For <see cref="Type"/> always returns true. For <see cref="PropertyInfo"/> and <see cref="MethodInfo"/> returns true only if all non-other accessors are static</remarks>
        ''' <exception cref="ArgumentNullException"><paramref name="Member"/> is null</exception>
        ''' <exception cref="System.MethodAccessException">The caller does not have permission to reflect on non-public methods and <paramref name="Member"/> is <see cref="EventInfo"/> or <see cref="PropertyInfo"/>.</exception>
        <Extension()> Public Function IsStatic(ByVal Member As MemberInfo) As Boolean
            If Member Is Nothing Then Throw New ArgumentNullException("Member")
            Select Case Member.MemberType
                Case MemberTypes.Constructor, MemberTypes.Method
                    Return DirectCast(Member, MethodBase).IsStatic
                Case MemberTypes.Event
                    With DirectCast(Member, EventInfo)
                        Return _
                            ((.GetAddMethod(True) IsNot Nothing AndAlso .GetAddMethod(True).IsStatic) OrElse .GetAddMethod(True) Is Nothing) AndAlso _
                            ((.GetRemoveMethod(True) IsNot Nothing AndAlso .GetRemoveMethod(True).IsStatic) OrElse .GetRemoveMethod(True) Is Nothing) AndAlso _
                            ((.GetRaiseMethod(True) IsNot Nothing AndAlso .GetRaiseMethod(True).IsStatic) OrElse .GetRaiseMethod(True) Is Nothing)
                    End With
                Case MemberTypes.Field : Return DirectCast(Member, FieldInfo).IsStatic
                Case MemberTypes.Property
                    With DirectCast(Member, PropertyInfo)
                        Return _
                            ((.GetGetMethod(True) IsNot Nothing AndAlso .GetGetMethod(True).IsStatic) OrElse .GetGetMethod(True) Is Nothing) AndAlso _
                            ((.GetSetMethod(True) IsNot Nothing AndAlso .GetSetMethod(True).IsStatic) OrElse .GetSetMethod(True) Is Nothing)
                    End With
                Case Else : Return True
            End Select
        End Function
        ''' <summary>Gets value indicating if member should be considered final (it cannot be overriden or inherited)</summary>
        ''' <param name="Member">Member to check</param>
        ''' <returns>True if memberis final</returns>
        ''' <remarks>For <see cref="FieldInfo"/> always returns true. For <see cref="EventInfo"/> and <see cref="PropertyInfo"/> all non-other members must be final to return true.</remarks>
        ''' <exception cref="ArgumentNullException"><paramref name="Member"/> is null</exception>
        ''' <exception cref="System.MethodAccessException">The caller does not have permission to reflect on non-public methods and <paramref name="Member"/> is <see cref="EventInfo"/> or <see cref="PropertyInfo"/>.</exception>
        <Extension()> Public Function IsFinal(ByVal Member As MemberInfo) As Boolean
            If Member Is Nothing Then Throw New ArgumentNullException("Member")
            Select Case Member.MemberType
                Case MemberTypes.Constructor, MemberTypes.Method
                    Return DirectCast(Member, MethodBase).IsFinal
                Case MemberTypes.Event
                    With DirectCast(Member, EventInfo)
                        Return _
                            ((.GetAddMethod(True) IsNot Nothing AndAlso .GetAddMethod(True).IsFinal) OrElse .GetAddMethod(True) Is Nothing) AndAlso _
                            ((.GetRemoveMethod(True) IsNot Nothing AndAlso .GetRemoveMethod(True).IsFinal) OrElse .GetRemoveMethod(True) Is Nothing) AndAlso _
                            ((.GetRaiseMethod(True) IsNot Nothing AndAlso .GetRaiseMethod(True).IsFinal) OrElse .GetRaiseMethod(True) Is Nothing)
                    End With
                Case MemberTypes.Property
                    With DirectCast(Member, PropertyInfo)
                        Return _
                            ((.GetGetMethod(True) IsNot Nothing AndAlso .GetGetMethod(True).IsFinal) OrElse .GetGetMethod(True) Is Nothing) AndAlso _
                            ((.GetSetMethod(True) IsNot Nothing AndAlso .GetSetMethod(True).IsFinal) OrElse .GetSetMethod(True) Is Nothing)
                    End With
                Case MemberTypes.TypeInfo, MemberTypes.NestedType
                    Return DirectCast(Member, Type).IsSealed
                Case Else : Return True
            End Select
        End Function
#End Region
        ''' <summary>Searches for property given method belongs to</summary>
        ''' <param name="Method">Method to search property for</param>
        ''' <param name="GetSetOnly">Search only for getters and setters</param>
        ''' <param name="Inherit">Search within methods of base types</param>
        ''' <returns>First property that has <paramref name="Method"/> as one of its accessors</returns>
        ''' <remarks>Search is done only within type where <paramref name="Method"/> is declared and optionally within it's base types</remarks>
        ''' <exception cref="ArgumentNullException"><paramref name="Method"/> is null</exception>
        <Extension()> Public Function GetProperty(ByVal Method As MethodInfo, Optional ByVal GetSetOnly As Boolean = False, Optional ByVal Inherit As Boolean = False) As PropertyInfo
            If Method Is Nothing Then Throw New ArgumentNullException("Method")
            If Method.DeclaringType Is Nothing Then Return Nothing
            For Each prp In Method.DeclaringType.GetProperties(BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Static Or BindingFlags.Instance Or If(Inherit, BindingFlags.Default, BindingFlags.DeclaredOnly))
                If GetSetOnly Then
                    If Method.Equals(prp.GetGetMethod(Not Method.IsPublic)) Then Return prp
                    If Method.Equals(prp.GetSetMethod(Not Method.IsPublic)) Then Return prp
                Else
                    For Each other In prp.GetAccessors(Not Method.IsPublic)
                        If Method.Equals(other) Then Return prp
                    Next other
                End If
            Next prp
            Return Nothing
        End Function
        ''' <summary>Searches for event given method belongs to</summary>
        ''' <param name="Method">Method to search event for</param>
        ''' <param name="StandardOnly">Search only for addres, removers and raisers</param>
        ''' <param name="Inherit">Search within methods of base types</param>
        ''' <returns>First event that has <paramref name="Method"/> as one of its accessors</returns>
        ''' <remarks>Search is done only within type where <paramref name="Method"/> is declared and optionally within it's base types</remarks>
        ''' <exception cref="ArgumentNullException"><paramref name="Method"/> is null</exception>
        <Extension()> Public Function GetEvent(ByVal Method As MethodInfo, Optional ByVal StandardOnly As Boolean = False, Optional ByVal Inherit As Boolean = False) As EventInfo
            If Method Is Nothing Then Throw New ArgumentNullException("Method")
            If Method.DeclaringType Is Nothing Then Return Nothing
            For Each ev In Method.DeclaringType.GetEvents(BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Static Or BindingFlags.Instance Or If(Inherit, BindingFlags.Default, BindingFlags.DeclaredOnly))
                If Method.Equals(ev.GetAddMethod(Not Method.IsPublic)) Then Return ev
                If Method.Equals(ev.GetRemoveMethod(Not Method.IsPublic)) Then Return ev
                If Method.Equals(ev.GetRaiseMethod(Not Method.IsPublic)) Then Return ev
                If Not StandardOnly Then
                    For Each other In ev.GetOtherMethods(Not Method.IsPublic)
                        If Method.Equals(other) Then Return ev
                    Next other
                End If
            Next ev
            Return Nothing
        End Function
        ''' <summary>Gets value indicating whether and if which the function is operator</summary>
        ''' <param name="Method">Method to investigate</param>
        ''' <param name="NonStandard">Also include operators that are not part of CLI standard (currently VB \, ^ and &amp; operators are supported)</param>
        ''' <returns>If function is operator returns one of <see cref="Operators"/> constants. If function is not operator (or it seems to be a operator but does not fit to operator it pretends to be) returns <see cref="Operators.no"/>.</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="Method"/> is null</exception>
        <Extension()> Function IsOperator(ByVal Method As MethodInfo, Optional ByVal NonStandard As Boolean = True) As Operators
            If Method Is Nothing Then Throw New ArgumentNullException("Method")
            If Method.IsSpecialName AndAlso Method.IsStatic AndAlso Method.Name.StartsWith("op_") AndAlso [Enum].GetNames(GetType(Operators)).Contains(Method.Name.Substring(3)) AndAlso Not Method.ReturnType.Equals(GetType(System.Void)) Then
                Dim Op As Operators = [Enum].Parse(GetType(Operators), Method.Name.Substring(3))
                If Op.NumberOfOperands <> Method.GetParameters.Length Then Return Operators.no
                If Not NonStandard AndAlso Not Op.IsStandard Then Return Operators.no
                Return Op
            Else
                Return Operators.no
            End If
        End Function
        ''' <summary>Gets number of operands of given operator</summary>
        ''' <param name="Operator">Operator to get number of operands of</param>
        ''' <returns>And-combination of <paramref name="Operator"/> and <see cref="Operators_masks.NoOfOperands"/></returns>
        <Extension()> Function NumberOfOperands(ByVal [Operator] As Operators) As Byte
            Return [Operator] And Operators_masks.NoOfOperands
        End Function
        ''' <summary>Gets value indicating if given operator is standard CLI operator</summary>
        ''' <param name="Operator">Operator to get information for</param>
        ''' <returns>Negation of and-combination of <paramref name="Operator"/> and <see cref="Operators_masks.NonStandard"/></returns>
        <Extension()> Function IsStandard(ByVal [Operator] As Operators) As Boolean
            Return Not ([Operator] And Operators_masks.NonStandard)
        End Function
        ''' <summary>Gets value indicating if operator is sassignment operator</summary>
        ''' <param name="Operator">Operator to get information for</param>
        ''' <returns>And-combination of <paramref name="Operator"/> and <see cref="Operators_masks.Assignment"/></returns>
        <Extension()> Function IsAssignment(ByVal [Operator] As Operators) As Boolean
            Return [Operator] And Operators_masks.Assignment
        End Function
        ''' <summary>Gets interfaces implemented by given type</summary>
        ''' <param name="Type">Type to get interfaces from</param>
        ''' <param name="Inherit">True to get all interfaces, false to get only interfaces implemented by this type directly</param>
        ''' <returns>Interfaces inplemented by this type. Whether all or only those implemented by this type directly depends on <paramref name="Inherit"/>.</returns>
        <Extension()> Public Function GetImplementedInterfaces(ByVal Type As Type, Optional ByVal Inherit As Boolean = False) As IEnumerable(Of Type)
            If Inherit Then Return Type.GetInterfaces
            Return From MyInterface In Type.GetInterfaces _
                Where Not UnionAll(If(Type.BaseType Is Nothing, DirectCast(New List(Of Type), IEnumerable(Of Type)), Type.BaseType.GetInterfaces), _
                    From MyInterface2 In Type.GetInterfaces _
                        Select DirectCast(MyInterface2.GetInterfaces, IEnumerable(Of Type)) _
                    ).Contains(MyInterface) _
                Select MyInterface
        End Function
        ''' <summary>Gets namespace of given <see cref="System.Type"/> as instance of <see cref="NamespaceInfo"/></summary>
        ''' <param name="Type">Type to get namespace of</param>
        ''' <returns><see cref="NamespaceInfo"/> constructed from <paramref name="Type"/>.<see cref="Type.[Module]">Module</see> and <paramref name="Type"/>.<see cref="Type.[Namespace]">Namespace</see>.</returns>
        ''' <remarks>Each type has namespace even when name of the namespace is an empty <see cref="String"/>.</remarks>
        ''' <exception cref="ArgumentNullException"><paramref name="Type"/> is null</exception>
        ''' <version version="1.5.2">Added <see cref="ArgumentNullException"/></version>
        <Extension()> Public Function GetNamespace(ByVal Type As Type) As NamespaceInfo
            If Type Is Nothing Then Throw New ArgumentNullException("Type")
            Return New NamespaceInfo(Type.Module, Type.Namespace)
        End Function
        ''' <summary>Gets value indicating if method is global method</summary>
        ''' <param name="Method">Method to test is it is global</param>
        ''' <returns>True when <paramref name="Method"/>.<see cref="MethodInfo.DeclaringType">DeclaringType</see> is null</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="Method"/> is null</exception>
        ''' <version version="1.5.2">Function introduced</version>
        <Extension(), EditorBrowsable(EditorBrowsableState.Advanced)> Function IsGlobal(ByVal Method As MethodInfo) As Boolean
            If Method Is Nothing Then Throw New ArgumentNullException("Method")
            Return Method.DeclaringType Is Nothing
        End Function
        ''' <summary>Gets value indicating if field is global field</summary>
        ''' <param name="Field">Field to test is it is global</param>
        ''' <returns>True when <paramref name="Field"/>.<see cref="MethodInfo.DeclaringType">DeclaringType</see> is null</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="Field"/> is null</exception>
        ''' <version version="1.5.2">Function introduced</version>
        <Extension(), EditorBrowsable(EditorBrowsableState.Advanced)> Function IsGobal(ByVal Field As FieldInfo) As Boolean
            If Field Is Nothing Then Throw New ArgumentNullException("Field")
            Return Field.DeclaringType Is Nothing
        End Function
        ''' <summary>Gets declaring namespace of global method</summary>
        ''' <param name="Method">Global method to get namespace of</param>
        ''' <returns>Namespace <paramref name="Method"/> contains in its name; or null when <paramref name="Method"/> is not global method</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="Method"/> is null</exception>
        ''' <version version="1.5.2">Function introduced</version>
        <Extension(), EditorBrowsable(EditorBrowsableState.Advanced)> Public Function GetNamespace(ByVal Method As MethodInfo) As NamespaceInfo
            If Method Is Nothing Then Throw New ArgumentNullException("Method")
            If Method.DeclaringType IsNot Nothing Then Return Nothing
            Dim NameParts = Method.Name.Split("."c)
            If NameParts.Length = 1 Then Return New NamespaceInfo(Method.[Module], "")
            Return New NamespaceInfo(Method.Module, String.Join("."c, NameParts, 0, NameParts.Length - 2))
        End Function
        ''' <summary>Gets declaring namespace of global field</summary>
        ''' <param name="Field">Global field to get namespace of</param>
        ''' <returns>Namespace <paramref name="Field"/> contains in its name; or null when <paramref name="Field"/> is not global field</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="Field"/> is null</exception>
        ''' <version version="1.5.2">Function introduced</version>
        <Extension(), EditorBrowsable(EditorBrowsableState.Advanced)> Public Function GetNamespace(ByVal Field As FieldInfo) As NamespaceInfo
            If Field Is Nothing Then Throw New ArgumentNullException("Field")
            If Field.DeclaringType IsNot Nothing Then Return Nothing
            Dim NameParts = Field.Name.Split("."c)
            If NameParts.Length = 1 Then Return New NamespaceInfo(Field.[Module], "")
            Return New NamespaceInfo(Field.Module, String.Join("."c, NameParts, 0, NameParts.Length - 2))
        End Function
#Region "Operators"
        ''' <summary>Gets operators of given kind defined by given type</summary>
        ''' <param name="Type">Type to look for operators on</param>
        ''' <param name="Operator">Type of operator to look for</param>
        ''' <exception cref="ArgumentNullException"><paramref name="Type"/> is null</exception>
        ''' <returns>Array of operators of kind <paramref name="Operator"/> specified for <paramref name="Type"/>. An empty aray when no operator was found.</returns>
        ''' <remarks>This overload looks only for public operators.</remarks>
        ''' <version stage="Nightly" version="1.5.2">Function introduced</version>
        <Extension()> Public Function GetOperators(ByVal Type As Type, ByVal [Operator] As Operators) As MethodInfo()
            If Type Is Nothing Then Throw New ArgumentNullException("Type")
            Return Type.GetOperators([Operator], BindingFlags.Public)
        End Function
        ''' <summary>Gets operators of given kind defined by given type</summary>
        ''' <param name="Type">Type to look for operators on</param>
        ''' <param name="Operator">Type of operator to look for</param>
        ''' <param name="BindingAttr">A bitmask comprised of one or more <see cref="System.Reflection.BindingFlags"/> that specify how the search is conducted. <see cref="BindingFlags.Instance"/> is ignored.</param>
        ''' <exception cref="ArgumentNullException"><paramref name="Type"/> is null</exception>
        ''' <returns>Array of operators of kind <paramref name="Operator"/> specified for <paramref name="Type"/>. An empty aray when no operator was found.</returns>
        ''' <version stage="Nightly" version="1.5.2">Function introduced</version>
        <Extension()> Public Function GetOperators(ByVal Type As Type, ByVal [Operator] As Operators, ByVal BindingAttr As BindingFlags) As MethodInfo()
            If Type Is Nothing Then Throw New ArgumentNullException("Type")
            Return (From Method In Type.GetMethods((BindingFlags.Static Or BindingAttr) And Not BindingFlags.Instance) _
                    Where Method.IsOperator = [Operator] _
                    Select Method).ToArray
        End Function
        ''' <summary>Gets all operator defined by given type</summary>
        ''' <param name="Type">Type to look for operators on</param>
        ''' <exception cref="ArgumentNullException"><paramref name="Type"/> is null</exception>
        ''' <returns>Array of operators defined by <paramref name="Type"/>. An empty array when <paramref name="Type"/> defines no operaors.</returns>
        ''' <remarks>This overload looks only for public operators.</remarks>
        ''' <version stage="Nightly" version="1.5.2">Function introduced</version>
        <Extension()> Public Function GetOperators(ByVal Type As Type) As MethodInfo()
            If Type Is Nothing Then Throw New ArgumentNullException("Type")
            Return Type.GetOperators(BindingFlags.Public)
        End Function
        ''' <summary>Gets all operator defined by given type</summary>
        ''' <param name="Type">Type to look for operators on</param>
        ''' <exception cref="ArgumentNullException"><paramref name="Type"/> is null</exception>
        ''' <returns>Array of operators defined by <paramref name="Type"/>. An empty array when <paramref name="Type"/> defines no operaors.</returns>
        ''' <remarks>This overload looks only for public operators.</remarks>
        ''' <param name="BindingAttr">A bitmask comprised of one or more <see cref="System.Reflection.BindingFlags"/> that specify how the search is conducted. <see cref="BindingFlags.Instance"/> is ignored.</param>
        ''' <version stage="Nightly" version="1.5.2">Function introduced</version>
        <Extension()> Public Function GetOperators(ByVal Type As Type, ByVal BindingAttr As BindingFlags) As MethodInfo()
            If Type Is Nothing Then Throw New ArgumentNullException("Type")
            Return (From Method In Type.GetMethods((BindingFlags.Static Or BindingAttr) And Not BindingFlags.Instance) _
                   Where Method.IsOperator <> Operators.no _
                   Select Method).ToArray
        End Function
        ''' <summary>Gets all operators that can be possibly used to cast from one type to another</summary>
        ''' <param name="TFrom">Type to cast from</param>
        ''' <param name="TTo">Type to cast to</param>
        ''' <returns>Array of implicit and explicit public cast operator defined on types <paramref name="TFrom"/> and <paramref name="TTo"/> accepting <paramref name="TFrom"/> (or its base type) as parameter and returning <paramref name="TTo"/> (or derived type). Base and derived type are in meanig of <see cref="Type.IsAssignableFrom"/>.</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="TFrom"/> or <paramref name="TTo"/> is null.</exception>
        ''' <version version="1.5.2">Function introduced</version>
        <EditorBrowsable(EditorBrowsableState.Advanced)> _
        Public Function GetCastOperators(ByVal TFrom As Type, ByVal TTo As Type) As Reflection.MethodInfo()
            If TFrom Is Nothing Then Throw New ArgumentNullException("TFrom")
            If TTo Is Nothing Then Throw New ArgumentNullException("TTo")
            Return (From op In TFrom.GetOperators(Operators.Implicit).Union(TFrom.GetOperators(Operators.Explicit)).Union(TTo.GetOperators(Operators.Implicit)).Union(TTo.GetOperators(Operators.Explicit)) _
                   Where op.GetParameters()(0).ParameterType.IsAssignableFrom(TFrom) AndAlso TTo.IsAssignableFrom(op.ReturnType) _
                   Select op).ToArray
        End Function
        ''' <summary>FInds best-fit (most specific) cast operator from one type to another</summary>
        ''' <param name="TFrom">Type to cast from</param>
        ''' <param name="TTo">Type to cast to</param>
        ''' <returns>The best operator to be used to cast type <paramref name="TFrom"/> to type <paramref name="TTo"/>, null if no operator was found</returns>
        ''' <exception cref="AmbiguousMatchException">Operators were found, but no one is most specific.</exception>
        ''' <remarks>Operators are obtained using <see cref="GetCastOperators"/> and then specificity is evaluated.
        ''' <list type="numbered">
        ''' <item>Only operatrs which argument is assignale from <paramref name="TFrom"/> and return type can be assigned to <paramref name="TTo"/> are considered. Required custom modifiers (modreq) of argument and return value must not be present.</item>
        ''' <item>Operators are ordered by distance (<see cref="ComputeDistance"/>) of operand and <paramref name="TFrom"/>, then by distance of <paramref name="TTo"/> and return type and then implicit befor explicit.</item>
        ''' <item>Firts operator in after such ordering is returned. If more operators has same order, ordering continues.</item>
        ''' <item>Operators are ordered by declaring type. First operators declared directly on <paramref name="TFrom"/>, second operators declared directly on <paramref name="TTo"/>, third operators declard directly on immediate base of <paramref name="TFrom"/>, fourth operators declared directly on immediate base of <paramref name="TTo"/>, fifth operators declared directly on immediate base of immediate base of <paramref name="TFrom"/> etc. <see cref="ComputeDistance"/> is used.</item>
        ''' <item>Firts operator in after such ordering is returned. If more operators has same order, ordering continues.</item>
        ''' <item>Operators are ordered by CLS-compliance. CLS-compilant first.</item>
        ''' <item>Firts operator in after such ordering is returned. If more operators has same order, ordering continues.</item>
        ''' <item>Operators are ordered by sum of numbers of optional custom modifiesr (modopt) on parameter and return value.</item>
        ''' <item>Firts operator in after such ordering is returned. If more operators has same order, <see cref="AmbiguousMatchException"/> is thrown.</item>
        ''' </list></remarks>
        <EditorBrowsable(EditorBrowsableState.Advanced)> _
        Public Function FindBestFitCastOperator(ByVal TFrom As Type, ByVal TTo As Type) As Reflection.MethodInfo
            Dim Operators = (From op In GetCastOperators(TFrom, TTo) _
                             Where op.GetParameters.Length = 1 AndAlso op.GetParameters()(0).GetRequiredCustomModifiers.Length = 0 AndAlso op.ReturnParameter.GetRequiredCustomModifiers.Length = 0 _
                            Select [Operator] = op, DistanceIn = ComputeDistance(op.GetParameters()(0).ParameterType, TFrom), DistanceOut = ComputeDistance(TTo, op.ReturnType), IsImplicit = op.IsOperator = ReflectionT.Operators.Implicit _
                            Order By DistanceIn, DistanceOut, If(IsImplicit, 0, 1)).ToArray
            If Operators.Length = 1 Then Return Operators(0).Operator
            If Operators.Length = 0 Then Return Nothing
            Dim Best = (From op In Operators _
                        Where op.DistanceIn = Operators(0).DistanceIn AndAlso op.DistanceOut = Operators(0).DistanceOut AndAlso op.IsImplicit = Operators(0).IsImplicit _
                        Select [Operator] = op.Operator, Order = _
                                 If(op.Operator.DeclaringType.Equals(TFrom), 0, _
                                 If(op.Operator.DeclaringType.Equals(TTo), 1, _
                                 If(op.Operator.DeclaringType.IsAssignableFrom(TFrom), ComputeDistance(op.Operator.DeclaringType, TFrom) * 2 + 1, _
                                 If(op.Operator.DeclaringType.IsAssignableFrom(TTo), ComputeDistance(op.Operator.DeclaringType, TTo) * 2 + 2, Integer.MaxValue)))) _
                        Order By Order).ToArray
            If Best.Length = 1 Then Return Best(0).Operator
            If Best(0).Order <> Best(1).Order Then Return Best(0).Operator
            Dim Best2 = (From op In Best _
                         Where op.Order = Best(0).Order _
                         Select [Operator] = op.Operator, CLCA = op.Operator.GetAttribute(Of CLSCompliantAttribute)() _
                         Select [Operator], CLCARank = If(CLCA Is Nothing, 0, If(CLCA.IsCompliant, 0, 1)) _
                         Order By CLCARank).ToArray
            If Best2.Length = 1 Then Return Best2(0).Operator
            If Best2(0).CLCARank <> Best2(1).CLCARank Then Return Best2(0).Operator
            Dim Best3 = (From op In Best2 _
                         Where op.CLCARank = Best2(0).CLCARank _
                         Select op.Operator, Rank = op.Operator.GetParameters()(0).GetOptionalCustomModifiers.Length + op.Operator.ReturnParameter.GetOptionalCustomModifiers.Length _
                         Order By Rank).ToArray
            If Best3.Length = 1 Then Return Best3(0).Operator
            If Best3(0).Rank = Best3(1).Rank Then Return Best3(0).Operator
            Throw New AmbiguousMatchException(ResourcesT.Exceptions.NoCastOperatorIsMostSpecific)
        End Function
        ''' <summary>Numerically evaluates distance between base type and derived type</summary>
        ''' <param name="BaseType">Type to be base of <paramref name="DerivedType"/></param>
        ''' <param name="DerivedType">Type to be derived from <paramref name="BaseType"/></param>
        ''' <returns>Value numericaly evaluating distance in inheritance hierarchy of types <paramref name="BaseType"/> and <paramref name="DerivedType"/>.</returns>
        ''' <remarks><paramref name="DerivedType"/> should be derived from <paramref name="BaseType"/> in way of <see cref="Type.IsAssignableFrom"/> or <paramref name="BaseType"/> should be underlying type of enumeration, when <paramref name="DerivedType"/> is enumeration.
        ''' <para>In case <paramref name="BaseType"/> and <paramref name="DerivedType"/> are swapped, negative value is returned.</para>
        ''' <para>Following rules apply</para>
        ''' <list type="table"><listheader><term>Rule</term><description>Return value</description></listheader>
        ''' <item><term><paramref name="BaseType"/> equals to <paramref name="DerivedType"/></term><description>0 (zero)</description></item>
        ''' <item><term><paramref name="BaseType"/> is not assignable from <paramref name="DerivedType"/>, but <paramref name="DerivedType"/> is assignable from <paramref name="BaseType"/></term><description>Function is called with parameters swapped and negated result is returned.</description></item>
        ''' <item><term><paramref name="DerivedType"/> is enum and its underlying type equals to <paramref name="BaseType"/></term><description>1</description></item>
        ''' <item><term><paramref name="BaseType"/> is enum and its underlying type equals to <paramref name="DerivedType"/></term><description>-1</description></item>
        ''' <item><term><paramref name="BaseType"/> is <see cref="Object"/></term><description><see cref="Integer.MaxValue"/></description></item>
        ''' <item><term><paramref name="BaseType"/> is <see cref="ValueType"/> and <paramref name="DerivedType"/> is value type</term><description><see cref="Integer.MaxValue"/> - 1</description></item>
        ''' <item><term><paramref name="BaseType"/> is class and <paramref name="DerivedType"/> derives from <paramref name="BaseType"/></term><description>Number of inheritance levels between <paramref name="DerivedType"/> and <paramref name="BaseType"/>. 1 direct inheritance, 2 when <paramref name="DerivedType"/> directly derives from class which directly derives from <paramref name="BaseType"/> etc.</description></item>
        ''' <item><term><paramref name="BaseType"/> is interface and <paramref name="DerivedType"/> directly implements it</term><description><see cref="Integer.MaxValue"/> - 4</description></item>
        ''' <item><term><paramref name="BaseType"/> is interface and <paramref name="DerivedType"/> derives from type which implements it</term><see cref="Integer.MaxValue"/> - 3</item>
        ''' <item><term><paramref name="BaseType"/> is generic parameter and <paramref name="DerivedType"/> is one of its constraints</term><description>1</description></item>
        ''' <item><term><paramref name="BaseType"/> is generic parameter and it has constraint which is not <paramref name="DerivedType"/>, but is assignable from <paramref name="DerivedType"/></term>Function is called for the constraint and <paramref name="DerivedType"/> and its result is incremented by 1. Minimum from all these situations (in case more constraints is assignable from <paramref name="DerivedType"/>) is returned.</item>
        ''' <item><term>None of 2 situations above are true and <paramref name="BaseType"/> is generic parameter assignable from <paramref name="DerivedType"/></term><description><see cref="Integer.MaxValue"/> - 4</description></item>
        ''' <item><term>Neither <paramref name="BaseType"/> is assignable from <paramref name="DerivedType"/> nor <paramref name="DerivedType"/> is assignable from <paramref name="BaseType"/> and neither <paramref name="BaseType"/> is underlying type of enumeration <paramref name="DerivedType"/> nor <paramref name="DerivedType"/> is underlying type of enumeration <paramref name="BaseType"/></term><description><see cref="ArgumentException"/> is thrown.</description></item>
        ''' </list></remarks>
        ''' <exception cref="ArgumentException">Types <paramref name="BaseType"/> and <paramref name="DerivedType"/> are not related in terms of class inheritance, iterface implementation, generic constraints and enum underlying type.</exception>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <seelaso cref="Type.IsAssignableFrom"/><seelaso cref="Type.IsInterface"/><seelaso cref="Type.IsValueType"/><seelaso cref="Type.Equals"/><seelaso cref="GetImplementedInterfaces"/><seelaso cref="Type.IsGenericParameter"/><seelaso cref="Type.GetGenericParameterConstraints"/><seelaso cref="[Enum].GetUnderlyingType"/>
        <EditorBrowsable(EditorBrowsableState.Advanced)> _
        Public Function ComputeDistance(ByVal BaseType As Type, ByVal DerivedType As Type) As Integer
            'Simple tests
            If BaseType.Equals(DerivedType) Then Return 0
            If Not BaseType.IsAssignableFrom(DerivedType) AndAlso DerivedType.IsAssignableFrom(BaseType) Then _
                Return -ComputeDistance(DerivedType, BaseType)
            If DerivedType.IsEnum AndAlso BaseType.Equals([Enum].GetUnderlyingType(DerivedType)) Then Return 1
            If BaseType.IsEnum AndAlso DerivedType.Equals([Enum].GetUnderlyingType(BaseType)) Then Return -1
            If Not BaseType.IsAssignableFrom(DerivedType) Then Throw New ArgumentException(ResourcesT.Exceptions.Types0And1AreNotCompatible.f(BaseType.FullName, DerivedType.FullName))
            If BaseType.Equals(GetType(Object)) Then Return Integer.MaxValue
            If BaseType.Equals(GetType(ValueType)) AndAlso BaseType.IsValueType Then Return Integer.MaxValue - 1
            If Not BaseType.IsInterface AndAlso Not BaseType.IsGenericParameter Then
                'Base class lookup
                Dim Level = 1I
                Dim CurrentBase = DerivedType.BaseType
                While CurrentBase IsNot Nothing
                    If CurrentBase.Equals(GetType(Object)) Then Return Integer.MaxValue 'Shoudl not happen
                    If CurrentBase.Equals(BaseType) Then Return Level
                    Level += 1
                    CurrentBase = CurrentBase.BaseType
                End While
            ElseIf BaseType.IsInterface Then
                'Interface lookup
                For Each DirectInterface In DerivedType.GetImplementedInterfaces(False)
                    If DirectInterface.Equals(BaseType) Then Return Integer.MaxValue - 4
                Next
                For Each IndirectInterface In DerivedType.GetImplementedInterfaces(True)
                    If IndirectInterface.Equals(BaseType) Then Return Integer.MaxValue - 3
                Next
            ElseIf BaseType.IsGenericParameter Then
                'Generic constraint lookup
                For Each Constraint In BaseType.GetGenericParameterConstraints
                    If Constraint.Equals(BaseType) Then Return 1
                Next
                Dim Min As Integer?
                For Each Constraint In BaseType.GetGenericParameterConstraints
                    If Constraint.IsGenericParameter AndAlso Constraint.IsAssignableFrom(DerivedType) Then
                        Dim Current = ComputeDistance(Constraint, DerivedType) + 1
                        If (Not Min.HasValue OrElse Min > Current) AndAlso Current >= 0 Then Min = Current
                    End If
                Next
                If Min.HasValue Then Return Min
                If BaseType.IsAssignableFrom(DerivedType) Then Return Integer.MaxValue - 4
            End If
            Throw New InvalidOperationException 'SHould not hapen
        End Function
#End Region
#Region "IsMemberOf"
#Region "Type"
        ''' <summary>Gets value indicating if given <see cref="Type"/> or object it is declared on is member of given <see cref="Assembly"/></summary>
        ''' <param name="Type"><see cref="Type"/> to observe parent of</param>
        ''' <param name="Assembly"><see cref="Assembly"/> to test if it is parent of <paramref name="Type"/></param>
        ''' <returns>True if <paramref name="Assembly"/> is declared inside <paramref name="Type"/></returns>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Type"/> or <paramref name="Assembly"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal Type As Type, ByVal Assembly As Assembly) As Boolean
            If Type Is Nothing Then Throw New ArgumentNullException("Type")
            If Assembly Is Nothing Then Throw New ArgumentNullException("Assembly")
            Return Type.Assembly.Equals(Assembly)
        End Function
        ''' <summary>Gets value indicating if given <see cref="Type"/> or object it is declared on is member of given <see cref="[Module]"/></summary>
        ''' <param name="Type"><see cref="Type"/> to observe parent of</param>
        ''' <param name="Module"><see cref="[Module]"/> to test if it is parent of <paramref name="Type"/></param>
        ''' <returns>True if <paramref name="Module"/> is declared inside <paramref name="Type"/></returns>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Type"/> or <paramref name="Module"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal Type As Type, ByVal [Module] As [Module]) As Boolean
            If Type Is Nothing Then Throw New ArgumentNullException("Type")
            If [Module] Is Nothing Then Throw New ArgumentNullException("Module")
            Return Type.Module.Equals([Module])
        End Function
        ''' <summary>Gets value indicating if given <see cref="Type"/> or object it is declared on is member of given <see cref="Type"/></summary>
        ''' <param name="Type"><see cref="Type"/> to observe parent of</param>
        ''' <param name="DeclaringType"><see cref="Type"/> to test if it is parent of <paramref name="Type"/></param>
        ''' <returns>True if <paramref name="DeclaringType"/> is declared inside <paramref name="Type"/></returns>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Type"/> or <paramref name="DeclaringType"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal Type As Type, ByVal DeclaringType As Type) As Boolean
            If Type Is Nothing Then Throw New ArgumentNullException("Type")
            If DeclaringType Is Nothing Then Throw New ArgumentNullException("DeclaringType")
            If Type.IsNested Then
                Return Type.DeclaringType.Equals(DeclaringType) OrElse Type.DeclaringType.IsMemberOf(DeclaringType)
            ElseIf Type.IsGenericParameter AndAlso Type.DeclaringType IsNot Nothing Then
                Return Type.DeclaringType.Equals(DeclaringType) OrElse Type.DeclaringType.IsMemberOf(DeclaringType)
            ElseIf Type.IsGenericParameter AndAlso Type.DeclaringMethod IsNot Nothing Then
                Return Type.DeclaringMethod.IsMemberOf(DeclaringType)
            End If
        End Function
        ''' <summary>Gets value indicating if given <see cref="Type"/> or object it is declared on is member of given <see cref="NamespaceInfo"/></summary>
        ''' <param name="Type"><see cref="Type"/> to observe parent of</param>
        ''' <param name="Namespace"><see cref="NamespaceInfo"/> to test if it is parent of <paramref name="Type"/></param>
        ''' <returns>True if <paramref name="Namespace"/> is declared inside <paramref name="Type"/></returns>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Type"/> or <paramref name="Namespace"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal Type As Type, ByVal [Namespace] As NamespaceInfo) As Boolean
            If Type Is Nothing Then Throw New ArgumentNullException("Type")
            If [Namespace] Is Nothing Then Throw New ArgumentNullException("Namespace")
            If Type.IsNested Then Return Type.DeclaringType.IsMemberOf([Namespace])
            For Each Type In [Namespace].GetTypes
                If Type.Equals(Type) Then Return True
            Next
            Return False
        End Function
        ''' <summary>Gets value indicating if given <see cref="Type"/> or object it is declared on is member of given <see cref="MethodInfo"/></summary>
        ''' <param name="Type"><see cref="Type"/> to observe parent of</param>
        ''' <param name="Method"><see cref="MethodInfo"/> to test if it is parent of <paramref name="Type"/></param>
        ''' <returns>True if <paramref name="Method"/> is declared inside <paramref name="Type"/></returns>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Type"/> or <paramref name="Method"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal Type As Type, ByVal Method As MethodInfo) As Boolean
            If Type Is Nothing Then Throw New ArgumentNullException("Type")
            If Method Is Nothing Then Throw New ArgumentNullException("Method")
            Return Type.IsGenericParameter AndAlso Type.DeclaringMethod IsNot Nothing AndAlso Type.DeclaringMethod.Equals(Method)
        End Function
#End Region
        ''' <summary>Gets value indicating if given <see cref="NamespaceInfo"/> or object it is declared on is member of given <see cref="NamespaceInfo"/></summary>
        ''' <param name="Namespace"><see cref="NamespaceInfo"/> to observe parent of</param>
        ''' <param name="ParentNamespace"><see cref="NamespaceInfo"/> to test if it is parent of <paramref name="Namespace"/></param>
        ''' <returns>True if <paramref name="ParentNamespace"/> is declared inside <paramref name="Namespace"/></returns>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Namespace"/> or <paramref name="ParentNamespace"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal [Namespace] As NamespaceInfo, ByVal ParentNamespace As NamespaceInfo) As Boolean
            If [Namespace] Is Nothing Then Throw New ArgumentNullException("Namespace")
            If ParentNamespace Is Nothing Then Throw New ArgumentNullException("ParentNamespace")
            For Each ns In ParentNamespace.GetNamespaces
                If ns.Equals([Namespace]) Then Return True
            Next
        End Function
        ''' <summary>Gets value indicating if given <see cref="[Module]"/> or object it is declared on is member of given <see cref="Assembly"/></summary>
        ''' <param name="Module"><see cref="[Module]"/> to observe parent of</param>
        ''' <param name="Assembly"><see cref="Assembly"/> to test if it is parent of <paramref name="Module"/></param>
        ''' <returns>True if <paramref name="Assembly"/> is declared inside <paramref name="Module"/></returns>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Module"/> or <paramref name="Assembly"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal [Module] As [Module], ByVal Assembly As Assembly) As Boolean
            If [Module] Is Nothing Then Throw New ArgumentNullException("Module")
            If Assembly Is Nothing Then Throw New ArgumentNullException("Assembly")
            Return [Module].Assembly.Equals(Assembly)
        End Function
#Region "IsMemberOf"
        ''' <summary>Gets value indicating if given <see cref="MemberInfo"/> or object it is declared on is member of given <see cref="Type"/></summary>
        ''' <param name="Member"><see cref="MemberInfo"/> to observe parent of</param>
        ''' <param name="Type"><see cref="Type"/> to test if it is parent of <paramref name="Member"/></param>
        ''' <returns>True if <paramref name="Type"/> is declared inside <paramref name="Member"/></returns>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Member"/> or <paramref name="Type"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal Member As MemberInfo, ByVal Type As Type) As Boolean
            If Member Is Nothing Then Throw New ArgumentNullException("Member")
            If Type Is Nothing Then Throw New ArgumentNullException("Type")
            If TypeOf Member Is Type Then Return DirectCast(Member, Type).IsMemberOf(Type)
            If Member.DeclaringType Is Nothing Then Return False
            Return Member.DeclaringType.Equals(Type) OrElse Member.DeclaringType.IsMemberOf(Type)
        End Function
        ''' <summary>Gets value indicating if given <see cref="MemberInfo"/> or object it is declared on is member of given <see cref="[Module]"/></summary>
        ''' <param name="Member"><see cref="MemberInfo"/> to observe parent of</param>
        ''' <param name="Module"><see cref="[Module]"/> to test if it is parent of <paramref name="Member"/></param>
        ''' <returns>True if <paramref name="Module"/> is declared inside <paramref name="Member"/></returns>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Member"/> or <paramref name="Module"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal Member As MemberInfo, ByVal [Module] As [Module]) As Boolean
            If Member Is Nothing Then Throw New ArgumentNullException("Member")
            If [Module] Is Nothing Then Throw New ArgumentNullException("Module")
            Return Member.Module.Equals([Module])
        End Function
        ''' <summary>Gets value indicating if given <see cref="MemberInfo"/> or object it is declared on is member of given <see cref="Assembly"/></summary>
        ''' <param name="Member"><see cref="MemberInfo"/> to observe parent of</param>
        ''' <param name="Assembly"><see cref="Assembly"/> to test if it is parent of <paramref name="Member"/></param>
        ''' <returns>True if <paramref name="Assembly"/> is declared inside <paramref name="Member"/></returns>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Member"/> or <paramref name="Assembly"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal Member As MemberInfo, ByVal Assembly As Assembly) As Boolean
            If Member Is Nothing Then Throw New ArgumentNullException("Member")
            If Assembly Is Nothing Then Throw New ArgumentNullException("Assembly")
            Return Member.Module.IsMemberOf(Assembly)
        End Function
        ''' <summary>Gets value indicating if given <see cref="MemberInfo"/> or object it is declared on is member of given <see cref="NamespaceInfo"/></summary>
        ''' <param name="Member"><see cref="MemberInfo"/> to observe parent of</param>
        ''' <param name="Namespace"><see cref="NamespaceInfo"/> to test if it is parent of <paramref name="Member"/></param>
        ''' <returns>True if <paramref name="Namespace"/> is declared inside <paramref name="Member"/></returns>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Member"/> or <paramref name="Namespace"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal Member As MemberInfo, ByVal [Namespace] As NamespaceInfo) As Boolean
            If Member Is Nothing Then Throw New ArgumentNullException("Member")
            If [Namespace] Is Nothing Then Throw New ArgumentNullException("Namespace")
            If TypeOf Member Is Type Then
                Return DirectCast(Member, Type).IsMemberOf([Namespace])
            ElseIf Member.DeclaringType Is Nothing AndAlso (TypeOf Member Is MethodInfo OrElse TypeOf Member Is FieldInfo) Then
                Return If(TypeOf Member Is MethodInfo, DirectCast(Member, MethodInfo).GetNamespace, DirectCast(Member, FieldInfo).GetNamespace).Equals([Namespace])
            ElseIf Member.DeclaringType Is Nothing Then
                Return Member.DeclaringType.IsMemberOf([Namespace])
            End If
            For Each Type In [Namespace].GetTypes(False)
                If Member.IsMemberOf(Type) Then Return True
            Next
            For Each Method In [Namespace].GetMethods(BindingFlags.NonPublic Or BindingFlags.Public)
                If Member.IsMemberOf(Method) Then Return True
            Next
            Return False
        End Function
#End Region
        ''' <summary>Gets value indicating if given <see cref="NamespaceInfo"/> or object it is declared on is member of given <see cref="[Module]"/></summary>
        ''' <param name="Namespace"><see cref="NamespaceInfo"/> to observe parent of</param>
        ''' <param name="Module"><see cref="[Module]"/> to test if it is parent of <paramref name="Namespace"/></param>
        ''' <returns>True if <paramref name="Module"/> is declared inside <paramref name="Namespace"/></returns>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Namespace"/> or <paramref name="Module"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal [Namespace] As NamespaceInfo, ByVal [Module] As [Module]) As Boolean
            If [Namespace] Is Nothing Then Throw New ArgumentNullException("Namespace")
            If [Module] Is Nothing Then Throw New ArgumentNullException("Module")
            Return [Namespace].Module.Equals([Module])
        End Function
        ''' <summary>Gets value indicating if given <see cref="NamespaceInfo"/> or object it is declared on is member of given <see cref="Assembly"/></summary>
        ''' <param name="Namespace"><see cref="NamespaceInfo"/> to observe parent of</param>
        ''' <param name="Assembly"><see cref="Assembly"/> to test if it is parent of <paramref name="Namespace"/></param>
        ''' <returns>True if <paramref name="Assembly"/> is declared inside <paramref name="Namespace"/></returns>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Namespace"/> or <paramref name="Assembly"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal [Namespace] As NamespaceInfo, ByVal Assembly As Assembly) As Boolean
            If [Namespace] Is Nothing Then Throw New ArgumentNullException("Namespace")
            If Assembly Is Nothing Then Throw New ArgumentNullException("Assembly")
            Return [Namespace].Module.IsMemberOf(Assembly)
        End Function
        ''' <summary>Gets value indicating if given <see cref="MethodInfo"/> or object it is declared on is member of given <see cref="PropertyInfo"/></summary>
        ''' <param name="Method"><see cref="MethodInfo"/> to observe parent of</param>
        ''' <param name="Property"><see cref="PropertyInfo"/> to test if it is parent of <paramref name="Method"/></param>
        ''' <returns>True if <paramref name="Property"/> is declared inside <paramref name="Method"/></returns>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Method"/> or <paramref name="Property"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal Method As MethodInfo, ByVal [Property] As PropertyInfo) As Boolean
            If Method Is Nothing Then Throw New ArgumentNullException("Method")
            If [Property] Is Nothing Then Throw New ArgumentNullException("Property")
            For Each acc In [Property].GetAccessors(True)
                If acc.Equals(Method) Then Return True
            Next
            Return False
        End Function
        ''' <summary>Gets value indicating if given <see cref="MethodInfo"/> or object it is declared on is member of given <see cref="EventInfo"/></summary>
        ''' <param name="Method"><see cref="MethodInfo"/> to observe parent of</param>
        ''' <param name="Event"><see cref="EventInfo"/> to test if it is parent of <paramref name="Method"/></param>
        ''' <returns>True if <paramref name="Event"/> is declared inside <paramref name="Method"/></returns>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Method"/> or <paramref name="Event"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal Method As MethodInfo, ByVal [Event] As EventInfo) As Boolean
            If Method Is Nothing Then Throw New ArgumentNullException("Method")
            If [Event] Is Nothing Then Throw New ArgumentNullException("Event")
            For Each acc In [Event].GetAccessors(True)
                If acc.Equals(Method) Then Return True
            Next
            Return False
        End Function
        ''' <summary>Gets value indicating if given <see cref="MemberInfo"/> or object it is declared on is member of given <see cref="MethodInfo"/></summary>
        ''' <param name="Member"><see cref="MemberInfo"/> to observe parent of</param>
        ''' <param name="Method"><see cref="MethodInfo"/> to test if it is parent of <paramref name="Member"/></param>
        ''' <returns>True if <paramref name="Method"/> is declared inside <paramref name="Member"/></returns>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Member"/> or <paramref name="Method"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal Member As MemberInfo, ByVal Method As MethodInfo) As Boolean
            If Member Is Nothing Then Throw New ArgumentNullException("Member")
            If Method Is Nothing Then Throw New ArgumentNullException("Method")
            If Member.DeclaringType Is Nothing Then Return False
            Return Member.DeclaringType.IsMemberOf(Method)
        End Function
        ''' <summary>Gets value indicating if given <see cref="MemberInfo"/> or object it is declared on is member of given <see cref="PropertyInfo"/></summary>
        ''' <param name="Member"><see cref="MemberInfo"/> to observe parent of</param>
        ''' <param name="Property"><see cref="PropertyInfo"/> to test if it is parent of <paramref name="Member"/></param>
        ''' <returns>True if <paramref name="Property"/> is declared inside <paramref name="Member"/></returns>
        ''' <remarks>This function is unlikely to return true when <paramref name="Member"/> isnot <see cref="MethodInfo"/> because it is improbable that generic property exists.</remarks>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="2"/> or <paramref name="6"/> is null</exception>
        <Extension(), EditorBrowsable(EditorBrowsableState.Never)> Public Function IsMemberOf(ByVal Member As MemberInfo, ByVal [Property] As PropertyInfo) As Boolean
            If Member Is Nothing Then Throw New ArgumentNullException("Member")
            If [Property] Is Nothing Then Throw New ArgumentNullException("Property")
            If Member.DeclaringType Is Nothing Then Return False
            If TypeOf Member Is MethodInfo AndAlso DirectCast(Member, MethodInfo).IsMemberOf([Property]) Then Return True
            Dim dc As Type = If(TypeOf Member Is Type, Member, Member.DeclaringType)
            If dc Is Nothing Then Return False
            For Each Method In [Property].GetAccessors(True)
                If dc.IsMemberOf(Method) Then Return True
            Next
            Return False
        End Function
        ''' <summary>Gets value indicating if given <see cref="MemberInfo"/> or object it is declared on is member of given <see cref="EventInfo"/></summary>
        ''' <param name="Member"><see cref="MemberInfo"/> to observe parent of</param>
        ''' <param name="Event"><see cref="EventInfo"/> to test if it is parent of <paramref name="Member"/></param>
        ''' <returns>True if <paramref name="Event"/> is declared inside <paramref name="Member"/></returns>
        ''' <remarks>This function is unlikely to return true when <paramref name="Member"/> isnot <see cref="MethodInfo"/> because it is improbable that generic event exists.</remarks>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="2"/> or <paramref name="6"/> is null</exception>
        <Extension(), EditorBrowsable(EditorBrowsableState.Never)> Public Function IsMemberOf(ByVal Member As MemberInfo, ByVal [Event] As EventInfo) As Boolean
            If Member Is Nothing Then Throw New ArgumentNullException("Member")
            If [Event] Is Nothing Then Throw New ArgumentNullException("Event")
            If Member.DeclaringType Is Nothing Then Return False
            If Member.DeclaringType Is Nothing Then Return False
            If TypeOf Member Is MethodInfo AndAlso DirectCast(Member, MethodInfo).IsMemberOf([Event]) Then Return True
            Dim dc As Type = If(TypeOf Member Is Type, Member, Member.DeclaringType)
            If dc Is Nothing Then Return False
            Try
                For Each Method In [Event].GetAccessors(True)
                    If dc.IsMemberOf(Method) Then Return True
                Next
            Catch ex As MethodAccessException
                For Each Method In [Event].GetAccessors(False)
                    If dc.IsMemberOf(Method) Then Return True
                Next
            End Try
            Return False
        End Function
        ''' <summary>Gets value indicating if given <see cref="ParameterInfo"/> or object it is declared on is member of given <see cref="MethodInfo"/></summary>
        ''' <param name="Param"><see cref="ParameterInfo"/> to observe parent of</param>
        ''' <param name="Method"><see cref="MethodInfo"/> to test if it is parent of <paramref name="Param"/></param>
        ''' <returns>True if <paramref name="Method"/> is declared inside <paramref name="Param"/></returns>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Param"/> or <paramref name="Method"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal Param As ParameterInfo, ByVal Method As MethodInfo) As Boolean
            If Param Is Nothing Then Throw New ArgumentNullException("Param")
            If Method Is Nothing Then Throw New ArgumentNullException("Method")
            For Each mParam In Method.GetParameters
                If mParam.Equals(Param) Then Return True
            Next
            Return Param.Equals(Method.ReturnParameter)
        End Function
        ''' <summary>Gets value indicating if given <see cref="ParameterInfo"/> or object it is declared on is member of given <see cref="Type"/></summary>
        ''' <param name="Param"><see cref="ParameterInfo"/> to observe parent of</param>
        ''' <param name="Type"><see cref="Type"/> to test if it is parent of <paramref name="Param"/></param>
        ''' <returns>True if <paramref name="Type"/> is declared inside <paramref name="Param"/></returns>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Param"/> or <paramref name="Type"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal Param As ParameterInfo, ByVal Type As Type) As Boolean
            If Param Is Nothing Then Throw New ArgumentNullException("Param")
            If Type Is Nothing Then Throw New ArgumentNullException("Type")
            Return Param.Member.IsMemberOf(Type)
        End Function
        ''' <summary>Gets value indicating if given <see cref="ParameterInfo"/> or object it is declared on is member of given <see cref="MemberInfo"/></summary>
        ''' <param name="Param"><see cref="ParameterInfo"/> to observe parent of</param>
        ''' <param name="Member"><see cref="MemberInfo"/> to test if it is parent of <paramref name="Param"/></param>
        ''' <returns>True if <paramref name="Member"/> is declared inside <paramref name="Param"/></returns>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Param"/> or <paramref name="Member"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal Param As ParameterInfo, ByVal Member As MemberInfo) As Boolean
            If Param Is Nothing Then Throw New ArgumentNullException("Param")
            If Member Is Nothing Then Throw New ArgumentNullException("Member")
            Return Param.Member.Equals(Member) OrElse Param.Member.IsMemberOf(Member)
        End Function
        ''' <summary>Gets value indicating if given <see cref="ParameterInfo"/> or object it is declared on is member of given <see cref="NamespaceInfo"/></summary>
        ''' <param name="Param"><see cref="ParameterInfo"/> to observe parent of</param>
        ''' <param name="Namespace"><see cref="NamespaceInfo"/> to test if it is parent of <paramref name="Param"/></param>
        ''' <returns>True if <paramref name="Namespace"/> is declared inside <paramref name="Param"/></returns>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Param"/> or <paramref name="Namespace"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal Param As ParameterInfo, ByVal [Namespace] As NamespaceInfo) As Boolean
            If Param Is Nothing Then Throw New ArgumentNullException("Param")
            If [Namespace] Is Nothing Then Throw New ArgumentNullException("Namespace")
            Return Param.Member.IsMemberOf([Namespace])
        End Function
        ''' <summary>Gets value indicating if given <see cref="ParameterInfo"/> or object it is declared on is member of given <see cref="[Module]"/></summary>
        ''' <param name="Param"><see cref="ParameterInfo"/> to observe parent of</param>
        ''' <param name="Module"><see cref="[Module]"/> to test if it is parent of <paramref name="Param"/></param>
        ''' <returns>True if <paramref name="Module"/> is declared inside <paramref name="Param"/></returns>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Param"/> or <paramref name="Module"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal Param As ParameterInfo, ByVal [Module] As [Module]) As Boolean
            If Param Is Nothing Then Throw New ArgumentNullException("Param")
            If [Module] Is Nothing Then Throw New ArgumentNullException("Module")
            Return Param.Member.IsMemberOf([Module])
        End Function
        ''' <summary>Gets value indicating if given <see cref="ParameterInfo"/> or object it is declared on is member of given <see cref="Assembly"/></summary>
        ''' <param name="Param"><see cref="ParameterInfo"/> to observe parent of</param>
        ''' <param name="Assembly"><see cref="Assembly"/> to test if it is parent of <paramref name="Param"/></param>
        ''' <returns>True if <paramref name="Assembly"/> is declared inside <paramref name="Param"/></returns>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Param"/> or <paramref name="Assembly"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal Param As ParameterInfo, ByVal Assembly As Assembly) As Boolean
            If Param Is Nothing Then Throw New ArgumentNullException("Param")
            If Assembly Is Nothing Then Throw New ArgumentNullException("Assembly")
            Return Param.Member.IsMemberOf(Assembly)
        End Function
#Region "Generic"
        ''' <summary>Gets value indicating if given <see cref="ParameterInfo"/> or object it is declared on is member of given CLI object</summary>
        ''' <param name="Param"><see cref="ParameterInfo"/> to observe parent of</param>
        ''' <param name="Parent"><see cref="Object"/> to test if it is parent of <paramref name="Param"/></param>
        ''' <returns>True if <paramref name="Parent"/> is declared inside <paramref name="Param"/></returns>
        ''' <remarks>Supported types of <paramref name="Parent"/> are <see cref="Assembly"/>, <see cref="[Module]"/>, <see cref="NamespaceInfo"/>, <see cref="MemberInfo"/>. For any other type, this function returns false.</remarks>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Param"/> or <paramref name="Parent"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal Param As ParameterInfo, ByVal Parent As Object) As Boolean
            If Param Is Nothing Then Throw New ArgumentNullException("Member")
            If Parent Is Nothing Then Throw New ArgumentNullException("Parent")
            If TypeOf Parent Is Assembly Then : Return Param.IsMemberOf(DirectCast(Parent, Assembly))
            ElseIf TypeOf Parent Is [Module] Then : Return Param.IsMemberOf(DirectCast(Parent, [Module]))
            ElseIf TypeOf Parent Is NamespaceInfo Then : Return Param.IsMemberOf(DirectCast(Parent, NamespaceInfo))
            ElseIf TypeOf Parent Is Type Then : Return Param.IsMemberOf(DirectCast(Parent, Type))
            ElseIf TypeOf Parent Is MemberInfo Then : Return Param.IsMemberOf(DirectCast(Parent, MethodInfo))
            Else : Return False
            End If
        End Function
        ''' <summary>Gets value indicating if given <see cref="MemberInfo"/> or object it is declared on is member of given CLI object</summary>
        ''' <param name="Member"><see cref="MemberInfo"/> to observe parent of</param>
        ''' <param name="Parent"><see cref="Object"/> to test if it is parent of <paramref name="Member"/></param>
        ''' <returns>True if <paramref name="Parent"/> is declared inside <paramref name="Member"/></returns>
        ''' <remarks>Supported types of <paramref name="Parent"/> are <see cref="Assembly"/>, <see cref="[Module]"/>, <see cref="NamespaceInfo"/>, <see cref="Type"/>, <see cref="MethodInfo"/>, <see cref="PropertyInfo"/>, <see cref="EventInfo"/>. For any other type, this function returns false.</remarks>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Member"/> or <paramref name="Parent"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal Member As MemberInfo, ByVal Parent As Object) As Boolean
            If Member Is Nothing Then Throw New ArgumentNullException("Member")
            If Parent Is Nothing Then Throw New ArgumentNullException("Parent")
            If TypeOf Parent Is Assembly Then : Return Member.IsMemberOf(DirectCast(Parent, Assembly))
            ElseIf TypeOf Parent Is [Module] Then : Return Member.IsMemberOf(DirectCast(Parent, [Module]))
            ElseIf TypeOf Parent Is NamespaceInfo Then : Return Member.IsMemberOf(DirectCast(Parent, NamespaceInfo))
            ElseIf TypeOf Parent Is Type Then : Return Member.IsMemberOf(DirectCast(Parent, Type))
            ElseIf TypeOf Parent Is MethodInfo Then : Return Member.IsMemberOf(DirectCast(Parent, MethodInfo))
            ElseIf TypeOf Parent Is PropertyInfo Then : Return Member.IsMemberOf(DirectCast(Parent, PropertyInfo))
            ElseIf TypeOf Parent Is EventInfo Then : Return Member.IsMemberOf(DirectCast(Parent, EventInfo))
            Else : Return False
            End If
        End Function
        ''' <summary>Gets value indicating if given <see cref="NamespaceInfo"/> or object it is declared on is member of given CLI object</summary>
        ''' <param name="Namespace"><see cref="NamespaceInfo"/> to observe parent of</param>
        ''' <param name="Parent"><see cref="Object"/> to test if it is parent of <paramref name="Namespace"/></param>
        ''' <returns>True if <paramref name="Parent"/> is declared inside <paramref name="Namespace"/></returns>
        ''' <remarks>Supported types of <paramref name="Parent"/> are <see cref="Assembly"/>, <see cref="[Module]"/>, <see cref="NamespaceInfo"/>. For any other type, this function returns false.</remarks>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Namespace"/> or <paramref name="Parent"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal [Namespace] As NamespaceInfo, ByVal Parent As Object) As Boolean
            If [Namespace] Is Nothing Then Throw New ArgumentNullException("Namespace")
            If Parent Is Nothing Then Throw New ArgumentNullException("Parent")
            If TypeOf Parent Is Assembly Then : Return [Namespace].IsMemberOf(DirectCast(Parent, Assembly))
            ElseIf TypeOf Parent Is [Module] Then : Return [Namespace].IsMemberOf(DirectCast(Parent, [Module]))
            ElseIf TypeOf Parent Is NamespaceInfo Then : Return [Namespace].IsMemberOf(DirectCast(Parent, NamespaceInfo))
            Else : Return False
            End If
        End Function
        ''' <summary>Gets value indicating if given <see cref="[Module]"/> or object it is declared on is member of given CLI object</summary>
        ''' <param name="Module"><see cref="[Module]"/> to observe parent of</param>
        ''' <param name="Parent"><see cref="Object"/> to test if it is parent of <paramref name="Module"/></param>
        ''' <returns>True if <paramref name="Parent"/> is declared inside <paramref name="Module"/></returns>
        ''' <remarks>Supported types of <paramref name="Parent"/> are <see cref="Assembly"/>. For any other type, this function returns false.</remarks>
        ''' <version version="1.5.2">Function introduced</version>
        ''' <exception cref="ArgumentNullException"><paramref name="Module"/> or <paramref name="Parent"/> is null</exception>
        <Extension()> Public Function IsMemberOf(ByVal [Module] As [Module], ByVal Parent As Object) As Boolean
            If [Module] Is Nothing Then Throw New ArgumentNullException("Namespace")
            If Parent Is Nothing Then Throw New ArgumentNullException("Parent")
            If TypeOf Parent Is Assembly Then : Return [Module].IsMemberOf(DirectCast(Parent, Assembly))
            Else : Return False
            End If
        End Function
#End Region
#End Region
        ''' <summary>Gtes all accessors of given event</summary>
        ''' <param name="Event">Event to get accessors of</param>
        ''' <param name="NonPublic">True to get non-public accessors as well as public</param>
        ''' <returns>Array of all accessors of <paramref name="Event"/></returns>
        ''' <exception cref="ArgumentNullException"><paramref name="Event"/> is null</exception>
        ''' <exception cref="MethodAccessException"><paramref name="NonPublic"/> is true, event accessor is non-public, and the caller does not have permission to reflect on non-public methods. </exception>
        ''' <remarks>If <paramref name="Event"/> does not support <see cref="M:System.Reflection.EventInfo.GetOtherMethods(System.Boolean)"/>, <see cref="M:System.Reflection.EventInfo.GetOtherMethods()"/> is used.</remarks>
        <Extension()> Function GetAccessors(ByVal [Event] As EventInfo, Optional ByVal NonPublic As Boolean = False) As MethodInfo()
            If [Event] Is Nothing Then Throw New ArgumentNullException("Event")
            Dim ret As New List(Of MethodInfo)
            If [Event].GetAddMethod(NonPublic) IsNot Nothing Then ret.Add([Event].GetAddMethod(NonPublic))
            If [Event].GetRemoveMethod(NonPublic) IsNot Nothing Then ret.Add([Event].GetRemoveMethod(NonPublic))
            If [Event].GetRaiseMethod(NonPublic) IsNot Nothing Then ret.Add([Event].GetRaiseMethod(NonPublic))
            Try
                ret.AddRange([Event].GetOtherMethods(NonPublic))
            Catch ex As NotImplementedException
                ret.AddRange([Event].GetOtherMethods())
            End Try
            Return ret.ToArray
        End Function

        ''' <summary>Searches for method given method overrides</summary>
        ''' <param name="Method">Method do find method it overrides</param>
        ''' <returns>Method in base class (or base base class etc.) of class <paramref name="Method"/> is defined in <paramref name="Method"/> overrides; null when no such method is found</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="Method"/> is null</exception>
        ''' <exception cref="ArgumentException"><paramref name="Method"/> is global method (its <see cref="MethodInfo.DeclaringType"/> is null)</exception>
        ''' <remarks>This function searches for method with same name and signature as <paramref name="Method"/> has. Search is done in base class of class <paramref name="Method"/> is defined in, if not found in base class of base class etc.</remarks>
        ''' <version version="1.5.2">Function introduced</version>
        <Extension()> _
        Public Function GetBaseClassMethod(ByVal Method As MethodInfo) As MethodInfo
            'Unit test done
            If Method Is Nothing Then Throw New ArgumentNullException("Method")
            If Method.DeclaringType Is Nothing Then Throw New ArgumentException(ResourcesT.Exceptions.CannotGetBaseClassMethodOfGlobalMethod)
            Dim type As Type = Method.DeclaringType
            Dim base As Type = type.BaseType
            Dim ret As MethodInfo = Nothing
            Do Until base Is Nothing
                If base Is Nothing Then Return Nothing
                If (Method.Attributes And MethodAttributes.NewSlot) = MethodAttributes.NewSlot Then Return Nothing 'Shadows
                For Each BaseMethod In base.GetMethods(BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance Or BindingFlags.DeclaredOnly) '
                    If BaseMethod.Name.Equals(Method.Name, StringComparison.InvariantCulture) AndAlso BaseMethod.IsVirtual AndAlso Not BaseMethod.IsFinal Then
                        If HasSameSignature(Method, BaseMethod, SignatureComparisonStrictness.Strict) Then
                            If ret IsNot Nothing Then Throw New AmbiguousMatchException()
                            ret = BaseMethod
                        End If
                    End If
                Next
                base = base.BaseType
                If ret IsNot Nothing Then Exit Do
            Loop
            Return ret
        End Function
        ''' <summary>Determines if two methods have same signatures. Several levels of signature comparison are available.</summary>
        ''' <param name="a">A <see cref="MethodInfo"/></param>
        ''' <param name="b">A <see cref="MethodInfo"/></param>
        ''' <param name="Strictness">Defines level of comparison</param>
        ''' <returns>True if <paramref name="a"/> and <paramref name="b"/> have same signature in meaning of <paramref name="Strictness"/>; false otherwise</returns>
        ''' <remarks>Signature comparison does not include comparison of custom attributes (with exception of <see cref="InAttribute"/> and <see cref="OutAttribute"/>) and does not include comparison of method attributes (such as if it is private, public or specialname). Callig convention is ignored as well.
        ''' <para>When comparing modreqs and modopts only first-level modifiers are taken in effect. Modifiers applied onto element-type of pointer, refernce or array and onto types of generic type are ignored. This is due to limitation of <see cref="ParameterInfo.GetOptionalCustomModifiers"/> and <see cref="ParameterInfo.GetRequiredCustomModifiers"/>.</para></remarks>
        ''' <exception cref="ArgumentNullException"><paramref name="a"/> or <paramref name="b"/> is null.</exception>
        ''' <version version="1.5.2">Function introduced</version>
        <Extension()> _
        Public Function HasSameSignature(ByVal a As MethodInfo, ByVal b As MethodInfo, ByVal Strictness As SignatureComparisonStrictness) As Boolean
            'Unt test done
            If a Is Nothing Then Throw New ArgumentNullException("a")
            If b Is Nothing Then Throw New ArgumentNullException("b")
            Dim pa = a.GetParameters
            Dim pb = b.GetParameters
            If pa.Length <> pb.Length Then Return False
            ReDim Preserve pa(pa.Length)
            ReDim Preserve pb(pb.Length)
            pa(pa.Length - 1) = a.ReturnParameter
            pb(pb.Length - 1) = b.ReturnParameter
            For i As Integer = 0 To pb.Length - 1
                If i = pb.Length - 1 AndAlso (Strictness And SignatureComparisonStrictness.IgnoreReturn) Then Exit For
                Dim patypefc = pa(i).ParameterType
                Dim pbtypefc = pb(i).ParameterType
                If Strictness And SignatureComparisonStrictness.TreatPointerAsReference Then
                    If patypefc.IsPointer Then patypefc = patypefc.GetElementType.MakeByRefType
                    If pbtypefc.IsPointer Then pbtypefc = pbtypefc.GetElementType.MakeByRefType
                End If
                If Strictness And SignatureComparisonStrictness.IgnoreByRef Then
                    If patypefc.IsByRef Then patypefc = patypefc.GetElementType
                    If pbtypefc.IsByRef Then pbtypefc = pbtypefc.GetElementType
                End If
                If Not patypefc.Equals(pbtypefc) Then Return False
                If (Strictness And SignatureComparisonStrictness.IgnoreDirection) = 0 Then
                    If pa(i).IsOut <> pb(i).IsOut Then Return False
                    If pa(i).IsIn <> pb(i).IsIn Then Return False
                End If
                If (Strictness And SignatureComparisonStrictness.IgnoreModReq) = 0 Then
                    Dim moda = pa(i).GetRequiredCustomModifiers
                    Dim modb = pb(i).GetRequiredCustomModifiers
                    If moda.Length <> modb.Length Then Return False
                    For Each [mod] In moda
                        If Not modb.Contains([mod]) Then Return False
                    Next
                End If
                If (Strictness And SignatureComparisonStrictness.IgnoreModOpt) = 0 Then
                    Dim moda = pa(i).GetOptionalCustomModifiers
                    Dim modb = pb(i).GetOptionalCustomModifiers
                    If moda.Length <> modb.Length Then Return False
                    For Each [mod] In moda
                        If Not modb.Contains([mod]) Then Return False
                    Next
                End If
            Next
            Return True
        End Function
        ''' <summary>Defines how method signature comparison is performed</summary>
        ''' <remarks>This enumeration is treaded as flags, each set or unset. Several predefined combinations of flags also exists.
        ''' <para>When <see cref="SignatureComparisonStrictness.IgnoreByRef"/> and <see cref="SignatureComparisonStrictness.TreatPointerAsReference"/> are both set:
        ''' Both - T* and T&amp; are treated as T. T*&amp; (reference to pointer) is treated as T* and T&amp;* (pointer to reference) is treated as T.</para></remarks>
        <Flags()> _
        Public Enum SignatureComparisonStrictness
            ''' <summary>Set this flag to ignore direction of method parameter. <see cref="InAttribute"/> and <see cref="OutAttribute"/> are ignored. Does not affect testing if parameter is passed by reference or by value.</summary>
            IgnoreDirection = 1
            ''' <summary>Ignore optional modifiers on parameters (modopts). Nested modopts are always ignored i.e. modopts on pointer/reference/array/generic internal type(s).</summary>
            IgnoreModOpt = 2
            ''' <summary>Ignore required modifiers on parameters (modreqs). Nested modreqs are always ignored i.e. modreqs on pointer/reference/array/generic internal type(s).</summary>
            IgnoreModReq = 4
            ''' <summary>Ignore return value completelly (ignores return type and return modopts and modreqs)</summary>
            IgnoreReturn = 8
            ''' <summary>Consider parameter passed by value and by reference to by of same type. Note: Physically the type of such parameters differs.</summary>
            IgnoreByRef = 16
            ''' <summary>Treat pointer to type (*) in same way as reference to type (&amp;, ByRef) - see <see cref="Type.IsByRef"/> and <see cref="Type.IsPointer"/>.
            ''' When combined with <see cref="IgnoreByRef"/>, pointer to type is treated as type itself.</summary>
            TreatPointerAsReference = 32
            ''' <summary>Default. Comparison includes type of parameter, direction, custpm and optional modifiers and does consider parameters passed by value and by reference to be of different type.</summary>
            Strict = 0
            ''' <summary>This how method signatures are compared according to CLS-rules - direction, modopts, modreqs and retun type are ignored. Note: CLS does not ignore return type for op_Implicit and op_Explicit operator methods (use <see cref="CLS">CLS</see> AND NOT <see cref="IgnoreReturn">IgnoreReturn</see> for them).</summary>
            ''' <seelaso cref="IgnoreDirection"/><seelaso cref="IgnoreModOpt"/><seelaso cref="IgnoreModReq"/> <seelaso cref="IgnoreReturn"/>
            CLS = IgnoreDirection Or IgnoreModOpt Or IgnoreModReq Or IgnoreReturn
            ''' <summary>Ignore both - optional and required modifiers (modopts and modreqs)</summary>
            ''' <seelaso cref="IgnoreModOpt"/><seelaso cref="IgnoreModReq"/>
            IgnoreModifiers = IgnoreModOpt Or IgnoreModReq
        End Enum
    End Module

    ''' <summary>Represents reflection namespace</summary>
    ''' <version version="1.5.2" stage="Nightly">Added implementation of <see cref="IEquatable(Of NamespaceInfo)"/></version>
    Public Class NamespaceInfo : Implements IEquatable(Of NamespaceInfo)
        ''' <summary>Contains value of the <see cref="[Module]"/> property</summary>
        Private ReadOnly _Module As [Module]
        ''' <summary>Contains value of the <see cref="Name"/> property</summary>
        Private ReadOnly _Name As String
        ''' <summary>Module the namespace is located in</summary>
        Public ReadOnly Property [Module]() As [Module]
            Get
                Return _Module
            End Get
        End Property
        ''' <summary>Name of namespace. Can be an empty string for global namespace</summary>
        Public ReadOnly Property Name$()
            Get
                Return _Name
            End Get
        End Property
        ''' <summary>Short name of namespace - only part after last dot (.).</summary>
        Public ReadOnly Property ShortName$()
            Get
                If Name = "" Then Return ""
                Dim Parts As String() = Name.Split("."c)
                Return Parts(Parts.Length - 1)
            End Get
        End Property
        ''' <summary>CTor</summary>
        ''' <param name="Module">Module the namespace is defined in</param>
        ''' <param name="Name">Name of namespace</param>
        ''' <exception cref="ArgumentNullException"><paramref name="Module"/> or <paramref name="Name"/> is null</exception>
        Public Sub New(ByVal [Module] As [Module], ByVal Name As String)
            If [Module] Is Nothing Then Throw New ArgumentNullException("Module")
            If Name Is Nothing Then Throw New ArgumentNullException("Name")
            Me._Module = [Module]
            Me._Name = Name
        End Sub
#Region "GetMembers"
        ''' <summary>Gets types located within current namespace</summary>
        ''' <param name="Nested">True to get nested types (types declared inside types in current namepace)</param>
        ''' <returns>Array of types defined in this namespace</returns>
        ''' <exception cref="System.Reflection.ReflectionTypeLoadException">One or more classes in a module could not be loaded.</exception>
        ''' <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        Public Function GetTypes(Optional ByVal Nested As Boolean = False) As Type()
            Return (From Type In Me.Module.GetTypes() Where (Nested OrElse Not Type.IsNested) AndAlso Type.Namespace = Me.Name Select Type).ToArray
        End Function
        ''' <summary>Gets global methods located in current namespace</summary>
        ''' <returns>Array of global methods defined in current namespace (it is in module <see cref="[Module]"/> with name starting with name of this namespace)</returns>
        ''' <version version="1.5.2">Function introduced</version>
        Public Function GetMethods() As MethodInfo()
            Return (From method In Me.Module.GetMethods Where method.GetNamespace.Equals(Me)).ToArray
        End Function
        ''' <summary>Gets global methods located in current namespace</summary>
        ''' <returns>Array of global methods defined in current namespace (it is in module <see cref="[Module]"/> with name starting with name of this namespace)</returns>
        ''' <version version="1.5.2">Function introduced</version>
        Public Function GetFields() As FieldInfo()
            Return (From field In Me.Module.GetFields Where field.GetNamespace.Equals(Me)).ToArray
        End Function

        ''' <summary>Gets global methods located in current namespace</summary>
        ''' <returns>Array of global methods defined in current namespace (it is in module <see cref="[Module]"/> with name starting with name of this namespace)</returns>
        ''' <param name="BindingFlags">A bitwise combination of <see cref="System.Reflection.BindingFlags"/> values that limit the search.</param>
        ''' <version version="1.5.2">Function introduced</version>
        Public Function GetMethods(ByVal BindingFlags As BindingFlags) As MethodInfo()
            Return (From method In Me.Module.GetMethods(BindingFlags) Where method.GetNamespace.Equals(Me)).ToArray
        End Function
        ''' <summary>Gets global methods located in current namespace</summary>
        ''' <returns>Array of global methods defined in current namespace (it is in module <see cref="[Module]"/> with name starting with name of this namespace)</returns>
        ''' <param name="BindingFlags">A bitwise combination of <see cref="System.Reflection.BindingFlags"/> values that limit the search.</param>
        ''' <version version="1.5.2">Function introduced</version>
        Public Function GetFields(ByVal BindingFlags As BindingFlags) As FieldInfo()
            Return (From field In Me.Module.GetFields(BindingFlags) Where field.GetNamespace.Equals(Me)).ToArray
        End Function
#End Region
        ''' <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        ''' <returns>True if <paramref name="obj"/> is <see cref="NamespaceInfo"/> and its <see cref="[Module]"/> equals to <see cref="[Module]"/> of current <see cref="NamespaceInfo"/> and also <see cref="Name">Names</see> or current <see cref="NamespaceInfo"/> and <paramref name="obj"/> equals.</returns>
        ''' <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />. </param>
        ''' <exception cref="T:System.NullReferenceException">The 
        ''' <paramref name="obj" /> parameter is null.</exception>
        Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
            Return TypeOf obj Is NamespaceInfo AndAlso Me.Module.Equals(DirectCast(obj, NamespaceInfo).Module) AndAlso Me.Name = DirectCast(obj, NamespaceInfo).Name
        End Function
        ''' <summary>Compares two <see cref="NamespaceInfo">NamespaceInfos</see> for equality</summary>
        ''' <param name="a">A <see cref="NamespaceInfo"/></param>
        ''' <param name="b">A <see cref="NamespaceInfo"/></param>
        ''' <returns>True if <paramref name="a"/> equals to <paramref name="b"/>.</returns>
        Public Shared Operator =(ByVal a As NamespaceInfo, ByVal b As NamespaceInfo) As Boolean
            Return a.Equals(b)
        End Operator
        ''' <summary>Compares two <see cref="NamespaceInfo">NamespaceInfos</see> for inequality</summary>
        ''' <param name="a">A <see cref="NamespaceInfo"/></param>
        ''' <param name="b">A <see cref="NamespaceInfo"/></param>
        ''' <returns>False if <paramref name="a"/> equals to <paramref name="b"/>.</returns>
        Public Shared Operator <>(ByVal a As NamespaceInfo, ByVal b As NamespaceInfo) As Boolean
            Return Not (a = b)
        End Operator
        ''' <summary>Gets parent namespace of current namespace</summary>
        ''' <returns>If <see cref="Name"/> of current namespace contains no dot an namespace with empty name is returned. If <see cref="Name"/> of current namespace is an empty string, null is returned.</returns>
        Public ReadOnly Property Parent() As NamespaceInfo
            Get
                If Me.Name = "" Then
                    Return Nothing
                ElseIf Me.Name.Contains("."c) Then
                    Return New NamespaceInfo(Me.Module, Me.Name.Substring(0, Me.Name.LastIndexOf("."c)))
                Else
                    Return New NamespaceInfo(Me.Module, "")
                End If
            End Get
        End Property
        ''' <summary>Gets all namespaces immediately contained in this namespace</summary>
        ''' <returns>Array of namespaces in this namespace</returns>
        ''' <remarks>Whe looking for namespaces all types in curret namespace are considered (even non-public). 
        ''' If you want filer some types use overloaded <see cref="M:Tools.ReflectionT.NamespaceInfo.GetNamespaces(System.Predicate`1[System.Type])"/>.</remarks>
        Public Function GetNamespaces() As NamespaceInfo()
            Return GetNamespaces(Function(t As Type) True)
        End Function
        ''' <summary>Gets namespaces immediatelly contained in this namespace when considering only selected types</summary>
        ''' <param name="TypeFiler">This function returns only such namespaces which contain at leas one type for which delegate function <paramref name="TypeFiler"/> returns true</param>
        ''' <returns>Array of namespaces in this namespace</returns>
        Public Function GetNamespaces(ByVal TypeFiler As System.Predicate(Of Type)) As NamespaceInfo()
            Dim NamespaceNames As New List(Of String)
            For Each t As Type In [Module].GetTypes
                If t.Namespace <> "" AndAlso (Me.Name = "" OrElse t.Namespace.StartsWith(Me.Name & ".")) Then
                    Dim NamePart As String
                    If Me.Name = "" AndAlso t.Namespace.Contains("."c) Then
                        NamePart = t.Namespace.Substring(0, t.Namespace.IndexOf("."c))
                    ElseIf Me.Name = "" Then
                        NamePart = t.Namespace
                    Else
                        NamePart = t.Namespace.Substring(Me.Name.Length + 1)
                        If NamePart.Contains("."c) Then NamePart = NamePart.Substring(0, NamePart.IndexOf("."c))
                    End If
                    If Not NamespaceNames.Contains(NamePart) AndAlso TypeFiler.Invoke(t) Then NamespaceNames.Add(NamePart)
                End If
            Next t
            Return (From np In NamespaceNames _
                Order By np _
                Select New NamespaceInfo(Me.Module, If(Me.Name = "", np, Me.Name & "." & np)) _
                ).ToArray
        End Function

        ''' <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        ''' <returns>true if the current object is equal to the 
        ''' <paramref name="other" /> parameter; otherwise, false.</returns>
        ''' <param name="other">An object to compare with this object.</param>
        ''' <version version="1.5.2">Function added</version>
        Public Overloads Function Equals(ByVal other As NamespaceInfo) As Boolean Implements System.IEquatable(Of NamespaceInfo).Equals
            Return Me.Equals(CObj(other))
        End Function
    End Class
    ''' <summary>Operators supported by CLI</summary>
    ''' <remarks>High order byte (exluding its MSB) is number that uniquely identifies the operator.
    ''' Low-order half-byte represents number of operands of the operator (1 or 2).
    ''' If MSB of low-order byte is set then operator is non-standard.
    ''' If LSB of high-order half-byle of low-order byte (9th LSB bit in whole number) is set then operator is assignment.
    ''' See <seealso cref="Operators_masks"/>.
    ''' Names of items of the enumeration are names of operator methods without 'op_' prefix.</remarks>
    Public Enum Operators As Short
        ''' <summary>No operator</summary>
        no = False
        ''' <summary>Decrement (unary, like C++/C# --)</summary>
        Decrement = &H101
        ''' <summary>Increment (unary, like C++/C# ++)</summary>
        Increment = &H201
        ''' <summary>Unary negation (unary minus operator like C++/C#/VB -)</summary>
        UnaryNegation = &H301
        ''' <summary>Unary plus (like C++/C#/VB +)</summary>
        UnaryPlus = &H401
        ''' <summary>Logical not (unary, like C++/C# !, VB Not)</summary>
        LogicalNot = &H501
        ''' <summary>True operator - if value should be treated as True (unary, like VB IsTrue)</summary>
        [True] = &H601
        ''' <summary>False operator - if value should be treated as False (unary, like VB IsFalse)</summary>
        [False] = &H701
        ''' <summary>Reference operator (unary, like C++ &amp;)</summary>
        [AddressOf] = &H801
        ''' <summary>Bitwise not operator (unary, like C++/C# ~, VB Not)</summary>
        OnesComplement = &H901
        ''' <summary>Pointer dereference (unary, like C++ *)</summary>
        PointerDereference = &HA01

        ''' <summary>Addition (binary, like C++/C#/VB +)</summary>
        Addition = &HB02
        ''' <summary>Subtraction (binary, like C++/C#/VB -)</summary>
        Subtraction = &HC02
        ''' <summary>Multiplication (binary, like C++/C#/VB *)</summary>
        Multiply = &HD02
        ''' <summary>Division (binary, like C++/C#/VB /)</summary>
        Division = &HE02
        ''' <summary>Modulus (division remainder, binary, like C++/C# %, VB Mod)</summary>
        Modulus = &HF02
        ''' <summary>Bitwise xor (exclusive or, binary, like C++/C# ^, VB Xor)</summary>
        ExclusiveOr = &H1002
        ''' <summary>Bitwise and (binary, like C++/C# &amp;, VB And)</summary>
        BitwiseAnd = &H1102
        ''' <summary>Bitwise or (binary, like C++/C# |, VB Or)</summary>
        BitwiseOr = &H1202
        ''' <summary>Logical and (binary, like C++/C# &amp;&amp;, VB AndAlso)</summary>
        LogicalAnd = &H1302
        ''' <summary>Logical or (binary, like C++/C# ||, VB OrElse)</summary>
        LogicalOr = &H1402
        ''' <summary>Assignment(binary, like C++/C#/VB =)</summary>
        Assign = &H1512
        ''' <summary>Left shift (binary, like C++/C#/VB &lt;&lt;)</summary>
        LeftShift = &H1602
        ''' <summary>Right shift (binary, like C++/C#/VB >>)</summary>
        RightShift = &H1702
        ''' <summary>Signed right shift (binary)</summary>
        SignedRightShif = &H1802
        ''' <summary>Unsigned right shift (binary)</summary>
        UnsignedRightShift = &H1902
        ''' <summary>Equality comparison (binary, like C++/C# ==, VB =)</summary>
        Equality = &H1A02
        ''' <summary>Greater than comparison (binary, like C++/C#/VB >)</summary>
        GreaterThan = &H1B02
        ''' <summary>Less than comparison (binary, like C++/C#/VB &lt;)</summary>
        LessThan = &H1C02
        ''' <summary>Inequality comparison (binary, like C++/C# !=; VB &lt;>)</summary>
        Inequality = &H1E02
        ''' <summary>Greater than or equal comparison (binary, like C++/C#/VB >=)</summary>
        GreaterThanOrEqual = &H1F02
        ''' <summary>Less than or equal comparison (binary, like C++/C#/VB &lt;=)</summary>
        LessThanOrEqual = &H200
        ''' <summary>Self-assignment of unsigned right shift (binary)</summary>
        UnsignedRightShiftAssignment = &H2012
        ''' <summary>Member selection (binary, like C++ ->)</summary>
        MemberSelection = &H2102
        ''' <summary>Self-assignment of right shift (binary, like C++/C#/VB >>=)</summary>
        RightShifAssignment = &H2212
        ''' <summary>Self-assigment of multiplication (binary, like C++/C#/VB *=)</summary>
        MultiplicationAssignment = &H2312
        ''' <summary>Selection of pointer to member (binary, like C++ ->*)</summary>
        PointerToMemberSelection = &H2402
        ''' <summary>Self-assignment of subtraction (binary, like C++/C#/VB -=)</summary>
        SubtractionAssignment = &H2512
        ''' <summary>Bitwise exclusive or self-assigment (binary, like C++/C# ^=)</summary>
        ExclusiveOrAssignment = &H2612
        ''' <summary>Self-assigment of left shift (binary, like C++/C#/VB &lt;&lt;=)</summary>
        LeftShiftAssignment = &H2712
        ''' <summary>Modulus (division remainder) self-assignment (binary, like C++/C# %=)</summary>
        ModulusAssignment = &H2812
        ''' <summary>Self-assigmment of addition (binary, like C++/C#/VB +=)</summary>
        AditionAssignment = &H2912
        ''' <summary>Self-assignment of witwise and (binary, like C++/C# &amp;=)</summary>
        BitwiseAndAssignment = &H2A12
        ''' <summary>Self-assignment of bitwise or (binary, like C++/C# |=)</summary>
        BitwiseOrAssignment = &H2B12
        ''' <summary>Comma (operation grouping, binary, like C++ ,)</summary>
        Comma = &H2C02
        ''' <summary>Self-assignment of division (binary, like C++/C#/VB /=)</summary>
        DivisionAssignment = &H2D12

        ''' <summary>String contactenation (VB specific, binary, like VB &amp;)</summary>
        Concatenate = &H2E82
        ''' <summary>Exponent (VB specific, binary, like VB ^)</summary>
        Exponent = &H2F82
        ''' <summary>Force-integral division (VB specific, binary, like VB \, C++/C# / on integers)</summary>
        IntegerDivision = &H3082

        ''' <summary>Implicit conversion (unary, like C# implicit, VB Narrowing CType)</summary>
        Implicit = &H3101
        ''' <summary>Explicit conversion (unary, like C# explicit, VB Widening CType)</summary>
        Explicit = &H3201
    End Enum
    ''' <summary>Masks for the <see cref="Operators"/> enumeration</summary>
    <Flags()> _
    Public Enum Operators_masks As Short
        ''' <summary>Masks operator number. This number is unique within <see cref="Operators"/>, but has no relation to anything in CLI.</summary>
        OperatorID = &H7F00
        ''' <summary>Masks number of operands</summary>
        NoOfOperands = &HF
        ''' <summary>Masks if operator is standard (0) or non-standard (1)</summary>
        NonStandard = &H80
        ''' <summary>Masks if operator is assignment (1) or not (0)</summary>
        Assignment = &H10
    End Enum
End Namespace
#End If