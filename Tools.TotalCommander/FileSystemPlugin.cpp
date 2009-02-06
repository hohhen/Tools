#include "stdafx.h"
#include "FileSystemPlugin.h"
#include "fsplugin.h"
#include "Exceptions.h"
#include <vcclr.h>

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace Tools::TotalCommanderT::ResourcesT;
using namespace Tools::ExtensionsT;

namespace Tools{namespace TotalCommanderT{
    //Global functions
    inline DateTime FileTimeToDateTime(::FILETIME value){
        return DateTime::FromFileTime((__int64) value.dwHighDateTime << 32 | (__int64) value.dwLowDateTime);
    }
    ::FILETIME DateTimeToFileTime(DateTime value){
        ::FILETIME ret;
        ret.dwLowDateTime = Numbers::Low(value.ToFileTime());
        ret.dwHighDateTime = Numbers::High(value.ToFileTime());
        return ret;
    }
    //RemoteInfo
    RemoteInfo::RemoteInfo(const RemoteInfoStruct &ri){
        this->SizeLow = ri.SizeLow;
        this->SizeHigh = ri.SizeHigh;
        this->Attr = (FileAttributes)ri.Attr;
        this->LastWriteTime = FileTimeToDateTime(ri.LastWriteTime);
    }
    inline QWORD RemoteInfo::Size::get(){ return (QWORD)this->SizeHigh << 32 | (QWORD)this->SizeLow;}
    void RemoteInfo::Size::set(QWORD value){
        this->SizeHigh = Numbers::High(value);
        this->SizeHigh = Numbers::Low(value);
    }

