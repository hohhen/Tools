﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:2.0.50727.3053
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace MetadataT.IptcT
    
    'This class was auto-generated by the StronglyTypedResourceBuilderEx class via the InternalResXFileCodeGeneratorEx custom tool.
    'To add or remove a member, edit your .ResX file then rerun the InternalResXFileCodeGeneratorEx custom tool or rebuild your VS.NET project.
    'Copyright (c) Dmytro Kryvko 2006-2008 (http://dmytro.kryvko.googlepages.com/)
    '''<summary>
    '''A strongly-typed resource class, for looking up localized strings, formatting them, etc.
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("DMKSoftware.CodeGenerators.Tools.StronglyTypedResourceBuilderEx", "2.2.5.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")>  _
    Friend Class IPTCResources
        
        Private Shared _resourceManager As Global.System.Resources.ResourceManager
        
        Private Shared _internalSyncObject As Object
        
        Private Shared _resourceCulture As Global.System.Globalization.CultureInfo
        
        <Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>  _
        Friend Sub New()
            MyBase.New
        End Sub
        
        '''<summary>
        '''Thread safe lock object used by this class.
        '''</summary>
        Friend Shared ReadOnly Property InternalSyncObject() As Object
            Get
                If Object.ReferenceEquals(_internalSyncObject, Nothing) Then
                    Global.System.Threading.Interlocked.CompareExchange(_internalSyncObject, New Object, Nothing)
                End If
                Return _internalSyncObject
            End Get
        End Property
        
        '''<summary>
        '''Returns the cached ResourceManager instance used by this class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Shared ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(_resourceManager, Nothing) Then
                    Global.System.Threading.Monitor.Enter(InternalSyncObject)
                    Try 
                        If Object.ReferenceEquals(_resourceManager, Nothing) Then
                            Global.System.Threading.Interlocked.Exchange(_resourceManager, New Global.System.Resources.ResourceManager("Tools.MetadataT.IptcT.IptcResources", GetType(IPTCResources).Assembly))
                        End If
                    Finally
                        Global.System.Threading.Monitor.Exit(InternalSyncObject)
                    End Try
                End If
                Return _resourceManager
            End Get
        End Property
        
        '''<summary>
        '''Overrides the current thread's CurrentUICulture property for all
        '''resource lookups using this strongly typed resource class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Shared Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return _resourceCulture
            End Get
            Set
                _resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'Number of components'.
        '''</summary>
        Friend Shared ReadOnly Property Components_d() As String
            Get
                Return ResourceManager.GetString("Components_d", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'Information Provider Reference A name, registered with the IPTC/NAA, identifying the provider that guarantees the uniqueness of the UNO'.
        '''</summary>
        Friend Shared ReadOnly Property IPR_d() As String
            Get
                Return ResourceManager.GetString("IPR_d", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'Information Provider Reference'.
        '''</summary>
        Friend Shared ReadOnly Property IPR_d2() As String
            Get
                Return ResourceManager.GetString("IPR_d2", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'IPR'.
        '''</summary>
        Friend Shared ReadOnly Property IPR_n() As String
            Get
                Return ResourceManager.GetString("IPR_n", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'Object Descriptor Element In conjunction with the UCD and the IPR, a string of characters ensuring the uniqueness of the UNO.'.
        '''</summary>
        Friend Shared ReadOnly Property ODE_d() As String
            Get
                Return ResourceManager.GetString("ODE_d", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'ODE'.
        '''</summary>
        Friend Shared ReadOnly Property ODE_n() As String
            Get
                Return ResourceManager.GetString("ODE_n", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'Object Variant Indicator A string of characters indicating technical variants of the object such as partial objects, or changes of file formats, and coded character sets.'.
        '''</summary>
        Friend Shared ReadOnly Property OVI_d() As String
            Get
                Return ResourceManager.GetString("OVI_d", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'String value of the property {0} can be maximally {1} bytes long.'.
        '''</summary>
        Friend Shared ReadOnly Property StringValueOfTheProperty0CanBeMaximally1BytesLong() As String
            Get
                Return ResourceManager.GetString("StringValueOfTheProperty0CanBeMaximally1BytesLong", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'String value of the property {0} must be {1} bytes long.'.
        '''</summary>
        Friend Shared ReadOnly Property StringValueOfTheProperty0MustBe1BytesLong() As String
            Get
                Return ResourceManager.GetString("StringValueOfTheProperty0MustBe1BytesLong", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'A text representation of the Subject Detail Number'.
        '''</summary>
        Friend Shared ReadOnly Property SubjectDetailName_d() As String
            Get
                Return ResourceManager.GetString("SubjectDetailName_d", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'Subject Detail Name'.
        '''</summary>
        Friend Shared ReadOnly Property SubjectDetailName_n() As String
            Get
                Return ResourceManager.GetString("SubjectDetailName_n", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'Detail component of Subject Reference Number'.
        '''</summary>
        Friend Shared ReadOnly Property SubjectDetailNumber_d() As String
            Get
                Return ResourceManager.GetString("SubjectDetailNumber_d", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'Subject Detail Number'.
        '''</summary>
        Friend Shared ReadOnly Property SubjectDetailNumber_n() As String
            Get
                Return ResourceManager.GetString("SubjectDetailNumber_n", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'A text representation of the Subject Matter Number'.
        '''</summary>
        Friend Shared ReadOnly Property SubjectMatterName_d() As String
            Get
                Return ResourceManager.GetString("SubjectMatterName_d", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'Subject Matter Name'.
        '''</summary>
        Friend Shared ReadOnly Property SubjectMatterName_n() As String
            Get
                Return ResourceManager.GetString("SubjectMatterName_n", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'Matter component of Subject Reference Number'.
        '''</summary>
        Friend Shared ReadOnly Property SubjectMatterNumber_d() As String
            Get
                Return ResourceManager.GetString("SubjectMatterNumber_d", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'Subject Matter Number'.
        '''</summary>
        Friend Shared ReadOnly Property SubjectMatterNumber_n() As String
            Get
                Return ResourceManager.GetString("SubjectMatterNumber_n", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'A text representation of the Subject Number (maximum 64 octets) consisting of graphic characters plus spaces either in English, as defined in Appendix H, or in the language of the service as indicated in DataSet Language Identifier (2:135)'.
        '''</summary>
        Friend Shared ReadOnly Property SubjectName_d() As String
            Get
                Return ResourceManager.GetString("SubjectName_d", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'Subject Name'.
        '''</summary>
        Friend Shared ReadOnly Property SubjectName_n() As String
            Get
                Return ResourceManager.GetString("SubjectName_n", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'Subject component of Subject Reference Number'.
        '''</summary>
        Friend Shared ReadOnly Property SubjectNumber_d() As String
            Get
                Return ResourceManager.GetString("SubjectNumber_d", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'Subject Number'.
        '''</summary>
        Friend Shared ReadOnly Property SubjectNumber_n() As String
            Get
                Return ResourceManager.GetString("SubjectNumber_n", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'Type of components'.
        '''</summary>
        Friend Shared ReadOnly Property Type_d() As String
            Get
                Return ResourceManager.GetString("Type_d", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''Looks up a localized string similar to 'UNO Creation Date Specifies a 24 hour period in which the further elements of the UNO have to be unique.'.
        '''</summary>
        Friend Shared ReadOnly Property UCD_d() As String
            Get
                Return ResourceManager.GetString("UCD_d", _resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''The stub formatting method returning the Components_d property value.
        '''</summary>
        '''<returns>The Components_d property value.</returns>
        Friend Shared Function Components_dFormat() As String
            Return Components_d
        End Function
        
        '''<summary>
        '''The stub formatting method returning the IPR_d property value.
        '''</summary>
        '''<returns>The IPR_d property value.</returns>
        Friend Shared Function IPR_dFormat() As String
            Return IPR_d
        End Function
        
        '''<summary>
        '''The stub formatting method returning the IPR_d2 property value.
        '''</summary>
        '''<returns>The IPR_d2 property value.</returns>
        Friend Shared Function IPR_d2Format() As String
            Return IPR_d2
        End Function
        
        '''<summary>
        '''The stub formatting method returning the IPR_n property value.
        '''</summary>
        '''<returns>The IPR_n property value.</returns>
        Friend Shared Function IPR_nFormat() As String
            Return IPR_n
        End Function
        
        '''<summary>
        '''The stub formatting method returning the ODE_d property value.
        '''</summary>
        '''<returns>The ODE_d property value.</returns>
        Friend Shared Function ODE_dFormat() As String
            Return ODE_d
        End Function
        
        '''<summary>
        '''The stub formatting method returning the ODE_n property value.
        '''</summary>
        '''<returns>The ODE_n property value.</returns>
        Friend Shared Function ODE_nFormat() As String
            Return ODE_n
        End Function
        
        '''<summary>
        '''The stub formatting method returning the OVI_d property value.
        '''</summary>
        '''<returns>The OVI_d property value.</returns>
        Friend Shared Function OVI_dFormat() As String
            Return OVI_d
        End Function
        
        '''<summary>
        '''Formats a localized string similar to 'String value of the property {0} can be maximally {1} bytes long.'.
        '''</summary>
        '''<param name="arg0">An object (0) to format.</param>
        '''<param name="arg1">An object (1) to format.</param>
        '''<returns>A copy of format string in which the format items have been replaced by the String equivalent of the corresponding instances of Object in arguments.</returns>
        Friend Shared Function StringValueOfTheProperty0CanBeMaximally1BytesLongFormat(ByVal arg0 As Object, ByVal arg1 As Object) As String
            Return String.Format(_resourceCulture, StringValueOfTheProperty0CanBeMaximally1BytesLong, arg0, arg1)
        End Function
        
        '''<summary>
        '''Formats a localized string similar to 'String value of the property {0} must be {1} bytes long.'.
        '''</summary>
        '''<param name="arg0">An object (0) to format.</param>
        '''<param name="arg1">An object (1) to format.</param>
        '''<returns>A copy of format string in which the format items have been replaced by the String equivalent of the corresponding instances of Object in arguments.</returns>
        Friend Shared Function StringValueOfTheProperty0MustBe1BytesLongFormat(ByVal arg0 As Object, ByVal arg1 As Object) As String
            Return String.Format(_resourceCulture, StringValueOfTheProperty0MustBe1BytesLong, arg0, arg1)
        End Function
        
        '''<summary>
        '''The stub formatting method returning the SubjectDetailName_d property value.
        '''</summary>
        '''<returns>The SubjectDetailName_d property value.</returns>
        Friend Shared Function SubjectDetailName_dFormat() As String
            Return SubjectDetailName_d
        End Function
        
        '''<summary>
        '''The stub formatting method returning the SubjectDetailName_n property value.
        '''</summary>
        '''<returns>The SubjectDetailName_n property value.</returns>
        Friend Shared Function SubjectDetailName_nFormat() As String
            Return SubjectDetailName_n
        End Function
        
        '''<summary>
        '''The stub formatting method returning the SubjectDetailNumber_d property value.
        '''</summary>
        '''<returns>The SubjectDetailNumber_d property value.</returns>
        Friend Shared Function SubjectDetailNumber_dFormat() As String
            Return SubjectDetailNumber_d
        End Function
        
        '''<summary>
        '''The stub formatting method returning the SubjectDetailNumber_n property value.
        '''</summary>
        '''<returns>The SubjectDetailNumber_n property value.</returns>
        Friend Shared Function SubjectDetailNumber_nFormat() As String
            Return SubjectDetailNumber_n
        End Function
        
        '''<summary>
        '''The stub formatting method returning the SubjectMatterName_d property value.
        '''</summary>
        '''<returns>The SubjectMatterName_d property value.</returns>
        Friend Shared Function SubjectMatterName_dFormat() As String
            Return SubjectMatterName_d
        End Function
        
        '''<summary>
        '''The stub formatting method returning the SubjectMatterName_n property value.
        '''</summary>
        '''<returns>The SubjectMatterName_n property value.</returns>
        Friend Shared Function SubjectMatterName_nFormat() As String
            Return SubjectMatterName_n
        End Function
        
        '''<summary>
        '''The stub formatting method returning the SubjectMatterNumber_d property value.
        '''</summary>
        '''<returns>The SubjectMatterNumber_d property value.</returns>
        Friend Shared Function SubjectMatterNumber_dFormat() As String
            Return SubjectMatterNumber_d
        End Function
        
        '''<summary>
        '''The stub formatting method returning the SubjectMatterNumber_n property value.
        '''</summary>
        '''<returns>The SubjectMatterNumber_n property value.</returns>
        Friend Shared Function SubjectMatterNumber_nFormat() As String
            Return SubjectMatterNumber_n
        End Function
        
        '''<summary>
        '''The stub formatting method returning the SubjectName_d property value.
        '''</summary>
        '''<returns>The SubjectName_d property value.</returns>
        Friend Shared Function SubjectName_dFormat() As String
            Return SubjectName_d
        End Function
        
        '''<summary>
        '''The stub formatting method returning the SubjectName_n property value.
        '''</summary>
        '''<returns>The SubjectName_n property value.</returns>
        Friend Shared Function SubjectName_nFormat() As String
            Return SubjectName_n
        End Function
        
        '''<summary>
        '''The stub formatting method returning the SubjectNumber_d property value.
        '''</summary>
        '''<returns>The SubjectNumber_d property value.</returns>
        Friend Shared Function SubjectNumber_dFormat() As String
            Return SubjectNumber_d
        End Function
        
        '''<summary>
        '''The stub formatting method returning the SubjectNumber_n property value.
        '''</summary>
        '''<returns>The SubjectNumber_n property value.</returns>
        Friend Shared Function SubjectNumber_nFormat() As String
            Return SubjectNumber_n
        End Function
        
        '''<summary>
        '''The stub formatting method returning the Type_d property value.
        '''</summary>
        '''<returns>The Type_d property value.</returns>
        Friend Shared Function Type_dFormat() As String
            Return Type_d
        End Function
        
        '''<summary>
        '''The stub formatting method returning the UCD_d property value.
        '''</summary>
        '''<returns>The UCD_d property value.</returns>
        Friend Shared Function UCD_dFormat() As String
            Return UCD_d
        End Function
    End Class
End Namespace
