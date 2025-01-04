namespace Dawn.DarkCrusade.InteractLuaVM;

internal static class Patterns
{
    // -------------------------------------------------------------------------
    // Entire function (Disassembly)
    
    // 0076aa40  void* sub_76aa40()
    //
    // 0076aa40  6aff               push    0xffffffff {var_4_1}  {0xffffffff}
    // 0076aa42  686b3c8200         push    0x823c6b {var_8}
    // 0076aa47  64a100000000       mov     eax, dword [fs:0x0]
    // 0076aa4d  50                 push    eax {ExceptionList}
    // 0076aa4e  64892500000000     mov     dword [fs:0x0], esp {ExceptionList}
    // 0076aa55  51                 push    ecx {var_10}
    // 0076aa56  6a0c               push    0xc {var_14}
    // 0076aa58  e8b54d0600         call    operator new
    // 0076aa5d  83c404             add     esp, 0x4
    // 0076aa60  890424             mov     dword [esp {var_10_1}], eax
    // 0076aa63  85c0               test    eax, eax
    // 0076aa65  c744240c00000000   mov     dword [esp+0xc {var_4}], 0x0
    // 0076aa6d  741b               je      0x76aa8a
    //
    // 0076aa6f  8bc8               mov     ecx, eax
    // 0076aa71  e8eafeffff         call    sub_76a960
    // 0076aa76  a3f838a100         mov     dword [data_a138f8], eax
    // 0076aa7b  8b4c2404           mov     ecx, dword [esp+0x4 {ExceptionList}]
    // 0076aa7f  64890d00000000     mov     dword [fs:0x0], ecx
    // 0076aa86  83c410             add     esp, 0x10
    // 0076aa89  c3                 retn     {__return_addr}
    //
    // 0076aa8a  8b4c2404           mov     ecx, dword [esp+0x4 {ExceptionList}]
    // 0076aa8e  33c0               xor     eax, eax  {0x0}
    // 0076aa90  a3f838a100         mov     dword [data_a138f8], eax  {0x0}
    // 0076aa95  64890d00000000     mov     dword [fs:0x0], ecx
    // 0076aa9c  83c410             add     esp, 0x10
    // 0076aa9f  c3                 retn     {__return_addr}
    
    // -------------------------------------------------------------------------
    // Pseudo C
    // 0076aa40  void* sub_76aa40()
    // 0076aa40  {
    // 0076aa40      int32_t var_4_1 = 0xffffffff;
    // 0076aa42      int32_t var_8 = 0x823c6b;
    // 0076aa4d      TEB* fsbase;
    // 0076aa4d      struct _EXCEPTION_REGISTRATION_RECORD* ExceptionList = fsbase->NtTib.ExceptionList;
    // 0076aa4e      fsbase->NtTib.ExceptionList = &ExceptionList;
    // 0076aa55      int32_t ecx;
    // 0076aa55      int32_t var_10 = ecx;
    // 0076aa58      void* eax_1 = operator new(0xc);
    // 0076aa60      void* var_10_1 = eax_1;
    // 0076aa65      int32_t var_4 = 0;
    // 0076aa65      
    // 0076aa6d      if (eax_1 != 0)
    // 0076aa6d      {
    // 0076aa71          int32_t* eax_2 = sub_76a960(eax_1);
    // 0076aa76          data_a138f8 = eax_2;
    // 0076aa7f          fsbase->NtTib.ExceptionList = ExceptionList;
    // 0076aa89          return eax_2;
    // 0076aa6d      }
    // 0076aa6d      
    // 0076aa8a      struct _EXCEPTION_REGISTRATION_RECORD* ExceptionList_1 = ExceptionList;
    // 0076aa90      data_a138f8 = 0;
    // 0076aa95      fsbase->NtTib.ExceptionList = ExceptionList_1;
    // 0076aa9f      return eax_1;
    // 0076aa40  }
    
    // From Binary Ninja (Disassembly) - sub_76aa40()
    // ----------------------------------------------
    // 0076aa6d  741b               je      0x76aa8a
    //
    // 0076aa6f  8bc8               mov     ecx, eax
    // 0076aa71  e8eafeffff         call    sub_76a960
    // 0076aa76  a3f838a100         mov     dword [data_a138f8], eax
    // 0076aa7b  8b4c2404           mov     ecx, dword [esp+0x4 {ExceptionList}]
    // 0076aa7f  64890d00000000     mov     dword [fs:0x0], ecx
    // 0076aa86  83c410             add     esp, 0x10
    // 0076aa89  c3                 retn     {__return_addr}

    // data_a138f8 is the value that the LUA VM's LuaConfig* is stored as a static variable.
    // It is used by LuaConfig::GetState to return a lua_State*
    
    // Function Signature:
    // struct lua_State* (__thiscall* const LuaConfig:LuaConfig::GetState(LuaConfig* this)
    
    // TL;DR : We find in a piece of code, a pointer to a static pointer which points to the LuaConfig.
    // eg. LuaConfig**

    public const string LUA_CONFIG_STATIC_POINTER_PATTERN = "A3 ?? ?? ?? ?? 8B 4C 24 04 64 89 0D ?? ?? ?? ?? 83 C4 ?? C3 8B 4C 24 04 33 C0 A3 ?? ?? ?? ?? 64 89 0D ?? ?? ?? ?? 83 C4 ?? C3 8B 0D ?? ?? ?? ?? 56";

}