    //FileSystemPlugin
#pragma region "FileSystemPlugin"
    FileSystemPlugin::FileSystemPlugin(){
         this->handleDictionary = gcnew Collections::Generic::Dictionary<int,Object^>();
         this->MaxHandle = 0;
    }
#pragma region "TC functions"
    int FileSystemPlugin::FsInit(int PluginNr,tProgressProc pProgressProc, tLogProc pLogProc,tRequestProc pRequestProc){
        this->pluginNr = PluginNr;
        this->pProgressProc = pProgressProc;
        this->pLogProc  = pLogProc;
        this->pRequestProc = pRequestProc;
        this->initialized = true;
        this->OnInit();
        return 0;
    }
    HANDLE FileSystemPlugin::FsFindFirst(char* Path,WIN32_FIND_DATA *FindData){
        Tools::TotalCommanderT::FindData% findData = *gcnew Tools::TotalCommanderT::FindData(*FindData);
        Object^ object;
        Exception^ exception = nullptr;
        try{
            object = this->FindFirst(gcnew String(Path),findData);
        }catch(IO::DirectoryNotFoundException^ ex){exception=ex;
        }catch(UnauthorizedAccessException^ ex){exception=ex;
        }catch(Security::SecurityException^ ex){exception=ex;
        }catch(IO::IOException^ ex){exception=ex;}
        if(exception != nullptr){
            SetLastError(Marshal::GetHRForException(exception));
            return INVALID_HANDLE_VALUE;
        }else if(object == nullptr){
            SetLastError(ERROR_NO_MORE_FILES);
            return INVALID_HANDLE_VALUE;
        }else{
            int handle = this->GetNextHandle();
            this->HandleDictionary->Add(handle, object);
            findData.Populate(*FindData);
            return (HANDLE)handle;
        }
    }
    BOOL FileSystemPlugin::FsFindNext(HANDLE Hdl,WIN32_FIND_DATA *FindData){
        Tools::TotalCommanderT::FindData% findData = *gcnew Tools::TotalCommanderT::FindData(*FindData);
        Object^ object = this->HandleDictionary->ContainsKey((int)Hdl) ? this->HandleDictionary[(int)Hdl] : nullptr;
        Exception^ exception = nullptr;
        bool ret;
        try{
            ret = this->FindNext(object,findData);
        }catch(IO::DirectoryNotFoundException^ ex){exception=ex;
        }catch(UnauthorizedAccessException^ ex){exception=ex;
        }catch(Security::SecurityException^ ex){exception=ex;
        }catch(IO::IOException^ ex){exception=ex;}
        if(exception != nullptr || !ret) return false;
        else{
            findData.Populate(*FindData);
            return true;
        }
    }
    int FileSystemPlugin::FsFindClose(HANDLE Hdl){
        Object^ object = this->HandleDictionary->ContainsKey((int)Hdl) ? this->HandleDictionary[(int)Hdl] : nullptr;
        this->FindClose(object);
        if(this->HandleDictionary->ContainsKey((int)Hdl)) this->HandleDictionary->Remove((int) Hdl);
        return 0;
    }
#pragma endregion
#pragma region ".NET Functions"
    int FileSystemPlugin::PluginNr::get(){
        if(!this->Initialized) throw gcnew InvalidOperationException(Exceptions::PluginNotInitialized);
        return this->pluginNr;
    }
    void FileSystemPlugin::OnInit(){/*do nothing*/}
    inline Collections::Generic::Dictionary<int,Object^>^ FileSystemPlugin::HandleDictionary::get(){ return this->handleDictionary; }
    inline int FileSystemPlugin::GetNextHandle(){ return ++this->MaxHandle; }
    inline bool FileSystemPlugin::Initialized::get(){return this->initialized;}
#pragma region "Callbacks"
    bool FileSystemPlugin::ProgressProc(String^ SourceName, String^ TargetName,int PercentDone){
        if(!this->Initialized) throw gcnew InvalidOperationException(Exceptions::PluginNotInitialized);
        char* sourceName = (char*)(void*)Marshal::StringToHGlobalAnsi(SourceName);
        char* targetName = (char*)(void*)Marshal::StringToHGlobalAnsi(TargetName);
        bool ret = this->pProgressProc(this->PluginNr,sourceName,targetName,PercentDone) == 1 ? true : false;
        Marshal::FreeHGlobal((IntPtr)sourceName);
        Marshal::FreeHGlobal((IntPtr)targetName);
        return ret;
    }
    void FileSystemPlugin::LogProc(LogKind MsgType,String^ LogString){
        if(!this->Initialized) throw gcnew InvalidOperationException(Exceptions::PluginNotInitialized);
        char* logString = (char*)(void*)Marshal::StringToHGlobalAnsi(LogString);
        this->pLogProc(this->PluginNr,(int)MsgType,logString);
        Marshal::FreeHGlobal((IntPtr)logString);
    }
    String^ FileSystemPlugin::RequestProc(InputRequestKind RequestType,String^ CustomTitle, String^ CustomText, String^ DefaultText, int maxlen){
        if(!this->Initialized) throw gcnew InvalidOperationException(Exceptions::PluginNotInitialized);
        if(DefaultText->Length > maxlen) throw gcnew ArgumentException(Exceptions::DefaultTextTooLong);
        if(maxlen < 1) throw gcnew ArgumentOutOfRangeException("maxlen");
        char* customTitle = (char*)(void*)Marshal::StringToHGlobalAnsi(CustomTitle);
        char* customText = (char*)(void*)Marshal::StringToHGlobalAnsi(CustomText);
        char* defaultText = new char[maxlen];
        if(DefaultText != nullptr)
            for(int i = 0; i < DefaultText->Length; i++)
                defaultText[i] = (char)DefaultText[i];
        Marshal::FreeHGlobal((IntPtr)customTitle);
        Marshal::FreeHGlobal((IntPtr)customText);
        if(this->pRequestProc(this->PluginNr, (int)RequestType, customTitle, customText, defaultText, maxlen)){
            String^ ret = gcnew String(defaultText);
            delete defaultText;
            return ret;
        }
        delete defaultText;
        return nullptr;
    }
#pragma endregion
    inline void FileSystemPlugin::FindClose(Object^ Status){/*do nothing*/}
#pragma endregion
#pragma region "Optional functions"

