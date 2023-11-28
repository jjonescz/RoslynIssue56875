```ps1
dotnet run -f net8.0 --runtimes net8.0 net481
```

```
BenchmarkDotNet v0.13.10, Windows 11 (10.0.22631.2715/23H2/2023Update/SunValley3) (Hyper-V)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-DGVKHI : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-QVBRTI : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256
```

| Method | Runtime              | Mean      | Ratio | Code Size |
|------- |--------------------- |----------:|------:|----------:|
| Before | .NET 8.0             | 0.3349 ns |  1.00 |      38 B |
| After  | .NET 8.0             | 0.1841 ns |  0.55 |      36 B |
| Before | .NET Framework 4.8.1 | 1.3028 ns |  3.88 |     114 B |
| After  | .NET Framework 4.8.1 | 0.4789 ns |  1.43 |      55 B |

## .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
```assembly
; RoslynIssue56875.Benchmarks.Before(System.Nullable`1<System.DateTime>)
;         public bool Before(DateTime? d) => d == DateTime.MaxValue;
;                                            ^^^^^^^^^^^^^^^^^^^^^^
       movzx     eax,byte ptr [rdx]
       mov       rcx,[rdx+8]
       test      al,al
       je        short M00_L00
       mov       rax,2BCA2875F4373FFF
       xor       rax,rcx
       shl       rax,2
       sete      al
       movzx     eax,al
       ret
M00_L00:
       xor       eax,eax
       ret
; Total bytes of code 38
```

## .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
```assembly
; RoslynIssue56875.Benchmarks.After(System.Nullable`1<System.DateTime>)
;         public bool After(DateTime? d) => d.HasValue && d.GetValueOrDefault() == DateTime.MaxValue;
;                                           ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
       cmp       byte ptr [rdx],0
       je        short M00_L00
       mov       rax,[rdx+8]
       mov       rcx,2BCA2875F4373FFF
       xor       rax,rcx
       shl       rax,2
       sete      al
       movzx     eax,al
       ret
M00_L00:
       xor       eax,eax
       ret
; Total bytes of code 36
```

## .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256
```assembly
; RoslynIssue56875.Benchmarks.Before(System.Nullable`1<System.DateTime>)
       sub       rsp,18
       vzeroupper
       xor       eax,eax
       mov       [rsp+8],rax
       mov       [rsp+10],rax
;         public bool Before(DateTime? d) => d == DateTime.MaxValue;
;                                            ^^^^^^^^^^^^^^^^^^^^^^
       vmovdqu   xmm0,xmmword ptr [rdx]
       vmovdqu   xmmword ptr [rsp+8],xmm0
       mov       rax,1BD1EDA1160
       mov       rax,[rax]
       mov       rdx,[rax+8]
       cmp       byte ptr [rsp+8],0
       jne       short M00_L00
       xor       eax,eax
       add       rsp,18
       ret
M00_L00:
       cmp       byte ptr [rsp+8],0
       jne       short M00_L01
       mov       eax,1
       add       rsp,18
       ret
M00_L01:
       mov       rax,[rsp+10]
       mov       rcx,3FFFFFFFFFFFFFFF
       and       rax,rcx
       and       rdx,rcx
       cmp       rax,rdx
       sete      al
       movzx     eax,al
       add       rsp,18
       ret
; Total bytes of code 114
```

## .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256
```assembly
; RoslynIssue56875.Benchmarks.After(System.Nullable`1<System.DateTime>)
       cmp       byte ptr [rdx],0
       je        short M00_L00
       mov       rax,[rdx+8]
       mov       rdx,1C2E9031160
       mov       rdx,[rdx]
       mov       rdx,[rdx+8]
       mov       rcx,3FFFFFFFFFFFFFFF
       and       rax,rcx
       and       rdx,rcx
       cmp       rax,rdx
       sete      al
       movzx     eax,al
       ret
;         public bool After(DateTime? d) => d.HasValue && d.GetValueOrDefault() == DateTime.MaxValue;
;                                           ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
M00_L00:
       xor       eax,eax
       ret
; Total bytes of code 55
```