    //MkDir
    BOOL FileSystemPlugin::FsMkDir(char* Path){
        Exception^ ex=nullptr;
        try{
            return this->MkDir(gcnew String(Path));
        }catch(IO::IOException^ ex__){ex=ex__;}catch(Security::SecurityException^ ex__){ex=ex__;}catch(UnauthorizedAccessException^ ex__){ex=ex__;} if(ex!=nullptr){
            return false;
        }
        return true;
    }
    inline bool FileSystemPlugin::MkDir(String^ Path){ throw gcnew NotSupportedException(); }
    //ExecuteFile
    int FileSystemPlugin::FsExecuteFile(HWND MainWin,char* RemoteName,char* Verb){
        Exception^ ex=nullptr;
        String^% remoteName = gcnew String(RemoteName);
        try{
            ExecExitCode ret =  this->ExecuteFile((IntPtr)MainWin, remoteName, gcnew String(Verb));
            String^ old = gcnew String(RemoteName);
            IntPtr ptr;
            if(old != remoteName) strcpy_s(RemoteName, remoteName->Length, (char*)(void*) (ptr = Marshal::StringToHGlobalAnsi(remoteName)));
            Marshal::FreeHGlobal(ptr);
            return (int) ret;
        }catch(InvalidOperationException^ ex__){ ex=ex__;
        }catch(IO::IOException^ ex__){ex=ex__;}catch(Security::SecurityException^ ex__){ex=ex__;}catch(UnauthorizedAccessException^ ex__){ex=ex__;} if(ex!=nullptr){}
        return (int) ExecExitCode::Error;
    }
    inline ExecExitCode FileSystemPlugin::ExecuteFile(IntPtr hMainWin, String^% RemoteName, String^ Verb){ throw gcnew NotSupportedException(); }
    //RenMovFile
    int FileSystemPlugin::FsRenMovFile(char* OldName,char* NewName,BOOL Move, BOOL OverWrite,RemoteInfoStruct* ri){
        try{
            return (int)this->RenMovFile(gcnew String(OldName), gcnew String(NewName), Move==0?false:true, OverWrite==0?false:true, RemoteInfo(*ri));
        }catch(UnauthorizedAccessException^){ return (int)FileSystemExitCode::ReadError; }
        catch(Security::SecurityException^){ return (int)FileSystemExitCode::ReadError; }
        catch(IO::FileNotFoundException^){ return (int)FileSystemExitCode::FileNotFound; }
        catch(IO::DirectoryNotFoundException^){ return (int)FileSystemExitCode::WriteError; }
        catch(IO::IOException^){ return (int)FileSystemExitCode::ReadError; }
        catch(InvalidOperationException^){ return (int)FileSystemExitCode::NotSupported; }
   }
    inline FileSystemExitCode FileSystemPlugin::RenMovFile(String^ OldName, String^ NewName, bool move, bool OverWrite, RemoteInfo info){ throw gcnew NotSupportedException(); }
    //GetFile
    int FileSystemPlugin::FsGetFile(char* RemoteName,char* LocalName,int CopyFlags, RemoteInfoStruct* ri){
        String^% localName = gcnew String(LocalName);
        try{
            FileSystemExitCode ret = this->GetFile(gcnew String(RemoteName), localName, (Tools::TotalCommanderT::CopyFlags)CopyFlags, RemoteInfo(*ri));
            String^ old = gcnew String(LocalName);
            IntPtr ptr;
            if(old != localName) strcpy_s(LocalName, localName->Length, (char*)(void*) (ptr = Marshal::StringToHGlobalAnsi(localName)));
            Marshal::FreeHGlobal(ptr);
            return (int) ret;
        }catch(UnauthorizedAccessException^){ return (int)FileSystemExitCode::ReadError; }
        catch(Security::SecurityException^){ return (int)FileSystemExitCode::ReadError; }
        catch(IO::FileNotFoundException^){ return (int)FileSystemExitCode::FileNotFound; }
        catch(IO::DirectoryNotFoundException^){ return (int)FileSystemExitCode::WriteError; }
        catch(IO::IOException^){ return (int)FileSystemExitCode::ReadError; }
        catch(InvalidOperationException^){ return (int)FileSystemExitCode::NotSupported; }
    }
    inline FileSystemExitCode FileSystemPlugin::GetFile(String^ RemoteName, String^% LocalName, CopyFlags CopyFlags, RemoteInfo info){ throw gcnew NotSupportedException(); }
    //PutFile
    int FileSystemPlugin::FsPutFile(char* LocalName,char* RemoteName,int CopyFlags){
        String^% remoteName = gcnew String(RemoteName);
        try{
            FileSystemExitCode ret = this->PutFile(gcnew String(LocalName), remoteName, (Tools::TotalCommanderT::CopyFlags)CopyFlags);
            String^ old = gcnew String(RemoteName);
            IntPtr ptr;
            if(old != remoteName) strcpy_s(RemoteName, remoteName->Length, (char*)(void*) (ptr = Marshal::StringToHGlobalAnsi(remoteName)));
            Marshal::FreeHGlobal(ptr);
            return (int) ret;
        }catch(UnauthorizedAccessException^){ return (int)FileSystemExitCode::ReadError; }
        catch(Security::SecurityException^){ return (int)FileSystemExitCode::ReadError; }
        catch(IO::FileNotFoundException^){ return (int)FileSystemExitCode::FileNotFound; }
        catch(IO::DirectoryNotFoundException^){ return (int)FileSystemExitCode::WriteError; }
        catch(IO::IOException^){ return (int)FileSystemExitCode::ReadError; }
        catch(InvalidOperationException^){ return (int)FileSystemExitCode::NotSupported; }
    }
    inline FileSystemExitCode FileSystemPlugin::PutFile(String^ LocalName, String^% RemoteName, CopyFlags CopyFlags){ throw gcnew NotSupportedException(); }
    //Delete file
    BOOL FileSystemPlugin::FsDeleteFile(char* RemoteName){
        try{ return this->DeleteFile(gcnew String(RemoteName)); }
        catch(UnauthorizedAccessException^){ return false; }
        catch(Security::SecurityException^){ return false; }
        catch(IO::IOException^){ return false; }
    }
    inline bool FileSystemPlugin::DeleteFile(String^ RemoteName){ throw gcnew NotSupportedException(); }
    //RemoveDir
    BOOL FileSystemPlugin::FsRemoveDir(char* RemoteName){
        try{ return this->RemoveDir(gcnew String(RemoteName)); }
        catch(UnauthorizedAccessException^){ return false; }
        catch(Security::SecurityException^){ return false; }
        catch(IO::IOException^){ return false; }
    }
    inline bool FileSystemPlugin::RemoveDir(String^ RemoteName){ throw gcnew NotSupportedException(); }
    //Disconect
    inline BOOL FileSystemPlugin::FsDisconnect(char* DisconnectRoot){
        return this->Disconnect(gcnew String(DisconnectRoot));
    }
    inline bool FileSystemPlugin::Disconnect(String^ DisconnectRoot){ throw gcnew NotSupportedException(); }
    //SetAttr
    BOOL FileSystemPlugin::FsSetAttr(char* RemoteName,int NewAttr){
        try{
            this->SetAttr(gcnew String(RemoteName), (StandardFileAttributes) NewAttr);}
        catch(UnauthorizedAccessException^){ return false; }
        catch(Security::SecurityException^){ return false; }
        catch(IO::IOException^){ return false; }
        return true;
    }
    inline void FileSystemPlugin::SetAttr(String^ RemoteName, StandardFileAttributes NewAttr){ throw gcnew NotSupportedException(); }
    //SetTime
    BOOL FileSystemPlugin::FsSetTime(char* RemoteName,::FILETIME *CreationTime, ::FILETIME *LastAccessTime,::FILETIME *LastWriteTime){
        Nullable<DateTime> create = Nullable<DateTime>();
        Nullable<DateTime> access = Nullable<DateTime>();
        Nullable<DateTime> write = Nullable<DateTime>();
        if(CreationTime!=nullptr) create = Nullable<DateTime>(FileTimeToDateTime(*CreationTime));
        if(LastAccessTime!=nullptr) access = Nullable<DateTime>(FileTimeToDateTime(*LastAccessTime));
        if(LastWriteTime!=nullptr) write = Nullable<DateTime>(FileTimeToDateTime(*LastWriteTime));
        try{ this->SetTime(gcnew String(RemoteName), create, access, write);}
        catch(UnauthorizedAccessException^){ return false; }
        catch(Security::SecurityException^){ return false; }
        catch(IO::IOException^){ return false; }
        return true;
    }
    inline void FileSystemPlugin::SetTime(String^ RemoteName, Nullable<DateTime> CreationTime, Nullable<DateTime> LastAccessTime, Nullable<DateTime> LastWriteTime){ throw gcnew NotSupportedException(); }
    //StatusInfo
    void FileSystemPlugin::FsStatusInfo(char* RemoteDir,int InfoStartEnd,int InfoOperation){
        OperationEventArgs^ e = gcnew OperationEventArgs(gcnew String(RemoteDir),(OperationKind)InfoOperation, (OperationStatus)InfoStartEnd);
        if(e->Status == OperationStatus::Start) this->OnOperationStarting(e);
        else if(e->Status == OperationStatus::End) this->OnOperationFinished(e);
        this->OnOperationStatusChanged(e);
    }
    void FileSystemPlugin::OnOperationStatusChanged(OperationEventArgs^ e){/*Do nothing*/}
    void FileSystemPlugin::OnOperationStarting(OperationEventArgs^ e){/*Do nothing*/}
    void FileSystemPlugin::OnOperationFinished(OperationEventArgs^ e){/*Do nothing*/}
    //GetDefRootName
    void FileSystemPlugin::FsGetDefRootName(char* DefRootName,int maxlen){
        String^ name = this->Name;
        char* namech = (char*)(void*)Marshal::StringToHGlobalAnsi(name);
        strcpy_s(DefRootName,Math::Min(name->Length,maxlen-1),namech);
        DefRootName[Math::Min(name->Length,maxlen-1)] = 0;
        Marshal::FreeHGlobal((IntPtr)(void*)namech);
    }
#pragma endregion
#pragma endregion

    //FindData
#pragma region "FindData"
    FindData::FindData(WIN32_FIND_DATA& Original){
        this->cAlternateFileName = gcnew String(Original.cAlternateFileName);
        this->cFileName = gcnew String(Original.cFileName);
        this->dwFileAttributes = (FileAttributes) Original.dwFileAttributes;
        this->dwReserved0 = (ReparsePointTags) Original.dwReserved0;
        this->dwReserved1 = Original.dwReserved1;
        this->ftCreationTime = FileTimeToDateTime(Original.ftCreationTime);
        this->ftLastAccessTime = FileTimeToDateTime(Original.ftLastAccessTime);
        this->ftLastAccessTime = FileTimeToDateTime(Original.ftLastWriteTime);
        this->nFileSizeLow = Original.nFileSizeLow;
        this->nFileSizeHigh = Original.nFileSizeHigh;
    }
    void FindData::Populate(WIN32_FIND_DATA &target){
        for(int i = 0; i < this->cFileName->Length; i++) target.cFileName[i] = this->cFileName[i];
        for(int i = this->cFileName->Length; i < MaxPath; i++) target.cFileName[i] = 0;
        for(int i = 0; i < this->cAlternateFileName->Length; i++) target.cAlternateFileName[i] = this->cAlternateFileName[i];
        for(int i = this->cAlternateFileName->Length; i < 14; i++) target.cAlternateFileName[i] = 0;
        target.dwFileAttributes = (DWORD) this->dwFileAttributes;
        target.dwReserved0 = (DWORD) this->dwReserved0;
        target.dwReserved1 = this->dwReserved1;
        target.ftCreationTime = DateTimeToFileTime(this->ftCreationTime);
        target.ftLastAccessTime = DateTimeToFileTime(this->ftLastAccessTime);
        target.ftLastWriteTime = DateTimeToFileTime(this->ftLastAccessTime);
        target.nFileSizeLow = this->nFileSizeLow;
        target.nFileSizeHigh = this->nFileSizeHigh;
    }

    WIN32_FIND_DATA FindData::ToFindData(){
        WIN32_FIND_DATA ret;
        Populate(ret);
        return ret;
    }
    inline FileAttributes FindData::Attributes::get(){ return this->dwFileAttributes; }
    void FindData::Attributes::set(FileAttributes value){ this->dwFileAttributes = value; }
    inline DateTime FindData::CreationTime::get(){ return this->ftCreationTime; }
    inline void FindData::CreationTime::set(DateTime value){ this->ftCreationTime = value; }
    inline DateTime FindData::AccessTime::get(){ return this->ftLastAccessTime; }
    inline void FindData::AccessTime::set(DateTime value){ this->ftLastAccessTime = value; }
    inline DateTime FindData::WriteTime::get(){ return this->ftLastWriteTime; }
    inline void FindData::WriteTime::set(DateTime value){this->ftLastWriteTime = value; }
    inline QWORD FindData::FileSize::get(){ return (QWORD)this->nFileSizeHigh <<32 | (QWORD)this->nFileSizeLow; }
    void FindData::FileSize::set(QWORD value){
        this->nFileSizeHigh = Numbers::High(value);
        this->nFileSizeLow = Numbers::Low(value);
    }
    inline ReparsePointTags FindData::ReparsePointTag::get(){ return this->dwReserved0;}
    inline void FindData::ReparsePointTag::set(ReparsePointTags value){ this->dwReserved0 = value;}
    [EditorBrowsableAttribute(EditorBrowsableState::Never)]
    inline DWORD FindData::Reserved1::get(){ return this->dwReserved1;}
    inline void FindData::Reserved1::set(DWORD value){ this->dwReserved1 = value;}
    inline String^ FindData::FileName::get(){ return this->cFileName;}
    void FindData::FileName::set(String^ value){
        if(value->Length > MaxPath) throw gcnew ArgumentException(Exceptions::NameTooLongFormat(MaxPath));
        this->cFileName = value;
    }
    inline String^ FindData::AlternateFileName::get(){ return this->cAlternateFileName; }
    void FindData::AlternateFileName::set(String^ value){
        if(value->Length > 14) throw gcnew ArgumentException(Exceptions::NameTooLongFormat(14));
        this->cFileName = value;
    }
#pragma endregion
    //OperationEventArgs
#pragma region "OperationEventArgs"
    OperationEventArgs::OperationEventArgs(System::String ^remoteDir, Tools::TotalCommanderT::OperationKind kind, Tools::TotalCommanderT::OperationStatus status){
        if(remoteDir == nullptr) throw gcnew ArgumentNullException("remoteDir");
        this->remoteDir = remoteDir;
        this->kind = kind;
        this->status = status;
    }
    inline String^ OperationEventArgs::RemoteDir::get(){return this->remoteDir;}
    inline OperationKind OperationEventArgs::Kind::get(){return this->kind;}
    inline OperationStatus OperationEventArgs::Status::get(){return this->status;}
#pragma endregion
}}